using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.Repositories;

public interface ISystemRepository
{
    Task <SystemConfigurations?> GetSystemConfiguration(CancellationToken token);
    Task UpsertSystemConfiguration(SystemConfigurations newConfig, CancellationToken token);
    Task InitializeDatabase(CancellationToken token);
}

