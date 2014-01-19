namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
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
}