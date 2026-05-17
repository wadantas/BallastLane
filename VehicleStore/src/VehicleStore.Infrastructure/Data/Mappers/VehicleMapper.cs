using VehicleStore.Domain.Entities;
using VehicleStore.Infrastructure.Data.Records;

namespace VehicleStore.Infrastructure.Data.Mappers;

internal static class VehicleMapper
{
    public static Vehicle ToDomain(VehicleRecord record) => new()
    {
        Id = record.Id,
        PlateNumber = record.PlateNumber,
        Document = record.Document,
        Brand = record.Brand,
        Model = record.Model,
        Year = record.Year,
        Price = record.Price,
        IsSold = record.IsSold,
        CreatedAt = record.CreatedAt,
        UpdatedAt = record.UpdatedAt
    };

    public static VehicleRecord ToRecord(Vehicle vehicle) => new()
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
}
