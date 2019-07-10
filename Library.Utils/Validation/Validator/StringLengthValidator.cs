using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
    internal class StringLengthValidator : Validator  
    {
        private int lowerBound;
        private int upperBound;

        public StringLengthValidator(int upperBound) 
            : this(0, upperBound)
        { 
        }

        public StringLengthValidator(int lowerBound, int upperBound)
            : this(lowerBound, upperBound, string.Empty, string.Empty)
        {
        }

        public StringLengthValidator(int lowerBound, int upperBound, string messageTemplate, string tag)
            : base(messageTemplate, tag)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        protected override internal void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            bool isValid = false;

			if (objectToValidate != null)
			{
				RangeChecker<int> checker = new RangeChecker<int>(this.lowerBound, this.upperBound);
				isValid = checker.IsInRange(objectToValidate.ToString().Length);
			}
			else
				isValid = (this.lowerBound <= 0);

            if (isValid == false)
                RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
        }
    }
}
