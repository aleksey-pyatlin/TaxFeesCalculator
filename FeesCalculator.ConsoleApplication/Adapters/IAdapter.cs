using System;
using System.Collections.Generic;
using FeesCalculator.BussinnesLogic.Messages;

namespace FeesCalculator.ConsoleApplication.Adapters
{
    public interface IAdapter
    {
        IEnumerable<OperationMessage> GetMessages(string dataDirectoryPath, List<string> files);
    }
}