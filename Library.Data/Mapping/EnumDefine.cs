using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Data.Mapping
{
    #region Enumeration
    /// <summary>
    /// Ϊ�����󶨱��
    /// </summary>
    [Flags]
    public enum ClauseBindingFlags
    {
        /// <summary>
        /// �κ�����¶�������
        /// </summary>
        None = 0,

        /// <summary>
        /// ��ʾ���Ի������Insert��
        /// </summary>
        Insert = 1,

        /// <summary>
        /// ��ʾ���Ի������Update��
        /// </summary>
        Update = 2,

        /// <summary>
        /// ��ʾ���Ի������Where��䲿��
        /// </summary>
        Where = 4,

        /// <summary>
        /// ��ʾ���Ի�����ڲ�ѯ�ķ����ֶ���
        /// </summary>
        Select = 8,

        /// <summary>
        /// ����������¶������
        /// </summary>
        All = 15,
    }

    /// <summary>
    /// ö�����͵�ʹ�÷���
    /// </summary>
    public enum EnumUsageTypes
    {
        /// <summary>
        /// ʹ��ö�����͵�ֵ(����)
        /// </summary>
        UseEnumValue = 0,

        /// <summary>
        /// ʹ��ö�����͵��ַ���
        /// </summary>
        UseEnumString
    }

    #endregion Enumeration
}