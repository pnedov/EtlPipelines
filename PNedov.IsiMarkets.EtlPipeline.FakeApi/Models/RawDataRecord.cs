namespace PNedov.IsiMarkets.EtlPipeline.FakeApi.Models;

/// <summary>
/// Represents a raw data record for customer transactions in the ETL pipeline.
/// </summary>
public class RawDataRecord
{
    /// <summary>
    /// Gets or sets the unique identifier for the customer transaction.
    /// </summary>
    public string CustomerTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the customer.
    /// </summary>
    public string CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public string Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public string UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the transaction.
    /// </summary>
    public string Discount { get; set; }

    /// <summary>
    /// Gets or sets the total price of the transaction.
    /// </summary>
    public string TotalPrice { get; set; }

    /// <summary>
    /// Gets or sets the payment method used for the transaction.
    /// </summary>
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the location where the transaction took place.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets the status of the transaction.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the customer transaction.
    /// </summary>
    public string CustomerTransactionTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer.
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// Gets or sets the email of the customer.
    /// </summary>
    public string CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public string ProductDescription { get; set; }
}

