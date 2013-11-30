using System;
using System.Collections.Generic;

namespace FeesCalculator.Data
{
    public class Payment
    {
        public DateTime Date { get; set; }

        public virtual decimal Amount { get; set; }

        public decimal Rate { get; set; }

        public virtual decimal NationalAmount
        {
            get
            {
                return
                    Math.Round(Amount * Rate, 0);
            }
        }

        
        public Payment PreviousPayment { get; set; }

        public PaymentType? OperationType
        {
            get; set; 
        }

        public List<SellPayment> Credits { get; set; }

        public Payment()
        {
            Credits = new List<SellPayment>();
        }

        public decimal RateDelta { get; set; }
    }


    //TODO: Remove sell payment
    public class SellPayment : Payment
    {
        public SellPayment()
        {
        }
    }//TODO: Remove sell payment
    
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