using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// 日志处理类
    /// </summary>
    /// <remarks>
    /// 包含Filters、Listeners的日志处理类
    /// </remarks>
    public sealed class Logger : IDisposable
    {
        private string loggerName = string.Empty;
        private List<FormattedTraceListenerBase> listeners = null;
        private LogFilterPipeline filters = null;
        private bool enableLog = true;

        private ReaderWriterLock rwLock = new ReaderWriterLock();
        private const int DefaultLockTimeout = 3000;
        //private static object sychronRoot = new object();

        /// <summary>
        /// Logger的名称
        /// </summary>
        /// <remarks>
        /// Logger的名称，一般从配置文件读取
        /// </remarks>
        public string Name
        {
            get
            {
                return this.loggerName;
            }
            set
            {
                this.loggerName = value;
            }
        }

        /// <summary>
        /// 表明Logger是否可用
        /// </summary>
        /// <remarks>
        /// 设置该Logger是否可用的布尔值
        /// </remarks>
        public bool EnableLog
        {
            get
            {
                return this.enableLog;
            }
            set
            {
                this.enableLog = value;
            }
        }

        /// <summary>
        /// Listener集合
        /// </summary>
        /// <remarks>
        /// 从配置文件中读取创建对象；如果没有，则返回初始List&lt;FormattedTraceListenerBase&gt;对象
        /// </remarks>
        public List<FormattedTraceListenerBase> Listeners
        {
            get
            {
                if (string.IsNullOrEmpty(this.loggerName) == false && LoggingSection.GetConfig().Loggers[Name] != null)
                    this.listeners = LoggingSection.GetConfig().Loggers[Name].LogListeners;
                else
                {
                    if (this.listeners == null)
                        this.listeners = new List<FormattedTraceListenerBase>();
                }

                return this.listeners;
            }
        }

        /// <summary>
        /// Filter集合
        /// </summary>
        /// <remarks>
        /// 从配置文件中读取、创建对象；如果没有，则返回初始LogFilterPipeline对象
        /// </remarks>
#if DELUXEWORKSTEST
        public LogFilterPipeline FilterPipeline
#else
        internal LogFilterPipeline FilterPipeline
#endif
        {
            get
            {
                if (string.IsNullOrEmpty(this.loggerName) == false && LoggingSection.GetConfig().Loggers[Name] != null)
                    this.filters = LoggingSection.GetConfig().Loggers[Name].LogFilters;
                else
                {
                    if (this.filters == null)
                        this.filters = new LogFilterPipeline();
                }

                return this.filters;
            }
        }

        internal Logger()
        {
        }

        //internal Logger(string loggerName, LogFilterPipeline filters, List<FormattedTraceListenerBase> listeners)
        //{
        //    ExceptionHelper.CheckStringIsNullOrEmpty(loggerName, "LoggerName不能为空");

        //    this._loggerName = loggerName;
        //    this._listeners = listeners;
        //    this._filters = filters;
        //}
        internal Logger(string loggerAliasName, bool enabled) : this()
        {
			ExceptionHelper.CheckStringIsNullOrEmpty(loggerAliasName, "LoggerName不能为空");

			this.loggerName = loggerAliasName;
            this.enableLog = enabled;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (FormattedTraceListenerBase listener in Listeners)
                listener.Dispose();
        }

        #region Process Log
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">待写的日志记录</param>
        /// <remarks>
        /// 写日志信息的方法
        /// lang="cs" region="Logger Write Test" tittle="写日志信息"></code>
        /// </remarks>
        public void Write(LogEntity log)
        {
            try
            {
				this.rwLock.AcquireReaderLock(Logger.DefaultLockTimeout);

                if (this.enableLog && this.FilterPipeline.IsMatch(log))
                {
                    ProcessLog(log);
                }
            }
            catch (LogException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex is LogException)
                    throw;
                else
                    throw new LogException("写日志信息时出错：" + ex.Message, ex);
            }
            finally
            {
                this.rwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="priority">日志优先级</param>
        /// <param name="eventId">日志事件ID</param>
        /// <param name="logEventType">日志事件类型</param>
        /// <param name="title">日志标题</param>
        /// <remarks>
        /// 根据传递的参数，构建LogEntity对象，并写入媒介
        /// lang="cs" region="Logger Write Test" tittle="写日志信息"></code>
        /// </remarks>
        public void Write(string message, LogPriority priority, int eventId,
                                TraceEventType logEventType, string title)
        {
            LogEntity log = new LogEntity(message);

            log.Priority = priority;
            log.EventID = eventId;
            log.LogEventType = logEventType;
            log.Title = title;

            Write(log);
        }

        private void ProcessLog(LogEntity log)
        {
            //if (!ShouldTrace(log.LogEventType)) 
            //    return;
            
            TraceEventCache cache = new TraceEventCache();

            //bool isTransfer = logEntry.Severity == TraceEventType.Transfer && logEntry.RelatedActivityId != null;

            foreach (TraceListener listener in this.Listeners)
            {
                try
                {
                    if (false == listener.IsThreadSafe)
                    {
                        Monitor.Enter(listener);//Monitor.Enter(sychronRoot);
                    }

                    listener.TraceData(cache, log.Source, log.LogEventType, log.EventID, log);

                    listener.Flush();
                }
                catch (Exception ex)
                {
                    if (listener is FormattedEventLogTraceListener)
                    {
                        try
                        {
                            string msg = string.Format("{1}[{0:yyyy-MM-dd HH:mm:ss}] \n 错误堆栈为：{2}", DateTime.Now, ex.Message, ex.StackTrace);
                            
                            EventLog.WriteEntry("Application", "写事件查看器异常：" + msg, EventLogEntryType.Warning);
                        }
                        catch(Exception)
                        {
                        }
                    }
                    else
                        throw;
                }
                finally
                {
                    if (false == listener.IsThreadSafe)
                    {
                        Monitor.Exit(listener); //Monitor.Exit(sychronRoot);
                    }
                }
            }
        }

        #endregion

        //private bool ShouldTrace(TraceEventType eventType)
        //{
        //    //TODO:
        //    return true;
        //}
    }
}
