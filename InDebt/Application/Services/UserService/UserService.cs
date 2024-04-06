using Application.Context;
using Application.DTOs;
using Application.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : BaseService, IUserService
{
    public UserService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor) : base(context, mapper, accessor)
    {
    }
    
    public async Task<BaseResponse> GetAllAsync(PaginationDto paginationDto)
    {
        var users = await Context.Users
            .Skip(paginationDto.SkipCount())
            .Take(paginationDto.PageSize)
            .AsNoTracking()
            .ProjectTo<UserResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();
        var response = new PaginationResponse<UserResponse>(await Context.Users.CountAsync(), users);
        return Ok(response);
    }
    
    public async Task<BaseResponse> GetByIdAsync(Guid id)
    {
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return NotFound(); ;
        }

        var response = Mapper.Map<UserResponse>(user);
        return Ok(response);
    }
    
    public async Task<BaseResponse> UpdateAsync(UserDto dto)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == UserId.Value);
        if (user is null)
        {
            return NotFound<UserResponse>();
        }

        user = Mapper.Map(dto, user);
        Context.Users.Update(user);
        await Context.SaveChangesAsync();
        var response = Mapper.Map<UserResponse>(user);
        return Ok(response);
    }

    public async Task<BaseResponse> DeleteAsync(Guid id)
    {
        if (UserId is null)
        {
            return UnAuthorize();
        }
        var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        if (UserId.Value != id)
        {
            return Forbid();
        }

        Context.Users.Remove(user);
        await Context.SaveChangesAsync();
        return Ok();
    }
}