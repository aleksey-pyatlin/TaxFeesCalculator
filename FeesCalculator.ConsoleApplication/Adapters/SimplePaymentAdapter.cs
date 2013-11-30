using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeesCalculator.BussinnesLogic.Messages;

namespace FeesCalculator.ConsoleApplication.Adapters
{
    public class SimplePaymentAdapter : IAdapter
    {
        public List<OperationMessage> GetMessages(string dataDirectoryPath, List<string> files)
        {
            throw new NotImplementedException();
        }
    }
}
