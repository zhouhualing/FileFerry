using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// �ӿڣ�����CacheQueueͨ��ʵ�ִ˽ӿ���������������
    /// </summary>
    public interface IScavenge
    {
        /// <summary>
        /// Cache����������
        /// </summary>
        void DoScavenging();
    }
}
