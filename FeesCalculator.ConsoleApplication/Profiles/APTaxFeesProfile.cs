using System.Collections.Generic;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.ConsoleApplication.Configuration;
using FeesCalculator.ConsoleApplication.Configuration.Bsb;
using FeesCalculator.ConsoleApplication.Configuration.Common;
using FeesCalculator.ConsoleApplication.Configuration.Mtb;
using FeesCalculator.ConsoleApplication.Utils;

namespace FeesCalculator.ConsoleApplication.Profiles
{
    public class APTaxFeesProfile : BaseTaxFeesProfile
    {
        private readonly string _dataDirectoryPath;
        private readonly IRateManager _rateManager;
        private readonly IHelperUtils _helperUtils;
       
        public APTaxFeesProfile(IRateManager rateManager,
            IHelperUtils helperUtils)
        {
            const string dataDirectory = @"C:\Users\Aleksey\Dropbox\Payments\Data\";
            this._dataDirectoryPath = dataDirectory;
            this._rateManager = rateManager;
            this._helperUtils = helperUtils;
        }

        public override void Init(AdaptersConfigurator adaptersConfigurator)
        {
            base.Init(adaptersConfigurator);
            _rateManager.ImportRates(_helperUtils.GetPath(_dataDirectoryPath,
                @"Rates\2010_usd_currecyRate.csv"));
            _rateManager.ImportRates(_helperUtils.GetPath(_dataDirectoryPath,
                @"Rates\2011_usd_currecyRate.csv"));
            _rateManager.ImportRates(_helperUtils.GetPath(_dataDirectoryPath,
                @"Rates\2012_usd_currecyRate.csv"));
            
            //--------------------------------------------------

            AdaptersConfigurator.Configurations.Add(new AdapterConfiguration<BaseAdapterConfiguration>
            {
                Factory = AdapterFactory.MtbBsClnt,
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
                    @"20130930.xml",
                    @"20131231.xml"
                })
            });

            AdaptersConfigurator.Configurations.Add(new AdapterConfiguration<BaseAdapterConfiguration>
            {
                Factory = AdapterFactory.BsbHtmlExportCsvImport,
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
