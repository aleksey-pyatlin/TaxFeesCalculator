using System;

namespace FeesCalculator.BussinnesLogic
{
    public interface IRateManager
    {
        decimal GetNationalRate(DateTime rateDate);
        void ImportRates(string importFilePath);
    }
}