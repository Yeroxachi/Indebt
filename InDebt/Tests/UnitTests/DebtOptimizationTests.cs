using Application.DTOs;
using Application.Mappers;
using Application.Responses;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class DebtOptimizationTests
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _accessor;
    private readonly Mock<IExchangeRateService> _exchangeRateService;

    public DebtOptimizationTests()
    {
        _mapper = TestHelper.GetMapper(new OptimizationApprovalMapperProfile(), new OptimizationRequestMapperProfile());
        _accessor = TestHelper.CreateAccessor();
        _exchangeRateService = new Mock<IExchangeRateService>();
        _exchangeRateService.Setup(x => x.CalculateExchangeRateAsync(It.IsAny<ExchangeRateDto>())).ReturnsAsync(
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
    }

    [Fact]
    public async void CreateAsync_ReturnCreatedStatusCode_Provided_CorrectUsernameAndGroup()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var groupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        
        //Act
        var response = await sut.CreateAsync(groupId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void CreateAsync_ReturnCreatedStatusCode_Provided_CorrectUsernameAndGroupWhenUserHasNotPermission()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var groupId = Guid.Parse(TestDataConstants.TestEntity4Id);
        
        //Act
        var response = await sut.CreateAsync(groupId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void AcceptApproval_ReturnOkStatusCode_ProvidedCorrectUsernameAndApprovalIdThatUserHasPermission()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var approvalId = Guid.Parse(TestDataConstants.TestEntity1Id);
        
        //Act
        var response = await sut.AcceptApprovalAsync(approvalId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    [Fact]
    public async void AcceptApproval_ReturnForbidStatusCode_ProvidedCorrectUsernameAndApprovalIdThatUserHasntPermission()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var approvalId = Guid.Parse(TestDataConstants.TestEntity2Id);
        
        //Act
        var response = await sut.AcceptApprovalAsync(approvalId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void AcceptApproval_ReturnNotFoundStatusCode_ProvidedCorrectUsernameAndInvalidApprovalId()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var approvalId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        
        //Act
        var response = await sut.AcceptApprovalAsync(approvalId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetAll_ReturnOkStatusCode_ProvidedCorrectUsername()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);

        //Act
        var response = await sut.GetAllAsync();
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetAllReceivedApprovals_ReturnOkStatusCode_ProvidedCorrectUsername()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);

        //Act
        var response = await sut.GetAllReceivedApprovalsAsync();
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetById_ReturnOkStatusCode_ProvidedCorrectUsernameAndCorrectDebtOptimization()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var optimizationId = Guid.Parse(TestDataConstants.TestEntity1Id);

        //Act
        var response = await sut.GetByIdAsync(optimizationId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetById_ReturnNotFoundStatusCode_ProvidedCorrectUsernameAndCorrectDebtButWithoutPermissionOptimization()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var optimizationId = Guid.Parse(TestDataConstants.TestEntity3Id);

        //Act
        var response = await sut.GetByIdAsync(optimizationId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void Optimize_ReturnForbiddenStatusCode_ProvidedCorrectUsernameAndCorrectDebtButWithoutPermissionOptimization()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var optimizationId = Guid.Parse(TestDataConstants.TestEntity3Id);

        //Act
        var response = await sut.OptimizeAsync(optimizationId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void Optimize_ReturnOkStatusCode_ProvidedCorrectUsernameAndCorrectDebt()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var optimizationId = Guid.Parse(TestDataConstants.TestEntity1Id);

        //Act
        var response = await sut.OptimizeAsync(optimizationId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void Optimize_ReturnNotFoundStatusCode_ProvidedCorrectUsernameAndIncorrectDebt()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        await using var context = new TestDbContextBuilder(nameof(DebtOptimizationTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .WithDebts()
            .WithDebtOptimization()
            .WithDebtOptimizationApprovals()
            .GetContext();
        var sut = new DebtOptimizationService(context, _mapper, _accessor, _exchangeRateService.Object);
        var optimizationId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);

        //Act
        var response = await sut.OptimizeAsync(optimizationId);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
}