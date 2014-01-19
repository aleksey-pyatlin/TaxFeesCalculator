using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.ConsoleApplication.Configuration.Bsb;
using FeesCalculator.ConsoleApplication.Configuration.Common;
using FeesCalculator.ConsoleApplication.Configuration.Mtb;
using FeesCalculator.ConsoleApplication.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                    case AdapterFactory.MtbBsClnt:
                    {
                        var newadaptersConfigurator = new AdapterConfiguration<BaseAdapterConfiguration>
                        {
                            Factory = AdapterFactory.MtbBsClnt,
                            Configurator = new MtbAdapterConfigurator
                            {
                                RootFolder = adapterConfiguration.Configurator.RootFolder
                            },
                            Adapter = (configurator) =>
                                new MtbAdapter(rateManager),
                            Files = adapterConfiguration.Files
                        };
                        
                        restoredAdaptersConfigurator.Configurations.Add(newadaptersConfigurator);   
                        
                        break;
                    }
                    case AdapterFactory.BsbHtmlExportCsvImport:
                    {
                        var newadaptersConfigurator = new AdapterConfiguration<BaseAdapterConfiguration>
                        {
                            Factory = AdapterFactory.BsbHtmlExportCsvImport,
                            Configurator = new BsbAdapterConfigurator
                            {
                                BsbIncommingPaymentsPath = GetBsbFiles(adapterConfiguration.Configurator.ExtensionData, "BsbIncommingPaymentsPath"),
                                BsbSellPaymentsPath = GetBsbFiles(adapterConfiguration.Configurator.ExtensionData, "BsbSellPaymentsPath"),
                                BsbFreeSellPaymentsPath = GetBsbFiles(adapterConfiguration.Configurator.ExtensionData, "BsbFreeSellPaymentsPath"),
                            },
                            Adapter = (configurator) =>
                            {
                                var config = (BsbAdapterConfigurator)configurator;
                                return new BsbAdapter(config.BsbIncommingPaymentsPath,
                                    config.BsbSellPaymentsPath,
                                    config.BsbFreeSellPaymentsPath,
                                    rateManager);
                            }
                        };

                        restoredAdaptersConfigurator.Configurations.Add(newadaptersConfigurator);   
                        
                        break;
                    }
                    case AdapterFactory.BelSwissClient:
                    {
                        restoredAdaptersConfigurator.Configurations.Add(new AdapterConfiguration<BaseAdapterConfiguration>()
                        {
                            Factory = AdapterFactory.BelSwissClient,
                            Configurator = new BaseAdapterConfiguration
                            {
                                RootFolder = adapterConfiguration.Configurator.RootFolder
                            },
                            
                        });
                        break;
                    }
                    default:
                        throw new Exception(String.Format("This factory is not supported: {0}", adapterConfiguration.Factory));
                     
                }
                
            }

            taxFeesProfile.Init(restoredAdaptersConfigurator);
            
            return taxFeesProfile;
        }

        private static string[] GetBsbFiles(JObject extensionData, string key)
        {
            var files  = extensionData[key].ToObject<List<String>>();
            return files.ToArray();
        }
    }
}