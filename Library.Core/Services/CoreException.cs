
using System;
using System.Runtime.Serialization;

namespace WD.Library.Core
{
    /// <summary>
    /// Base class for exceptions thrown by the Application core.
    /// </summary>
    [Serializable()]
	public class CoreException : Exception
	{
		public CoreException() : base()
		{
		}
		
		public CoreException(string message) : base(message)
		{
		}
		
		public CoreException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected CoreException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
