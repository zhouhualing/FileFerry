using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using WD.Library.Configuration;

namespace WD.Library.Config
{
    /// <summary>
    /// Cache��������Ϣ
    /// </summary>
    public sealed class ServiceContainerSettingsSection : ConfigurationSection
    {
        /// <summary>
        /// ��ȡCache��������Ϣ
        /// </summary>
        /// <returns>Cache��������Ϣ</returns>
        public static ServiceContainerSettingsSection GetConfig()
        {
            ServiceContainerSettingsSection result = (ServiceContainerSettingsSection)ConfigurationBroker.GetSection("serviceSettings");

            if (result == null)
                result = new ServiceContainerSettingsSection();

            return result;
        }

        private ServiceContainerSettingsSection()
        {
        }


        /// <summary>
        /// ����ÿ��Cache���е�����
        /// </summary>
        [ConfigurationProperty("services")]
        public ServiceSettingCollection Services
        {
            get
            {
                return (ServiceSettingCollection)this["services"];
            }
        }
    }

    /// <summary>
    /// ÿ��Cache���е����ü���
    /// </summary>
    public sealed class ServiceSettingCollection : ConfigurationElementCollection
    {
        private ServiceSettingCollection()
        {
        }

        /// <summary>
        /// ��ȡ����Ԫ�صļ�ֵ
        /// </summary>
        /// <param name="element">����Ԫ��</param>
        /// <returns>��ֵ</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceSetting)element).TypeName;
        }

        /// <summary>
        /// ��������Ԫ��
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceSetting();
        }

        /// <summary>
        /// ��ȡָ�����͵Ķ�������
        /// </summary>
        /// <param name="type">����</param>
        /// <returns>��������</returns>
        public ServiceSetting this[System.Type type]
        {
            get
            {
                return (ServiceSetting)BaseGet(type.FullName);
            }
        }

        public new ServiceSetting this[String type]
        {
            get
            {
                return (ServiceSetting)BaseGet(type);
            }
        }
    }

    /// <summary>
    /// ÿ��Cache���е�����
    /// </summary>
    public sealed class ServiceSetting : ConfigurationElement
    {
        private const int CacheDefaultQueueLength = 100;

        internal ServiceSetting()
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
    }
}
