
using System;

namespace WD.Library.Core
{	
	/// <summary>
	/// Condition evaluator that compares the state of the parameter with a specified value.
	/// The parameter has to implement <see cref="IOwnerState"/>.
	/// </summary>
	public class OwnerStateConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object parameter, Condition condition)
		{
			return false;
		}
	}
}
