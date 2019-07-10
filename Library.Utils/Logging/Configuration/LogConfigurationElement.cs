
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Threading;
using WD.Library.Configuration;

namespace WD.Library.Logging
{
    /// <summary>
    /// ��־���ýڵĻ���
    /// </summary>
    /// <remarks>
    /// TypeConfigurationElement�������࣬�Զ������־������Ϣ�Ĵ���
    /// ��־���ýڵĹ������࣬�ڸ�������չ��һ����������ExtendedAttributes������һ��IDictionary���󣬿��������д��������ĸ��ֳ�ʼ������
    /// </remarks>
    public class LogConfigurationElement : TypeConfigurationElement
    {
        internal static IDictionary<string, IDictionary<string, string>> extendedAttributes = new Dictionary<string, IDictionary<string, string>>();
        private ReaderWriterLock dictionaryRWLock = new ReaderWriterLock();
        private IDictionary<string, string> instanceDic = new Dictionary<string, string>();
        private string fullPath = string.Empty;

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public LogConfigurationElement()
        {
        }

        internal LogConfigurationElement(string fullPath)
        {
            this.fullPath = fullPath;
        }

        /// <summary>
        /// �������ýڵĽ�������
        /// </summary>
        /// <param name="reader">�������ļ��н��ж�ȡ������ <seealso cref="System.Xml.XmlReader"/>��</param>
        /// <param name="serializeCollectionKey">Ϊ true����ֻ���л����ϵļ����ԣ�����Ϊ false��</param>
        protected override void  DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);

            string instanceKey = this.fullPath + "~" + this.Name;

            dictionaryRWLock.AcquireWriterLock(1000);
            try
            {
                if (extendedAttributes.Keys.Contains(instanceKey) == false)
                    extendedAttributes.Add(instanceKey, this.instanceDic);
                else
                {
                    throw new LogException(string.Format("���ýڵ����ʧ�ܣ������Ƿ���name����ͬΪ��\"{0}\"�����ý�", this.Name));
                }
            }
            finally
            {
                dictionaryRWLock.ReleaseWriterLock();
            }
        }
        
        /// <summary>
        /// ��չ����
        /// </summary>
        /// <remarks>
        /// �ֵ䣬���ڴ���Զ������ýڵ���չ����
        /// </remarks>
        public IDictionary<string, string> ExtendedAttributes
        {
            get
            {
                IDictionary<string, string> result;
                string instanceKey = this.fullPath + "~" + this.Name;

                dictionaryRWLock.AcquireReaderLock(1000);

                try
                {
                    if (extendedAttributes.TryGetValue(instanceKey, out result) == false)
                        result = new Dictionary<string, string>();

                    return result;
                }
                finally
                {
                    dictionaryRWLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// ������չ����
        /// </summary>
        /// <param name="name">���Ե����ơ�</param>
        /// <param name="value">���Ե�ֵ��</param>
        /// <returns>��������л�����������δ֪���ԣ���Ϊ true��</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (this.Properties.Contains(name))
                return base.OnDeserializeUnrecognizedAttribute(name, value);
            else
            {            
                this.instanceDic.Add(name, value);

                return true; 
            }
        }
    }
}
