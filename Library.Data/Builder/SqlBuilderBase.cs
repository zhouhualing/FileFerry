using System;
using System.Collections.Generic;
using System.Text;

namespace WD.Library.Data.Builder
{
    /// <summary>
    /// ISqlBuilder��ʵ�ֻ���
    /// </summary>
    public abstract class SqlBuilderBase : ISqlBuilder
    {
        #region ISqlBuilder ��Ա
        /// <summary>
        /// ��鲢�޸����ű��
        /// </summary>
        /// <param name="data">��Ҫ�����ַ���</param>
        /// <param name="addQuotation">�Ƿ����</param>
        /// <returns>���ؼ�����ַ���</returns>
        public virtual string CheckQuotationMark(string data, bool addQuotation)
        {
            string result = data.Replace("'", "''");

            if (addQuotation)
                result = "'" + result + "'";

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string GetDBStringLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string GetDBByteLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string GetDBSubStringStr(string data, int start, int length, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public abstract string FormatDateTime(DateTime dt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullStr"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public abstract string DBNullToString(string data, string nullStr, bool addQuotation);

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBCurrentTimeFunction
        { get;}

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStrConcatSymbol
        {get;}

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStatementBegin
        { get;}

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStatementEnd
        { get;}

        /// <summary>
        /// 
        /// </summary>
        public abstract string DBStatementSeperator
        { get;}

        #endregion

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="data">��Ҫ�������ַ���</param>
        /// <param name="addQuotation">�������</param>
        /// <returns></returns>
        protected virtual string AddQuotation(string data, bool addQuotation)
        {
            string result = data;

            if (addQuotation)
                result = CheckQuotationMark(data, addQuotation);

            return result;
        }
    }
}
