using PNedov.IsiMarkets.EtlPipeline.Models;
using PNedov.IsiMarkets.EtlPipeline.Transormers;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace PNedov.IsiMarkets.EtlPipeline.UnitTests.Transformers;

public class ConvertTimestampTransformerTests
{
    private readonly Mock<ILogger<ITransformer>> _loggerMock;
    private readonly ConvertTimestampTransformer _transformer;

    public ConvertTimestampTransformerTests()
    {
        _loggerMock = new Mock<ILogger<ITransformer>>();
        _transformer = new ConvertTimestampTransformer(_loggerMock.Object);
    }

    [Fact]
    public void ApplyBusinessRule_ValidTimestamp_TransformsToUtc()
    {
        // Arrange
        var records = new List<RawDataRecord>{ new RawDataRecord { CustomerTransactionTimestamp = "2023-10-01T12:34:56" } };

        // Act
        records = (List<RawDataRecord>)_transformer.ApplyBusinessRule(records);

        // Assert
        Assert.Equal("2023-10-01T09:34:56.0000000Z", records[0].CustomerTransactionTimestamp);
    }

    [Fact]
    public void Format_ValidTimestamp_FormatsCorrectly()
    {
        // Arrange
        var records = new List<RawDataRecord> { new RawDataRecord { CustomerTransactionTimestamp = "2023-10-01T12:34:56" }};

        // Act
        records = (List<RawDataRecord>)_transformer.Format(records);

        // Assert
        Assert.Equal("2023-10-01T12:34:56.000Z", records[0].CustomerTransactionTimestamp);
    }

    [Fact]
    public void Validate_ValidTimestamp_DoesNotThrowException()
    {
        // Arrange
        var records = new List<RawDataRecord> { new RawDataRecord { CustomerTransactionTimestamp = "2023-10-01T12:34:56" } };

        // Act & Assert
        var exception = Record.Exception(() => _transformer.Validate(records));
        Assert.Null(exception);
    }
}

