
using System;
using System.Collections.Concurrent;
using System.ComponentModel.Design;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WD.Library.Utils;

namespace WD.Library.Core
{
    /// <summary>
    /// A thread-safe service container class.
    /// </summary>
    public sealed class CoreServiceContainer : IServiceProvider, ICoreServiceContainer,IDisposable
    {
        readonly ConcurrentStack<IServiceProvider> fallbackProviders = new ConcurrentStack<IServiceProvider>();
        readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        readonly Dictionary<string, object> items = new Dictionary<string, object>();
        readonly List<Type> servicesToDispose = new List<Type>();
        readonly List<string> itemsToDispose = new List<string>();
        readonly Dictionary<Type, object> taskCompletionSources = new Dictionary<Type, object>(); 

        private static CoreServiceContainer _default;

        /// <summary>
        /// This class' default instance.
        /// </summary>
        public static CoreServiceContainer Default
        {
            get
            {
                return _default ?? (_default = new CoreServiceContainer());
            }
        }

        public CoreServiceContainer()
        {
            Logger.Debug("CoreServiceContainer start.");

            services.Add(typeof(IServiceContainer), this);
            services.Add(typeof(ICoreServiceContainer), this);
        }

        public void AddFallbackProvider(IServiceProvider provider)
        {
            this.fallbackProviders.Push(provider);
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            object instance;
            lock (services)
            {
                if (services.TryGetValue(serviceType, out instance))
                {
                    ServiceCreatorCallback callback = instance as ServiceCreatorCallback;
                    if (callback != null)
                    {
                        instance = callback(this, serviceType);
                        if (instance != null)
                        {
                            services[serviceType] = instance;
                            OnServiceInitialized(serviceType, instance);
                        }
                        else
                        {
                            services.Remove(serviceType);
                        }
                    }
                }
            }
            if (instance != null)
                return instance;

            foreach (var fallbackProvider in fallbackProviders)
            {
                instance = fallbackProvider.GetService(serviceType);
                if (instance != null)
                    return instance;
            }
            return null;
        }

        public object GetService(string name)
        {
            object instance;
            lock (items)
            {
                if (items.TryGetValue(name, out instance))
                {
                    if (instance != null)
                        return instance;
                }
            }
            return null;
        }


        public void Dispose()
        {
            Type[] disposableTypes;
            lock (services)
            {
                disposableTypes = servicesToDispose.ToArray();
                //services.Clear();
                servicesToDispose.Clear();
            }
            // dispose services in reverse order of their creation
            for (int i = disposableTypes.Length - 1; i >= 0; i--)
            {
                IDisposable disposable = null;
                lock (services)
                {
                    object serviceInstance;
                    if (services.TryGetValue(disposableTypes[i], out serviceInstance))
                    {
                        disposable = serviceInstance as IDisposable;
                        if (disposable != null)
                            services.Remove(disposableTypes[i]);
                    }
                }
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            string[] disposableItems;
            lock (items)
            {
                disposableItems = itemsToDispose.ToArray();
                itemsToDispose.Clear();
            }
            for (int i = disposableItems.Length - 1; i >= 0; i--)
            {
                IDisposable disposable = null;
                lock (items)
                {
                    object objInstance;
                    if (items.TryGetValue(disposableItems[i], out objInstance))
                    {
                        disposable = objInstance as IDisposable;
                        if (disposable != null)
                            items.Remove(disposableItems[i]);
                    }
                }
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        void OnServiceInitialized(Type serviceType, object serviceInstance)
        {
            IDisposable disposableService = serviceInstance as IDisposable;
            if (disposableService != null)
                servicesToDispose.Add(serviceType);

            dynamic taskCompletionSource;
            if (taskCompletionSources.TryGetValue(serviceType, out taskCompletionSource))
            {
                taskCompletionSources.Remove(serviceType);
                taskCompletionSource.SetResult((dynamic)serviceInstance);
            }
        }

        public void AddService(string name,object serviceInstance)
        {
            lock (items)
            {
                items.Add(name, serviceInstance);
            }
        }

        public void AddService<T>(object serviceInstance)
        {
            AddService(typeof(T), serviceInstance);
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            lock (services)
            {
                services.Add(serviceType, serviceInstance);
                OnServiceInitialized(serviceType, serviceInstance);
            }
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            AddService(serviceType, serviceInstance);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            lock (services)
            {
                services.Add(serviceType, callback);
            }
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            AddService(serviceType, callback);
        }

        public void RemoveService(Type serviceType)
        {
            lock (services)
            {
                object instance;
                if (services.TryGetValue(serviceType, out instance))
                {
                    services.Remove(serviceType);
                    IDisposable disposableInstance = instance as IDisposable;
                    if (disposableInstance != null)
                        servicesToDispose.Remove(serviceType);
                }
            }
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            RemoveService(serviceType);
        }

        public void RemoveService(string serviceName)
        {
            lock (items)
            {
                object instance;
                if (items.TryGetValue(serviceName, out instance))
                {
                    items.Remove(serviceName);
                    IDisposable disposableInstance = instance as IDisposable;
                    if (disposableInstance != null)
                        itemsToDispose.Remove(serviceName);
                }
            }
        }

        public Task<T> GetFutureService<T>()
        {
            Type serviceType = typeof(T);
            lock (services)
            {
                if (services.ContainsKey(serviceType))
                {
                    return Task.FromResult((T)GetService(serviceType));
                }
                else
                {
                    object taskCompletionSource;
                    if (taskCompletionSources.TryGetValue(serviceType, out taskCompletionSource))
                    {
                        return ((TaskCompletionSource<T>)taskCompletionSource).Task;
                    }
                    else
                    {
                        var tcs = new TaskCompletionSource<T>();
                        taskCompletionSources.Add(serviceType, tcs);
                        return tcs.Task;
                    }
                }
            }
        }

        public T Resolve<T>()
        {
            return GetService<T>();
        }


        public T Resolve<T>(string name) where T:class
        {
            var svc = GetService(name);
            if (svc == null || !(svc is T))
                return default(T);

            return svc as T;
        }

        public bool IsRegistered<T>()
        {
            return GetService<T>() != null;
        }

        public bool IsRegistered(string name)
        {
            return GetService(name) != null;
        }

        public bool IsRegistered(Type typeToCheck)
        {
            return GetService(typeToCheck) != null; 
        }

        public ICoreServiceContainer RegisterInstance(Type t, object instance)
        {
            AddService(t, instance);
            return this;
        }
        public ICoreServiceContainer RegisterInstance<T>(object instance)
        {
            AddService(typeof(T), instance);
            return this;
        }

        public ICoreServiceContainer RegisterInstance(string serviceName,object instance)
        {
            AddService(serviceName, instance);
            return this;
        }
    }
}
