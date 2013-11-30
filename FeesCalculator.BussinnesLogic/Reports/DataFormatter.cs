using System;

namespace FeesCalculator.BussinnesLogic.Reports
{
    public class DataFormatter
    {
        public static string ToDecimal(decimal amount)
        {
            if (amount == 0)
                return String.Empty;
            return amount.ToString("0,0.0");
        }

        public static string ToDateTime(DateTime date)
        {
            if (date == DateTime.MinValue)
                return String.Empty;
            return date.ToShortDateString();
        }
    }
}