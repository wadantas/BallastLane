using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Vehicles.MarkVehicleAsSold;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Vehicles.MarkVehicleAsSold;

public class MarkVehicleAsSoldUseCaseTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();

    [Fact]
    public async Task ExecuteAsync_WhenVehicleExists_MarksAsSold()
    {
        var vehicle = TestData.CreateVehicle();
        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new MarkVehicleAsSoldUseCase(_vehicleRepository.Object);

        await sut.ExecuteAsync(vehicle.Id);

        _vehicleRepository.Verify(r => r.MarkAsSoldAsync(vehicle.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenVehicleNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _vehicleRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Vehicle?)null);

        var sut = new MarkVehicleAsSoldUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Vehicle with id '{id}' was not found*");
        _vehicleRepository.Verify(r => r.MarkAsSoldAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenVehicleAlreadySold_ThrowsBusinessException()
    {
        var vehicle = TestData.CreateVehicle(isSold: true);
        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new MarkVehicleAsSoldUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(vehicle.Id);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Vehicle is already marked as sold.");
        _vehicleRepository.Verify(r => r.MarkAsSoldAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
