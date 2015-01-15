using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeesCalculator.BussinnesLogic.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeesCalculator.ConsoleApplication.Adapters
{
    public class JsonPaymentAdapter : IAdapter
    {
        public JsonSerializerSettings GetJsonSettigns()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            jsonSerializerSettings.Converters.Add(new StringEnumConverter());

            return jsonSerializerSettings; 
        }

        public OperationMessageContainer GetContainer(string dataDirectoryPath, string file)
        {
            String paymentsPath = Path.Combine(dataDirectoryPath, file);
            string content = File.ReadAllText(paymentsPath);
            return JsonConvert.DeserializeObject<OperationMessageContainer>(content,
                       GetJsonSettigns());
        }

        public IEnumerable<OperationMessage> GetMessages(string dataDirectoryPath, List<string> files)
        {
            var operationMessages = new List<OperationMessage>();
            foreach (String file in files)
            {
                operationMessages.AddRange(GetContainer(dataDirectoryPath, file).GetMessages());
            }

            return operationMessages;
        }
    }
}
