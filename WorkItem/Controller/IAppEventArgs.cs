///////////////////////////////////////////////////////////////////////////////
// File Name : IAppEventArgs.cs
// Author    : zhou hualing
// Create At :
// Summary   : 事件参数的基础接口。
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Collections.Generic;

namespace Framework.WorkItem
{
    public interface IAppEventArgs
    {
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="key">参数键名</param>
        /// <returns>参数值</returns>
        object GetData(string key);

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="key">参数键名</param>
        /// <param name="val">参数值</param>
        void AddData(string key,object val);
    }
}
