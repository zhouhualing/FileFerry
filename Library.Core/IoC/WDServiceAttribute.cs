
using System;
using System.Collections.Generic;

namespace WD.Library.Core
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = false)]
    public class WDServiceAttribute : Attribute
    {
        public WDServiceAttribute()
        {
        }

        public WDServiceAttribute(string staticPropertyPath)
        {
            this.StaticPropertyPath = staticPropertyPath;
        }

        public string StaticPropertyPath { get; set; }

        public Type FallbackImplementation { get; set; }
    }

    public sealed class FallbackServiceProvider : IServiceProvider
    {
        Dictionary<Type, object> fallbackServiceDict = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            object instance;
            lock (fallbackServiceDict)
            {
                if (!fallbackServiceDict.TryGetValue(serviceType, out instance))
                {
                    var attrs = serviceType.GetCustomAttributes(typeof(WDServiceAttribute), false);
                    if (attrs.Length == 1)
                    {
                        var attr = (WDServiceAttribute)attrs[0];
                        if (attr.FallbackImplementation != null)
                        {
                            instance = Activator.CreateInstance(attr.FallbackImplementation);
                        }
                    }
                    fallbackServiceDict.Add(serviceType, instance);
                }
            }
            return instance;
        }
    }
}
