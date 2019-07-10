using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// ���Key��Value����object��ContextCache
    /// </summary>
    public sealed class ObjectContextCache : ContextCacheQueueBase<object, object>
    {
        /// <summary>
        /// ObjectContextCache��ʵ�����˴����������ԣ���̬����
        /// </summary>
        public static ObjectContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<ObjectContextCache>();
            }
        }

        private ObjectContextCache()
        {
        }
    }
}
