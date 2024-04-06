using Application.Services;
using AutoMapper;
using Domain.Enums;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class NotificationServiceTests
{
    private readonly IMapper _mapper;

    public NotificationServiceTests()
    {
        _mapper = TestHelper.GetMappers();
    }

    [Fact]
    public async void GetAllAsync_ReturnsOkCode()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);

        // Act
        var response = await sut.GetAllUnreadAsync(TestHelper.GetPaginationDto());

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void MarkAsReadAsync_ReturnsOkCode_WhenCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);
        var notificationId = Guid.Parse(TestDataConstants.TestEntity2Id);

        // Act
        var response = await sut.MarkAsReadAsync(notificationId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void MarkAsReadAsync_ReturnsForbiddenCode_WhenCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);
        var notificationId = Guid.Parse(TestDataConstants.TestEntity1Id);

        // Act
        var response = await sut.MarkAsReadAsync(notificationId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void MarkAsReadAsync_ReturnsNotFoundCode_WhenCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);
        var notificationId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);

        // Act
        var response = await sut.MarkAsReadAsync(notificationId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void MarkAsReadAsync_ReturnsBadRequestCode_WhenCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);
        var notificationId = Guid.Parse(TestDataConstants.TestEntity2Id);
        var notification = await context.Notifications.FindAsync(notificationId);
        notification.IsRead = true;

        // Act
        var response = await sut.MarkAsReadAsync(notificationId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void DeleteAsync_ReturnsOkCode_WhenCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);

        // Act
        var response = await sut.DeleteAsync(Guid.Parse(TestDataConstants.TestEntity2Id));

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void DeleteAsync_ReturnsNotFoundCode_WhenInCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);

        // Act
        var response = await sut.DeleteAsync(Guid.Parse(TestDataConstants.IncorrectTestEntity1Id));

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void DeleteAsync_ReturnsForbiddenCode_WhenInCorrectGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        await using var context = new TestDbContextBuilder(nameof(NotificationServiceTests))
            .WithUsers().WithGroups().WithCurrencies().WithDebts().WithNotifications().GetContext();
        var accessor = TestHelper.CreateAccessor();
        var sut = new NotificationService(context, _mapper, accessor);

        // Act
        var response = await sut.DeleteAsync(Guid.Parse(TestDataConstants.TestEntity1Id));

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}