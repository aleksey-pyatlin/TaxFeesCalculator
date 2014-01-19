using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.ConsoleApplication.Adapters.Bsb;
using FeesCalculator.Data;

namespace FeesCalculator.ConsoleApplication.Adapters
{
    public class MtbAdapter : IAdapter
    {
        private readonly IRateManager _rateManager;
        
        private readonly Filter _filter;

        private const String RateInfoPattertn =
            @"USD\s{0,}(?<Amount>[\d.]{0,})\s{0,}/BYR\s{0,}(?<AmountNat>[\d.]{0,})\s{0,}\w{1,}\s{0,}(?<Rate>[\d.]{0,})\s{0,}";

        private const String FreeRateInfoPattern =
            @"–”¡. › ¬. USD\s{0,}(?<Amount>[\d.]{0,})\s{0,}. œŒ  ”–—”\s{0,}(?<Rate>\d{0,})\s{0,}.";

        private const String TaxMessagePattern = @"(?<QUARTER>\w{1,})\s{1,} ¬¿–“¿À\s{1,}(?<YEAR>\d{1,})√";

        public MtbAdapter(IRateManager rateManager)
        {
            _rateManager = rateManager;
            _filter = new Filter();
        }

        public IEnumerable<OperationMessage> GetMessages(string dataDirectoryPath, List<string> files)
        {
            List<OperationMessage> messages = new List<OperationMessage>();

            foreach (var fileName in files)
            {
                String paymentsPath = Path.Combine(dataDirectoryPath, fileName);
                String xmlContent = "";
    //
                xmlContent = File.ReadAllText(paymentsPath, Settings.FileEncoding);
    //            xmlContent = xmlContent.Replace("<?xml:stylesheet type=\"text/xsl\" ?>", "");
                //File.WriteAllText(_paymentsPath, xmlContent, Settings.FileEncoding);
                CultureInfo cultureInfo = new CultureInfo("ru-Ru");

                XElement payments = XElement.Parse(xmlContent);

                var statements = STATEMENTS.LoadFromFile(paymentsPath, Settings.FileEncoding);
                
            
                foreach (var statement in statements.STATEMENTBY)
                {
                    var statementDate = statement.STATEMENTDATE;
                    var dataactuality = statement.DATAACTUALITY;
                    //TODO: Verify DATAACTUALITY and StatementDate
                    var statementDateParse = DateTime.Parse(statementDate, cultureInfo);
                    var dataactualityParsed = DateTime.Parse(dataactuality, cultureInfo);
                    if (!(statementDateParse.Year == dataactualityParsed.Year
                        & statementDateParse.Month == dataactualityParsed.Month
                        & statementDateParse.Day == dataactualityParsed.Day))
                    {
                        //throw new ArgumentException("statement.DATAACTUALITY != statement.STATEMENTDATE");
                    }

                    var account = statement.ACCOUNT;
                    var openingBalance = statement.OPENINGBALANCE;
                    var closingBalance = statement.CLOSINGBALANCE;

                    var count = statement.CREDITDOCUMENTS.Count;
                    foreach (var document in statement.CREDITDOCUMENTS)
                    {
                        //incomming
                        if (account == 3013117183016 && document.AMOUNT != "0" &&
                            document.PAYER == "DEUTSCHE BANK TRUST COMPANY AMERICAS")
                        {
                            var operationDate = DateTime.Parse(statementDate, cultureInfo);
                            _filter.AddIncommingMessage(messages, new IncommingPaymentMessage()
                                                              {
                                                                  Date = operationDate,
                                                                  Amount = decimal.Parse(document.AMOUNT, cultureInfo),
                                                                  Rate = _rateManager.GetNationalRate(operationDate)
                                                              }, document.DOCUMENTNUMBER.ToString());
                        }

                        // white schema
                        //–”¡. › ¬. USD 1008. œŒ  ”–—” 8410.
                        if (account == 3013117180019 && statement.CURRCODE == 974 &&
                            document.GROUND.Contains("–”¡. › ¬. USD"))
                        {
                            var operationDate = DateTime.Parse(statementDate, cultureInfo);
                            var rateInfo = GetFreeSellOperationInfo(document.GROUND);

                            //check

                            SellMessage sellMessage = new SellMessage()
                                                          {
                                                              Date = operationDate,
                                                              Amount = rateInfo.Amount,
                                                              Rate = rateInfo.Rate,
                                                              SellType = SellType.Free
                                                          };
                            if (rateInfo.Amount != sellMessage.Amount)
                                throw new Exception(String.Format("RateInfo is incorrect.: {0}", document.GROUND));

                            _filter.AddSellMessage(messages, sellMessage);
                        }
                    }

                    //Console.WriteLine();
                    count = statement.DEBETDOCUMENTS.Count;
                    foreach (var document in statement.DEBETDOCUMENTS)
                    {
                        //mandatory sell
                        if (account == 3013117183016 && document.AMOUNT != "0" &&
                            document.GROUND.StartsWith("—Œ√À¿—ÕŒ œŒ–”◊≈Õ»≈ Õ¿ œ–Œƒ¿∆”", true, null))
                        {
                            var operationDate = DateTime.Parse(statementDate, cultureInfo);
                            var rateInfo = GetSellOperationInfo(document.GROUND);

                            //check
                            SellMessage sellMessage = new SellMessage()
                                                          {
                                                              Date = operationDate,
                                                              //Amount = decimal.Parse(document.AMOUNT, cultureInfo),
                                                              Amount = rateInfo.Amount,
                                                              Rate = rateInfo.Rate,
                                                              SellType = SellType.Mandatory
                                                          };
                            if (rateInfo.AmountNat/rateInfo.Rate != sellMessage.Amount ||
                                rateInfo.Amount != sellMessage.Amount)
                                throw new Exception(String.Format("RateInfo is incorrect.: {0}", document.GROUND));

                            _filter.AddSellMessage(messages, sellMessage);
                        }

                        //free sale
                        // black scheme
                        if (account == 3013117180019 && statement.CURRCODE == 840 && 
                            (document.GROUND.Contains("BYR")))
                        {
                            var operationDate = DateTime.Parse(statementDate, cultureInfo);
                            var rateInfo = GetSellOperationInfo(document.GROUND);

                            //check

                            SellMessage sellMessage = new SellMessage()
                                                          {
                                                              Date = operationDate,
                                                              Amount = rateInfo.Amount,
                                                              Rate = rateInfo.Rate,
                                                              SellType = SellType.Free
                                                          };
                            if (rateInfo.AmountNat/rateInfo.Rate != sellMessage.Amount ||
                                rateInfo.Amount != sellMessage.Amount)
                                throw new Exception(String.Format("RateInfo is incorrect.: {0}", document.GROUND));

                            _filter.AddSellMessage(messages, sellMessage);
                        }

                        if (document.GROUND.StartsWith("Õ‡ÎÓ„ ÔÓ ÛÔÓ˘∏ÌÌÓÈ ÒËÒÚÂÏÂ Ì‡ÎÓ„ÓÓ·ÎÓÊÂÌËˇ", StringComparison.InvariantCultureIgnoreCase))
                        {
                            TaxInfo taxInfo = GetTaxInfo(document.GROUND);
                            messages.Add(new TaxPaymentMessage()
                            {
                                Amount = decimal.Parse(document.AMOUNT, cultureInfo),
                                QuarterType = taxInfo.QuarterType,
                                YearNumber = taxInfo.YearNumber
                            });
                        }
                    }
                    //Console.WriteLine();
                }
            }

            return messages;
        }
      
