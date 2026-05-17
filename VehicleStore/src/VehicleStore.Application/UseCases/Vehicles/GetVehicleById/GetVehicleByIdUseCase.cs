using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Vehicles.GetVehicleById;

public class GetVehicleByIdUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetVehicleByIdUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Vehicle> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _vehicleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vehicle with id '{id}' was not found.");
    }
}
