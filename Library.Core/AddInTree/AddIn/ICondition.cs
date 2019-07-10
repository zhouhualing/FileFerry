
using System;

namespace WD.Library.Core
{
	/// <summary>
	/// Default actions, when a condition is failed.
	/// </summary>
	public enum ConditionFailedAction {
		Nothing,
		Exclude,
		Disable
	}
		
	/// <summary>
	/// Interface for single condition or complex condition.
	/// </summary>
	public interface ICondition
	{
		string Name {
			get;
		}
		/// <summary>
		/// Returns the action which occurs, when this condition fails.
		/// </summary>
		ConditionFailedAction Action {
			get;
			set;
		}
		
		/// <summary>
		/// Returns true, when the condition is valid otherwise false.
		/// </summary>
		bool IsValid(object parameter);
	}
}
