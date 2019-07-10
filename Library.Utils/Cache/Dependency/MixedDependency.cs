using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Caching
{
    /// <summary>
    /// ���Cache�����࣬��AbsoluteTimeDependency��SlidingTimeDependency��FileCacheDependency���������
    /// �������κ�һ�����ʱ����Ϊ���MixedDependency��ص�Cache�����
    /// </summary>
    public sealed class MixedDependency : DependencyBase
    {
        private DependencyBase[] dependencyArray = null;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="dependencyArray">����AbsoluteTimeDependency��SlidingTimeDependency��FileCacheDependency�����������</param>
        /// <remarks>        
        /// </remarks>
        public MixedDependency(params DependencyBase[] dependencyArray)
        {
            this.dependencyArray = dependencyArray;

            //������޸�ʱ���������ʱ����г�ʼ��
            this.UtcLastModified = DateTime.UtcNow;
            this.UtcLastAccessTime = DateTime.UtcNow;
        }

        /// <summary>
        /// ���ԣ��жϴ�Cache�����Ƿ����
        /// </summary>
        /// <remarks>
        
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                bool result = false;

                for (int index = 0; index < this.dependencyArray.Length; index++)
                    if (this.dependencyArray[index].HasChanged)
                    {
                        result = true;
                        break;
                    }

                return result;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		internal protected override void SetChanged()
		{
			for (int index = 0; index < this.dependencyArray.Length; index++)
				this.dependencyArray[index].SetChanged();
		}

        /// <summary>
        /// ʵ��IDisposable�ӿ�
        /// </summary>
        public override void Dispose()
        {
            foreach (DependencyBase bcd in this.dependencyArray)
                bcd.Dispose();
        }

        /// <summary>
        /// ����,��ȡ������Cache���������ʱ���UTCʱ��ֵ
        /// </summary>
        public override DateTime UtcLastAccessTime
        {
            get
            {
                return base.UtcLastAccessTime;
            }
            set
            {
                for (int i = 0; i < this.dependencyArray.Length; i++)
                    this.dependencyArray[i].UtcLastAccessTime = value;

                base.UtcLastAccessTime = value;
            }
        }

        /// <summary>
        /// ����,��ȡ������Cache������޸�ʱ���UTCʱ��ֵ
        /// </summary>
        public override DateTime UtcLastModified
        {
            get
            {
                return base.UtcLastModified;
            }
            set
            {
                for (int i = 0; i < this.dependencyArray.Length; i++)
                    this.dependencyArray[i].UtcLastModified = value;

                base.UtcLastModified = value;
            }
        }

		/// <summary>
		/// ��Dependency����󶨵�CacheItemʱ���ô˷������صݹ������Dependency��CacheItemBinded����
		/// </summary>
		protected internal override void CacheItemBinded()
		{
			for (int i = 0; i < this.dependencyArray.Length; i++)
			{
				this.dependencyArray[i].CacheItem = this.CacheItem;
				this.dependencyArray[i].CacheItemBinded();
			}
		}
    }
}
