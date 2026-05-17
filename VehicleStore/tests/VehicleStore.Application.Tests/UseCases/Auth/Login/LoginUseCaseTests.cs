using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Auth.Login;
using VehicleStore.Domain.Enums;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Auth.Login;

public class LoginUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ITokenService> _tokenService = new();

    [Fact]
    public async Task ExecuteAsync_WhenCredentialsAreValid_ReturnsLoginOutput()
    {
        var user = TestData.CreateUser(username: "johndoe", role: UserRole.Admin);
        var input = new LoginInput("  johndoe  ", "secret");

        _userRepository.Setup(r => r.GetByUsernameAsync("johndoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("secret", user.PasswordHash)).Returns(true);
        _tokenService.Setup(t => t.GenerateToken(user)).Returns("jwt-token");

        var sut = new LoginUseCase(_userRepository.Object, _passwordHasher.Object, _tokenService.Object);

        var result = await sut.ExecuteAsync(input);

        result.Token.Should().Be("jwt-token");
        result.UserId.Should().Be(user.Id);
        result.Username.Should().Be("johndoe");
        result.Role.Should().Be(UserRole.Admin);
        _tokenService.Verify(t => t.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserNotFound_ThrowsBusinessException()
    {
        var input = new LoginInput("unknown", "secret");
        _userRepository.Setup(r => r.GetByUsernameAsync("unknown", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        var sut = new LoginUseCase(_userRepository.Object, _passwordHasher.Object, _tokenService.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Invalid username or password.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsInvalid_ThrowsBusinessException()
    {
        var user = TestData.CreateUser();
        var input = new LoginInput(user.Username, "wrong-password");

        _userRepository.Setup(r => r.GetByUsernameAsync(user.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(input.Password, user.PasswordHash)).Returns(false);

        var sut = new LoginUseCase(_userRepository.Object, _passwordHasher.Object, _tokenService.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Invalid username or password.");
        _tokenService.Verify(t => t.GenerateToken(It.IsAny<Domain.Entities.User>()), Times.Never);
    }
}
