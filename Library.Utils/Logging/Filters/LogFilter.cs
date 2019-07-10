using System;
using System.Collections.Generic;
using System.Text;

using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// �����࣬ʵ��ILogFilter�ӿ�
    /// </summary>
    /// <remarks>
    /// ����LogFilter�Ļ���
    /// ����ʱ��Ϊʹ���Ƶ�Filter֧�ֿ����ã������ڸ���������ʵ�ֲ���ΪLogConfigurationElement����Ĺ��캯��
    /// </remarks>
    public abstract class LogFilter : ILogFilter
    {
        private string name = string.Empty;

        /// <summary>
        /// Filter����
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public LogFilter()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
		/// <param name="filterName">Filter����</param>
        /// <remarks>
        /// name��������Ϊ�գ������׳��쳣
        /// </remarks>
        public LogFilter(string filterName)
        {
			ExceptionHelper.CheckStringIsNullOrEmpty(filterName, "Filter�����Ʋ���Ϊ��");

			this.name = filterName;
        }

        /// <summary>
        /// ���󷽷���ʵ����־����
        /// </summary>
        /// <param name="log">��־����</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        public abstract bool IsMatch(LogEntity log);
    }
}
