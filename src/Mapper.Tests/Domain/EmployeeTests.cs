using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class EmployeeTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            Surname = "Doe",
            GeoMarkId = Guid.NewGuid()
        };

        // Assert
        Assert.False(employee.IsArchived);
    }

    [Fact]
    public void FullName_WithoutPatronymic_ShouldReturnFirstNameAndSurname()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            Surname = "Doe",
            GeoMarkId = Guid.NewGuid()
        };

        // Act
        var fullName = employee.FullName;

        // Assert
        Assert.Equal("John Doe", fullName);
    }

    [Fact]
    public void FullName_WithPatronymic_ShouldReturnFullName()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Ivan",
            Patronymic = "Petrovich",
            Surname = "Sidorov",
            GeoMarkId = Guid.NewGuid()
        };

        // Act
        var fullName = employee.FullName;

        // Assert
        Assert.Equal("Ivan Sidorov Petrovich", fullName);
    }

    [Fact]
    public void FullName_WithEmptyPatronymic_ShouldReturnFirstNameAndSurname()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            Patronymic = "   ",
            Surname = "Smith",
            GeoMarkId = Guid.NewGuid()
        };

        // Act
        var fullName = employee.FullName;

        // Assert
        Assert.Equal("Jane Smith", fullName);
    }

    [Fact]
    public void Employee_ShouldAcceptAllOptionalProperties()
    {
        // Arrange
        var geoMarkId = Guid.NewGuid();
        var photoId = Guid.NewGuid();

        // Act
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            Surname = "Johnson",
            Patronymic = "Marie",
            Phone = "+1234567890",
            Cabinet = "Room 301",
            Comment = "Team Lead",
            Email = "alice.johnson@example.com",
            GeoMarkId = geoMarkId,
            EmployeePhotoId = photoId,
            IsArchived = true
        };

        // Assert
        Assert.Equal("Alice", employee.FirstName);
        Assert.Equal("Johnson", employee.Surname);
        Assert.Equal("Marie", employee.Patronymic);
        Assert.Equal("+1234567890", employee.Phone);
        Assert.Equal("Room 301", employee.Cabinet);
        Assert.Equal("Team Lead", employee.Comment);
        Assert.Equal("alice.johnson@example.com", employee.Email);
        Assert.Equal(geoMarkId, employee.GeoMarkId);
        Assert.Equal(photoId, employee.EmployeePhotoId);
        Assert.True(employee.IsArchived);
    }
}
