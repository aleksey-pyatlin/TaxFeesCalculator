namespace FeesCalculator.ConsoleApplication.Adapters
{
    public class RateInfo
    {
        public decimal Rate { get; set; }

        public decimal AmountNat { get; set; }

        public decimal Amount { get; set; }

        public decimal Commission
        {
            get; set; 
        }
    }
}