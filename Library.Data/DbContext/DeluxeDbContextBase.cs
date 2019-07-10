#region using
using System;
using System.Web;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Data.Common;
using System.Transactions;
using System.Collections.Generic;
using WD.Library.Core;
using WD.Library.Data.Properties;
#endregion

namespace WD.Library.Data
{
    internal abstract class DeluxeDbContextBase : DbContext
    {
        #region Protected type(Class)
        protected class ReferenceConnection
        {
            private DbConnection connection = null;
            private int referenceCount = 0;
            private string name = string.Empty;

            /// <summary>
            /// ��������
            /// </summary>
            /// <param name="connName">��������</param>
            /// <param name="conn">���ݿ����Ӷ���</param>
            public ReferenceConnection(string connName, DbConnection conn)
            {
                this.name = connName;
                this.connection = conn;
                this.referenceCount++;
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public DbConnection Connection
            {
                get { return this.connection; }
                set { this.connection = value; }
            }

            public int ReferenceCount
            {
                get { return this.referenceCount; }
                set { this.referenceCount = value; }
            }
        }

        protected class Connections : Dictionary<string, ReferenceConnection>
        {
        }

        protected class GraphWithoutTransaction : Connections
        {
        }
        #endregion

        #region Private const and field
        /// <summary>
        /// Private const
        /// <remarks>
        ///     db context key name prefix
        /// </remarks>
        /// </summary>
        private const string NamePrefix = "DeluxeWorks.Context";

        /// <summary>
        /// Private const
        /// <remarks>
        ///     the context key name postfix that doesn't exists in transaction.
        /// </remarks>
        /// </summary>
        private const string NamePostfixWithoutTransaction = ".GraphWithoutTx";

        /// <summary>
        /// Current context entity management target (without transaction support).
        /// <remarks>
        ///     the Key type is a logical database name
        /// </remarks>
        /// </summary>
        [ThreadStatic]
        private static GraphWithoutTransaction graphWithoutTx = null;

        /// <summary>
        /// Logical database name
        /// </summary>
        private string name;

        /// <summary>
        /// ���ݿ��߼�����
        /// </summary>
        /// <remarks>
        /// ���ݿ����Ӷ��������������GenericDatabaseFactory����
        /// </remarks>
        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// ��ǰ�������Ƿ������ӵĴ�����
        /// </summary>
        protected bool IsConnectionCreator
        {
            get
            {
                return this.isConnectionCreator;
            }
            set
            {
                this.isConnectionCreator = value;
            }
        }

        /// <summary>
        /// Internal connection object for non-transactional context
        /// </summary>
        private DbConnection connection = null;

        /// <summary>
        /// Internal transaction object for non-transactional context
        /// </summary>
        private DbTransaction localTransaction = null;

        ///// <summary>
        ///// Key of current context exists in HttpContext or Thread
        ///// </summary>
        //private string contextKey;

        /// <summary>
        /// Whether exists a transaction in constructor
        /// </summary>
        private bool isInTransaction = false;

        /// <summary>
        /// Whether this context created a new DbConnection instance.
        /// </summary>
        private bool isConnectionCreator = false;
        #endregion

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="autoClose">�漴�ر�</param>
        public DeluxeDbContextBase(string name, bool autoClose)
        {
            InitDbContext(name, autoClose);
        }

        #region Public property
        /// <summary>
        /// Current context connection.
        /// </summary>
        public override DbConnection Connection
        {
            get
            {
                return this.connection;
            }
            internal protected set
            {
                this.connection = value;
            }
        }

        public override DbTransaction LocalTransaction
        {
            get
            {
                return this.localTransaction;
            }
            protected internal set
            {
                this.localTransaction = value;
            }
        }

        /// <summary>
        /// �Ƿ��������й����������
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                return isInTransaction;
            }
        }
        #endregion

        #region IDisposable ��Ա
        /// <summary>
        /// ɾ����������
        /// <remarks>
        ///     the dispose process is varied according to whether a Current Transaction exists.
        /// <list type="bullet">
        /// </list>
        /// </remarks>
        /// </summary>
        public override void Dispose()
        {
            ReleaseConnection();

            // not transactional operation
            if (this.AutoClose)
            {
                RemoveConnection();
            }
        }

        #endregion

        #region Protected methods
        /// <summary>
        /// �����ʼ��
        /// </summary>
        protected virtual void OnInitWithTransaction()
        {
        }
        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        protected virtual DbConnection OnGetConnectionWithTransaction(Transaction ts)
        {
            return null;
        }

