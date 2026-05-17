using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.UseCases.Auth.Login;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginOutput> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(input.Username.Trim(), cancellationToken)
            ?? throw new BusinessException("Invalid username or password.");

        if (!_passwordHasher.Verify(input.Password, user.PasswordHash))
            throw new BusinessException("Invalid username or password.");

        var token = _tokenService.GenerateToken(user);
        return new LoginOutput(token, user.Id, user.Username, user.Role);
    }
}
