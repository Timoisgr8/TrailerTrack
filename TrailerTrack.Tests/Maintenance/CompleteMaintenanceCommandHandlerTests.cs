
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TrailerTrack.Application.Assets.Commands;
using TrailerTrack.Application.HireEvents.Commands;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Application.Maintenance.Commands;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Tests.Maintenance;

public class CompleteMaintenanceCommandHandlerTests
{
    private readonly Mock<IMaintenanceLogRepository> _maintenanceLogRepositoryMock;
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IValidator<CompleteMaintenanceCommand>> _validatorMock;

    private readonly CompleteMaintenanceCommandHandler _handler;

    public CompleteMaintenanceCommandHandlerTests()
    {
        _maintenanceLogRepositoryMock = new Mock<IMaintenanceLogRepository>();
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _validatorMock = new Mock<IValidator<CompleteMaintenanceCommand>>();
        _handler = new CompleteMaintenanceCommandHandler(
            _maintenanceLogRepositoryMock.Object,
            _assetRepositoryMock.Object,
            _currentUserMock.Object,
            _validatorMock.Object
            );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.UnderMaintenance);
        var maintenanceLog = MaintenanceLog.StartMaintenance("Fix brakes", asset.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        var command = new CompleteMaintenanceCommand("All done.", "John Doe", 100M, maintenanceLog.Id);

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _maintenanceLogRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(maintenanceLog);

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(maintenanceLog.AssetId, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }


    [Fact]
    public async Task Handle_MaintenanceLogNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new CompleteMaintenanceCommand("All done.", "John Doe", 100M, Guid.NewGuid());

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _maintenanceLogRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync((MaintenanceLog?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_MaintenanceLogAlreadyComplete_ReturnsFailure()
    {
        // Arrange
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.UnderMaintenance);
        var maintenanceLog = MaintenanceLog.StartMaintenance("Fix brakes", asset.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        maintenanceLog.CompleteMaintenance("All done.", "John Doe", 100M);

        var command = new CompleteMaintenanceCommand("All done.", "John Doe", 100M, maintenanceLog.Id);

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _maintenanceLogRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(maintenanceLog);

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(maintenanceLog.AssetId, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already completed");
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }


    [Fact]
    public async Task Handle_AssetNotFound_ReturnsFailure()
    {
        var maintenanceLog = MaintenanceLog.StartMaintenance("Fix brakes", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
        var command = new CompleteMaintenanceCommand("All done.", "John Doe", 100M, maintenanceLog.Id);

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _maintenanceLogRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(maintenanceLog);

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(maintenanceLog.AssetId, default))
            .ReturnsAsync((Asset?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailure()
    {
        // Arrange
        var command = new CompleteMaintenanceCommand("All done.", "", 100M, Guid.NewGuid());

        // Validation fails
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("PerformedBy", "Performed by is required.")
        };

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Performed by is required.");

        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsFailure()
    {
        // Arrange
        var command = new CompleteMaintenanceCommand("All done.", "", 100M, Guid.NewGuid());

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unauthorised");

        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }
}