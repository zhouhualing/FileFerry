using System;
using System.Collections.Generic;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// ������࣬ʵ��ILogFormatter�ӿ�
    /// </summary>
    /// <remarks>
    /// ����LogFormatter�Ļ��࣬
    /// ����ʱ��Ϊʹ���Ƶ�Formatter֧�ֿ����ã������ڸ���������ʵ�ֲ���ΪLogConfigurationElement����Ĺ��캯��
    /// </remarks>
    public abstract class LogFormatter : ILogFormatter
    {
        private string name = string.Empty;

        /// <summary>
        /// Formatter������
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        ///  ȱʡ���캯��
        /// </summary>
        public LogFormatter()
        {
        }

        /// <summary>
        /// ���캯��������Name����LogFormatter����
        /// </summary>
        /// <param name="formattername">Formatter������</param>
        /// <remarks>
        /// formattername��������Ϊ�գ������׳��쳣
        /// </remarks>
        public LogFormatter(string formattername)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(formattername, "Formatter�����ֲ���Ϊ��");

            this.name = formattername;
        }

        /// <summary>
        /// ���󷽷�����ʽ��LogEntity�����һ���ַ���
        /// </summary>
        /// <param name="log">����ʽ����LogEntity����</param>
        /// <returns>��ʽ���ɵ��ַ���</returns>
        /// <remarks>
        /// �����������ʵ��
        /// </remarks>
        public abstract string Format(LogEntity log);
    }
}
