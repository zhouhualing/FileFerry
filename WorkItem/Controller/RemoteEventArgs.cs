///////////////////////////////////////////////////////////////////////////////
// File Name : RemoteEventArgs.cs
// Author    : zhou hualing
// Create At :
// Summary   : 远程事件传递的参数类型。可以在不同的进程、域间传递参数值。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    public class RemoteEventArgs : MarshalByRefObject, IAppEventArgs
    {
        private IAppEventArgs _args = null;

        public RemoteEventArgs(IAppEventArgs args)
        {
            Debug.Assert(args != null);

            _args = args;
        }

        public object GetData(string key)
        {
            return _args.GetData(key);
        }

        public void AddData(string key, object val)
        {
            _args.AddData(key, val);
        }
    }
}
