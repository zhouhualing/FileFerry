using System;
using System.Collections;
using System.Collections.Generic;

namespace WD.Library.Core
{
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
	{
		private readonly IDictionary<TKey, TValue> source;

		public int Count
		{
			get
			{
				return this.source.Count;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.source.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.source.Values;
			}
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return ((IDictionary)this.source).Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return ((IDictionary)this.source).Values;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.source).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.source).SyncRoot;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.source[key];
			}
			set
			{
				ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return ((IDictionary)this.source)[key];
			}
			set
			{
				ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
			}
		}

		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionaryToWrap)
		{
			if (dictionaryToWrap == null)
			{
				throw new ArgumentNullException("dictionaryToWrap");
			}
			this.source = dictionaryToWrap;
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		public bool ContainsKey(TKey key)
		{
			return this.source.ContainsKey(key);
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.source.TryGetValue(key, out value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.source.Contains(item);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.source.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
			return false;
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return this.source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.source.GetEnumerator();
		}

		void IDictionary.Add(object key, object value)
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		void IDictionary.Clear()
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		bool IDictionary.Contains(object key)
		{
			return ((IDictionary)this.source).Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)this.source).GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			ReadOnlyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.source).CopyTo(array, index);
		}

		private static void ThrowNotSupportedException()
		{
			throw new NotSupportedException("This Dictionary is read-only");
		}
	}
}
