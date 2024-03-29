﻿
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WD.Library.Core
{
    public interface IAddInTree
	{
		/// <summary>
		/// Gets the AddIns that are registered for this AddIn tree.
		/// </summary>
		IReadOnlyList<AddIn> AddIns { get; }
		
		/// <summary>
		/// Gets a dictionary of registered doozers.
		/// </summary>
		ConcurrentDictionary<string, IDoozer> Doozers { get; }
		
		/// <summary>
		/// Gets a dictionary of registered condition evaluators.
		/// </summary>
		ConcurrentDictionary<string, IConditionEvaluator> ConditionEvaluators { get; }
		
		/// <summary>
		/// Builds the items in the path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <param name="throwOnNotFound">If true, throws a <see cref="TreePathNotFoundException"/>
		/// if the path is not found. If false, an empty list is returned when the
		/// path is not found.</param>
		IReadOnlyList<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true);
		
		/// <summary>
		/// Builds a single item in the addin tree.
		/// </summary>
		/// <param name="path">A path to the item in the addin tree.</param>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <exception cref="TreePathNotFoundException">The path does not
		/// exist or does not point to an item.</exception>
		object BuildItem(string path, object parameter);
		
		object BuildItem(string path, object parameter, IEnumerable<ICondition> additionalConditions);
		
		/// <summary>
		/// Gets the <see cref="AddInTreeNode"/> representing the specified path.
		/// </summary>
		/// <param name="path">The path of the AddIn tree node</param>
		/// <param name="throwOnNotFound">
		/// If set to <c>true</c>, this method throws a
		/// <see cref="TreePathNotFoundException"/> when the path does not exist.
		/// If set to <c>false</c>, <c>null</c> is returned for non-existing paths.
		/// </param>
		AddInTreeNode GetTreeNode(string path, bool throwOnNotFound = true);
	}
}
