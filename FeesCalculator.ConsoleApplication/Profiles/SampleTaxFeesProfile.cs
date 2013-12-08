using System;
using System.Collections.Generic;
using System.IO;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.ConsoleApplication.Configuration;
using FeesCalculator.ConsoleApplication.Utils;
using FeesCalculator.Data;
using Newtonsoft.Json;

namespace FeesCalculator.ConsoleApplication.Profiles
{
    public class SampleTaxFeesProfile : ITaxFeesProfile
    {
        
        private readonly RateManager _rateManager;
        private readonly IHelperUtils _helperUtils;

        public SampleTaxFeesProfile(RateManager rateManager, IHelperUtils helperUtils)
        {
            _rateManager = rateManager;
            _helperUtils = helperUtils;
        }

        public void Init()
        {
        }

        public IEnumerable<OperationMessage> GetOperations()
        {
            List<OperationMessage> operationMessage = new List<OperationMessage>();
            operationMessage.AddRange(GetOperationsFor3Quarter2012());
            operationMessage.AddRange(GetOperationsFor3Quarter2013());
            
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore};
            jsonSerializerSettings.Converters.Add
                (new Newtonsoft.Json.Converters.StringEnumConverter());
            
            File.WriteAllText(@"operationMessage.json", JsonConvert.SerializeObject(operationMessage, 
                Formatting.Indented, jsonSerializerSettings));
            
            return operationMessage;
        }

        private IEnumerable<OperationMessage> GetOperationsFor3Quarter2013()
        {
            int refDocument = 10;
            const int firstIncomeAmount = 1500;
            var firstIncomeDate = new DateTime(2013, 9, 2);
            var mandatorySellaryDate = new DateTime(2013, 9, 9);
            var freeSellaryDate = new DateTime(2013, 9, 12);
            var freeSellaryDate2 = new DateTime(2013, 9, 13);
          
            return new List<OperationMessage>
            {
                new IncommingPaymentMessage()
                {
                    Amount = firstIncomeAmount,
                    Date = firstIncomeDate,
                    Rate = _rateManager.GetNationalRate(firstIncomeDate), 
                    RefDocument = refDocument++.ToString()
                },
                new SellMessage()
                {
                    Amount = (decimal) (firstIncomeAmount*0.3),
                    Date = mandatorySellaryDate,
                    SellType = SellType.Mandatory,
                    RefDocument = refDocument++.ToString()
                },
                new SellMessage()
                {
                    Amount = (decimal) (firstIncomeAmount*0.4),
                    Date = freeSellaryDate,
                    SellType = SellType.Free,
                    Rate = _rateManager.GetNationalRate(freeSellaryDate) + 100, // national rate + 100 rubley - simulate free sellary with benefit
                    RefDocument = refDocument++.ToString()
                },
                new SellMessage()
                {
                    Amount = (decimal) (firstIncomeAmount*0.2),
                    Date = freeSellaryDate2,
                    SellType = SellType.Free,
                    Rate = _rateManager.GetNationalRate(freeSellaryDate2) - 100, // national rate + 100 rubley - simulate free sellary without benefits
                    RefDocument = refDocument++.ToString()
                },
                new TaxSellMessage()
                            {
                                Amount = 190000,
                                QuarterType = QuarterType.Three,
                                YearNumber = 2013,
                                RefDocument = "12",
                                Date = freeSellaryDate2.AddDays(10)
                            }
            };

        }

        private IEnumerable<OperationMessage> GetOperationsFor3Quarter2012()
        {
            int refDocument = 100;
            const int firstIncomeAmount = 1000;
            var firstIncomeDate = new DateTime(2012, 9, 2);
            var mandatorySellaryDate = new DateTime(2012, 9, 9);
            var freeSellaryDate = new DateTime(2012, 9, 12);
            var freeSellaryDate2 = new DateTime(2012, 9, 13);

            return new List<OperationMessage>
            {
                new IncommingPaymentMessage()
                {
                    Amount = firstIncomeAmount,
                    Date = firstIncomeDate,
                    Rate = _rateManager.GetNationalRate(firstIncomeDate),
                    RefDocument = refDocument++.ToString()
                },
                new SellMessage()
                {
                    Amount = (decimal) (firstIncomeAmount*0.3),
                    Date = mandatorySellaryDate,
                    SellType = SellType.Mandatory,
                    RefDocument = refDocument++.ToString()
                },
                new SellMessage()
                {
                    Amount = (decimal) (firstIncomeAmount*0.4),
                    Date = freeSellaryDate,
                    SellType = SellType.Free,
                    Rate = _rateManager.GetNationalRate(freeSellaryDate) + 100, // national rate + 100 rubley - simulate free sellary with benefit
                    RefDocument = refDocument++.ToString()
                },
                new SellMessage()
                {
                    Amount = (decimal) (firstIncomeAmount*0.2),
                    Date = freeSellaryDate2,
                    SellType = SellType.Free,
                    Rate = _rateManager.GetNationalRate(freeSellaryDate2) - 100, // national rate + 100 rubley - simulate free sellary without benefits
                    RefDocument = refDocument++.ToString()
                },
                new TaxSellMessage()
                            {
                                Amount = 100000,
                                QuarterType = QuarterType.Three,
                                YearNumber = 2012,
                                RefDocument = refDocument++.ToString(),
                                Date = freeSellaryDate2.AddDays(10)
                            }
            };
        }
    }
}