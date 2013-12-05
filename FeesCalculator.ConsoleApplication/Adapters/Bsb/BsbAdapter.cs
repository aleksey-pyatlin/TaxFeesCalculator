using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.BussinnesLogic.Messages;
using FeesCalculator.Data;

namespace FeesCalculator.ConsoleApplication.Adapters.Bsb
{
    public class BsbAdapter : IAdapter
    {
        private List<String> _customers;

        private const String SellMessagePattern = @"(?<DocumentId>\d{1,})\sот\s(?<DocumentDate>\d{2,}.\d{2,}.\d{2,})г.Удержана комиссия\s(?<Commission>\d{1,})\sруб. Курс продажи\s(?<Rate>\d{1,})";
        private const String TaxMessagePattern = @"(?<Quarter>\w{1,})\s{1,}квартал (?<Year>\d{1,})г";

        private readonly string[] _bsbIncommingPaymentsPath;
        private readonly string[] _mandatorySellPaths;
        private readonly string[] _freeSellPaths;
        private readonly IRateManager _rateManager;
        private readonly Filter _filter;

        public BsbAdapter(string[] bsbIncommingPaymentsPath, string[] mandatorySellPaths, string[] freeSellPaths, IRateManager rateManager)
        {
            _bsbIncommingPaymentsPath = bsbIncommingPaymentsPath;
            //7003 BYR
            _mandatorySellPaths = mandatorySellPaths;
            _freeSellPaths = freeSellPaths;
            _rateManager = rateManager;
            _filter = new Filter();

            InitCustomers();
        }

        private void InitCustomers()
        {
            _customers = new List<string>(new string[]
                                             {
                                                 "FL 327463319",
                                                 "SUITE 360FLOWER MOUND TX 75022",
                                             });
        }

        public List<OperationMessage> GetMessages(String dataDirectoryFull, List<string> files)
        {
            List<OperationMessage> messages = GetIncommingMessages();
            messages.AddRange(GetTaxgMessages());
            messages.AddRange(GetFreeSellsgMessages());
            messages.AddRange(GetMandatorySellsgMessages());

            return messages;
        }

