using System;
using System.Configuration;

namespace WD.Library.Data.Configuration
{
    /// <summary>
    /// �Զ������ݿ�ִ�й����¼�������������
    /// </summary>
    class DbEventArgsConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired=true)]
        public virtual string Type
        {
            get
            {
                return (string)this["type"];
            }
        }
    }
}
