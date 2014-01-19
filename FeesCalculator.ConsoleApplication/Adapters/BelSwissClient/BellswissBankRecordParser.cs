using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
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
}