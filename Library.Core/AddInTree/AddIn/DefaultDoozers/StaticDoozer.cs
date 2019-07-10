
using System;

namespace WD.Library.Core
{
	/// <summary>
	/// Retrieves an object instance by accessing a static field or property
	/// via System.Reflection.
	/// </summary>
	/// <attribute name="class" use="required">
	/// The fully qualified type name of the class that contains the static field/property.
	/// </attribute>
	/// <attribute name="member" use="required">
	/// The name of the static field or property.
	/// </attribute>
	/// <usage>Everywhere where objects are expected.</usage>
	/// <returns>
	/// The value of the field/property.
	/// </returns>
	public class StaticDoozer : IDoozer
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
			Codon codon = args.Codon;
			Type type = codon.AddIn.FindType(codon.Properties["class"]);
			if (type == null)
				return null;
			var memberName = codon.Properties["member"];
			var field = type.GetField(memberName);
			if (field != null)
				return field.GetValue(null);
			var property = type.GetProperty(memberName);
			if (property != null)
				return property.GetValue(null);
			throw new MissingFieldException("Field or property '" + memberName + "' not found in type " + type.FullName);
		}
	}
}
