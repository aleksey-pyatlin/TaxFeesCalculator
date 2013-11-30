using System;
using System.Runtime.Serialization;

namespace FeesCalculator.BussinnesLogic.Messages
{
    [Serializable]
    [KnownType(typeof(OperationMessage))]
    public class IncommingPaymentMessage : OperationMessage
    {

    }
}