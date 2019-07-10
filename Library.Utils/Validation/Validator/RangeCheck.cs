using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
    internal class RangeChecker<T> where T : IComparable
    {
        private T lowerBound;
        private T upperBound;

        public RangeChecker(T lowerBound, T upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public bool IsInRange(T target)
        {
            return target.CompareTo(this.lowerBound) >= 0 && target.CompareTo(this.upperBound) <= 0;
        }
    }
}
