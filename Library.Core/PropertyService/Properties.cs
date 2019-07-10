
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

namespace WD.Library.Core
{
	public interface IMementoCapable
	{
		DataProperties CreateMemento();

		void SetMemento(DataProperties memento);
	}
	
	public sealed class DataProperties : INotifyPropertyChanged, ICloneable
	{
		public static readonly Version FileVersion = new Version(1, 0, 0);
		
		object syncRoot;
		DataProperties parent;
		Dictionary<string, object> dict = new Dictionary<string, object>();
		
		#region Constructor
		public DataProperties()
		{
			this.syncRoot = new object();
		}
		
		private DataProperties(DataProperties parent)
		{
			this.parent = parent;
			this.syncRoot = parent.syncRoot;
		}
		#endregion
		
		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnPropertyChanged(string key)
		{
			var handler = Volatile.Read(ref PropertyChanged);
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(key));
		}
		#endregion
		
		#region IsDirty
		bool isDirty;
		public bool IsDirty {
			get { return isDirty; }
			set {
				lock (syncRoot) {
					if (value)
						MakeDirty();
					else
						CleanDirty();
				}
			}
		}
		
		void MakeDirty()
		{
			// called within syncroot
			if (!isDirty) {
				isDirty = true;
				if (parent != null)
					parent.MakeDirty();
			}
		}
		
		void CleanDirty()
		{
			if (isDirty) {
				isDirty = false;
				foreach (var properties in dict.Values.OfType<DataProperties>()) {
					properties.CleanDirty();
				}
			}
		}
		#endregion
		
		#region Keys/Contains
		public IReadOnlyList<string> Keys {
			get {
				lock (syncRoot) {
					return dict.Keys.ToArray();
				}
			}
		}
		
		public bool Contains(string key)
		{
			lock (syncRoot) {
				return dict.ContainsKey(key);
			}
		}
		#endregion
		
		#region Get and Set
		/// <summary>
		/// Retrieves a string value from this Properties-container.
		/// Using this indexer is equivalent to calling <c>Get(key, string.Empty)</c>.
		/// </summary>
		public string this[string key] {
			get {
				lock (syncRoot) {
					object val;
					dict.TryGetValue(key, out val);
					return val as string ?? string.Empty;
				}
			}
			set {
				Set(key, value);
			}
		}

        /// <summary>
        /// Retrieves a single element from this Properties-container.
        /// </summary>
        /// <param name="key">Key of the item to retrieve</param>
        /// <param name="defaultValue">Default value to be returned if the key is not present.</param>
        public T Get<T>(string key, T defaultValue)
        {
            lock (syncRoot)
            {
                object val;
                if (dict.TryGetValue(key, out val))
                {
                    try
                    {
                        return (T)Deserialize(val, typeof(T));
                    }
                    catch (SerializationException ex)
                    {
                        //Logger.Error(ex.Message,ex);
                        return defaultValue;
                    }
                }
                else
                {
                    return defaultValue;
                }
            }
        }
		
		public void Set<T>(string key, T value)
		{
			object serializedValue = Serialize(value, typeof(T), key);
			SetSerializedValue(key, serializedValue);
		}
		
		void SetSerializedValue(string key, object serializedValue)
		{
			if (serializedValue == null) {
				Remove(key);
				return;
			}
			lock (syncRoot) {
				object oldValue;
				if (dict.TryGetValue(key, out oldValue)) {
					if (object.Equals(serializedValue, oldValue))
						return;
					HandleOldValue(oldValue);
				}
				dict[key] = serializedValue;
			}
			OnPropertyChanged(key);
		}
		#endregion
		
		#region GetList/SetList
		/// <summary>
		/// Retrieves the list of items stored with the specified key.
		/// If no entry with the specified key exists, this method returns an empty list.
		/// </summary>
		/// <remarks>
		/// This method returns a copy of the list used internally; you need to call
		/// <see cref="SetList"/> if you want to store the changed list.
		/// </remarks>
		public IReadOnlyList<T> GetList<T>(string key)
		{
			lock (syncRoot) {
				object val;
				if (dict.TryGetValue(key, out val)) {
					object[] serializedArray = val as object[];
					if (serializedArray != null) {
						try {
							T[] array = new T[serializedArray.Length];
							for (int i = 0; i < array.Length; i++) {
								array[i] = (T)Deserialize(serializedArray[i], typeof(T));
							}
							return array;
						} catch (XamlObjectWriterException ex) {
                            //Logger.Error(ex.Message,ex);
						} catch (NotSupportedException ex) {
                            //Logger.Error(ex.Message,ex);
						}
					} else {
						//Logger.Error("Properties.GetList(" + key + ") - this entry is not a list");
					}
				}
				return new T[0];
			}
		}
		
