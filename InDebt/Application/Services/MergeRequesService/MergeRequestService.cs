using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class MergeRequestService : BaseService, IMergeRequestService
{
    public MergeRequestService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
    }

    public async Task<BaseResponse> GetAllAsync()
    {
        if (UserGroups is null)
        {
            return UnAuthorize();
        }
        var merges = await Context.MergeRequestGroups
            .Where(x => UserGroups.Contains(x.GroupId))
            .Select(x => x.MergeRequest).Distinct()
            .Include(x=>x.Groups)
            .ToListAsync();
        var response = Mapper.Map<List<MergeRequest>, List<MergeRequestResponse>>(merges);
        return Ok(response);
    }

    public async Task<BaseResponse> GetByIdAsync(Guid id)
    {
        var merge = await Context.Merges
            .Include(x => x.Groups)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (merge is null)
        {
            return NotFound();
        }

        if (!merge.Groups.Any(x => UserGroups.Contains(x.GroupId)))
        {
            return Forbid();
        }
        
        var response = Mapper.Map<MergeRequest, MergeRequestResponse>(merge);
        return Ok(response);
    }

    public async Task<BaseResponse> UpdateAsync(MergeRequestDto dto, Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var merge = await Context.Merges.FirstOrDefaultAsync(x => x.Id == id);
        if (merge is null)
        {
            return NotFound();
        }

        if (merge.InitiatorId != UserId.Value)
        {
            return Forbid();
        }

        if (!dto.GroupsId.All(x => UserGroups.Contains(x)))
        {
            return Forbid();
        }

        merge.NewGroupName = dto.NewName;
        merge.NewDescription = dto.Description;
        merge.Groups = dto.GroupsId.Select(x => new MergeRequestGroup
        {
            GroupId = x
        }).ToList();
        
        await Context.SaveChangesAsync();
        var response = Mapper.Map<MergeRequest, MergeRequestResponse>(merge);
        return Ok(response);
    }

    public async Task<BaseResponse> CreateAsync(MergeRequestDto dto)
    {
        if (UserGroups is null || UserId is null)
        {
            return UnAuthorize();
        }

        if (dto.GroupsId.Any(x => UserGroups.All(y => y != x)))
        {
            return Forbid();
        }

        var users = await Context.UserGroups.Where(x => dto.GroupsId.Contains(x.GroupId)).Select(x => x.UserId).Where(x=> x != UserId.Value).Distinct().ToArrayAsync();
        var newMerge = new MergeRequest
        {
            Id = Guid.NewGuid(),
            InitiatorId = UserId.Value,
            NewGroupName = dto.NewName,
            NewDescription = dto.Description,
            Groups = dto.GroupsId.Select(x => new MergeRequestGroup
            {
                GroupId = x
            }).ToList(),
            Approvals = users.Select(x => new MergeRequestApproval
            {
                UserId = x
            }).ToList()
        };

        await Context.Merges.AddAsync(newMerge);
        await Context.SaveChangesAsync();
        var response = Mapper.Map<MergeRequestResponse>(newMerge);
        return Created(response);
    }

    public async Task<BaseResponse> GetAllReceivedAsync()
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var approvals = await Context.Approvals
            .Where(x => x.UserId == UserId.Value && !x.Approved)
            .ToListAsync();
        var response = Mapper.Map<IReadOnlyCollection<MergeRequestApprovalResponse>>(approvals);
        return Ok(response);
    }

    public async Task<BaseResponse> AcceptAsync(Guid approvalId, bool accepted)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var approval = await Context.Approvals.FirstOrDefaultAsync(x => x.Id == approvalId && !x.Approved);
        if (approval is null)
        {
            return NotFound();
        }

        if (approval.UserId != UserId.Value)
        {
            return Forbid();
        }

        var mergeRequest = await Context.Merges
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == approval.MergeRequestId);
        if (mergeRequest.MergeStatus != MergeStatus.Pending)
        {
            return BadRequest("Incorrect merge request status.");
        }

        if (accepted)
        {
            approval.Approved = true;
            if (mergeRequest.Approvals.All(x => x.Approved))
            {
                mergeRequest.MergeStatus = MergeStatus.ReadyToMerge;
            }
        }
        else
        {
            approval.Approved = false;
            mergeRequest.MergeStatus = MergeStatus.Declined;
        }

        await Context.SaveChangesAsync();
        var response = Mapper.Map<MergeRequestApprovalResponse>(approval);
        return Ok(response);
    }

    public async Task<BaseResponse> CompleteMerge(Guid mergeId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var mergeRequest = await Context.Merges
            .Include(x=> x.Groups)
            .ThenInclude(x=> x.Group)
            .ThenInclude(x=> x.Users)
            .FirstOrDefaultAsync(x => x.Id == mergeId);
        if (mergeRequest is null)
        {
            return NotFound();
        }

        if (UserId.Value != mergeRequest.InitiatorId)
        {
            return Forbid();
        }

        if (mergeRequest.MergeStatus != MergeStatus.ReadyToMerge)
        {
            return BadRequest("There are some pending approvals.");
        }

        await MergeGroupIntoOne(mergeRequest);
        return Ok();
    }

    private async Task MergeGroupIntoOne(MergeRequest mergeRequest)
    {
        var groups = mergeRequest.Groups.Select(x => x.Group).ToHashSet();
        var users = groups.SelectMany(x => x.Users).Select(x=> x.UserId).ToHashSet();
        var groupsId = groups.Select(x => x.Id).ToArray();
        var debts = await Context.Debts.Where(d => groupsId.Contains(d.GroupId)).ToListAsync();

        var newGroup = new Group
        {
            Name = mergeRequest.NewGroupName,
            Description = mergeRequest.NewDescription,
            Users = users.Select(x=> new UserGroup
            {
                UserId = x
            }).ToList()
        };

        var connectedMergeRequests = await Context.MergeRequestGroups
            .Where(x => groupsId.Contains(x.GroupId) && x.MergeRequestId != mergeRequest.Id)
            .Select(x => x.MergeRequest)
            .ToListAsync();
        foreach (var connectedMergeRequest in connectedMergeRequests)
        {
            var approvalsOfConnectedMergeRequest =
                await Context.Approvals.Where(x => x.MergeRequestId == connectedMergeRequest.Id).ToArrayAsync();
            Context.Approvals.RemoveRange(approvalsOfConnectedMergeRequest);
        }
        mergeRequest.MergeStatus = MergeStatus.Completed;
        await Context.Groups.AddAsync(newGroup);
        foreach (var debt in debts)
        {
            debt.Group = newGroup;
            debt.GroupId = newGroup.Id;
        }
        Context.Groups.RemoveRange(groups);
        await Context.SaveChangesAsync();
    }
}