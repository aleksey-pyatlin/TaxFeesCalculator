using System;
using System.Collections.Generic;
using FeesCalculator.BussinnesLogic.Messages;

namespace FeesCalculator.ConsoleApplication.Adapters.Bsb
{
    public class Filter
    {
        private List<String> incommingMessages = new List<String>();
        private List<String> sellMessages = new List<String>();
        
        public void AddIncommingMessage(List<OperationMessage> messages,
                                        IncommingPaymentMessage incommingPaymentMessage, String dateActuality)
        {
            //TODO: Check by date time with data
            String key = dateActuality.ToString() + incommingPaymentMessage.Amount;

            if (!incommingMessages.Contains(key))
            {

                messages.Add(incommingPaymentMessage);
                incommingMessages.Add(key);
            }
        }

        public void AddSellMessage(List<OperationMessage> messages,
                                   SellMessage sellMessage)
        {
            String key = sellMessage.Date.ToString() + sellMessage.SellType.ToString() + sellMessage.Amount;
            if (!sellMessages.Contains(key))
            {
                messages.Add(sellMessage);
                sellMessages.Add(key);
            }
        }
    }
}