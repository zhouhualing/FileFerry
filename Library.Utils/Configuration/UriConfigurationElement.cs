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
    /// 关于Uri的配置项
    /// </summary>
    public class UriConfigurationElement : NamedConfigurationElement
    {
        /// <summary>
        /// Uri的地址字符串
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
        /// 配置的Uri
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
    /// 关于Uri的配置项集合
    /// </summary>
    public class UriConfigurationCollection : NamedConfigurationElementCollection<UriConfigurationElement>
    {
    }

    internal sealed class UriContextCache : ContextCacheQueueBase<string, Uri>
    {
        /// <summary>
        /// 此处必须是属性，动态计算，不能是ReadOnly变量
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
