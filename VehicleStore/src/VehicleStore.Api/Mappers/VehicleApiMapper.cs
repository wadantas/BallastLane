using VehicleStore.Api.Contracts.Responses;
using VehicleStore.Api.Contracts.Signatures;
using VehicleStore.Application.UseCases.Vehicles.RegisterVehicle;
using VehicleStore.Application.UseCases.Vehicles.UpdateVehicle;
using VehicleStore.Domain.Entities;

namespace VehicleStore.Api.Mappers;

public static class VehicleApiMapper
{
    public static RegisterVehicleInput ToInput(RegisterVehicleSignature signature) =>
        new(signature.PlateNumber, signature.Document, signature.Brand, signature.Model, signature.Year, signature.Price);

    public static UpdateVehicleInput ToInput(Guid id, UpdateVehicleSignature signature) =>
        new(id, signature.PlateNumber, signature.Document, signature.Brand, signature.Model, signature.Year, signature.Price);

    public static VehicleResponse ToResponse(Vehicle vehicle) => new()
    {
        Id = vehicle.Id,
        PlateNumber = vehicle.PlateNumber,
        Document = vehicle.Document,
        Brand = vehicle.Brand,
        Model = vehicle.Model,
        Year = vehicle.Year,
        Price = vehicle.Price,
        IsSold = vehicle.IsSold,
        CreatedAt = vehicle.CreatedAt,
        UpdatedAt = vehicle.UpdatedAt
    };

    public static RegisterVehicleResponse ToResponse(RegisterVehicleOutput output) =>
        new() { Id = output.Id };
}
