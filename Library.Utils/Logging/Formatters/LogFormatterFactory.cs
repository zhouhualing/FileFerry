using System;
using System.Collections.Generic;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Logging
{
    internal class LogFormatterFactory
    {
        public static ILogFormatter GetFormatter(LogConfigurationElement formatterElement)
        {
            if (formatterElement != null)
            {
                try
                {
                    return (ILogFormatter)TypeCreator.CreateInstance(formatterElement.Type, formatterElement);
                }
                catch (Exception ex)
                {
                    throw new LogException("创建Formatter对象时出错：" + ex.Message, ex);
                }
            }
            else
                return null;
        }
    }
}
