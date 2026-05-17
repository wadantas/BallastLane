namespace VehicleStore.Api.Contracts.Responses;

public class VehicleResponse
{
    public Guid Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public bool IsSold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
