using System.Collections.Generic;
using System.Linq;

namespace FeesCalculator.BussinnesLogic
{
    public class QuarterContainer
    {
        private readonly Dictionary<QuarterKey, Quarter> _quarters = new Dictionary<QuarterKey, Quarter>();

        public Dictionary<QuarterKey, Quarter> Quarters
        {
            get { return _quarters; }
        }

        public bool ContainsKey(QuarterKey quarterType)
        {
            return _quarters.ContainsKey(quarterType);
        }

        public Quarter GetQuarter(QuarterKey quarterKey, IRateManager rateManager)
        {
            if (!ContainsKey(quarterKey))
                _quarters.Add(quarterKey, GetNewQuarter(quarterKey, rateManager));

            return _quarters[quarterKey];
        }

        private Quarter GetNewQuarter(QuarterKey quarterKey, IRateManager rateManager)
        {
            Quarter quarter = new Quarter() {
                Type = quarterKey.Type, 
                Year = new Year()
                           {
                               Number = quarterKey.YearNumber
                           },
                Previous = GetPrevious()
            };
            quarter.OnClose += delegate(Quarter quarter1)
                                   {
                                       PaymentOperations.AddRemain(Calendar.GetLastDayOfMonth(QuarterResolver.GetLasttMonth(quarter1.Type), quarter1.Year.Number), quarter1, rateManager);
                                   };
            ClosePreviouQuarter();
            return quarter;
        }

        private Quarter GetPrevious()
        {
            if(_quarters.Count > 0)
            {
                return _quarters.Values.Last();
            }

            return null;
        }

        private void ClosePreviouQuarter()
        {
            //Previous by number
            if(_quarters.Count > 0)
            {
                _quarters.Values.Last().Close();
            }
        }
    }
}