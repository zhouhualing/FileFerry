using System;
using System.Collections.Generic;
using System.Text;
namespace WD.Library.Data
{
    /// <summary>
    /// ָ���������¹��̵�ö��
    /// </summary>
    /// <remarks>
    ///     ���������������ӵ�ö��
    ///     
    /// </remarks>
    public enum UpdateBehavior
    {
        /// <summary>
        /// DataAdapter�ı�׼���̣�ִ��������Ϊֹ
        /// </summary>
        Standard,

        /// <summary>
        /// �������ִ�к�������
        /// </summary>
        Continue,

        /// <summary>
        /// ������Ϊһ�������ύ
        /// </summary>
        Transactional
    }

}
