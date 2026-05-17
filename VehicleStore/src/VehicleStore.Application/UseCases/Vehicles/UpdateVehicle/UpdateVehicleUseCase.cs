using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Vehicles.UpdateVehicle;

public class UpdateVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public UpdateVehicleUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task ExecuteAsync(UpdateVehicleInput input, CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new NotFoundException($"Vehicle with id '{input.Id}' was not found.");

        if (vehicle.IsSold)
            throw new BusinessException("Cannot update a vehicle that has already been sold.");

        var plateConflict = await _vehicleRepository.GetByPlateNumberAsync(input.PlateNumber, cancellationToken);
        if (plateConflict is not null && plateConflict.Id != input.Id)
            throw new ConflictException($"A vehicle with plate number '{input.PlateNumber}' already exists.");

        vehicle.PlateNumber = input.PlateNumber.Trim().ToUpperInvariant();
        vehicle.Document = input.Document.Trim();
        vehicle.Brand = input.Brand.Trim();
        vehicle.Model = input.Model.Trim();
        vehicle.Year = input.Year;
        vehicle.Price = input.Price;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
    }
}
