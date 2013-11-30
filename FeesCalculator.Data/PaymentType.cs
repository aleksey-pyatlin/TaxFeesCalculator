using System;

namespace FeesCalculator.Data
{
    public enum PaymentType
    {
        Incomm,

        Remain,

        [Obsolete]
        Sell
    }
}