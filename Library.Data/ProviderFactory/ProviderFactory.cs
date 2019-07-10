using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace ChinaCustoms.Framework.DeluxeWorks.Library.Data
{
    abstract internal class ProviderFactory
    {
        public abstract DbProviderFactory Create();
    }
}
