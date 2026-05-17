using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Users.CreateUser;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateUserOutput> ExecuteAsync(
        CreateUserInput input,
        CancellationToken cancellationToken = default)
    {
        var existingUsername = await _userRepository.GetByUsernameAsync(input.Username, cancellationToken);
        if (existingUsername is not null)
            throw new ConflictException($"Username '{input.Username}' is already taken.");

        var existingEmail = await _userRepository.GetByEmailAsync(input.Email, cancellationToken);
        if (existingEmail is not null)
            throw new ConflictException($"Email '{input.Email}' is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = input.Username.Trim(),
            Email = input.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(input.Password),
            Role = input.Role,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _userRepository.CreateAsync(user, cancellationToken);
        return new CreateUserOutput(id);
    }
}
