using System;
using System.Collections.Generic;
using System.Text;

using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// 日志记录优先级枚举
    /// </summary>
    /// <remarks>
    /// 共分五级优先级
    /// </remarks>
    public enum LogPriority
    {
        /// <summary>
        /// 最低优先级
        /// </summary>
        Lowest,

        /// <summary>
        /// 低优先级
        /// </summary>
        BelowNormal,

        /// <summary>
        /// 普通
        /// </summary>
        Normal,

        /// <summary>
        /// 高优先级
        /// </summary>
        AboveNormal,

        /// <summary>
        /// 最高优先级
        /// </summary>
        Highest,
    }
}
