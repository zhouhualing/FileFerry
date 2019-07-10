using System;
using System.Text;
using System.Collections.Generic;
using WD.Library.Caching;

namespace WD.Library.Core
{
	internal sealed class XmlMappingsCache : CacheQueue<System.Type, XmlObjectMappingItemCollection>
	{
		public static readonly XmlMappingsCache Instance = CacheManager.GetInstance<XmlMappingsCache>();

		private XmlMappingsCache()
		{
		}
	}
}
