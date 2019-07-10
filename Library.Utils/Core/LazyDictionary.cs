using System;
using System.Collections;
using System.Collections.Generic;

namespace WD.Library.Core
{
	public class LazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private readonly Func<IDictionary<TKey, TValue>> m_getter;

		private IDictionary<TKey, TValue> m_inner;

		public ICollection<TKey> Keys
		{
			get
			{
				return this.Inner.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.Inner.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.Inner[key];
			}
			set
			{
				LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
			}
		}

		public int Count
		{
			get
			{
				return this.Inner.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		private IDictionary<TKey, TValue> Inner
		{
			get
			{
				if (this.m_inner == null)
				{
					this.m_inner = this.m_getter();
				}
				return this.m_inner;
			}
		}

		public LazyDictionary(Func<IDictionary<TKey, TValue>> getter)
		{
			this.m_getter = getter;
		}

		public void Add(TKey key, TValue value)
		{
			LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		public bool ContainsKey(TKey key)
		{
			return this.Inner.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.Inner.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		public void Clear()
		{
			LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.Inner.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			LazyDictionary<TKey, TValue>.ThrowNotSupportedException();
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.Inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Inner.GetEnumerator();
		}

		private static void ThrowNotSupportedException()
		{
			throw new NotSupportedException("This Dictionary is read-only");
		}
	}
}
