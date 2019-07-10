
using System;
using System.Collections;

namespace WD.Library.Core
{
	/// <summary>
	/// Interface for classes that can build objects out of codons.
	/// </summary>
	/// <remarks>http://en.wikipedia.org/wiki/Fraggle_Rock#Doozers</remarks>
	public interface IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		bool HandleConditions { get; }
		
		/// <summary>
		/// Construct the item.
		/// </summary>
		/// <returns>
		/// The constructed item.
		/// May return an object implementing <see cref="IBuildItemsModifier"/> for returning
		/// multiple arguments.
		/// </returns>
		object BuildItem(BuildItemArgs args);
	}
}
