﻿
using System;

namespace WD.Library.Core
{
	/// <summary>
	/// Condition evaluator that lazy-loads another condition evaluator and executes it.
	/// </summary>
	sealed class LazyConditionEvaluator : IConditionEvaluator
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public LazyConditionEvaluator(AddIn addIn, DataProperties properties)
		{
			if (addIn == null)
				throw new ArgumentNullException("addIn");
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
		}
		
		public bool IsValid(object parameter, Condition condition)
		{
			IConditionEvaluator evaluator = (IConditionEvaluator)addIn.CreateObject(className);
			if (evaluator == null)
				return false;
			addIn.AddInTree.ConditionEvaluators[name] = evaluator;
			return evaluator.IsValid(parameter, condition);
		}
		
		public override string ToString()
		{
			return String.Format("[LazyLoadConditionEvaluator: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
	}
}
