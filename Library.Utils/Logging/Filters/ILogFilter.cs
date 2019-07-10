using System;
using System.Collections.Generic;
using System.Text;

using WD.Library.Core;
using WD.Library.Accessories;

namespace WD.Library.Logging
{
    /// <summary>
    /// �ӿڣ�������־������
    /// </summary>
#if DELUXEWORKSTEST
    public interface ILogFilter : IFilter<LogEntity>
#else
    internal interface ILogFilter : IFilter<LogEntity>
#endif
    {
        /// <summary>
        /// ����
        /// </summary>
        new string Name
        {
            get;
            set;
        }

        /// <summary>
        /// �ӿڷ�����ʵ����־����
        /// </summary>
        /// <param name="log">��־����</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        new bool IsMatch(LogEntity log);
    }
}
