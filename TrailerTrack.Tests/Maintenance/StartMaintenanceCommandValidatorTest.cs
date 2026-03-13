using FluentAssertions;
using FluentValidation;
using TrailerTrack.Application.Maintenance.Commands;

namespace TrailerTrack.Tests.Maintenance;

public class StartMaintenanceCommandValidatorTests
{
    private readonly IValidator<StartMaintenanceCommand> _validator = new StartMaintenanceCommandValidator();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        var command = new StartMaintenanceCommand("Fix brakes", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_InvalidDescription_FailsValidation(string description)
    {
        var command = new StartMaintenanceCommand(description, Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Description is required.");
    }

    [Fact]
    public async Task Validate_ExpectedCompletionBeforeStartDate_FailsValidation()
    {
        var command = new StartMaintenanceCommand("Fix brakes", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Expected completion date must be after the start date.");
    }

    [Fact]
    public async Task Validate_NullExpectedCompletionAt_PassesValidation()
    {
        // ExpectedCompletionAt is optional so null should pass
        var command = new StartMaintenanceCommand("Fix brakes", Guid.NewGuid(), DateTime.UtcNow, null);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}