using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Validation
{
	/// <summary>
	/// 枚举类型是否是缺省值的校验器（如果是，则相当于为空，报出错误）
	/// </summary>
	public class EnumDefaultValueValidator : Validator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageTemplate"></param>
		/// <param name="tag"></param>
		public EnumDefaultValueValidator(string messageTemplate, string tag) :
			base(messageTemplate, tag)
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
			bool isValid = true;

			if (objectToValidate != null && objectToValidate.GetType().IsEnum)
			{
				object defaultData = Activator.CreateInstance(objectToValidate.GetType());
				isValid = objectToValidate.Equals(defaultData) == false;
			}

			if (isValid == false)
				RecordValidationResult(validateResults, this.MessageTemplate, currentObject, key);
		}
	}
}
