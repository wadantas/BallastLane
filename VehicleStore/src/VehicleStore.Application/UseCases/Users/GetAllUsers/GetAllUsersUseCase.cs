using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;

namespace VehicleStore.Application.UseCases.Users.GetAllUsers;

public class GetAllUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<IReadOnlyList<User>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return _userRepository.GetAllAsync(cancellationToken);
    }
}
