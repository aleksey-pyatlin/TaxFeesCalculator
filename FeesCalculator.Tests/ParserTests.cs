using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.BussinnesLogic.Reports;
using FeesCalculator.ConsoleApplication.Adapters;
using FeesCalculator.Data;
using NUnit.Framework;

namespace FeesCalculator.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Calculate()
        {
            var adapter = new BellswissAdapter(new BellswissBankRecordParser(new BellwissBankExportParser()));
            var rateManager = new RateManager();
            var operationMessages = new List<OperationMessage>();
            var directory = @"C:\Dropbox\Apps\TextFees\4th Quarter 2013\";
            var files = Directory.GetFiles(directory).Select(Path.GetFileName).ToList();

            operationMessages.AddRange(adapter.GetMessages(directory, files));
            AddNationalRateToIncommingMessages(operationMessages, rateManager);
            var arrivalConsumptionManager = new ArrivalConsumptionManager(rateManager);
            arrivalConsumptionManager.Calculate(operationMessages);
            arrivalConsumptionManager.Close();

            var arrivalConsumptionPresentation = new ArrivalConsumptionPresentation();
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


        [Test]
        public void ParseRecords()
        {
            string folder = @"C:\Dropbox\Apps\TextFees\4th Quarter 2013\";
            Directory.GetFiles(folder).ToList().SelectMany(file =>
            {
                var fileStream = new FileStream(file, FileMode.Open);
                var parser = new BellwissBankExportParser();
                var recordParser = new BellswissBankRecordParser(parser);
                return recordParser.GetRecords(fileStream);
            }).OrderBy(r => r.Date).
                ToList().
                ForEach(record => Console.WriteLine("{0}   {1}   -{2}({3}) +{4}({5})", 
                record.Date.ToString("dd MMM yyyy"), record.Title,
                record.DebitCurrency, record.DebitRuble, record.CreditCurrency, record.CreditRuble));
        }

        [Test]
        public void ParseTokens()
        {
            var parser =
                new BellwissBankExportParser();

            foreach (Token token in parser.GetTokens(null))
            {
                Console.WriteLine(token.Header + "   " + token.Value);
            }
        }
    }

    public class BellswissAdapter : IAdapter
    {
        private readonly Regex freeSellPattern = new Regex(@"Продажа ин. вал. согласно заявке");
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
                    sellMessages.AddRange(allRecords.Where(r => r.Title.Contains("после обязательной продажи")).
                        Select(r => new SellMessage
                        {
                            SellType = SellType.Mandatory,
                            Date = r.Date,
                            Amount = r.DebitCurrency,
                            RefDocument = ""
                        }));
                    sellMessages.AddRange(allRecords.Where(r => r.Title.Contains("Продажа ин. вал. согласно заявке"))
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


    public interface IBellswissBankRecordParser
    {
        IEnumerable<Record> GetRecords(Stream stream);
    }

    public class BellswissBankRecordParser : IBellswissBankRecordParser
    {
        private readonly IBellwissBankExportParser parser;

        public BellswissBankRecordParser(IBellwissBankExportParser parser)
        {
            this.parser = parser;
        }

        public IEnumerable<Record> GetRecords(Stream stream)
        {
            DateTime currDate = DateTime.MinValue;
            Decimal credit = 0;
            Decimal creditRuble = 0;
            Decimal debit = 0;
            Decimal debitRuble = 0;
            string target = null;

            foreach (Token token in parser.GetTokens(stream))
            {
                if (token.Header == "DocDate")
                {
                    currDate = DateTime.Parse(token.Value, CultureInfo.GetCultureInfo("ru-RU"));
                }
                if (token.Header == "Nazn")
                {
                    target = token.Value;
                }
                if (token.Header == "DebQ")
                {
                    debit = Decimal.Parse(token.Value);
                }
                if (token.Header == "CreQ")
                {
                    credit = Decimal.Parse(token.Value);
                }
                if (token.Header == "Deb" || token.Header == "Db")
                {
                    debitRuble = Decimal.Parse(token.Value);
                }
                if (token.Header == "Cre" || token.Header == "Credit")
                {
                    creditRuble = Decimal.Parse(token.Value);
                    yield return new Record(target, currDate, debit, credit, debitRuble, creditRuble);
                }
            }
        }
    }

    public class Record
    {
        public Record(string title, DateTime date, Decimal debitCurrency, Decimal creditCurrency, Decimal debitRuble,
            Decimal creditRuble)
        {
            CreditRuble = creditRuble;
            DebitRuble = debitRuble;
            Title = title;
            Date = date;
            DebitCurrency = debitCurrency;
            CreditCurrency = creditCurrency;
        }

        public DateTime Date
        {
            get;
            private set;
        }

        public string Title
        {
            get;
            private set;
        }

        public Decimal DebitCurrency
        {
            get;
            private set;
        }

        public Decimal CreditCurrency
        {
            get;
            private set;
        }

        public Decimal DebitRuble
        {
            get;
            private set;
        }

        public Decimal CreditRuble
        {
            get;
            private set;
        }
    }

    public interface IBellwissBankExportParser
    {
        IEnumerable<Token> GetTokens(Stream stream);
    }

    public class BellwissBankExportParser : IBellwissBankExportParser
    {
        private readonly Regex entryRegex = new Regex(@"(.+)=(.*)");
        private readonly Regex ignoreString = new Regex(@"\*+.+");
        private readonly Regex inputRegex = new Regex(@"IN_PARAM");
        private readonly Regex outRegex = new Regex(@"OUT_PARAM");
        private readonly StreamReader reader;

        public IEnumerable<Token> GetTokens(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.GetEncoding(1251)))
            {
                string allText = reader.ReadToEnd();
                allText = Regex.Replace(allText, ignoreString.ToString(), String.Empty);
                allText = allText.Replace("\r\n", String.Empty);
                bool isInputParametersSection = false;

                string[] nextLines = allText.Split(new[] {"[", "]", "^"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (
                    string nextLine in nextLines)
                {
                    if (nextLine == "\n" || nextLine == "\r\n")
                    {
                        continue;
                    }
                    if (nextLine.StartsWith("#"))
                    {
                        continue;
                    }
                    if (inputRegex.Match(nextLine).Success)
                    {
                        isInputParametersSection = true;
                        continue;
                    }
                    if (outRegex.Match(nextLine).Success)
                    {
                        isInputParametersSection = false;
                        continue;
                    }
                    Match entryMatch = entryRegex.Match(nextLine);
                    if (entryMatch.Success)
                    {
                        yield return new Token(entryMatch.Groups[1].Value, entryMatch.Groups[2].Value,
                            isInputParametersSection ? TokenType.Input : TokenType.Output);
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot parse string: " + nextLine);
                    }
                }
            }
        }
    }

    public class Token
    {
        public Token(string header, string value)
        {
            Header = header;
            Value = value;
            TokenType = TokenType.Output;
        }

        public Token(string header, string value, TokenType tokenType)
        {
            Value = value;
            Header = header;
            TokenType = tokenType;
        }

        public TokenType TokenType
        {
            get;
            private set;
        }

        public string Header
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }
    }

    public enum TokenType
    {
        Input,
        Output
    }
}