
using System;
using System.Runtime.Serialization;

namespace WD.Library.Core
{
	/// <summary>
	/// Is thrown when the AddInTree could not find the requested path.
	/// </summary>
	[Serializable()]
	public class TreePathNotFoundException : CoreException
	{
		/// <summary>
		/// Constructs a new <see cref="TreePathNotFoundException"/>
		/// </summary>
		public TreePathNotFoundException(string path) : base("Treepath not found: " + path)
		{
		}
		
		/// <summary>
		/// Constructs a new <see cref="TreePathNotFoundException"/>
		/// </summary>
		public TreePathNotFoundException() : base()
		{
		}
		
		/// <summary>
		/// Constructs a new <see cref="TreePathNotFoundException"/>
		/// </summary>
		public TreePathNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		/// <summary>
		/// Deserializes a <see cref="TreePathNotFoundException"/>
		/// </summary>
		protected TreePathNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
