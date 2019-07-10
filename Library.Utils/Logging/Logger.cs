using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// ��־������
    /// </summary>
    /// <remarks>
    /// ����Filters��Listeners����־������
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
        /// Logger������
        /// </summary>
        /// <remarks>
        /// Logger�����ƣ�һ��������ļ���ȡ
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
        /// ����Logger�Ƿ����
        /// </summary>
        /// <remarks>
        /// ���ø�Logger�Ƿ���õĲ���ֵ
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
        /// Listener����
        /// </summary>
        /// <remarks>
        /// �������ļ��ж�ȡ�����������û�У��򷵻س�ʼList&lt;FormattedTraceListenerBase&gt;����
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
        /// Filter����
        /// </summary>
        /// <remarks>
        /// �������ļ��ж�ȡ�������������û�У��򷵻س�ʼLogFilterPipeline����
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
        //    ExceptionHelper.CheckStringIsNullOrEmpty(loggerName, "LoggerName����Ϊ��");

        //    this._loggerName = loggerName;
        //    this._listeners = listeners;
        //    this._filters = filters;
        //}
        internal Logger(string loggerAliasName, bool enabled) : this()
        {
			ExceptionHelper.CheckStringIsNullOrEmpty(loggerAliasName, "LoggerName����Ϊ��");

			this.loggerName = loggerAliasName;
            this.enableLog = enabled;
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            foreach (FormattedTraceListenerBase listener in Listeners)
                listener.Dispose();
        }

        #region Process Log
        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="log">��д����־��¼</param>
        /// <remarks>
        /// д��־��Ϣ�ķ���
        /// lang="cs" region="Logger Write Test" tittle="д��־��Ϣ"></code>
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
                    throw new LogException("д��־��Ϣʱ������" + ex.Message, ex);
            }
            finally
            {
                this.rwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="priority">��־���ȼ�</param>
        /// <param name="eventId">��־�¼�ID</param>
        /// <param name="logEventType">��־�¼�����</param>
        /// <param name="title">��־����</param>
        /// <remarks>
        /// ���ݴ��ݵĲ���������LogEntity���󣬲�д��ý��
        /// lang="cs" region="Logger Write Test" tittle="д��־��Ϣ"></code>
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
                            string msg = string.Format("{1}[{0:yyyy-MM-dd HH:mm:ss}] \n �����ջΪ��{2}", DateTime.Now, ex.Message, ex.StackTrace);
                            
                            EventLog.WriteEntry("Application", "д�¼��鿴���쳣��" + msg, EventLogEntryType.Warning);
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