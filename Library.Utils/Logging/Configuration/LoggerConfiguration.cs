
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace WD.Library.Logging
{
    /// <summary>
    /// Logger配置节类
    /// </summary>
    /// <remarks>
    /// Logger配置节对象，包含Filters和Listeners集合对象
    /// </remarks>
    public sealed class LoggerElement : ConfigurationElement
    {
        private LogFilterPipeline filters = null;
        private List<FormattedTraceListenerBase> listeners = null;

        private ReaderWriterLock dicRWLock = new ReaderWriterLock();
        internal static IDictionary<string, LogConfigurationElementCollection> elementCollectionDic = new Dictionary<string, LogConfigurationElementCollection>();
        
        internal LoggerElement()
        {
        }

        //static LoggerElement()
        //{
        //    elementCollectionDic.Clear();
        //}

        /// <summary>
        /// Logger的名称
        /// </summary>
        /// <remarks>
        /// 键值，必配项
        /// </remarks>
        [ConfigurationProperty("name", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
        }

        /// <summary>
        /// Logger是否可用的标志
        /// </summary>
        [ConfigurationProperty("enable")]
        public bool Enabled
        {
            get
            {
                return (bool)this["enable"];
            }
        }

        /// <summary>
        /// Filters配置集合
        /// </summary>
        /// <remarks>
        /// 返回LogConfigurationElementCollection对象
        /// </remarks>
        public LogConfigurationElementCollection LogFilterElementCollection
        {
            get
            {
                string key = this.Name + "~Filters";
                LogConfigurationElementCollection result;

                dicRWLock.AcquireReaderLock(1000);

                try
                {
                    if (elementCollectionDic.TryGetValue(key, out result) == false)
                        result = new LogConfigurationElementCollection();

                    return result;
                }
                finally
                {
                    dicRWLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// Listeners配置集合
        /// </summary>
        /// <remarks>
        /// 返回LogListenerElements对象
        /// </remarks>
        public LogListenerElementCollection LogListenerElementCollection
        {
            get
            {
                string key = this.Name + "~Listeners";

                LogConfigurationElementCollection result;

                dicRWLock.AcquireReaderLock(1000);

                try
                {
                    if (elementCollectionDic.TryGetValue(key, out result) == false)
                        result = new LogListenerElementCollection();

                    return (LogListenerElementCollection)result;
                }
                finally
                {
                    dicRWLock.ReleaseReaderLock();
                }
            }
        }

        internal LogFilterPipeline LogFilters
        {
            get
            {
                if (this.filters == null)
                    this.filters = LogFilterFactory.GetFilterPipeLine(this.LogFilterElementCollection);

                return this.filters;
            }
        }

        internal List<FormattedTraceListenerBase> LogListeners
        {
            get
            {
                if (this.listeners == null)
                    this.listeners = TraceListenerFactory.GetListeners(this.LogListenerElementCollection);

                return this.listeners;
            }
        }

        /// <summary>
        /// 重载方法
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            bool result = false;

            switch(elementName)
            {
                case "Filters":
                    CreateFilterElementCollection(reader);
                    result = true;
                    break;
                case "Listeners":
                    CreateListenerElementCollection(reader);
                    result = true;
                    break;
                default:
                    result = base.OnDeserializeUnrecognizedElement(elementName, reader);
                    break;
            }

            return result;
        }

        private void CreateFilterElementCollection(System.Xml.XmlReader reader)
        {
            LogConfigurationElementCollection logFilterElementCollection = new LogConfigurationElementCollection(this.Name + "~Filters");
            
            logFilterElementCollection.DeserializeElementDirectly(reader, false);

            this.dicRWLock.AcquireWriterLock(1000);
            try
            {
                elementCollectionDic.Add(this.Name + "~Filters", logFilterElementCollection);
            }
            finally
            {
                this.dicRWLock.ReleaseWriterLock();
            }
        }

        private void CreateListenerElementCollection(System.Xml.XmlReader reader)
        {
            LogListenerElementCollection logListenerElementCollection = new LogListenerElementCollection(this.Name + "~Listeners");
            logListenerElementCollection.DeserializeElementDirectly(reader, false);

            this.dicRWLock.AcquireWriterLock(1000);
            try
            {
                elementCollectionDic.Add(this.Name + "~Listeners", logListenerElementCollection);
            }
            finally
            {
                this.dicRWLock.ReleaseWriterLock();
            }
        }
    }

    /// <summary>
    /// LoggerElement集合类
    /// </summary>
    /// <remarks>
    /// ConfigurationElementCollection的派生类，不可继承
    /// </remarks>
    public sealed class LoggerElementCollection : ConfigurationElementCollection
    {
        private LoggerElementCollection()
        {
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="element">LoggerElement对象</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerElement)element).Name;
        }

        /// <summary>
        /// 创建新的LoggerElement对象
        /// </summary>
        /// <returns>LoggerElement对象</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerElement();
        }

        /// <summary>
        /// 根据键值索引，返回LoggerElement对象
        /// </summary>
        /// <param name="name">LoggerElement名称</param>
        /// <returns>LoggerElement对象</returns>
        /// <remarks>
        /// </remarks>
        new public LoggerElement this[string name]
        {
            get
            {
                return (LoggerElement)BaseGet(name);
            }
            //set
            //{
            //    if (BaseGet(name) != null)
            //        BaseRemoveAt(name);

            //    BaseAdd(name, value);
            //}
        }
    }
}
