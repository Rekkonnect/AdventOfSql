using Microsoft.Data.SqlClient;

namespace AdventOfSql.Infrastructure;

public static class ConnectionHelpers
{
    public static SqlConnection CreateConnection()
    {
        const string connectionString = """
            Server=.;
            Trusted_Connection=True;
            TrustServerCertificate=True;
            """;
        return new(connectionString);
    }

    public static async Task<SqlConnection> CreateOpenConnection()
    {
        var connection = CreateConnection();
        await connection.OpenAsync();
        return connection;
    }
}
