using System;
using System.Collections.Generic;
using System.Linq;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.Data;

namespace FeesCalculator.BussinnesLogic
{
    public class ArrivalConsumptionManager
    {
        private readonly QuarterContainer _quarterContainer;

        public Dictionary<QuarterKey, Quarter> Quarters
        {
            get { return _quarterContainer.Quarters; }
        }

        private readonly IRateManager _rateManager;

        public ArrivalConsumptionManager(IRateManager rateManager)
        {
            _rateManager = rateManager;
            _quarterContainer = new QuarterContainer();
        }

        public void Income(IncommingPaymentMessage incommingPaymentMessage)
        {
            Payment payment = new Payment()
                                  {
                                      Date = incommingPaymentMessage.Date,
                                      Amount = incommingPaymentMessage.Amount,
                                      Rate = incommingPaymentMessage.Rate,
                                      OperationType = PaymentType.Incomm
                                  };
            if (incommingPaymentMessage.Date.Year < 2013)
                AddRemainCalc(incommingPaymentMessage);

            Quarter quarter = PaymentOperations.AddToDebit(payment, _quarterContainer, _rateManager);
            quarter.Incomming += payment.Amount*payment.Rate;
        }

        private Quarter AddToCredit(SellPayment payment)
        {
            var quarterType = QuarterResolver.GetQuarterNumber(payment.Date);
            var quarter = _quarterContainer.GetQuarter(quarterType, _rateManager);

            AddReferenceToDebit(payment, quarter.Debits);
            PaymentOperations.AddReferenceOnPreviousDebit(payment, quarter.Debits);

            quarter.Credits.Add(payment);

            return quarter;
        }

        private Quarter AddToOutOfProfit(OutOfProfitPayment payment)
        {
            var quarterType = QuarterResolver.GetQuarterNumber(payment.Date);
            var quarter = _quarterContainer.GetQuarter(quarterType, _rateManager);


            quarter.OutOfProfits.Add(payment);

            return quarter;
        }

        private void AddReferenceToDebit(SellPayment payment, List<Payment> debits)
        {
            if (debits.Count > 0)
                debits[debits.Count - 1].Credits.Add(payment);
        }

        public void Sell(SellMessage sellMessage)
        {
            AddRemainCalc(sellMessage);

            var payment = new SellPayment()
                              {
                                  Date = sellMessage.Date,
                                  Amount = sellMessage.Amount,
                                  Rate = sellMessage.Rate,
                                  OperationType = PaymentType.Sell
                              };

            Quarter quarter = AddToCredit(payment);
            PaymentOperations.SetDeltaRate(payment, quarter);
        }

        public void OutOfProfit(OutOfProfitMessage sellMessage)
        {
            AddRemainCalc(sellMessage);

            var payment = new OutOfProfitPayment()
            {
                Date = sellMessage.Date,
                AmountNat = sellMessage.AmountNat,
                Ground = sellMessage.Ground
            };

            Quarter quarter = AddToOutOfProfit(payment);
            if(payment.AmountNat > 0)
                quarter.OutOfProfit += payment.AmountNat;
            //PaymentOperations.SetDeltaRate(payment, quarter);
        }

        private void AddRemainCalc(OperationMessage sellMessage)
        {
            SellPayment payment = GetRemainCalc(sellMessage.Date) as SellPayment;
            if (payment == null)
            {
                PaymentOperations.AddRemain(sellMessage as SellMessage, sellMessage.Date, _quarterContainer, _rateManager);
            }
        }

        private Payment GetRemainCalc(DateTime date)
        {
            var quarterType = QuarterResolver.GetQuarterNumber(date);
            var quarter = _quarterContainer.GetQuarter(quarterType, _rateManager);

            return (from debit in quarter.Debits
                    where debit.Date == date && debit.OperationType == PaymentType.Remain
                    select debit).FirstOrDefault();
        }

        public QuarterResult GetFinalAmount(QuarterType quartertype, int yearNumber)
        {
            QuarterResult quarterResult = new QuarterResult();
            var quarterKey = new QuarterKey() {Type = quartertype, YearNumber = yearNumber};
            var quarter = _quarterContainer.GetQuarter(quarterKey, _rateManager);

            quarterResult.DeltaRate = quarter.OutOfProfit;
            quarterResult.Incomming = quarter.Incomming;

            return quarterResult;
        }

        public void Close()
        {
            if (_quarterContainer.Quarters.Any())
                _quarterContainer.Quarters.Values.Last().Close();
        }

        public void Calculate(List<OperationMessage> messages)
        {
            messages = PrepeareMessages(messages);
            foreach (var operationMessage in messages)
            {
                var incommingPaymentMessage = operationMessage as IncommingPaymentMessage;
                if (incommingPaymentMessage != null)
                    Income(incommingPaymentMessage);
                else
                {
                    var sellMessage = operationMessage as SellMessage;
                    if (sellMessage != null)
                        Sell(sellMessage);
                    else
                    {
                        var outOfProfitMessage = operationMessage as OutOfProfitMessage;
                        if (outOfProfitMessage != null)
                            OutOfProfit(outOfProfitMessage);
                    }
                }
            }
        }

        private List<OperationMessage> PrepeareMessages(List<OperationMessage> messages)
        {
            return (from m in messages
                       orderby m.Date, GetPaymentType(m)
                       select m).ToList();
            
        }

        private int GetPaymentType(OperationMessage operationMessage)
        {
            if (operationMessage is IncommingPaymentMessage) return 1;
            if (operationMessage is SellMessage) return 2;
            if (operationMessage is OutOfProfitMessage) return 3;
            return 0;
        }
    }
}