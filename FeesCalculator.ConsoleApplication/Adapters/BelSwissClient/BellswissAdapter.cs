using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.Data;

namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
    public class BellswissAdapter : IAdapter
    {
        private readonly Regex freeSellPattern = new Regex(@"ѕродажа ин. вал. согласно за€вке");
        private readonly IBellswissBankRecordParser exportParser;

        public BellswissAdapter(IBellswissBankRecordParser exportParser)
        {
            this.exportParser = exportParser;
        }

        public IEnumerable<OperationMessage> GetMessages(string dataDirectoryPath, List<string> files)
        {
            var paymentMessages = new List<OperationMessage>();
            var sellMessages = new List<OperationMessage>();
            foreach (string file in files)
            {
                string path = Path.Combine(dataDirectoryPath, file);
                using (FileStream fileStream = File.Open(path, FileMode.Open))
                {
                    List<Record> allRecords = exportParser.GetRecords(fileStream).ToList();
                    paymentMessages.AddRange(allRecords.Where(r => r.Title.Contains("BARCELLONA LABS INC")).
                        Select(r => new IncommingPaymentMessage
                        {
                            Amount = r.CreditCurrency,
                            Comment = r.Title,
                            Date = r.Date,
                            RefDocument = ""
                        }));
                    sellMessages.AddRange(allRecords.Where(r => r.Title.Contains("после об€зательной продажи")).
                        Select(r => new SellMessage
                        {
                            SellType = SellType.Mandatory,
                            Date = r.Date,
                            Amount = r.DebitCurrency,
                            RefDocument = ""
                        }));
                    sellMessages.AddRange(allRecords.Where(r => r.Title.Contains("ѕродажа ин. вал. согласно за€вке"))
                        .Select(r => new SellMessage
                        {
                            SellType = SellType.Free,
                            Date = r.Date,
                            Amount = r.DebitCurrency,
                            RefDocument = ""
                        }));
                }
            }
            return paymentMessages.Union(sellMessages);
        }
    }
}