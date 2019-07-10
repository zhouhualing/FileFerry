using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
namespace Framework.WorkItem
{
    public class WorkItemContainer : PrimaryController
    {
        private ConcurrentDictionary<object,object> concurrentDictionary = new ConcurrentDictionary<object, object>();

        public void Register(object key,object val)
        {
            concurrentDictionary.TryAdd(key, val);
        }
        public void Register<T>(object key, T val)
        {
            concurrentDictionary.TryAdd(key, val);
        }
        public object Get(object key)
        {
            object obj = null;
            var result = concurrentDictionary.TryGetValue(key, out obj);
            if (result)
                return obj;
            return null;
        }

        public T Get<T>(object key)
        {
            object obj = null;
            var result = concurrentDictionary.TryGetValue(key, out obj);
            if (result)
                return (T)obj;
            return default(T);
        }

        private static object synRoot = new object();
        public static Control uiObject;
        public WorkItemContainer()
        {
            instance = this;
        }
        private static WorkItemContainer instance;
        public static WorkItemContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (synRoot)
                    {
                        if (instance == null)
                        {
                            instance = new WorkItemContainer();
                        }
                    }
                }
                return instance;
            }
        }

        public static Control UIObject
        {
            get
            {
                return uiObject;
            }
            set
            {
                if (uiObject != value)
                {
                    uiObject = value;
                }
            }
        }
    }
}
