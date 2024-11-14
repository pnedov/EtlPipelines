using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.APIExtractors;

/// <summary>
/// Extracts data from a SQLite database.
/// </summary>
public class SqliteExtractor : IExtractor
{
    /// <summary>
    /// Asynchronously extracts raw data records from the SQLite database.
    /// </summary>
    public async Task<IEnumerable<RawDataRecord>> ExtractDataAsync(CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
