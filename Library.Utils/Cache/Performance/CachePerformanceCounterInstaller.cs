//初始化性能计数器类

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace WD.Library.Caching
{
    public class CachePerformanceCounterInstaller
    {
        private CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
        public virtual string CategoryName
        {
            get
            {
                return "DeluxeWorks Caching";
            }
        }

        public virtual CounterCreationDataCollection CounterCreationDataCollection
        {
            get
            {
                ccdc.AddRange(new System.Diagnostics.CounterCreationData[] {
                    new System.Diagnostics.CounterCreationData("Cache Entries", "应用中缓存内的总项数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Cache Hits", "应用的缓存命中数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Cache Misses", "应用的缓存未命中数。", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Cache Hit Ratio", "应用的缓存命中率", System.Diagnostics.PerformanceCounterType.RawFraction),
                    new System.Diagnostics.CounterCreationData("Cache Hit Ratio Base", string.Empty, System.Diagnostics.PerformanceCounterType.RawBase)});
                return ccdc;
            }
        }

        public virtual void Install()
        {
            if (PerformanceCounterCategory.Exists(CategoryName))
            {
                return;
            }

            PerformanceCounterCategory.Create(CategoryName, "", PerformanceCounterCategoryType.MultiInstance, CounterCreationDataCollection);
        }

        /// <summary>
        /// 初始化性能计数器安装
        /// </summary>
        public CachePerformanceCounterInstaller()
        {

        }
    }
}