        private TaxInfo GetTaxInfo(string description)
        {
            Regex regex = new Regex(TaxMessagePattern);
            Match match = regex.Match(description);
            var quarter = match.Groups["Quarter".ToUpper()].Value;
            var rate = Int32.Parse(match.Groups["Year".ToUpper()].Value);

            return new TaxInfo()
            {
                YearNumber = rate,
                QuarterType = QuarterResolver.GetQuarterNumber(quarter)
            };
        }


        public static RateInfo GetSellOperationInfo(string ground)
        {
            if(ground == "—Œ√À¿—ÕŒ œŒ–”◊≈Õ»≈ Õ¿ œ–Œƒ¿∆” N 32 Œ“ 20120604.  À»≈Õ“ œﬂ“À»Õ ¿À≈ —≈… œ¿¬ÀŒ¬»◊ »Õƒ»¬»ƒ”¿À‹Õ€… œ–≈ƒœ–»Õ»Ã¿“≈À‹ Ã»Õ—  –≈—œ”¡À» ¿ ¡≈À¿–”—‹. USD 537;3/BYR 4507947  ”–— 8390 ¡≈« Õƒ— ”ƒ≈–∆¿Õ¿  ŒÃ»——»ﬂ ¬ –¿«Ã≈–≈ 9016")
                return new RateInfo()
                                    {
                                        Rate = Decimal.Parse("8390", Settings.EngCultureInfo),
                                        AmountNat =
                                            Decimal.Parse("4507947", Settings.EngCultureInfo),
                                        Amount = Decimal.Parse("537.3", Settings.EngCultureInfo),
                                    };


            Regex regex = new Regex(RateInfoPattertn);
            Match match = regex.Match(ground);
            RateInfo rateInfo = new RateInfo()
                                    {
                                        Rate = Decimal.Parse(match.Groups["Rate"].Value, Settings.EngCultureInfo),
                                        AmountNat =
                                            Decimal.Parse(match.Groups["AmountNat"].Value, Settings.EngCultureInfo),
                                        Amount = Decimal.Parse(match.Groups["Amount"].Value, Settings.EngCultureInfo),
                                    };

            return rateInfo;
        }

        private RateInfo GetFreeSellOperationInfo(string ground)
        {
            Regex regex = new Regex(FreeRateInfoPattern);
            Match match = regex.Match(ground);
            RateInfo rateInfo = new RateInfo()
                                    {
                                        Rate = Decimal.Parse(match.Groups["Rate"].Value, Settings.EngCultureInfo),
                                        Amount = Decimal.Parse(match.Groups["Amount"].Value, Settings.EngCultureInfo),
                                    };

            return rateInfo;
        }


        private static object GetDocumentCount(XElement statement, string parentDocumentsElName,
                                               string documentElementName)
        {
            int count = 0;
            var parent = statement.Elements(parentDocumentsElName);
            count = parent.Elements(documentElementName).Count();
            return count;
        }
    }
}