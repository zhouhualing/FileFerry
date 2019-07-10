using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace WD.Library.Core
{
	/// <summary>
	/// 需要监控和记录日志的性能对象。主要用于计量执行时间
	/// </summary>
	public class MonitorData
	{
		private string instanceName = string.Empty;
		private string monitorName = string.Empty;
		private Stopwatch stopwatch = new Stopwatch();
		private TextWriter logWriter = new StringWriter(new StringBuilder(256));
		private bool enableLogging = true;
		private bool enablePFCounter = true;
		private bool hasErrors = false;

		/// <summary>
		/// 
		/// </summary>
		public string InstanceName
		{
			get { return instanceName; }
			set { instanceName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool HasErrors
		{
			get { return hasErrors; }
			set { hasErrors = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string MonitorName
		{
			get { return monitorName; }
			set { monitorName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool EnablePFCounter
		{
			get { return enablePFCounter; }
			set { enablePFCounter = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public bool EnableLogging
		{
			get { return enableLogging; }
			set { enableLogging = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public TextWriter LogWriter
		{
			get { return logWriter; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Stopwatch Stopwatch
		{
			get { return this.stopwatch; }
		}
	}
}
