using Mapper.Application.Features.Employees.Commands.CreateEmployee;

namespace Mapper.Tests.Validators;

public class CreateEmployeeValidatorTests
{
    private readonly CreateEmployeeValidator _validator;

    public CreateEmployeeValidatorTests()
    {
        _validator = new CreateEmployeeValidator();
    }

    [Fact]
    public void Validator_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: "Doe",
            Patronymic: null,
            Phone: "+1234567890",
            Email: "john.doe@example.com",
            Cabinet: "Room 101",
            Comment: "Team Lead",
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_EmptyFirstName_ShouldHaveError(string? firstName)
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: firstName!,
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: null,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FirstName));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_EmptySurname_ShouldHaveError(string? surname)
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: surname!,
            Patronymic: null,
            Phone: null,
            Email: null,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Surname));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test")]
    public void Validator_InvalidEmail_ShouldHaveError(string email)
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: email,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Email));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("firstname.lastname@company.com")]
    public void Validator_ValidEmail_ShouldNotHaveError(string email)
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: email,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validator_EmptyGeoMarkId_ShouldHaveError()
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: null,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.Empty
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.GeoMarkId));
    }

    [Fact]
    public void Validator_TooLongFirstName_ShouldHaveError()
    {
        // Arrange
        var longName = new string('A', 101);
        var command = new CreateEmployeeCommand(
            FirstName: longName,
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: null,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FirstName));
    }

    [Fact]
    public void Validator_TooLongComment_ShouldHaveError()
    {
        // Arrange
        var longComment = new string('A', 501);
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: null,
            Cabinet: null,
            Comment: longComment,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Comment));
    }

    [Fact]
    public void Validator_NullEmail_ShouldNotHaveError()
    {
        // Arrange
        var command = new CreateEmployeeCommand(
            FirstName: "John",
            Surname: "Doe",
            Patronymic: null,
            Phone: null,
            Email: null,
            Cabinet: null,
            Comment: null,
            GeoMarkId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }
}
