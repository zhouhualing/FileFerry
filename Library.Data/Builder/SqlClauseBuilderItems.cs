using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using WD.Library.Core;

namespace WD.Library.Data.Builder
{
	/// <summary>
	/// ����Sql��乹����Ļ���
	/// </summary>
	public abstract class SqlClauseBuilderItemBase
	{
		/// <summary>
		/// �õ�Data��Sql�ַ�������
		/// </summary>
		/// <param name="builder">������</param>
		/// <returns>���ؽ�data�����sql���Ľ��</returns>
		public abstract string GetDataDesp(ISqlBuilder builder);
	}

	/// <summary>
	/// �����ݵ�Sql��乹����Ļ���
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemWithData : SqlClauseBuilderItemBase
	{
		private object data = null;
		private bool isExpression = false;

		/// <summary>
		/// ����
		/// </summary>
		public object Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		/// <summary>
		/// �������е�Data�Ƿ���sql���ʽ
		/// </summary>
		public bool IsExpression
		{
			get { return this.isExpression; }
			set { this.isExpression = value; }
		}

		/// <summary>
		/// �õ�Data��Sql�ַ�������
		/// </summary>
		/// <param name="builder">������</param>
		/// <returns>���ؽ�data�����sql���Ľ��</returns>
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
	/// In�����������
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemInOperator : SqlCaluseBuilderItemWithData
	{
	}

	/// <summary>
	/// ÿһ������������ֶ����ƺ��ֶε�ֵ������
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemIUW : SqlCaluseBuilderItemWithData
	{
		private string operation = SqlClauseBuilderBase.EqualTo;

		/// <summary>
		/// ���췽��
		/// </summary>
		public SqlClauseBuilderItemIUW()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// Sql����е��ֶ���
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField���Բ���Ϊ�ջ���ַ���");
				this.dataField = value;
			}
		}

		/// <summary>
		/// �ֶκ�����֮��Ĳ�����
		/// </summary>
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}
	}

	/// <summary>
	/// ����������ʽ�Ĺ�����
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
		/// ���췽��
		/// </summary>
		public SqlClauseBuilderItemOrd()
		{
		}

		/// <summary>
		/// Sql����е��ֶ���
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField���Բ���Ϊ�ջ���ַ���");
				this.dataField = value;
			}
		}

		/// <summary>
		/// ������
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
		/// �õ�Data��Sql�ַ�������
		/// </summary>
		/// <param name="builder">������</param>
		/// <returns>���ؽ�data�����sql���Ľ��</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			if (this.sortDirection == FieldSortDirection.Descending)
				result = "DESC";

			return result;
		}
	}

	/// <summary>
	/// �ֶε���������
	/// </summary>
	public enum FieldSortDirection
	{
		/// <summary>
		/// ����
		/// </summary>
		Ascending,

		/// <summary>
		/// ����
		/// </summary>
		Descending
	}

	/// <summary>
	/// �߼������
	/// </summary>
	public enum LogicOperatorDefine
	{
		/// <summary>
		/// ���롱����
		/// </summary>
		[EnumItemDescription(Description = "���롱����", ShortName = "AND")]
		And,

		/// <summary>
		/// ���򡱲���
		/// </summary>
		[EnumItemDescription(Description = "���򡱲���", ShortName = "OR")]
		Or
	}
}
