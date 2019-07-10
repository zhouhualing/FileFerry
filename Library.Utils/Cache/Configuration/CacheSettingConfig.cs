using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using WD.Library.Configuration;

namespace WD.Library.Caching
{
    /// <summary>
    /// Cache��������Ϣ
    /// </summary>
    public sealed class CacheSettingsSection : ConfigurationSection
    {
        /// <summary>
        /// ��ȡCache��������Ϣ
        /// </summary>
        /// <returns>Cache��������Ϣ</returns>
        public static CacheSettingsSection GetConfig()
        {
            CacheSettingsSection result = (CacheSettingsSection)ConfigurationBroker.GetSection("cacheSettings");

            if (result == null)
                result = new CacheSettingsSection();

            return result;
        }

        private CacheSettingsSection()
        {
        }

        /// <summary>
        /// ȱʡ�Ķ��г���
        /// </summary>
        [ConfigurationProperty("defaultQueueLength", DefaultValue = 100)]
        public int DefaultQueueLength
        {
            get
            {
                return (int)this["defaultQueueLength"];
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        public TimeSpan ScanvageInterval
        {
            get
            {
                return TimeSpan.FromSeconds(this.ScanvageIntervalSeconds);
            }
        }

        [ConfigurationProperty("scanvageInterval", DefaultValue = 60)]
        private int ScanvageIntervalSeconds
        {
            get
            {
                return (int)this["scanvageInterval"];
            }
        }

		/*
        /// <summary>
        /// ���Cache����û�ж������ܼ�����ʵ�����ƣ�����ʹ�õ�ȱʡ��Cache���е����ܼ�������ʵ�����ơ�
        /// ���DefaultInstanceNameҲû�ж��壬����ʹ��Cache���е���������
        /// </summary>
        [ConfigurationProperty("defaultInstanceName", DefaultValue = "")]
        public string DefaultInstanceName
        {
            get
            {
                return (string)this["defaultInstanceName"];
            }
        }
		*/

        /// <summary>
        /// ����ÿ��Cache���е�����
        /// </summary>
        [ConfigurationProperty("queueSettings")]
        public QueueSettingCollection QueueSettings
        {
            get
            {
                return (QueueSettingCollection)this["queueSettings"];
            }
        }
    }

    /// <summary>
    /// ÿ��Cache���е����ü���
    /// </summary>
    public sealed class QueueSettingCollection : ConfigurationElementCollection
    {
        private QueueSettingCollection()
        {
        }

        /// <summary>
        /// ��ȡ����Ԫ�صļ�ֵ
        /// </summary>
        /// <param name="element">����Ԫ��</param>
        /// <returns>��ֵ</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueueSetting)element).TypeName;
        }

        /// <summary>
        /// ��������Ԫ��
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueSetting();
        }

        /// <summary>
        /// ��ȡָ�����͵Ķ�������
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>��������</returns>
        public QueueSetting this[System.Type type]
        {
            get
            {
                return (QueueSetting)BaseGet(type.FullName);
            }
        }
    }

    /// <summary>
    /// ÿ��Cache���е�����
    /// </summary>
    public sealed class QueueSetting : ConfigurationElement
    {
        private const int CacheDefaultQueueLength = 100;

        internal QueueSetting()
        {
        }

        /// <summary>
        /// �������������
        /// </summary>
        [ConfigurationProperty("typeName", Options = ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string TypeName
        {
            get
            {
                return (string)this["typeName"];
            }
        }

        /// <summary>
        /// ���е����
        /// </summary>
        [ConfigurationProperty("queueLength", DefaultValue = CacheDefaultQueueLength)]
        public int QueueLength
        {
            get
            {
                return (int)this["queueLength"];
            }
        }

		/*
        /// <summary>
        /// ��Cache��������Ӧ�����ܼ�������ʵ������
        /// </summary>
        [ConfigurationProperty("instanceName", DefaultValue = "")]
        public string InstanceName
        {
            get
            {
                return (string)this["instanceName"];
            }
        }
		 * */
    }
}
