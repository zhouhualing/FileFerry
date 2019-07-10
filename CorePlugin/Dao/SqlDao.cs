using IBatisNet.Common;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WD.CorePlugin;
using WD.Library.Core;
using WD.Library.Reflection;

namespace WD.CorePlugin
{
    public class SqlDao : IBaseDao
    {
        public virtual MapperBase Mapper
        {
            get; set;
        }
        public SqlDao()
        {
            Mapper = new SqlServerMapper();
        }
        public SqlDao(string mapperName)
        {
            Mapper = new SqlServerMapper(mapperName);
        }

        public string GetRuntimeSql(ISqlMapper sqlMapper, string statementName, object paramObject)
        {
            string result = string.Empty;
            try
            {
                IMappedStatement statement = sqlMapper.GetMappedStatement(statementName);
                if (!sqlMapper.IsSessionStarted)
                {
                    sqlMapper.OpenConnection();
                }
                RequestScope scope = statement.Statement.Sql.GetRequestScope(statement, paramObject, sqlMapper.LocalSession);
                result = scope.PreparedStatement.PreparedSql;
            }
            catch (Exception ex)
            {
                ExceptionHelper.NullCheck(Mapper, "Mapper could not be empty");
                Mapper.Close();
                result = "获取SQL语句出现异常:" + ex.Message;
            }
            return result;
        }

        public IList<T> ExecuteQueryList<T>(string statement, IDaoObject entryCriteria) where T : class
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var queryList = Mapper.Get().QueryForList(statement, table);
                if (queryList == null)
                {
                    return null;
                }
                var list = new List<T>();
                foreach (IDictionary dbEntry in queryList)
                {
                    var entryDataObject = new DaoObject();
                    entryDataObject.SetDictionary(dbEntry);
                    var entry = CreateNewDataObjectInstance<T>(entryDataObject);
                    if (entry is IDaoObject)
                    {
                        (entry as IDaoObject).SetDictionary(dbEntry);
                    }
                    list.Add(entry);
                }
                return list;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }


