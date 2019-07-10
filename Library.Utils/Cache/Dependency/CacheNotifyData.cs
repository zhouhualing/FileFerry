using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WD.Library.Core;

namespace WD.Library.Caching
{
	/// <summary>
	/// Cache���֪ͨ����
	/// </summary>
	public enum CacheNotifyType
	{
		/// <summary>
		/// �����Ѿ��Ƿ�
		/// </summary>
		Invalid = 1,

		/// <summary>
		/// �����޸ģ����֪ͨ�У������Ѿ����������
		/// </summary>
		Update = 2,

		/// <summary>
		/// �����������
		/// </summary>
		Clear = 3
	}

	/// <summary>
	/// Cache���ʱ�����ݰ�
	/// </summary>
	[Serializable]
	public class CacheNotifyData
	{
		private const string VersionMatchTemplate = @"Version=([0-9.]{1,})(,)";

		private CacheNotifyType notifyType = CacheNotifyType.Invalid;

		private string cacheQueueTypeDesp = string.Empty;

		private object cacheKey = null;
		private object cacheData = null;

		[NonSerialized]
		private Type cacheQueueType = null;

		/// <summary>
		/// ���췽��
		/// </summary>
		public CacheNotifyData()
		{
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="queueType">Cache���е�����</param>
		/// <param name="key">Cache��key</param>
		/// <param name="nType">֪ͨ������</param>
		public CacheNotifyData(Type queueType, object key, CacheNotifyType nType)
		{
			InitTypeDesp(queueType);
			this.cacheKey = key;
			this.notifyType = nType;
		}

		/// <summary>
		/// ֪ͨ������
		/// </summary>
		public CacheNotifyType NotifyType
		{
			get
			{
				return this.notifyType;
			}
			set
			{
				this.notifyType = value;
			}
		}

		/// <summary>
		/// Cache���е�����
		/// </summary>
		public Type CacheQueueType
		{
			get
			{
				if (this.cacheQueueType == null && string.IsNullOrEmpty(this.cacheQueueTypeDesp) == false)
				{
					string typeName = Regex.Replace(this.cacheQueueTypeDesp, VersionMatchTemplate, string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

					this.cacheQueueType = TypeCreator.GetTypeInfo(typeName);
				}

				return this.cacheQueueType;
			}
		}

		/// <summary>
		/// Cache���е���������
		/// </summary>
		public string CacheQueueTypeDesp
		{
			get
			{
				return this.cacheQueueTypeDesp;
			}
			set
			{
				this.cacheQueueTypeDesp = value;
				this.cacheQueueType = null;
			}
		}

		/// <summary>
		/// Cache�������
		/// </summary>
		public object CacheData
		{
			get
			{
				return this.cacheData;
			}
			set
			{
				this.cacheData = value;
			}
		}

		/// <summary>
		/// Cache���Key
		/// </summary>
		public object CacheKey
		{
			get
			{
				return this.cacheKey;
			}
			set
			{
				this.cacheKey = value;
			}
		}

		private void InitTypeDesp(Type queueType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(queueType != null, "queueType");
			ExceptionHelper.FalseThrow(queueType.IsSubclassOf(typeof(CacheQueueBase)), "queueType���ͱ�����CacheQueueBase��������");

			this.cacheQueueTypeDesp = queueType.AssemblyQualifiedName;
			this.cacheQueueType = queueType;
		}
	}
}
