using Microsoft.Data.Sqlite;
using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.APIExtractors;

/// <summary>
/// Extracts data from a SQLite database.
/// </summary>
public class SqliteExtractor : IExtractor
{
    private readonly IConfiguration _configuration;

    public SqliteExtractor(IConfiguration configuration)
    {
        _configuration = configuration;
;   }

    /// <summary>
    /// Asynchronously extracts raw data records from the SQLite database.
    /// </summary>
    public async Task<IEnumerable<RawDataRecord>> ExtractDataAsync(CancellationToken token)
    {
        var records = new List<RawDataRecord>();

        var configPath = _configuration["ConnectionStrings:TransactionsSQLiteDB"] ?? string.Empty;
        var fullDbPath
            = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath));

        using (var connection = new SqliteConnection($"Data Source={fullDbPath}"))
        {
            await connection.OpenAsync(token);

            using (var command = new SqliteCommand("SELECT * FROM transactions", connection))
            {
                using (var reader = await command.ExecuteReaderAsync(token))
                {
                    while (await reader.ReadAsync(token))
                    {
                        var record = new RawDataRecord
                        {
                            CustomerTransactionId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                            CustomerId = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            ProductId = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            Quantity = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            UnitPrice = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Discount = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                            TotalPrice = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                            PaymentMethod = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                            Location = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                            Status = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                            CustomerTransactionTimestamp = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                            CustomerName = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                            CustomerEmail = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                            ProductName = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                            ProductDescription = reader.IsDBNull(14) ? string.Empty : reader.GetString(14)
                        };
                        records.Add(record);
                    }
                }
            }
        }

        return records;
    }
}
