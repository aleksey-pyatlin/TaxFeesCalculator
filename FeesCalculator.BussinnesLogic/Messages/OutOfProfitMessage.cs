using System;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    public class OutOfProfitMessage : OperationMessage
    {
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