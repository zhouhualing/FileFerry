
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace WD.Library.Logging
{
    /// <summary>
    /// Logger���ý���
    /// </summary>
    /// <remarks>
    /// Logger���ýڶ��󣬰���Filters��Listeners���϶���
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
        /// Logger������
        /// </summary>
        /// <remarks>
        /// ��ֵ��������
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
        /// Logger�Ƿ���õı�־
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
        /// Filters���ü���
        /// </summary>
        /// <remarks>
        /// ����LogConfigurationElementCollection����
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
        /// Listeners���ü���
        /// </summary>
        /// <remarks>
        /// ����LogListenerElements����
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
        /// ���ط���
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
    /// LoggerElement������
    /// </summary>
    /// <remarks>
    /// ConfigurationElementCollection�������࣬���ɼ̳�
    /// </remarks>
    public sealed class LoggerElementCollection : ConfigurationElementCollection
    {
        private LoggerElementCollection()
        {
        }

        /// <summary>
        /// ��ȡ��ֵ
        /// </summary>
        /// <param name="element">LoggerElement����</param>
        /// <returns>��ֵ</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerElement)element).Name;
        }

        /// <summary>
        /// �����µ�LoggerElement����
        /// </summary>
        /// <returns>LoggerElement����</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerElement();
        }

        /// <summary>
        /// ���ݼ�ֵ����������LoggerElement����
        /// </summary>
        /// <param name="name">LoggerElement����</param>
        /// <returns>LoggerElement����</returns>
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
