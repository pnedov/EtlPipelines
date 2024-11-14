using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.APIExtractors;

public interface IExtractor
{
    Task<IEnumerable<RawDataRecord>> ExtractDataAsync(CancellationToken token);
}
