using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using WD.Library.Core;

namespace WD.Library.Configuration
{
    /// <summary>
    /// ������Ϣ��������
    /// </summary>
    public class TypeConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// ����������Ϣ
        /// </summary>
        /// <remarks>һ�����QualifiedName ��QuanlifiedTypeName, AssemblyName����ʽ</remarks>
        [ConfigurationProperty("type", IsRequired = true)]
        public virtual string Type
        {
            get
            {
                return (string)this["type"];
            }
        }

        /// <summary>
        /// ���������ʵ��
        /// </summary>
        /// <param name="ctorParams">����ʵ���ĳ�ʼ������</param>
        /// <returns>������󶨷�ʽ��̬����һ��ʵ��</returns>
        public object CreateInstance(params object[] ctorParams)
        {
            return TypeCreator.CreateInstance(Type, ctorParams);
        }
    }

    /// <summary>
    /// ���͵�����Ԫ�ؼ���
    /// </summary>
    public class TypeConfigurationCollection : NamedConfigurationElementCollection<TypeConfigurationElement>
    {
    }
}
