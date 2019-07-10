using System;
using System.Text;
using System.Collections.Generic;
using WD.Library.Core;

namespace WD.Library.Caching
{
    /// <summary>
    /// ��Cache�йص����ܼ�������װ
    /// </summary>
    public sealed class CachingPerformanceCounters
    {
        private PerformanceCounterWrapper entriesCounter;
        private PerformanceCounterWrapper hitsCounter;
        private PerformanceCounterWrapper missesCounter;
        private PerformanceCounterWrapper hitRatioCounter;
        private PerformanceCounterWrapper hitRatioBaseCounter;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="instanceName">ʵ������</param>
        public CachingPerformanceCounters(string instanceName)
        {
            PerformanceCounterInitData data = new PerformanceCounterInitData(
                "DeluxeWorks Caching", "Cache Entries", instanceName);
            this.entriesCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Hits";
            this.hitsCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Misses";
            this.missesCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Hit Ratio";
            this.hitRatioCounter = new PerformanceCounterWrapper(data);

            data.CounterName = "Cache Hit Ratio Base";
            this.hitRatioBaseCounter = new PerformanceCounterWrapper(data);
        }

        /// <summary>
        /// Cache��ļ���
        /// </summary>
        public PerformanceCounterWrapper EntriesCounter
        {
            get
            {
                return this.entriesCounter;
            }
        }

        /// <summary>
        /// ���д���
        /// </summary>
        public PerformanceCounterWrapper HitsCounter
        {
            get
            {
                return this.hitsCounter;
            }
        }

        /// <summary>
        /// û�����еĴ���
        /// </summary>
        public PerformanceCounterWrapper MissesCounter
        {
            get
            {
                return this.missesCounter;
            }
        }

        /// <summary>
        /// �������е����д���
        /// </summary>
        public PerformanceCounterWrapper HitRatioCounter
        {
            get
            {
                return this.hitRatioCounter;
            }
        }

        /// <summary>
        /// �������е��ܷ�����
        /// </summary>
        public PerformanceCounterWrapper HitRatioBaseCounter
        {
            get
            {
                return this.hitRatioBaseCounter;
            }
        }
    }
}