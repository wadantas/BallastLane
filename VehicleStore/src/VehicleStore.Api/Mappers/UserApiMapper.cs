using VehicleStore.Api.Contracts.Responses;
using VehicleStore.Api.Contracts.Signatures;
using VehicleStore.Application.UseCases.Auth.Login;
using VehicleStore.Application.UseCases.Users.CreateUser;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Enums;

namespace VehicleStore.Api.Mappers;

public static class UserApiMapper
{
    public static CreateUserInput ToInput(CreateUserSignature signature) =>
        new(signature.Username, signature.Email, signature.Password, Enum.Parse<UserRole>(signature.Role, true));

    public static LoginInput ToInput(LoginSignature signature) =>
        new(signature.Username, signature.Password);

    public static CreateUserResponse ToResponse(CreateUserOutput output) =>
        new() { Id = output.Id };

    public static UserResponse ToResponse(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        Role = user.Role.ToString(),
        CreatedAt = user.CreatedAt
    };

    public static LoginResponse ToResponse(LoginOutput output) => new()
    {
        Token = output.Token,
        UserId = output.UserId,
        Username = output.Username,
        Role = output.Role.ToString()
    };
}
