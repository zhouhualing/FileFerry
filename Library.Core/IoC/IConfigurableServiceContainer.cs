using System;

namespace WD.Library.Core
{
    public interface IConfigurableServiceContainer: ICoreServiceContainer
    {
        string ConfigFile { get; set; }
    }
}
