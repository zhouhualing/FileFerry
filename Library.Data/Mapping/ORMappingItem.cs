using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using WD.Library.Core;

namespace WD.Library.Data.Mapping
{
    /// <summary>
    /// ӳ���ϵ��
    /// </summary>
    /// <remarks>
    /// ��������������ʵ���������ݿ��ֶμ����ӳ��ʱ�Ĺ�ϵ
    /// </remarks>
    public class ORMappingItem
    {
        private string propertyName = string.Empty;
        private string subClassPropertyName = string.Empty;
        private string dataFieldName = string.Empty;
        private bool isIdentity = false;
        private bool primaryKey = false;
        private int length = 0;
        private bool isNullable = true;
        private string subClassTypeDescription = string.Empty;
        private ClauseBindingFlags bindingFlags = ClauseBindingFlags.All;
        private string defaultExpression = string.Empty;

        private MemberInfo memberInfo = null;
        private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;

        /// <summary>
        /// ���췽��
        /// </summary>
        public ORMappingItem()
        {
        }

        /// <summary>
        /// д�뵽XmlWriter
        /// </summary>
        /// <param name="writer">XML��д��</param>
        public void WriteToXml(XmlWriter writer)
        {
            XmlHelper.AppendNotNullAttr(writer, "propertyName", this.propertyName);
            XmlHelper.AppendNotNullAttr(writer, "dataFieldName", this.dataFieldName);

            if (this.isIdentity)
                XmlHelper.AppendNotNullAttr(writer, "isIdentity", this.isIdentity);

            if (this.primaryKey)
                XmlHelper.AppendNotNullAttr(writer, "primaryKey", this.primaryKey);

            if (this.length != 0)
                XmlHelper.AppendNotNullAttr(writer, "length", this.length);

            if (this.isNullable == false)
                XmlHelper.AppendNotNullAttr(writer, "isNullable", this.isNullable);

            if (this.bindingFlags != ClauseBindingFlags.All)
                XmlHelper.AppendNotNullAttr(writer, "bindingFlags", this.bindingFlags.ToString());

            XmlHelper.AppendNotNullAttr(writer, "defaultExpression", this.defaultExpression);

            if (this.enumUsage != EnumUsageTypes.UseEnumValue)
                XmlHelper.AppendNotNullAttr(writer, "enumUsage", this.enumUsage.ToString());

            XmlHelper.AppendNotNullAttr(writer, "subClassPropertyName", this.subClassPropertyName);
            XmlHelper.AppendNotNullAttr(writer, "subClassTypeDescription", this.subClassTypeDescription);
        }

        /// <summary>
        /// ��XmlReader����������
        /// </summary>
        /// <param name="reader">Xml�Ķ���</param>
        /// <param name="mi">��Ա����</param>
        public void ReadFromXml(XmlReader reader, MemberInfo mi)
        {
            this.memberInfo = mi;

            this.propertyName = XmlHelper.GetAttributeValue(reader, "propertyName", this.propertyName);
            this.dataFieldName = XmlHelper.GetAttributeValue(reader, "dataFieldName", this.dataFieldName);

            this.isIdentity = XmlHelper.GetAttributeValue(reader, "isIdentity", this.isIdentity);
            this.primaryKey = XmlHelper.GetAttributeValue(reader, "primaryKey", this.primaryKey);
            this.length = XmlHelper.GetAttributeValue(reader, "length", this.length);

            this.isNullable = XmlHelper.GetAttributeValue(reader, "isNullable", this.isNullable);
            this.bindingFlags = XmlHelper.GetAttributeValue(reader, "bindingFlags", this.bindingFlags);

            this.defaultExpression = XmlHelper.GetAttributeValue(reader, "defaultExpression", this.defaultExpression);

            this.enumUsage = XmlHelper.GetAttributeValue(reader, "enumUsage", this.enumUsage);
            this.subClassPropertyName = XmlHelper.GetAttributeValue(reader, "subClassPropertyName", this.subClassPropertyName);
            this.subClassTypeDescription = XmlHelper.GetAttributeValue(reader, "subClassTypeDescription", this.subClassTypeDescription);
        }

        /// <summary>
        /// Enum��ֵ��������
        /// </summary>
        public EnumUsageTypes EnumUsage
        {
            get { return this.enumUsage; }
            set { this.enumUsage = value; }
        }

        /// <summary>
        /// ��Ӧ������Ϊ��ʱ�����ṩ��ȱʡֵ���ʽ
        /// </summary>
        public string DefaultExpression
        {
            get
            {
                return this.defaultExpression;
            }
            set
            {
                this.defaultExpression = value;
            }
        }

        /// <summary>
        /// ��Ӧ������ֵ���������ЩSql�����
        /// </summary>
        public ClauseBindingFlags BindingFlags
        {
            get { return this.bindingFlags; }
            set { this.bindingFlags = value; }
        }

        /// <summary>
        /// �ֶ��Ƿ�Ϊ��
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
            set { this.isNullable = value; }
        }

        /// <summary>
        /// �ֶγ���
        /// </summary>
        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public string PropertyName
        {
            get { return this.propertyName; }
            set { this.propertyName = value; }
        }

        /// <summary>
        /// ������Ӷ��󣬶�Ӧ���Ӷ������Ե�����
        /// </summary>
        public string SubClassPropertyName
        {
            get { return this.subClassPropertyName; }
            set { this.subClassPropertyName = value; }
        }

        /// <summary>
        /// ��Ӧ�����ݿ��ֶ���
        /// </summary>
        public string DataFieldName
        {
            get { return this.dataFieldName; }
            set { this.dataFieldName = value; }
        }

        /// <summary>
        /// �Ƿ��ʶ��
        /// </summary>
        public bool IsIdentity
        {
            get { return this.isIdentity; }
            set { this.isIdentity = value; }
        }

        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool PrimaryKey
        {
            get { return this.primaryKey; }
            set { this.primaryKey = value; }
        }

        /// <summary>
        /// MemberInfo��
        /// </summary>
        /// <remarks>
        /// Obtains information about the attributes of a member and provides access to member metadata. 
        /// </remarks>
        public MemberInfo MemberInfo
        {
            get { return this.memberInfo; }
            internal set { this.memberInfo = value; }
        }

        /// <summary>
        /// �Ӷ������������
        /// </summary>
        public string SubClassTypeDescription
        {
            get { return subClassTypeDescription; }
            internal set { this.subClassTypeDescription = value; }
        }

		/// <summary>
		/// ����һ��MappingItem
		/// </summary>
		/// <returns></returns>
		public ORMappingItem Clone()
		{
			ORMappingItem newItem = new ORMappingItem();

			newItem.dataFieldName = this.dataFieldName;
			newItem.propertyName = this.propertyName;
			newItem.subClassPropertyName = this.subClassPropertyName;
			newItem.isIdentity = this.isIdentity;
			newItem.primaryKey = this.primaryKey;
			newItem.length = this.length;
			newItem.isNullable = this.isNullable;
			newItem.subClassTypeDescription = this.subClassTypeDescription;
			newItem.bindingFlags = this.bindingFlags;
			newItem.defaultExpression = this.defaultExpression;
			newItem.memberInfo = this.memberInfo;
			newItem.enumUsage = this.enumUsage;

			return newItem;
		}
    }
}
