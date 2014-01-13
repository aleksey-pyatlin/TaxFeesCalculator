using System;
using System.Collections.Generic;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Exceptions;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.ConsoleApplication.Configuration;
using FeesCalculator.ConsoleApplication.Utils;

namespace FeesCalculator.ConsoleApplication
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            WrapExceptionHadnling(delegate() 
            {
                IHelperUtils helperUtils = new HelperUtils();
                var rateManager = new RateManager();
                var profiles = new List<ITaxFeesProfile>();

                //TODO: Move this setting to command line parameter - /profile:"..\..\Data\Profiles\Jon.Doe.Profile.json"
                const string profilePath = @"..\..\Data\Profiles\Jon.Doe.Profile.json";

                ITaxFeesProfile profile = ProfileFactory.GetProfile(rateManager, helperUtils, profilePath);
                profiles.Add(profile);

                Run(profiles, rateManager);
            });
        }

        private static void WrapExceptionHadnling(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (MissingInternetConnectionException missingInternetConnectionException)
            {
                Console.WriteLine("Error: {0}", missingInternetConnectionException);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        private static void Run(IEnumerable<ITaxFeesProfile> profiles, RateManager rateManager)
        {
            var operationMessages = new List<OperationMessage>();
            foreach (var profile in profiles)
            {
                operationMessages.AddRange(profile.GetOperations());
            }

            AddNationalRateToIncommingMessages(operationMessages, rateManager);

            var arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);

            arrivalConsumptionManager.Calculate(operationMessages);
            arrivalConsumptionManager.Close();

            var arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
            AddTaxInfo(operationMessages, arrivalConsumptionManager.Quarters);
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

        private static void AddTaxInfo(IEnumerable<OperationMessage> operationMessages,
            Dictionary<QuarterKey, Quarter> quarters)
        {
            foreach (OperationMessage operationMessage in operationMessages)
            {
                var taxSellMessage = operationMessage as TaxPaymentMessage;
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

