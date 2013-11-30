using System.Globalization;
using System.Text;

namespace FeesCalculator.BussinnesLogic
{
    public static class Settings
    {
        public static readonly CultureInfo EngCultureInfo = new CultureInfo("en-US");
        public static readonly CultureInfo RuCultureInfo = new CultureInfo("ru-Ru");

        public static Encoding FileEncoding = Encoding.GetEncoding(1251);
    }
}