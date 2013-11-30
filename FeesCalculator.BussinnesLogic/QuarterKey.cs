using System.Collections.Generic;
using FeesCalculator.Tests;

namespace FeesCalculator.BussinnesLogic
{
    public class QuarterKey //: IComparer<QuarterKey>

    {
        public QuarterType Type { get; set; }

        public int YearNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (QuarterKey)) return false;
            return Equals((QuarterKey) obj);
        }

        public bool Equals(QuarterKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && other.YearNumber == YearNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ YearNumber;
            }
        }
    }
}