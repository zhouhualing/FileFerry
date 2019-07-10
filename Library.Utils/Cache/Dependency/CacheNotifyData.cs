using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WD.Library.Core;

namespace WD.Library.Caching
{
	/// <summary>
	/// Cache变更通知类型
	/// </summary>
	public enum CacheNotifyType
	{
		/// <summary>
		/// 数据已经非法
		/// </summary>
		Invalid = 1,

		/// <summary>
		/// 数据修改，变更通知中，包含已经变更的数据
		/// </summary>
		Update = 2,

		/// <summary>
		/// 清除所有数据
		/// </summary>
		Clear = 3
	}

	/// <summary>
	/// Cache变更时的数据包
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
		/// 构造方法
		/// </summary>
		public CacheNotifyData()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="queueType">Cache队列的类型</param>
		/// <param name="key">Cache的key</param>
		/// <param name="nType">通知的类型</param>
		public CacheNotifyData(Type queueType, object key, CacheNotifyType nType)
		{
			InitTypeDesp(queueType);
			this.cacheKey = key;
			this.notifyType = nType;
		}

		/// <summary>
		/// 通知的类型
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
		/// Cache队列的类型
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
		/// Cache队列的类型描述
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
		/// Cache项的数据
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
		/// Cache项的Key
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
			ExceptionHelper.FalseThrow(queueType.IsSubclassOf(typeof(CacheQueueBase)), "queueType类型必须是CacheQueueBase的派生类");

			this.cacheQueueTypeDesp = queueType.AssemblyQualifiedName;
			this.cacheQueueType = queueType;
		}
	}
}
