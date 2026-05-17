using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;
using VehicleStore.Domain.Enums;

namespace VehicleStore.Infrastructure.Data;

public class DevelopmentDataSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DevelopmentDataSeeder> _logger;

    public DevelopmentDataSeeder(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<DevelopmentDataSeeder> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _environment = environment;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_environment.IsDevelopment())
            return;

        var username = _configuration["Seed:AdminUsername"] ?? "admin";
        var existing = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (existing is not null)
            return;

        var password = _configuration["Seed:AdminPassword"] ?? "Admin@123";
        var admin = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = _configuration["Seed:AdminEmail"] ?? "admin@vehiclestore.local",
            PasswordHash = _passwordHasher.Hash(password),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(admin, cancellationToken);
        _logger.LogInformation("Default admin user '{Username}' was created for development.", username);
    }
}
