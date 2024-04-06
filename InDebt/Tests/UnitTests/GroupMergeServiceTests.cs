using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Tests.Helpers;
using Xunit;

namespace Tests.UnitTests;

public class GroupMergeServiceTests
{
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _accessor;

    public GroupMergeServiceTests()
    {
        _mapper = TestHelper.GetMappers();
        _accessor = TestHelper.CreateAccessor();
    }

    [Fact]
    public async void CreateMergeBetweenTwoGroups_ReturnCreatedStatusCode_WhenUsernameProvidedAndAllTwoGroupsAreCorrect()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Created;
        using var context = new TestDbContextBuilder(nameof(GroupMergeServiceTests))
            .WithUsers()
            .WithGroupsForMerge()
            .GetContext();
        var sut = new MergeRequestService(context, _mapper, _accessor);
        var firstGroupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var secondGroupId = Guid.Parse(TestDataConstants.TestEntity2Id);

        var dto = new MergeRequestDto
        {
            NewName = "",
            Description = "",
            GroupsId = new []
            {
                firstGroupId,
                secondGroupId
            }
        };
        //Act
        var response = await sut.CreateAsync(dto);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void CreateMergeBetweenTwoGroups_ReturnCreatedStatusCode_WhenUsernameProvidedAndUserHasNotAllGroupsInRequest()
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        using var context = new TestDbContextBuilder(nameof(GroupMergeServiceTests))
            .WithUsers()
            .WithGroups()
            .GetContext();
        var sut = new MergeRequestService(context, _mapper, _accessor);
        var firstGroupId = Guid.Parse(TestDataConstants.TestEntity1Id);
        var secondGroupId = Guid.Parse(TestDataConstants.TestEntity3Id);

        var dto = new MergeRequestDto
        {
            NewName = "",
            Description = "",
            GroupsId = new []
            {
                firstGroupId,
                secondGroupId
            }
        };
        //Act
        var response = await sut.CreateAsync(dto);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Fact]
    public async void GetUserReceivedConfirmationsAsync_ReturnsOkStatusCode_WhenUsernameProvided()
    {
        // Arrange
        const ResponseCode expectedCode = ResponseCode.Ok;
        using var context = new TestDbContextBuilder(nameof(GroupMergeServiceTests))
            .WithUsers()
            .WithGroups()
            .WithMerge()
            .WithMergeConfirmations()
            .GetContext();
        var sut = new MergeRequestService(context, _mapper, _accessor);

        // Act
        var response = await sut.GetAllReceivedAsync();

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
    
    [Theory]
    [InlineData(TestDataConstants.IncorrectTestEntity1Id,TestDataConstants.TestEntity2Id)]
    [InlineData(TestDataConstants.TestEntity1Id,TestDataConstants.IncorrectTestEntity1Id)]
    public async void CreateMergeBetweenTwoGroups_ReturnNotFoundStatusCode_WhenInvalidValidInviteIdAndUsernameProvided(string inputFirstGroupId, string inputSecondGroupId)
    {
        //Arrange
        const ResponseCode expectedCode = ResponseCode.Forbidden;
        using var context = new TestDbContextBuilder(nameof(GroupMergeServiceTests))
            .WithUsers()
            .WithGroupsForMerge()
            .GetContext();
        var sut = new MergeRequestService(context, _mapper, _accessor);
        var firstGroupId = Guid.Parse(inputFirstGroupId);
        var secondGroupId = Guid.Parse(inputSecondGroupId);

        var dto = new MergeRequestDto
        {
            NewName = "",
            Description = "",
            GroupsId = new []
            {
                firstGroupId,
                secondGroupId
            }
        };
        //Act
        var response = await sut.CreateAsync(dto);
        
        //Assert
        Assert.Equal(expectedCode, response.Code);
        
    }

    [Fact]
    public async void AcceptConfirmation_ReturnOkResponseCode_LoggedInAndCorrectConfirmationProvided()
    {
        const ResponseCode expectedCode = ResponseCode.Ok;
        const bool accepted = true;
        using var context = new TestDbContextBuilder(nameof(GroupMergeServiceTests))
            .WithUsers()
            .WithGroups()
            .WithMerge()
            .WithMergeConfirmations()
            .GetContext();
        var sut = new MergeRequestService(context, _mapper, _accessor);

        var confirmationId = Guid.Parse(TestDataConstants.TestEntity1Id);

        // Act
        var response = await sut.AcceptAsync(confirmationId, accepted);

        // Assert
        Assert.Equal(expectedCode, response.Code);
    }
}