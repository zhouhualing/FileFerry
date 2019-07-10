using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
	/// <summary>
	/// CacheItem�Ļ���
	/// </summary>
	public abstract class CacheItemBase : IDisposable
	{
		private CacheQueueBase cacheQueue;

		//���CacheItem��ص�Dependency,���ж�CacheItem�Ĺ���
		private DependencyBase dependency = null;

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="cacheQueueBase"></param>
		public CacheItemBase(CacheQueueBase cacheQueueBase)
		{
			this.cacheQueue = cacheQueueBase;
		}

		/// <summary>
		/// ��Cache���������Cache��
		/// </summary>
		public void RemoveCacheItem()
		{
			this.cacheQueue.RemoveItem(this);
		}

		//Ϊ��CacheDependency�ܹ���CacheItem��ȡ��������Queueʵ������Ӵ˹�������

		/// <summary>
		/// ��ǰCache�������ڵ�CacheQueue��
		/// </summary>
		public CacheQueueBase Queue
		{
			get
			{
				return this.cacheQueue;
			}
		}

		/// <summary>
		/// ���ԣ���ȡ���������CacheItem�������Dependency
		/// </summary>
		public DependencyBase Dependency
		{
			get { return this.dependency; }
			set { this.dependency = value; }
		}

		/// <summary>
		/// ����CacheItem����Ϊ�������
		/// </summary>
		public void SetChanged()
		{
			if (this.dependency != null)
				this.dependency.SetChanged();
		}
		#region IDisposable ��Ա

		/// <summary>
		/// ʵ��IDisposable�ӿ�
		/// </summary>
		public void Dispose()
		{
			if (this.Dependency != null)
				this.Dependency.Dispose();
		}

		#endregion

		//Ϊ��CacheDependency�ܹ���CacheItem��ȡ��Key��Value����Ӵ��鷽����

		/// <summary>
		/// �õ�CacheItem��Ӧ��Key��Value
		/// </summary>
		/// <returns>���CacheItem��Key��Value����Ҫ����������</returns>
		public abstract KeyValuePair<object, object> GetKeyValue();

		//Ϊ��CacheDependency�ܹ�ΪCacheItem����Value����Ӵ��鷽����

		/// <summary>
		/// ����CacheItem��ֵ
		/// </summary>
		/// <param name="value">ΪCacheItem����Value</param>
		public abstract void SetValue(object value);
	}
}
