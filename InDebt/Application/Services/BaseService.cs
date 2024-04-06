using Application.Context;
using Application.Helpers;
using Application.Responses;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public abstract class BaseService
{
    protected readonly IInDebtContext Context;
    protected readonly IMapper Mapper;
    private readonly IHttpContextAccessor _accessor;

    protected BaseService(IInDebtContext context, IMapper mapper, IHttpContextAccessor accessor)
    {
        Context = context;
        Mapper = mapper;
        _accessor = accessor;
    }

    protected string Username => GetClaimValue(Constants.UsernameClaimType);
    protected Guid? DefaultCurrencyId  {
        get
        {
            var defaultCurrency = Context.Currencies.FirstOrDefault(x => x.CurrencyCode == "USD");

            return defaultCurrency?.Id;
        }
    }
    protected Guid? UserId
    {
        get
        {
            var userIdValue = GetClaimValue(Constants.UserIdClaimName);
            if (userIdValue is null)
            {
                return null;
            }

            return Guid.Parse(userIdValue);
        }
    }

    protected Guid[] UserGroups
    {
        get
        {
            var groupsValue = GetClaimValue(Constants.UserGroupsIdsClaimName); 
            if (groupsValue.IsNullOrEmpty()) 
            {
                return null;
            }

            return groupsValue.Split(",").Select(Guid.Parse).ToArray();
        }
    }

    private string GetClaimValue(string claimType) => _accessor.HttpContext.User?.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
    protected BaseResponse Ok()
    {
        return new BaseResponse();
    }

    protected BaseResponse<TE> Ok<TE>(TE data) where TE : class
    {
        return new BaseResponse<TE>(data);
    }

    protected BaseResponse Created()
    {
        return new BaseResponse(ResponseCode.Created);
    }

    protected BaseResponse<TE> Created<TE>(TE data) where TE : class
    {
        return new BaseResponse<TE>(data, ResponseCode.Created);
    }

    protected BaseResponse UnAuthorize()
    {
        return new BaseResponse(ResponseCode.UnAuthorize);
    }

    protected BaseResponse<TE> UnAuthorize<TE>() where TE : class
    {
        return new BaseResponse<TE>(code: ResponseCode.UnAuthorize);
    }

    protected BaseResponse<TE> Forbid<TE>() where TE : class
    {
        return new BaseResponse<TE>(code: ResponseCode.Forbidden);
    }

    protected BaseResponse Forbid()
    {
        return new BaseResponse(ResponseCode.Forbidden);
    }

    protected BaseResponse NotFound()
    {
        return new BaseResponse(ResponseCode.NotFound);
    }

    protected BaseResponse<TE> NotFound<TE>() where TE : class
    {
        return new BaseResponse<TE>(code: ResponseCode.NotFound);
    }

    protected BaseResponse BadRequest(string message)
    {
        return new BaseResponse(ResponseCode.BadRequest, errorMessage: message);
    }

    protected BaseResponse<TE> BadRequest<TE>(string message) where TE : class
    {
        return new BaseResponse<TE>(code: ResponseCode.BadRequest, errorMessage: message);
    }
}