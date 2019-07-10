using System.Data.Common;
using System.Data.OracleClient;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    /// <summary>
    /// OracleClientFactory Factory Class
    /// </summary>
    internal class OracleProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return OracleClientFactory.Instance;
        }
    }
}
