using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Vehicles.GetVehicleById;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Vehicles.GetVehicleById;

public class GetVehicleByIdUseCaseTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();

    [Fact]
    public async Task ExecuteAsync_WhenVehicleExists_ReturnsVehicle()
    {
        var vehicle = TestData.CreateVehicle();
        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new GetVehicleByIdUseCase(_vehicleRepository.Object);

        var result = await sut.ExecuteAsync(vehicle.Id);

        result.Should().BeSameAs(vehicle);
    }

    [Fact]
    public async Task ExecuteAsync_WhenVehicleNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _vehicleRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Vehicle?)null);

        var sut = new GetVehicleByIdUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(id);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Vehicle with id '{id}' was not found*");
    }
}
