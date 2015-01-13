namespace FeesCalculator.Tests.Adapters.Mtb
{
    using FeesCalculator.ConsoleApplication.Adapters;

    using NUnit.Framework;

    [TestFixture]
    public class MtbAdapterTests
    {
        [TestCase("СОГЛАСНО ПОРУЧЕНИЕ НА ПРОДАЖУ N 32 ОТ 20120604. КЛИЕНТ ПЯТЛИН АЛЕКСЕЙ ПАВЛОВИЧ ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ МИНСК РЕСПУБЛИКА БЕЛАРУСЬ. USD 537;3/BYR 4507947 КУРС 8390 БЕЗ НДС УДЕРЖАНА КОМИССИЯ В РАЗМЕРЕ 9016")]
        public static void GetSellOperationInfo(string ground)
        {
            var rateInfo = MtbAdapter.GetSellOperationInfo(ground);
            Assert.IsNotNull(rateInfo);

        }
    }
}