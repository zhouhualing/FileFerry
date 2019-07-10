using System.Data.Common;
using System.Data.SqlClient;


namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    /// <summary>
    /// SqlClientFactory Factory Class
    /// </summary>
    internal class SqlProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return SqlClientFactory.Instance;
        }
    }
}
