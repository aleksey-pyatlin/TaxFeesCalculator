using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace FeesCalculator.Tests
{
    [TestFixture]
    public class PaymentTests
    {
        [Test]
        public void IncomeTest()
        {
            var cultureInfo = new CultureInfo("en-US");
            MockRepository mockRepository = new MockRepository();
            IRateManager rateManager = mockRepository.StrictMock<IRateManager>();
            ArrivalConsumptionManager arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);

            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("12/25/2010", cultureInfo))).Return(2500).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("12/27/2010", cultureInfo))).Return(2800).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("12/31/2010", cultureInfo))).Return(2850).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("2/2/2011", cultureInfo))).Return(3050).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("2/3/2011", cultureInfo))).Return(3070).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("2/5/2011", cultureInfo))).Return(3120).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("2/6/2011", cultureInfo))).Return(3130).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("4/5/2011", cultureInfo))).Return(3100).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("4/15/2011", cultureInfo))).Return(2900).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("3/31/2011", cultureInfo))).Return(3140).Repeat.Any();
            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("6/30/2011", cultureInfo))).Return(5000).Repeat.Any();
//            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("12/31/2009"))).Return(2863);
//            //first work day
//            Expect.Call(rateManager.GetNationalRate(DateTime.Parse("1/4/2010"))).Return(2859);

            mockRepository.ReplayAll();
            List<OperationMessage> messages = new List<OperationMessage>();

            messages.Add(new IncommingPaymentMessage()
                             {
                                 Date = DateTime.Parse("12/25/2010", cultureInfo),
                                 Amount = (decimal) 1500,
                                 Rate = 2700
                             });

            messages.Add(new SellMessage()
                             {
                                 Date = DateTime.Parse("12/27/2010", cultureInfo),
                                 Amount = (decimal) 1000,
                                 Rate = 2900
                             });

            messages.Add(new IncommingPaymentMessage()
                             {
                                 Date = DateTime.Parse("2/2/2011", cultureInfo),
                                 Amount = (decimal) 1000,
                                 Rate = 3050
                             });

            messages.Add(new SellMessage()
                             {
                                 Date = DateTime.Parse("2/3/2011", cultureInfo),
                                 Amount = (decimal) 300,
                                 SellType = SellType.Mandatory,
                                 Rate = 3090
                             });

            messages.Add(new SellMessage()
                             {
                                 Date = DateTime.Parse("2/3/2011", cultureInfo),
                                 Amount = (decimal) 50,
                                 SellType = SellType.Mandatory,
                                 Rate = 3120
                             });

            messages.Add(new IncommingPaymentMessage()
                             {
                                 Date = DateTime.Parse("2/5/2011", cultureInfo),
                                 Amount = (decimal) 100,
                                 Rate = 3120
                             });

            messages.Add(new IncommingPaymentMessage()
                             {
                                 Date = DateTime.Parse("2/6/2011", cultureInfo),
                                 Amount = (decimal) 200,
                                 Rate = 3130
                             });

            messages.Add(new SellMessage()
                             {
                                 Date = DateTime.Parse("4/5/2011", cultureInfo),
                                 Amount = (decimal) 500,
                                 SellType = SellType.Free,
                                 Rate = 3150
                             });

            messages.Add(new SellMessage()
                             {
                                 Date = DateTime.Parse("4/15/2011", cultureInfo),
                                 Amount = (decimal) 100,
                                 SellType = SellType.Free,
                                 Rate = 3100
                             });

            arrivalConsumptionManager.Calculate(messages);
            arrivalConsumptionManager.Close();

            ArrivalConsumptionPresentation arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
            arrivalConsumptionPresentation.Render(arrivalConsumptionManager.Quarters);

            QuarterResult finalForFourQuarter = arrivalConsumptionManager.GetFinalAmount(QuarterType.Four, 2010);
            Assert.AreEqual((decimal) 275000, (decimal) finalForFourQuarter.DeltaRate);
            Assert.AreEqual((decimal) 4050000, (decimal) finalForFourQuarter.Incomming);
            Assert.AreEqual((decimal) 4325000, (decimal) finalForFourQuarter.Summary);


            QuarterResult finalForOneQuarter = arrivalConsumptionManager.GetFinalAmount(QuarterType.One, 2011);
            Assert.AreEqual((decimal) 3988000, (decimal) finalForOneQuarter.Incomming);
            Assert.AreEqual((decimal) 223000, (decimal) finalForOneQuarter.DeltaRate);
            Assert.AreEqual((decimal) 4211000, (decimal) finalForOneQuarter.Summary);

            QuarterResult finalForTwoQuarter = arrivalConsumptionManager.GetFinalAmount(QuarterType.Two, 2011);
            Assert.AreEqual((decimal) 0, (decimal) finalForTwoQuarter.Incomming);
            Assert.AreEqual((decimal) 1830000, (decimal) finalForTwoQuarter.DeltaRate);
            Assert.AreEqual((decimal) 1830000, (decimal) finalForTwoQuarter.Summary);

            mockRepository.VerifyAll();
        }

        [Test]
        [Ignore]
        public void SequenceTest()
        {
            var lines  = File.ReadAllLines(
                        @"..\..\Data\SequenceMessageTestData.txt"
                        );
            List<OperationMessage> messages = new List<OperationMessage>();
            foreach (var line in lines)
            {
                var values = line.Split('|');
                if (values[0] == "IncommingPaymentMessage")
                {
                    IncommingPaymentMessage testData = JsonHelper.FromJson<IncommingPaymentMessage>(values[1]);
                    messages.Add(testData);
                }
                if (values[0] == "SellMessage")
                {
                    SellMessage testData = JsonHelper.FromJson<SellMessage>(values[1]);
                    messages.Add(testData);
                }


            }


            RateManager rateManager = new RateManager();
            rateManager.ImportRates(@"D:\Views\Learning\Svn\apps\FeesCalc\FeesCalculator.ConsoleApplication\Data\Rates\2010_usd_currecyRate.csv");
            rateManager.ImportRates(@"D:\Views\Learning\Svn\apps\FeesCalc\FeesCalculator.ConsoleApplication\Data\Rates\2011_usd_currecyRate.csv");
            String paymentsPath =
                @"D:\Views\Learning\Svn\apps\FeesCalc\FeesCalculator.ConsoleApplication\Data\Mtb\20120103.xml";

           ArrivalConsumptionManager arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);

            arrivalConsumptionManager.Calculate(messages);
            arrivalConsumptionManager.Close();

            ArrivalConsumptionPresentation arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
            arrivalConsumptionPresentation.Render(arrivalConsumptionManager.Quarters);
            var debits =
                arrivalConsumptionManager.Quarters[new QuarterKey() {Type = QuarterType.Three, YearNumber = 2011}].
                    Debits;
            Assert.AreEqual((decimal)0, (decimal)debits[debits.Count - 1].Amount);
        
        }
    }
}