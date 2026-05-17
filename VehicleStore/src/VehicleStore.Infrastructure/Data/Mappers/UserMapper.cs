using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Enums;
using VehicleStore.Infrastructure.Data.Records;

namespace VehicleStore.Infrastructure.Data.Mappers;

internal static class UserMapper
{
    public static User ToDomain(UserRecord record) => new()
    {
        Id = record.Id,
        Username = record.Username,
        Email = record.Email,
        PasswordHash = record.PasswordHash,
        Role = Enum.Parse<UserRole>(record.Role, ignoreCase: true),
        CreatedAt = record.CreatedAt
    };

    public static UserRecord ToRecord(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        PasswordHash = user.PasswordHash,
        Role = user.Role.ToString(),
        CreatedAt = user.CreatedAt
    };
}
