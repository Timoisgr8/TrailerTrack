using FluentAssertions;
using FluentValidation;
using TrailerTrack.Application.Maintenance.Commands;

namespace TrailerTrack.Tests.Maintenance;

public class CompleteMaintenanceCommandValidatorTests
{
    private readonly IValidator<CompleteMaintenanceCommand> _validator = new CompleteMaintenanceCommandValidator();

    [Fact]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        var command = new CompleteMaintenanceCommand("Tire maintenance complete.", "John Doe", 100M, Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_InvalidDescription_FailsValidation(string description)
    {
        var command = new CompleteMaintenanceCommand(description, "John Doe", 100M, Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Completed notes are required.");
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_InvalidPerformedBy_FailsValidation(string performedBy)
    {
        var command = new CompleteMaintenanceCommand("Tire maintenance complete.", performedBy, 100M, Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Performed by is required.");
    }

    [Fact]
    public async Task Validate_NonNegativeCost_FailsValidation()
    {
        var command = new CompleteMaintenanceCommand("Tire maintenance complete.", "John Doe", -1M, Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Cost must be non negative.");
    }

    [Fact]
    public async Task Validate_ZeroCost_PassesValidation()
    {
        var command = new CompleteMaintenanceCommand("Tire maintenance complete.", "John Doe", 0M, Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NullCost_PassesValidation()
    {
        var command = new CompleteMaintenanceCommand("Tire maintenance complete.", "John Doe", null, Guid.NewGuid());
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}