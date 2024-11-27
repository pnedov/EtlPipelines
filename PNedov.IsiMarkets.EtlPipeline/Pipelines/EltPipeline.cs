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
    public readonly string Id;
    private readonly IEnumerable<IExtractor> _extractors;
    private readonly IEnumerable<ITransformer> _transformers;
    private readonly IProductsRepository _productsRepository;
    private readonly ICustomersRepository _customersRepository;
    private readonly ILogger<EltPipeline> _logger;

    public EltPipeline(
        IEnumerable<IExtractor> extractors,
        IEnumerable<ITransformer> transformers,
        IProductsRepository productsRepository,
        ICustomersRepository customersRepository,
        ILogger<EltPipeline> logger)
    {
        Id = Guid.NewGuid().ToString();
        _extractors = extractors;
        _transformers = transformers;
        _productsRepository = productsRepository;
        _customersRepository = customersRepository;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        foreach (var extractor in _extractors)
        {
            try
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

                    var product = new Products
                    {
                        UniqueId = Guid.Parse(record.ProductId),
                        Name = record.ProductName,
                        Description = record.ProductDescription
                    };
                    var productsId = await _productsRepository.UpsertProductAsync(product, cancellationToken);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the ETL pipeline.");
            }
        }
    }
}

