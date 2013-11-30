using System.Collections.Generic;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.ConsoleApplication.Configuration;
using FeesCalculator.ConsoleApplication.Configuration.Bsb;
using FeesCalculator.ConsoleApplication.Configuration.Mtb;
using FeesCalculator.ConsoleApplication.Utils;

namespace FeesCalculator.ConsoleApplication.Profiles
{
    public class APFeesConfigurator : IFeesConfigurator
    {
        private readonly string _dataDirectoryPath;
        private readonly IRateManager _rateManager;
        private readonly IHelperUtils _helperUtils;
        private AdaptersConfigurator _adaptersConfigurator;

        public APFeesConfigurator(IRateManager rateManager,
            IHelperUtils helperUtils)
        {
            const string dataDirectory = @"C:\Users\Aleksey\Dropbox\Payments\Data\";
            this._dataDirectoryPath = dataDirectory;
            this._rateManager = rateManager;
            this._helperUtils = helperUtils;
        }

        public IEnumerable<OperationMessage> GetOperations()
        {
            List<OperationMessage> operationMessages = new List<OperationMessage>();
            foreach (var configuration in _adaptersConfigurator.Configurations)
            {
                IAdapterConfiguration adapterConfiguration = configuration.Configurator;
                IAdapter adapter = configuration.Adapter(adapterConfiguration);
                operationMessages.AddRange(adapter.GetMessages(adapterConfiguration.RootFolder, 
                    configuration.Files));
            }

            return operationMessages;
        }

        public void Init()
        {
            _rateManager.ImportRates(_helperUtils.GetPath(_dataDirectoryPath,
                @"Rates\2010_usd_currecyRate.csv"));
            _rateManager.ImportRates(_helperUtils.GetPath(_dataDirectoryPath,
                @"Rates\2011_usd_currecyRate.csv"));
            _rateManager.ImportRates(_helperUtils.GetPath(_dataDirectoryPath,
                @"Rates\2012_usd_currecyRate.csv"));
            
            //--------------------------------------------------
            _adaptersConfigurator = new AdaptersConfigurator();
            _adaptersConfigurator.Configurations.Add(new AdapterConfiguration<IAdapterConfiguration>
            {
                Configurator = new MtbAdapterConfigurator
                {
                    RootFolder = _helperUtils.GetPath(_dataDirectoryPath, @"Mtb")
                },
                Adapter = (configurator) =>
                    new MtbAdapter(_rateManager),
                Files = new List<string>(new[]
                {
                    @"mtb_data.xml",
                    @"mtb_data_21_03_2012_to_19_04_2012.xml",
                    @"20120720.xml",
                    @"20121017.xml",
                    @"20120923.xml",
                    @"20120720_only_fees.xml",
                    @"20130108.xml",
                    @"20130409.xml",
                    @"20130717.xml",
                    @"20130909.xml",
                    @"20130930.xml"
                })
            });

            _adaptersConfigurator.Configurations.Add(new AdapterConfiguration<IAdapterConfiguration>
            {
                Configurator = new BsbAdapterConfigurator
                {
                    BsbIncommingPaymentsPath = new[]
                    {
                        _helperUtils.GetPath(_dataDirectoryPath, 
                            @"Bsb\csv\incomming\3013008537032 _bsb_2010_year.csv"),
                        _helperUtils.GetPath(_dataDirectoryPath, 
                            @"Bsb\csv\incomming\3013008537032 _bsb_2011_year.csv")
                    },
                    BsbSellPaymentsPath = new[]
                    {
                        _helperUtils.GetPath(_dataDirectoryPath, 
                            @"Bsb\csv\sells\3013008537003 _bsb_2010_year.csv"),
                        _helperUtils.GetPath(_dataDirectoryPath, 
                            @"Bsb\csv\sells\3013008537003 _bsb_2011_year.csv")
                    },
                    BsbFreeSellPaymentsPath = new[]
                    {
                        _helperUtils.GetPath(_dataDirectoryPath, 
                            @"Bsb\csv\sells\free\3013008537016 _bsb_2010_year.csv"),
                        _helperUtils.GetPath(_dataDirectoryPath, 
                            @"Bsb\csv\sells\free\3013008537016 _bsb_2011_year.csv")
                    }
                },
                Adapter = (configurator) =>
                {
                    var config = (BsbAdapterConfigurator)configurator;
                    return new BsbAdapter(config.BsbIncommingPaymentsPath,
                        config.BsbSellPaymentsPath,
                        config.BsbFreeSellPaymentsPath,
                        _rateManager);
                }
            });
        }
    }
}
