using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using WD.Library.Properties;

namespace WD.Library.Core
{
	/// <summary>
	/// ��������������㷨(Least Recently Used Algorithm)ʵ�ְ�������ʹ�õ�Ԫ�ط���LruDictionary��ǰ�������������ʹ�õ�Ԫ�ط���LruDictionary�ĺ�
	/// </summary>
	/// <typeparam name="TKey">LruDictionary�ļ�ֵKey������</typeparam>
	/// <typeparam name="TValue">LruDictionary��Value������</typeparam>
	/// <remarks>��������������㷨(Least Recently Used Algorithm)ʵ��LruDictionary��LruDictionary���������ʹ�õ�Ԫ�ط�����󲿣��������ʹ�õ�Ԫ�ط�����ǰ����
	/// LruDictionary�е�ÿ��Ԫ������������ɡ�Key��Value������KeyΪLruDictionary�ļ�ֵ��
	/// </remarks>
	public sealed class LruDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private int maxLength = 100;
		private static object syncRoot = new object();
		private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> innerDictionary =
			new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();

		private LinkedList<KeyValuePair<TKey, TValue>> innerLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();

		#region ���췽��
		/// <summary>
		/// LruDictionaryû�в����Ĺ��캯����
		/// </summary>
		/// <remarks>�ù��캯����LruDictionary����󳤶���100���˹��췽�������ڹ��첻ָ����󳤶ȵ�LruDictionary��
		public LruDictionary()
		{
		}

		/// <summary>
		/// ������󳤶�ΪmaxLength��LruDictionary��
		/// </summary>
		/// <param name="maxLruLength">��Ҫ���õ�LruDictionary����󳤶ȡ�</param>
		/// <remarks>��Ĭ������£�LruDictionary����󳤶�Ϊ100���˹��췽�������ڹ���ָ����󳤶ȵ�LruDictionary��
		/// </remarks>
		public LruDictionary(int maxLruLength)
		{
			ExceptionHelper.TrueThrow<InvalidOperationException>(maxLruLength < 0, "maxLruLength must >= 0");

			this.maxLength = maxLruLength;
		}
		#endregion

		#region ��������
		/// <summary>
		/// ͬ������LruDictionary������
		/// </summary>
		/// <remarks>ͬ������LruDictionary�����ԣ���������ֻ���ġ�</remarks>
		public object SyncRoot
		{
			get
			{
				return ((ICollection)this.innerDictionary).SyncRoot;
			}
		}

		/// <summary>
		/// LruDictionary����󳤶�
		/// </summary>
		/// <remarks>LruDictionary����󳤶ȣ��������ǿɶ���д�ġ�</remarks>
		public int MaxLength
		{
			get { return this.maxLength; }
			set { this.maxLength = value; }
		}

		/// <summary>
		/// LruDictionary�ĵ�ǰ����
		/// </summary>
		/// <remarks>��ǰLruDictionary�ĳ��ȣ���������ֻ���ġ�</remarks>
		public int Count
		{
			get
			{
				return this.innerDictionary.Count;
			}
		}
		#endregion

		#region ���з���
		/// <summary>
		/// ��LruDictionary�����һ��Ԫ�أ��û���Ҫ�����Ԫ�ص�Key��Valueֵ���롣
		/// </summary>
		/// <param name="key">��LruDictionary������Ԫ�ص�Keyֵ</param>
		/// <param name="data">��LruDictionary������Ԫ�ص�Valueֵ</param>
		/// <remarks>��LruDictionary�����һ��Ԫ��ʱ����Ԫ������Key��Valueֵ��ɡ�����LRUԭ�򣬾���ʹ�õ�Ԫ�ط���LruDictionary��ǰ�档
		///</remarks>
		public void Add(TKey key, TValue data)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-Add-key");

			//����Ѿ����ڣ��׳��쳣
			ExceptionHelper.TrueThrow<ArgumentException>(this.innerDictionary.ContainsKey(key),
				Resource.DuplicateKey, key);