        public List<OperationMessage> GetFreeSellsgMessages()
        {
            List<OperationMessage> messages = new List<OperationMessage>();
            int i = 0;
            foreach (var paymentsPath in _freeSellPaths)
            {
                BsbSummary natAccountOperations = GetBsbOperations(File.ReadAllLines(_mandatorySellPaths[i], Settings.FileEncoding));
                i++;

                List<OperationMessage> yearMessages = new List<OperationMessage>();
                String[] lines = File.ReadAllLines(paymentsPath, Settings.FileEncoding);
                BsbSummary bsbSummary = GetBsbOperations(lines);
                foreach (var bsbOperation in bsbSummary.Operations)
                {
                    if (bsbOperation.Destination.StartsWith("Продажа ин", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var docId = GetAmount(bsbOperation.Content, 1);
                        var natReletedOperations = from o in natAccountOperations.Operations
                                                   where o.Date == bsbOperation.Date
                                                   select o;
                        var rateInfoOperation = (from natDayOperattion in natReletedOperations
                                                 where GetAmount(natDayOperattion.Content, 1) == docId && !natDayOperattion.IsUsed
                                                select natDayOperattion).ToList();

                        decimal rate = 0;
                        decimal amount = 0;
                        decimal usdAmount = GetAmount(bsbOperation.Content, 5);

                        if (rateInfoOperation.Count != 1)
                        {
                            if (usdAmount == (decimal)3.5 && bsbOperation.Date == DateTime.Parse("3/19/2010", Settings.EngCultureInfo)) 
                            {
                                rateInfoOperation[0].IsUsed = true;
                                amount = GetAmount(rateInfoOperation[0].Content); ;
                            }
                                
                            else
                            {
                                throw new Exception("Nat amount collision.");
                            }
                        }
                        else
                        {
                            amount = GetAmount(rateInfoOperation[0].Content);
                        }
                        rate = Math.Round(amount / usdAmount);
                        if(rate == null)
                            throw new Exception("Rate is indefined."); 

                        _filter.AddSellMessage(yearMessages, new SellMessage()
                        {
                            Date = bsbOperation.Date,
                            Amount = usdAmount,
                            Rate = rate,
                            //Commission = rateInfo.Commission,
                            SellType = SellType.Free
                        });
                    }
                }

                if (!CheckUSDAmounts(bsbSummary, yearMessages))
                {
                    new Exception("Amount are incorrects.");
                }

                messages.AddRange(yearMessages);
            }

            return messages;
        }

        public List<OperationMessage> GetTaxgMessages()
        {
            List<OperationMessage> messages = new List<OperationMessage>();
            foreach (var paymentsPath in _mandatorySellPaths)
            {
                List<OperationMessage> yearMessages = new List<OperationMessage>();
                String[] lines = File.ReadAllLines(paymentsPath, Settings.FileEncoding);
                BsbSummary bsbSummary = GetBsbOperations(lines);
                foreach (var bsbOperation in bsbSummary.Operations)
                {
                    //RateInfo rateInfo = GetRateInfo(bsbOperation);
                    if (bsbOperation.Description.Contains("проце") || bsbOperation.Description.Contains("ЗА КОНСУЛЬТИРОВАНИЕ И РАЗРАБОТКУ САЙТА"))
                    {
                        yearMessages.Add(new OutOfProfitMessage()
                        {
                            Date = bsbOperation.Date,
                            AmountNat = GetAmount(bsbOperation.Content),
                            Ground = bsbOperation.Description
//                            Amount = rateInfo.Amount,
//                            Rate = rateInfo.Rate,
//                            Commission = rateInfo.Commission,
                            //SellType = bsbOperation.Description.Contains("за обязательную продажу") ? SellType.Mandatory :  SellType.Free
                        });
                    }
                    else
                    {
                        if (bsbOperation.Description.StartsWith("Налог по упрощённой системе налогообложения", StringComparison.InvariantCultureIgnoreCase))
                        {
                            TaxInfo taxInfo = GetTaxInfo(bsbOperation.Description);
                            messages.Add(new TaxSellMessage()
                                         {
                                             Amount = GetAmount(bsbOperation.Content, 5),
                                             QuarterType = taxInfo.QuarterType,
                                             YearNumber = taxInfo.YearNumber
                                         });
                        }
                    }

                }

                if (!CheckAmounts(bsbSummary, yearMessages))
                {
                   // new Exception("Amount are incorrects.");
                }

                messages.AddRange(yearMessages);
            }

            return messages;
        }

        public List<OperationMessage> GetMandatorySellsgMessages()
        {
            List<OperationMessage> messages = new List<OperationMessage>();
            foreach (var paymentsPath in _bsbIncommingPaymentsPath)
            {
                List<OperationMessage> yearMessages = new List<OperationMessage>();
                String[] lines = File.ReadAllLines(paymentsPath, Settings.FileEncoding);
                BsbSummary bsbSummary = GetBsbOperations(lines);
                foreach (var bsbOperation in bsbSummary.Operations)
                {
                    //RateInfo rateInfo = GetRateInfo(bsbOperation);
                    if (bsbOperation.Content.Contains("6;USD ;6901800078404"))
                    {
                        _filter.AddSellMessage(yearMessages, new SellMessage()
                        {
                            Date = bsbOperation.Date,
                            Amount = GetAmount(bsbOperation.Content, 5),
                            Rate = _rateManager.GetNationalRate(bsbOperation.Date),
                            //Commission = rateInfo.Commission,
                            SellType = SellType.Mandatory
                        });
                    }
                    

                }

                if (!CheckAmounts(bsbSummary, yearMessages))
                {
                    // new Exception("Amount are incorrects.");
                }

                messages.AddRange(yearMessages);
            }

            return messages;
        }



        private TaxInfo GetTaxInfo(string description)
        {
            Regex regex = new Regex(TaxMessagePattern);
            Match match = regex.Match(description);
            var quarter = match.Groups["Quarter"].Value;
            var rate = Int32.Parse(match.Groups["Year"].Value);

            return new TaxInfo()
                       {
                           YearNumber = rate,
                           QuarterType = QuarterResolver.GetQuarterNumber(quarter)                            
                       };
        }

        private RateInfo GetRateInfo(BsbOperation bsbOperation)
        {
            decimal amountNat = GetAmount(bsbOperation.Content);
            Regex regex = new Regex(SellMessagePattern);
            Match match = regex.Match(bsbOperation.Description);

            try 
            {
                var commission = Decimal.Parse(match.Groups["Commission"].Value, Settings.EngCultureInfo);
                var rate = Decimal.Parse(match.Groups["Rate"].Value, Settings.EngCultureInfo);


                RateInfo rateInfo = new RateInfo()
                {
                    Rate = rate,
                    AmountNat = (amountNat + commission),
                    Amount = Math.Round((amountNat + commission) / rate, 2),
                    Commission = commission
                };

//                if (rateInfo.AmountNat != (rateInfo.Rate * rateInfo.Amount))
//                    throw new Exception();

                return rateInfo;
            }
            catch
            {

            }

            return null;

        }

        public List<OperationMessage> GetIncommingMessages()
        {
            List<OperationMessage> messages = new List<OperationMessage>();
            foreach (var paymentsPath in _bsbIncommingPaymentsPath)
            {
                List<OperationMessage> yearMessages = new List<OperationMessage>();
            
                String[] lines = File.ReadAllLines(paymentsPath, Settings.FileEncoding);
                BsbSummary bsbSummary = GetBsbOperations(lines);
                foreach (var bsbOperation in bsbSummary.Operations)
                {
                    if (IsIncommingFromCustomer(bsbOperation.Destination))
                    {
                        if (bsbOperation.Date.Year == 2010 && bsbOperation.Date.Month == 7 && bsbOperation.Date.Day == 1)
                        {
                            // Workaround to simulate declaration calculation.
                            // File name: УСН_2010.xls
                            // 12	Выписка банка за 01.07.2010	Оплата по акту от 30.06.2010 $2200 по курсу 3023	 6,650,600 			 6,650,600 
                            bsbOperation.Date = new DateTime(2010, 6, 30);
                        }
                        if (bsbOperation.Date.Year == 2010 && bsbOperation.Date.Month == 10 && bsbOperation.Date.Day == 4)
                        {
                            // Workaround to simulate declaration calculation.
                            // File name: 3_Квартал_УСН_2010.xls
                            // 6	Выписка банка за 04.10.2010	Оплата по акту от 30.09.2010 $2200 по курсу 3009	 6,619,800 			 6,619,800 
                            bsbOperation.Date = new DateTime(2010, 9, 30);
                        }

                        _filter.AddIncommingMessage(yearMessages, new IncommingPaymentMessage()
                                             {
                                                 Date = bsbOperation.Date,
                                                 Amount = GetAmount(bsbOperation.Content),
                                                 Rate = _rateManager.GetNationalRate(bsbOperation.Date),
                                                 
                                             }, bsbOperation.Date.ToString());
                    }
                }

                if (!CheckAmounts(bsbSummary, yearMessages))
                {
                    new Exception("Amount are incorrects.");
                }

                messages.AddRange(yearMessages);
            }

            return messages;
        }


        private bool CheckUSDAmounts(BsbSummary bsbSummary, List<OperationMessage> yearMessages)
        {
            decimal summary = GetAmount(bsbSummary.Content);
            decimal calSummary = 0;
            yearMessages.ForEach(x =>
            {
                SellMessage sellMessage = x as SellMessage;
                calSummary += x.Amount;
            });
            return summary == calSummary;
        }

        private bool CheckAmounts(BsbSummary bsbSummary, List<OperationMessage> yearMessages)
        {
            decimal summary = GetAmount(bsbSummary.Content);
            decimal calSummary = 0;
            yearMessages.ForEach(x =>
                                     {
                                         SellMessage sellMessage = x as SellMessage;
                                         calSummary += (x.Amount*x.Rate);
//                                         if (sellMessage != null)
//                                             calSummary -= sellMessage.Commission;
                                     });
            return summary == calSummary;
       }


        private decimal GetAmount(string content, int? index = null)
        {
            var values = content.Split(';');
            int? valIndex = index;
            if (!valIndex.HasValue)
                valIndex = values.Count() - 1;
            
            var value = values[valIndex.Value].Replace(" ", "").Replace(",", ".");
            return Decimal.Parse(value, Settings.EngCultureInfo);
        }

        private bool IsIncommingFromCustomer(string content)
        {
            return _customers.Any(content.Contains);
        }


        private BsbSummary GetBsbOperations(string[] lines)
        {
            BsbSummary bsbSummary = new BsbSummary();
            List<BsbOperation> bsbOperations = new List<BsbOperation>();
            int lineNumber = 0;
            foreach (var line in lines)
            {
                var values = line.Split(';');
                DateTime operationDate;
                if (DateTime.TryParse(values[0], Settings.RuCultureInfo, DateTimeStyles.None, out operationDate))
                {
                    bsbOperations.Add(new BsbOperation()
                                          {
                                              Date = operationDate,
                                              Content = lines[lineNumber],
                                              Destination = lines[lineNumber + 1],
                                              Description = lines[lineNumber + 2]
                                          });
                }

                if (line.StartsWith("Обороты"))
                    bsbSummary = new BsbSummary()
                                     {
                                         Content = line
                                     };

                lineNumber++;
            }

            bsbSummary.Operations = bsbOperations;
            return bsbSummary;
        }
        
    }
}