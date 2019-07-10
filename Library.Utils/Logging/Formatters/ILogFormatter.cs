using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Logging
{
    /// <summary>
    /// �ӿڣ���ʽ����־��¼
    /// </summary>
    /// <remarks>
    /// ���彫��־��¼LogEntity�����ʽ�����ַ����ĸ�ʽ������ͨ��ʵ�ָýӿ���ʵ�ֶ��Ƶĸ�ʽ�������磺�ı���XML��
    /// </remarks>
    public interface ILogFormatter
    {
        /// <summary>
        /// ����
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// �ӿڷ�������ʽ��LogEntity�����һ���ַ���
        /// </summary>
        /// <param name="log">LogEntity����</param>
        /// <returns>��ʽ���ɵ��ַ���</returns>
        string Format(LogEntity log);
    }
}
