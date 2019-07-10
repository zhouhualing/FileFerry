using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

using WD.Library.Caching;

namespace WD.Library.Configuration
{
	/// <summary>
	/// ���ڴ�� Configuration �� Cache
	/// </summary>
	sealed class ConfigurationCache : PortableCacheQueue<string, System.Configuration.Configuration>
	{
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
		private static readonly ConfigurationCache instance = CacheManager.GetInstance<ConfigurationCache>();

		/// <summary>
		/// ��ȡʵ��
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
    /// ���ڴ��ConfigurationSection��Cache
    /// </summary>
	sealed class ConfigurationSectionCache : PortableCacheQueue<string, ConfigurationSection>
    {
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        public static readonly ConfigurationSectionCache Instance = CacheManager.GetInstance<ConfigurationSectionCache>();

        private ConfigurationSectionCache()
        {
        }
    } // class end
} // namespace end
