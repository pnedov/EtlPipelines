using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

[Table("transction_statuses")]
public class TransctionStatuses
{
    [Column("id", Order = 1, TypeName = "int")]
    [Key]
    public int Id { get; set; }

    [Column("iname", Order = 2, TypeName = "nvarchar(24)")]
    public string Status { get; set; }

    [ForeignKey("TransctionStatusesId")]
    public ICollection<CustomerTransactions> CustomerTransctions { get; set; } = new List<CustomerTransactions>();
}

public enum TransactionStates
{
    Completed = 1,
    Cancelled = 2,
    Refunded = 3,
    Pending = 4
}
