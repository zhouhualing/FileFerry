
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace WD.Library.Core
{
    public interface ICoreServiceContainer : IServiceContainer
    {
        T Resolve<T>();
        T Resolve<T>(string name) where T : class;
        bool IsRegistered<T>();
        bool IsRegistered(Type typeToCheck);
        bool IsRegistered(string name);
        ICoreServiceContainer RegisterInstance(Type t, object instance);
        ICoreServiceContainer RegisterInstance<T>(object instance);
        ICoreServiceContainer RegisterInstance(string name, object instance);
    }
}