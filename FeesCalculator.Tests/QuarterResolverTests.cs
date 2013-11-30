using System;
using FeesCalculator.BussinnesLogic;
using FeesCalculator.Data;
using NUnit.Framework;

namespace FeesCalculator.Tests
{
    [TestFixture]
    public class QuarterResolverTests
    {
        [TestCase("2/1/2011", Result = QuarterType.One)]
        [TestCase("4/1/2011", Result = QuarterType.Two)]
        [TestCase("7/1/2011", Result = QuarterType.Three)]
        [TestCase("11/11/2011", Result = QuarterType.Four)]
        public QuarterType GetQuarterNumberTest(String dateTimeString)
        {
            DateTime dateTime = DateTime.Parse(dateTimeString);
            return QuarterResolver.GetQuarterNumber(dateTime).Type;
        }

        [TestCase(QuarterType.One, Result = 3)]
        [TestCase(QuarterType.Two, Result = 6)]
        [TestCase(QuarterType.Three, Result = 9)]
        [TestCase(QuarterType.Four, Result = 12)]
        public int GetQuarterNumberTest(QuarterType quarterType)
        {
            return QuarterResolver.GetLasttMonth(quarterType);
        }
    }
}