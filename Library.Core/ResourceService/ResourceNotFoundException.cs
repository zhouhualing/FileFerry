
using System;
using System.Runtime.Serialization;

namespace WD.Library.Core
{
	/// <summary>
	/// Is thrown when the GlobalResource manager can't find a requested
	/// resource.
	/// </summary>
	[Serializable()]
	public class ResourceNotFoundException : CoreException
	{
		string resourceName;
		public string ResourceName { get { return resourceName; } }
		
		public ResourceNotFoundException(string resourceName) : base("Resource not found : " + resourceName)
		{
			this.resourceName = resourceName;
		}
		
		public ResourceNotFoundException() : base()
		{
		}
		
		public ResourceNotFoundException(string resourceName, Exception innerException) : base("Resource not found : " + resourceName, innerException)
		{
			this.resourceName = resourceName;
		}
		
		protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
