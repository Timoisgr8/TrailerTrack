
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TrailerTrack.Application.Assets.Commands;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Application.Maintenance.Commands;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Tests.Maintenance;

public class StartMaintenanceCommandHandlerTests
{
    private readonly Mock<IMaintenanceLogRepository> _maintenanceLogRepositoryMock;
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IValidator<StartMaintenanceCommand>> _validatorMock;
    private readonly StartMaintenanceCommandHandler _handler;

    public StartMaintenanceCommandHandlerTests()
    {
        _maintenanceLogRepositoryMock = new Mock<IMaintenanceLogRepository>();
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _validatorMock = new Mock<IValidator<StartMaintenanceCommand>>();

        _handler = new StartMaintenanceCommandHandler(
            _maintenanceLogRepositoryMock.Object,
            _assetRepositoryMock.Object,
            _currentUserMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        var command = new StartMaintenanceCommand("Fix brakes", asset.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.AssetId, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Once);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }


    [Fact]
    public async Task Handle_AssetNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new StartMaintenanceCommand("Fix brakes", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.AssetId, default))
            .ReturnsAsync((Asset?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }


    [Fact]
    public async Task Handle_AssetRetired_ReturnsFailure()
    {
        // Arrange
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.Retired);
        var command = new StartMaintenanceCommand("Fix brakes", asset.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.AssetId, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("retired asset");

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_AssetHiredOut_ReturnsFailure()
    {
        // Arrange
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.HiredOut);
        var command = new StartMaintenanceCommand("Fix brakes", asset.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.AssetId, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("hired out asset");

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }


    [Fact]
    public async Task Handle_AssetUnderMaintenance_ReturnsFailure()
    {
        // Arrange
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.UnderMaintenance);
        var command = new StartMaintenanceCommand("Fix brakes", asset.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.AssetId, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already under maintenance");

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailure()
    {
        // Arrange
        var command = new StartMaintenanceCommand("", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        // Validation fails
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Description", "Description is required.")
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
        result.Error.Should().Contain("Description is required.");

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsFailure()
    {
        // Arrange
        var command = new StartMaintenanceCommand("", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unauthorised");

        _maintenanceLogRepositoryMock.Verify(r => r.AddAsync(It.IsAny<MaintenanceLog>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }
}