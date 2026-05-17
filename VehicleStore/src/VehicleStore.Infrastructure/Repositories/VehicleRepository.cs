using Microsoft.Data.SqlClient;
using VehicleStore.Application.Interfaces;
using VehicleStore.Domain.Entities;
using VehicleStore.Infrastructure.Data;
using VehicleStore.Infrastructure.Data.Mappers;
using VehicleStore.Infrastructure.Data.Records;

namespace VehicleStore.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public VehicleRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, PlateNumber, Document, Brand, Model, Year, Price, IsSold, CreatedAt, UpdatedAt
            FROM Vehicles
            WHERE Id = @Id
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return VehicleMapper.ToDomain(MapVehicle(reader));
    }

    public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, PlateNumber, Document, Brand, Model, Year, Price, IsSold, CreatedAt, UpdatedAt
            FROM Vehicles
            WHERE PlateNumber = @PlateNumber
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@PlateNumber", plateNumber.Trim().ToUpperInvariant());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        return VehicleMapper.ToDomain(MapVehicle(reader));
    }

    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id, PlateNumber, Document, Brand, Model, Year, Price, IsSold, CreatedAt, UpdatedAt
            FROM Vehicles
            ORDER BY CreatedAt DESC
            """;

        var vehicles = new List<Vehicle>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
            vehicles.Add(VehicleMapper.ToDomain(MapVehicle(reader)));

        return vehicles;
    }

    public async Task<Guid> CreateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Vehicles (Id, PlateNumber, Document, Brand, Model, Year, Price, IsSold, CreatedAt, UpdatedAt)
            VALUES (@Id, @PlateNumber, @Document, @Brand, @Model, @Year, @Price, @IsSold, @CreatedAt, @UpdatedAt)
            """;

        var record = VehicleMapper.ToRecord(vehicle);

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        AddVehicleParameters(command, record);

        await command.ExecuteNonQueryAsync(cancellationToken);
        return record.Id;
    }

    public async Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Vehicles
            SET PlateNumber = @PlateNumber,
                Document = @Document,
                Brand = @Brand,
                Model = @Model,
                Year = @Year,
                Price = @Price,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id
            """;

        var record = VehicleMapper.ToRecord(vehicle);

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", record.Id);
        command.Parameters.AddWithValue("@PlateNumber", record.PlateNumber);
        command.Parameters.AddWithValue("@Document", record.Document);
        command.Parameters.AddWithValue("@Brand", record.Brand);
        command.Parameters.AddWithValue("@Model", record.Model);
        command.Parameters.AddWithValue("@Year", record.Year);
        command.Parameters.AddWithValue("@Price", record.Price);
        command.Parameters.AddWithValue("@UpdatedAt", (object?)record.UpdatedAt ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM Vehicles WHERE Id = @Id";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task MarkAsSoldAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Vehicles
            SET IsSold = 1, UpdatedAt = @UpdatedAt
            WHERE Id = @Id
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static VehicleRecord MapVehicle(SqlDataReader reader) => new()
    {
        Id = reader.GetGuid(reader.GetOrdinal("Id")),
        PlateNumber = reader.GetString(reader.GetOrdinal("PlateNumber")),
        Document = reader.GetString(reader.GetOrdinal("Document")),
        Brand = reader.GetString(reader.GetOrdinal("Brand")),
        Model = reader.GetString(reader.GetOrdinal("Model")),
        Year = reader.GetInt32(reader.GetOrdinal("Year")),
        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
        IsSold = reader.GetBoolean(reader.GetOrdinal("IsSold")),
        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
            ? null
            : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
    };

    private static void AddVehicleParameters(SqlCommand command, VehicleRecord record)
    {
        command.Parameters.AddWithValue("@Id", record.Id);
        command.Parameters.AddWithValue("@PlateNumber", record.PlateNumber);
        command.Parameters.AddWithValue("@Document", record.Document);
        command.Parameters.AddWithValue("@Brand", record.Brand);
        command.Parameters.AddWithValue("@Model", record.Model);
        command.Parameters.AddWithValue("@Year", record.Year);
        command.Parameters.AddWithValue("@Price", record.Price);
        command.Parameters.AddWithValue("@IsSold", record.IsSold);
        command.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);
        command.Parameters.AddWithValue("@UpdatedAt", (object?)record.UpdatedAt ?? DBNull.Value);
    }
}
