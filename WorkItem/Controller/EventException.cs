///////////////////////////////////////////////////////////////////////////////
// File Name : EventException.cs
// Author    : zhou hualing
// Create At :
// Summary   : 事件处理异常。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.WorkItem
{
    [Serializable]
    class EventException : System.Exception
    {
        public EventException()
        {
        }

        public EventException(string msg)
            : base(msg)
        {
        }

        public EventException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        protected EventException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
