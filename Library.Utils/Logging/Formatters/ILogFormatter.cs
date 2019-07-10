using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Logging
{
    /// <summary>
    /// 接口，格式化日志记录
    /// </summary>
    /// <remarks>
    /// 定义将日志记录LogEntity对象格式化成字符串的格式化器，通过实现该接口来实现定制的格式化器，如：文本、XML等
    /// </remarks>
    public interface ILogFormatter
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 接口方法，格式化LogEntity对象成一个字符串
        /// </summary>
        /// <param name="log">LogEntity对象</param>
        /// <returns>格式化成的字符串</returns>
        string Format(LogEntity log);
    }
}
