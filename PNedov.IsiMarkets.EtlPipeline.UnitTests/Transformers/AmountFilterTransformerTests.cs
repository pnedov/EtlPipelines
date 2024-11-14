using PNedov.IsiMarkets.EtlPipeline.Models;
using PNedov.IsiMarkets.EtlPipeline.Transormers;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace PNedov.IsiMarkets.EtlPipeline.UnitTests.Transformers;

public class AmountFilterTransformerTests
{
    private readonly Mock<ILogger<ITransformer>> _loggerMock;
    private readonly AmountFilterTransformer _transformer;

    public AmountFilterTransformerTests()
    {
        _loggerMock = new Mock<ILogger<ITransformer>>();
        _transformer = new AmountFilterTransformer(_loggerMock.Object);
    }

    [Fact]
    public void ApplyBusinessRule_ShouldApplyDiscount_WhenQuantityGreaterThan10()
    {
        // Arrange
        var records = new List<RawDataRecord>
        {
            new RawDataRecord { Quantity = "15", UnitPrice = "20.00" }
        };

        // Act
        records = (List<RawDataRecord>)_transformer.ApplyBusinessRule(records);

        // Assert
        Assert.Equal("2.00", records[0].Discount);
        Assert.Equal("270.00", records[0].TotalPrice);
    }

    [Fact]
    public void Format_ShouldFormatNumericalAndTimestampValues()
    {
        // Arrange
        var records = new List<RawDataRecord>
        {
            new RawDataRecord
            {
                Quantity = "15.123",
                UnitPrice = "20.456",
                Discount = "2.789",
                TotalPrice = "300.123",
                CustomerTransactionTimestamp = "2023-10-01T12:34:56.789Z"
            }
        };

        // Act
        records = (List<RawDataRecord>)_transformer.Format(records);

        // Assert
        Assert.Equal("15.12", FormatFloat(records[0].Quantity));
        Assert.Equal("20.46", FormatFloat(records[0].UnitPrice));
        Assert.Equal("2.79", FormatFloat(records[0].Discount));
        Assert.Equal("300.12", FormatFloat(records[0].TotalPrice));
        Assert.Equal("2023-10-01T18:34:56.789Z", FormatTimestamp(records[0].CustomerTransactionTimestamp));
    }

    [Fact]
    public void Validate_ShouldLogWarning_WhenRecordIsInvalid()
    {
        // Arrange
        var records = new List<RawDataRecord> { new RawDataRecord { Quantity = "invalid" } };

        // Act
        _transformer.Validate(records);

        // Assert
        _loggerMock.Verify(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid record")),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }


    private string FormatFloat(string value)
    {
        return float.TryParse(value, out float result) ? result.ToString("F2") : value;
    }

    private string FormatTimestamp(string value)
    {
        return DateTime.TryParse(value, out DateTime result) ? result.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : value;
    }
}
