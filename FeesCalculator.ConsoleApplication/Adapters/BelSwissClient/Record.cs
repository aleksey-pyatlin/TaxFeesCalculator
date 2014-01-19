using System;

namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
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
}