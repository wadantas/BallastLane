using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Vehicles.UpdateVehicle;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Vehicles.UpdateVehicle;

public class UpdateVehicleUseCaseTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();

    [Fact]
    public async Task ExecuteAsync_WhenValid_UpdatesVehicle()
    {
        var vehicle = TestData.CreateVehicle();
        var input = new UpdateVehicleInput(vehicle.Id, "  xyz9876  ", " 98765432100 ", " Honda ", " Civic ", 2023, 75000m);

        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _vehicleRepository.Setup(r => r.GetByPlateNumberAsync(input.PlateNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var sut = new UpdateVehicleUseCase(_vehicleRepository.Object);

        await sut.ExecuteAsync(input);

        vehicle.PlateNumber.Should().Be("XYZ9876");
        vehicle.Document.Should().Be("98765432100");
        vehicle.Brand.Should().Be("Honda");
        vehicle.Model.Should().Be("Civic");
        vehicle.Year.Should().Be(2023);
        vehicle.Price.Should().Be(75000m);
        vehicle.UpdatedAt.Should().NotBeNull();
        _vehicleRepository.Verify(r => r.UpdateAsync(vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenVehicleNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        var input = new UpdateVehicleInput(id, "ABC1234", "123", "Brand", "Model", 2024, 10000m);
        _vehicleRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);

        var sut = new UpdateVehicleUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Vehicle with id '{id}' was not found*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenVehicleIsSold_ThrowsBusinessException()
    {
        var vehicle = TestData.CreateVehicle(isSold: true);
        var input = new UpdateVehicleInput(vehicle.Id, "ABC1234", "123", "Brand", "Model", 2024, 10000m);
        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new UpdateVehicleUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Cannot update a vehicle that has already been sold.");
        _vehicleRepository.Verify(r => r.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlateBelongsToAnotherVehicle_ThrowsConflictException()
    {
        var vehicle = TestData.CreateVehicle();
        var otherVehicle = TestData.CreateVehicle(plateNumber: "TAKEN123");
        var input = new UpdateVehicleInput(vehicle.Id, "TAKEN123", "123", "Brand", "Model", 2024, 10000m);

        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _vehicleRepository.Setup(r => r.GetByPlateNumberAsync(input.PlateNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(otherVehicle);

        var sut = new UpdateVehicleUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*A vehicle with plate number 'TAKEN123' already exists*");
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlateBelongsToSameVehicle_UpdatesSuccessfully()
    {
        var vehicle = TestData.CreateVehicle(plateNumber: "ABC1234");
        var input = new UpdateVehicleInput(vehicle.Id, "ABC1234", "123", "Brand", "Model", 2024, 10000m);

        _vehicleRepository.Setup(r => r.GetByIdAsync(vehicle.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);
        _vehicleRepository.Setup(r => r.GetByPlateNumberAsync(input.PlateNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicle);

        var sut = new UpdateVehicleUseCase(_vehicleRepository.Object);

        await sut.ExecuteAsync(input);

        _vehicleRepository.Verify(r => r.UpdateAsync(vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }
}
