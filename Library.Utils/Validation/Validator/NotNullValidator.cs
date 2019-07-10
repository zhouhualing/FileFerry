using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
    internal class NotNullValidator : Validator
    {
        public NotNullValidator() : base(string.Empty)
        { 
        }

        public NotNullValidator(string messageTemplate)
            : base(messageTemplate)
        {
        }

        protected internal override  void DoValidate(object objectToValidate,
                                                    object currentTarget,
                                                    string key,
                                                    ValidationResults validationResults)
        {
            if (objectToValidate == null)
            {
                RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
            }
        }

    }
}
