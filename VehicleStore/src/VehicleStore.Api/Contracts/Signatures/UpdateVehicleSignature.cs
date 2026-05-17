namespace VehicleStore.Api.Contracts.Signatures;

public class UpdateVehicleSignature
{
    public string PlateNumber { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
}
