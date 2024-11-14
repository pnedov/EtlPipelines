using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

[Table("products")]
[Index(nameof(Name))]
public class Products
{
    [Key]
    [Column("id", Order = 1, TypeName = "int")]
    public int Id { get; set; }

    [Column("unique_id", Order = 2, TypeName = "uniqueidentifier")]
    public Guid UniqueId { get; set; }

    [Column("iname", Order = 3, TypeName = "nvarchar(64)")]
    public string Name { get; set; }

    [Column("idesc", Order = 4, TypeName = "nvarchar(max)")]
    public string Description { get; set; }

    [ForeignKey("ProductsId")]
    public ICollection<CustomerTransactions> CustomerTransctions { get; set; } = new List<CustomerTransactions>();
}

