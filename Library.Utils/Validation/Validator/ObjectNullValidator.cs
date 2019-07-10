using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
	/// <summary>
	/// 
	/// </summary>
	public class ObjectNullValidator : Validator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageTemplate"></param>
		/// <param name="tag"></param>
		public ObjectNullValidator(string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
		}
 
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectToValidate"></param>
		/// <param name="currentObject"></param>
		/// <param name="key"></param>
		/// <param name="validateResults"></param>
		protected internal override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			if (objectToValidate == null)
				RecordValidationResult(validateResults, this.MessageTemplate, currentObject, key);
		}
	}
}
