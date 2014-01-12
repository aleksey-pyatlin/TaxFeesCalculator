using System;
using System.IO;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.ConsoleApplication.Configuration.Common;
using FeesCalculator.ConsoleApplication.Utils;
using Newtonsoft.Json;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public class ProfileFactory 
    {
        public static ITaxFeesProfile GetProfile(RateManager rateManager, IHelperUtils helperUtils, string profilePath)
        {
            var jsonSerializerSettings = JsonUtils.GetJsonSettings();
            AdaptersConfigurator adaptersConfigurator = JsonConvert.DeserializeObject<AdaptersConfigurator>(File.ReadAllText(profilePath), jsonSerializerSettings);
            ITaxFeesProfile taxFeesProfile = new BaseTaxFeesProfile();
            AdaptersConfigurator restoredAdaptersConfigurator = new AdaptersConfigurator();

            foreach (var adapterConfiguration in adaptersConfigurator.Configurations)
            {
                switch (adapterConfiguration.Factory)
                {
                    case AdapterFactory.JsonFeesCalc:
                    {
                        var newadaptersConfigurator = new AdapterConfiguration<BaseAdapterConfiguration>
                        {
                            Adapter = configuration => new JsonPaymentAdapter(),
                            Configurator = new JsonAdapterConfigurator()
                            {
                                RootFolder = adapterConfiguration.Configurator.RootFolder
                            },
                            Files = adapterConfiguration.Files
                        };

                        restoredAdaptersConfigurator.Configurations.Add(newadaptersConfigurator);   
                        
                        break;
                    }
                    default:
                        throw new Exception(String.Format("This factory is not supported: {0}", adapterConfiguration.Factory));
                     
                }
                
            }

            taxFeesProfile.Init(restoredAdaptersConfigurator);
            
            return taxFeesProfile;
        }
    }
}