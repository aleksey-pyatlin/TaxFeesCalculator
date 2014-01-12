using System;
using FeesCalculator.Data;

namespace FeesCalculator.BussinnesLogic
{
    public class QuarterResolver
    {
        public static QuarterKey GetQuarterNumber(DateTime dateTime)
        {
            QuarterType quarterType = GetQuarterType(dateTime);
            return new QuarterKey() {Type = quarterType, YearNumber = dateTime.Year};
        }

        private static QuarterType GetQuarterType(DateTime dateTime)
        {
            int month = dateTime.Month;
            if(0 < month && month < 4) 
            {
                return  QuarterType.One;
            } 
            
            if(3 < month && month < 7) 
            {
                return QuarterType.Two;
            }

            if (6 < month && month < 10)
            {
                return QuarterType.Three;
            }

            if (9 < month && month < 13)
            {
                return QuarterType.Four;
            }

            throw new Exception("Quarter is undefined.");
        }

        public static int GetLasttMonth(QuarterType type)
        {
            return GetFirstMonth(type) + 2;
        }

        public static int GetFirstMonth(QuarterType type)
        {
            switch (type)
            {
                case QuarterType.One:
                    return 1;
                    
                case  QuarterType.Two:
                    return 4;

                case QuarterType.Three:
                    return 7;

                case QuarterType.Four:
                    return 10;
            }

            throw new Exception("Quarter is undefined.");
        }

        public static QuarterType GetQuarterNumber(string quarter)
        {
            switch (quarter)
            {
                case "I":
                    return QuarterType.One;

                case "II":
                    return QuarterType.Two;

                case "III":
                    return QuarterType.Three;

                case "IV":
                case "VI": //This is fix.
                    return QuarterType.Four;
            }

            throw new Exception("Quarter is undefined.");
        }
    }
}