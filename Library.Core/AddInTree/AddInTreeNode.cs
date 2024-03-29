﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WD.Library.Core
{
	/// <summary>
	/// Represents an extension path in the <see cref="AddInTree"/>.
	/// </summary>
	public sealed class AddInTreeNode
	{
		readonly object lockObj = new object();
		Dictionary<string, AddInTreeNode> childNodes = new Dictionary<string, AddInTreeNode>();
		ReadOnlyCollection<Codon> codons;
		List<IEnumerable<Codon>> codonInput;
		
		/// <summary>
		/// A dictionary containing the child paths.
		/// </summary>
		public Dictionary<string, AddInTreeNode> ChildNodes {
			get {
				return childNodes;
			}
		}
		
		public void AddCodons(IEnumerable<Codon> newCodons)
		{
			if (newCodons == null)
				throw new ArgumentNullException("newCodons");
			lock (lockObj) {
				if (codonInput == null) {
					codonInput = new List<IEnumerable<Codon>>();
					if (codons != null)
						codonInput.Add(codons);
				}
				codonInput.Add(newCodons);
			}
		}
		
		/// <summary>
		/// A list of child <see cref="Codon"/>s.
		/// </summary>
		public ReadOnlyCollection<Codon> Codons {
			get {
				lock (lockObj) {
					if (codons == null) {
						if (codonInput == null) {
							codons = new ReadOnlyCollection<Codon>(new Codon[0]);
						} else {
							codons = TopologicalSort.Sort(codonInput).AsReadOnly();
							codonInput = null;
						}
					}
					return codons;
				}
			}
		}
		
		/// <summary>
		/// Builds the child items in this path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <param name="additionalConditions">Additional conditions applied to the node.</param>
		public List<T> BuildChildItems<T>(object parameter, IEnumerable<ICondition> additionalConditions = null)
		{
			var codons = this.Codons;
			List<T> items = new List<T>(codons.Count);
			foreach (Codon codon in codons) {
				object result = BuildChildItem(codon, parameter, additionalConditions);
				if (result == null)
					continue;
				IBuildItemsModifier mod = result as IBuildItemsModifier;
				if (mod != null) {
					mod.Apply(items);
				} else if (result is T) {
					items.Add((T)result);
				} else {
					throw new InvalidCastException("The AddInTreeNode <" + codon.Name + " id='" + codon.Id
					                               + "'> returned an instance of " + result.GetType().FullName
					                               + " but the type " + typeof(T).FullName + " is expected.");
				}
			}
			return items;
		}
		
		public object BuildChildItem(Codon codon, object parameter, IEnumerable<ICondition> additionalConditions = null)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			
			AddInTreeNode subItemNode;
			childNodes.TryGetValue(codon.Id, out subItemNode);
			
			IReadOnlyCollection<ICondition> conditions;
			if (additionalConditions == null)
				conditions = codon.Conditions;
			else if (codon.Conditions == null)
				conditions = additionalConditions.ToList();
			else
				conditions = additionalConditions.Concat(codon.Conditions).ToList();
			
			return codon.BuildItem(new BuildItemArgs(parameter, codon, conditions, subItemNode));
		}
		
		/// <summary>
		/// Builds a specific child items in this path.
		/// </summary>
		/// <param name="childItemID">
		/// The ID of the child item to build.
		/// </param>
		/// <param name="parameter">The owner used to create the objects.</param>
		/// <param name="additionalConditions">Additional conditions applied to the created object</param>
		/// <exception cref="TreePathNotFoundException">
		/// Occurs when <paramref name="childItemID"/> does not exist in this path.
		/// </exception>
		public object BuildChildItem(string childItemID, object parameter, IEnumerable<ICondition> additionalConditions = null)
		{
			foreach (Codon codon in this.Codons) {
				if (codon.Id == childItemID) {
					return BuildChildItem(codon, parameter, additionalConditions);
				}
			}
			throw new TreePathNotFoundException(childItemID);
		}
	}
}
