using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace WD.Library.Core
{
	/// <summary>
	/// ��������������UUID����
	/// </summary>
	public static class UuidHelper
	{
		[DllImport("rpcrt4.dll", SetLastError = true)]
		private static extern int UuidCreateSequential(out Guid guid);

		/// <summary>
		/// ����������UUID���ײ������Windows API UuidCreateSequential�������Է��֣�
		/// UuidCreateSequential���ڶ�CPU����״̬�£��п��ܻ�����ظ����ݣ��������������еĲ������ƣ����ӳ�1���롣
		/// ���⣬UuidCreateSequential�����ɺ����������йأ�����������������ϲ���Windows Mobile���ֻ���������µ��������ӣ�
		/// ����UuidCreateSequential������ʱ�����������ʹ�ô�ͳ��Guid�����Uuid��
		/// </summary>
		/// <returns>�ڱ�������������Guid</returns>
		public static Guid NewUuid()
		{
			Guid result;

			lock (typeof(UuidHelper))
			{
                try
                {

                    int hr = UuidCreateSequential(out result);

                    if (hr == 0)
                        result = Guid.NewGuid();
                }
                catch (Exception)
                {
                    return Guid.NewGuid();
                }

				Thread.Sleep(1);
			}

			return result;
		}

		/// <summary>
		/// ����������UUID���ײ������Windows API UuidCreateSequential
		/// </summary>
		/// <returns>�ڱ�������������Guid</returns>
		public static string NewUuidString()
		{
			Guid result = NewUuid();

			byte[] guidBytes = result.ToByteArray();

			for (int i = 0; i < 8; i++)
			{
				byte t = guidBytes[15 - i];
				guidBytes[15 - i] = guidBytes[i];
				guidBytes[i] = t;
			}

			return new Guid(guidBytes).ToString();
		}
	}
}
