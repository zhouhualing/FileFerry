///////////////////////////////////////////////////////////////////////////////
// File Name : CommandException.cs
// Author    : zhou hualing
// Create At :
// Summary   : 命令处理异常
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.WorkItem
{
    [Serializable]
    public class CommandException : System.Exception
    {
        public CommandException()
        {
        }

        public CommandException(string msg)
            : base(msg)
        {
        }

        public CommandException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        protected CommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
