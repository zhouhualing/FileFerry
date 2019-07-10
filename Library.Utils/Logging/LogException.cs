using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Logging
{
    /// <summary>
    /// ��־�쳣��
    /// </summary>
    /// <remarks>
    /// ��־ϵͳ�Զ����쳣����
    /// </remarks>
	[Serializable]
	//˵����
	//		1��һ������Ȼʵ���� ISerializable�ӿڣ��������û������[Serializable]����Ȼ���ᱻ���л���
	//		2��.NET����ʱ�������κ������� [Serializable]���ԵĶ���������л���
	//			���ͨ��.NET��ܶ����ȱʡ���л������ܹ�ʹһ���౻���л�����ô�����Ķ���Ҳ�ض�����ȷ�����л���
	//			�������Ҫ�Զ������л������������ͱ���ʵ��ISerializable�ӿڣ�ͬʱ����������[Serializable]���ԡ�
    public sealed class LogException : Exception
    {
        public LogException()
            : base()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="message">�쳣��Ϣ</param>
        /// <remarks>
        /// �����쳣��Ϣ��������־�쳣��
        /// </remarks>
        public LogException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// ���صĹ��캯��
        /// </summary>
        /// <param name="message">�쳣��Ϣ</param>
        /// <param name="exception">ԭʼ�쳣����</param>
        /// <remarks>
        /// ��ԭʼ�쳣��ת���LogException
        /// lang="cs" region="Process Log" title="д��־���쳣����"></code>
        /// </remarks>
        public LogException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
