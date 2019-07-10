using IBatisNet.DataMapper;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WD.Library.Core;

namespace WD.CorePlugin
{
    public interface IBaseDao
    {
        void BeginTransaction();

        void CommitTransaction();

        void RollBackTransaction();

        int ExecuteUpdate(string statement, object entry);
        void ExecuteUpdate<T>(T instance, string stmtId) where T:class;
        int ExecuteUpdate(System.Collections.Hashtable hash, String stmtId);

        int ExecuteDelete(string statement, object entry);

        bool ExecuteInsert(string statement, object entry);

        bool ExecuteInsertGeneric<T>(string statement,T entry) where T : class;

        T ExecuteQueryObject<T>(string statement, object entry) where T : class;

        IList<T> ExecuteQueryList<T>(string statement, object entry) where T : class;

        string ExecuteQuerySql(string statement, object entry);

        int ExecuteQueryValueToInt(string statement, object entry);

        decimal ExecuteQueryValueToDecimal(string statement, object entry);

        IList<int> ExecuteQueryListToInt(string statement, object entry);

        IList<decimal> ExecuteQueryListToDecimal(string statement, object entry);

        DataTable ExecuteQueryDataTable(string statement, object entry);

        DataSet ExecuteQueryDataSet(string statement, object entry);
        
        T ExecuteQueryDataTableToObject<T>(string statement, object entry) where T : class;

        IList<T> ExecuteQueryDataTableToList<T>(string statement, object entry) where T : class;

        IList<T> ExecuteQueryDataTableToList<T>(DataTable dt) where T : class;
    }
}
