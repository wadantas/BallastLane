using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Vehicles.MarkVehicleAsSold;

public class MarkVehicleAsSoldUseCase
{
    private readonly IVehicleRepository _vehicleRepository;

    public MarkVehicleAsSoldUseCase(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vehicle with id '{id}' was not found.");

        if (vehicle.IsSold)
            throw new BusinessException("Vehicle is already marked as sold.");

        await _vehicleRepository.MarkAsSoldAsync(id, cancellationToken);
    }
}
