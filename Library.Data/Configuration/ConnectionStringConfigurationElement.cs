using System;
using System.Configuration;
using WD.Library.Configuration;
using WD.Library.Core;
using WD.Library.Data.Properties;

namespace WD.Library.Data.Configuration
{
	/// <summary>
	/// ���Ӵ����������Ԫ��
	/// </summary>
	/// <remarks>����Builders���ڸýڵ㹹����Ϻ���У���˶Ը����ԵĴ�����ú���ش��õİ취</remarks>
	sealed class ConnectionStringConfigurationElement : ConnectionStringConfigurationElementBase
	{
	}

	[ConfigurationCollection(typeof(ConnectionStringConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	sealed class ConnectionStringConfigurationElementCollection : NamedConfigurationElementCollection<ConnectionStringConfigurationElement>
	{
	}
}