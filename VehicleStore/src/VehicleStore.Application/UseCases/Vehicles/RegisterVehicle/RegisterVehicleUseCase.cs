using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Vehicles.RegisterVehicle;

public class RegisterVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public RegisterVehicleUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<RegisterVehicleOutput> ExecuteAsync(
        RegisterVehicleInput input,
        CancellationToken cancellationToken = default)
    {
        var existing = await _vehicleRepository.GetByPlateNumberAsync(input.PlateNumber, cancellationToken);
        if (existing is not null)
            throw new ConflictException($"A vehicle with plate number '{input.PlateNumber}' already exists.");

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            PlateNumber = input.PlateNumber.Trim().ToUpperInvariant(),
            Document = input.Document.Trim(),
            Brand = input.Brand.Trim(),
            Model = input.Model.Trim(),
            Year = input.Year,
            Price = input.Price,
            IsSold = false,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _vehicleRepository.CreateAsync(vehicle, cancellationToken);
        return new RegisterVehicleOutput(id);
    }
}
