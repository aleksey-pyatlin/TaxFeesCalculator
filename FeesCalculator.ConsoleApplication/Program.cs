using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.ConsoleApplication.Configuration;
using FeesCalculator.ConsoleApplication.Profiles;
using FeesCalculator.ConsoleApplication.Utils;
using Newtonsoft.Json;

namespace FeesCalculator.ConsoleApplication
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            IHelperUtils helperUtils = new HelperUtils();
            var rateManager = new RateManager();
            
            var profiles = new List<ITaxFeesProfile>();
            
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
            jsonSerializerSettings.Converters.Add
                (new Newtonsoft.Json.Converters.StringEnumConverter());
            
            const bool useAPProfile = false;
            if (useAPProfile)
            {
                var apProfile = new APTaxFeesProfile(rateManager, helperUtils);
                profiles.Add(apProfile);
            }

            SampleTaxFeesProfile sampleTaxFeesProfile = new SampleTaxFeesProfile(rateManager, helperUtils);
            profiles.Add(sampleTaxFeesProfile);
            Run(profiles, rateManager);
        }

        private static void Run(IEnumerable<ITaxFeesProfile> profiles, RateManager rateManager)
        {
            var operationMessages = new List<OperationMessage>();
            foreach (var profile in profiles)
            {
                profile.Init();
                operationMessages.AddRange(profile.GetOperations());
            }

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
    }
}

