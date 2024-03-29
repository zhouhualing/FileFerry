﻿
using System;
using System.Collections;

namespace WD.Library.Core
{
	/// <summary>
	/// This doozer lazy-loads another doozer when it has to build an item.
	/// It is used internally to wrap doozers specified in addins.
	/// </summary>
	sealed class LazyLoadDoozer : IDoozer
	{
		AddIn addIn;
		string name;
		string className;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public LazyLoadDoozer(AddIn addIn, DataProperties properties)
		{
			this.addIn      = addIn;
			this.name       = properties["name"];
			this.className  = properties["class"];
		}
		
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				IDoozer doozer = (IDoozer)addIn.CreateObject(className);
				if (doozer == null) {
					return false;
				}
				addIn.AddInTree.Doozers[name] = doozer;
				return doozer.HandleConditions;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			IDoozer doozer = (IDoozer)addIn.CreateObject(className);
			if (doozer == null) {
				return null;
			}
			addIn.AddInTree.Doozers[name] = doozer;
			return doozer.BuildItem(args);
		}
		
		public override string ToString()
		{
			return String.Format("[LazyLoadDoozer: className = {0}, name = {1}]",
			                     className,
			                     name);
		}
	}
}
