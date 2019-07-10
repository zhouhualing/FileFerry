
using System;

namespace WD.Library.Core
{
	/// <summary>
	/// Interface for classes that can evaluate conditions defined in the addin tree.
	/// </summary>
	public interface IConditionEvaluator
	{
		bool IsValid(object parameter, Condition condition);
	}
}
