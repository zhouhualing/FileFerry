using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using WD.Library.Core;
using WD.Library.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// Logger工厂类
    /// </summary>
    /// <remarks>
    /// 用于创建Logger对象的工厂类
    /// </remarks>
    public sealed class LoggerFactory
    {
        private static IDictionary loggers = new Dictionary<string, Logger>();

        private LoggerFactory()
        {
        }

        /// <summary>
        /// 根据Name, 从配置文件读取，并创建Logger对象
        /// </summary>
        /// <param name="name">Logger的名称</param>
        /// <returns>读取的Logger对象</returns>
        /// <remarks>
        /// lang="cs" region="Create Logger Test" tittle="获取Logger对象"></code>
        /// </remarks>
        public static Logger Create(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "传递的Logger对象的名称为空");
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

            ExceptionHelper.FalseThrow(logelement != null, "未能找到名字为：{0}的Logger的配置节", name);
            return new Logger(logelement.Name, logelement.Enabled);
        }
    }
}