        protected virtual GraphWithoutTransaction GraphWithoutTx
        {
            get
            {
                return StaticGraphWithoutTx;
            }
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <returns></returns>
        protected static GraphWithoutTransaction StaticGraphWithoutTx
        {
            get
            {
                GraphWithoutTransaction result;
                string itemKey = NamePrefix + NamePostfixWithoutTransaction;

                if (EnvironmentHelper.Mode == InstanceMode.Web)
                    result = (GraphWithoutTransaction)HttpContext.Current.Items[itemKey];
                else
                    result = DeluxeDbContextBase.graphWithoutTx;

                if (result == null)
                {
                    result = new GraphWithoutTransaction();

                    if (EnvironmentHelper.Mode == InstanceMode.Web)
                        HttpContext.Current.Items[itemKey] = result;
                    else
                        DeluxeDbContextBase.graphWithoutTx = result;
                }

                return result;
            }
        }

        /// <summary>
        /// �ͷ�����
        /// </summary>
        protected void ReleaseConnection()
        {
            Connections connections = GraphWithoutTx;

            lock (connections)
            {
                ReferenceConnection rConnection = null;

                if (connections.TryGetValue(this.Name, out rConnection))
                    rConnection.ReferenceCount--;
            }
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        protected void RemoveConnection()
        {
            Connections connections = GraphWithoutTx;

            lock (connections)
            {
                ReferenceConnection rConnection = null;

                if (connections.TryGetValue(this.Name, out rConnection))
                {
                    if (rConnection.ReferenceCount == 0)
                    {
                        if (this.IsInTransaction == false)
                        {
                            try
                            {
                                if (rConnection.Connection.State != ConnectionState.Closed)
                                    rConnection.Connection.Close();

                                WriteTraceInfo(rConnection.Connection.DataSource + "." + rConnection.Connection.Database
                                            + "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]",
                                            " Close Connection ");
                            }
                            finally
                            {
                                connections.Remove(this.Name);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// ��ʼ�������������
        /// </summary>
        /// <param name="name">������������</param>
        /// <param name="autoClose">�Ƿ��Զ��ر�</param>
        private void InitDbContext(string name, bool autoClose)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "name");

            this.name = name;

            // current execution without transaction
            if (Transaction.Current == null)
            {
                this.isInTransaction = false;
            }
            else
            {
                this.isInTransaction = true;
                Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler(CompleteIndividualTransaction);
                OnInitWithTransaction();
            }

            this.connection = CreateConnection(name, out this.isConnectionCreator);
        }

        /// <summary>
        /// ����һ������
        /// <remarks>
        ///     the connection retrieve process is as the following procedure:
        /// <list type="bullet">
        ///     <item>if no transaction exists, this method create and return a new DbConnection instance</item>
        ///     <item>if transaction exists, this method should return a cached DbConnection instance</item>
        /// </list>
        /// <param name="isConnectionCreator">������������������Ƿ񴴽��ɹ�</param>
        /// <param name="name">���ݿ���������</param>
        /// </remarks>
        /// </summary>
        private DbConnection CreateConnection(string name, out bool isConnectionCreator)
        {
            DbConnection connection;
            isConnectionCreator = false;

            // non-transactional operation
            GraphWithoutTransaction connections = GraphWithoutTx;

            if (Transaction.Current == null)
                connection = GetConnectionWithoutTx(name);
            else
                connection = OnGetConnectionWithTransaction(Transaction.Current);

            if ((connection != null) && (connection.State != ConnectionState.Open))
            {
                if (string.IsNullOrEmpty(connection.ConnectionString))
                    connection.ConnectionString = DbConnectionManager.GetConnectionString(name);

                connection.Open();

                WriteTraceInfo(connection.DataSource + "." + connection.Database
                    + "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff") + "]", " Open Connection ");
            }

            return connection;
        }

        protected virtual void OnTransactionCompleted(TransactionEventArgs args)
        {
        }

        /// <summary>
        /// �������Ƶõ�GraphWithoutTx�����Ӷ�����������ڣ��Զ�����һ���µ����Ӷ���û��Open��
        /// </summary>
        /// <param name="connName">���ݿ���������</param>
        /// <returns>GraphWithoutTx�����Ӷ���</returns>
        protected DbConnection GetConnectionWithoutTx(string connName)
        {
            ReferenceConnection refConnection = GetRefConnectionWithoutTx(connName);

            DbConnection connection = null;

            if (refConnection != null)
                connection = refConnection.Connection;

            return connection;
        }

        /// <summary>
        /// �������Ƶõ���ǰ���õ�����
        /// </summary>
        /// <param name="connName">���ݿ���������</param>
        /// <returns>���Ӷ���</returns>
        protected ReferenceConnection GetRefConnectionWithoutTx(string connName)
        {
            ReferenceConnection refConnection = null;

            GraphWithoutTransaction connections = GraphWithoutTx;
            lock (connections)
            {
                if (connections.TryGetValue(connName, out refConnection) == false)
                {
                    DbConnection connection = DbConnectionManager.GetConnection(connName);
                    this.isConnectionCreator = true;

                    refConnection = new ReferenceConnection(connName, connection);
                    connections.Add(connName, refConnection);
                }
                else
                    refConnection.ReferenceCount++;
            }

            return refConnection;
        }

        protected static void WriteTraceInfo(string info, string category)
        {
#if DELUXEWORKSTEST
			Trace.WriteLine(info, category);
#endif
        }

        protected static void WriteTraceInfo(string info)
        {
#if DELUXEWORKSTEST
			Trace.WriteLine(info);
#endif
        }
        #endregion

        #region Event handler
        /// <summary>
        /// Event handler when transaction has completed.
        /// <remarks>
        ///     clear all associated DbConnection and remove associated graph element.
        /// </remarks>
        /// </summary>
        private void CompleteIndividualTransaction(object sender, TransactionEventArgs args)
        {
            WriteTraceInfo("CompleteIndividualTransaction ManagedThreadId :"
                + Thread.CurrentThread.ManagedThreadId.ToString());

            OnTransactionCompleted(args);
        }
        #endregion
    }
}