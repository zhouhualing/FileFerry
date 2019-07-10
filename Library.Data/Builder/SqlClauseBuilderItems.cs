using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Data.Builder
{
	/// <summary>
	/// 所有Sql语句构造项的基类
	/// </summary>
	public abstract class SqlClauseBuilderItemBase
	{
		/// <summary>
		/// 得到Data的Sql字符串描述
		/// </summary>
		/// <param name="builder">构造器</param>
		/// <returns>返回将data翻译成sql语句的结果</returns>
		public abstract string GetDataDesp(ISqlBuilder builder);
	}

	/// <summary>
	/// 带数据的Sql语句构造项的基类
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemWithData : SqlClauseBuilderItemBase
	{
		private object data = null;
		private bool isExpression = false;

		/// <summary>
		/// 数据
		/// </summary>
		public object Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		/// <summary>
		/// 构想项中的Data是否是sql表达式
		/// </summary>
		public bool IsExpression
		{
			get { return this.isExpression; }
			set { this.isExpression = value; }
		}

		/// <summary>
		/// 得到Data的Sql字符串描述
		/// </summary>
		/// <param name="builder">构造器</param>
		/// <returns>返回将data翻译成sql语句的结果</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			if (this.data == null || this.data is DBNull)
				result = "NULL";
			else
			{
				if (this.data is DateTime)
				{
					DateTime minDate = new DateTime(1753, 1, 1);

					if ((DateTime)this.data < minDate)
						result = "NULL";
					else
						result = builder.FormatDateTime((DateTime)this.data);
				}
				else if (this.data is System.Guid)
				{
					if ((Guid)this.data == Guid.Empty)
						result = "NULL";
					else
						result = builder.CheckQuotationMark(this.data.ToString(), true);
				}
				else
				{
					if (this.isExpression == false && (this.data is string || this.data.GetType().IsEnum))
						result = builder.CheckQuotationMark(this.data.ToString(), true);
					else
						if (this.data is bool)
							result = ((int)Convert.ChangeType(this.data, typeof(int))).ToString();
						else
							result = this.data.ToString();
				}
			}

			return result;
		}
	}

	/// <summary>
	/// In操作的语句项
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemInOperator : SqlCaluseBuilderItemWithData
	{
	}

	/// <summary>
	/// 每一个构造项，包括字段名称和字段的值等内容
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemIUW : SqlCaluseBuilderItemWithData
	{
		private string operation = SqlClauseBuilderBase.EqualTo;

		/// <summary>
		/// 构造方法
		/// </summary>
		public SqlClauseBuilderItemIUW()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// Sql语句中的字段名
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField属性不能为空或空字符串");
				this.dataField = value;
			}
		}

		/// <summary>
		/// 字段和数据之间的操作符
		/// </summary>
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}
	}

	/// <summary>
	/// 构造排序表达式的构造项
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemOrd : SqlClauseBuilderItemBase
	{
		/// <summary>
		/// 
		/// </summary>
		private FieldSortDirection sortDirection = FieldSortDirection.Ascending;
		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// 构造方法
		/// </summary>
		public SqlClauseBuilderItemOrd()
		{
		}

		/// <summary>
		/// Sql语句中的字段名
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField属性不能为空或空字符串");
				this.dataField = value;
			}
		}

		/// <summary>
		/// 排序方向
		/// </summary>
		public FieldSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
			set
			{
				this.sortDirection = value;
			}
		}

		/// <summary>
		/// 得到Data的Sql字符串描述
		/// </summary>
		/// <param name="builder">构造器</param>
		/// <returns>返回将data翻译成sql语句的结果</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			if (this.sortDirection == FieldSortDirection.Descending)
				result = "DESC";

			return result;
		}
	}

	/// <summary>
	/// 字段的排序方向定义
	/// </summary>
	public enum FieldSortDirection
	{
		/// <summary>
		/// 升序
		/// </summary>
		Ascending,

		/// <summary>
		/// 降序
		/// </summary>
		Descending
	}

	/// <summary>
	/// 逻辑运算符
	/// </summary>
	public enum LogicOperatorDefine
	{
		/// <summary>
		/// “与”操作
		/// </summary>
		[EnumItemDescription(Description = "“与”操作", ShortName = "AND")]
		And,

		/// <summary>
		/// “或”操作
		/// </summary>
		[EnumItemDescription(Description = "“或”操作", ShortName = "OR")]
		Or
	}
}
