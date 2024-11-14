using PNedov.IsiMarkets.EtlPipeline.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;

/// <summary>
/// Handles operations related to system configurations and database initialization.
/// </summary>
/// <param name="_context"></param>
public class SystemRepository(TransactionsDbContext _context) : ISystemRepository

{
    public readonly string DbTableName = "customers_transactions";

    public async Task<SystemConfigurations?> GetSystemConfiguration(CancellationToken token)
    {
        return await _context.SystemConfigurations.FirstOrDefaultAsync(token);
    }

    /// <summary>
    /// Initializes the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task InitializeDatabase(CancellationToken cancellationToken)
    {
        _context.Database.EnsureCreated();
        if (!IsTableExistsAsync(cancellationToken))
        {
            await CreateTables(cancellationToken);
        }

        var defaultConfig = new SystemConfigurations
        {
            UniqueId = Guid.NewGuid()
        };

        _context.SystemConfigurations.Add(defaultConfig);
        _context.SaveChanges();
    }

    /// <summary>
    /// Inserts / updates the system configuration
    /// </summary>
    /// <param name="newConfig">The new system configuration to upsert.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task UpsertSystemConfiguration(SystemConfigurations newConfig, CancellationToken cancellationToken)
    {
        var existingConfig = _context.SystemConfigurations.Where(x => x.Id == newConfig.Id).FirstOrDefaultAsync(cancellationToken);
        if (existingConfig != null)
        {
            _context.SystemConfigurations.Update(newConfig);
        }
        else
        {
            _context.SystemConfigurations.Add(newConfig);
        }

        await _context.SaveChangesAsync();
    }

    private bool IsTableExistsAsync(CancellationToken cancellationToken)
    {
        var query = "SELECT COUNT(object_id) as count FROM [sys].[objects] WHERE [name] = @table_name";
        using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@table_name", DbTableName);
                connection.Open();
                var count = (int)command.ExecuteScalar();
                connection.Close();

                return count > 0;
            }
        }
    }

    private async Task CreateTables(CancellationToken token)
    {
        var dbCreator = _context.GetService<IRelationalDatabaseCreator>();
        await dbCreator.CreateTablesAsync(token);
    }
}

