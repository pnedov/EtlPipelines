using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PNedov.IsiMarkets.EtlPipeline.Models;

[ExcludeFromCodeCoverage]

[Table("system_configurations")]
public class SystemConfigurations
{
    [Key]
    [Column("id", Order = 1, TypeName = "int")]
    public int Id { get; set; }

    [Column("unique_id", Order = 2, TypeName = "uniqueidentifier")]
    public Guid UniqueId { get; set; }
}
