using System;
using System.Collections.Generic;
using System.Text;

using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// ��־��¼���ȼ�ö��
    /// </summary>
    /// <remarks>
    /// �����弶���ȼ�
    /// </remarks>
    public enum LogPriority
    {
        /// <summary>
        /// ������ȼ�
        /// </summary>
        Lowest,

        /// <summary>
        /// �����ȼ�
        /// </summary>
        BelowNormal,

        /// <summary>
        /// ��ͨ
        /// </summary>
        Normal,

        /// <summary>
        /// �����ȼ�
        /// </summary>
        AboveNormal,

        /// <summary>
        /// ������ȼ�
        /// </summary>
        Highest,
    }
}
