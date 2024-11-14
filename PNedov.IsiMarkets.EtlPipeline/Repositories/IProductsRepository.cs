using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;

public interface IProductsRepository
{
    Task<IEnumerable<Products>> GetProductsAsync(int skip, int take, CancellationToken token);
    Task<int> UpsertProductAsync(Products product, CancellationToken token);
}

