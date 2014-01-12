using System;
using System.Collections.Generic;
using System.IO;
using FeesCalculator.BussinnesLogic.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeesCalculator.ConsoleApplication.Adapters
{
    public class JsonPaymentAdapter : IAdapter
    {
        public List<OperationMessage> GetMessages(string dataDirectoryPath, List<string> files)
        {
            var operationMessages = new List<OperationMessage>();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            jsonSerializerSettings.Converters.Add
                (new StringEnumConverter());


            foreach (string file in files)
            {
                String paymentsPath = Path.Combine(dataDirectoryPath, file);
                string content = File.ReadAllText(paymentsPath);
                OperationMessageContainer operationMessageContainer = JsonConvert.DeserializeObject<OperationMessageContainer>(content,
                    jsonSerializerSettings);
                operationMessages.AddRange(operationMessageContainer.GetMessages());
            }

            return operationMessages;
        }
    }
}
