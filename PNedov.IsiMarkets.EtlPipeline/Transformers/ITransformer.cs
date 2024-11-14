using PNedov.IsiMarkets.EtlPipeline.Models;
using System.Collections.Generic;

namespace PNedov.IsiMarkets.EtlPipeline.Transormers;

public interface ITransformer
{
    IEnumerable<RawDataRecord> Validate(IEnumerable<RawDataRecord> records);
    IEnumerable<RawDataRecord> Format(IEnumerable<RawDataRecord> records);
    IEnumerable<RawDataRecord> ApplyBusinessRule(IEnumerable<RawDataRecord> records);
}
