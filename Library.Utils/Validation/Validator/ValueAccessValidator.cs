using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Validation
{
    internal class ValueAccessValidator : Validator
    {
        private ValueAccess valueAccess;
        private Validator innerValidator;

        public ValueAccessValidator(ValueAccess valueAccess, Validator innerValidator) : base(string.Empty, string.Empty)
        {
            this.valueAccess = valueAccess;
            this.innerValidator = innerValidator;
        }

        protected internal override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            if (objectToValidate != null)
                this.innerValidator.DoValidate(this.valueAccess.GetValue(objectToValidate), currentTarget, key, validationResults);
        }
    }
}
