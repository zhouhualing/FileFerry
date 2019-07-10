using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using WD.Library.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// ��־������
    /// </summary>
    /// <remarks>
    /// ConfigurationSection�������࣬���ɼ̳�
    /// </remarks>
    public sealed class LoggingSection : ConfigurationSection
    {
        private LoggingSection()
        {
        }

        /// <summary>
        /// ��ȡ��־���ýڶ���
        /// </summary>
        /// <returns>LoggingSection����</returns>
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
                    throw new LogException("��ȡ��־������Ϣ����" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Logger���ý��༯��
        /// </summary>
        /// <remarks>
        /// ����LoggerElements����
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
        /// Formatter���ý��༯��
        /// </summary>
        /// <remarks>
        /// ����LogConfigurationElementCollection����
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
        ///  ���ط���
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
