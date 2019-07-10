using System.Data.Common;
using System.Data.OleDb;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    /// <summary>
    /// OracleClientFactory Factory Class
    /// </summary>
    internal class OleProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return OleDbFactory.Instance;
        }
    }
}
