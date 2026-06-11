using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ExamScheduleService.Infrastructure.Persistence;

/// <summary>
/// Creates SQL Server connections using the connection string supplied by dependency injection.
/// </summary>
public sealed class SqlHelper
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlHelper"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when the database connection string is missing.</exception>
    public SqlHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("StudentPortalDb") ?? throw new InvalidOperationException("Connection string 'StudentPortalDb' is not configured.");
    }

    /// <summary>
    /// Creates a new unopened SQL connection.
    /// </summary>
    /// <returns>A database connection instance.</returns>
    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
