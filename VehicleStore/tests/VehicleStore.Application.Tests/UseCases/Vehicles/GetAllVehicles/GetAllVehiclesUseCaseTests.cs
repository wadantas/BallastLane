using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Vehicles.GetAllVehicles;
using VehicleStore.Domain.Entities;

namespace VehicleStore.Application.Tests.UseCases.Vehicles.GetAllVehicles;

public class GetAllVehiclesUseCaseTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();

    [Fact]
    public async Task ExecuteAsync_ReturnsVehiclesFromRepository()
    {
        IReadOnlyList<Vehicle> vehicles = [TestData.CreateVehicle(), TestData.CreateVehicle(plateNumber: "XYZ9876")];
        _vehicleRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        var sut = new GetAllVehiclesUseCase(_vehicleRepository.Object);

        var result = await sut.ExecuteAsync();

        result.Should().BeEquivalentTo(vehicles);
        _vehicleRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
