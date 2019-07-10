using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WD.Library.Validation
{
    internal class RegexValidator : Validator
    {
        private string pattern;
        private RegexOptions options;
     
      
        public RegexValidator(string pattern)
            : this(pattern, RegexOptions.None)
        { }

     
        public RegexValidator(string pattern, RegexOptions options)
            : this(pattern, options, string.Empty)
        { }

        public RegexValidator(string pattern, string messageTemplate)
            : this(pattern, RegexOptions.None, messageTemplate)
        { }

      
        public RegexValidator(string pattern, RegexOptions options, string messageTemplate)
           : this(pattern, options, messageTemplate, string.Empty)
        { }

       
        public RegexValidator(string pattern, RegexOptions options, string messageTemplate, string tag)
            : base(messageTemplate, tag)
        {
            this.pattern = pattern;
            this.options = options;

        }

        protected internal override void DoValidate(object objectToValidate,
                                                   object currentTarget,
                                                   string key,
                                                   ValidationResults validationResults)
        {
            bool matchFlag = false;


            if (objectToValidate != null && !string.IsNullOrEmpty(this.pattern))
            {
                Regex regex = new Regex(this.pattern, this.options);
                matchFlag = regex.IsMatch(objectToValidate.ToString());
            }

            if (objectToValidate == null || matchFlag == false)
            {
                RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
            }
        }
   }
}
