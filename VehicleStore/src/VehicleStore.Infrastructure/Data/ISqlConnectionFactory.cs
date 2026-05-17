using Microsoft.Data.SqlClient;

namespace VehicleStore.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
