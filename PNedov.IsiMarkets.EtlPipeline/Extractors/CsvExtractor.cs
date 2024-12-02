using CsvHelper;
using CsvHelper.Configuration;
using PNedov.IsiMarkets.EtlPipeline.Models;
using System.Globalization;

namespace PNedov.IsiMarkets.EtlPipeline.APIExtractors;

/// <summary>
/// Extract data from a CSV file
/// </summary>
public class CsvExtractor : IExtractor
{
    private readonly IConfiguration _configuration;

    public CsvExtractor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Action method for asynchronously extracts data from the CSV source file.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task<IEnumerable<RawDataRecord>> ExtractDataAsync(CancellationToken cancellationToken)
    {
        var records = new List<RawDataRecord>();
        var hasHeaderRecordConfig = _configuration["CsvConfiguration:HasHeaderRecord"];
        var hasHeaderRecord = !string.IsNullOrEmpty(hasHeaderRecordConfig) && bool.Parse(hasHeaderRecordConfig);
        var delimiter = _configuration["CsvConfiguration:Delimiter"] ?? string.Empty;
        var csvFile = _configuration["CsvConfiguration:FileName"] ?? string.Empty;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeaderRecord,
            Delimiter = delimiter ?? ","
        };

        var filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, csvFile));

        if (File.Exists(filePath))
        {
            filePath = Path.GetFullPath(filePath);
            var backupFilePath = filePath + ".bak";

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                while (await csv.ReadAsync())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var record = csv.GetRecord<RawDataRecord>();
                    records.Add(record);
                }
            }

            if (File.Exists(backupFilePath))
            {
                File.Delete(backupFilePath);
            }

            File.Move(filePath, backupFilePath);
        }

        return await Task.FromResult(records);
    }
}