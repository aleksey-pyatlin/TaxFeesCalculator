using System.Collections.Generic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.ConsoleApplication.Adapters;

namespace FeesCalculator.ConsoleApplication.Configuration
{
    public class BaseTaxFeesProfile : ITaxFeesProfile
    {
        public AdaptersConfigurator AdaptersConfigurator { get; set; }

        public virtual void Init(AdaptersConfigurator adaptersConfigurator)
        {
            AdaptersConfigurator = adaptersConfigurator;
        }

        public virtual IEnumerable<OperationMessage> GetOperations()
        {
            List<OperationMessage> operationMessages = new List<OperationMessage>();
            foreach (var configuration in AdaptersConfigurator.Configurations)
            {
                IAdapterConfiguration adapterConfiguration = configuration.Configurator;
                IAdapter adapter = configuration.Adapter(adapterConfiguration);
                operationMessages.AddRange(adapter.GetMessages(adapterConfiguration.RootFolder,
                    configuration.Files));
            }

            return operationMessages;
           
        }
    }
}