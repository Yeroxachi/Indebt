using Application.DTOs;
using Application.Mappers;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class UserServiceTests
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _accessor;

    public UserServiceTests()
    {
        _mapper = TestHelper.GetMapper(new UserMapperProfile());
        _accessor = TestHelper.CreateAccessor();
    }

    [Fact]
    public async void GetAllUsers_ReturnsOKStatusCodeAndUserData()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetAllAsync(TestHelper.GetPaginationDto());

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void GetUserById_ReturnsOKStatusAndUser_WhenValidGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var userId = Guid.Parse(TestDataConstants.TestEntity1Id);
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetByIdAsync(userId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void GetUserById_ReturnsNotFoundStatusCode_WhenInvalidGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var userId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetByIdAsync(userId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void UpdateUser_ReturnsOkCodeWithUpdatedUser_WhenValidUserIdAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var userId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var dto = new UserDto()
        {
            Name = "name",
            Surname = "surname",
            Email = "updateemail@test.com",
            Username = "updateusername"
        };
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.UpdateAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        var user = await context.Users.FindAsync(userId);
        Assert.NotNull(user);
        Assert.Equal(dto.Username, user.Username);
        Assert.Equal(dto.Email, user.Email);
    }

    [Fact]
    public async void UpdateUser_ReturnsNotFound_WhenInvalidUserIdAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var dto = new UserDto()
        {
            Email = TestDataConstants.IncorrectEmail,
            Username = TestDataConstants.IncorrectEntityUsername,
            Name = TestDataConstants.IncorrectEntityUsername,
            Surname = TestDataConstants.IncorrectEntityUsername
        };
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.UpdateAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void DeleteUser_RemovesUser_WhenValidIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var userId = Guid.Parse(TestDataConstants.TestEntity1Id);
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.DeleteAsync(userId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        var user = await context.Users.FindAsync(userId);
        Assert.Null(user);
    }

    [Fact]
    public async void DeleteUser_ReturnsNotFoundCode_WhenInvalidIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var userId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        using var context = new TestDbContextBuilder(nameof(UserServiceTests)).WithUsers().GetContext();
        var sut = new UserService(context, _mapper, _accessor);

        // Act
        var response = await sut.DeleteAsync(userId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}