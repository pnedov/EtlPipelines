using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

[Table("customers")]
[Index(nameof(FirstName), nameof(LastName), nameof(CustomerEmail))]
public class Customers
{
    [Column("id", Order = 1, TypeName = "int")]
    [Key]
    public int Id { get; set; }

    [Column("unique_id", Order = 2, TypeName = "uniqueidentifier")]
    public Guid UniqueId { get; set; }

    [Column("fname", Order = 3, TypeName = "nvarchar(32)")]
    public string FirstName { get; set; }

    [Column("lname", Order = 4, TypeName = "nvarchar(32)")]
    public string LastName { get; set; }

    [Column("cust_email", Order = 5, TypeName = "nvarchar(64)")]
    public string CustomerEmail { get; set; }

    [ForeignKey("CustomerId")]
    public List<CustomerTransactions> CustomerTransctions { get; set; }
}

