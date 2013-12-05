using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    [KnownType(typeof(OperationMessage))]
    public class IncommingPaymentMessage : OperationMessage
    {
        public IncommingPaymentMessage()
            : base(OperationMessageType.Incomme)
        {
        }

        [JsonIgnore]
        public override decimal Rate { get; set; }

        public String Comment {
            get { return "ƒанный платеж содержит информацию о получении валюты на валютный счет."; }
        }
    }
}