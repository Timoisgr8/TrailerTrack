
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TrailerTrack.Application.Assets.Commands;
using TrailerTrack.Domain.Entities;
using TrailerTrack.Domain.Enums;
using TrailerTrack.Domain.Interfaces;

namespace TrailerTrack.Tests.Assets;

public class CreateAssetCommandHandlerTests
{
    private readonly Mock<IAssetRepository> _repositoryMock;
    private readonly Mock<IValidator<CreateAssetCommand>> _validatorMock;
    private readonly CreateAssetCommandHandler _handler;

    public CreateAssetCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAssetRepository>();
        _validatorMock = new Mock<IValidator<CreateAssetCommand>>();
        _handler = new CreateAssetCommandHandler(_repositoryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateAssetCommand("TRL-001", "Depot A", AssetType.BoxTrailer);

        // Validation passes
        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Asset code does not already exist
        _repositoryMock
            .Setup(r => r.ExistsByCodeAsync(command.AssetCode, default))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        // Verify asset was saved
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Asset>(), default), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }


    [Fact]
    public async Task Handle_DuplicateAssetCode_ReturnsFailure()
    {
        // Arrange
        var command = new CreateAssetCommand("TRL-001", "Depot A", AssetType.BoxTrailer);

        // Validation passes
        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult());

        // Asset code already exists
        _repositoryMock
            .Setup(r => r.ExistsByCodeAsync(command.AssetCode, default))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");

        // Verify nothing was saved
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Asset>(), default), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ReturnsFailure()
    {
        // Arrange
        var command = new CreateAssetCommand("", "Depot A", AssetType.BoxTrailer);

        // Validation fails
        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("AssetCode", "Asset code is required.")
        };
        _validatorMock
            .Setup(v => v.ValidateAsync(command, default))
            .ReturnsAsync(new ValidationResult(failures));

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Asset code is required.");

        // Verify nothing was saved
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Asset>(), default), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
    }

}