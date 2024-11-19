using PNedov.IsiMarkets.EtlPipeline.Models; 

namespace PNedov.IsiMarkets.EtlPipeline.Transormers;

/// <summary>
/// Responsible for applying business rules, formatting, and validating
/// </summary>
public class AmountFilterTransformer : ITransformer
{
    private readonly ILogger<ITransformer> _logger;

    public AmountFilterTransformer(ILogger<ITransformer> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ensure the record has the required fields
    /// </summary>
    /// <param name="record"></param>
    /// <return>Processed raw data records</return>
    public IEnumerable<RawDataRecord> ApplyBusinessRule(IEnumerable<RawDataRecord> records)
    {
        foreach (var record in records)
        {
            if (record == null)
            {
                continue;
            }

            // Example business rule: Apply a discount if the quantity is greater than 10
            if (float.TryParse(record.Quantity, out float quantity) && quantity > 10)
            {
                if (float.TryParse(record.UnitPrice, out float unitPrice))
                {
                    float discountRate = 0.1f; // 10% discount
                    float discountAmount = unitPrice * discountRate;
                    record.Discount = discountAmount.ToString("F2");

                    float totalPrice = (unitPrice - discountAmount) * quantity;
                    record.TotalPrice = totalPrice.ToString("F2");
                }
            }
        }

        return records;
    }

    /// <summary>
    /// Formats the numerical and timestamp values of the given records
    /// </summary>
    /// <param name="records"></param>
    public IEnumerable<RawDataRecord> Format(IEnumerable<RawDataRecord> records)
    {
        if (records == null) return Enumerable.Empty<RawDataRecord>();

        foreach (var record in records)
        {
            record.Quantity = FormatFloat(record.Quantity);
            record.UnitPrice = FormatFloat(record.UnitPrice);
            record.Discount = FormatFloat(record.Discount);
            record.TotalPrice = FormatFloat(record.TotalPrice);
            record.CustomerTransactionTimestamp = FormatTimestamp(record.CustomerTransactionTimestamp);
        }

        return records;
    }

    /// <summary>
    /// Validates the given collection of raw data.
    /// </summary>
    /// <param name="records"></param>
    public IEnumerable<RawDataRecord> Validate(IEnumerable<RawDataRecord> records)
    {
        foreach (var record in records)
        {
            if (record == null)
            {
                continue;
            }

            bool isValid = true;

            isValid &= float.TryParse(record.Quantity, out _);
            isValid &= float.TryParse(record.UnitPrice, out _);
            isValid &= string.IsNullOrEmpty(record.Discount) || float.TryParse(record.Discount, out _);
            isValid &= string.IsNullOrEmpty(record.TotalPrice) || float.TryParse(record.TotalPrice, out _);
            isValid &= DateTime.TryParse(record.CustomerTransactionTimestamp, out _);

            isValid &= !string.IsNullOrEmpty(record.CustomerTransactionId);
            isValid &= !string.IsNullOrEmpty(record.CustomerId);
            isValid &= !string.IsNullOrEmpty(record.ProductId);
            isValid &= !string.IsNullOrEmpty(record.PaymentMethod);
            isValid &= !string.IsNullOrEmpty(record.Location);
            isValid &= !string.IsNullOrEmpty(record.Status);
            isValid &= !string.IsNullOrEmpty(record.CustomerName);
            isValid &= !string.IsNullOrEmpty(record.CustomerEmail);
            isValid &= !string.IsNullOrEmpty(record.ProductName);
            isValid &= !string.IsNullOrEmpty(record.ProductDescription);

            if (!isValid)
            {
                _logger.LogWarning("AmountFilterTransformer.Validate - Invalid record found: {@Record}", record);
            }
        }

        return records;
    }

    private string FormatFloat(string value)
    {
        return float.TryParse(value, out float result) ? result.ToString("F2") : value;
    }

    private string FormatTimestamp(string value)
    {
        return DateTime.TryParse(value, out DateTime result) ? result.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : value;
    }
}

