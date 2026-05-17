namespace VehicleStore.Application.UseCases.Vehicles.RegisterVehicle;

public record RegisterVehicleInput(
    string PlateNumber,
    string Document,
    string Brand,
    string Model,
    int Year,
    decimal Price);
