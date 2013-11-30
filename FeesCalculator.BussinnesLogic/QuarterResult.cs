namespace FeesCalculator.BussinnesLogic
{
    public class QuarterResult
    {
        public decimal Incomming { get; set; }

        public decimal DeltaRate { get; set; }

        public decimal Summary
        {
            get { return Incomming + DeltaRate; }
        }
    }
}