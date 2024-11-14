using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

// This class represents a raw data record extracted from the API.
public class RawDataRecord
{
    // Unique identifier for the customer transaction.
    public string CustomerTransactionId { get; set; }

    // Unique identifier for the customer.
    public string CustomerId { get; set; }

    // Unique identifier for the product.
    public string ProductId { get; set; }

    // Quantity of the product purchased.
    public string Quantity { get; set; }

    // Unit price of the product.
    public string UnitPrice { get; set; }

    // Discount applied to the transaction.
    public string Discount { get; set; }

    // Total price of the transaction after discount.
    public string TotalPrice { get; set; }

    // Payment method used for the transaction.
    public string PaymentMethod { get; set; }

    // Location where the transaction took place.
    public string Location { get; set; }

    // Status of the transaction.
    public string Status { get; set; }

    // Timestamp of the customer transaction.
    public string CustomerTransactionTimestamp { get; set; }

    // Name of the customer.
    public string CustomerName { get; set; }

    // Email of the customer.
    public string CustomerEmail { get; set; }

    // Name of the product.
    public string ProductName { get; set; }

    // Description of the product.
    public string ProductDescription { get; set; }
}
