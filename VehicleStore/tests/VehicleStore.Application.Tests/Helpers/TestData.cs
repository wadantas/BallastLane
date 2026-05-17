using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Enums;

namespace VehicleStore.Application.Tests.Helpers;

internal static class TestData
{
    public static User CreateUser(
        Guid? id = null,
        string username = "johndoe",
        string email = "john@example.com",
        string passwordHash = "hashed-password",
        UserRole role = UserRole.User)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Vehicle CreateVehicle(
        Guid? id = null,
        string plateNumber = "ABC1234",
        bool isSold = false)
    {
        return new Vehicle
        {
            Id = id ?? Guid.NewGuid(),
            PlateNumber = plateNumber,
            Document = "12345678901",
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2024,
            Price = 85000m,
            IsSold = isSold,
            CreatedAt = DateTime.UtcNow
        };
    }
}
