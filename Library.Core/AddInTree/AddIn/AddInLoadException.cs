
using System;
using System.Runtime.Serialization;

namespace WD.Library.Core
{
	/// <summary>
	/// Exception used when loading an AddIn fails.
	/// </summary>
	[Serializable]
	public class AddInLoadException : CoreException
	{
		public AddInLoadException() : base()
		{
		}
		
		public AddInLoadException(string message) : base(message)
		{
		}
		
		public AddInLoadException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected AddInLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
