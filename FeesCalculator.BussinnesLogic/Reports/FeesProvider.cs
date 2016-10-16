using System;
using System.Collections.Generic;

namespace FeesCalculator.BussinnesLogic.Reports
{
    public class FeesProvider
    {
        public static decimal GetFeesPercent(KeyValuePair<QuarterKey, Quarter> quarter)
        {
            if (quarter.Value.Year.Number > 2010 && quarter.Value.Year.Number < 2014 && (int)quarter.Value.Type > 1)
            {
                return (decimal)0.02;
            }

            switch (quarter.Value.Year.Number)
            {
                case 2010:
                case 2011:
                    return (decimal) 0.08;
                case 2012:
                case 2013:
                    return (decimal)0.02;
                case 2014:
                case 2015:
                case 2016:
                    return (decimal)0.05;
            }

            throw new Exception(String.Format("For {0} year fees percent is undefined.", quarter.Value.Year.Number));
        }

        public static decimal GetDeltaRateFeesPercent(KeyValuePair<QuarterKey, Quarter> quarter)
        {
            if (quarter.Value.Year.Number < 2012)
            {
                return (decimal)0.08;
            }

            if (quarter.Value.Year.Number == 2012)
            {
                return (decimal)0.07;
            }

            if (quarter.Value.Year.Number >= 2013)
            {
                return (decimal)0.05;
            }

            throw new Exception(String.Format("For {0} year delta rate fees percent is undefined.", quarter.Value.Year.Number));
        }
    }
}