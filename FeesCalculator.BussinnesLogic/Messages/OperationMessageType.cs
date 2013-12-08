using System;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Flags]
    public enum OperationMessageType
    {
        Sell = 1,
        Income = 2,
        Tax = 4,
        OutOfProfit = 8
    }
}