        public DataSet ExecuteQueryDataSet(ISqlMapper sqlMapper, string statementName, object paramObject)
        {
            DataSet dataset = new DataSet();
            try
            {
                IMappedStatement statement = sqlMapper.GetMappedStatement(statementName);
                if (!sqlMapper.IsSessionStarted)
                {
                    sqlMapper.OpenConnection();
                }
                RequestScope scope = statement.Statement.Sql.GetRequestScope(statement, paramObject, sqlMapper.LocalSession);
                statement.PreparedCommand.Create(scope, sqlMapper.LocalSession, statement.Statement, paramObject);
                sqlMapper.LocalSession.CreateDataAdapter(scope.IDbCommand).Fill(dataset);
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
            return dataset;
        }
        public bool ExecuteInsert(string statement,object entry)
        {
            try
            {
                ExceptionHelper.NullCheck(entry);
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);
                Mapper.Get().Insert(statement, table);

                return true;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public bool ExecuteInsertGeneric<T>(string stmtId,T instance) where T : class
        {
            try
            {
                ExceptionHelper.NullCheck(instance);
                Mapper.Get().Insert(stmtId, instance);
                return true;
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }            
        }


        public bool ExecuteInsert(System.Collections.Hashtable hash, string stmtId)
        {
            try
            {
                Mapper.Get().Insert(stmtId, hash);
                return true;
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public T ExecuteSelectObject<T>(System.Collections.Hashtable hash, string stmtId)
        {
            try
            {
                T result = Mapper.Get().QueryForObject<T>(stmtId, hash);
                return result;
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public IList<T> ExecuteSelectList<T>(System.Collections.Hashtable hash, string stmtId)
        {
            try
            {
                IList<T> result = Mapper.Get().QueryForList<T>(stmtId, hash);
                return result;
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public int ExecuteSelectCount(System.Collections.Hashtable hash, String stmtId)
        {
            try
            {
                int result = Mapper.Get().QueryForObject<int>(stmtId, hash);
                return result;
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public void ExecuteUpdate<T>(T instance, string stmtId) where T:class
        {
            try
            {
                ExceptionHelper.NullCheck(instance);
                Mapper.Get().Update(stmtId, instance);
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public int ExecuteUpdate(System.Collections.Hashtable hash, String stmtId)
        {
            try
            {
               return Mapper.Get().Update(stmtId, hash);
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public int ExecuteUpdate(string statement, object entry)
        {
            try
            {
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);
                return Mapper.Get().Update(statement, table);
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public void ExecuteDelete(System.Collections.Hashtable hash, String stmtId)
        {
            try
            {
                Mapper.Get().Delete(stmtId, hash);
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }
        public DataSet ExecuteQueryDataSet(System.Collections.Hashtable hash, String stmtId)
        {
            try
            {
                return ExecuteQueryDataSet(Mapper.Get(), stmtId, hash);
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public void BeginTransaction()
        {
            try
            {
                IDalSession session = Mapper.Get().LocalSession;
                if (session != null && session.Transaction == null)
                {
                    session.BeginTransaction(IsolationLevel.ReadCommitted);
                }
                else
                {
                    Mapper.Get().BeginTransaction(IsolationLevel.ReadCommitted);
                }
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (IsTransactionStart())
                {
                    Mapper.Get().CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }

        }

        public void RollBackTransaction()
        {
            try
            {
                if (IsTransactionStart())
                {
                    Mapper.Get().RollBackTransaction();
                }
            }
            catch (Exception ex)
            {
                Mapper.Close();
                throw ex;
            }
        }



        public int ExecuteDelete(string statement, object entry)
        {
            try
            {
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);
                var result = Mapper.Get().Delete(statement, table);

                return result;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public T ExecuteQueryObject<T>(string statement, object entry) where T : class
        {
            try
            {
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);

                var queryObject = (IDictionary)Mapper.Get().QueryForObject(statement, table);
                if (queryObject == null)
                {
                    return null;
                }
                return CreateModel<T>(queryObject);
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<T> ExecuteQueryList<T>(string statement, object entry) where T : class
        {
            try
            {
                var table = GetHashtable(entry);
                var queryList = Mapper.Get().QueryForList(statement, table);
                if (queryList == null)
                {
                    return null;
                }
                var list = new List<T>();
                foreach (IDictionary dbEntry in queryList)
                {                    
                    list.Add(CreateModel<T>(dbEntry));
                }
                return list;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public string ExecuteQuerySql(string statement, object entry)
        {
            var table = entry == null ? null : PropertyUtility.GetProperties(entry);
            string result = string.Empty;
            try
            {
                IMappedStatement stmt = Mapper.Get().GetMappedStatement(statement);
                if (!Mapper.Get().IsSessionStarted)
                {
                    Mapper.Get().OpenConnection();
                }
                RequestScope scope = stmt.Statement.Sql.GetRequestScope(stmt, table, Mapper.Get().LocalSession);
                result = scope.PreparedStatement.PreparedSql;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                result = "获取SQL语句出现异常:" + ex.Message;
            }
            return result;
        }

        public int ExecuteQueryValueToInt(string statement, object entry)
        {
            try
            {
                var table = PropertyUtility.GetProperties(entry);
                var queryCount = Mapper.Get().QueryForObject(statement, table);
                if (queryCount == null)
                {
                    return 0;
                }
                return (int)queryCount;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public decimal ExecuteQueryValueToDecimal(string statement, object entry)
        {
            try
            {
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);
                var queryCount = Mapper.Get().QueryForObject(statement, table);
                if (queryCount == null)
                {
                    return 0;
                }
                return (decimal)queryCount;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<int> ExecuteQueryListToInt(string statement, object entry)
        {
            try
            {
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);
                var queryList = Mapper.Get().QueryForList(statement, table);
                if (queryList == null || queryList.Count == 0)
                {
                    return null;
                }

                IList<int> _list = new List<int>();
                foreach (var obj in queryList)
                {
                    if (obj == null)
                    {
                        _list.Add(0);
                    }
                    else
                    {
                        _list.Add((int)obj);
                    }
                }

                return _list;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<decimal> ExecuteQueryListToDecimal(string statement, object entry)
        {
            try
            {
                var table = entry == null ? null : PropertyUtility.GetProperties(entry);
                var queryList = Mapper.Get().QueryForList(statement, table);
                if (queryList == null || queryList.Count == 0)
                {
                    return null;
                }

                IList<decimal> _list = new List<decimal>();
                foreach (var obj in queryList)
                {
                    if (obj == null)
                    {
                        _list.Add(0);
                    }
                    else
                    {
                        _list.Add((decimal)obj);
                    }
                }

                return _list;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public DataSet ExecuteQueryDataSet(string statement, object entry)
        {
            var table = entry == null ? null : PropertyUtility.GetProperties(entry);

            bool isSessionLocal = false;
            ISqlMapSession session = Mapper.Get().LocalSession;

            DataSet ds = new DataSet(statement);
            if (!Mapper.Get().IsSessionStarted)
            {
                Mapper.Get().OpenConnection();
            }
            if (session == null)
            {
                session = Mapper.Get().CreateSqlMapSession();
                isSessionLocal = true;
            }
            try
            {
                IMappedStatement stmt = Mapper.Get().GetMappedStatement(statement);
                RequestScope request = stmt.Statement.Sql.GetRequestScope(stmt, table, session);
                stmt.PreparedCommand.Create(request, session, stmt.Statement, table);
                Mapper.Get().LocalSession.CreateDataAdapter(request.IDbCommand).Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            return ds;
        }

        public string ExecuteQuerySql(string statementName, IDaoObject entryCriteria)
        {
            var table = GenerateTableForQuery(entryCriteria);
            string result = string.Empty;
            try
            {
                IMappedStatement statement = Mapper.Get().GetMappedStatement(statementName);
                if (!Mapper.Get().IsSessionStarted)
                {
                    Mapper.Get().OpenConnection();
                }
                RequestScope scope = statement.Statement.Sql.GetRequestScope(statement, table, Mapper.Get().LocalSession);
                result = scope.PreparedStatement.PreparedSql;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                result = "获取SQL语句出现异常:" + ex.Message;
            }
            return result;
        }

        public int ExecuteQueryValueToInt(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var queryCount = Mapper.Get().QueryForObject(statement, table);
                if (queryCount == null)
                {
                    return 0;
                }
                return (int)queryCount;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public decimal ExecuteQueryValueToDecimal(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var queryCount = Mapper.Get().QueryForObject(statement, table);
                if (queryCount == null)
                {
                    return 0;
                }
                return (decimal)queryCount;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<int> ExecuteQueryListToInt(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var queryList = Mapper.Get().QueryForList(statement, table);
                if (queryList == null || queryList.Count == 0)
                {
                    return null;
                }

                IList<int> _list = new List<int>();
                foreach (var obj in queryList)
                {
                    if (obj == null)
                    {
                        _list.Add(0);
                    }
                    else
                    {
                        _list.Add((int)obj);
                    }
                }

                return _list;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<decimal> ExecuteQueryListToDecimal(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var queryList = Mapper.Get().QueryForList(statement, table);
                if (queryList == null || queryList.Count == 0)
                {
                    return null;
                }

                IList<decimal> _list = new List<decimal>();
                foreach (var obj in queryList)
                {
                    if (obj == null)
                    {
                        _list.Add(0);
                    }
                    else
                    {
                        _list.Add((decimal)obj);
                    }
                }

                return _list;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }


        public DataTable ExecuteQueryDataTable(string statement, object entry)
        {
            var table = entry == null ? null : PropertyUtility.GetProperties(entry);

            bool isSessionLocal = false;
            ISqlMapSession session = Mapper.Get().LocalSession;
            DataTable dataTable = null;
            if (session == null)
            {
                session = Mapper.Get().CreateSqlMapSession();
                isSessionLocal = true;
            }
            try
            {
                IMappedStatement stmt = Mapper.Get().GetMappedStatement(statement);
                dataTable = new DataTable(statement);
                RequestScope request = stmt.Statement.Sql.GetRequestScope(stmt, table, session);
                stmt.PreparedCommand.Create(request, session, stmt.Statement, table);
                using (request.IDbCommand)
                {
                    dataTable.Load(request.IDbCommand.ExecuteReader());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            return dataTable;
        }

        public T ExecuteQueryDataTableToObject<T>(string statement, IDaoObject entryCriteria) where T : class
        {
            try
            {
                var dataTable = ExecuteQueryDataTable(statement, entryCriteria);
                if (dataTable == null || dataTable.Rows.Count == 0)
                {
                    return null;
                }

                var entryDataObject = new DaoObject();
                var dictionary = (IDictionary)new Hashtable();
                DataRow dr = dataTable.Rows[0];
                foreach (DataColumn dc in dataTable.Columns)
                {
                    dictionary.Add(dc.ColumnName, dr[dc.ColumnName]);
                    entryDataObject.SetDictionary(dictionary);
                }
                var entry = CreateNewDataObjectInstance<T>(entryDataObject);
                if (entry is IDaoObject)
                {
                    (entry as IDaoObject).SetDictionary(dictionary);
                }
                return entry;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }

        }

        public IList<T> ExecuteQueryDataTableToList<T>(string statement, IDaoObject entryCriteria) where T : class
        {
            try
            {
                var dataTable = ExecuteQueryDataTable(statement, entryCriteria);
                return CovertDateTableToList<T>(dataTable);
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public DataTable ExecuteQueryDataTable(string statementName, IDaoObject entryCriteria)
        {
            var table = GenerateTableForQuery(entryCriteria);

            bool isSessionLocal = false;
            ISqlMapSession session = Mapper.Get().LocalSession;
            DataTable dataTable = null;
            if (session == null)
            {
                session = Mapper.Get().CreateSqlMapSession();
                isSessionLocal = true;
            }
            try
            {
                IMappedStatement statement = Mapper.Get().GetMappedStatement(statementName);
                dataTable = new DataTable(statementName);
                RequestScope request = statement.Statement.Sql.GetRequestScope(statement, table, session);
                statement.PreparedCommand.Create(request, session, statement.Statement, table);
                using (request.IDbCommand)
                {
                    dataTable.Load(request.IDbCommand.ExecuteReader());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            return dataTable;
        }


        public DataSet ExecuteQueryDataSet(string statementName, IDaoObject entryCriteria)
        {
            var table = GenerateTableForQuery(entryCriteria);

            bool isSessionLocal = false;
            ISqlMapSession session = Mapper.Get().LocalSession;

            DataSet ds = new DataSet(statementName);
            if (!Mapper.Get().IsSessionStarted)
            {
                Mapper.Get().OpenConnection();
            }
            if (session == null)
            {
                session = Mapper.Get().CreateSqlMapSession();
                isSessionLocal = true;
            }
            try
            {
                IMappedStatement statement = Mapper.Get().GetMappedStatement(statementName);
                RequestScope request = statement.Statement.Sql.GetRequestScope(statement, table, session);
                statement.PreparedCommand.Create(request, session, statement.Statement, table);
                Mapper.Get().LocalSession.CreateDataAdapter(request.IDbCommand).Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            return ds;
        }

        public DataSet ExecuteQueryDataSet(string statementName, IDaoObject entryCriteria, IDictionary<string, ParameterDirection> dictParmDirection, out Hashtable outputParam)
        {
            //保存参数字段名称，必须保证跟存储过程顺序完全一致
            var paraNameList = GenerateSortedTableForQuery(entryCriteria);
            //传递的参数集合(参数名称,参数值)
            var table = GenerateTableForQuery(entryCriteria);

            DataSet ds = new DataSet();

            bool isSessionLocal = false;
            ISqlMapSession session = Mapper.Get().LocalSession;
            if (session == null)
            {
                session = Mapper.Get().CreateSqlMapSession();
                session.OpenConnection();
                isSessionLocal = true;
            }
            try
            {
                IMappedStatement statement = Mapper.Get().GetMappedStatement(statementName);
                RequestScope request = statement.Statement.Sql.GetRequestScope(statement, table, session);
                statement.PreparedCommand.Create(request, session, statement.Statement, table);

                //获取DbCommand对象
                CommandType cmdType = CommandType.StoredProcedure;
                IDbCommand cmd = session.CreateCommand(cmdType);
                cmd.CommandText = request.IDbCommand.CommandText;

                //构建存储过程对象，参数顺序必须要一致
                foreach (string de in paraNameList)
                {
                    IDbDataParameter dbParam = cmd.CreateParameter();
                    dbParam.ParameterName = de;
                    dbParam.Value = entryCriteria.GetPropertyValue(de);
                    if (dictParmDirection != null && dictParmDirection.ContainsKey(de))
                    {
                        dbParam.Direction = dictParmDirection[de];
                    }
                    cmd.Parameters.Add(dbParam);
                }

                //执行存储过程，获取DataSet
                cmd.Connection = session.Connection;
                IDbDataAdapter adapter = session.CreateDataAdapter(cmd);
                adapter.Fill(ds);

                //给输出参数赋值
                outputParam = new Hashtable();
                foreach (IDataParameter parameter in cmd.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Output)
                    {
                        outputParam[parameter.ParameterName] = parameter.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isSessionLocal)
                {
                    session.CloseConnection();
                }
            }
            return ds;
        }

        public T ExecuteQueryObject<T>(string statement, IDaoObject entryCriteria) where T : class
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var queryObject = (IDictionary)Mapper.Get().QueryForObject(statement, table);
                if (queryObject == null)
                {
                    return null;
                }
                var entryDataObject = new DaoObject();
                entryDataObject.SetDictionary(queryObject);
                var entry = CreateNewDataObjectInstance<T>(entryDataObject);
                if (entry is IDaoObject)
                {
                    (entry as IDaoObject).SetDictionary(queryObject);
                }
                return entry;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }


        public int ExecuteUpdateEntry(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                return Mapper.Get().Update(statement, table);
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public int ExecuteDeleteEntry(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                var result = Mapper.Get().Delete(statement, table);

                return result;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public bool ExecuteInsert(string statement, IDaoObject entryCriteria)
        {
            try
            {
                var table = GenerateTableForQuery(entryCriteria);
                Mapper.Get().Insert(statement, table);

                return true;
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }


        public T ExecuteQueryDataTableToObject<T>(string statement, object entry) where T : class
        {
            try
            {
                return ExecuteQueryDataTableToList<T>(statement,entry).FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<T> ExecuteQueryDataTableToList<T>(string statement, object entry) where T : class
        {
            try
            {
                var dataTable = ExecuteQueryDataTable(statement, entry);
                return CovertDateTableToList<T>(dataTable);
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        public IList<T> ExecuteQueryDataTableToList<T>(DataTable dt) where T : class
        {
            try
            {
                return CovertDateTableToList<T>(dt);
            }
            catch (Exception ex)
            {
                if (!IsTransactionStart())
                {
                    Mapper.Close();
                }
                throw ex;
            }
        }

        private bool IsTransactionStart()
        {
            if (Mapper.Get().LocalSession != null && Mapper.Get().LocalSession.IsTransactionStart)
            {
                return true;
            }
            return false;
        }

        private T CreateModel<T>(IDictionary dictionary) where T : class
        {
            var type = typeof(T);
            var entry = System.Activator.CreateInstance(type) as T;
            if (entry == null)
            {
                return null;
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var enu = dictionary.GetEnumerator();

            while (enu.MoveNext())
            {
                var pair = (DictionaryEntry)enu.Current;
                dict.Add(pair.Key.ToString().ToLower(), pair.Value);
            }

            //给对象赋值
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (!PropertyUtility.IsValidProperty(property))
                    continue;
                var lowerKey = property.Name.ToLower();
                if (!dict.ContainsKey(lowerKey))
                    continue;
                    
                var value = dict[lowerKey];
                //注意：DBNull.Value是为了排除字段值为value={}的情况
                if (value != null && value != DBNull.Value)
                {
                    if (property.PropertyType.Name == "Byte")
                    {
                        if (Convert.ToBoolean(value)) //当值为true时
                        {
                            property.SetValue(entry, (Byte)1, null);
                        }
                        else if (!Convert.ToBoolean(value)) //当值为false时
                        {
                            property.SetValue(entry, (Byte)0, null);
                        }
                        else //当值为其他时
                        {
                            property.SetValue(entry, (Byte)value, null);
                        }
                    }
                    else if (property.PropertyType.Name.Equals("INT32", StringComparison.OrdinalIgnoreCase))
                    {
                        property.SetValue(entry, Convert.ToInt32(value), null);
                    }
                    else
                    {
                        string fullName = property.PropertyType.FullName;
                        string str = null;
                        if (fullName.Length > 14)
                        {
                            str = fullName.Substring((fullName.IndexOf('[') + 2), 14);
                        }
                        if (property.PropertyType.Name == "Nullable`1" && str == "System.Boolean")//针对MySql中bit类型字段，在实体对象中类型是bool
                        {
                            property.SetValue(entry, Convert.ToBoolean(value), null);
                        }
                        else
                        {
                            property.SetValue(entry, value, null);
                        }
                    }
                }
                else
                {
                    property.SetValue(entry, null, null);
                }
            }

            return entry;
        }

        private IList<T> CovertDateTableToList<T>(DataTable dataTable) where T : class
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return null;
            }
            var list = new List<T>();
            foreach (DataRow dr in dataTable.Rows)
            {
                var dictionary = new Hashtable();
                foreach (DataColumn dc in dataTable.Columns)
                {
                    dictionary.Add(dc.ColumnName, dr[dc.ColumnName]);
                }

                var obj = CreateModel<T>(dictionary);

                list.Add(obj);
            }
            return list;
        }

        private Hashtable GetHashtable(object entry)
        {
            Hashtable table;
            if (entry == null)
                table = null;
            else
                table = PropertyUtility.GetProperties(entry);
            return table;
        }
        private IDictionary GenerateTableForQuery(IDaoObject entryCriteria)
        {
            var table = new Hashtable();

            if (entryCriteria != null)
            {
                var properties = entryCriteria.GetProperties();
                while (properties.MoveNext())
                {
                    var pair = properties.Current;
                    table.Add(pair.Key, pair.Value);
                }
            }
            return table;
        }
        private T CreateNewDataObjectInstance<T>(IDaoObject dictionary) where T : class
        {
            var abstractType = typeof(T);

            //根据对象类型实例化并赋值
            var myType = abstractType.FullName; //命名空间.类型名
            var myAssembly = abstractType.Assembly.FullName.Split(',')[0]; //程序集名称
            var classType = string.Format("{0},{1}", myType, myAssembly);
            var type = Type.GetType(classType);
            if (type == null)
            {
                return null;
            }

            //根据类型创建对象
            var entry = System.Activator.CreateInstance(type) as T;
            if (entry == null)
            {
                return null;
            }
            //给对象赋值
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var value = dictionary.GetPropertyValue(property.Name.ToLower());
                //注意：DBNull.Value是为了排除字段值为value={}的情况
                if (value != null && value != DBNull.Value)
                {
                    if (property.PropertyType.Name == "Byte")
                    {
                        if (Convert.ToBoolean(value)) //当值为true时
                        {
                            property.SetValue(entry, (Byte)1, null);
                        }
                        else if (!Convert.ToBoolean(value)) //当值为false时
                        {
                            property.SetValue(entry, (Byte)0, null);
                        }
                        else //当值为其他时
                        {
                            property.SetValue(entry, (Byte)value, null);
                        }
                    }
                    else if (property.PropertyType.Name.Equals("INT32", StringComparison.OrdinalIgnoreCase))
                    {
                        property.SetValue(entry, Convert.ToInt32(value), null);
                    }
                    else
                    {
                        string fullName = property.PropertyType.FullName;
                        string str = null;
                        if (fullName.Length > 14)
                        {
                            str = fullName.Substring((fullName.IndexOf('[') + 2), 14);
                        }
                        if (property.PropertyType.Name == "Nullable`1" && str == "System.Boolean")//针对MySql中bit类型字段，在实体对象中类型是bool
                        {
                            property.SetValue(entry, Convert.ToBoolean(value), null);
                        }
                        else
                        {
                            property.SetValue(entry, value, null);
                        }
                    }
                }
                else
                {
                    property.SetValue(entry, null, null);
                }
            }

            return entry;
        }

        private IList<string> GenerateSortedTableForQuery(IDaoObject entryCriteria)
        {
            //var table = new SortedDictionary<string, object>();
            var paraNameList = new List<string>();
            if (entryCriteria != null)
            {
                var properties = entryCriteria.GetProperties();
                while (properties.MoveNext())
                {
                    var pair = properties.Current;
                    //table.Add(pair.Key, pair.Value);
                    paraNameList.Add(pair.Key);
                }
            }
            return paraNameList;
        }

    }
}
