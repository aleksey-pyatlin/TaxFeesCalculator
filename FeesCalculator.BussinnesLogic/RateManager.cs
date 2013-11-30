using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic.ExRatesServiceReference;

namespace FeesCalculator.BussinnesLogic
{
    public class RateManager : IRateManager
    {
        public Dictionary<DateTime, Decimal> Rates = new Dictionary<DateTime, Decimal>();
        
        public decimal GetNationalRate(DateTime rateDate)
        {
            if (!Rates.ContainsKey(rateDate))
            {
                decimal rate = AddFromWeb(rateDate);
                if (rate > 0)
                {
                    Rates.Add(rateDate, rate);
                }
                
            }
            
            if(Rates.ContainsKey(rateDate))
            {
                return Rates[rateDate];
            }

            return Rates.Values.Last();
        }

        private Decimal AddFromWeb(DateTime rateDate)
        {
            ExRatesSoapClient exRatesSoapClient = new ExRatesSoapClient();
            var result = exRatesSoapClient.ExRatesDyn(145, rateDate, rateDate);
            if(result != null)
            {
                var value = result.Tables[0].Rows[0][1];
                return Decimal.Parse(value.ToString());
            }
            
            return 0;
        }

        public void ImportRates(string importFilePath)
        {
            var lines = File.ReadLines(importFilePath);
            foreach (var line in lines.Skip(1))
            {
                string[] values = line.Split(',');
                DateTime rateDate = DateTime.Parse(values[0], Settings.RuCultureInfo);
                Decimal rate = Decimal.Parse((values[1] + values[2]).Trim('"', ' '), Settings.EngCultureInfo);
                Rates.Add(rateDate, rate);
            }    
        }
    }
}