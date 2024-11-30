using PNedov.IsiMarkets.EtlPipeline.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;

/// <summary>
/// Repository class for managing product data in the database.
/// </summary>
public class ProductsRepository : IProductsRepository
{
    private readonly TransactionsDbContext _context;

    public ProductsRepository(TransactionsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Products>> GetProductsAsync(int skip, int take, CancellationToken token)
    {
        return await _context.Products
                       .OrderBy(p => p.Name)
                       .Skip(skip)
                       .Take(take)
                       .ToListAsync(token);
    }

    /// <summary>
    /// Inserts / updates product
    /// </summary>
    /// <param name="product">The product to upsert.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The ID product.</returns>

    public async Task<int> UpsertProductAsync(Products product, CancellationToken cancellationToken)
    {
        var commandText = "";
        var parameters = new List<SqlParameter>();
        var outputParameter = new SqlParameter();

        if (product != null && product.UniqueId != Guid.Empty)
        {
            commandText = "EXEC sp_updateproduct @unique_id, @iname, @idesc, @updated_id OUTPUT";
            parameters.AddRange(new[]
            {
                    new SqlParameter("@unique_id", product.UniqueId),
                    new SqlParameter("@iname", product.Name),
                    new SqlParameter("@idesc", product.Description)
            });
            outputParameter = new SqlParameter("@updated_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(outputParameter);
            await _context.Database.ExecuteSqlRawAsync(commandText, parameters.ToArray(), cancellationToken);

            if (outputParameter.Value == DBNull.Value)
            {
                commandText = "EXEC sp_addnewproduct @unique_id, @iname, @idesc, @new_id OUTPUT";
                outputParameter = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameters.Add(outputParameter);
                await _context.Database.ExecuteSqlRawAsync(commandText, parameters.ToArray(), cancellationToken);
            }
        }

        return (int)outputParameter.Value;
    }
}

