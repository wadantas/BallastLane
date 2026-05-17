using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Vehicles.DeleteVehicle;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Vehicles.DeleteVehicle;

public class DeleteVehicleUseCaseTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();

    [Fact]
    public async Task ExecuteAsync_WhenVehicleExists_DeletesVehicle()
    {
        var vehicle = TestData.CreateVehicle();
        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new DeleteVehicleUseCase(_vehicleRepository.Object);

        await sut.ExecuteAsync(vehicle.Id);

        _vehicleRepository.Verify(r => r.DeleteAsync(vehicle.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenVehicleNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _vehicleRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Vehicle?)null);

        var sut = new DeleteVehicleUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Vehicle with id '{id}' was not found*");
        _vehicleRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
