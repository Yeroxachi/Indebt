using Domain.Entities;
using Xunit;

namespace Tests.DomainTests;

public class BaseEntityTests
{
    [Fact]
    public void Equals_ReturnsTrue_WhenTwoEntitiesWithSameIdProvided()
    {
        // Arrange
        var group = new Group
        {
            Name = "name",
            Description = "description",
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };
        var other = new Group
        {
            Name = "name1",
            Description = "description1",
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        // Act
        var response = group.Equals(other);

        // Assert
        Assert.True(response);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenTwoDifferentTypeEntitiesWithSameIdProvided()
    {
        // Arrange
        var group = new Group
        {
            Name = "name",
            Description = "description",
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };
        var other = new User
        {
            Username = "username1",
            Name = "name1",
            Surname = "surname1",
            Email = "email1@test.com",
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
        };

        // Act
        var response = group.Equals(other);

        // Assert
        Assert.False(response);
    }
}