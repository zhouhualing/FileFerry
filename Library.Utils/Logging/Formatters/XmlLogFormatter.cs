using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using WD.Library.Properties;

namespace WD.Library.Logging
{
    /// <summary>
    /// Xml��ʽ����
    /// </summary>
    /// <remarks>
    /// LogFormatter�������࣬����ʵ��Xml��ʽ��
    /// </remarks>
    public sealed class XmlLogFormatter : LogFormatter
    {
        private const string DefaultValue = "";

        private XmlLogFormatter()
        {
        }
        
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="name">Formatter����</param>
        /// <remarks>
        /// �������ƣ�����XmlLogFormatter����
        /// </remarks>
        public XmlLogFormatter(string name)
            : base(name)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="element">���ýڶ���</param>
        /// <remarks>
        /// ����������Ϣ������TextLogFormatter����
        /// </remarks>
        public XmlLogFormatter(LogConfigurationElement element)
            : base(element.Name)
        {
        }

        /// <summary>
        /// ��LogEntity�����ʽ����XML��
        /// </summary>
        /// <param name="log">LogEntity����</param>
        /// <returns>��ʽ���õ�XML��</returns>
        /// <remarks>
        /// ���ط�����ʵ�ָ�ʽ��
        /// </remarks>
        public override string Format(LogEntity log)
        {
            StringBuilder result = new StringBuilder();
            Format(log, result);
            return result.ToString();
        }

        private void Format(object obj, StringBuilder result)
        {
            if (obj == null) 
                return;

            result.Append(CreateOpenElement(CrateRootName(obj)));

            if (Type.GetTypeCode(obj.GetType()) == TypeCode.Object)
            {
                foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
                {
                    result.Append(CreateOpenElement(propertyInfo.Name));

                    if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && Type.GetTypeCode(propertyInfo.PropertyType) == TypeCode.Object)
                    {
                        IEnumerable values = (IEnumerable)propertyInfo.GetValue(obj, null);
                        if (values != null)
                        {
                            foreach (object value in values)
                            {
                                Format(value, result);
                            }
                        }
                    }
                    else
                    {
                        result.Append(ConvertToString(propertyInfo, obj));
                    }

                    result.Append(CreateCloseElement(propertyInfo.Name));
                }
            }
            else
            {
                result.Append(obj.ToString());
            }
            result.Append(CreateCloseElement(CrateRootName(obj)));
        }

        private static string CrateRootName(object obj)
        {
            return obj.GetType().Name;
        }

        private static string CreateOpenElement(string name)
        {
            return string.Format(Resource.Culture, "<{0}>", name);
        }
        private static string CreateCloseElement(string name)
        {
            return string.Format(Resource.Culture, "</{0}>", name);
        }

        private static string ConvertToString(PropertyInfo propertyInfo, object obj)
        {
            object value = propertyInfo.GetValue(obj, null);
            string result = XmlLogFormatter.DefaultValue;

            if (value != null)
            {
                if (value is DateTime)
                    result = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
                else
                    result = value.ToString();
            }

            return result;
        }
    }
}
