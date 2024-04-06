using Application.Mappers;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class BalanceServiceTests
{
    
    private readonly IMapper _mapper;
    private readonly IExchangeRateService _exchangeRateService;

    public BalanceServiceTests()
    {
        _mapper = TestHelper.GetMapper(new UserMapperProfile());
        _exchangeRateService = TestHelper.ExchangeRateServiceMock();
    }
    
    [Fact]
    public async void GetTotal_ReturnsTotalBalanceOfUserInSystem_WhenValidUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(BalanceServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new BalanceService(context, _mapper, accessor, _exchangeRateService);

        // Act
        var response = await sut.GetTotal(null);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetTotalIncome_ReturnsTotalIncomeBalanceOfUserInSystem_WhenValidUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(BalanceServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new BalanceService(context, _mapper, accessor, _exchangeRateService);

        // Act
        var response = await sut.GetTotalIncome(null);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetTotalOutcome_ReturnsTotalOutcomeBalanceOfUserInSystem_WhenValidUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(BalanceServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new BalanceService(context, _mapper, accessor, _exchangeRateService);

        // Act
        var response = await sut.GetTotalOutcome(null);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Theory]
    [InlineData(TestDataConstants.TestEntity1Id, ResponseCode.Ok)]
    [InlineData(TestDataConstants.TestEntity4Id, ResponseCode.Ok)]
    public async void GetTotalInGroup_ReturnsTotalBalanceOfUserInGroup_WhenValidUsernameProvided(string groupId, ResponseCode expectedCode)
    {
        // Arrange
        await using var context = new TestDbContextBuilder(nameof(BalanceServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new BalanceService(context, _mapper, accessor, _exchangeRateService);

        // Act
        var response = await sut.GetTotal(Guid.Parse(groupId));

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Theory]
    [InlineData(TestDataConstants.TestEntity1Id, ResponseCode.Ok)]
    [InlineData(TestDataConstants.TestEntity4Id, ResponseCode.Ok)]
    public async void GetTotalIncomeInGroup_ReturnsTotalIncomeBalanceOfUserInGroup_WhenValidUsernameProvided(string groupId, ResponseCode expectedCode)
    {
        // Arrange
        await using var context = new TestDbContextBuilder(nameof(BalanceServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new BalanceService(context, _mapper, accessor, _exchangeRateService);

        // Act
        var response = await sut.GetTotalIncome(Guid.Parse(groupId));

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Theory]
    [InlineData(TestDataConstants.TestEntity1Id, ResponseCode.Ok)]
    [InlineData(TestDataConstants.TestEntity4Id, ResponseCode.Ok)]
    public async void GetTotalOutcomeInGroup_ReturnsTotalOutcomeBalanceOfUserInGroup_WhenValidUsernameProvided(string groupId, ResponseCode expectedCode)
    {
        // Arrange
        await using var context = new TestDbContextBuilder(nameof(BalanceServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new BalanceService(context, _mapper, accessor, _exchangeRateService);

        // Act
        var response = await sut.GetTotalOutcome(Guid.Parse(groupId));

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
}