using System;
using System.Collections.Generic;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Caching
{
    /// <summary>
    /// ����ʱ���������ͻ��˴����ڳ�ʼ������Ķ���ʱ��
    /// ��Ҫ�ṩһ�����صľ���ʱ����Ϊ���ڽ�ֹʱ�䣬����ǰʱ�䳬��Ԥ���趨�Ĺ���ʱ��ʱ��
    /// ��Ϊ�����������ص�Cache ����ڡ�
    /// </summary>
    public sealed class AbsoluteTimeDependency : DependencyBase
    {
        private DateTime expiredUtcTime;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="expiredTime">������Ĺ���ʱ��</param>
        /// <remarks>���캯��</remarks>
        public AbsoluteTimeDependency(DateTime expiredTime)
        {
            //�������Ĳ���ΪDateTime.MinValue�Ļ������ܽ���UTCʱ���ת��
            //����ᵼ��ʱ������8Сʱ�Ĵ���
            if(expiredTime.Equals(DateTime.MinValue))
                this.expiredUtcTime = expiredTime;
            else
                this.expiredUtcTime = expiredTime.ToUniversalTime();

            //��ʼ��ʱ����Cache�������޸�ʱ���������ʱ������Ϊ��ǰʱ��
            this.UtcLastModified = DateTime.UtcNow; ;
            this.UtcLastAccessTime = DateTime.UtcNow;
        }

        /// <summary>
        /// ���ԣ���ȡ����ʱ���UTCʱ��ֵ
        /// </summary>
        /// <remarks>
        /// </remarks>
        public DateTime ExpiredUtcTime
        {
            get { return this.expiredUtcTime; }
        }

        /// <summary>
        /// ���ԣ���ȡ����ʱ��ı���ʱ��ֵ
        /// </summary>
        /// <remarks>
        /// </remarks>
        public DateTime ExpiredTime
        {
            get 
            {
                if (this.expiredUtcTime.Equals(DateTime.MinValue))
                    return this.expiredUtcTime;
                return this.expiredUtcTime.ToLocalTime(); 
            }
        }

        /// <summary>
        /// ���ԣ���ȡ��Dependency�Ƿ����
        /// </summary>
        /// <remarks>        
        /// </remarks>
        public override bool HasChanged
        {
            get
            {
                return this.expiredUtcTime <= DateTime.UtcNow;
            }
        }
    }
}
