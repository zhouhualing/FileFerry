
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WD.Library.Core
{
	public interface IPropertyService : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the configuration directory. (usually "%ApplicationData%\%ApplicationName%")
		/// </summary>
		DirectoryName ConfigDirectory { get; }
		
		/// <summary>
		/// Gets the data directory (usually "ApplicationRootPath\data")
		/// </summary>
		DirectoryName DataDirectory { get; }
		
		/// <summary>
		/// Gets the main properties container for this property service.
		/// </summary>
		DataProperties MainPropertiesContainer { get; }
		
		/// <inheritdoc cref="DataProperties.Get{T}(string, T)"/>
		T Get<T>(string key, T defaultValue);
		
		/// <inheritdoc cref="DataProperties.NestedProperties"/>
		DataProperties NestedProperties(string key);
		
		/// <inheritdoc cref="DataProperties.SetNestedProperties"/>
		void SetNestedProperties(string key, DataProperties nestedProperties);
		
		/// <inheritdoc cref="DataProperties.Contains"/>
		bool Contains(string key);
		
		/// <inheritdoc cref="DataProperties.Set{T}(string, T)"/>
		void Set<T>(string key, T value);
		
		/// <inheritdoc cref="DataProperties.GetList"/>
		IReadOnlyList<T> GetList<T>(string key);
		
		/// <inheritdoc cref="DataProperties.SetList"/>
		void SetList<T>(string key, IEnumerable<T> value);
		
		/// <inheritdoc cref="DataProperties.Remove"/>
		void Remove(string key);
		
		/// <summary>
		/// Saves the main properties to disk.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Loads extra properties that are not part of the main properties container.
		/// Unlike <see cref="NestedProperties"/>, multiple calls to <see cref="LoadExtraProperties"/>
		/// will return different instances, as the properties are re-loaded from disk every time.
		/// To save the properties, you need to call <see cref="SaveExtraProperties"/>.
		/// </summary>
		/// <returns>Properties container that was loaded; or an empty properties container
		/// if no container with the specified key exists.</returns>
		DataProperties LoadExtraProperties(string key);
		
		/// <summary>
		/// Saves extra properties that are not part of the main properties container.
		/// </summary>
		void SaveExtraProperties(string key, DataProperties p);
	}
}
