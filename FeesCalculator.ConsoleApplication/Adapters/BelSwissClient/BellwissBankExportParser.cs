using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FeesCalculator.ConsoleApplication.Adapters.BelSwissClient
{
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
}