using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Transormers;

/// <summary>
/// Responsible for remove duplicate records, format string properties, and validate records.
/// </summary>
public class RemoveDuplicatesTransformer : ITransformer
{
    private readonly HashSet<string> _seenRecords = new();
    private readonly ILogger<ITransformer> _logger;

    public RemoveDuplicatesTransformer(ILogger<ITransformer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Applies business rules to remove duplicate records
    /// </summary>
    /// <param name="records">Collection of raw data records.</param>
    /// <exception cref="InvalidOperationException">Thrown when an invalid operation occurs.</exception>
    /// 
    public IEnumerable<RawDataRecord> ApplyBusinessRule(IEnumerable<RawDataRecord> records)
    {
        var uniqueRecords = new HashSet<string>();
        var filteredRecords = new List<RawDataRecord>();

        foreach (var record in records)
        {
            var key = $"{record.CustomerTransactionId}-{record.CustomerId}-{record.ProductId}";
            if (uniqueRecords.Add(key))
            {
                filteredRecords.Add(record);
            }
        }

        return filteredRecords;
    }

    /// <summary>
    /// Formats all string properties
    /// </summary>
    /// <param name="records">Collection of raw data records.</param>
    public IEnumerable<RawDataRecord> Format(IEnumerable<RawDataRecord> records)
    {
        foreach (var record in records)
        {
            record.CustomerTransactionId = record.CustomerTransactionId?.Trim();
            record.CustomerId = record.CustomerId?.Trim();
            record.ProductId = record.ProductId?.Trim();
            record.Quantity = record.Quantity?.Trim();
            record.UnitPrice = record.UnitPrice?.Trim();
            record.Discount = record.Discount?.Trim();
            record.TotalPrice = record.TotalPrice?.Trim();
            record.PaymentMethod = record.PaymentMethod?.Trim();
            record.Location = record.Location?.Trim();
            record.Status = record.Status?.Trim();
            record.CustomerTransactionTimestamp = record.CustomerTransactionTimestamp?.Trim();
            record.CustomerName = record.CustomerName?.Trim();
            record.CustomerEmail = record.CustomerEmail?.Trim();
            record.ProductName = record.ProductName?.Trim();
            record.ProductDescription = record.ProductDescription?.Trim();
        }

        return records;
    }

    /// <summary>
    /// Validates the records for null or empty fields
    /// </summary>
    /// <param name="records">Collection of raw data records.</param>
    /// <exception cref="ArgumentException">Thrown when an argument is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an invalid operation occurs.</exception>
    public IEnumerable<RawDataRecord> Validate(IEnumerable<RawDataRecord> records)
    {
        if (records == null || !records.Any())
        {
            _logger.LogError("Records collection is null or empty.");

            return Enumerable.Empty<RawDataRecord>();
        }

        var validRecords = new List<RawDataRecord>();

        foreach (var record in records)
        {
            if (string.IsNullOrWhiteSpace(record.CustomerTransactionId) ||
                string.IsNullOrWhiteSpace(record.CustomerId) ||
                string.IsNullOrWhiteSpace(record.ProductId))
            {
                _logger.LogError($"One or more fields in the record are null or empty. Removing record with ID: {record.CustomerTransactionId} - {record.CustomerId} - {record.ProductId}");
            }
            else
            {
                validRecords.Add(record);
            }
        }

        return validRecords;
    }
}

