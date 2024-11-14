using PNedov.IsiMarkets.EtlPipeline.APIExtractors;
using PNedov.IsiMarkets.EtlPipeline.Models;
using PNedov.IsiMarkets.EtlPipeline.Repositories;
using PNedov.IsiMarkets.EtlPipeline.Transormers;

namespace PNedov.IsiMarkets.EtlPipeline.ETLPipelines;

/// <summary>
/// Represents an ETL (Extract, Transform, Load) pipeline that orchestrates the extraction data
/// </summary>
public class EltPipeline
{
    private IEnumerable<IExtractor> _extractors { get; set; }
    private IEnumerable<ITransformer> _transformers { get; set; }
    private readonly IProductsRepository _productsRepository;
    private readonly ICustomersRepository _customersRepository;

    public EltPipeline(
        IEnumerable<IExtractor> extractors,
        IEnumerable<ITransformer> transformers,
        IProductsRepository productsRepository,
        ICustomersRepository customersRepository)
    {
        _extractors = extractors;
        _transformers = transformers;
        _productsRepository = productsRepository;
        _customersRepository = customersRepository;
    }

    /// <summary>
    /// Executes the ETL pipelines
    /// </summary>
    /// <param name="cancellationToken">.</param>
    /// <returns></returns>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        foreach (var extractor in _extractors)
        {
            var rawData = await extractor.ExtractDataAsync(cancellationToken);
            foreach (var record in rawData)
            {
                foreach (var transformer in _transformers)
                {
                    transformer.Validate([record]);
                    transformer.ApplyBusinessRule([record]);
                    transformer.Format([record]);
                }

                // Insert / Update logic for Products
                var product = new Products
                {
                    UniqueId = Guid.Parse(record.ProductId),
                    Name = record.ProductName,
                    Description = record.ProductDescription
                };
                var productsId = await _productsRepository.UpsertProductAsync(product, cancellationToken);

                // Insert / Update logic for Customers
                var customer = new Customers
                {
                    UniqueId = Guid.Parse(record.CustomerId),
                    FirstName = record.CustomerName.Split(' ')[0],
                    LastName = record.CustomerName.Contains(' ') ? record.CustomerName.Split(' ')[1] : string.Empty,
                    CustomerEmail = record.CustomerEmail
                };
                var customersId = await _customersRepository.UpsertCustomerAsync(customer, cancellationToken);

                var transaction = new CustomerTransactions
                {
                    UniqueId = Guid.Parse(record.CustomerTransactionId),
                    CustomerId = customersId,
                    ProductsId = productsId,
                    Quantity = decimal.Parse(record.Quantity),
                    UnitPrice = decimal.Parse(record.UnitPrice),
                    Discount = decimal.Parse(record.Discount),
                    TotalPrice = decimal.Parse(record.TotalPrice),
                    Location = record.Location,
                    Timestamp = DateTime.Parse(record.CustomerTransactionTimestamp),
                    PaymentMethodsId = (int)Enum.Parse(typeof(PaymentMethods), record.PaymentMethod),
                    TransctionStatusesId = (int)Enum.Parse(typeof(TransactionStates), record.Status)
                };

                await _customersRepository.UpsertTransctionAsync(transaction, cancellationToken);
            }
        }
    }
}

