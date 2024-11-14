using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PNedov.IsiMarkets.EtlPipeline.FakeApi.Models;

namespace PNedov.IsiMarkets.EtlPipeline.FakeApi.Controllers;

/// <summary>
/// Controller for handling fake API requests related to customer transactions.
/// </summary>
[ApiController]
[Route("[controller]")]
public class FakeApiController : ControllerBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeApiController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration settings.</param>
    public FakeApiController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the customer transactions asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of customer transactions.</returns>
    [HttpGet("customertransactions")]
    public async Task<IActionResult> GetCustomerTransactionsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var newRawDataRecords = new List<RawDataRecord>();
        var jsonFileName = _configuration["FilePathToCustomerTransactionsJson"] ?? string.Empty;
       
        var filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jsonFileName));
        using (var reader = new StreamReader(filePath))
        {
            var content = await reader.ReadToEndAsync();
            var json = JArray.Parse(content);

            if (json.Count == 0)
            {
                return Ok(newRawDataRecords);
            }

            newRawDataRecords = json.Select(data => new RawDataRecord
            {
                CustomerTransactionId = (string)data["CustomerTransactionId"],
                CustomerId = (string)data["CustomerId"],
                ProductId = (string)data["ProductId"],
                Quantity = (string)data["Quantity"],
                UnitPrice = (string)data["UnitPrice"],
                Discount = (string)data["Discount"],
                TotalPrice = (string)data["TotalPrice"],
                PaymentMethod = (string)data["PaymentMethod"],
                Location = (string)data["Location"],
                Status = (string)data["Status"],
                CustomerTransactionTimestamp = (string)data["CustomerTransactionTimestamp"],
                CustomerName = (string)data["CustomerName"],
                CustomerEmail = (string)data["CustomerEmail"],
                ProductName = (string)data["ProductName"],
                ProductDescription = (string)data["ProductDescription"]
            }).ToList();
        }

        return Ok(newRawDataRecords);
    }
}

