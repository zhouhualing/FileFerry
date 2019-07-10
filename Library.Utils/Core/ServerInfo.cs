using System;
using System.Text;
using System.Collections.Generic;

namespace WD.Library.Core
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ServerInfo
	{
		private string serverName = string.Empty;
		private LogOnIdentity identity;
		private int port = 0;

		/// <summary>
		/// 
		/// </summary>
		public LogOnIdentity Identity
		{
			get { return identity; }
			set { identity = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string ServerName
		{
			get { return serverName; }
			set { serverName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Port
		{
			get { return this.port; }
			set { this.port = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public ServerInfo()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="identity"></param>
		public ServerInfo(string serverName, LogOnIdentity identity)
		{
			this.serverName = serverName;
			this.identity = identity;
		}
	}
}
