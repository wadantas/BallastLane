using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Application.UseCases.Auth.Login;

public sealed class LoginUseCase(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
{
    public async Task<LoginOutput> ExecuteAsync(LoginInput input, CancellationToken cancellationToken = default)
    {
        var username = input.Username.Trim();
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user is null || !passwordHasher.Verify(input.Password, user.PasswordHash))
            throw new BusinessException("Invalid username or password.");

        var token = tokenService.GenerateToken(user);

        return new LoginOutput(token, user.Id, user.Name, user.Username);
    }
}
