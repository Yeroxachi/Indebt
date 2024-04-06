using Application.DTOs;
using Application.Helpers;
using Application.Mappers;
using Application.Responses;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace Tests.Helpers;

internal static class TestHelper
{
    public static Mapper GetMappers()
    {
        var mappers = new List<Profile>
        {
            new UserMapperProfile(),
            new GroupInviteMapperProfile(),
            new GroupMapperProfile(),
            new MergeRequestMapperProfile(),
            new GroupMergeConfirmationMapperProfile(),
            new NotificationMapperProfile()
        };
        var configuration = new MapperConfiguration(m => m.AddProfiles(mappers));
        return new Mapper(configuration);
    }

    public static Mapper GetMapper(params Profile[] profile)
    {
        var configuration = new MapperConfiguration(m => m.AddProfiles(profile));
        return new Mapper(configuration);
    }

    public static IOptions<AuthOptions> CreateAuthOptions()
    {
        var authOptions = new AuthOptions
        {
            Audience = TestDataConstants.AuthAudience,
            Issuer = TestDataConstants.AuthIssuer,
            Key = TestDataConstants.AuthKey,
            AccessTokenLifeTime = 360,
            RefreshTokenLifeTime = 86000*2
        };
        var options = Options.Create(authOptions);
        return options;
    }

    public static IHttpContextAccessor GetAccessor(string username)
    {
        var mock = new Mock<IHttpContextAccessor>();
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, username),
        };
        mock.Setup(x => x.HttpContext.User.Claims).Returns(claims);
        return mock.Object;
    }
        
    public static IOptions<EmailServiceOptions> CreateEmailServiceOptions()
    {
        var emailServiceOptions = new EmailServiceOptions
        {
            SenderName = TestDataConstants.SenderName,
            SenderAddress = TestDataConstants.SenderAddress,
            Host = TestDataConstants.Host,
            Password = TestDataConstants.Password,
            Port = TestDataConstants.Port,
            SslState = TestDataConstants.SslState
        };
        var options = Options.Create(emailServiceOptions);
        return options;
    }
        
    public static IOptions<EnvironmentOptions> CreateEnvironmentOptions()
    {
        var environmentOptions = new EnvironmentOptions
        {
            HostUrl = TestDataConstants.Host
        };
        var options = Options.Create(environmentOptions);
        return options;
    }
        
    public static IHttpContextAccessor CreateAccessor()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        var usernameClaim = new Claim(ClaimTypes.Name, TestDataConstants.LoggedUsername);
        var userIdClaim = new Claim(Constants.UserIdClaimName, TestDataConstants.TestEntity1Id);
        var userGroups = string.Join(',',TestDataConstants.TestEntity1Id, TestDataConstants.TestEntity2Id);
        var userGroupsClaim = new Claim(Constants.UserGroupsIdsClaimName, userGroups);
        var identity = new ClaimsIdentity(new[] { usernameClaim, userIdClaim, userGroupsClaim }); // this uses basic auth
        var principal = new ClaimsPrincipal(identity);
        accessor.Setup(e => e.HttpContext.User).Returns(principal);
        return accessor.Object;
    }

    public static IEmailService CreateEmailService()
    {
        var emailService = new Mock<IEmailService>();
        emailService.Setup(x => x.SendEmailAsync(new EmailDto()
                { ReceiverEmail = TestDataConstants.DefaultEmail, Subject = "", Message = "", ReceiverName = "" }))
            .Callback(() => { });
        return emailService.Object;
    }

    public static IExchangeRateService ExchangeRateServiceMock()
    {
        var mock = new Mock<IExchangeRateService>();
        mock.Setup(x => x.CalculateExchangeRateAsync(It.IsAny<ExchangeRateDto>()))
            .ReturnsAsync(
                new BaseResponse<ExchangeRateResponse>
                {
                    Code = ResponseCode.Ok,
                    Data = new ExchangeRateResponse
                    {
                        BaseCode = TestDataConstants.CurrencyCodeUsd,
                        TargetCode = TestDataConstants.CurrencyCodeKzt,
                        ConversionRate = TestDataConstants.ConversionRate,
                        ConversionResult = 500
                    }
                });
        return mock.Object;
    }

    public static PaginationDto GetPaginationDto()
    {
        return new PaginationDto
        {
            PageSize = 50,
            PageNumber = 1,
        };
    }
}