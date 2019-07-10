using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using WD.Library.Properties;

namespace WD.Library.Logging
{
    /// <summary>
    /// �ı���ʽ����
    /// </summary>
    /// <remarks>
    /// LogFormatter�������࣬����ʵ���ı���ʽ��
    /// </remarks>
    public sealed class TextLogFormatter : LogFormatter
    {
        /// <summary>
        /// ��ʽģ��
        /// </summary>
        private string template;

        /// <summary>
        /// Array of token formatters.
        /// </summary>
        private ArrayList tokenFunctions;

        private const string TimeStampToken = "{timestamp}";
        private const string MessageToken = "{message}";
        private const string PriorityToken = "{priority}";
        private const string EventIdToken = "{eventid}";
        private const string EventTypeToken = "{eventtype}";
        private const string TitleToken = "{title}";
        private const string MachineToken = "{machine}";
        private const string ActivityidToken = "{activity}";
        private const string NewLineToken = "{newline}";
        private const string TabToken = "{tab}";
        private const string StackTraceToken = "{stacktrace}";

        /// <summary>
        ///  ȱʡ���캯��
        /// </summary>
        private TextLogFormatter()
        {
            RegisterTokenFunctions();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="element">���ýڶ���</param>
        /// <remarks>
        /// ����������Ϣ������TextLogFormatter����
        /// </remarks>
        public TextLogFormatter(LogConfigurationElement element)
            : base(element.Name)
        {
            if (element.ExtendedAttributes.TryGetValue("template", out this.template) == false)
                this.template = Resource.DefaultTextFormat;

            RegisterTokenFunctions();
        }

        /// <summary>
        ///  ���캯��
        /// </summary>
        /// <param name="name">Formatter����</param>
        /// <param name="template">��ʽģ��</param>
        /// <remarks>
        /// ���template����Ϊ�գ���ʹ��ȱʡ��ʽģ��
        /// </remarks>
        public TextLogFormatter(string name, string template)
            : base(name)
        {
            if (false == string.IsNullOrEmpty(template))
            {
                this.template = template;
            }
            else
            {
                this.template = Resource.DefaultTextFormat;
            }

            RegisterTokenFunctions();
        }

        /// <summary>
        /// ���캯����ʹ��ȱʡ�ı���ʽ
        /// </summary>
        /// <param name="name">Formatter����</param>
        /// <remarks>
        /// �������ƺ�ȱʡ�ĸ�ʽģ�崴��TextLogFormatter����
        /// </remarks>
        public TextLogFormatter(string name)
            : this(name, Resource.DefaultTextFormat)
        {
        }

        /// <summary>
        /// ��ʽģ��
        /// </summary>
        /// <remarks>
        /// Ҫ��ʽ���ɵ�ģ���ַ���
        /// </remarks>
        public string Template
        {
            get 
            { 
                return this.template; 
            }
            set 
            { 
                this.template = value; 
            }
        }

        #region Format Implementation
        /// <summary>
        /// �ı���ʽ������
        /// </summary>
        /// <param name="log">����ʽ����LogEntity����</param>
        /// <returns>��ʽ���ɵ��ı���</returns>
        /// <remarks>
        /// ���ط�����ʵ���ı���ʽ��
        /// </remarks>
        public override string Format(LogEntity log)
        {
            return Format(CreateTemplateBuilder(), log);
        }

        /// <summary>
        /// �ı���ʽ��
        /// </summary>
        /// <param name="templateBuilder">������ʽ��ģ�崮��StringBuilder</param>
        /// <param name="log">����ʽ����LogEntity����</param>
        /// <returns>��ʽ���ɵ��ı���</returns>
        private string Format(StringBuilder templateBuilder, LogEntity log)
        {
            templateBuilder.Replace(TextLogFormatter.TimeStampToken, log.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss fff"));
			templateBuilder.Replace(TextLogFormatter.TitleToken, log.Title);
			templateBuilder.Replace(TextLogFormatter.MessageToken, log.Message);
			templateBuilder.Replace(TextLogFormatter.EventIdToken, log.EventID.ToString(Resource.Culture));
			templateBuilder.Replace(TextLogFormatter.EventTypeToken, log.LogEventType.ToString());
            templateBuilder.Replace(TextLogFormatter.StackTraceToken, log.StackTrace);
			templateBuilder.Replace(TextLogFormatter.PriorityToken, log.Priority.ToString());
			templateBuilder.Replace(TextLogFormatter.MachineToken, log.MachineName);

			templateBuilder.Replace(TextLogFormatter.ActivityidToken, log.ActivityID.ToString("D", Resource.Culture));

            FormatTokenFunctions(templateBuilder, log);

			templateBuilder.Replace(TextLogFormatter.NewLineToken, Environment.NewLine);
			templateBuilder.Replace(TextLogFormatter.TabToken, "\t");

            return templateBuilder.ToString();
        }

        /// <summary>
        /// ����ģ�幹����
        /// </summary>
        /// <returns>������ʽ��ģ�崮��StringBuilder</returns>
        private StringBuilder CreateTemplateBuilder()
        {
            StringBuilder templateBuilder =
                            new StringBuilder((this.template == null) || (this.template.Length > 0) ? this.template : Resource.DefaultTextFormat);
            return templateBuilder;
        }
        #endregion

        private void FormatTokenFunctions(StringBuilder templateBuilder, LogEntity log)
        {
			foreach (TokenFunction token in this.tokenFunctions)
            {
                token.Format(templateBuilder, log);
            }
        }

        private void RegisterTokenFunctions()
        {
			this.tokenFunctions = new ArrayList();
			this.tokenFunctions.Add(new DictionaryToken());
			this.tokenFunctions.Add(new KeyValueToken());
			this.tokenFunctions.Add(new TimeStampToken());
			this.tokenFunctions.Add(new ReflectedPropertyToken());
        }
    }
}