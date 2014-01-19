using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.ConsoleApplication.Adapters.BelSwissClient;
using NUnit.Framework;

namespace FeesCalculator.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Calculate()
        {
            var adapter = new BellswissAdapter(new BellswissBankRecordParser(new BellwissBankExportParser()));
            var rateManager = new RateManager();
            var operationMessages = new List<OperationMessage>();
            var directory = @"C:\Dropbox\Apps\TextFees\4th Quarter 2013\";
            var files = Directory.GetFiles(directory).Select(Path.GetFileName).ToList();

            operationMessages.AddRange(adapter.GetMessages(directory, files));
            AddNationalRateToIncommingMessages(operationMessages, rateManager);
            var arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);
            arrivalConsumptionManager.Calculate(operationMessages);
            arrivalConsumptionManager.Close();

            var arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
            arrivalConsumptionPresentation.Render(arrivalConsumptionManager.Quarters);
        }

        private static void AddNationalRateToIncommingMessages(List<OperationMessage> operationMessages, RateManager rateManager)
        {
            foreach (var operationMessage in operationMessages)
            {
                var incommingMessage = operationMessage as IncommingPaymentMessage;
                if (incommingMessage != null && incommingMessage.Rate <= 0)
                {
                    incommingMessage.Rate = rateManager.GetNationalRate(incommingMessage.Date);
                }

                var sellMessage = operationMessage as SellMessage;
                if (sellMessage != null && sellMessage.Rate <= 0)
                {
                    sellMessage.Rate = rateManager.GetNationalRate(sellMessage.Date);
                }
            }
        }


        [Test]
        public void ParseRecords()
        {
            string folder = @"C:\Dropbox\Apps\TextFees\4th Quarter 2013\";
            Directory.GetFiles(folder).ToList().SelectMany(file =>
            {
                var fileStream = new FileStream(file, FileMode.Open);
                var parser = new BellwissBankExportParser();
                var recordParser = new BellswissBankRecordParser(parser);
                return recordParser.GetRecords(fileStream);
            }).OrderBy(r => r.Date).
                ToList().
                ForEach(record => Console.WriteLine("{0}   {1}   -{2}({3}) +{4}({5})", 
                record.Date.ToString("dd MMM yyyy"), record.Title,
                record.DebitCurrency, record.DebitRuble, record.CreditCurrency, record.CreditRuble));
        }

        [Test]
        public void ParseTokens()
        {
            var parser =
                new BellwissBankExportParser();

            foreach (Token token in parser.GetTokens(null))
            {
                Console.WriteLine(token.Header + "   " + token.Value);
            }
        }
    }
}