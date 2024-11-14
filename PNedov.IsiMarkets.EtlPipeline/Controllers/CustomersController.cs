using Microsoft.AspNetCore.Mvc;
using PNedov.IsiMarkets.EtlPipeline.Repositories;
using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Controllers;

/// <summary>
/// The CustomersController class provides API endpoints for managing customer data and their transactions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomersRepository _customerRepository;

    public CustomersController(ICustomersRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Retrieving customers with pagination
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCustomers(int skip, int take, CancellationToken token)
    {
        var customers = await _customerRepository.GetCustomersAsync(skip, take, token);

        return Ok(customers);
    }

    /// <summary>
    /// Retrieves a paginated list of transactions for a specific customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="skip">The number of transactions to skip.</param>
    /// <param name="take">The number of transactions to take.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A list of customer transactions.</returns>
    [HttpGet("{customerId}/transactions")]
    public async Task<IActionResult> GetCustomerTransactions(Guid customerId, int skip, int take, CancellationToken token)
    {

        var transactions = await _customerRepository.GetCustomerTransactionsAsync(customerId, skip, take, token);

        return Ok(transactions);
    }

    /// <summary>
    /// Handles the upsert operation for a customer entity.
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpsertCustomer([FromBody] Customers customer, CancellationToken token)
    {
        var result = await _customerRepository.UpsertCustomerAsync(customer, token);

        return Ok(result);
    }

    /// <summary>
    /// Upserting customer and transaction data.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="transactionId"></param>
    /// <param name="transaction"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPut("{customerId}/transactions/{transactionId}")]
    public async Task<IActionResult> UpsertTransaction(Guid customerId, Guid transactionId, [FromBody] CustomerTransactions transaction, CancellationToken token)
    {
        var result = await _customerRepository.UpsertTransctionAsync(transaction, token);

        return Ok(result);
    }
}

