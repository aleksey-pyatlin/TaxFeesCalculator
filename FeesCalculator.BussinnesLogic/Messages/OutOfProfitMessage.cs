using System;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    public class OutOfProfitMessage : OperationMessage
    {
        public OutOfProfitMessage()
            : base(OperationMessageType.OutOfProfit)
        {
        }

        public decimal AmountNat
        {
            get; set; 
        }

        public String Ground
        {
            get; set; 
        }

        
    }
}