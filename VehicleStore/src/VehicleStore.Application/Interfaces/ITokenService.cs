using VehicleStore.Domain.Entities;

namespace VehicleStore.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
