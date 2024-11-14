using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.APIExtractors;

/// <summary>
/// Extracts data from an API endpoint.
/// </summary>
public class ApiExtractor : IExtractor
{
    private readonly IConfiguration _configuration;
    private readonly int _maxRetryAttempts = 3;
    private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);
    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiExtractor"/> class.
    /// </summary>
    /// <param name="configuration">The configuration settings.</param>
    public ApiExtractor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Asynchronously extracts data from the API endpoint.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>Returns a collection of <see cref="RawDataRecord"/> or an empty list if the response is null.</returns>
    public async Task<IEnumerable<RawDataRecord>> ExtractDataAsync(CancellationToken cancellationToken)
    {
        var baseUrl = _configuration["ApiFakeBaseUrl"];
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<RawDataRecord>>($"{baseUrl}", cancellationToken);

        return response ?? [];
    }
}






