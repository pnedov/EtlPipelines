using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;

public interface ICustomersRepository
{
    Task<IEnumerable<Customers>> GetCustomersAsync(int skip, int take, CancellationToken token);
    Task<IEnumerable<CustomerTransactions>> GetCustomerTransactionsAsync(Guid customerId, int skip, int take, CancellationToken token);
    Task<int> UpsertCustomerAsync(Customers customer, CancellationToken token);
    Task<int> UpsertTransctionAsync(CustomerTransactions transaction, CancellationToken token);
}

