using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Data.Mapping
{
    /// <summary>
    /// ORMӳ������
    /// </summary>
    /// <remarks>
    /// ORMӳ���������
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ORFieldMappingAttribute : System.Attribute
    {
        private string dataFieldName = string.Empty;
        private bool isIdentity = false;
        private bool primaryKey = false;
        private int length = 0;
        private bool isNullable = true;

        /// <summary>
        /// ���췽��
        /// </summary>
        protected ORFieldMappingAttribute()
        {
        }

        /// <summary>
        /// ȡ�ֶζ�Ӧ��ֵ
        /// </summary>
        /// <param name="fieldName">�ֶ�</param>
        /// <remarks>
        /// 
        /// </remarks>
        public ORFieldMappingAttribute(string fieldName)
        {
            this.dataFieldName = fieldName;
        }

        /// <summary>
        /// ȡ�ֶζ�Ӧ��ֵ
        /// </summary>
        /// <param name="fieldName">�ֶ�</param>
        /// <param name="nullable">�Ƿ�Ϊ��</param>
        /// <remarks>
        /// 
        /// </remarks>
        public ORFieldMappingAttribute(string fieldName, bool nullable)
            : this(fieldName)
        {
            this.isNullable = nullable;
        }

        /// <summary>
        /// �ֶ��Ƿ��Ϊ��
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
            set { this.isNullable = value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        /// <summary>
        /// �ֶ���
        /// </summary>
        public string DataFieldName
        {
            get { return this.dataFieldName; }
            set { this.dataFieldName = value; }
        }

        /// <summary>
        /// �Ƿ��ʶ��
        /// </summary>
        /// <remarks>
        /// �Ƿ��ʶ�У��Ƿ���TRUE���񷵻�FALSE
        /// </remarks>
        public bool IsIdentity
        {
            get { return this.isIdentity; }
            set { this.isIdentity = value; }
        }

        /// <summary>
        /// �Ƿ�����
        /// </summary>
        /// <remarks>
        /// �Ƿ��������Ƿ���TRUE���񷵻�FALSE
        /// </remarks>
        public bool PrimaryKey
        {
            get { return this.primaryKey; }
            set { this.primaryKey = value; }
        }
    }

    /// <summary>
    /// ����Mappingʱ���Ե�����
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class NoMappingAttribute : System.Attribute
    {
    }
}