			LinkedListNode<KeyValuePair<TKey, TValue>> node =
				new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, data));

			lock (syncRoot)
			{
				this.innerDictionary.Add(key, node);
				this.innerLinkedList.AddFirst(node);

				if (this.innerLinkedList.Count >= MaxLength)
				{
					for (int i = 0; i < this.innerLinkedList.Count - MaxLength + 1; i++)
					{
						LinkedListNode<KeyValuePair<TKey, TValue>> lastNode = this.innerLinkedList.Last;

						if (this.innerDictionary.ContainsKey(lastNode.Value.Key))
						{
							this.innerDictionary.Remove(lastNode.Value.Key);
							this.innerLinkedList.Remove(lastNode);
						}
					}
				}
			}
		}

		/// <summary>
		/// ��ȡ������LruDictionary��Ԫ�ؼ�ֵΪkeyֵ��Valueֵ
		/// </summary>
		/// <param name="key">Ҫ��õ�Ԫ�صļ�ֵ</param>
		/// <returns>LruDictionary�м�ֵkey��Valueֵ</returns>
		/// <remarks>�������ǿɶ���д�ġ�
		/// </remarks>
		public TValue this[TKey key]
		{
			get
			{
				ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-get-key");

				LinkedListNode<KeyValuePair<TKey, TValue>> node = this.innerDictionary[key];
				//û���ҵ������Զ��׳��쳣
				lock (syncRoot)
				{
					MoveNodeToFirst(node);
				}
				return node.Value.Value;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-set-key");

				LinkedListNode<KeyValuePair<TKey, TValue>> node;

				lock (syncRoot)
				{
					if (this.innerDictionary.TryGetValue(key, out node))
					{
						MoveNodeToFirst(node);

						node.Value = new KeyValuePair<TKey, TValue>(key, value);
					}
					else
						Add(key, value);
				}
			}
		}

		/// <summary>
		/// ɾ��LruDictionary�м�ֵΪKeyֵ��Ԫ��
		/// </summary>
		/// <param name="key">��ֵKey</param>
		/// <remarks>��LruDictionary�в�������ֵΪKey��Ԫ�أ���ϵͳ�Զ����׳��쳣��
		public void Remove(TKey key)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-Remove-key");
			LinkedListNode<KeyValuePair<TKey, TValue>> node = null;

			lock (syncRoot)
			{
				if (this.innerDictionary.TryGetValue(key, out node))
				{
					this.innerDictionary.Remove(key);
					this.innerLinkedList.Remove(node);
				}
			}
		}

		/// <summary>
		/// �ж�LruDictionary���Ƿ������ֵΪKeyֵ��Ԫ��
		/// </summary>
		/// <param name="key">��ֵKey</param>
		/// <returns>��LruDictionary�а�����ֵΪkeyֵ��Ԫ��,�򷵻�true�����򷵻�false</returns>
		/// <remarks>������ֵΪtrue�����ڸ�Keyֵ��Ԫ�ظ�ʹ�ù�����Ѹ�Ԫ�ط���LruDictionary����ǰ�档
		/// </remarks>
		public bool ContainsKey(TKey key)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-ContainsKey-key");

			return this.innerDictionary.ContainsKey(key);
		}

		/// <summary>
		/// �ж�LruDictionary���Ƿ������ֵΪKeyֵ��Ԫ�ء����������򷵻�ֵ��true�����Դ�data��ȡ����ֵ�����򷵻�false��
		/// </summary>
		/// <param name="key">��ֵkey</param>
		/// <param name="data">��ֵkey��Valueֵ</param>
		/// <returns>����true��false</returns>
		/// <remarks>������ֵΪtrue�����ڸ�Keyֵ��Ԫ�ظ�ʹ�ù�����Ѹ�Ԫ�ط���LruDictionary����ǰ�档
		/// </remarks>
		public bool TryGetValue(TKey key, out TValue data)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(key == null, "LruDictionary-TryGetValue-key");

			LinkedListNode<KeyValuePair<TKey, TValue>> node;
			data = default(TValue);

			lock (syncRoot)
			{
				bool result = this.innerDictionary.TryGetValue(key, out node);

				if (result)
				{
					MoveNodeToFirst(node);

					data = node.Value.Value;
				}

				return result;
			}
		}

		/// <summary>
		/// ���LruDictionary�ڵ�����ֵ
		/// </summary>
		/// <remarks>��ʱLruDictionary��û���κ�Ԫ��
		/// </remarks>
		public void Clear()
		{
			lock (syncRoot)
			{
				this.innerDictionary.Clear();
				this.innerLinkedList.Clear();
			}
		}
		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> ��Ա
		/// <summary>
		/// ���LruDictionary������Ԫ�ص�ö����
		/// </summary>
		/// <returns>LruDictionary������Ԫ�ص�ö����</returns>
		/// <remarks>		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return EnumItems();
		}

		#endregion

		#region IEnumerable ��Ա

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return EnumItems();
		}

		#endregion

		private IEnumerator<KeyValuePair<TKey, TValue>> EnumItems()
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> node = this.innerLinkedList.First;

			while (node != null)
			{
				yield return node.Value;
				node = node.Next;
			}
		}

		private void MoveNodeToFirst(LinkedListNode<KeyValuePair<TKey, TValue>> node)
		{
			LinkedList<KeyValuePair<TKey, TValue>> list = node.List;

			list.Remove(node);
			list.AddFirst(node);
		}
	}
}
