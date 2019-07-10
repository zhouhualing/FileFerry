using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using WD.Library.Configuration;

namespace WD.Library.Config
{
    /// <summary>
    /// Cache的配置信息
    /// </summary>
    public sealed class ServiceContainerSettingsSection : ConfigurationSection
    {
        /// <summary>
        /// 获取Cache的配置信息
        /// </summary>
        /// <returns>Cache的配置信息</returns>
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
        /// 具体每个Cache队列的设置
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
    /// 每个Cache队列的设置集合
    /// </summary>
    public sealed class ServiceSettingCollection : ConfigurationElementCollection
    {
        private ServiceSettingCollection()
        {
        }

        /// <summary>
        /// 获取配置元素的键值
        /// </summary>
        /// <param name="element">配置元素</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceSetting)element).TypeName;
        }

        /// <summary>
        /// 创建配置元素
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceSetting();
        }

        /// <summary>
        /// 获取指定类型的队列设置
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>队列设置</returns>
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
    /// 每个Cache队列的设置
    /// </summary>
    public sealed class ServiceSetting : ConfigurationElement
    {
        private const int CacheDefaultQueueLength = 100;

        internal ServiceSetting()
        {
        }

        /// <summary>
        /// 对象的类型名称
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
