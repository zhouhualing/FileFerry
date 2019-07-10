using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Data.Builder
{
	/// <summary>
	/// Sql子句构造器的抽象基类
	/// </summary>
	public abstract class SqlClauseBuilderBase : CollectionBase
	{
		#region 条件项的运算符常量定义
		/// <summary>
		/// 等号
		/// </summary>
		internal const string EqualTo = "=";

		/// <summary>
		/// 大于等于
		/// </summary>
		internal const string GreaterThanOrEqualTo = ">=";

		/// <summary>
		/// 大于
		/// </summary>
		internal const string GreaterThan = ">";

		/// <summary>
		/// 小于等于
		/// </summary>
		internal const string LessThanOrEqualTo = "<=";

		/// <summary>
		/// 小于
		/// </summary>
		internal const string LessThan = "<";

		/// <summary>
		/// 不等于
		/// </summary>
		internal const string NotEqualTo = "<>";

		/// <summary>
		/// LIKE运算符
		/// </summary>
		internal const string Like = "LIKE";

		/// <summary>
		/// IS运算符
		/// </summary>
		internal const string Is = "IS";

		/// <summary>
		/// IN运算符
		/// </summary>
		internal const string In = "IN";
		#endregion 条件项的运算符常量定义

		/// <summary>
		/// 按照下标访问构造项
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public SqlClauseBuilderItemBase this[int i]
		{
			get
			{
				return (SqlClauseBuilderItemBase)List[i];
			}
		}

		/// <summary>
		/// 抽象方法，将所有的构造项生成一个SQL
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		public abstract string ToSqlString(ISqlBuilder sqlBuilder);

	}

	/// <summary>
	/// In操作的Sql语句构造器
	/// </summary>
	[Serializable]
	public class InSqlClauseBuilder : SqlClauseBuilderBase
	{
		/// <summary>
		/// 添加一个构造项
		/// </summary>SqlCaluseBuilderBase
		/// <typeparam name="T">数据的类型</typeparam>
		/// <param name="data">In操作的数据</param>
		public void AppendItem<T>(params T[] data)
		{
			AppendItem<T>(false, data);
		}

		/// <summary>
		/// 添加一个构造项
		/// </summary>
		/// <typeparam name="T">数据的类型</typeparam>
		/// <param name="data">In操作的数据</param>
		/// <param name="isExpression">是否是表达式</param>
		public void AppendItem<T>(bool isExpression, params T[] data)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(data != null, "data");

			for (int i = 0; i < data.Length; i++)
			{
				SqlCaluseBuilderItemInOperator item = new SqlCaluseBuilderItemInOperator();

				item.IsExpression = isExpression;
				item.Data = data[i];

				List.Add(item);
			}
		}

		/// <summary>
		/// 生成Sql语句（格式为：数据1,数据2，...）
		/// </summary>
		/// <param name="builder">Sql语句构造器</param>
		/// <returns>生成Sql语句（格式为：数据1,数据2，...）</returns>
		public override string ToSqlString(ISqlBuilder builder)
		{
			StringBuilder strB = new StringBuilder(256);

			for (int i = 0; i < List.Count; i++)
			{
				if (strB.Length > 0)
					strB.Append(", ");

				strB.Append(((SqlCaluseBuilderItemInOperator)List[i]).GetDataDesp(builder));
			}

			return strB.ToString();
		}

		/// <summary>
		/// 生成Sql语句，加上In操作符。如果没有数据，In操作符也不生成
		/// </summary>
		/// <param name="builder">Sql语句构造器</param>
		/// <returns>构造的In语句</returns>
		public string ToSqlStringWithInOperator(ISqlBuilder builder)
		{
			string result = string.Empty;
			string inClause = ToSqlString(builder);

			if (string.IsNullOrEmpty(inClause) == false)
				result = string.Format("IN ({0})", inClause);

			return result;
		}
	}

	/// <summary>
	/// Insert、Update、Where语句构造器的基类
	/// </summary>
	public abstract class SqlClauseBuilderIUW : SqlClauseBuilderBase
	{
		/// <summary>
		/// 添加一个构造项
		/// </summary>SqlCaluseBuilderBase
		/// <typeparam name="T">数据的类型</typeparam>
		/// <param name="dataField">Sql语句中的字段名</param>
		/// <param name="data">操作的数据</param>
		public void AppendItem<T>(string dataField, T data)
		{
			AppendItem<T>(dataField, data, SqlClauseBuilderBase.EqualTo);
		}

		/// <summary>
		/// 添加一个构造项
		/// </summary>
		/// <typeparam name="T">数据的类型</typeparam>
		/// <param name="dataField">Sql语句中的字段名</param>
		/// <param name="data">操作的数据</param>
		/// <param name="op">操作运算符</param>
		public void AppendItem<T>(string dataField, T data, string op)
		{
			AppendItem<T>(dataField, data, op, false);
		}

		/// <summary>
		/// 添加一个构造项
		/// </summary>
		/// <typeparam name="T">数据的类型</typeparam>
		/// <param name="dataField">Sql语句中的字段名</param>
		/// <param name="data">操作的数据</param>
		/// <param name="op">操作运算符</param>
		/// <param name="isExpression">操作的数据是否是表达式</param>
		public void AppendItem<T>(string dataField, T data, string op, bool isExpression)
		{
			SqlClauseBuilderItemIUW item = new SqlClauseBuilderItemIUW();

			item.DataField = dataField;
			item.IsExpression = isExpression;
			item.Operation = op;
			item.Data = data;

			List.Add(item);
		}
	}

	/// <summary>
	/// 提供一组字段和值的集合，帮助生成UPDATE语句的SET部分
	/// </summary>
	[Serializable]
	public class UpdateSqlClauseBuilder : SqlClauseBuilderIUW
	{
		/// <summary>
		/// 生成Update语句的SET部分（不包括SET）
		/// </summary>
		/// <param name="sqlBuilder">Sql语句构造器</param>
		/// <returns>构造的Update子句(不含update部分)</returns>
		public override string ToSqlString(ISqlBuilder sqlBuilder)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

			StringBuilder strB = new StringBuilder(256);

			foreach (SqlClauseBuilderItemIUW item in List)
			{
				if (strB.Length > 0)
					strB.Append(", ");

				strB.Append(item.DataField);
				strB.AppendFormat(" {0} ", item.Operation);
				strB.Append(item.GetDataDesp(sqlBuilder));
			}

			return strB.ToString();
		}
	}

	/// <summary>
	/// 提供一组字段和值的集合，帮助生成INSERT语句的字段名称和VALUES部分
	/// </summary>
	[Serializable]
	public class InsertSqlClauseBuilder : SqlClauseBuilderIUW
	{
		/// <summary>
		/// 生成INSERT语句的字段名称和VALUES部分
		/// </summary>
		/// <param name="sqlBuilder">Sql语句构造器</param>
		/// <returns>构造的Insert子句(不含insert部分)</returns>
		public override string ToSqlString(ISqlBuilder sqlBuilder)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

			StringBuilder strBFields = new StringBuilder(256);
			StringBuilder strBValues = new StringBuilder(256);

			foreach (SqlClauseBuilderItemIUW item in List)
			{
				if (item.Data != null && item.Data != DBNull.Value)
				{
					if (strBFields.Length > 0)
						strBFields.Append(", ");

					strBFields.Append(item.DataField);

					if (strBValues.Length > 0)
						strBValues.Append(", ");

					strBValues.Append(item.GetDataDesp(sqlBuilder));
				}
			}

			string result = string.Empty;

			if (strBValues.Length > 0)
				result = string.Format("({0}) VALUES({1})", strBFields.ToString(), strBValues.ToString());

			return result;
		}
	}

	/// <summary>
	/// 可连接的Sql子句的接口
	/// </summary>
	public interface IConnectiveSqlClause
	{
		/// <summary>
		/// 连接时的逻辑运算符
		/// </summary>
		LogicOperatorDefine LogicOperator
		{
			get;
			set;
		}

		/// <summary>
		/// 子句是否为空
		/// </summary>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// 生成Sql子句
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		string ToSqlString(ISqlBuilder sqlBuilder);
	}


	/// <summary>
	/// 提供一组字段和值的集合，帮助生成WHERE语句
	/// </summary>
	[Serializable]
	public class WhereSqlClauseBuilder : SqlClauseBuilderIUW, IConnectiveSqlClause
	{
		private LogicOperatorDefine logicOperator = LogicOperatorDefine.And;

		/// <summary>
		/// 构造方法
		/// </summary>
		public WhereSqlClauseBuilder()
			: base()
		{
		}

		/// <summary>
		/// 构造方法，可以指定生成条件表达式时的逻辑运算符
		/// </summary>
		/// <param name="lod">逻辑运算符</param>
		public WhereSqlClauseBuilder(LogicOperatorDefine lod)
			: base()
		{
			this.logicOperator = lod;
		}

		/// <summary>
		/// 条件表达式之间的逻辑运算符
		/// </summary>
		public LogicOperatorDefine LogicOperator
		{
			get
			{
				return this.logicOperator;
			}
			set
			{
				this.logicOperator = value;
			}
		}

		/// <summary>
		/// 判断是否不存在任何条件表达式
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		/// <summary>
		/// 帮助生成WHERE语句
		/// </summary>
		/// <param name="sqlBuilder">语句构造器</param>
		/// <returns>构造的Where子句(不含where部分)</returns>
		public override string ToSqlString(ISqlBuilder sqlBuilder)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

			StringBuilder strB = new StringBuilder(256);

			foreach (SqlClauseBuilderItemIUW item in List)
			{
				if (strB.Length > 0)
					strB.AppendFormat(" {0} ", EnumItemDescriptionAttribute.GetAttribute(this.logicOperator).ShortName);

				strB.Append(item.DataField);
				strB.AppendFormat(" {0} ", item.Operation);
				strB.Append(item.GetDataDesp(sqlBuilder));
			}
			return strB.ToString();
		}
	}

	/// <summary>
	/// 提供一组字段和值的集合，帮助生成ORDER BY语句的字段排序部分
	/// </summary>
	[Serializable]
	public class OrderBySqlClauseBuilder : SqlClauseBuilderBase
	{
		/// <summary>
		/// 添加一个构造项
		/// </summary>
		/// <param name="dataField">操作的数据</param>
		/// <param name="sortDirection">排序方式</param>
		public void AppendItem(string dataField, FieldSortDirection sortDirection)
		{
			SqlClauseBuilderItemOrd item = new SqlClauseBuilderItemOrd();

			item.DataField = dataField;
			item.SortDirection = sortDirection;
			List.Add(item);
		}

		/// <summary>
		/// 帮助生成ORDER BY语句的字段排序部分
		/// </summary>
		/// <param name="sqlBuilder">Sql语句构造器</param>
		/// <returns>构造出的Order By子句</returns>
		public override string ToSqlString(ISqlBuilder sqlBuilder)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(sqlBuilder != null, "sqlBuilder");

			StringBuilder strB = new StringBuilder(256);

			foreach (SqlClauseBuilderItemOrd item in List)
			{
				if (strB.Length > 0)
					strB.Append(", ");

				strB.Append(item.DataField);

				string desp = item.GetDataDesp(sqlBuilder);

				if (false == string.IsNullOrEmpty(desp))
					strB.Append(" " + desp);
			}

			return strB.ToString();
		}
	}

	/// <summary>
	/// 可连接的Sql子句的集合，可以统一生成Sql语句，各语句之间使用括号隔离
	/// </summary>
	[Serializable]
	public class ConnectiveSqlClauseCollection : CollectionBase, IConnectiveSqlClause
	{
		private LogicOperatorDefine logicOperator = LogicOperatorDefine.And;

		/// <summary>
		/// 构造方法
		/// </summary>
		public ConnectiveSqlClauseCollection()
			: base()
		{
		}

		/// <summary>
		/// 构造方法，可以指定生成条件表达式时的逻辑运算符
		/// </summary>
		/// <param name="lo">逻辑运算符</param>
		public ConnectiveSqlClauseCollection(LogicOperatorDefine lo)
			: base()
		{
			this.logicOperator = lo;
		}

		/// <summary>
		/// 增加一个可连接的Sql子句
		/// </summary>
		/// <param name="clause">Sql子句</param>
		public void Add(IConnectiveSqlClause clause)
		{
			List.Add(clause);
		}

		/// <summary>
		/// 获取或设置一个可连接的Sql子句
		/// </summary>
		/// <param name="index">Sql子句</param>
		/// <returns>可连接的Sql子句</returns>
		public IConnectiveSqlClause this[int index]
		{
			get
			{
				return (IConnectiveSqlClause)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// 将所有的构造项生成一个SQL
		/// </summary>
		/// <param name="sqlBuilder">Sql语句的构造器</param>
		/// <returns></returns>
		public string ToSqlString(ISqlBuilder sqlBuilder)
		{
			StringBuilder strB = new StringBuilder(256);

			for (int i = 0; i < this.Count; i++)
			{
				IConnectiveSqlClause clause = this[i];

				if (clause.IsEmpty == false)
				{
					if (strB.Length > 0)
						strB.AppendFormat(" {0} ", EnumItemDescriptionAttribute.GetAttribute(this.logicOperator).ShortName);

					strB.AppendFormat("({0})", clause.ToSqlString(sqlBuilder));
				}
			}

			return strB.ToString();
		}

		#region Protected
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		protected override void OnInsert(int index, object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
			base.OnInsert(index, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(newValue != null, "newValue");
			base.OnSet(index, oldValue, newValue);
		}
		#endregion

		#region IConnectiveSqlClause 成员

		/// <summary>
		/// Sql子句之间的逻辑运算符
		/// </summary>
		public LogicOperatorDefine LogicOperator
		{
			get
			{
				return this.logicOperator;
			}
			set
			{
				this.logicOperator = value;
			}
		}

		/// <summary>
		/// 判断条件表达式是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				bool result = true;

				foreach (IConnectiveSqlClause clause in List)
				{
					if (clause.IsEmpty == false)
					{
						result = false;
						break;
					}
				}

				return result;
			}
		}
		#endregion
	}

}
