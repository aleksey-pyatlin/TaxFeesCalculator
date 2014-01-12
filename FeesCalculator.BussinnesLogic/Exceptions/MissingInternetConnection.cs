using System;

namespace FeesCalculator.BussinnesLogic.Exceptions
{
    public class MissingInternetConnectionException : Exception
    {
        public MissingInternetConnectionException()
            : base("Connection is not available. Please verify it.")
        {
        }
    }
}