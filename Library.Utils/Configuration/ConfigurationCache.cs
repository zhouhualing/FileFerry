using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

using WD.Library.Caching;

namespace WD.Library.Configuration
{
	/// <summary>
	/// 用于存放 Configuration 的 Cache
	/// </summary>
	sealed class ConfigurationCache : PortableCacheQueue<string, System.Configuration.Configuration>
	{
        /// <summary>
        /// 获取实例
        /// </summary>
		private static readonly ConfigurationCache instance = CacheManager.GetInstance<ConfigurationCache>();

		/// <summary>
		/// 获取实例
		/// </summary>
		public static ConfigurationCache Instance
		{
			get
			{
				return ConfigurationCache.instance;
			}
		}

		private ConfigurationCache()
		{ 
			
		}
	} // class end

    /// <summary>
    /// 用于存放ConfigurationSection的Cache
    /// </summary>
	sealed class ConfigurationSectionCache : PortableCacheQueue<string, ConfigurationSection>
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        public static readonly ConfigurationSectionCache Instance = CacheManager.GetInstance<ConfigurationSectionCache>();

        private ConfigurationSectionCache()
        {
        }
    } // class end
} // namespace end
