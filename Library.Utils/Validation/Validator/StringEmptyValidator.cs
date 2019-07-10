using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
	/// <summary>
	/// 字符串为空的校验逻辑
	/// </summary>
	internal class StringEmptyValidator : Validator
	{
		public StringEmptyValidator(string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
		}

		protected internal override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validationResults)
		{
			if (objectToValidate is string || objectToValidate == null)
			{
				if (string.IsNullOrEmpty((string)objectToValidate))
					RecordValidationResult(validationResults, this.MessageTemplate, currentObject, key);
			}
		}
	}
}
