using PNedov.IsiMarkets.EtlPipeline.Controllers;
using PNedov.IsiMarkets.EtlPipeline.Models;

namespace PNedov.IsiMarkets.EtlPipeline.APIExtractors;

/// <summary>
/// Extracts data from an API endpoint.
/// </summary>
public class ApiExtractor : IExtractor
{
    private ILogger<ApiExtractor> _logger;
    private readonly IConfiguration _configuration;
    private readonly int _maxRetryAttempts = 3;
    private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);
    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiExtractor"/> class.
    /// </summary>
    /// <param name="configuration">The configuration settings.</param>
    public ApiExtractor(IConfiguration configuration, ILogger<ApiExtractor> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Asynchronously extracts data from the API endpoint with pagination.
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns></returns>
    public async Task<IEnumerable<RawDataRecord>> ExtractDataAsync(CancellationToken cancellationToken)
    {
        var baseUrl = _configuration["ApiFakeBaseUrl"];
        var size = _configuration["ApiPageSize"];
        var allData = new List<RawDataRecord>();
        int page = 1;
        bool hasMoreData = false;

        try
        {
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<RawDataRecord>>($"{baseUrl}?page={page}&size={size}", cancellationToken);
                if (response != null && response.Any())
                {
                    allData.AddRange(response);
                    page++;
                    hasMoreData = true;
                }
                else
                {
                    hasMoreData = false;
                }
            } while (hasMoreData && !cancellationToken.IsCancellationRequested);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            _logger.LogError($"An error occurred while extracting data from the API: {ex.Message}");
        }

        return allData;
    }
}






