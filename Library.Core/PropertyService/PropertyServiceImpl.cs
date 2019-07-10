
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace WD.Library.Core
{
	public class PropertyServiceImpl : IPropertyService
	{
		readonly DataProperties properties;
		
		public PropertyServiceImpl()
		{
			properties = new DataProperties();
		}
		
		public PropertyServiceImpl(DataProperties properties)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");
			this.properties = properties;
		}
		
		public virtual DirectoryName ConfigDirectory {
			get {
				throw new NotImplementedException();
			}
		}
		
		public virtual DirectoryName DataDirectory {
			get {
				throw new NotImplementedException();
			}
		}
		
		/// <inheritdoc cref="DataProperties.Get{T}(string, T)"/>
		public T Get<T>(string key, T defaultValue)
		{
			return properties.Get(key, defaultValue);
		}
		
		/// <inheritdoc cref="DataProperties.NestedProperties"/>
		public DataProperties NestedProperties(string key)
		{
			return properties.NestedProperties(key);
		}
		
		/// <inheritdoc cref="DataProperties.SetNestedProperties"/>
		public void SetNestedProperties(string key, DataProperties nestedProperties)
		{
			properties.SetNestedProperties(key, nestedProperties);
		}
		
		/// <inheritdoc cref="DataProperties.Contains"/>
		public bool Contains(string key)
		{
			return properties.Contains(key);
		}
		
		/// <inheritdoc cref="DataProperties.Set{T}(string, T)"/>
		public void Set<T>(string key, T value)
		{
			properties.Set(key, value);
		}
		
		/// <inheritdoc cref="DataProperties.GetList"/>
		public IReadOnlyList<T> GetList<T>(string key)
		{
			return properties.GetList<T>(key);
		}
		
		/// <inheritdoc cref="DataProperties.SetList"/>
		public void SetList<T>(string key, IEnumerable<T> value)
		{
			properties.SetList(key, value);
		}
		
		/// <inheritdoc cref="DataProperties.Remove"/>
		public void Remove(string key)
		{
			properties.Remove(key);
		}
		
		public event PropertyChangedEventHandler PropertyChanged {
			add { properties.PropertyChanged += value; }
			remove { properties.PropertyChanged -= value; }
		}
		
		public DataProperties MainPropertiesContainer {
			get { return properties; }
		}
		
		public virtual void Save()
		{
		}

		public virtual DataProperties LoadExtraProperties(string key)
		{
			return new DataProperties();
		}

		public virtual void SaveExtraProperties(string key, DataProperties p)
		{
		}
	}
}
