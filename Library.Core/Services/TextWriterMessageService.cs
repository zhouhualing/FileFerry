
using System;
using System.IO;

namespace WD.Library.Core
{
	public class TextWriterMessageService : IMessageService
	{
		readonly TextWriter writer;
		
		public TextWriterMessageService(TextWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			this.writer = writer;
		}
		
		public void ShowError(string message)
		{
			writer.WriteLine(message);
		}
		
		public void ShowException(Exception ex, string message = null)
		{
			if (message != null) {
				writer.WriteLine(message);
			}
			if (ex != null) {
				writer.WriteLine(ex.ToString());
			}
		}
		
		public void ShowWarning(string message)
		{
			writer.WriteLine(message);
		}
		
		public void ShowMessage(string message, string caption)
		{
			writer.WriteLine(caption + ": " + message);
		}
		
		public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
		{
			writer.WriteLine(dialogName + ": " + message + " (" + fileName + ")");
			if (exceptionGot != null)
				writer.WriteLine(exceptionGot.ToString());
		}
		
		public void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			writer.WriteLine(StringParser.Format(formatstring, formatitems));
		}
		
		public void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			writer.WriteLine(StringParser.Format(formatstring, formatitems));
		}
		
		public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
		{
			writer.WriteLine(StringParser.Format(formatstring, formatitems));
		}

	}
}
