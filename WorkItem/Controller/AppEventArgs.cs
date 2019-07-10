///////////////////////////////////////////////////////////////////////////////
// File Name : AppEventArgs.cs
// Author    : zhou hualing
// Create At :
// Summary   : 默认的事件参数类型。适用于进程内使用。可以继承，以应对需要传递
//              额外数据的情况。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    [Serializable]
    public class AppEventArgs : IAppEventArgs
    {
        public static AppEventArgs Empty = new AppEventArgs();


        private Hashtable _data;


        public void AddData(string key, object val)
        {
            if (_data == null)
                _data = new Hashtable();

            _data.Add(key, val);
        }
        
        public object GetData(string key)
        {
            if (_data == null)
                return null;
            else
                return _data[key];
        }
    }
}
