
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TrailerTrack.Application.Assets.Commands;
using TrailerTrack.Application.Interfaces;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Tests.Assets;

public class RetireAssetCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _repositoryMock;
    private readonly Mock<IValidator<RetireAssetCommand>> _validatorMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly RetireAssetCommandHandler _handler;


    public RetireAssetCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAssetRepository>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _validatorMock = new Mock<IValidator<RetireAssetCommand>>();

        _handler = new RetireAssetCommandHandler(
            _repositoryMock.Object,
            _currentUserMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new RetireAssetCommand(Guid.NewGuid());
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);

        // Validation passes
        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Asset code does not already exist
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify asset was saved
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_AssetNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new RetireAssetCommand(Guid.NewGuid());

        // Validation passes
        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Asset code does not already exist
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync((Asset?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");

        // Verify asset was not saved
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_AssetAlreadyRetired_ReturnsFailure()
    {
        // Arrange
        var command = new RetireAssetCommand(Guid.NewGuid());
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.Retired);

        // Validation passes
        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Asset code does not already exist
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already retired");

        // Verify asset was not saved
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }


    [Fact]
    public async Task Handle_AssetHiredOut_ReturnsFailure()
    {
        // Arrange
        var command = new RetireAssetCommand(Guid.NewGuid());
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.HiredOut);

        // Validation passes
        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(true);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Asset code does not already exist
        _repositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("currently hired out");

        // Verify asset was not saved
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }


    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailure()
    {
        // Arrange
        var command = new RetireAssetCommand(Guid.NewGuid());

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Id", "Id is required")
        };

        // Validation fails
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
        result.Error.Should().Contain("Id is required");

        // Verify asset was not saved
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotAdmin_ReturnsFailure()
    {
        // Arrange
        var command = new RetireAssetCommand(Guid.NewGuid());

        _currentUserMock
            .Setup(c => c.IsInRole("Admin"))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unauthorised");

        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }
}