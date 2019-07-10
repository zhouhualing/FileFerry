using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using WD.Library.Core;
using WD.Library.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// Logger������
    /// </summary>
    /// <remarks>
    /// ���ڴ���Logger����Ĺ�����
    /// </remarks>
    public sealed class LoggerFactory
    {
        private static IDictionary loggers = new Dictionary<string, Logger>();

        private LoggerFactory()
        {
        }

        /// <summary>
        /// ����Name, �������ļ���ȡ��������Logger����
        /// </summary>
        /// <param name="name">Logger������</param>
        /// <returns>��ȡ��Logger����</returns>
        /// <remarks>
        /// lang="cs" region="Create Logger Test" tittle="��ȡLogger����"></code>
        /// </remarks>
        public static Logger Create(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "���ݵ�Logger���������Ϊ��");
            Logger logger = null;
            lock (loggers)
            {
                if (loggers[name] != null)
                    logger = (Logger)loggers[name];
                else
                {
                    logger = GetLoggerFromConfig(name);
                    if (loggers.Contains(name))
                        loggers[name] = logger;
                    else
                        loggers.Add(logger.Name, logger);
                }
            }

            return logger;
        }

        public static Logger Create()
        {
            return new Logger();
        }

        private static Logger GetLoggerFromConfig(string name)
        {
            LoggerElement logelement = LoggingSection.GetConfig().Loggers[name];

            ExceptionHelper.FalseThrow(logelement != null, "δ���ҵ�����Ϊ��{0}��Logger�����ý�", name);
            return new Logger(logelement.Name, logelement.Enabled);
        }
    }
}
