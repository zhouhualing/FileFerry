using System;
using System.Web;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using WD.Library.Core;
using WD.Library.Caching;

namespace WD.Library.Configuration
{
    /// <summary>
    /// ����Uri��������
    /// </summary>
    public class UriConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// Uri�ĵ�ַ�ַ���
        /// </summary>
        [ConfigurationProperty("uri")]
        private string UriString
        {
            get
            {
                return (string)this["uri"];
            }
        }

        /// <summary>
        /// ���õ�Uri
        /// </summary>
        public Uri Uri
        {
            get
            {
                Uri result;

                if (UriContextCache.Instance.TryGetValue(UriString, out result) == false)
                {
                    result = UriHelper.ResolveUri(UriString);

                    UriContextCache.Instance.Add(UriString, result);
                }

                return result;
            }
        }
    }

    /// <summary>
    /// ����Uri���������
    /// </summary>
    public class UriConfigurationCollection : NamedConfigurationElementCollection<UriConfigurationElement>
    {
    }

    internal sealed class UriContextCache : ContextCacheQueueBase<string, Uri>
    {
        /// <summary>
        /// �˴����������ԣ���̬���㣬������ReadOnly����
        /// </summary>
        public static UriContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<UriContextCache>();
            }
        }

        private UriContextCache()
        {
        }
    }
}
