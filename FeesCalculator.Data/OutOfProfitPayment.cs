namespace FeesCalculator.Data
{
    public class OutOfProfitPayment : Payment
    {
        public decimal AmountNat { get; set; }

        public string Ground { get; set; }

        public override decimal Amount
        {
            get { return AmountNat; }
            set { AmountNat = value; }
        }

        public override decimal NationalAmount
        {
            get { return AmountNat; }
        }
    }
}