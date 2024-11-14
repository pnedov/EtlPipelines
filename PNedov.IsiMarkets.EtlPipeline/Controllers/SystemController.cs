using Microsoft.AspNetCore.Mvc;
using PNedov.IsiMarkets.EtlPipeline.Repositories;
using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Controllers;

/// <summary>
/// Responsible for handling retrieving and updating system configurations
/// </summary>
[ApiController]
[Route("system")]
public class SystemController : ControllerBase
{
    private ILogger<SystemController> _logger;
    private ISystemRepository _serviceRepo;

    public SystemController(ISystemRepository service, ILogger<SystemController> logger)
    {
        _logger = logger;
        _serviceRepo = service;
    }

    /// <summary>
    /// Retrieves the system configuration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("configuration")]
    public async Task<ActionResult<string>> GetSystemConfiguration(CancellationToken cancellationToken)
    {
        await _serviceRepo.GetSystemConfiguration(cancellationToken);

        return RedirectToAction("Index", "System");
    }

    /// <summary>
    /// This method handles the upsert operation for a system entity
    /// </summary>
    /// <param name="newConfig"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("configuration")]
    public async Task<ActionResult<string>> UpsertSystemConfiguration(SystemConfigurations newConfig, CancellationToken cancellationToken)
    {
        await _serviceRepo.UpsertSystemConfiguration(newConfig, cancellationToken);

        return RedirectToAction("Index", "System");
    }

    /// <summary>
    /// Initializes the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("initialize")]
    public async Task<ActionResult<string>> InitializeDatabase(CancellationToken cancellationToken)
    {
        await _serviceRepo.InitializeDatabase(cancellationToken);

        return RedirectToAction("Index", "Customers");
    }
}

