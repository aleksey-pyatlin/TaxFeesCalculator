using System;
using System.Collections.Generic;
using FeesCalculator.Data;

namespace FeesCalculator.BussinnesLogic
{
    public static class PaymentOperations
    {
        public static void AddRemain(DateTime remainDate, QuarterContainer quarterContainer,
                                     IRateManager rateManager)
        {
            SellPayment payment = new SellPayment()
                                      {
                                          Date = remainDate,
                                          Rate = rateManager.GetNationalRate(remainDate),
                                          OperationType = PaymentType.Remain
                                      };

            Quarter quarter = AddToDebit(payment, quarterContainer, rateManager);

            if(IsFirstRemainPayment(quarter))
            {
                if(quarter.Previous != null)
                {
                    var lastRetainPaymentFromPreviousQuarter = quarter.Previous.Debits[quarter.Previous.Debits.Count - 1];
                    payment.PreviousPayment = lastRetainPaymentFromPreviousQuarter;
                }
            }
            decimal amount = GetDebitAmount(payment);
            payment.Amount = amount;

            if (payment.Amount < 0)
            {
                RemoveFromDebit(payment, quarterContainer, rateManager);
            }

            SetDeltaRate(payment, quarter);
        }

        private static bool IsFirstRemainPayment(Quarter quarter)
        {
            if(quarter.Debits.Count == 1)
            {
                return true;
            }

            return false;
        }

        public static void AddRemain(DateTime remainDate, Quarter quarter, IRateManager rateManager)
        {
            SellPayment payment = new SellPayment()
            {
                Date = remainDate,
                Rate = rateManager.GetNationalRate(remainDate),
                OperationType = PaymentType.Remain
            };

            quarter = AddToDebit(payment, quarter);

            decimal amount = GetDebitAmount(payment);
            payment.Amount = amount;

            SetDeltaRate(payment, quarter);
        }

        public static Quarter AddToDebit(Payment payment, QuarterContainer quarterContainer, IRateManager rateManager)
        {
            var quarterKey = QuarterResolver.GetQuarterNumber(payment.Date);
            Quarter quarter = quarterContainer.GetQuarter(quarterKey, rateManager);
            return AddToDebit(payment, quarter);
        }

        public static Quarter RemoveFromDebit(Payment payment, QuarterContainer quarterContainer, IRateManager rateManager)
        {
            var quarterKey = QuarterResolver.GetQuarterNumber(payment.Date);
            Quarter quarter = quarterContainer.GetQuarter(quarterKey, rateManager);
            return RemoveFromDebit(payment, quarter);
        }

        public static Quarter AddToDebit(Payment payment, Quarter quarter)
        {
            AddReferenceOnPreviousDebit(payment, quarter.Debits);
            quarter.Debits.Add(payment);

            return quarter;
        } 
        
        public static Quarter RemoveFromDebit(Payment payment, Quarter quarter)
        {
            quarter.Debits.Remove(payment);
            return quarter;
        }

        public static void SetDeltaRate(SellPayment payment, Quarter quarter)
        {
            if (payment.PreviousPayment != null)
            {
                var deltaRate = (payment.Rate - payment.PreviousPayment.Rate)*payment.Amount;
                payment.RateDelta = deltaRate;
                if (deltaRate > 0)
                {
                    quarter.OutOfProfit += deltaRate;
                }
            }
        }

        public static decimal GetDebitAmount(SellPayment payment)
        {
            if (payment.PreviousPayment != null)
            {
                decimal amount = GetPreviousDebitAmount(payment);
                payment.PreviousPayment.Credits.ForEach(x => amount -= x.Amount);
                return amount;
            }

            return payment.Amount;
        }

        private static decimal GetPreviousDebitAmount(Payment previousPayment)
        {
            if (previousPayment.PreviousPayment != null && previousPayment.PreviousPayment.OperationType != PaymentType.Remain )
            {
                return previousPayment.PreviousPayment.Amount + GetPreviousDebitAmount(previousPayment.PreviousPayment);
            }

            if (previousPayment.PreviousPayment != null)
                return previousPayment.PreviousPayment.Amount;

            return 0;

        }


        public static void AddReferenceOnPreviousDebit(Payment payment, List<Payment> payments)
        {
            if (payments.Count > 0)
                payment.PreviousPayment = payments[payments.Count - 1];
        }
    }
}