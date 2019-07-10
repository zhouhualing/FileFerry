using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// Cache���е������
    /// </summary>
    public abstract class CacheQueueBase
    {
        private CachingPerformanceCounters totalCounters;
        private CachingPerformanceCounters counters;

        /// <summary>
        /// Cache�������
        /// </summary>
        public abstract int Count
        {
            get;
        }

		/// <summary>
		/// ���Cache����
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// �Ƿ񶼱��Ϊ����
		/// </summary>
		public abstract void SetChanged();
        
		/// <summary>
        /// �鷽����ɾ��Cache��
        /// </summary>
        /// <param name="cacheItem">��ɾ����Cache��</param>
        internal protected abstract void RemoveItem(CacheItemBase cacheItem);

        /// <summary>
        /// ���췽������ʼ������ָ��
        /// </summary>
        protected CacheQueueBase()
        {
           this.InitPerformanceCounters();
        }
        
        /// <summary>
        /// ��ʼ�����ܼ���ָ��
        /// </summary>
        protected void InitPerformanceCounters()
        {
            if (this.totalCounters == null)
                this.totalCounters = new CachingPerformanceCounters("_Total_");

			if (this.counters == null)
				this.counters = new CachingPerformanceCounters(this.GetType().Name);
        }

        /// <summary>
        /// ����Cache������ָ��
        /// </summary>
        protected CachingPerformanceCounters TotalCounters
        {
            get
            {
                return this.totalCounters;
            }
        }

        /// <summary>
        /// ����ָ��
        /// </summary>
        protected CachingPerformanceCounters Counters
        {
            get
            {
                return this.counters;
            }
        }
    }
}
