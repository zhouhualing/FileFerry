
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// LogListener���ýڵĻ���
    /// </summary>
    /// <remarks>
    /// LogConfigurationElement�������࣬��չLogFormatter��������Ϣ
    /// </remarks>
    public class LogListenerElement : LogConfigurationElement
    {
        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public LogListenerElement()
        {
        }

        internal LogListenerElement(string fullPath)
            : base(fullPath)
        {
        }

        /// <summary>
        /// LogFormatter������
        /// </summary>
        [ConfigurationProperty("formatter", IsRequired=false)]
        public string LogFormatterName
        {
            get
            {
                return (string)this["formatter"];
            }
        }
    }

    /// <summary>
    /// LogListenerElement������
    /// </summary>
    /// <remarks>
    /// LogConfigurationElementCollection��������
    /// </remarks>
    public sealed class LogListenerElementCollection : LogConfigurationElementCollection
    {
        private string fullPath = string.Empty;

        internal LogListenerElementCollection()
        {
        }

        internal LogListenerElementCollection(string fullPath)
            : base(fullPath)
        {
            this.fullPath = fullPath;
        }

        /// <summary>
        /// �����µ�LogListenerElement����
        /// </summary>
        /// <returns>LogListenerElement����</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogListenerElement(this.fullPath);
        }

        /// <summary>
        /// ��ȡ��ֵ
        /// </summary>
        /// <param name="element">LogListenerElement����</param>
        /// <returns>��ֵ</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogListenerElement)element).Name;
        }

        /// <summary>
        /// ���ݼ�ֵ����������LogListenerElement����
        /// </summary>
        /// <param name="name">LogListenerElement����</param>
        /// <returns>LogListenerElement����</returns>
        /// <remarks>
        /// </remarks>
        new public LogListenerElement this[string name]
        {
            get
            {
                return (LogListenerElement)BaseGet(name);
            }
        }
    }
}
