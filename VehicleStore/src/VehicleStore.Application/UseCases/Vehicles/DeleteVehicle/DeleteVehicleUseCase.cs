using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Vehicles.DeleteVehicle;

public class DeleteVehicleUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteVehicleUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vehicle with id '{id}' was not found.");

        await _vehicleRepository.DeleteAsync(vehicle.Id, cancellationToken);
    }
}
