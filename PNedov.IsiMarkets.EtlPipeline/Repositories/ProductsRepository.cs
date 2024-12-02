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

    /// <summary>
    /// Retrieves a list of products from the database asynchronously.
    /// </summary>
    /// <param name="skip">The number of products to skip.</param>
    /// <param name="take">The number of products to retrieve.</param>
    /// <param name="token">Token to monitor for cancellation requests.</param>
    /// <returns>Asynchronus task result contains an IEnumerable of Products.</returns>
    public async Task<IEnumerable<Products>> GetProductsAsync(int skip, int take, CancellationToken token)
    {
        var products = new List<Products>();
        var commandText = "EXEC sp_getproducts @skip, @take";
        var parameters = new[]
        {
            new SqlParameter("@skip", skip),
            new SqlParameter("@take", take)
        };

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = commandText;
            command.Parameters.AddRange(parameters);

            await _context.Database.OpenConnectionAsync(token);

            using (var reader = await command.ExecuteReaderAsync(token))
            {
                while (await reader.ReadAsync(token))
                {
                    var product = new Products
                    {
                        UniqueId = reader.IsDBNull(0) ? Guid.Empty : reader.GetGuid(reader.GetOrdinal("unique_id")),
                        Name = reader.IsDBNull(reader.GetOrdinal("iname")) ? string.Empty : reader.GetString(reader.GetOrdinal("iname")),
                        Description = reader.IsDBNull(reader.GetOrdinal("idesc")) ? string.Empty : reader.GetString(reader.GetOrdinal("idesc"))
                    };
                    products.Add(product);
                }
            }

            await _context.Database.CloseConnectionAsync();
        }

        return products;
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

