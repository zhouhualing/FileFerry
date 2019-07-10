using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// ���ڴ��ͨ�����͵�CacheQueue
    /// </summary>
    public sealed class ObjectCacheQueue : CacheQueue<object, object>
    {
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        private static readonly ObjectCacheQueue instance = CacheManager.GetInstance<ObjectCacheQueue>();

		/// <summary>
		/// ��ȡʵ��
		/// </summary>
		public static ObjectCacheQueue Instance
		{
			get
			{
				return ObjectCacheQueue.instance;
			}
		}

        //ʵ��SingleTonģʽ
        private ObjectCacheQueue()
        {
        }
    }
}
