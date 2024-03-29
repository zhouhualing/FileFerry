﻿using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Validation
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class DateTimeEmptyValidatorAttribute : ValidatorAttribute
	{
		/// <summary>
		/// 创建校验器
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new DateTimeEmptyValidator(this.MessageTemplate, this.Tag);
		}
	}
}
