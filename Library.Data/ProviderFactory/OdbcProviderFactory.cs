using System.Data.Common;
using System.Data.Odbc;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{   
    /// <summary>
    /// OdbcFactory Factory Calass
    /// </summary>
    internal class OdbcProviderFactory : ProviderFactory
    {
        public override DbProviderFactory Create()
        {
            return OdbcFactory.Instance;
        }
    }
}
