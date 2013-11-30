using System.Collections.Generic;
using FeesCalculator.Data;

namespace FeesCalculator.BussinnesLogic
{
    public class Quarter
    {
        public Year Year { get; set; }

        public List<Payment> Credits { get; set; }

        public QuarterType Type { get; set; }

        public decimal OutOfProfit { get; set; }

        public decimal Incomming { get; set; }

        public decimal Summary
        {
            get { return Incomming + OutOfProfit; }
        }

        public List<Payment> Debits { get; set; }

        public Quarter Previous { get; set; }

        public decimal PaidTaxAmount { get; set; }

        public decimal CalcFees { get; set; }

        public List<OutOfProfitPayment> OutOfProfits
        {
            get; set; }

        public Quarter()
        {
            Credits = new List<Payment>();
            Debits = new List<Payment>();
            OutOfProfits = new List<OutOfProfitPayment>();
        }

        public void Close()
        {
            if(OnClose != null)
            {
                OnClose(this);
            }
        }


        public event QuarterHandler OnClose;
    }
}