using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class GroupInviteService : BaseService, IGroupInviteService
{
    public GroupInviteService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
    }

    public async Task<BaseResponse> SendAsync(GroupInviteDto dto)
    {
        if (UserGroups is null || UserId is null)
        {
            return UnAuthorize();
        }
        
        var group = await Context.Groups
            .Include(e=> e.Users)
            .FirstOrDefaultAsync(u => u.Id == dto.GroupId);
        if (group is null)
        {
            return NotFound();
        }

        if (!UserGroups.Contains(group.Id))
        {
            return Forbid();
        }
        
        var invited = await Context.Users.FirstOrDefaultAsync(x => x.Id == dto.InvitedId);
        if (invited is null)
        {
            return NotFound();
        }
        
        if (group.Users.Any(x => x.UserId == invited.Id))
        {
            return BadRequest($"The user {invited.Username} is already in this group");
        }

        var invite = await Context.GroupInvites.FirstOrDefaultAsync(x => x.InvitedId == invited.Id && x.GroupId == group.Id);
        if (invite is not null)
        {
            return BadRequest($"The user {invited.Username} is already has invite to group {group.Id}");            
        }
        var newInvite = new GroupInvite
        {
            GroupId = dto.GroupId,
            InvitedId = invited.Id,
            InviteStatus = InviteStatus.Invited,
            InviterId = UserId.Value
        };

        Context.GroupInvites.Add(newInvite);
        await Context.SaveChangesAsync();
        var response = Mapper.Map<GroupInviteResponse>(newInvite);
        return Created(response);
    }

    public async Task<BaseResponse> AcceptAsync(Guid inviteId)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var invite = await Context.GroupInvites
            .FirstOrDefaultAsync(x => x.Id == inviteId && x.InviteStatus == InviteStatus.Invited);
        if (invite is null)
        {
            return NotFound();
        }

        if (invite.InvitedId != UserId.Value)
        {
            return Forbid();
        }
        
        invite.InviteStatus = InviteStatus.Accepted;
        var newUserGroup = new UserGroup
        {
            UserId = UserId.Value,
            GroupId = invite.GroupId
        };
        
        await Context.UserGroups.AddAsync(newUserGroup);
        await Context.SaveChangesAsync();
        return Ok(invite);
    }

    public async Task<BaseResponse> GetAllReceivedAsync()
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var invites = await Context.GroupInvites
            .Where(x => x.InvitedId == UserId.Value && x.InviteStatus == InviteStatus.Invited).ToListAsync();
        
        var response = Mapper.Map<IReadOnlyCollection<GroupInviteResponse>>(invites);
        return Ok(response);
    }
}
