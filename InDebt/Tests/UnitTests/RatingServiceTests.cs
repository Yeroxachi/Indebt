using Application.Services;
using AutoMapper;
using Domain.Enums;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class RatingServiceTests
{
    private readonly IMapper _mapper;

    public RatingServiceTests()
    {
        _mapper = TestHelper.GetMapper();
    }
    
    [Fact]
    public async void GetRatingOfUserById_GivenUserThatExistInSystem_ResultShouldBeOkResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(RatingServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithTransactions()
            .GetContext();
        var sut = new RatingService(context,_mapper,TestHelper.CreateAccessor());
        var userId =Guid.Parse(TestDataConstants.TestEntity2Id);
        // Act
        var response = await sut.GetRatingOfUserByIdAsync(userId);
        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetRatingOfUserById_GivenUserThatNotExistInSystem_ResultShouldBeNotFoundResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        using var context = new TestDbContextBuilder(nameof(RatingServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithTransactions()
            .GetContext();
        var sut = new RatingService(context,_mapper,TestHelper.CreateAccessor());
        var userId =Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        // Act
        var response = await sut.GetRatingOfUserByIdAsync(userId);
        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetRatingOfUserById_UserHasntValidAccessToken_ResultShouldBeUnAuthorizeResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.UnAuthorize;
        using var context = new TestDbContextBuilder(nameof(RatingServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithTransactions()
            .GetContext();
        var sut = new RatingService(context,_mapper,TestHelper.GetAccessor(TestDataConstants.IncorrectEntityUsername));
        var userId =Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        // Act
        var response = await sut.GetRatingOfUserByIdAsync(userId);
        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetRatingOfUserByGroupId_GivenGroupIdThatExistInSystem_ResultShouldBeOkResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(RatingServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithTransactions()
            .GetContext();
        var sut = new RatingService(context,_mapper,TestHelper.CreateAccessor());
        var groupId =Guid.Parse(TestDataConstants.TestEntity1Id);
        // Act
        var response = await sut.GetRatingOfUsersByGroupIdAsync(groupId);
        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetRatingOfUserByGroupId_GivenGroupIdThatNotExistInSystem_ResultShouldBeForbiddenResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        using var context = new TestDbContextBuilder(nameof(RatingServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithTransactions()
            .GetContext();
        var sut = new RatingService(context,_mapper,TestHelper.CreateAccessor());
        var groupId =Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        // Act
        var response = await sut.GetRatingOfUsersByGroupIdAsync(groupId);
        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetRatingOfUserByGroupId_UserHasntValidAccessToken_ResultShouldBeUnAuthorizeResponseCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.UnAuthorize;
        using var context = new TestDbContextBuilder(nameof(RatingServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithTransactions()
            .GetContext();
        var sut = new RatingService(context,_mapper,TestHelper.GetAccessor(TestDataConstants.IncorrectEntityUsername));
        var groupId =Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        // Act
        var response = await sut.GetRatingOfUsersByGroupIdAsync(groupId);
        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}