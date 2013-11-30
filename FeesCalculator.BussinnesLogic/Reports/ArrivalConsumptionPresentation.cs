using System;
using System.Collections.Generic;
using FeesCalculator.Data;
using FeesCalculator.Tests;

namespace FeesCalculator.BussinnesLogic.Reports
{
    public class ArrivalConsumptionPresentation
    {
        private const ConsoleColor Color = ConsoleColor.Gray;
        
        public void Render(Dictionary<QuarterKey, Quarter> quarters)
        {
            #region Debit columns

            List<Column<Payment>> columns = new List<Column<Payment>>()
                                       {
                                           new DebitColumn()
                                               {
                                                   Name = "Date",
                                                   Size = 10,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDateTime(x.Date);
                                                                   }
                                               },
                                               
                                               new DebitColumn()
                                               {
                                                   Name = "Incomm",
                                                   Size = 7,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.Amount);
                                                                   }
                                               },
                                               
                                               new DebitColumn()
                                               {
                                                   Name = "Nat.Rate",
                                                   Size = 8,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.Rate);
                                                                   }
                                               },
                                               
                                               new DebitColumn()
                                               {
                                                   Name = "Summ",
                                                   Size = 12,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.NationalAmount);
                                                                   }
                                               }, 
                                               
                                               new DebitColumn()
                                               {
                                                   Name = "Oper.",
                                                   Size = 6,
                                                   CellValue = x =>
                                                                   {
                                                                       return x.OperationType.HasValue ? x.OperationType.ToString() : String.Empty;
                                                                   }
                                               },

                                              

                                               // | Date | Amout sell | Rate | Summ | Delta
                                       };

            List<Column<OutOfProfitPayment>> columnsOut = new List<Column<OutOfProfitPayment>>()
                                       {
                                           new Column<OutOfProfitPayment>()
                                               {
                                                   Name = "Date",
                                                   Size = 10,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDateTime(x.Date);
                                                                   }
                                               },
                                               
                                               new Column<OutOfProfitPayment>()
                                               {
                                                   Name = "Amount",
                                                   Size = 15,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.Amount);
                                                                   }
                                               },
                                               
                                               new Column<OutOfProfitPayment>()
                                               {
                                                   Name = "Ground",
                                                   Size = 50,
                                                   CellValue = x =>
                                                                   { 
                                                                       return x.Ground.Substring(0,48) + "...";
                                                                   }
                                               }
                                       };

            #endregion

            #region Credit columns

            List<Column<SellPayment>> creditColumns = new List<Column<SellPayment>>()
                                       {
                                           new CreditColumn()
                                               {
                                                   Name = "Sell Date",
                                                   Size = 10,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDateTime(x.Date);
                                                                   }


                                               },

                                               new CreditColumn()
                                               {
                                                   Name = "Sell $",
                                                   Size = 7,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.Amount);
                                                                   }


                                               },
                                               new CreditColumn()
                                               {
                                                   Name = "Rate $",
                                                   Size = 7,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.Rate);
                                                                   }


                                               },
                                               
                                               new CreditColumn()
                                               {
                                                   Name = "Sell Summ",
                                                   Size = 12,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.NationalAmount);
                                                                   }


                                               },
                                               
                                               new CreditColumn()
                                               {
                                                   Name = "Rate Delta",
                                                   Size = 10,
                                                   CellValue = x =>
                                                                   { 
                                                                       return DataFormatter.ToDecimal(x.RateDelta);
                                                                   }


                                               },
                                       };
            #endregion

            foreach (var quarter in quarters)
            {
                Console.WriteLine("*************************************************");
                Console.WriteLine("Year: {0} Quarter: {1}", quarter.Value.Year.Number, quarter.Value.Type);
                PrintGridHeader(columns, creditColumns);
                foreach (var debit in quarter.Value.Debits)
                {
                    PrintRow(columns, creditColumns, debit);
                }

                Console.WriteLine("\r\nOut of profit:\r\n".ToUpper());
                columnsOut.ForEach(x => Print(x.Size, x.Name));
                Console.WriteLine();
                
                foreach (var outOfProfitPayment in quarter.Value.OutOfProfits)
                {
                    PrintProfitRow(columnsOut, outOfProfitPayment);
                }

                PrintTotal(columns, creditColumns, quarter);
                Console.WriteLine();
            }

            //return;

            decimal paidTaxAmount = 0;
            decimal calcFees = 0;
            foreach (var quarter in quarters)
            {
                if (quarter.Value.Type == QuarterType.Four && quarter.Value.Year.Number == 2011)
                {
                    continue;
                }
                paidTaxAmount += quarter.Value.PaidTaxAmount;
                calcFees += quarter.Value.CalcFees;
            }

            Console.Write("WITHOUT LAST QUARTER \r\n\tCalc: {0} \r\n\tPaid: {1} \r\n\t".ToUpper(), DataFormatter.ToDecimal(calcFees), DataFormatter.ToDecimal(paidTaxAmount));
            var delta = paidTaxAmount - calcFees;
            
            if(delta < 0)
                Console.WriteLine("Need pay: {0}", DataFormatter.ToDecimal(delta));
            else
            {
                Console.WriteLine("Over paid: {0}", DataFormatter.ToDecimal(delta));
            }
            Quarter quarter2 = quarters[new QuarterKey(){Type = QuarterType.Four,YearNumber = 2011}];
            paidTaxAmount += quarter2.PaidTaxAmount;
            calcFees += quarter2.CalcFees;

            Console.WriteLine("FINAL \r\n\tCalc: {0} \r\n\tPaid: {1} \r\n\tNeed pay: {2}".ToUpper(), DataFormatter.ToDecimal(calcFees), DataFormatter.ToDecimal(paidTaxAmount), DataFormatter.ToDecimal(paidTaxAmount - calcFees));

            Console.WriteLine("\r\nBY YEARS:");
            PrintTaxInfo(quarters, 2009);
            PrintTaxInfo(quarters, 2010);
            PrintTaxInfo(quarters, 2011);
            PrintTaxInfo(quarters, 2012);
            PrintTaxInfo(quarters, 2013);


            decimal totalProfit = 0;
            foreach (var quarter in quarters)
            {
                if (quarter.Value.Year.Number == 2011)
                {
                    totalProfit += quarter.Value.Summary;
                }
            }

            Console.WriteLine("\r\nTOTAL PROFIT:{0}", totalProfit);

            
            Dictionary<QuarterType, decimal> taxes = new Dictionary<QuarterType, decimal>();
            taxes.Add(QuarterType.One, 15530420);
            taxes.Add(QuarterType.Two, 16867630);
            taxes.Add(QuarterType.Three, 49448637);
            taxes.Add(QuarterType.Four, 0);

            foreach (var quarter in quarters)
            {
                if (quarter.Value.Year.Number == 2011)
                {
                    decimal current = taxes[quarter.Value.Type];
                    Console.WriteLine("{0}: {1}", quarter.Value.Type, Math.Round(quarter.Value.Summary - current));
                }
            }
        }

        private void PrintTaxInfo(Dictionary<QuarterKey, Quarter> quarters, int year)
        {
            decimal paidTaxAmount = 0;
            decimal calcFees = 0;
            foreach (var quarter in quarters)
            {
                if (quarter.Value.Year.Number != year)
                    continue;
                paidTaxAmount += quarter.Value.PaidTaxAmount;
                calcFees += quarter.Value.CalcFees;
            }

            Console.Write("YEAR {2} \r\n\tCalc: {0} \r\n\tPaid: {1} \r\n\t".ToUpper(), DataFormatter.ToDecimal(calcFees), DataFormatter.ToDecimal(paidTaxAmount), year);
            var delta = paidTaxAmount - calcFees;

            if (delta < 0)
                Console.WriteLine("Need pay: {0}", DataFormatter.ToDecimal(delta));
            else
            {
                Console.WriteLine("Over paid: {0}", DataFormatter.ToDecimal(delta));
            }
        }

        private void PrintTotal(List<Column<Payment>> columns, List<Column<SellPayment>> creditColumns, KeyValuePair<QuarterKey, Quarter> quarter)
        {
            Console.WriteLine("Totals:");
            //TODO: Use other way of binding
            List<Column<Payment>> ncolumns  = new List<Column<Payment>>(columns.ToArray());
            var old = ncolumns[3].CellValue;
            ncolumns[3].CellValue = payment => DataFormatter.ToDecimal(quarter.Value.Incomming);
            PrintCells(columns, new Payment(), false);
            PrintCells<SellPayment>(creditColumns, new SellPayment() { RateDelta = quarter.Value.OutOfProfit }, false);
            ncolumns[3].CellValue = old;

            var feesPercent = FeesProvider.GetFeesPercent(quarter);
            var deltaRateFeesPercent = FeesProvider.GetDeltaRateFeesPercent(quarter);
            var incommingFees = Math.Round(feesPercent*quarter.Value.Incomming);
            var deltaRateFees = Math.Round(deltaRateFeesPercent*quarter.Value.OutOfProfit);
            
            Console.Write("\r\n\r\n======> Final Totals: {0} ====> Incomm Fees ({1} %): {2} Delta Rate Fees: ({3} %): {4}", 
                DataFormatter.ToDecimal(quarter.Value.Summary), 
                feesPercent * 100, 
                incommingFees,
                deltaRateFeesPercent * 100, 
                deltaRateFees);
            
            var calcFees = incommingFees + deltaRateFees;
            Console.Write("\r\nTotal Fees: : ");
            PrintColor(ConsoleColor.Cyan, false, "{0}", calcFees);
            Console.Write(" Paid: ");
            PrintColor(ConsoleColor.Yellow, false, "{0}", quarter.Value.PaidTaxAmount);
            Console.Write(" Delta: ");

            var delta = quarter.Value.PaidTaxAmount - calcFees;
            if(delta > 0)
                PrintColor(ConsoleColor.Green, true, "{0}", delta);
            else
            {
                PrintColor(ConsoleColor.Red, true, "{0}", delta);
            }

            quarter.Value.CalcFees = calcFees;
        }

        private void PrintColor(ConsoleColor color, bool newLine, string message, params object[] parameters)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message + (newLine ? Environment.NewLine : String.Empty),parameters);
            Console.ForegroundColor = old;
        }

        private void PrintRow(List<Column<Payment>> columns, List<Column<SellPayment>> creditColumns, Payment debit)
        {
            if(debit.Amount > 0)
            {
                PrintCells(columns, debit, false, debit.OperationType == PaymentType.Incomm ? ConsoleColor.Cyan : Color);
                PrintCells<SellPayment>(creditColumns, new SellPayment() { RateDelta = debit.RateDelta }, false);
                foreach (var credit in debit.Credits)
                {
                    Console.WriteLine();
                    PrintCells(columns, debit, true);
                    PrintCells<SellPayment>(creditColumns, credit, false);
                }
                Console.WriteLine();
            }
        }

        private void PrintProfitRow(List<Column<OutOfProfitPayment>> columns, OutOfProfitPayment outOfProfitPayment)
        {
            if (outOfProfitPayment.Amount > 0)
            {
                PrintCells(columns, outOfProfitPayment, false, ConsoleColor.Cyan);
                Console.WriteLine();
            }
        }

        private void PrintCells<TColumn>(List<Column<TColumn>> columns, TColumn payment, bool skip, ConsoleColor color = Color)
            
        {
            String cellValue;
            foreach (var column in columns)
            {
                cellValue = String.Empty;
                if (!skip && column.CellValue != null)
                {
                    cellValue = column.CellValue(payment);
                }

                Print(column.Size, cellValue, color);
            }

        }

        private void PrintGridHeader(List<Column<Payment>> columns, List<Column<SellPayment>> creditColumns)
        {
            columns.ForEach(x=> Print(x.Size, x.Name) );
            creditColumns.ForEach(x=> Print(x.Size, x.Name) );
            Console.WriteLine();
        }

        private void Print(int size, string name, ConsoleColor color = Color)
        {
            PrintColor(color, false,"| {0} ", name.PadRight(size, ' '));
        }

    }
}