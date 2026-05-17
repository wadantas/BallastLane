using Microsoft.Data.SqlClient;
using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;
using VehicleStore.Infrastructure.Data;
using VehicleStore.Infrastructure.Data.Mappers;
using VehicleStore.Infrastructure.Data.Records;

namespace VehicleStore.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public UserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Username, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Id = @Id
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return UserMapper.ToDomain(MapUser(reader));
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Username, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Username = @Username
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Username", username);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return UserMapper.ToDomain(MapUser(reader));
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Username, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Email = @Email
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return UserMapper.ToDomain(MapUser(reader));
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, Username, Email, PasswordHash, Role, CreatedAt
            FROM Users
            ORDER BY CreatedAt DESC
            """;

        var users = new List<User>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
            users.Add(UserMapper.ToDomain(MapUser(reader)));

        return users;
    }

    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Users (Id, Username, Email, PasswordHash, Role, CreatedAt)
            VALUES (@Id, @Username, @Email, @PasswordHash, @Role, @CreatedAt)
            """;

        var record = UserMapper.ToRecord(user);

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", record.Id);
        command.Parameters.AddWithValue("@Username", record.Username);
        command.Parameters.AddWithValue("@Email", record.Email);
        command.Parameters.AddWithValue("@PasswordHash", record.PasswordHash);
        command.Parameters.AddWithValue("@Role", record.Role);
        command.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);

        await command.ExecuteNonQueryAsync(cancellationToken);
        return record.Id;
    }

    private static UserRecord MapUser(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        Username = reader.GetString(reader.GetOrdinal("Username")),
        Email = reader.GetString(reader.GetOrdinal("Email")),
        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
        Role = reader.GetString(reader.GetOrdinal("Role")),
        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
    };
}
