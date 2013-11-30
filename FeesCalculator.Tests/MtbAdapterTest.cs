using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeesCalculator.ConsoleApplication.Adapters;
using NUnit.Framework;

namespace FeesCalculator.Tests
{
    [TestFixture]
    class MtbAdapterTest
    {
        [Test]
        public void Test()
        {
            const string ground = "СОГЛАСНО ПОРУЧЕНИЕ НА ПРОДАЖУ N 18 ОТ 20120417. КЛИЕНТ ПЯТЛИН АЛЕКСЕЙ ПАВЛОВИЧ ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ МИНСК РЕСПУБЛИКА БЕЛАРУСЬ. USD 537.3/BYR 4303773 КУРС 8010 БЕЗ НДС УДЕРЖАНА КОМИССИЯ В РАЗМЕРЕ 8608";
            var result = MtbAdapter.GetSellOperationInfo(ground);
            Assert.AreEqual(537.3, result.Amount);
            Assert.AreEqual(8010, result.Rate);
        } 
        
        [Test]
        public void Test2()
        {
            const string ground = "СОГЛАСНО П/п с продажей N 2 ОТ 20110803. КЛИЕНТ ПЯТЛИН АЛЕКСЕЙ ПАВЛОВИЧ ИНДИВИДУАЛЬНЫЙ ПРЕДПРИНИМАТЕЛЬ МИНСК РЕСПУБЛИКА БЕЛАРУСЬ. USD 3819/BYR 18988068 КУРС 4972 БЕЗ НДС";
            var result = MtbAdapter.GetSellOperationInfo(ground);
            Assert.AreEqual(3819,  result.Amount);
            Assert.AreEqual(4972, result.Rate);
        }

    }
}
