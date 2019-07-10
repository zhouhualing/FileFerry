using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Data;
using System.Data.Common;
using System.Web;

using WD.Library.Core;
using WD.Library.Data.Properties;

namespace WD.Library.Data
{
    /// <summary>
    /// Generic database processing context.
    /// <remarks>
    /// <list type="bullet">
    ///     <item>this context is attatch to current HttpContext(web app) or Thread CurrentContext property.</item>
    ///     <item>the primary goal is to harmonize the Transaction management in a call stack.</item>
    ///     <item>itself could be disposed automatically.</item>
    /// </list>
    /// </remarks>
    /// </summary>
    public abstract class DbContext : IDisposable
    {
        private bool autoClose = true;

        /// <summary>
        /// ��ȡһ��DbContext����
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="autoClose">�Ƿ��Զ��ر�</param>
        /// <returns>DbContext����</returns>
        public static DbContext GetContext(string name, bool autoClose)
        {
			//�õ�ӳ�����������
			name = DbConnectionMappingContext.GetMappedConnectionName(name);

            DbProviderFactory factory = DbConnectionManager.GetDbProviderFactory(name);

            DbConnectionStringBuilder csb = factory.CreateConnectionStringBuilder();

            csb.ConnectionString = DbConnectionManager.GetConnectionString(name);

            bool enlist = true;

            if (csb.ContainsKey("enlist"))
                enlist = (bool)csb["enlist"];

            DbContext result = null;

            if (enlist)
                result = new AutoEnlistDbContext(name, autoClose);
            else
                result = new NotEnlistDbContext(name, autoClose);

            result.autoClose = autoClose;

            return result;
        }

        /// <summary>
        /// ���ػ�ȡDbContext����
        /// </summary>
        /// <param name="name">��������</param>
        /// <returns></returns>
        public static DbContext GetContext(string name)
        {
            return GetContext(name, true);
        }

        #region Public ��Ա

        /// <summary>
        /// �Ƿ��Զ��ر�
        /// </summary>
        public bool AutoClose
        {
            get
            {
                return this.autoClose;
            }
            private set
            {
                this.autoClose = value;
            }
        }

        /// <summary>
        /// �������Ӷ���
        /// </summary>
        public abstract DbConnection Connection
        {
            get;
            internal protected set;
        }

        /// <summary>
        /// �����������
        /// </summary>
        public abstract DbTransaction LocalTransaction
        {
            get;
            internal protected set;
        }

        /// <summary>
        /// ������������
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
