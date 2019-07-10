using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
    internal class IntegerRangeValidator : Validator
    {
        private int lowerBound;
        private int upperBound;

        public IntegerRangeValidator(int lowerBound, int upperBound)
            : base(string.Empty, string.Empty)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public IntegerRangeValidator(int lowerBound, int upperBound, string messageTemplate, string tag)
            : base(messageTemplate, tag)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        protected internal override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            bool isValid = false;

            if (objectToValidate != null)
            {
                RangeChecker<int> checker = new RangeChecker<int>(this.lowerBound, this.upperBound);
                isValid = checker.IsInRange((int)objectToValidate);
            }

            if (isValid == false)
                RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
        }
    }
}
