using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

[Table("customers_transactions")]
public class CustomerTransactions
{
    [Column("id", Order = 1, TypeName = "int")]
    [Key]
    public int Id { get; set; }

    [Column("unique_id", Order = 2, TypeName = "uniqueidentifier")]
    public Guid UniqueId { get; set; }

    [Column("quantity", Order = 3, TypeName = "decimal(19, 4)")]
    public decimal Quantity { get; set; }

    [Column("unitprice", Order = 4, TypeName = "decimal(19, 4)")]
    public decimal UnitPrice { get; set; }

    [Column("discount", Order = 5, TypeName = "decimal(19, 4)")]
    public decimal Discount { get; set; }

    [Column("total_price", Order = 6, TypeName = "decimal(19, 4)")]
    public decimal TotalPrice { get; set; }

    [Column("location", Order = 7, TypeName = "nvarchar(128)")]
    public string Location { get; set; }

    [Column("timestamp", Order = 8, TypeName = "datetime2")]
    public DateTime Timestamp { get; set; }

    [Column("customers_id", Order = 9, TypeName = "int")]
    public int CustomerId { get; set; }

    [Column("products_id", Order = 10, TypeName = "int")]
    public int ProductsId { get; set; }

    [Column("payment_methods_id", Order = 11, TypeName = "int")]
    public int PaymentMethodsId { get; set; }

    [Column("transction_statuses_id", Order = 12, TypeName = "int")]
    public int TransctionStatusesId { get; set; }

    public Customers Customers { get; set; } = new();
    public Products Products { get; set; } = new();
    public PaymentMethodTypes PaymentMethods { get; set; } = new();
    public TransctionStatuses Statuses { get; set; } = new();
}

