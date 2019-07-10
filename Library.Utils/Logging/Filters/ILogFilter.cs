using System;
using System.Collections.Generic;
using System.Text;

using WD.Library.Core;
using WD.Library.Accessories;

namespace WD.Library.Logging
{
    /// <summary>
    /// 接口，定义日志过滤器
    /// </summary>
#if DELUXEWORKSTEST
    public interface ILogFilter : IFilter<LogEntity>
#else
    internal interface ILogFilter : IFilter<LogEntity>
#endif
    {
        /// <summary>
        /// 名称
        /// </summary>
        new string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 接口方法，实现日志过滤
        /// </summary>
        /// <param name="log">日志对象</param>
        /// <returns>布尔值，true：通过，false：不通过</returns>
        new bool IsMatch(LogEntity log);
    }
}
