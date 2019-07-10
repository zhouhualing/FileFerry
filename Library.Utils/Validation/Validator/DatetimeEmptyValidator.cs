using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Validation
{
	/// <summary>
	/// 日期类型的为空判断的校验器
	/// </summary>
	public class DateTimeEmptyValidator : Validator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageTemplate"></param>
		/// <param name="tag"></param>
		public DateTimeEmptyValidator(string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectToValidate"></param>
		/// <param name="currentObject"></param>
		/// <param name="key"></param>
		/// <param name="validationResults"></param>
		protected internal override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validationResults)
		{
			if ((DateTime)objectToValidate == DateTime.MinValue)
				RecordValidationResult(validationResults, this.MessageTemplate, currentObject, key);
		}
	}
}
