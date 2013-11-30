using System;

namespace FeesCalculator.BussinnesLogic.Reports
{
    public class Column<T>
    {
        public int Size { get; set; }

        public string Name { get; set; }

        public Func<T, string> CellValue { get; set; }
    }
}