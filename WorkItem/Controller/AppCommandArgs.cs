///////////////////////////////////////////////////////////////////////////////
// File Name : AppCommandArgs.cs
// Author    : zhou hualing
// Create At :
// Summary   : 命令参数的基础类型。可以继承，以应对特殊的参数传递。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    /// <summary>
    /// 命令参数。
    /// 针对不同的命令，必要时可以重载。
    /// 重载时，务必声明 [Serializable] 属性。
    /// </summary>
    [Serializable]
    public class AppCommandArgs
    {
        public static AppCommandArgs Empty = new AppCommandArgs();

        private Hashtable _data ;

        public AppCommandArgs() 
        {
        
        }

        /// <summary>
        /// 设置数据键/值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        public void AddData(string key,object obj)
        {
            if (_data == null)
                _data = new Hashtable();
            
            _data.Add(key, obj);
        }

        /// <summary>
        /// 根据键获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object GetData(string key)
        {
            if (_data == null)
                return null;
            else
                return _data[key];
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearData()
        {
            if (_data != null)
                _data.Clear();
        }

        public void SetData(string key,object obj)
        {
            _data[key] = obj;
        }

        internal bool TestData(string key)
        {
            if (_data == null)
                return false;
            else
                return _data.ContainsKey(key);
        }
    }
}
