/*
 * Created by SharpDevelop.
 * User: jr.partizan
 * Date: 10.10.2011
 * Time: 23:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;

namespace FeesCalculator.ConsoleApplication
{
    internal class Program
    {
        private const String RootDataPath = @"C:\Views\Own\Svn\apps\FeesCalc\FeesCalculator.ConsoleApplication\Data";
        
        public static void Main(string[] args)
        {
            RateManager rateManager = new RateManager();
            rateManager.ImportRates(Path.Combine(RootDataPath, @"Rates\2010_usd_currecyRate.csv"));
            rateManager.ImportRates(Path.Combine(RootDataPath, @"Rates\2011_usd_currecyRate.csv"));
            rateManager.ImportRates(Path.Combine(RootDataPath, @"Rates\2012_usd_currecyRate.csv"));
            string mtbRoot = Path.Combine(RootDataPath, @"Mtb");
            string[] mtbPaymentsPaths = new[] { 
            @"mtb_data.xml", 
            @"mtb_data_21_03_2012_to_19_04_2012.xml",
            @"20120720.xml", 
            @"20121017.xml", 
            @"20130108.xml", 
            @"20120923.xml", 
            @"20120720_only_fees.xml" 
            };
            /* TODO: Skip bsb */

            String[] bsbIncommingPaymentsPath = new[]
                                                    {
                                                        Path.Combine(RootDataPath, @"Bsb\csv\incomming\3013008537032 _bsb_2010_year.csv"),
                                                        Path.Combine(RootDataPath, @"Bsb\csv\incomming\3013008537032 _bsb_2011_year.csv"),

                                                    };
            
            String[] bsbSellPaymentsPath = new[]
                                                    {
                                                        Path.Combine(RootDataPath, @"Bsb\csv\sells\3013008537003 _bsb_2010_year.csv"),
                                                        Path.Combine(RootDataPath, @"Bsb\csv\sells\3013008537003 _bsb_2011_year.csv"),

                                                    };

            String[] bsbFreeSellPaymentsPath = new[]
                                                    {
                                                        Path.Combine(RootDataPath, @"Bsb\csv\sells\free\3013008537016 _bsb_2010_year.csv"),
                                                        Path.Combine(RootDataPath, @"Bsb\csv\sells\free\3013008537016 _bsb_2011_year.csv"),
                                                    };

            List<OperationMessage> operationMessages = new List<OperationMessage>();

            /* TODO: Skip bsb */
            BsbAdapter bsbAdapter = new BsbAdapter(bsbIncommingPaymentsPath, bsbSellPaymentsPath, bsbFreeSellPaymentsPath, rateManager);
            operationMessages.AddRange(bsbAdapter.GetMessages());

            foreach (var mtbPaymentsPath in mtbPaymentsPaths)
            {
                MtbAdapter mtbAdapter = new MtbAdapter(Path.Combine(mtbRoot, mtbPaymentsPath), rateManager);
                operationMessages.AddRange(mtbAdapter.GetMessages());
            }
           
            ArrivalConsumptionManager arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);

            arrivalConsumptionManager.Calculate(operationMessages);
            arrivalConsumptionManager.Close();

            ArrivalConsumptionPresentation arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
            AddTaxInfo(operationMessages, arrivalConsumptionManager.Quarters);
            arrivalConsumptionPresentation.Render(arrivalConsumptionManager.Quarters);
        }

        private static void AddTaxInfo(List<OperationMessage> operationMessages, Dictionary<QuarterKey, Quarter> quarters)
        {
            foreach (var operationMessage in operationMessages)
            {
                TaxSellMessage taxSellMessage = operationMessage as TaxSellMessage;
                if (taxSellMessage != null)
                {
                    var quarterKey = new QuarterKey() { Type = taxSellMessage.QuarterType, YearNumber = taxSellMessage.YearNumber };
                    if (quarters.ContainsKey(quarterKey))
                    {
                        quarters[quarterKey].PaidTaxAmount = taxSellMessage.Amount;
                    }
                }
            }
        }

        private static void WriteTestData(List<OperationMessage> operationMessages)
        {
            var days = new List<int>(new int[]
            {
                15,22,31
            });
            List<OperationMessage> testdata = (from m in operationMessages
                                               where m.Date.Month == 8 && m.Date.Year == 2011 && days.Contains(m.Date.Day)
                                               select m).ToList();

            File.Delete(
                        @"D:\Views\Learning\Svn\tests\FeesCalc\FeesCalculator.Tests\Data\SequenceMessageTestData.txt"
                        );
            foreach (var operationMessage in testdata)
            {
                if (operationMessage is IncommingPaymentMessage)
                {
                    String testData = JsonHelper.ToJson((IncommingPaymentMessage)operationMessage);
                    File.AppendAllText(
                        @"D:\Views\Learning\Svn\tests\FeesCalc\FeesCalculator.Tests\Data\SequenceMessageTestData.txt",
                         "IncommingPaymentMessage|" + testData + Environment.NewLine);
                }

                if (operationMessage is SellMessage)
                {
                    String testData = JsonHelper.ToJson((SellMessage)operationMessage);
                    File.AppendAllText(
                        @"D:\Views\Learning\Svn\tests\FeesCalc\FeesCalculator.Tests\Data\SequenceMessageTestData.txt",
                        "SellMessage|" + testData + Environment.NewLine);
                }
            }
            
        }
    }
}

