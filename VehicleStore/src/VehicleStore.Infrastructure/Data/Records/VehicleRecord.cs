namespace VehicleStore.Infrastructure.Data.Records;

internal sealed class VehicleRecord
{
    public Guid Id { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public string Document { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int Year { get; init; }
    public decimal Price { get; init; }
    public bool IsSold { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
