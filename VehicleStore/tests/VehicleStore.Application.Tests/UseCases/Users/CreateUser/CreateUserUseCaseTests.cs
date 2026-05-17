using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Users.CreateUser;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Enums;
using VehicleStore.Domain.Exceptions;

namespace VehicleStore.Application.Tests.UseCases.Users.CreateUser;

public class CreateUserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    [Fact]
    public async Task ExecuteAsync_WhenValid_CreatesUserAndReturnsId()
    {
        var input = new CreateUserInput("  johndoe  ", "  John@Example.COM  ", "secret", UserRole.Admin);
        User? capturedUser = null;

        _userRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasher.Setup(h => h.Hash(input.Password)).Returns("hashed-secret");
        _userRepository.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => capturedUser = user)
            .ReturnsAsync((User user, CancellationToken _) => user.Id);

        var sut = new CreateUserUseCase(_userRepository.Object, _passwordHasher.Object);

        var result = await sut.ExecuteAsync(input);

        result.Id.Should().Be(capturedUser!.Id);
        capturedUser.Username.Should().Be("johndoe");
        capturedUser.Email.Should().Be("john@example.com");
        capturedUser.PasswordHash.Should().Be("hashed-secret");
        capturedUser.Role.Should().Be(UserRole.Admin);
        _passwordHasher.Verify(h => h.Hash("secret"), Times.Once);
        _userRepository.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUsernameExists_ThrowsConflictException()
    {
        var input = new CreateUserInput("johndoe", "new@example.com", "secret", UserRole.User);
        _userRepository.Setup(r => r.GetByUsernameAsync(input.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreateUser(username: input.Username));

        var sut = new CreateUserUseCase(_userRepository.Object, _passwordHasher.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Username 'johndoe' is already taken*");
        _userRepository.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailExists_ThrowsConflictException()
    {
        var input = new CreateUserInput("newuser", "taken@example.com", "secret", UserRole.User);
        _userRepository.Setup(r => r.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepository.Setup(r => r.GetByEmailAsync(input.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreateUser(email: input.Email));

        var sut = new CreateUserUseCase(_userRepository.Object, _passwordHasher.Object);

        var act = () => sut.ExecuteAsync(input);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Email 'taken@example.com' is already registered*");
        _userRepository.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
