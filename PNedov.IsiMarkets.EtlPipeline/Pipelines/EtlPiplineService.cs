using System.Collections.Concurrent;
using PNedov.IsiMarkets.EtlPipeline.APIExtractors;
using PNedov.IsiMarkets.EtlPipeline.Repositories;
using PNedov.IsiMarkets.EtlPipeline.Transormers;

namespace PNedov.IsiMarkets.EtlPipeline.ETLPipelines;

/// <summary>
/// Manages the execution of ETL pipelines at regular intervals.
/// </summary>
public class EtlPiplineService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Timer _timer;
    private readonly IEnumerable<EltPipeline> _pipelines;
    private readonly int _sysIntervalSeconds;
    private readonly IConfiguration _configuration;
    private readonly IProductsRepository _productсRepository;
    private readonly ICustomersRepository _customersRepository;
    private readonly ILogger<ITransformer> _logger;
    private readonly ILogger<EltPipeline> _loggerEltPipeline;
    private readonly ConcurrentDictionary<string, Task> _runningTasks;

    public EtlPiplineService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IProductsRepository productRepository,
        ICustomersRepository customersRepository,
        ILogger<ITransformer> logger,
        ILogger<EltPipeline> loggerEltPipeline)
    {
        _serviceProvider = serviceProvider;
        _productсRepository = productRepository;
        _customersRepository = customersRepository;
        _configuration = configuration;
        _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        _sysIntervalSeconds = _configuration.GetValue<int>("SysTimeIntervalSeconds");
        _logger = logger;
        _loggerEltPipeline = loggerEltPipeline;
        _runningTasks = new ConcurrentDictionary<string, Task>();

        var _extractors = new List<IExtractor>() {
            new ApiExtractor(configuration),
            new CsvExtractor(configuration),
            new SqliteExtractor(configuration)
        };

        var _transformers = new List<ITransformer>() {
            new AmountFilterTransformer(_logger),
            new ConvertTimestampTransformer(_logger),
            new RemoveDuplicatesTransformer(_logger) 
        };

        _pipelines = [new EltPipeline(_extractors, _transformers, _productсRepository, _customersRepository, loggerEltPipeline)];
    }

    /// <summary>
    /// Starts the ETL pipeline service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_sysIntervalSeconds));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the ETL pipeline service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async void TimerCallback(object? state)
    {
        foreach (var pipeline in _pipelines)
        {
            if (_runningTasks.TryGetValue(pipeline.Id, out var runningTask) && !runningTask.IsCompleted)
            {
                continue;
            }
            _runningTasks[pipeline.Id] = pipeline.RunAsync(CancellationToken.None);
            await _runningTasks[pipeline.Id];
        }
    }
}

