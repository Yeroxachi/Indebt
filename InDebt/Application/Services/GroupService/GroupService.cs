using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class GroupService : BaseService, IGroupService
{
    public GroupService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
    }

    public async Task<BaseResponse> GetAllAsync(PaginationDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }

        var totalCount = await Context.UserGroups
            .Where(x => x.UserId == UserId)
            .CountAsync();
        var groups = await Context.UserGroups
            .Where(x => x.UserId == UserId)
            .Select(x => x.Group)
            .Skip(dto.SkipCount())
            .Take(dto.PageSize)
            .AsNoTracking()
            .ProjectTo<GroupResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();
        var response = new PaginationResponse<GroupResponse>(totalCount, groups);
        return Ok(response);
    }

    public async Task<BaseResponse> GetByIdAsync(Guid groupId)
    {
        var group = await Context.Groups.FirstOrDefaultAsync(u => u.Id == groupId);
        if (group is null)
        {
            return NotFound();
        }

        var response = Mapper.Map<GroupResponse>(group);
        return Ok(response);
    }

    public async Task<BaseResponse> CreateAsync(GroupDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        
        var newGroup = new Group
        {
            Description = dto.Description,
            Name = dto.Name,
            Users = new List<UserGroup>
            {
                new()
                {
                    UserId = UserId.Value
                }
            }
        };
        
        await Context.Groups.AddAsync(newGroup);
        await Context.SaveChangesAsync();
        var response = Mapper.Map<GroupResponse>(newGroup);
        return Created(response);
    }

    public async Task<BaseResponse> UpdateAsync(Guid groupId, GroupDto dto)
    {
        var group = await Context.Groups.FirstOrDefaultAsync(u => u.Id == groupId);
        if (group is null)
        {
            return NotFound();
        }
        
        if (UserGroups is null || !UserGroups.Contains(groupId))
        {
            return Forbid();
        }
        
        group = Mapper.Map(dto, group);
        Context.Groups.Update(group);
        await Context.SaveChangesAsync();
        var response = Mapper.Map<GroupResponse>(group);
        return Ok(response);
    }

    public async Task<BaseResponse> DeleteAsync(Guid groupId)
    {
        var group = await Context.Groups.FirstOrDefaultAsync(u => u.Id == groupId);
        if (group is null)
        {
            return NotFound();
        }
        
        if (UserGroups is null || !UserGroups.Contains(groupId))
        {
            return Forbid();
        }

        Context.Groups.Remove(group);
        await Context.SaveChangesAsync();
        return Ok();
    }
}