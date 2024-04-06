using Application.DTOs;
using Application.Mappers;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class GroupServiceTests
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _accessor;

    public GroupServiceTests()
    {
        _mapper = TestHelper.GetMapper(new GroupMapperProfile());
        _accessor = TestHelper.CreateAccessor();
    }

    [Fact]
    public async void GetAllGroups_ReturnsOKStatusCodeAndUserData_WhenUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetAllAsync(TestHelper.GetPaginationDto());

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
        
    [Fact]
    public async void GetGroupById_ReturnsOKStatusAndUser_WhenValidGuidProvided()
    {
        // Arrange
        var groupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetByIdAsync(groupId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void GetGroupById_ReturnsNotFoundStatusCode_WhenInvalidGuidProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.NotFound;
        var groupId = Guid.Parse(TestDataConstants.IncorrectTestEntity1Id);
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper,_accessor);

        // Act
        var response = await sut.GetByIdAsync(groupId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void CreateGroup_ReturnsOkStatusCode_WhenValidDtoAndUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        var dto = new GroupDto()
        {
            Name = "name",
            Description = "description"
        };
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var response = await sut.CreateAsync(dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void UpdateGroup_ReturnsOkStatusCode_WhenValidGuidAndDtoProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var groupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var dto = new GroupDto()
        {
            Description = "updateddescription",
            Name = "updatedname"
        };
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var respones = await sut.UpdateAsync(groupId, dto);

        // Assert
        Assert.Equal(expectedCode, respones.Code);
        var group = await context.Groups.FindAsync(groupId);
        Assert.NotNull(group);
        Assert.Equal(dto.Name, group.Name);
        Assert.Equal(dto.Description, group.Description);
    }

    [Theory]
    [InlineData( TestDataConstants.IncorrectTestEntity1Id, ResponseCode.NotFound)]
    [InlineData( TestDataConstants.TestEntity3Id, ResponseCode.Forbidden)]
    public async void UpdateGroup_ReturnsFailStatusCode_WhenInValidDataProvided(Guid inputGroupId, ResponseCode responseCode)
    {
        // Arrange
        var expectedCode = responseCode;
        var groupId = inputGroupId;
        var dto = new GroupDto()
        {
            Name = "name",
            Description = "description"
        };
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var response = await sut.UpdateAsync(groupId, dto);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }

    [Fact]
    public async void DeleteGroup_RemovesGroup_WhenValidIdProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        var groupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests)).WithUsers().WithGroups().GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var response = await sut.DeleteAsync(groupId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
        var group = await context.Groups.FindAsync(groupId);
        Assert.Null(group);
    }

    [Theory]
    [InlineData(TestDataConstants.IncorrectTestEntity1Id, ResponseCode.NotFound)]
    [InlineData(TestDataConstants.TestEntity3Id, ResponseCode.Forbidden)]
    public async void DeleteGroup_ReturnsFailStatusCode_WhenInvalidIdProvided( Guid inputGroupId, ResponseCode responseCode)
    {
        // Arrange
        var expectedCode = responseCode;
        var groupId = inputGroupId;
        using var context = new TestDbContextBuilder(nameof(GroupServiceTests))
            .WithUsers()
            .WithGroups()
            .GetContext();
        var sut = new GroupService(context, _mapper, _accessor);

        // Act
        var response = await sut.DeleteAsync(groupId);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}