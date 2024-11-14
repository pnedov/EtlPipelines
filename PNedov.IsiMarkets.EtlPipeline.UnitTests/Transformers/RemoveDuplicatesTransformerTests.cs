using PNedov.IsiMarkets.EtlPipeline.Models;
using PNedov.IsiMarkets.EtlPipeline.Transormers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PNedov.IsiMarkets.EtlPipeline.UnitTests.Transformers;

public class RemoveDuplicatesTransformerTests
{
    private readonly Mock<ILogger<ITransformer>> _mockLogger;
    private readonly RemoveDuplicatesTransformer _transformer;

    public RemoveDuplicatesTransformerTests()
    {
        _mockLogger = new Mock<ILogger<ITransformer>>();
        _transformer = new RemoveDuplicatesTransformer(_mockLogger.Object);
    }

    [Fact]
    public void ApplyBusinessRule_RemovesDuplicateRecords()
    {
        // Arrange
        var records = new List<RawDataRecord>
        {
            new RawDataRecord { CustomerTransactionId = "1", CustomerId = "C1", ProductId = "P1" },
            new RawDataRecord { CustomerTransactionId = "1", CustomerId = "C1", ProductId = "P1" },
            new RawDataRecord { CustomerTransactionId = "2", CustomerId = "C2", ProductId = "P2" }
        };

        // Act
        records = (List<RawDataRecord>)_transformer.ApplyBusinessRule(records);

        // Assert
        Assert.Equal(2, records.Count);
    }

    [Fact]
    public void Format_TrimStringProperties()
    {
        // Arrange
        var records = new List<RawDataRecord>
        {
            new RawDataRecord { CustomerTransactionId = "1", CustomerId = "C1", ProductId = "P1" },
            new RawDataRecord { CustomerTransactionId = "1", CustomerId = "C1", ProductId = "P1" },
            new RawDataRecord { CustomerTransactionId = "2", CustomerId = "C2", ProductId = "P2" }
        };

        // Act
        records = (List<RawDataRecord>)_transformer.Format(records);

        // Assert
        Assert.Equal("1", records[0].CustomerTransactionId);
        Assert.Equal("C1", records[0].CustomerId);
        Assert.Equal("P1", records[0].ProductId);
    }

    [Fact]
    public void Validate_RemovesInvalidRecords()
    {
        // Arrange
        var records = new List<RawDataRecord>
        {
            new RawDataRecord
            {
                CustomerTransactionId = "1",
                CustomerId = "C1",
                ProductId = "P1",
            },
            new RawDataRecord
            {
                CustomerTransactionId = "",
                CustomerId = "C2",
                ProductId = "P2",
            },
            new RawDataRecord
            {
                CustomerTransactionId = "3",
                CustomerId = "C3",
                ProductId = ""
            }
        };

        // Act
        records = _transformer.Validate(records).ToList();

        // Assert
        Assert.Single(records);
    }
}
