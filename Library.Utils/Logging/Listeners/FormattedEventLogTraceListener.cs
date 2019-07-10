using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using WD.Library.Core;

namespace WD.Library.Logging
{
    /// <summary>
    /// EventLog��������Listerner��
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase�����࣬����־����EventLog��
    /// </remarks>
    public sealed class FormattedEventLogTraceListener : FormattedTraceListenerWrapperBase
    {
        private const string DefaultLogName = "";
        private const string DefaultMachineName = ".";
        private const string DefaultSource = "";

        private string logName = DefaultLogName;
        private string source = DefaultSource;

        private string formatterName = string.Empty;

        /// <summary>
        /// �ı���ʽ����
        /// </summary>
        /// <remarks>
        /// ��Listener������ĸ�ʽ����
        /// </remarks>
        public override ILogFormatter Formatter
        {
            get
            {
                ILogFormatter formatter = base.Formatter;
                if (false == string.IsNullOrEmpty(this.formatterName))// != string.Empty)
                {
                    LogConfigurationElement formatterelement = LoggingSection.GetConfig().LogFormatterElements[this.formatterName];

                    formatter = LogFormatterFactory.GetFormatter(formatterelement);
                }

                return formatter;
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="element">LogListenerElement����</param>
        /// <remarks>
        /// ����������Ϣ����FormattedEventLogTraceListener����
        /// lang="cs" region="EventLogTraceListener Test" tittle="����Listener����"></code>
        /// </remarks>
        public FormattedEventLogTraceListener(LogListenerElement element)
        {
            this.formatterName = element.LogFormatterName;
            this.Name = element.Name;

            if (element.ExtendedAttributes.TryGetValue("logName", out this.logName) == false)
				this.logName = FormattedEventLogTraceListener.DefaultLogName;

            if (element.ExtendedAttributes.TryGetValue("source", out this.source) == false)
				this.source = string.IsNullOrEmpty(this.logName) ? FormattedEventLogTraceListener.DefaultSource : this.logName;

            this.SlaveListener = new EventLogTraceListener();
        }
        
        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        /// <remarks>
        /// ��EventLogTraceListener��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener()
            : base(new EventLogTraceListener())
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="formater">ILogFormatterʵ��</param>
        /// <remarks>
        /// ��EventLogTraceListener��Formatter��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener(ILogFormatter formater)
            : base(new EventLogTraceListener(), formater)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="eventLog">EventLog����</param>
        /// <remarks>
        /// ��EventLogTraceListener(EventLog)��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog)
            : base(new EventLogTraceListener(eventLog))
        {
            this.source = eventLog.Source;            
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="eventLog">EventLog����</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        /// <remarks>
        /// ��EventLogTraceListener(EventLog)��Formatter��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog, ILogFormatter formatter)
            : base(new EventLogTraceListener(eventLog), formatter)
        {
        }

        /// <summary>
        /// ���캯������EventLogTraceListener(EventLog)��Formatter��ʼ��һ��ʵ������
        /// </summary>
        /// <param name="source">EventLog�е��¼���Դ</param>
        /// <param name="logName">EventLog�е���־����</param>
        /// <param name="machineName">��¼�¼���־�Ļ�������</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FormattedEventLogTraceListener(string source, string logName, string machineName, ILogFormatter formatter)
            : base(new EventLogTraceListener(new EventLog(logName, NormalizeMachineName(machineName), source)), formatter)
        {
        }

        /// <summary>
        /// �Ƿ��̰߳�ȫ����EventLogTraceListenerΪtrue
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// ���ط�����д���ļ�
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="logEventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="eventID">�¼�����ֵ��ʶ��</param>
        /// <param name="data">Ҫ��¼����־����</param>
        /// <remarks>
        /// lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, object data)
        {
            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;
                
                //ȡLogEntity�����Source���ԣ����Ϊ����ȡ��Դ�����õ�ȱʡֵ
                string sourceName = string.IsNullOrEmpty(logData.Source) ? this.source : logData.Source;

                string registerLogName = RegisterSourceToLogName(sourceName, this.logName);

                EventLog eventlog = new EventLog(registerLogName, FormattedEventLogTraceListener.DefaultMachineName, sourceName);

                //if (eventlog.OverflowAction == OverflowAction.OverwriteOlder && eventlog.MinimumRetentionDays > 3)
                //    eventlog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);

                //if (eventlog.MaximumKilobytes < 1024)
                //    eventlog.MaximumKilobytes = 2048;

                (this.SlaveListener as EventLogTraceListener).EventLog = eventlog;
            }
          
            base.TraceData(eventCache, source, logEventType, eventID, data);
        }

        #region ���õ����乹�캯��
        ///// <summary>
        ///// ���캯������EventLogTraceListener(string)��ʼ��һ��ʵ������
        ///// <see cref="EventLogTraceListener"/> initialized with a source name.
        ///// </summary>
        ///// <param name="source">The source name for the wrapped listener.</param>
        //public FormattedEventLogTraceListener(string source)
        //    : base(new EventLogTraceListener(source))
        //{
        //}

        ///// <summary>
        ///// ���캯������EventLogTraceListener(string)��Formatter��ʼ��һ��ʵ������
        ///// </summary>
        ///// <param name="source">The source name for the wrapped listener.</param>
        ///// <param name="formatter">The formatter for the wrapper.</param>
        //public FormattedEventLogTraceListener(string source, ILogFormatter formatter)
        //    : base(new EventLogTraceListener(source), formatter)
        //{
        //}

        ///// <summary>
        ///// ���캯������EventLogTraceListener(EventLog)��Formatter��ʼ��һ��ʵ������
        ///// </summary>
        ///// <param name="source">The source name for the wrapped listener.</param>
        ///// <param name="logName">The name of the event log.</param>
        ///// <param name="formatter">The formatter for the wrapper.</param>
        //public FormattedEventLogTraceListener(string source, string logName, ILogFormatter formatter)
        //    : base(new EventLogTraceListener(new EventLog(logName, DefaultMachineName, source)), formatter)
        //{
        //}
        #endregion

        private static string NormalizeMachineName(string machineName)
        {
			return string.IsNullOrEmpty(machineName) ? FormattedEventLogTraceListener.DefaultMachineName : machineName;
        }

        /// <summary>
        /// ע����־��Դ����־���Ƶ�ӳ���ϵ
        /// </summary>
        /// <param name="source">��Դ</param>
        /// <param name="logName">��־����</param>
        private string RegisterSourceToLogName(string source, string logName)
        {
            string registeredLogName = logName;

            EventSourceCreationData creationData = new EventSourceCreationData(source, logName);
            
            if (EventLog.SourceExists(source))
            {
                string originalLogName = EventLog.LogNameFromSourceName(source, FormattedEventLogTraceListener.DefaultMachineName);
                
                //sourceע�����־���ƺ�ָ����logName��һ�£��Ҳ�����source����
                //���¼���־Դ��System��������־���ƣ�����ɾ����[System.InvalidOperationException]��
                if (string.Compare(logName, originalLogName, true) != 0 && string.Compare(source, originalLogName, true) != 0)
                {
                    //ɾ�����еĹ�������ע��
                    EventLog.DeleteEventSource(source, FormattedEventLogTraceListener.DefaultMachineName);
                    EventLog.CreateEventSource(creationData);
                }
                else
                    registeredLogName = originalLogName;
            }
            else 
                //sourceδ�ڸ÷�������ע���
                EventLog.CreateEventSource(creationData);

            return registeredLogName;
        }

    }
}
