
using System;

namespace WD.Library.Core
{
	public interface IMessageService
	{
		void ShowException(Exception ex, string message = null);
		
		void ShowError(string message);
		
		void ShowWarning(string message);
		
		void ShowMessage(string message, string caption = null);
		
	}
	
	sealed class FallbackMessageService : TextWriterMessageService
	{
		public FallbackMessageService() : base(Console.Out) {}
	}
	
}
