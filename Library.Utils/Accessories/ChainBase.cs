using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Accessories
{
    /// <summary>
    /// ְ����ģʽ�Ļ��࣬ʵ����IChain(ְ����Interface)�ӿڡ�
    /// </summary>
    /// <typeparam name="T">ְ������ÿһ���ڵ������</typeparam>
    /// <remarks>
    /// �û����ṩ��ְ�����Ļ�������ʵ�֣�����������Ĳ����Լ����ݻ�ȡ��
    /// </remarks>
    public abstract class ChainBase<T> : IChain<T>
        where T : class
    {
        /// <summary>
        /// ������������ÿһ��ı�
        /// </summary>
		/// <remarks>������������ÿһ��ı�</remarks>
        protected List<T> chainItems;

        /// <summary>
        /// ���������Ĺ��캯��
        /// </summary>
        /// <remarks>���������Ĺ��캯��
        public ChainBase()
        {
			this.chainItems = new List<T>();
        }

        /// <summary>
        /// ְ�����ĵ�һ���ڵ�
        /// </summary>
        /// <remarks>
        /// ְ�����ĵ�һ���ڵ㣬��������ֻ���ġ�
        public T Head
        {
            get
            {
				return this.chainItems.Count > 0 ? this.chainItems[0] : null;
            }
        }

        /// <summary>
        /// ��ְ���������һ���ڵ�
        /// </summary>
        /// <param name="item">��Ҫ��ְ��������ӵĽڵ�</param>
        /// <remarks>
        /// ��ְ���������һ���ڵ�
        public void Add(T item)
        {
			this.chainItems.Add(item);
        }

        /// <summary>
        /// ��ְ���������һ��ڵ㡣
        /// </summary>
		/// <param name="items">�ڵ���</param>
        /// <remarks>
        /// ��ְ���������һ��ڵ㡣
        public void AddRange(IEnumerable items)
        {
			foreach (T item in items)
                Add(item);
        }

        /// <summary>
        /// ��ְ�����л�õ�ǰ�ڵ����һ���ڵ㡣
        /// </summary>
        /// <param name="current">��ǰ�Ľڵ�</param>
        /// <returns>��ȡְ�����ĵ�ǰ�ڵ����һ���ڵ㣬���ָ���Ľڵ�Ϊ�ջ�ǰ�ڵ����һ���ڵ�Ϊ�գ��򷵻ؿա�
        /// </returns>
        /// <remarks>
        /// ���ְ������ָ���ڵ����һ���ڵ�
        public T GetNext(T current)
        {
			for (int i = 0; i < this.chainItems.Count - 1; i++)
			{
				if (ReferenceEquals(current, this.chainItems[i]))
					return this.chainItems[i + 1];
			}

            return null;
        }

    }
}