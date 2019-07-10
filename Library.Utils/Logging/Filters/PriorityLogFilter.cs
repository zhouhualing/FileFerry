using System;
using System.Collections.Generic;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// ���ȼ�������
    /// </summary>
    /// <remarks>
    /// LogFilter�������࣬ʵ�ָ������ȼ�������־��¼
    /// </remarks>
    public sealed class PriorityLogFilter : LogFilter
    {
        private LogPriority minPriority = LogPriority.Normal;

        private PriorityLogFilter()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <remarks>
        /// </remarks>
        public PriorityLogFilter(string name) : base(name)
        {
            
        }

        /// <summary>
        /// ���صĹ��캯��
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="minPriority">���ȼ���ֵ</param>
        /// <remarks>
        /// </remarks>
        public PriorityLogFilter(string name, LogPriority minPriority)
            : base(name)
        {
            this.minPriority = minPriority;
        }

        /// <summary>
        /// ���صĹ��캯�����������ļ��ж�ȡ������
        /// </summary>
        /// <param name="element">���ö���</param>
        /// <remarks>
        /// </remarks>
        public PriorityLogFilter(LogConfigurationElement element)
            : base(element.Name)
        {
            string strMin;

            if (element.ExtendedAttributes.TryGetValue("minPriority", out strMin) == false)
                strMin = LogPriority.Normal.ToString();
                
            //ExceptionHelper.TrueThrow<LogException>(string.IsNullOrEmpty(strminpriority), "û���������ļ����ҵ�minPriority����");

            this.minPriority = (LogPriority)Enum.Parse(typeof(LogPriority), strMin);
        }

        /// <summary>
        /// ���ȼ���ֵ
        /// </summary>
        public LogPriority MinPriority
        {
            get
            {
                return this.minPriority;
            }
        }

        /// <summary>
        /// ��д�ķ���������ʵ�����ȼ�����
        /// </summary>
        /// <param name="log">��־��¼</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        /// <remarks>
        /// ֻ�����ȼ����ڵ���minPriority����־��¼����ͨ��
        /// </remarks>
        public override bool IsMatch(LogEntity log)
        {
            return log.Priority >= this.minPriority;
        }
    }
}
