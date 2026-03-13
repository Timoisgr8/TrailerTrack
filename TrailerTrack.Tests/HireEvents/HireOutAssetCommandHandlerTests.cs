
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TrailerTrack.Application.HireEvents.Commands;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Tests.Assets;

public class HireOutAssetCommandHandlerTests
{
    private readonly Mock<IHireEventRepository> _hireEventRepositoryMock;
    private readonly Mock<IAssetRepository> _assetRepositoryMock;
    private readonly Mock<IValidator<HireOutAssetCommand>> _validatorMock;
    private readonly HireOutAssetCommandHandler _handler;

    public HireOutAssetCommandHandlerTests()
    {
        _hireEventRepositoryMock = new Mock<IHireEventRepository>();
        _assetRepositoryMock = new Mock<IAssetRepository>();
        _validatorMock = new Mock<IValidator<HireOutAssetCommand>>();
        _handler = new HireOutAssetCommandHandler(_hireEventRepositoryMock.Object, _assetRepositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new HireOutAssetCommand(Guid.NewGuid(), "John Doe", "Customer A", "0400000000", DateTime.UtcNow);
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _hireEventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<HireEvent>(), default), Times.Once);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }


    [Fact]
    public async Task Handle_AssetNotFound_ReturnsFailure()
    {
        // Arrange
        var command = new HireOutAssetCommand(Guid.NewGuid(), "John Doe", "Customer A", "0400000000", DateTime.UtcNow);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync((Asset?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");


        _hireEventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<HireEvent>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_AssetNotAvaliableForHire_ReturnsFailure()
    {
        // Arrange
        var command = new HireOutAssetCommand(Guid.NewGuid(), "John Doe", "Customer A", "0400000000", DateTime.UtcNow);
        var asset = Asset.Create("TRL-001", "Depot A", AssetType.BoxTrailer);
        asset.UpdateStatus(AssetStatus.HiredOut);

        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        _assetRepositoryMock
            .Setup(r => r.GetByIdAsync(command.Id, default))
            .ReturnsAsync(asset);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not available for hire");


        _hireEventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<HireEvent>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }


    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailure()
    {
        // Arrange
        var command = new HireOutAssetCommand(Guid.NewGuid(), "", "Customer A", "0400000000", DateTime.UtcNow);

        // Validation fails
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("PerformedBy", "Performed by is required.")
        };


        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Performed by is required.");


        _hireEventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<HireEvent>(), default), Times.Never);
        _assetRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }
}