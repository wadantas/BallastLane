namespace VehicleStore.Api.Contracts.Signatures;

public class CreateUserSignature
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}