		/// <summary>
		/// Sets a list of elements in this Properties-container.
		/// The elements will be serialized using a TypeConverter if possible, or XAML serializer otherwise.
		/// </summary>
		/// <remarks>Passing <c>null</c> or an empty list as value has the same effect as calling <see cref="Remove"/>.</remarks>
		public void SetList<T>(string key, IEnumerable<T> value)
		{
			if (value == null) {
				Remove(key);
				return;
			}
			T[] array = value.ToArray();
			if (array.Length == 0) {
				Remove(key);
				return;
			}
			object[] serializedArray = new object[array.Length];
			for (int i = 0; i < array.Length; i++) {
				serializedArray[i] = Serialize(array[i], typeof(T), null);
			}
			SetSerializedValue(key, serializedArray);
		}
		
		#endregion
		
		#region Serialization
		object Serialize(object value, Type sourceType, string key)
		{
			if (value == null)
				return null;
			TypeConverter c = TypeDescriptor.GetConverter(sourceType);
			if (c != null && c.CanConvertTo(typeof(string)) && c.CanConvertFrom(typeof(string))) {
				return c.ConvertToInvariantString(value);
			}
			
			var element = new XElement("SerializedObject");
			if (key != null) {
				element.Add(new XAttribute("key", key));
			}
			using (var xmlWriter = element.CreateWriter()) {
				XamlServices.Save(xmlWriter, value);
			}
			return element;
		}
		
		object Deserialize(object serializedVal, Type targetType)
		{
			if (serializedVal == null)
				return null;
			XElement element = serializedVal as XElement;
			if (element != null) {
				using (var xmlReader = element.Elements().Single().CreateReader()) {
					return XamlServices.Load(xmlReader);
				}
			} else {
				string text = serializedVal as string;
				if (text == null)
					throw new InvalidOperationException("Cannot read a properties container as a single value");
				TypeConverter c = TypeDescriptor.GetConverter(targetType);
				return c.ConvertFromInvariantString(text);
			}
		}
		#endregion
		
		#region Remove
		/// <summary>
		/// Removes the entry (value, list, or nested container) with the specified key.
		/// </summary>
		public bool Remove(string key)
		{
			bool removed = false;
			lock (syncRoot) {
				object oldValue;
				if (dict.TryGetValue(key, out oldValue)) {
					removed = true;
					HandleOldValue(oldValue);
					MakeDirty();
					dict.Remove(key);
				}
			}
			if (removed)
				OnPropertyChanged(key);
			return removed;
		}
		#endregion
		
		#region Nested Properties
		/// <summary>
		/// Gets the parent property container.
		/// </summary>
		public DataProperties Parent {
			get {
				lock (syncRoot) {
					return parent;
				}
			}
		}
		public DataProperties NestedProperties(string key)
		{
			bool isNewContainer = false;
			DataProperties result;
			lock (syncRoot) {
				object oldValue;
				dict.TryGetValue(key, out oldValue);
				result = oldValue as DataProperties;
				if (result == null) {
					result = new DataProperties(this);
					dict[key] = result;
					isNewContainer = true;
				}
			}
			if (isNewContainer)
				OnPropertyChanged(key);
			return result;
		}
		
		void HandleOldValue(object oldValue)
		{
			DataProperties p = oldValue as DataProperties;
			if (p != null) {
				Debug.Assert(p.parent == this);
				p.parent = null;
			}
		}
		
		public void SetNestedProperties(string key, DataProperties properties)
		{
			if (properties == null) {
				Remove(key);
				return;
			}
			lock (syncRoot) {
				for (DataProperties ancestor = this; ancestor != null; ancestor = ancestor.parent) {
					if (ancestor == properties)
						throw new InvalidOperationException("Cannot add a properties container to itself.");
				}
				
				object oldValue;
				if (dict.TryGetValue(key, out oldValue)) {
					if (oldValue == properties)
						return;
					HandleOldValue(oldValue);
				}
				lock (properties.syncRoot) {
					if (properties.parent != null)
						throw new InvalidOperationException("Cannot attach nested properties that already have a parent.");
					MakeDirty();
					properties.SetSyncRoot(syncRoot);
					properties.parent = this;
					dict[key] = properties;
				}
			}
			OnPropertyChanged(key);
		}
		
