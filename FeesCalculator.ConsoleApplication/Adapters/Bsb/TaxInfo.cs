using FeesCalculator.Data;

namespace FeesCalculator.ConsoleApplication.Adapters.Bsb
{
    public class TaxInfo
    {
        public QuarterType QuarterType
        {
            get; set; 
        }

        public int YearNumber { get; set; }

        public string Comment {
            get
            {
                return string.Format("Данная операция содержит информацию об оплате налога за определенный квартал и она будет учитываться при рассчете переплаты и недоплаты в налоговую.");
            }
        }
    }
}