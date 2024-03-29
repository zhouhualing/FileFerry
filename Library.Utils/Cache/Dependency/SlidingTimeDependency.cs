using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// 相对时间依赖，客户端代码在初始化此类的对象时，需要提供一个TimeSpan类型的过期时间段，
    /// 当从初始化此类对象到经过此时间段时，认为与此依赖项相关的Cache项过期。
    /// </summary>
    public sealed class SlidingTimeDependency : DependencyBase
    {
        private TimeSpan cacheItemExpirationTime = TimeSpan.Zero;

        /// <summary>
        /// 获取初始化时设定的过期时间间隔
        /// </summary>
        /// <remarks>        
        /// </remarks>
        public TimeSpan CacheItemExpirationTime
        {
            get { return this.cacheItemExpirationTime;  }
        }

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expirationTime">过期时间间隔</param>
        /// <remarks>
        
        /// </remarks>
        public SlidingTimeDependency(TimeSpan expirationTime)
        {
            this.cacheItemExpirationTime = expirationTime;

            //更新最后修改时间和最后访问时间
            UtcLastModified = DateTime.UtcNow;
            UtcLastAccessTime = DateTime.UtcNow;
        }
        #endregion 
        
        /// <summary>
        /// 属性，获取本Dependency是否过期
        /// </summary>
        /// <remarks>
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                return (DateTime.UtcNow - this.UtcLastModified) > this.CacheItemExpirationTime;
            }
        }
    }
}
