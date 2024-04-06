using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class GroupInviteServiceTests
{
    private readonly Mapper _mapper;
    private readonly IHttpContextAccessor _accessor;

    public GroupInviteServiceTests()
    {
        _mapper = TestHelper.GetMappers();
        _accessor = TestHelper.CreateAccessor();
    }

    [Fact]
    public async void SendInviteAsync_ReturnsCreatedStatusCode_WhenUsernameAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);
        var groupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var invitedId = Guid.Parse(TestDataConstants.TestEntity2Id);
        var dto = new GroupInviteDto()
        {
            GroupId = groupId,
            InvitedId = invitedId
        };

        // Act
        var response = await sut.SendAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.IncorrectTestEntity1Id, TestDataConstants.TestEntity1Id)]
    [InlineData(TestDataConstants.TestEntity1Id, TestDataConstants.IncorrectTestEntity1Id)]
    public async void SendInviteAsync_ReturnsNotFoundStatusCode_WhenInvalidDataProvided(string inputGroupId, string inputInvitedId)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .WithGroupInvites()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);
        var groupId = Guid.Parse(inputGroupId);
        var invitedId = Guid.Parse(inputInvitedId);
        var dto = new GroupInviteDto()
        {
            GroupId = groupId,
            InvitedId = invitedId
        };

        // Act
        var response = await sut.SendAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void SendInviteAsync_ReturnsForbiddenStatusCode_WhenUserIsNotInGroup()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .WithGroupInvites()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);
        var groupId = Guid.Parse(TestDataConstants.TestEntity3Id);
        var invitedId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var dto = new GroupInviteDto()
        {
            GroupId = groupId,
            InvitedId = invitedId
        };

        // Act
        var response = await sut.SendAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void SendInviteAsync_ReturnsBadRequestStatusCode_WhenUserAlreadyInGroup()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.BadRequest;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .WithGroupInvites()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);
        var groupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var invitedId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var dto = new GroupInviteDto()
        {
            GroupId = groupId,
            InvitedId = invitedId
        };

        // Act
        var response = await sut.SendAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Theory]
    [InlineData(TestDataConstants.IncorrectTestEntity1Id)]
    public async void AcceptInviteAsync_ReturnsNotFoundStatusCode_WhenInvalidValidInviteIdAndUsernameProvided(string inputInviteId)
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .WithGroupInvites()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);
        var inviteId = Guid.Parse(inputInviteId);

        // Act
        var response = await sut.AcceptAsync(inviteId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void AcceptInviteAsync_ReturnsForbiddenStatusCode_WhenUserNotInvited()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .WithGroupInvites()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);
        var inviteId = Guid.Parse(TestDataConstants.TestEntity1Id);

        // Act
        var response = await sut.AcceptAsync(inviteId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void GetUserReceivedInvitesAsync_ReturnsOkStatusCode_WhenUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(GroupInviteServiceTests))
            .WithUsers()
            .WithGroups()
            .WithGroupInvites()
            .GetContext();
        var sut = new GroupInviteService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetAllReceivedAsync();

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}