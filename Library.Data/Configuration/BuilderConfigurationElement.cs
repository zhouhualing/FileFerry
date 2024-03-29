
using System;
using System.Configuration;
using WD.Library.Configuration;

namespace WD.Library.Data.Configuration
{
    class BuilderConfigurationElement : TypeConfigurationElement 
    {
        /// <summary>
        /// Builder������ConnectionString��Attribute����
        /// </summary>
        [ConfigurationProperty("attributeName")]
        public string AttributeName
        {
            get 
            {
                return (string)this["attributeName"]; 
            }
        }
    }

    [ConfigurationCollection(typeof(BuilderConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    class BuildersConfiguratonElementCollection : NamedConfigurationElementCollection<BuilderConfigurationElement> 
    {
    }
}
