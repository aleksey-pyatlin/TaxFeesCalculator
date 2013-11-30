using System;
using System.Collections.Generic;
using FeesCalculator.BussinnesLogic.Messages;

namespace FeesCalculator.ConsoleApplication.Adapters
{
    public interface IAdapter
    {
        List<OperationMessage> GetMessages(String dataDirectoryPath, List<string> files);
    }
}