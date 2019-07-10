
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// LogListener配置节的基类
    /// </summary>
    /// <remarks>
    /// LogConfigurationElement的派生类，扩展LogFormatter的名称信息
    /// </remarks>
    public class LogListenerElement : LogConfigurationElement
    {
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public LogListenerElement()
        {
        }

        internal LogListenerElement(string fullPath)
            : base(fullPath)
        {
        }

        /// <summary>
        /// LogFormatter的名称
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
    /// LogListenerElement集合类
    /// </summary>
    /// <remarks>
    /// LogConfigurationElementCollection的派生类
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
        /// 创建新的LogListenerElement对象
        /// </summary>
        /// <returns>LogListenerElement对象</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogListenerElement(this.fullPath);
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <returns>键值</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LogListenerElement)element).Name;
        }

        /// <summary>
        /// 根据键值索引，返回LogListenerElement对象
        /// </summary>
        /// <param name="name">LogListenerElement名称</param>
        /// <returns>LogListenerElement对象</returns>
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
