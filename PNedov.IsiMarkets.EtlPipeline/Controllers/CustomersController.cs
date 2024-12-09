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
    private ILogger<CustomersController> _logger;
    private readonly ICustomersRepository _customerRepository;

    public CustomersController(ICustomersRepository customerRepository, ILogger<CustomersController> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieving customers with pagination
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCustomers(int skip, int take, CancellationToken cancellationToken)
    {
        try
        {
            var customers = await _customerRepository.GetCustomersAsync(skip, take, cancellationToken);

            return Ok(customers);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            _logger.LogError($"An error occurred while getting customers data from central repository: {ex.Message}");
        }

        return NoContent();
    }

    /// <summary>
    /// Retrieves a paginated list of transactions for a specific customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="skip">The number of transactions to skip.</param>
    /// <param name="take">The number of transactions to take.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns></returns>
    [HttpGet("{customerId}/transactions")]
    public async Task<IActionResult> GetCustomerTransactions(Guid customerId, int skip, int take, CancellationToken cancellationToken)
    {
        try
        {
            var transactions = await _customerRepository.GetCustomerTransactionsAsync(customerId, skip, take, cancellationToken);

            return Ok(transactions);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            _logger.LogError($"An error occurred while getting transaction data from central repository: {ex.Message}");
        }

        return NoContent();
    }

    /// <summary>
    /// Handles the upsert operation for a customer entity.
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpsertCustomer([FromBody] Customers customer, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _customerRepository.UpsertCustomerAsync(customer, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            _logger.LogError($"An error occurred while modify customer data in central repository: {ex.Message}");
        }

        return NoContent();
    }

    /// <summary>
    /// Upserting customer and transaction data.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="transactionId"></param>
    /// <param name="transaction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{customerId}/transactions/{transactionId}")]
    public async Task<IActionResult> UpsertTransaction(Guid customerId, Guid transactionId, [FromBody] CustomerTransactions transaction, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _customerRepository.UpsertTransctionAsync(transaction, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            _logger.LogError($"An error occurred while modify transaction data in central repository: {ex.Message}");
        }

        return NoContent();
    }
}

