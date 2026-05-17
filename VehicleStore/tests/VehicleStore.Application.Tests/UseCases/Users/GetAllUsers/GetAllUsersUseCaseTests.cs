using FluentAssertions;
using Moq;
using VehicleStore.Application.Interfaces;
using VehicleStore.Application.Tests.Helpers;
using VehicleStore.Application.UseCases.Users.GetAllUsers;
using VehicleStore.Domain.Entities;

namespace VehicleStore.Application.Tests.UseCases.Users.GetAllUsers;

public class GetAllUsersUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepository = new();

    [Fact]
    public async Task ExecuteAsync_ReturnsUsersFromRepository()
    {
        IReadOnlyList<User> users = [TestData.CreateUser(), TestData.CreateUser(username: "jane")];
        _userRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var sut = new GetAllUsersUseCase(_userRepository.Object);

        var result = await sut.ExecuteAsync();

        result.Should().BeEquivalentTo(users);
        _userRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
