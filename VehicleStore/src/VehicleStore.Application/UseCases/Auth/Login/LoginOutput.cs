using VehicleStore.Domain.Enums;

namespace VehicleStore.Application.UseCases.Auth.Login;

public record LoginOutput(string Token, Guid UserId, string Username, UserRole Role);
