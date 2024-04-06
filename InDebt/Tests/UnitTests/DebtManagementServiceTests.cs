using Application.DTOs;
using Application.Mappers;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class DebtManagementServiceTests
{
    private readonly Mapper _mapper;

    public DebtManagementServiceTests()
    {
        _mapper = TestHelper.GetMapper(new DebtMapperProfile());
    }

    [Theory]
    [InlineData(true , true)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(null, true)]
    [InlineData(false, null)]
    [InlineData(null, null)]
        
    public async void
        GetAllReceivedWithCompleted_ReturnsAllDebtsOfUserWithChosenCompletedAndIsBorrowed_WhenValidUsernameProvided(bool? completed, bool? isBorrower)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.GetAll(completed, isBorrower, TestHelper.GetPaginationDto());

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void GetById_ReturnsDebtResponse_WhenValidDebtIdAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var debtId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.GetById(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        Assert.NotNull(response);
    }

    [Fact]
    public async void GetById_ReturnsNotFoundStatusCode_WhenInvalidDebtIdAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var debtId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.GetById(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Create_ReturnsCreatedStatusCode_WhenValidDtoAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity2Id),
            GroupId = Guid.Parse(TestDataConstants.TestEntity3Id),
            Amount = TestDataConstants.TestDebtAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeUsd
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers()
            .WithGroups()
            .WithCurrencies()
            .GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.IncorrectDate, TestDataConstants.ReminderDate)]
    [InlineData(TestDataConstants.EndDate, TestDataConstants.IncorrectDate)]
    public async void Create_ReturnsBadRequestStatusCode_WhenInValidDtoProvided(string inputEndDate, DateTime inputReminderDate)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        var endDate = DateTime.Parse(inputEndDate);
        var reminderDate = inputReminderDate;
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity2Id),
            GroupId = Guid.Parse(TestDataConstants.TestEntity3Id),
            Amount = TestDataConstants.TestDebtAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            EndDate = endDate,
            ReminderDate = reminderDate
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.IncorrectTestEntity1Id, TestDataConstants.TestEntity1Id, TestDataConstants.CurrencyCodeUsd)]
    [InlineData(TestDataConstants.TestEntity2Id, TestDataConstants.IncorrectTestEntity1Id, TestDataConstants.CurrencyCodeUsd)]
    [InlineData(TestDataConstants.TestEntity2Id, TestDataConstants.TestEntity1Id, TestDataConstants.IncorrectTestEntity1Id)]
    public async void Create_ReturnsNotFoundStatusCode_WhenInvalidDtoAndUsernameProvided(string borrowerId,
        string groupId, string currencyCode)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(borrowerId),
            GroupId = Guid.Parse(groupId),
            Amount = TestDataConstants.TestDebtAmount,
            CurrencyCode = currencyCode
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.TestEntity2Id)]
    public async void Create_ReturnsForbiddenStatusCode_WhenInvalidDtoAndUsernameProvided( string groupId)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity2Id),
            GroupId = Guid.Parse(groupId),
            Amount = TestDataConstants.TestDebtAmount,
            CurrencyCode = TestDataConstants.CurrencyCodeUsd
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Create(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Fact]
    public async void Accept_ReturnsNotFountStatusCode_WhenInvalidDebtIdAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var debtId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Accept(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Accept_ReturnsBadRequestStatusCode_WhenAlreadyAcceptedDebtIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        var debtId = Guid.Parse(TestDataConstants.TestEntity5Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var debt = await context.Debts.FindAsync(debtId);
        debt!.Approved = true;
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Accept(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Accept_ReturnsForbiddenStatusCode_WhenInvalidDebtIdAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var debtId = Guid.Parse(TestDataConstants.TestEntity3Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Accept(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsUpdatedDebtResponse_WhenValidDtoGuidAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var debtId = Guid.Parse(TestDataConstants.TestEntity4Id);
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity1Id),
            GroupId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 150
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Update(debtId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        //Assert.Equal(dto.Amount, response.Data.Amount);
    }
        
    [Fact]
    public async void Update_ReturnsNotFoundStatusCode_WhenInvalidDebtIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var debtId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity1Id),
            GroupId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 150
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Update(debtId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsForbiddenStatusCode_WhenInValidUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        var debtId = Guid.Parse(TestDataConstants.TestEntity2Id);
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity1Id),
            GroupId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 150
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Update(debtId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Update_ReturnsBadRequestStatusCode_WhenDebtAlreadyCompleted()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        var debtId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var dto = new DebtDto
        {
            BorrowerId = Guid.Parse(TestDataConstants.TestEntity1Id),
            GroupId = Guid.Parse(TestDataConstants.TestEntity1Id),
            CurrencyCode = TestDataConstants.CurrencyCodeUsd,
            Amount = 150
        };
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var debt = await context.Debts.FindAsync(debtId);
        debt!.Approved = true;
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Update(debtId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void Delete_ReturnsOkStatusCode_WhenValidDataProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var debtId = Guid.Parse(TestDataConstants.TestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Delete(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        var debt = await context.Debts.FindAsync(debtId);
        Assert.Null(debt);
    }

    [Fact]
    public async void Delete_ReturnsNotFound_WhenInvalidDebtIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var debtId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        await using var context = new TestDbContextBuilder(nameof(DebtManagementServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new DebtService(context, _mapper, accessor);

        // Act
        var response = await sut.Delete(debtId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}