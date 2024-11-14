using Microsoft.EntityFrameworkCore;
using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;


/// <summary>
/// Custom DbContext class for interacting with the TransactionsDB database.
/// </summary>
public class TransactionsDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public TransactionsDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        object value = optionsBuilder.UseSqlServer(_configuration.GetConnectionString("TransactionsDB"), option =>
        {
            option.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName);
        });

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Products>(e =>
        {
            e.HasKey(k => k.Id);
        });

        modelBuilder.Entity<Customers>(e =>
        {
            e.HasKey(k => k.Id);
        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Products> Products { get; set; }
    public DbSet<Customers> Customers { get; set; }
    public DbSet<CustomerTransactions> CustomerTransactions { get; set; }
    public DbSet<PaymentMethodTypes> PaymentMethodTypes { get; set; }
    public DbSet<TransctionStatuses> TransctionStatuses { get; set; }
    public DbSet<SystemConfigurations> SystemConfigurations { get; set; }
}
