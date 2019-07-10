//��ʼ�����ܼ�������

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
                    new System.Diagnostics.CounterCreationData("Cache Entries", "Ӧ���л����ڵ���������", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Cache Hits", "Ӧ�õĻ�����������", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Cache Misses", "Ӧ�õĻ���δ��������", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Cache Hit Ratio", "Ӧ�õĻ���������", System.Diagnostics.PerformanceCounterType.RawFraction),
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
        /// ��ʼ�����ܼ�������װ
        /// </summary>
        public CachePerformanceCounterInstaller()
        {

        }
    }
}