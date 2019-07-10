
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Configuration;
using WD.Library.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// LogConfigurationElement集合类
    /// </summary>
    /// <remarks>
    /// TypeConfigurationCollection的派生类
    /// </remarks>
    public class LogConfigurationElementCollection : TypeConfigurationCollection
    {
        /// <summary>
        /// 配置节所在的路径
        /// </summary>
        private string fullPath = string.Empty;
        
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        protected internal LogConfigurationElementCollection()
        {
        }

        internal LogConfigurationElementCollection(string fullPath)
        {
            this.fullPath = fullPath;
        }

        /// <summary>
        /// 创建新的LogConfigurationElement对象
        /// </summary>
        /// <returns>LogConfigurationElement对象</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LogConfigurationElement(this.fullPath);
        }

        /// <summary>
        /// 根据键值索引，返回LogConfigurationElement对象
        /// </summary>
        /// <param name="name">LogConfigurationElement名称</param>
        /// <returns>LogConfigurationElement对象</returns>
        /// <remarks>
        /// </code>
        /// </remarks>
        public new LogConfigurationElement this[string name]
        {
            get
            {
                return (LogConfigurationElement)InnerGet(name);
            }
        }

        internal void DeserializeElementDirectly(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            this.DeserializeElement(reader, serializeCollectionKey);
        }
    }
    
    
    
    //public class LogConfigurationElementCollectionBase<T> : ConfigurationElementCollection, IEnumerable<T>
    //    where T : LogConfigurationElementBase, new()
    //{
    //    public void ForEach(Action<T> action)
    //    {
    //        for (int index = 0; index < Count; index++)
    //        {
    //            action(Get(index));
    //        }
    //    }

    //    public T Get(int index)
    //    {
    //        return (T)base.BaseGet(index);
    //    }

    //    public T Get(string name)
    //    {
    //        return BaseGet(name) as T;
    //    }

    //    public void Add(T element)
    //    {
    //        BaseAdd(element, true);
    //    }

    //    public bool Contains(string name)
    //    {
    //        return BaseGet(name) != null;
    //    }

    //    public void Remove(string name)
    //    {
    //        BaseRemove(name);
    //    }

    //    public void Clear()
    //    {
    //        BaseClear();
    //    }

    //    public new IEnumerator<T> GetEnumerator()
    //    {
    //        return new GenericEnumeratorWrapper<T>(base.GetEnumerator());
    //    }

    //    protected override ConfigurationElement CreateNewElement()
    //    {
    //        T element = new T();
    //        Type elementType = Type.GetType(element.ElementType);

    //        if (elementType != null)
    //            return Activator.CreateInstance(elementType) as ConfigurationElement;
    //        else
    //            return new T();
    //    }

    //    protected override object GetElementKey(ConfigurationElement element)
    //    {
    //        T namedElement = (T)element;
    //        return namedElement.Name;
    //    }
    //}

    //internal class GenericEnumeratorWrapper<T> : IEnumerator<T>
    //{
    //    private IEnumerator wrappedEnumerator;

    //    internal GenericEnumeratorWrapper(IEnumerator wrappedEnumerator)
    //    {
    //        this.wrappedEnumerator = wrappedEnumerator;
    //    }

    //    T IEnumerator<T>.Current
    //    {
    //        get { return (T)this.wrappedEnumerator.Current; }
    //    }

    //    void IDisposable.Dispose()
    //    {
    //        this.wrappedEnumerator = null;
    //    }

    //    object IEnumerator.Current
    //    {
    //        get { return this.wrappedEnumerator.Current; }
    //    }

    //    bool IEnumerator.MoveNext()
    //    {
    //        return this.wrappedEnumerator.MoveNext();
    //    }

    //    void IEnumerator.Reset()
    //    {
    //        this.wrappedEnumerator.Reset();
    //    }
    //}
}
