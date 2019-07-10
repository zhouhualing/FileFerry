﻿
using System;
using System.Collections;

namespace WD.Library.Core
{
	/// <summary>
	/// Creates a string.
	/// </summary>
	/// <attribute name="text" use="required">
	/// The string to return.
	/// </attribute>
	/// <returns>
	/// The string specified by 'text', passed through the StringParser.
	/// </returns>
	public class StringDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return StringParser.Parse(args.Codon.Properties["text"]);
		}
	}
}
