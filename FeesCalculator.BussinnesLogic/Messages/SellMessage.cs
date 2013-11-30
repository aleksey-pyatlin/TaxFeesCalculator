using System;
using System.Runtime.Serialization;
using FeesCalculator.Data;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    [KnownType(typeof(OperationMessage))]
    public class SellMessage: OperationMessage
    {
        public SellType SellType { get; set; }

        [Obsolete]
        public decimal Commission { get; set; }
    }
}