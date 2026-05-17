using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;

namespace VehicleStore.Application.UseCases.Vehicles.GetAllVehicles;

public class GetAllVehiclesUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetAllVehiclesUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public Task<IReadOnlyList<Vehicle>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _vehicleRepository.GetAllAsync(cancellationToken);
    }
}
