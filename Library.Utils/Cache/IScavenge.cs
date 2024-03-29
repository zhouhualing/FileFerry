using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// 接口，各中CacheQueue通过实现此接口完成自身的清理工作
    /// </summary>
    public interface IScavenge
    {
        /// <summary>
        /// Cache对列清理方法
        /// </summary>
        void DoScavenging();
    }
}
