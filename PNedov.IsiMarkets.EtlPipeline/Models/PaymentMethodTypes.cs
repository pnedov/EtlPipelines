using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

[Table("payment_methods")]
public class PaymentMethodTypes
{
    [Column("id", Order = 1)]
    [Key]
    public int id { get; set; }

    [Column("iname", Order = 2, TypeName = "nvarchar(24)")]
    public string Type { get; set; }

    [ForeignKey("PaymentMethodsId")]
    public ICollection<CustomerTransactions> CustomerTransctions { get; set; } = new List<CustomerTransactions>();
}

public enum PaymentMethods
{
    Cash = 1,
    CreditCard = 2,
    DebitCard = 3,
    DigitalWallet = 4,
    Voucher = 5
}



