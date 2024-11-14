using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Transormers;

/// <summary>
/// Responsible for transforming timestamps in raw data
/// </summary>
public class ConvertTimestampTransformer : ITransformer
{
    private readonly ILogger<ITransformer> _logger;

    public ConvertTimestampTransformer(ILogger<ITransformer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Applies business rules to a collection of raw data
    /// </summary>
    /// <param name="records"></param>
    public IEnumerable<RawDataRecord> ApplyBusinessRule(IEnumerable<RawDataRecord> records)
    {
        foreach (var record in records)
        {
            if (DateTime.TryParse(record.CustomerTransactionTimestamp, out DateTime parsedDate))
            {
                record.CustomerTransactionTimestamp = parsedDate.ToUniversalTime().ToString("o");
            }
            else
            {
                _logger.LogError($"Invalid timestamp format: {record.CustomerTransactionTimestamp}");
            }
        }

        return records;
    }

    /// <summary>
    /// Formats the CustomerTransactionTimestamp field
    /// </summary>
    /// <param name="records"></param>
    public IEnumerable<RawDataRecord> Format(IEnumerable<RawDataRecord> records)
    {
        foreach (var record in records)
        {
            if (DateTime.TryParse(record.CustomerTransactionTimestamp, out DateTime parsedDate))
            {
                record.CustomerTransactionTimestamp = parsedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }
            else
            {
                _logger.LogError($"Invalid timestamp format: {record.CustomerTransactionTimestamp}");
            }
        }

        return records;
    }

    /// <summary>
    /// Validates the CustomerTransactionTimestamp field
    /// </summary>
    /// <param name="records"></param>
    public IEnumerable<RawDataRecord> Validate(IEnumerable<RawDataRecord> records)
    {
        foreach (var record in records)
        {
            if (!DateTime.TryParse(record.CustomerTransactionTimestamp, out _))
            {
                _logger.LogError($"Invalid timestamp format: {record.CustomerTransactionTimestamp}");
            }
        }

        return records;
    }
}

