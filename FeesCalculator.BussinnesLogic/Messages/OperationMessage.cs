using System;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    public class OperationMessage
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Date, Amount);
        }
    }
}