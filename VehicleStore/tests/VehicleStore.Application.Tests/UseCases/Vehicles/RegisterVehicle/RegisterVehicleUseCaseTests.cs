using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Vehicles.RegisterVehicle;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Vehicles.RegisterVehicle;

public class RegisterVehicleUseCaseTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository = new();

    [Fact]
    public async Task ExecuteAsync_WhenValid_CreatesVehicleAndReturnsId()
    {
        var input = new RegisterVehicleInput("  abc1234  ", " 12345678901 ", " Toyota ", " Corolla ", 2024, 85000m);
        Vehicle? capturedVehicle = null;

        _vehicleRepository.Setup(r => r.GetByPlateNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);
        _vehicleRepository.Setup(r => r.CreateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .Callback<Vehicle, CancellationToken>((vehicle, _) => capturedVehicle = vehicle)
            .ReturnsAsync((Vehicle vehicle, CancellationToken _) => vehicle.Id);

        var sut = new RegisterVehicleUseCase(_vehicleRepository.Object);

        var result = await sut.ExecuteAsync(input);

        result.Id.Should().Be(capturedVehicle!.Id);
        capturedVehicle.PlateNumber.Should().Be("ABC1234");
        capturedVehicle.Document.Should().Be("12345678901");
        capturedVehicle.Brand.Should().Be("Toyota");
        capturedVehicle.Model.Should().Be("Corolla");
        capturedVehicle.Year.Should().Be(2024);
        capturedVehicle.Price.Should().Be(85000m);
        capturedVehicle.IsSold.Should().BeFalse();
        _vehicleRepository.Verify(r => r.CreateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPlateExists_ThrowsConflictException()
    {
        var input = new RegisterVehicleInput("ABC1234", "12345678901", "Toyota", "Corolla", 2024, 85000m);
        _vehicleRepository.Setup(r => r.GetByPlateNumberAsync(input.PlateNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreateVehicle(plateNumber: "ABC1234"));

        var sut = new RegisterVehicleUseCase(_vehicleRepository.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*A vehicle with plate number 'ABC1234' already exists*");
        _vehicleRepository.Verify(r => r.CreateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
