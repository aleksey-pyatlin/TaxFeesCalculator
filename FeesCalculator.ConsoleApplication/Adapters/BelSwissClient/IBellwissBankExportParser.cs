using System.Collections.Generic;
using System.IO;

namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
    public interface IBellwissBankExportParser
    {
        IEnumerable<Token> GetTokens(Stream stream);
    }
}