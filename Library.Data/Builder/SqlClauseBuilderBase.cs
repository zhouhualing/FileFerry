using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Data.Builder
{
	/// <summary>
	/// Sql�Ӿ乹�����ĳ������
	/// </summary>
	public abstract class SqlClauseBuilderBase : CollectionBase
	{
		#region ��������������������
		/// <summary>
		/// �Ⱥ�
		/// </summary>
		internal const string EqualTo = "=";

		/// <summary>
		/// ���ڵ���
		/// </summary>
		internal const string GreaterThanOrEqualTo = ">=";

		/// <summary>
		/// ����
		/// </summary>
		internal const string GreaterThan = ">";

		/// <summary>
		/// С�ڵ���
		/// </summary>
		internal const string LessThanOrEqualTo = "<=";

		/// <summary>
		/// С��
		/// </summary>
		internal const string LessThan = "<";

		/// <summary>
		/// ������
		/// </summary>
		internal const string NotEqualTo = "<>";

		/// <summary>
		/// LIKE�����
		/// </summary>
		internal const string Like = "LIKE";

		/// <summary>
		/// IS�����
		/// </summary>
		internal const string Is = "IS";

		/// <summary>
		/// IN�����
		/// </summary>
		internal const string In = "IN";
		#endregion ��������������������

		/// <summary>
		/// �����±���ʹ�����
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
		/// ���󷽷��������еĹ���������һ��SQL
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		public abstract string ToSqlString(ISqlBuilder sqlBuilder);

	}

	/// <summary>
	/// In������Sql��乹����
	/// </summary>
	[Serializable]
	public class InSqlClauseBuilder : SqlClauseBuilderBase
	{
		/// <summary>
		/// ���һ��������
		/// </summary>SqlCaluseBuilderBase
		/// <typeparam name="T">���ݵ�����</typeparam>
		/// <param name="data">In����������</param>
		public void AppendItem<T>(params T[] data)
		{
			AppendItem<T>(false, data);
		}

		/// <summary>
		/// ���һ��������
		/// </summary>
		/// <typeparam name="T">���ݵ�����</typeparam>
		/// <param name="data">In����������</param>
		/// <param name="isExpression">�Ƿ��Ǳ��ʽ</param>
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
		/// ����Sql��䣨��ʽΪ������1,����2��...��
		/// </summary>
		/// <param name="builder">Sql��乹����</param>
		/// <returns>����Sql��䣨��ʽΪ������1,����2��...��</returns>
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
		/// ����Sql��䣬����In�����������û�����ݣ�In������Ҳ������
		/// </summary>
		/// <param name="builder">Sql��乹����</param>
		/// <returns>�����In���</returns>
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
	/// Insert��Update��Where��乹�����Ļ���
	/// </summary>
	public abstract class SqlClauseBuilderIUW : SqlClauseBuilderBase
	{
		/// <summary>
		/// ���һ��������
		/// </summary>SqlCaluseBuilderBase
		/// <typeparam name="T">���ݵ�����</typeparam>
		/// <param name="dataField">Sql����е��ֶ���</param>
		/// <param name="data">����������</param>
		public void AppendItem<T>(string dataField, T data)
		{
			AppendItem<T>(dataField, data, SqlClauseBuilderBase.EqualTo);
		}

		/// <summary>
		/// ���һ��������
		/// </summary>
		/// <typeparam name="T">���ݵ�����</typeparam>
		/// <param name="dataField">Sql����е��ֶ���</param>
		/// <param name="data">����������</param>
		/// <param name="op">���������</param>
		public void AppendItem<T>(string dataField, T data, string op)
		{
			AppendItem<T>(dataField, data, op, false);
		}

		/// <summary>
		/// ���һ��������
		/// </summary>
		/// <typeparam name="T">���ݵ�����</typeparam>
		/// <param name="dataField">Sql����е��ֶ���</param>
		/// <param name="data">����������</param>
		/// <param name="op">���������</param>
		/// <param name="isExpression">�����������Ƿ��Ǳ��ʽ</param>
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
	/// �ṩһ���ֶκ�ֵ�ļ��ϣ���������UPDATE����SET����
	/// </summary>
	[Serializable]
	public class UpdateSqlClauseBuilder : SqlClauseBuilderIUW
	{
		/// <summary>
		/// ����Update����SET���֣�������SET��
		/// </summary>
		/// <param name="sqlBuilder">Sql��乹����</param>
		/// <returns>�����Update�Ӿ�(����update����)</returns>
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
	/// �ṩһ���ֶκ�ֵ�ļ��ϣ���������INSERT�����ֶ����ƺ�VALUES����
	/// </summary>
	[Serializable]
	public class InsertSqlClauseBuilder : SqlClauseBuilderIUW
	{
		/// <summary>
		/// ����INSERT�����ֶ����ƺ�VALUES����
		/// </summary>
		/// <param name="sqlBuilder">Sql��乹����</param>
		/// <returns>�����Insert�Ӿ�(����insert����)</returns>
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
	/// �����ӵ�Sql�Ӿ�Ľӿ�
	/// </summary>
	public interface IConnectiveSqlClause
	{
		/// <summary>
		/// ����ʱ���߼������
		/// </summary>
		LogicOperatorDefine LogicOperator
		{
			get;
			set;
		}

		/// <summary>
		/// �Ӿ��Ƿ�Ϊ��
		/// </summary>
		bool IsEmpty
		{
			get;
		}

		/// <summary>
		/// ����Sql�Ӿ�
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		string ToSqlString(ISqlBuilder sqlBuilder);
	}


	/// <summary>
	/// �ṩһ���ֶκ�ֵ�ļ��ϣ���������WHERE���
	/// </summary>
	[Serializable]
	public class WhereSqlClauseBuilder : SqlClauseBuilderIUW, IConnectiveSqlClause
	{
		private LogicOperatorDefine logicOperator = LogicOperatorDefine.And;

		/// <summary>
		/// ���췽��
		/// </summary>
		public WhereSqlClauseBuilder()
			: base()
		{
		}

		/// <summary>
		/// ���췽��������ָ�������������ʽʱ���߼������
		/// </summary>
		/// <param name="lod">�߼������</param>
		public WhereSqlClauseBuilder(LogicOperatorDefine lod)
			: base()
		{
			this.logicOperator = lod;
		}

		/// <summary>
		/// �������ʽ֮����߼������
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
		/// �ж��Ƿ񲻴����κ��������ʽ
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.Count == 0;
			}
		}

		/// <summary>
		/// ��������WHERE���
		/// </summary>
		/// <param name="sqlBuilder">��乹����</param>
		/// <returns>�����Where�Ӿ�(����where����)</returns>
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
	/// �ṩһ���ֶκ�ֵ�ļ��ϣ���������ORDER BY�����ֶ����򲿷�
	/// </summary>
	[Serializable]
	public class OrderBySqlClauseBuilder : SqlClauseBuilderBase
	{
		/// <summary>
		/// ���һ��������
		/// </summary>
		/// <param name="dataField">����������</param>
		/// <param name="sortDirection">����ʽ</param>
		public void AppendItem(string dataField, FieldSortDirection sortDirection)
		{
			SqlClauseBuilderItemOrd item = new SqlClauseBuilderItemOrd();

			item.DataField = dataField;
			item.SortDirection = sortDirection;
			List.Add(item);
		}

		/// <summary>
		/// ��������ORDER BY�����ֶ����򲿷�
		/// </summary>
		/// <param name="sqlBuilder">Sql��乹����</param>
		/// <returns>�������Order By�Ӿ�</returns>
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
	/// �����ӵ�Sql�Ӿ�ļ��ϣ�����ͳһ����Sql��䣬�����֮��ʹ�����Ÿ���
	/// </summary>
	[Serializable]
	public class ConnectiveSqlClauseCollection : CollectionBase, IConnectiveSqlClause
	{
		private LogicOperatorDefine logicOperator = LogicOperatorDefine.And;

		/// <summary>
		/// ���췽��
		/// </summary>
		public ConnectiveSqlClauseCollection()
			: base()
		{
		}

		/// <summary>
		/// ���췽��������ָ�������������ʽʱ���߼������
		/// </summary>
		/// <param name="lo">�߼������</param>
		public ConnectiveSqlClauseCollection(LogicOperatorDefine lo)
			: base()
		{
			this.logicOperator = lo;
		}

		/// <summary>
		/// ����һ�������ӵ�Sql�Ӿ�
		/// </summary>
		/// <param name="clause">Sql�Ӿ�</param>
		public void Add(IConnectiveSqlClause clause)
		{
			List.Add(clause);
		}

		/// <summary>
		/// ��ȡ������һ�������ӵ�Sql�Ӿ�
		/// </summary>
		/// <param name="index">Sql�Ӿ�</param>
		/// <returns>�����ӵ�Sql�Ӿ�</returns>
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
		/// �����еĹ���������һ��SQL
		/// </summary>
		/// <param name="sqlBuilder">Sql���Ĺ�����</param>
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

		#region IConnectiveSqlClause ��Ա

		/// <summary>
		/// Sql�Ӿ�֮����߼������
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
		/// �ж��������ʽ�Ƿ�Ϊ��
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
