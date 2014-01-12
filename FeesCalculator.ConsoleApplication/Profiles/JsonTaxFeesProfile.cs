using System;
using System.Collections.Generic;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.ConsoleApplication.Configuration;
using FeesCalculator.ConsoleApplication.Configuration.Common;
using FeesCalculator.ConsoleApplication.Utils;
using FeesCalculator.Data;

namespace FeesCalculator.ConsoleApplication.Profiles
{
    //TODO: Move this to unit test for Jon Doe profile.
    public class JsonTaxFeesProfile : BaseTaxFeesProfile
    {
        
        private readonly RateManager _rateManager;
        private readonly IHelperUtils _helperUtils;

        public JsonTaxFeesProfile(RateManager rateManager, IHelperUtils helperUtils)
        {
            _rateManager = rateManager;
            _helperUtils = helperUtils;
        }

        public override void Init(AdaptersConfigurator adaptersConfigurator)
        {
            AdaptersConfigurator.Configurations.Add(new AdapterConfiguration<BaseAdapterConfiguration>
            {
                Factory = AdapterFactory.JsonFeesCalc,
                Configurator = new JsonAdapterConfigurator
                {
                    RootFolder = _helperUtils.GetPath(@"..\..\..\FeesCalculator.ConsoleApplication\Data", @"SimpleAdapter"),
                },
                Adapter = (configurator) =>
                    new JsonPaymentAdapter(),
                Files = new List<string>(new[]
                {
                    @"3_Quarter_2012.json",
                    @"3_Quater_2013.json"
                })
            });
        }

//        public override IEnumerable<OperationMessage> GetOperations()
//        {
//            List<OperationMessage> operationMessage = new List<OperationMessage>();
//            var quarter2012 = GetOperationsFor3Quarter2012();
//            operationMessage.AddRange(quarter2012.GetMessages());
//
//            var quarter2013 = GetOperationsFor3Quarter2013();
//            
//            operationMessage.AddRange(quarter2013.GetMessages());
//            
//            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings {DefaultValueHandling = DefaultValueHandling.Ignore};
//            jsonSerializerSettings.Converters.Add
//                (new Newtonsoft.Json.Converters.StringEnumConverter());
//
//
//            File.WriteAllText(@"..\..\..\FeesCalculator.ConsoleApplication\Data\Profiles\Jon.Doe.Profile.json", JsonConvert.SerializeObject(AdaptersConfigurator,
//                Formatting.Indented, jsonSerializerSettings));
//
//
//            File.WriteAllText(@"..\..\..\FeesCalculator.ConsoleApplication\Data\SimpleAdapter\3_Quarter_2012.json", JsonConvert.SerializeObject(quarter2012, 
//                Formatting.Indented, jsonSerializerSettings));
//
//            File.WriteAllText(@"..\..\..\FeesCalculator.ConsoleApplication\Data\SimpleAdapter\3_Quater_2013.json", JsonConvert.SerializeObject(quarter2013, 
//                Formatting.Indented, jsonSerializerSettings));
//            
//            return operationMessage;
//        }

        private OperationMessageContainer GetOperationsFor3Quarter2013()
        {
            int refDocument = 10;
            const int firstIncomeAmount = 1500;
            var firstIncomeDate = new DateTime(2013, 9, 2);
            var mandatorySellaryDate = new DateTime(2013, 9, 9);
            var freeSellaryDate = new DateTime(2013, 9, 12);
            var freeSellaryDate2 = new DateTime(2013, 9, 13);

            IList<IncommingPaymentMessage> incommingPaymentMessage = new List<IncommingPaymentMessage>();
            incommingPaymentMessage.Add(new IncommingPaymentMessage()
                {
                    Amount = firstIncomeAmount,
                    Date = firstIncomeDate,
                    RefDocument = refDocument++.ToString()
                });

            List<SellMessage> sellMessages = new List<SellMessage>();
            sellMessages.AddRange( new[]{
            
                
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
                }});

            List<TaxPaymentMessage> taxSellMessages = new List<TaxPaymentMessage>
            {
                new TaxPaymentMessage()
                {
                    Amount = 210000,
                    QuarterType = QuarterType.Three,
                    YearNumber = 2013,
                    RefDocument = "12",
                    Date = freeSellaryDate2.AddDays(10)
                }
            };

            OperationMessageContainer operationMessages = new OperationMessageContainer();
            operationMessages.IncommingPaymentMessages = incommingPaymentMessage;
            operationMessages.SellMessages = sellMessages;
            operationMessages.TaxPaymentMessages = taxSellMessages;
            return operationMessages;
        }

        private OperationMessageContainer GetOperationsFor3Quarter2012()
        {
            int refDocument = 100;
            const int firstIncomeAmount = 1000;
            var firstIncomeDate = new DateTime(2012, 9, 2);
            var mandatorySellaryDate = new DateTime(2012, 9, 9);
            var freeSellaryDate = new DateTime(2012, 9, 12);
            var freeSellaryDate2 = new DateTime(2012, 9, 13);

            List<IncommingPaymentMessage> incommingPaymentMessage = new List<IncommingPaymentMessage>
            {
                new IncommingPaymentMessage()
                {
                    Amount = firstIncomeAmount,
                    Date = firstIncomeDate,
                    RefDocument = refDocument++.ToString()
                },
                
            };
            List<TaxPaymentMessage> taxPaymentMessages = new List<TaxPaymentMessage>
            {
                 new TaxPaymentMessage()
                            {
                                Amount = 592000,
                                QuarterType = QuarterType.Three,
                                YearNumber = 2012,
                                RefDocument = refDocument++.ToString(),
                                Date = freeSellaryDate2.AddDays(10)
                            }
            };

            List<SellMessage> sellMessages = new List<SellMessage>
            {
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
                }
               
            };

            OperationMessageContainer operationMessages = new OperationMessageContainer();
            operationMessages.IncommingPaymentMessages = incommingPaymentMessage;
            operationMessages.SellMessages = sellMessages;
            operationMessages.TaxPaymentMessages = taxPaymentMessages;
            return operationMessages;
        }
    }
}