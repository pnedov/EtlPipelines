using PNedov.IsiMarkets.EtlPipeline.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;

/// <summary>
/// Repository class for managing customer-related data operations.
/// </summary>
public class CustomersRepository : ICustomersRepository
{
    private readonly TransactionsDbContext _context;

    public CustomersRepository(TransactionsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customers>> GetCustomersAsync(int skip, int take, CancellationToken token)
    {
        return await _context.Customers
                       .OrderBy(c => c.LastName)
                       .ThenBy(c => c.FirstName)
                       .Skip(skip)
                       .Take(take)
                       .ToListAsync(token);
    }

    public async Task<IEnumerable<CustomerTransactions>> GetCustomerTransactionsAsync(Guid customerId, int skip, int take, CancellationToken token)
    {
        return await _context.CustomerTransactions
                       .Where(t => t.UniqueId == customerId)
                       .OrderBy(t => t.Timestamp)
                       .Skip(skip)
                       .Take(take)
                       .ToListAsync(token);
    }

    /// <summary>
    /// Inserts or updates a customer
    /// </summary>
    /// <param name="customer">The customer to upsert.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> UpsertCustomerAsync(Customers customer, CancellationToken cancellationToken)
    {
        var commandText = "";
        var parameters = new List<SqlParameter>();
        var outputParameter = new SqlParameter();

        if (customer != null && customer.UniqueId != Guid.Empty)
        {
            commandText = "EXEC sp_updatecustomer @unique_id, @fname, @lname, @updated_id OUTPUT";
            parameters.AddRange(new[]
            {
                new SqlParameter("@unique_id", customer.UniqueId),
                new SqlParameter("@fname", customer.FirstName),
                new SqlParameter("@lname", customer.LastName),
            });
            outputParameter = new SqlParameter("@updated_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(outputParameter);
            await _context.Database.ExecuteSqlRawAsync(commandText, parameters.ToArray(), cancellationToken);

            if (outputParameter.Value == DBNull.Value)
            {
                commandText = "EXEC sp_addnewcustomer @unique_id, @fname, @lname, @cust_email, @new_id OUTPUT";
                parameters.AddRange(new[]
                {
                    new SqlParameter("@cust_email", SqlDbType.NVarChar, 64) { Value = customer.CustomerEmail, Direction = ParameterDirection.Input }
                });
                outputParameter = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameters.Add(outputParameter);
                await _context.Database.ExecuteSqlRawAsync(commandText, parameters.ToArray(), cancellationToken);
            }
        }

        return (int)outputParameter.Value;
    }

    /// <summary>
    /// Inserts / updates a customer transaction
    /// </summary>
    /// <param name="transaction">The customer transaction to upsert.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> UpsertTransctionAsync(CustomerTransactions transaction, CancellationToken cancellationToken)
    {
        var commandText = "";
        var parameters = new List<SqlParameter>();
        var outputParameter = new SqlParameter();

        if (transaction != null && transaction.UniqueId != Guid.Empty)
        {
           commandText = @"EXEC sp_updatetransaction @unique_id, @unitprice, @quantity, 
                                              @discount, @total_price, @timestamp,
                                              @location, @customers_id, @products_id,
                                              @payment_methods_id, @transction_statuses_id, @updated_id OUTPUT";
           parameters.AddRange(new[]
           {
                new SqlParameter("@unique_id", transaction.UniqueId),
                new SqlParameter("@unitprice", transaction.UnitPrice),
                new SqlParameter("@quantity", transaction.Quantity),
                new SqlParameter("@discount", transaction.Discount),
                new SqlParameter("@total_price", transaction.TotalPrice),
                new SqlParameter("@timestamp", transaction.Timestamp),
                new SqlParameter("@location", transaction.Location),
                new SqlParameter("@customers_id", transaction.CustomerId),
                new SqlParameter("@products_id", transaction.ProductsId),
                new SqlParameter("@payment_methods_id", transaction.PaymentMethodsId),
                new SqlParameter("@transction_statuses_id", transaction.TransctionStatusesId)
            });
            outputParameter = new SqlParameter("@updated_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            parameters.Add(outputParameter);
            await _context.Database.ExecuteSqlRawAsync(commandText, parameters.ToArray(), cancellationToken);

            if (outputParameter.Value == DBNull.Value)
            {
                commandText = @"sp_addnewtransaction @unique_id, @unitprice, @quantity,
                                                    @timestamp, @discount, @total_price,
                                                    @location, @products_id, @customers_id,
                                                    @payment_methods_id, @transction_statuses_id, @new_id OUTPUT";

                outputParameter = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                parameters.Add(outputParameter);
                await _context.Database.ExecuteSqlRawAsync(commandText, parameters.ToArray(), cancellationToken);
            }
        }

        return (int)outputParameter.Value;
    }
}



