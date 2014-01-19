using System.Collections.Generic;
using System.IO;

namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
    public interface IBellswissBankRecordParser
    {
        IEnumerable<Record> GetRecords(Stream stream);
    }
}