using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Core
{
	/// <summary>
	/// 封装用户登录信息的类  
	/// </summary>
	/// <remarks>封装用户登录信息的类，包括登录名（可包含域名），登录名（不含域名），域名和口令。</remarks>
	[Serializable]
	public class LogOnIdentity
	{
		private string logOnName = string.Empty;
		private string logOnNameWithoutDomain = string.Empty;
		private string domain = string.Empty;
		private string password = string.Empty;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logonName">登录名，可以含域名</param>
		/// <remarks>构造函数
		
		/// </remarks>
		public LogOnIdentity(string logonName)
		{
			LogOnName = logonName;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logonUserName">登录名，可以含域名</param>
		/// <param name="pwd">口令</param>
		/// <remarks>
		/// 构造函数
		
		/// </remarks>
		public LogOnIdentity(string logonUserName, string pwd)
		{
			LogOnName = logonUserName;

			this.password = pwd;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logonUserName">登录名，可以含域名</param>
		/// <param name="pwd">口令</param>
		/// <param name="logonDomain">域名</param>
		/// <remarks>
		/// 构造函数
		
		/// </remarks>
		public LogOnIdentity(string logonUserName, string pwd, string logonDomain)
		{
			LogOnName = logonUserName;

			this.password = pwd;
			if (string.IsNullOrEmpty(logonDomain) == false)
				this.domain = logonDomain;
		}

		/// <summary>
		/// 登录名，可以含域名
		/// </summary>
		/// <remarks>该属性是可读可写的</remarks>
		public string LogOnName
		{
			get
			{
				return this.logOnName;
			}
			set
			{
				this.logOnName = value;
				AnalysisLogOnName(this.logOnName);
			}
		}

		/// <summary>
		/// 不含域名的登录名
		/// </summary>
		/// <remarks>该属性是只读的</remarks>
		public string LogOnNameWithoutDomain
		{
			get
			{
				return this.logOnNameWithoutDomain;
			}
		}

		/// <summary>
		/// 包含域名的登录名
		/// </summary>
		public string LogOnNameWithDomain
		{
			get
			{
				string result = this.logOnNameWithoutDomain;

				if (string.IsNullOrEmpty(this.domain) == false)
				{
					if (this.domain.IndexOf(".") >= 0)
						result = this.logOnNameWithoutDomain + "@" + this.domain;
					else
						result = this.domain + "\\" + this.logOnNameWithoutDomain;
				}

				return result;
			}
		}

		/// <summary>
		/// 域名
		/// </summary>
		/// <remarks>该属性是只读的</remarks>
		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		/// <summary>
		/// 口令
		/// </summary>
		/// <remarks>该属性是可读可写的</remarks>
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		private void AnalysisLogOnName(string strLogOnName)
		{
			this.logOnNameWithoutDomain = string.Empty;
			this.domain = string.Empty;

			if (string.IsNullOrEmpty(strLogOnName) == false)
			{
				string[] nameParts = strLogOnName.Split('/', '\\');

				string strNameWithoutDomain = string.Empty;

				if (nameParts.Length > 1)
				{
					this.domain = nameParts[0];
					strNameWithoutDomain = nameParts[1];
				}
				else
					strNameWithoutDomain = nameParts[0];

				string[] nameParts2 = strNameWithoutDomain.Split('@');

				this.logOnNameWithoutDomain = nameParts2[0];

				if (nameParts2.Length > 1)
					if (string.IsNullOrEmpty(this.domain))
						this.domain = nameParts2[1];
			}
		}
	}
}
