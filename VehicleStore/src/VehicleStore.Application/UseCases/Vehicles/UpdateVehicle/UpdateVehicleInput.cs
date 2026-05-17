namespace VehicleStore.Application.UseCases.Vehicles.UpdateVehicle;

public record UpdateVehicleInput(
    Guid Id,
    string PlateNumber,
    string Document,
    string Brand,
    string Model,
    int Year,
    decimal Price);
