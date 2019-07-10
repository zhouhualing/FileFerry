using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// ���ʱ���������ͻ��˴����ڳ�ʼ������Ķ���ʱ����Ҫ�ṩһ��TimeSpan���͵Ĺ���ʱ��Σ�
    /// ���ӳ�ʼ��������󵽾�����ʱ���ʱ����Ϊ�����������ص�Cache����ڡ�
    /// </summary>
    public sealed class SlidingTimeDependency : DependencyBase
    {
        private TimeSpan cacheItemExpirationTime = TimeSpan.Zero;

        /// <summary>
        /// ��ȡ��ʼ��ʱ�趨�Ĺ���ʱ����
        /// </summary>
        /// <remarks>        
        /// </remarks>
        public TimeSpan CacheItemExpirationTime
        {
            get { return this.cacheItemExpirationTime;  }
        }

        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="expirationTime">����ʱ����</param>
        /// <remarks>
        
        /// </remarks>
        public SlidingTimeDependency(TimeSpan expirationTime)
        {
            this.cacheItemExpirationTime = expirationTime;

            //��������޸�ʱ���������ʱ��
            UtcLastModified = DateTime.UtcNow;
            UtcLastAccessTime = DateTime.UtcNow;
        }
        #endregion 
        
        /// <summary>
        /// ���ԣ���ȡ��Dependency�Ƿ����
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
