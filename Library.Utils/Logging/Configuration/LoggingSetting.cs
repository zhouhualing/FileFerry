using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using WD.Library.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// 日志配置类
    /// </summary>
    /// <remarks>
    /// ConfigurationSection的派生类，不可继承
    /// </remarks>
    public sealed class LoggingSection : ConfigurationSection
    {
        private LoggingSection()
        {
        }

        /// <summary>
        /// 获取日志配置节对象
        /// </summary>
        /// <returns>LoggingSection对象</returns>
        /// <remarks>
        /// </remarks>
        public static LoggingSection GetConfig()
        {
            try
            {
                LoggingSection section = (LoggingSection)ConfigurationBroker.GetSection("LoggingSettings");

                if (section == null)
                    section = new LoggingSection();

                return section;
            }
            catch (Exception ex)
            {
                if (ex is LogException)
                    throw;
                else
                    throw new LogException("读取日志配置信息错误：" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Logger配置节类集合
        /// </summary>
        /// <remarks>
        /// 返回LoggerElements对象
        /// </remarks>
        [ConfigurationProperty("Loggers")]
        public LoggerElementCollection Loggers
        {
            get
            {
                return (LoggerElementCollection)this["Loggers"];
            }
        }

        /// <summary>
        /// Formatter配置节类集合
        /// </summary>
        /// <remarks>
        /// 返回LogConfigurationElementCollection对象
        /// </remarks>
        [ConfigurationProperty("Formatters")]
        public LogConfigurationElementCollection LogFormatterElements
        {
            get
            {
                return (LogConfigurationElementCollection)this["Formatters"];
            }
        }

        /// <summary>
        ///  重载方法
        /// </summary>
        /// <param name="reader"></param>
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            LogConfigurationElement.extendedAttributes.Clear();
            LoggerElement.elementCollectionDic.Clear();

            base.DeserializeSection(reader);
        }
    }
}
