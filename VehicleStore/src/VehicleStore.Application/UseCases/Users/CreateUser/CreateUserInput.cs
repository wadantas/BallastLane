using VehicleStore.Domain.Enums;

namespace VehicleStore.Application.UseCases.Users.CreateUser;

public record CreateUserInput(
    string Username,
    string Email,
    string Password,
    UserRole Role);
