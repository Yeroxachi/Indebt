using Application.DTOs;
using Application.Mappers;
using Application.Responses;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Moq;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class TransactionManagementServiceTests
{
    private readonly Mapper _mapper;
    private readonly Mock<IExchangeRateService> _exchangeRate;

    public TransactionManagementServiceTests()
    {
        _mapper = TestHelper.GetMapper(new TransactionMapperProfile());
        _exchangeRate = new Mock<IExchangeRateService>();
    }

    [Theory]
    [InlineData(TestDataConstants.TestEntity1Id, TransactionType.Incoming)]
    [InlineData(TestDataConstants.TestEntity1Id, TransactionType.Outgoing)]
    [InlineData(TestDataConstants.TestEntity1Id, null)]
    public async void GetAll_ReturnsTransactionResponsesForNotNullDebtIds(string debtId, TransactionType? transactionType)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.GetAll(Guid.Parse(debtId), transactionType, TestHelper.GetPaginationDto());

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Theory]
    [InlineData(null, TransactionType.Incoming)]
    [InlineData(null, TransactionType.Outgoing)]
    [InlineData(null, null)]
    public async void GetAll_ReturnsTransactionResponsesForNullDebtIds (Guid? debtId, TransactionType? transactionType)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.GetAll(debtId, transactionType, TestHelper.GetPaginationDto());

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Fact]
    public async void GetById_ReturnsTransactionResponses_withCorrectOwner()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.GetById(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Fact]
    public async void GetById_ReturnsForbidden_NotOwnerOfTransaction()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity3Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.GetById(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Fact]
    public async void GetAllByDebtId_ReturnsTransactionResponses_withCorrectOwner()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var debtId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.GetById(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
        
        
    [Fact]
    public async void Create_ReturnsCreatedNotConfirmedTransaction_WhenValidDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        var debtId = Guid.Parse(TestDataConstants.TestEntity2Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);
        var dto = new TransactionDto
        {
            DebtId = debtId,
            Amount = TestDataConstants.TestTransactionAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeUsd
        };

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Create_ReturnsCreatedNotConfirmedTransaction_WithDifferentCurrencyProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        var debtId = Guid.Parse(TestDataConstants.TestEntity2Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        _exchangeRate
            .Setup(x => x.CalculateExchangeRateAsync(It.IsAny<ExchangeRateDto>()))
            .ReturnsAsync(new BaseResponse<ExchangeRateResponse>()
                { Data = new ExchangeRateResponse()
                {
                    BaseCode = TestDataConstants.CurrencyCodeUsd,
                    TargetCode = TestDataConstants.CurrencyCodeKzt,
                    ConversionRate = TestDataConstants.ConversionRate,
                    ConversionResult = 0.10M
                } });
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);
        var dto = new TransactionDto
        {
            DebtId = debtId,
            Amount = TestDataConstants.TestTransactionAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeKzt
        };

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.IncorrectTestEntity1Id, TestDataConstants.CurrencyCodeUsd)]
    [InlineData(TestDataConstants.TestEntity1Id, TestDataConstants.IncorrectEntityUsername)]
    public async void Create_ReturnsNotFoundStatusCode_WhenInValidDtoProvided(string inputDebtId, string currencyCode)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var debtId = Guid.Parse(inputDebtId);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);
        var dto = new TransactionDto
        {
            DebtId = debtId,
            Amount = TestDataConstants.TestTransactionAmount,
            CurrencyCode = currencyCode
        };

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Create_ReturnsForbiddenStatusCode_WhenInValidDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var debtId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);
        var dto = new TransactionDto
        {
            DebtId = debtId,
            Amount = TestDataConstants.TestTransactionAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeUsd
        };

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Create_ReturnsBadRequestStatusCode_WhenInValidDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        var debtId = Guid.Parse(TestDataConstants.TestEntity2Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);
        var dto = new TransactionDto
        {
            DebtId = debtId,
            Amount = TestDataConstants.IncorrectTransactionAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeUsd
        };

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Accept_ReturnsCompletedTransaction_WhenValidIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Accept(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Fact]
    public async void Accept_ReturnsNotFoundStatusCode_WhenInvalidIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var transactionId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Accept(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Accept_ReturnsBadRequest_WhenDebtStatusAlreadyAccepted()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var transaction = await context.Transactions.FindAsync(transactionId);
        transaction.Approved = true;
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Accept(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsTransactionResponse_WhenValidGuidAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity2Id);
        var dto = new TransactionDto
        {
            DebtId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 10
        };
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Update(transactionId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsForbiddenStatusCode_WhenInvalidUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var dto = new TransactionDto
        {
            DebtId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 10
        };
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Update(transactionId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsNotFoundStatusCode_WhenInvalidGuidAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var transactionId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        var dto = new TransactionDto
        {
            DebtId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 50
        };
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Update(transactionId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsBadRequestStatusCode_WhenInvalidGuidAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity2Id);
        var dto = new TransactionDto
        {
            DebtId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 50
        };
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var transaction = await context.Transactions.FindAsync(transactionId);
        transaction.Approved = true;
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Update(transactionId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsNotFoundStatusCode_WhenInvalidGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var transactionId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        var dto = new TransactionDto
        {
            DebtId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 50
        };
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Update(transactionId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Delete_ReturnsOkStatusCode_WhenValidTransactionIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Delete(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        var transaction = context.Transactions.FirstOrDefault(x => x.Id == transactionId);
        Assert.Null(transaction);
    }

    [Fact]
    public async void Delete_ReturnsForbiddenStatusCode_WhenInValidTransactionIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var transactionId = Guid.Parse(TestDataConstants.TestEntity2Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Delete(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Delete_ReturnsNotFoundStatusCode_WhenInValidTransactionIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var transactionId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(TransactionManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithTransactions().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new TransactionService(context, _mapper, accessor, _exchangeRate.Object);

        // Act
        var response = await sut.Delete(transactionId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}