		void SetSyncRoot(object newSyncRoot)
		{
			this.syncRoot = newSyncRoot;
			foreach (var properties in dict.Values.OfType<DataProperties>()) {
				properties.SetSyncRoot(newSyncRoot);
			}
		}
		#endregion
		
		#region Clone
		/// <summary>
		/// Creates a deep clone of this Properties container.
		/// </summary>
		public DataProperties Clone()
		{
			lock (syncRoot) {
				return CloneWithParent(null);
			}
		}
		
		DataProperties CloneWithParent(DataProperties parent)
		{
			DataProperties copy = parent != null ? new DataProperties(parent) : new DataProperties();
			foreach (var pair in dict) {
				DataProperties child = pair.Value as DataProperties;
				if (child != null)
					copy.dict.Add(pair.Key, child.CloneWithParent(copy));
				else
					copy.dict.Add(pair.Key, pair.Value);
			}
			return copy;
		}
		
		object ICloneable.Clone()
		{
			return Clone();
		}
		#endregion
		
		#region ReadFromAttributes
		internal static DataProperties ReadFromAttributes(XmlReader reader)
		{
			DataProperties properties = new DataProperties();
			if (reader.HasAttributes) {
				for (int i = 0; i < reader.AttributeCount; i++) {
					reader.MoveToAttribute(i);
					string val = reader.NameTable.Add(reader.Value);
					properties[reader.Name] = val;
				}
				reader.MoveToElement();
			}
			return properties;
		}
		#endregion
		
		#region Load/Save
		public static DataProperties Load(FileName fileName)
		{
			return Load(XDocument.Load(fileName).Root);
		}
		
		public static DataProperties Load(XElement element)
		{
			DataProperties properties = new DataProperties();
			properties.LoadContents(element.Elements());
			return properties;
		}
		
		void LoadContents(IEnumerable<XElement> elements)
		{
			foreach (var element in elements) {
				string key = (string)element.Attribute("key");
				if (key == null)
					continue;
				switch (element.Name.LocalName) {
					case "Property":
						dict[key] = element.Value;
						break;
					case "Array":
						dict[key] = LoadArray(element.Elements());
						break;
					case "SerializedObject":
						dict[key] = new XElement(element);
						break;
					case "Properties":
						DataProperties child = new DataProperties(this);
						child.LoadContents(element.Elements());
						dict[key] = child;
						break;
				}
			}
		}
		
		static object[] LoadArray(IEnumerable<XElement> elements)
		{
			List<object> result = new List<object>();
			foreach (var element in elements) {
				switch (element.Name.LocalName) {
					case "Null":
						result.Add(null);
						break;
					case "Element":
						result.Add(element.Value);
						break;
					case "SerializedObject":
						result.Add(new XElement(element));
						break;
				}
			}
			return result.ToArray();
		}
		
		public void Save(FileName fileName)
		{
			new XDocument(Save()).Save(fileName);
		}
		
		public XElement Save()
		{
			lock (syncRoot) {
				return new XElement("Properties", SaveContents());
			}
		}
		
		IReadOnlyList<XElement> SaveContents()
		{
			List<XElement> result = new List<XElement>();
			foreach (var pair in dict) {
				XAttribute key = new XAttribute("key", pair.Key);
				DataProperties child = pair.Value as DataProperties;
				if (child != null) {
					var contents = child.SaveContents();
					if (contents.Count > 0)
						result.Add(new XElement("Properties", key, contents));
				} else if (pair.Value is object[]) {
					object[] array = (object[])pair.Value;
					XElement[] elements = new XElement[array.Length];
					for (int i = 0; i < array.Length; i++) {
						XElement obj = array[i] as XElement;
						if (obj != null) {
							elements[i] = new XElement(obj);
						} else if (array[i] == null) {
							elements[i] = new XElement("Null");
						} else {
							elements[i] = new XElement("Element", (string)array[i]);
						}
					}
					result.Add(new XElement("Array", key, elements));
				} else if (pair.Value is XElement) {
					result.Add(new XElement((XElement)pair.Value));
				} else {
					result.Add(new XElement("Property", key, (string)pair.Value));
				}
			}
			return result;
		}
		#endregion
	}
}
