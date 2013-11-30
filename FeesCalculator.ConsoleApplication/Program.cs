using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.ConsoleApplication.Profiles;
using FeesCalculator.ConsoleApplication.Utils;

namespace FeesCalculator.ConsoleApplication
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            IHelperUtils helperUtils = new HelperUtils();
            var rateManager = new RateManager();
            
            var operationMessages = new List<OperationMessage>();
            APFeesConfigurator apFeesConfigurator = new APFeesConfigurator(rateManager, helperUtils);
            apFeesConfigurator.Init();
            operationMessages.AddRange(apFeesConfigurator.GetOperations());

            Run(operationMessages, rateManager);
        }

        private static void Run(List<OperationMessage> operationMessages, RateManager rateManager)
        {
            var arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);

            arrivalConsumptionManager.Calculate(operationMessages);
            arrivalConsumptionManager.Close();

            var arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
            AddTaxInfo(operationMessages, arrivalConsumptionManager.Quarters);
            arrivalConsumptionPresentation.Render(arrivalConsumptionManager.Quarters);
        }

        private static void AddTaxInfo(IEnumerable<OperationMessage> operationMessages,
            Dictionary<QuarterKey, Quarter> quarters)
        {
            foreach (OperationMessage operationMessage in operationMessages)
            {
                var taxSellMessage = operationMessage as TaxSellMessage;
                if (taxSellMessage != null)
                {
                    var quarterKey = new QuarterKey
                    {
                        Type = taxSellMessage.QuarterType,
                        YearNumber = taxSellMessage.YearNumber
                    };
                    if (quarters.ContainsKey(quarterKey))
                    {
                        quarters[quarterKey].PaidTaxAmount = taxSellMessage.Amount;
                    }
                }
            }
        }

        private static void WriteTestData(List<OperationMessage> operationMessages)
        {
            var days = new List<int>(new[]
            {
                15, 22, 31
            });
            List<OperationMessage> testdata = (from m in operationMessages
                where m.Date.Month == 8 && m.Date.Year == 2011 && days.Contains(m.Date.Day)
                select m).ToList();

            File.Delete(
                @"D:\Views\Learning\Svn\tests\FeesCalc\FeesCalculator.Tests\Data\SequenceMessageTestData.txt"
                );
            foreach (OperationMessage operationMessage in testdata)
            {
                if (operationMessage is IncommingPaymentMessage)
                {
                    String testData = JsonHelper.ToJson((IncommingPaymentMessage) operationMessage);
                    File.AppendAllText(
                        @"D:\Views\Learning\Svn\tests\FeesCalc\FeesCalculator.Tests\Data\SequenceMessageTestData.txt",
                        "IncommingPaymentMessage|" + testData + Environment.NewLine);
                }

                if (operationMessage is SellMessage)
                {
                    String testData = JsonHelper.ToJson((SellMessage) operationMessage);
                    File.AppendAllText(
                        @"D:\Views\Learning\Svn\tests\FeesCalc\FeesCalculator.Tests\Data\SequenceMessageTestData.txt",
                        "SellMessage|" + testData + Environment.NewLine);
                }
            }
        }
    }
}

