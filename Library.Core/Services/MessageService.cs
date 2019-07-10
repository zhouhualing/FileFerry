using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace WD.Library.Core
{
    public class MessageService : IMessageService
    {
        public void ShowError(string message)
        {
            Debug.WriteLine(message);
        }

        public void ShowException(Exception ex, string message = null)
        {
            Debug.WriteLine(ex.ToString());
        }

        public void ShowMessage(string message, string caption = null)
        {
            Debug.WriteLine(message);
        }

        public void ShowWarning(string message)
        {
            Debug.WriteLine(message);
        }

    }
}