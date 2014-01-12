using System.Collections.Generic;

namespace FeesCalculator.BussinnesLogic.Messages
{
    public class OperationMessageContainer
    {
        public IList<IncommingPaymentMessage> IncommingPaymentMessages { get; set; }

        public IList<SellMessage> SellMessages { get; set; }
        public IList<TaxPaymentMessage> TaxPaymentMessages { get; set; }

        public IList<OperationMessage> GetMessages()
        {
            List<OperationMessage> operationMessages = new List<OperationMessage>();
            operationMessages.AddRange(IncommingPaymentMessages);
            operationMessages.AddRange(SellMessages);
            operationMessages.AddRange(TaxPaymentMessages);
            return operationMessages;
        }
    }
}