using System.Collections.Generic;
using FeesCalculator.BussinnesLogic.Messages;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public interface ITaxFeesProfile
    {
        void Init();

        IEnumerable<OperationMessage> GetOperations();
    }
}