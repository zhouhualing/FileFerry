using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WD.Library.Core;
using WD.Library.Data.Properties;

namespace WD.Library.Data.DataObjects
{
	/// <summary>
	/// ֻ�����ݶ��󼯺���������
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	[ComVisible(true)]
	[DebuggerDisplay("Count = {Count}")]
	[NonXElementSerializedFields(typeof(CollectionBase), "list")]
	public abstract class ReadOnlyDataObjectCollectionBase<T> : CollectionBase, IEnumerable<T>, IXmlSerilizableList
	{
		/// <summary>
		/// ��������ÿһ��Ԫ��
		/// </summary>
		/// <param name="action"></param>
		public virtual void ForEach(Action<T> action)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(action != null, "action");

			foreach (T item in List)
				action(item);
		}

		/// <summary>
		/// �жϼ������Ƿ����ĳԪ��
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual bool Exists(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			bool result = false;

			foreach (T item in List)
			{
				if (match(item))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// �жϼ�����ÿ��Ԫ���Ƿ�����ĳ����
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual bool TrueForAll(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			bool result = true;

			foreach (T item in List)
			{
				if (match(item) == false)
				{
					result = false;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// �ڼ����в�������ƥ�������ĵ�һ��Ԫ��
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual T Find(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			T result = default(T);

			foreach (T item in List)
			{
				if (match(item))
				{
					result = item;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// �Ӻ���ǰ���ң��ҵ���һ��ƥ���Ԫ��
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual T FindLast(Predicate<T> match)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(match != null, "match");

			T result = default(T);

			for (int i = this.Count - 1; i >= 0; i--)
			{
				if (match((T)List[i]))
				{
					result = (T)List[i];
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// �ҵ�����ƥ������������Ԫ��
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public virtual IList<T> FindAll(Predicate<T> match)
		{
			IList<T> result = new List<T>();

			foreach (T item in List)
			{
				if (match(item))
					result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// �Ƿ����ĳ��Ԫ��
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual bool Contains(T item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			return List.Contains(item);
		}

		/// <summary>
		/// �õ�ĳ��Ԫ�ص�λ��
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public virtual int IndexOf(T item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			return List.IndexOf(item);
		}

		/// <summary>
		/// ���Ƶ���ļ�����
		/// </summary>
		/// <param name="collection"></param>
		public virtual void CopyTo(ICollection<T> collection)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(collection != null, "collection");

			this.ForEach(delegate(T item) { collection.Add(item); });
		}

		/// <summary>
		/// ת��������
		/// </summary>
		/// <returns></returns>
		public virtual T[] ToArray()
		{
			T[] result = new T[this.Count];

			for (int i = 0; i < this.Count; i++)
				result[i] = (T)List[i];

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void InnerAdd(T obj)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

			List.Add(obj);
		}
		#region IEnumerable<T> ��Ա

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public new IEnumerator<T> GetEnumerator()
		{
			foreach (T item in List)
				yield return item;
		}

		#endregion

		#region IXmlSerilizableList Members

		void IXmlSerilizableList.Add(object data)
		{
			InnerAdd((T)data);
		}

		void IXmlSerilizableList.Clear()
		{
			List.Clear();
		}

		#endregion
	}

	/// <summary>
	/// ���ݶ��󼯺���������
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	[ComVisible(true)]
	public abstract class DataObjectCollectionBase<T> : ReadOnlyDataObjectCollectionBase<T>
	{
		/// <summary>
		/// �ӱ�ļ����и���(��ӵ����еļ�����)
		/// </summary>
		/// <param name="data"></param>
		public void CopyFrom(IEnumerable<T> data)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

			IEnumerator<T> enumerator = data.GetEnumerator();

			while (enumerator.MoveNext())
				InnerAdd(enumerator.Current);
		}

		/// <summary>
		/// ɾ�����������ļ�¼
		/// </summary>
		/// <param name="match"></param>
		public void Remove(Predicate<T> match)
		{
			int i = 0;

			while (i < this.Count)
			{
				T data = (T)List[i];

				if (match(data))
					this.RemoveAt(i);
				else
					i++;
			}
		}
	}

	/// <summary>
	/// �ܹ��༭�����ݶ��󼯺���������
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	[ComVisible(true)]
	public abstract class EditableDataObjectCollectionBase<T> : DataObjectCollectionBase<T>
	{
		/// <summary>
		/// ���һ������
		/// </summary>
		/// <param name="data"></param>
		public virtual void Add(T data)
		{
			InnerAdd(data);
		}

		/// <summary>
		/// ��д����
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual T this[int index]
		{
			get { return (T)List[index]; }
			set { List[index] = value; }
		}
	}

	/// <summary>
	/// ��Key�ļ����࣬���԰���Key��Index���ַ�ʽ����
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TItem"></typeparam>
	[Serializable]
	public abstract class EditableKeyedDataObjectCollectionBase<TKey, TItem> : EditableDataObjectCollectionBase<TItem>
	{
		[NonSerialized]
		private Hashtable _InnerDict = new Hashtable();

		/// <summary>
		/// ȷ��Item��Key
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected abstract TKey GetKeyForItem(TItem item);

		/// <summary>
		/// ����Key��������ȡ����
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public TItem this[TKey key]
		{
			get
			{
				return (TItem)_InnerDict[key];
			}
		}

		/// <summary>
		/// ָ����key�Ƿ����
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(TKey key)
		{
			return _InnerDict.ContainsKey(key);
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsert(int index, object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

			TItem data = (TItem)value;

			TKey key = GetKeyForItem(data);

			ExceptionHelper.TrueThrow<ArgumentException>(_InnerDict.ContainsKey(key),
				Resource.DuplicateDescriptorKey, typeof(TItem).Name, key);

			_InnerDict.Add(GetKeyForItem(data), data);

			base.OnInsert(index, data);
		}

		/// <summary>
		/// ���Ƶ��ֵ���
		/// </summary>
		/// <param name="dict"></param>
		public void CopyTo(IDictionary<TKey, TItem> dict)
		{
			dict.Clear();

			foreach (DictionaryEntry entry in this._InnerDict)
				dict.Add((TKey)entry.Key, (TItem)entry.Value);
		}

		/// <summary>
		/// ���ֶ��и���
		/// </summary>
		/// <param name="dict"></param>
		public void CopyFrom(IDictionary<TKey, TItem> dict)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dict != null, "dict");

			this.Clear();
			_InnerDict.Clear();

			foreach (var keyValue in dict)
			{
				List.Add(keyValue.Value);
			}
		}

		/// <summary>
		/// ��ɾ��ʱ
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnRemove(int index, object value)
		{
			base.OnRemove(index, value);

			TItem data = (TItem)value;
			_InnerDict.Remove(GetKeyForItem(data));
		}

		/// <summary>
		/// ��ɾ�����ʱ
		/// </summary>
		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			_InnerDict.Clear();
		}
	}
}
