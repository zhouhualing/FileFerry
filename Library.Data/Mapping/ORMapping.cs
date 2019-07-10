using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using WD.Library.Core;
using WD.Library.Data.Builder;
using WD.Library.Data.Properties;

namespace WD.Library.Data.Mapping
{
	/// <summary>
	/// ӳ������У��¼��Ĳ���
	/// </summary>
	public sealed class MappingEventArgs
	{
		/// <summary>
		/// ���ݶ����������
		/// </summary>
		public string PropertyName
		{
			get;
			internal set;
		}

		/// <summary>
		/// ����Դ���ֶ�����
		/// </summary>
		public string DataFieldName
		{
			get;
			internal set;
		}

		/// <summary>
		/// ӳ��ʱ�����ݶ���
		/// </summary>
		public object Graph
		{
			get;
			internal set;
		}
	}

	/// <summary>
	/// ����Դӳ�䵽����ʱ�������Ҫ�����Ӷ����򴥷��˶���
	/// </summary>
	/// <param name="dataSource">����Դ</param>
	/// <param name="args"></param>
	/// <param name="useDefaultObject">�Ƿ�ʹ��ȱʡ�Ĺ������</param>
	/// <returns></returns>
	public delegate object CreateSubObjectDelegate(object dataSource, MappingEventArgs args, ref bool useDefaultObject);

	/// <summary>
	/// ӳ��Data������ʱ��ί�м��϶���
	/// </summary>
	public class DataToObjectDeligations
	{
		/// <summary>
		/// 
		/// </summary>
		public event CreateSubObjectDelegate CreateSubObject;

		internal object OnCreateSubObjectDelegate(object dataSource, MappingEventArgs args, ref bool useDefaultObject)
		{
			object result = null;

			if (CreateSubObject != null)
				result = CreateSubObject(dataSource, args, ref useDefaultObject);

			return result;
		}
	}

	/// <summary>
	/// ����ORMapping�Ĺ�����
	/// </summary>
	/// <remarks>
	/// �ṩһЩ����������ֶν���ת���ľ�̬����
	/// </remarks>
	public static class ORMapping
	{
		private class RelativeAttributes
		{
			public ORFieldMappingAttribute FieldMapping = null;
			public SqlBehaviorAttribute SqlBehavior = null;
			public NoMappingAttribute NoMapping = null;
			public List<SubClassORFieldMappingAttribute> SubClassFieldMappings = new List<SubClassORFieldMappingAttribute>();
			public List<SubClassSqlBehaviorAttribute> SubClassFieldSqlBehaviors = new List<SubClassSqlBehaviorAttribute>();
			public SubClassTypeAttribute SubClassType = null;
		}

		private delegate void DoSqlClauseBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph);

		/// <summary>
		/// ��ȡ����������ֶ�֮���ӳ���ϵ
		/// </summary>
		/// <typeparam name="T">���������</typeparam>
		/// <returns>ӳ���ϵ����</returns>
		/// <remarks>��ȡ����������ֶ�֮���ӳ���ϵ
		/// <see cref="WD.Library.Data.Mapping.ORMappingItemCollection"/>
		/// </remarks>
		public static ORMappingItemCollection GetMappingInfo<T>()
		{
			return InnerGetMappingInfo(typeof(T));
		}

		#region Object To Sql
		/// <summary>
		/// ���ݶ���ƴInsert���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="builder">����Sql������͵�Builder��TSqlBuilder��PlSqlBuilder</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶ��Լ��ඨ���ϵı���������������Insert���</returns>
		public static string GetInsertSql<T>(T graph, ISqlBuilder builder, params string[] ignorProperties)
		{
			ORMappingItemCollection mapping = InnerGetMappingInfoByObject(graph);
			//ORMappingItemCollection mapping = GetMappingInfo<T>();

			return GetInsertSql<T>(graph, mapping, builder, ignorProperties);
		}

		/// <summary>
		/// ���ݶ���ƴInsert���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="mapping">ӳ���ϵ</param>
		/// <param name="builder">����Sql������͵�Builder��TSqlBuilder��PlSqlBuilder</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶ��Լ��ඨ���ϵı���������������Insert���</returns>
		public static string GetInsertSql<T>(T graph, ORMappingItemCollection mapping, ISqlBuilder builder, params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");
			ExceptionHelper.FalseThrow<ArgumentNullException>(builder != null, "builder");

			InsertSqlClauseBuilder insertBuilder = GetInsertSqlClauseBuilder(graph, mapping, ignorProperties);

			return string.Format("INSERT INTO {0} {1}", mapping.TableName, insertBuilder.ToSqlString(builder));
		}

		/// <summary>
		/// ���ݶ���ƴInsert���ʱ�ķ���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>InsertSqlClauseBuilder���󣬹�ƴInsert���ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶΣ�����InsertSqlClauseBuilder�����Թ�����ƴInsert�����ֶ����ƺ�Values����
		/// <see cref="WD.Library.Data.Builder.InsertSqlClauseBuilder"/>
		/// </remarks>
		public static InsertSqlClauseBuilder GetInsertSqlClauseBuilder<T>(T graph, params string[] ignorProperties)
		{
			return GetInsertSqlClauseBuilder<T>(graph, InnerGetMappingInfoByObject(graph), ignorProperties);
			//return GetInsertSqlClauseBuilder<T>(graph, GetMappingInfo<T>(), ignorProperties);
		}

		/// <summary>
		/// ���ݶ���ƴInsert���ʱ�ķ���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="mapping">ӳ���ϵ</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>InsertSqlClauseBuilder���󣬹�ƴInsert���ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶΣ�����InsertSqlClauseBuilder�����Թ�����ƴInsert�����ֶ����ƺ�Values����
		/// <see cref="WD.Library.Data.Builder.InsertSqlClauseBuilder"/>
		/// </remarks>
		public static InsertSqlClauseBuilder GetInsertSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

			InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

			FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Insert,
				new DoSqlClauseBuilder<T>(DoInsertUpdateSqlClauseBuilder<T>), ignorProperties);

			return builder;
		}

		/// <summary>
		/// ���ݶ���ƴUpdate���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="builder">����Sql������͵�Builder��TSqlBuilder��PlSqlBuilder</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶ��Լ��ඨ���ϵı���������������Update���</returns>
		public static string GetUpdateSql<T>(T graph, ISqlBuilder builder, params string[] ignorProperties)
		{
			ORMappingItemCollection mapping = InnerGetMappingInfoByObject(graph);
			//ORMappingItemCollection mapping = GetMappingInfo<T>();

			return GetUpdateSql<T>(graph, mapping, builder, ignorProperties);
		}

		/// <summary>
		/// ���ݶ���ƴInsert���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="mapping">ӳ���ϵ</param>
		/// <param name="builder">����Sql������͵�Builder��TSqlBuilder��PlSqlBuilder</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶ��Լ��ඨ���ϵı���������������Insert���</returns>
		public static string GetUpdateSql<T>(T graph, ORMappingItemCollection mapping, ISqlBuilder builder, params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");
			ExceptionHelper.FalseThrow<ArgumentNullException>(builder != null, "builder");

			UpdateSqlClauseBuilder updateBuilder = GetUpdateSqlClauseBuilder(graph, mapping, ignorProperties);
			WhereSqlClauseBuilder whereBuilder = GetWhereSqlClauseBuilderByPrimaryKey(graph, mapping);

			return string.Format("UPDATE {0} SET {1} WHERE {2}",
				mapping.TableName,
				updateBuilder.ToSqlString(builder),
				whereBuilder.ToSqlString(builder));
		}

		/// <summary>
		/// ���ݶ���ƴUpdate���ʱ�ķ���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>UpdateSqlClauseBuilder���󣬹�ƴUpdate���ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶΣ�����UpdateSqlClauseBuilder�����Թ�����ƴUpdate�����ֶ����ƺ�Values����
		/// <see cref="WD.Library.Data.Builder.UpdateSqlClauseBuilder"/>
		/// </remarks>
		public static UpdateSqlClauseBuilder GetUpdateSqlClauseBuilder<T>(T graph, params string[] ignorProperties)
		{
			return GetUpdateSqlClauseBuilder<T>(graph, InnerGetMappingInfoByObject(graph), ignorProperties);
			//return GetUpdateSqlClauseBuilder<T>(graph, GetMappingInfo<T>(), ignorProperties);
		}

		/// <summary>
		/// ���ݶ���ƴUpdate���ʱ�ķ���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="mapping">ӳ���ϵ</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>UpdateSqlClauseBuilder���󣬹�ƴUpdate���ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶΣ�����UpdateSqlClauseBuilder�����Թ�����ƴUpdate�����ֶ����ƺ�Values����
		public static UpdateSqlClauseBuilder GetUpdateSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

			UpdateSqlClauseBuilder builder = new UpdateSqlClauseBuilder();

			FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Update,
				new DoSqlClauseBuilder<T>(DoInsertUpdateSqlClauseBuilder<T>), ignorProperties);

			return builder;
		}

		/// <summary>
		/// ���ݶ���ƴWhere�Ӿ�ķ���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>WhereSqlClauseBuilder����ƴWhere�Ӿ�ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶΣ�����WhereSqlClauseBuilder�����Թ�����ƴWhere�Ӿ�ʹ��
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder<T>(T graph, params string[] ignorProperties)
		{
			return GetWhereSqlClauseBuilder<T>(graph, InnerGetMappingInfoByObject(graph), ignorProperties);
			//return GetWhereSqlClauseBuilder<T>(graph, GetMappingInfo<T>(), ignorProperties);
		}

		/// <summary>
		/// ���ݶ���ƴWhere�Ӿ�ķ���
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="graph">����</param>
		/// <param name="mapping">ӳ���ϵ</param>
		/// <param name="ignorProperties">���Ե��ֶ�</param>
		/// <returns>WhereSqlClauseBuilder����ƴWhere�Ӿ�ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ���Ͷ���ӳ��ʱ��Ҫ���Ե��ֶΣ�����WhereSqlClauseBuilder�����Թ�����ƴWhere�Ӿ�ʹ��
		/// <see cref="WD.Library.Data.Builder.WhereSqlClauseBuilder"/>
		/// </remarks>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilder<T>(T graph, ORMappingItemCollection mapping, params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Where,
				new DoSqlClauseBuilder<T>(DoWhereSqlClauseBuilder<T>), ignorProperties);

			return builder;
		}

		/// <summary>
		/// ���ݶ������������Ե����ԣ�����WhereSqlClauseBuilder����
		/// </summary>
		/// <typeparam name="T">����</typeparam>
		/// <param name="graph">����</param>
		/// <param name="ignorProperties">���Ե��ֶ�����</param>
		/// <returns>WhereSqlClauseBuilder����ƴWhere�Ӿ�ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ������������Ե����ԣ�����WhereSqlClauseBuilder�����Թ�����ƴWhere�Ӿ�ʹ��
		/// <see cref="WD.Library.Data.Builder.WhereSqlClauseBuilder"/>
		/// </remarks>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilderByPrimaryKey<T>(T graph, params string[] ignorProperties)
		{
			return GetWhereSqlClauseBuilderByPrimaryKey(graph, InnerGetMappingInfoByObject(graph), ignorProperties);
			//return GetWhereSqlClauseBuilderByPrimaryKey(graph, GetMappingInfo<T>(), ignorProperties);
		}

		/// <summary>
		/// ���ݶ������������Ե����ԣ�����WhereSqlClauseBuilder����
		/// </summary>
		/// <typeparam name="T">����</typeparam>
		/// <param name="graph">����</param>
		/// <param name="mapping">ӳ���ϵ</param>
		/// <param name="ignorProperties">���Ե��ֶ�����</param>
		/// <returns>WhereSqlClauseBuilder����ƴWhere�Ӿ�ʹ��</returns>
		/// <remarks>
		/// ���ݴ���Ķ������������Ե����ԣ�����WhereSqlClauseBuilder�����Թ�����ƴWhere�Ӿ�ʹ��
		/// <see cref="WD.Library.Data.Builder.WhereSqlClauseBuilder"/>
		/// </remarks>
		public static WhereSqlClauseBuilder GetWhereSqlClauseBuilderByPrimaryKey<T>(T graph, ORMappingItemCollection mapping, params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(mapping != null, "mapping");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			FillSqlClauseBuilder(builder, graph, mapping, ClauseBindingFlags.Where,
				new DoSqlClauseBuilder<T>(DoWhereSqlClauseBuilderByPrimaryKey<T>), ignorProperties);

			return builder;
		}
		#endregion Object To Sql

		#region DataRow To Object
		/// <summary>
		/// ��DataRow��ֵд�뵽������
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="row">DataRow����</param>
		/// <param name="graph">����</param>
		/// <remarks>
		/// �������DataRow�е���ֵд�뵽����graph��
		/// </remarks>
		public static void DataRowToObject<T>(DataRow row, T graph)
		{
			DataRowToObject(row, InnerGetMappingInfoByObject(graph), graph);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <param name="graph"></param>
		/// <param name="dod"></param>
		public static void DataRowToObject<T>(DataRow row, T graph, DataToObjectDeligations dod)
		{
			DataRowToObject(row, InnerGetMappingInfoByObject(graph), graph, dod);
		}

		/// <summary>
		/// ��DataRow��ֵд�뵽������
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="row">DataRow����</param>
		/// <param name="items">ӳ���ϵ</param>
		/// <param name="graph">����</param>
		public static void DataRowToObject<T>(DataRow row, ORMappingItemCollection items, T graph)
		{
			DataRowToObject(row, items, graph, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="row">DataRow����</param>
		/// <param name="items">ӳ���ϵ</param>
		/// <param name="graph">����</param>
		/// <param name="dod"></param>
		public static void DataRowToObject<T>(DataRow row, ORMappingItemCollection items, T graph, DataToObjectDeligations dod)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(row != null, "row");
			ExceptionHelper.FalseThrow<ArgumentNullException>(items != null, "items");
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");
			ExceptionHelper.FalseThrow<ArgumentNullException>(row.Table != null, "row.Table");

			foreach (DataColumn column in row.Table.Columns)
			{
				if (items.Contains(column.ColumnName))
				{
					ORMappingItem item = items[column.ColumnName];

					System.Type realType = GetRealType(item.MemberInfo);

					object data = row[column];
					if (Convertible(realType, data))
						SetValueToObject(item, graph, ConvertData(item, data), row, dod);
				}
			}
		}
		#endregion Data To Object

		#region DataReader to Object
		/// <summary>
		/// ��DataReader��ֵд�뵽������
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="dr">IDataReader����</param>
		/// <param name="graph">����</param>
		/// <remarks>
		/// �������DataReader�е���ֵд�뵽����graph��
		/// </remarks>
		public static void DataReaderToObject<T>(IDataReader dr, T graph)
		{
			DataReaderToObject(dr, InnerGetMappingInfoByObject(graph), graph);
		}

		/// <summary>
		/// ��DataReader��ֵд�뵽������
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="dr">IDataReader����</param>
		/// <param name="items">ӳ���ϵ</param>
		/// <param name="graph">����</param>
		public static void DataReaderToObject<T>(IDataReader dr, ORMappingItemCollection items, T graph)
		{
			DataReaderToObject(dr, items, graph, null);
		}

		/// <summary>
		/// ��DataReader��ֵд�뵽������
		/// </summary>
		/// <typeparam name="T">��������</typeparam>
		/// <param name="dr">IDataReader����</param>
		/// <param name="items">ӳ���ϵ</param>
		/// <param name="graph">����</param>
		/// <param name="dod"></param>
		public static void DataReaderToObject<T>(IDataReader dr, ORMappingItemCollection items, T graph, DataToObjectDeligations dod)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(dr != null, "dr");
			ExceptionHelper.FalseThrow<ArgumentNullException>(items != null, "items");
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			DataTable schemaTable = dr.GetSchemaTable();

			foreach (DataRow row in schemaTable.Rows)
			{
				string columnName = row["ColumnName"].ToString();
				if (items.Contains(columnName))
				{
					ORMappingItem item = items[row["ColumnName"].ToString()];
					System.Type realType = GetRealType(item.MemberInfo);

					object data = dr[columnName];
					if (Convertible(realType, data))
						SetValueToObject(item, graph, ConvertData(item, data), dr, dod);
				}
			}
		}
		#endregion

		#region ˽�з���
		private static object ConvertData(ORMappingItem item, object data)
		{
			try
			{
				System.Type realType = GetRealType(item.MemberInfo);

				return DataConverter.ChangeType(data, realType);
			}
			catch (System.Exception ex)
			{
				throw new SystemSupportException(
					string.Format(Resource.ConvertDataFieldToPropertyError,
						item.DataFieldName, item.PropertyName, ex.Message),
					ex
					);
			}
		}

		private static bool Convertible(System.Type targetType, object data)
		{
			bool result = true;

			if (data == null && targetType.IsValueType)
				result = false;
			else
			{
				if (data == DBNull.Value)
				{
					if (targetType != typeof(DBNull) && targetType != typeof(string))
						result = false;
				}
			}

			return result;
		}

		private static void FillSqlClauseBuilder<T>(
				SqlClauseBuilderIUW builder,
				T graph,
				ORMappingItemCollection mapping,
				ClauseBindingFlags bindingFlags,
				DoSqlClauseBuilder<T> builderDelegate,
				params string[] ignorProperties)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			foreach (ORMappingItem item in mapping)
			{
				if (Array.Exists<string>(ignorProperties,
											delegate(string target)
											{
												return (string.Compare(target, item.PropertyName, true) == 0);
											}) == false)
				{
					if ((item.BindingFlags & bindingFlags) != ClauseBindingFlags.None)
						builderDelegate(builder, item, graph);
				}
			}
		}

		private static void DoInsertUpdateSqlClauseBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
		{
			if (item.IsIdentity == false)
			{
				object data = GetValueFromObject(item, graph);

				if ((data == null || data == DBNull.Value || (data != null && data.Equals(TypeCreator.GetTypeDefaultValue(data.GetType())))) &&
						string.IsNullOrEmpty(item.DefaultExpression) == false)
					builder.AppendItem(item.DataFieldName, item.DefaultExpression, SqlClauseBuilderBase.EqualTo, true);
				else
					builder.AppendItem(item.DataFieldName, data);
			}
		}

		private static void DoWhereSqlClauseBuilder<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
		{
			object data = GetValueFromObject(item, graph);

			if ((data == null || data == DBNull.Value))
				builder.AppendItem(item.DataFieldName, data, SqlClauseBuilderBase.Is);
			else
				builder.AppendItem(item.DataFieldName, data);
		}

		private static void DoWhereSqlClauseBuilderByPrimaryKey<T>(SqlClauseBuilderIUW builder, ORMappingItem item, T graph)
		{
			if (item.PrimaryKey)
			{
				object data = GetValueFromObject(item, graph);

				if ((data == null || data == DBNull.Value))
					builder.AppendItem(item.DataFieldName, data, SqlClauseBuilderBase.Is);
				else
					builder.AppendItem(item.DataFieldName, data);
			}
		}

		private static void SetValueToObject(ORMappingItem item, object graph, object data, object row, DataToObjectDeligations dod)
		{
			if (string.IsNullOrEmpty(item.SubClassPropertyName))
				SetMemberValueToObject(item.MemberInfo, graph, data);
			else
			{
				if (graph != null)
				{
					MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (mi == null)
						mi = graph.GetType().GetField(item.PropertyName,
							BindingFlags.Instance | BindingFlags.Public);

					if (mi != null)
					{
						object subGraph = GetMemberValueFromObject(mi, graph);

						if (subGraph == null)
						{
							bool useDefaultObject = true;

							if (dod != null)
							{
								MappingEventArgs args = new MappingEventArgs();

								args.DataFieldName = item.DataFieldName;
								args.PropertyName = item.PropertyName;
								args.Graph = graph;

								subGraph = dod.OnCreateSubObjectDelegate(row, args, ref useDefaultObject);
							}

							if (useDefaultObject)
							{
								if (string.IsNullOrEmpty(item.SubClassTypeDescription) == false)
									subGraph = TypeCreator.CreateInstance(item.SubClassTypeDescription);
								else
									subGraph = Activator.CreateInstance(GetRealType(mi), true);
							}

							SetMemberValueToObject(item.MemberInfo, subGraph, data);
							SetMemberValueToObject(mi, graph, subGraph);
						}
						else
							SetMemberValueToObject(item.MemberInfo, subGraph, data);
					}
				}
			}
		}

		private static void SetMemberValueToObject(MemberInfo mi, object graph, object data)
		{
			data = DecorateDate(data);

			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					PropertyInfo pi = (PropertyInfo)mi;
					if (pi.CanWrite)
						pi.SetValue(graph, data, null);
					break;
				case MemberTypes.Field:
					FieldInfo fi = (FieldInfo)mi;
					fi.SetValue(graph, data);
					break;
				default:
					ThrowInvalidMemberInfoTypeException(mi);
					break;
			}
		}

		/// <summary>
		/// �����ݽ����������Σ�������������͵����Լӹ�
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static object DecorateDate(object data)
		{
			object result = data;

			if (data is DateTime)
			{
				DateTime dt = (DateTime)data;

				if (dt.Kind == DateTimeKind.Unspecified)
					result = DateTime.SpecifyKind(dt, DateTimeKind.Local);
			}

			return result;
		}

		private static object GetValueFromObject(ORMappingItem item, object graph)
		{
			object data = null;

			if (string.IsNullOrEmpty(item.SubClassPropertyName))
				data = GetValueFromObjectDirectly(item, graph);
			else
			{
				if (graph != null)
				{
					MemberInfo mi = graph.GetType().GetProperty(item.PropertyName,
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (mi == null)
						mi = graph.GetType().GetField(item.PropertyName,
							BindingFlags.Instance | BindingFlags.Public);

					if (mi != null)
					{
						object subGraph = GetMemberValueFromObject(mi, graph);

						if (subGraph != null)
							data = GetValueFromObjectDirectly(item, subGraph);
					}
				}
			}

			return data;
		}

		private static object GetValueFromObjectDirectly(ORMappingItem item, object graph)
		{
            //we allow memberinfo null,Hualing
            object data = null;
            if (item.MemberInfo == null)
            {
                data = Reflection.ReflectionCompiler.CreateGetter(Reflection.PropertyUtility.GetPropertyInfo(graph.GetType(), item.PropertyName))(graph);
            }
            else
            {
                data = GetMemberValueFromObject(item.MemberInfo, graph);
            }

			if (data != null)
			{
				System.Type dataType = data.GetType();
				if (dataType.IsEnum)
				{
					if (item.EnumUsage == EnumUsageTypes.UseEnumValue)
						data = (int)data;
					else
						data = data.ToString();
				}
				else
					if (dataType == typeof(TimeSpan))
						data = ((TimeSpan)data).TotalSeconds;
			}

			return data;
		}

		private static object GetMemberValueFromObject(MemberInfo mi, object graph)
		{
			try
			{
				object data = null;

				switch (mi.MemberType)
				{
					case MemberTypes.Property:
						PropertyInfo pi = (PropertyInfo)mi;
						if (pi.CanRead)
							data = pi.GetValue(graph, null);
						break;
					case MemberTypes.Field:
						FieldInfo fi = (FieldInfo)mi;
						data = fi.GetValue(graph);
						break;
					default:
						ThrowInvalidMemberInfoTypeException(mi);
						break;
				}

				return data;
			}
			catch (System.Exception ex)
			{
				System.Exception realEx = ExceptionHelper.GetRealException(ex);

				throw new ApplicationException(string.Format("��ȡ����{0}ֵ��ʱ�����{1}", mi.Name, realEx.Message));
			}
		}

		private static ORMappingItemCollection GetMappingItemCollection(System.Type type)
		{
			ORMappingItemCollection result = new ORMappingItemCollection();

			ORTableMappingAttribute tableAttr =
				(ORTableMappingAttribute)ORTableMappingAttribute.GetCustomAttribute(type, typeof(ORTableMappingAttribute), true);

			if (tableAttr != null)
				result.TableName = tableAttr.TableName;
			else
				result.TableName = type.Name;

			MemberInfo[] mis = GetTypeMembers(type);

			foreach (MemberInfo mi in mis)
			{
				if (mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property)
				{
					ORMappingItemCollection items = CreateMappingItems(mi);

					MergeMappingItems(result, items);
				}
			}

			return result;
		}

		private static void MergeMappingItems(ORMappingItemCollection dest, ORMappingItemCollection src)
		{
			foreach (ORMappingItem item in src)
				if (dest.Contains(item.DataFieldName) == false)
					dest.Add(item);
		}

		private static ORMappingItemCollection CreateMappingItems(MemberInfo mi)
		{
			ORMappingItemCollection result = null;
			bool isDoMapping = false;

			RelativeAttributes attrs = null;

			if (mi.Name != "Item" && GetRealType(mi).GetInterface("ICollection") == null)
			{
				attrs = GetRelativeAttributes(mi);

				if (attrs.NoMapping == null)
					isDoMapping = true;
			}

			if (isDoMapping == true)
			{
				if (attrs != null)
				{
					if (attrs.SubClassFieldMappings.Count > 0 || attrs.SubClassFieldSqlBehaviors.Count > 0)
						result = GetMappingItemsBySubClass(attrs, mi);
					else
						result = GetMappingItems(attrs, mi);
				}
			}
			else
				result = new ORMappingItemCollection();

			return result;
		}

		private static ORMappingItemCollection GetMappingItems(RelativeAttributes attrs, MemberInfo mi)
		{
			ORMappingItemCollection items = new ORMappingItemCollection();

			ORMappingItem item = new ORMappingItem();
			item.PropertyName = mi.Name;
			item.DataFieldName = mi.Name;

			if (attrs.FieldMapping != null)
				FillMappingItemByAttr(item, attrs.FieldMapping);

			if (attrs.SqlBehavior != null)
				FillMappingItemByBehaviorAttr(item, attrs.SqlBehavior);

			item.MemberInfo = mi;

			items.Add(item);

			return items;
		}

		private static ORMappingItemCollection GetMappingItemsBySubClass(RelativeAttributes attrs, MemberInfo sourceMI)
		{
			ORMappingItemCollection items = new ORMappingItemCollection();
			System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

			MemberInfo[] mis = GetTypeMembers(subType);

			foreach (SubClassORFieldMappingAttribute attr in attrs.SubClassFieldMappings)
			{
				MemberInfo mi = GetMemberInfoByName(attr.SubPropertyName, mis);

				if (mi != null)
				{
					if (items.Contains(attr.DataFieldName) == false)
					{
						ORMappingItem item = new ORMappingItem();

						item.PropertyName = sourceMI.Name;
						item.SubClassPropertyName = attr.SubPropertyName;
						item.MemberInfo = mi;

						if (attrs.SubClassType != null)
							item.SubClassTypeDescription = attrs.SubClassType.TypeDescription;

						FillMappingItemByAttr(item, attr);

						items.Add(item);
					}
				}
			}

			foreach (SubClassSqlBehaviorAttribute attr in attrs.SubClassFieldSqlBehaviors)
			{
				ORMappingItem item = FindItemBySubClassPropertyName(attr.SubPropertyName, items);

				if (item != null)
					FillMappingItemByBehaviorAttr(item, attr);
			}

			return items;
		}

		internal static MemberInfo GetSubClassMemberInfoByName(string subClassPropertyName, MemberInfo sourceMI)
		{
			RelativeAttributes attrs = GetRelativeAttributes(sourceMI);

			System.Type subType = attrs.SubClassType != null ? attrs.SubClassType.Type : GetRealType(sourceMI);

			return GetSubClassMemberInfoByName(subClassPropertyName, subType);
		}

		internal static MemberInfo GetSubClassMemberInfoByName(string subClassPropertyName, System.Type subType)
		{
			MemberInfo[] mis = GetTypeMembers(subType);

			return GetMemberInfoByName(subClassPropertyName, mis);
		}

		private static MemberInfo[] GetTypeMembers(System.Type type)
		{
			List<MemberInfo> list = new List<MemberInfo>();

			PropertyInfo[] pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

			Array.ForEach(pis, delegate(PropertyInfo pi) { list.Add(pi); });

			FieldInfo[] fis = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

			Array.ForEach(fis, delegate(FieldInfo fi) { list.Add(fi); });

			return list.ToArray();
		}

		private static MemberInfo GetMemberInfoByName(string name, MemberInfo[] mis)
		{
			MemberInfo result = null;

			foreach (MemberInfo mi in mis)
			{
				if (mi.Name == name)
				{
					result = mi;
					break;
				}
			}

			return result;
		}

		private static void FillMappingItemByBehaviorAttr(ORMappingItem item, SqlBehaviorAttribute sba)
		{
			item.BindingFlags = sba.BindingFlags;
			item.DefaultExpression = sba.DefaultExpression;
			item.EnumUsage = sba.EnumUsage;
		}

		private static ORMappingItem FindItemBySubClassPropertyName(string subPropertyName, ORMappingItemCollection items)
		{
			ORMappingItem result = null;

			foreach (ORMappingItem item in items)
			{
				if (item.SubClassPropertyName == subPropertyName)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		private static void FillMappingItemByAttr(ORMappingItem item, ORFieldMappingAttribute fm)
		{
			item.DataFieldName = fm.DataFieldName;
			item.IsIdentity = fm.IsIdentity;
			item.IsNullable = fm.IsNullable;
			item.Length = fm.Length;
			item.PrimaryKey = fm.PrimaryKey;
		}

		private static System.Type GetRealType(MemberInfo mi)
		{
			System.Type type = null;

			switch (mi.MemberType)
			{
				case MemberTypes.Property:
					type = ((PropertyInfo)mi).PropertyType;
					break;
				case MemberTypes.Field:
					type = ((FieldInfo)mi).FieldType;
					break;
				default:
					ThrowInvalidMemberInfoTypeException(mi);
					break;
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "System.Nullable`1")
				type = type.GetGenericArguments()[0];

			return type;
		}

		private static void ThrowInvalidMemberInfoTypeException(MemberInfo mi)
		{
			throw new InvalidOperationException(string.Format(Resource.InvalidMemberInfoType,
				mi.Name,
				mi.MemberType));
		}

		private static ORMappingItemCollection InnerGetMappingInfoByObject<T>(T graph)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

			System.Type type = null;

			if (typeof(T).IsInterface)
				type = typeof(T);
			else
				type = graph.GetType();

			return InnerGetMappingInfo(type);
		}

		private static ORMappingItemCollection InnerGetMappingInfo(System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			ORMappingItemCollection result = null;

			if (ORMappingContextCache.Instance.TryGetValue(type, out result) == false)
			{
				result = ORMappingsCache.Instance.GetOrAddNewValue(type, 
					(cache, key) => cache.Add(key, GetMappingItemCollection(type)));
			}

			return result;
		}

		private static RelativeAttributes GetRelativeAttributes(MemberInfo mi)
		{
			RelativeAttributes result = new RelativeAttributes();

			Attribute[] attrs = Attribute.GetCustomAttributes(mi, true);

			foreach (Attribute attr in attrs)
			{
				if (attr is NoMappingAttribute)
				{
					result.NoMapping = (NoMappingAttribute)attr;
					break;
				}
				else
					if (attr is SubClassORFieldMappingAttribute)
						result.SubClassFieldMappings.Add((SubClassORFieldMappingAttribute)attr);
					else
						if (attr is SubClassSqlBehaviorAttribute)
							result.SubClassFieldSqlBehaviors.Add((SubClassSqlBehaviorAttribute)attr);
						else
							if (attr is SubClassTypeAttribute)
								result.SubClassType = (SubClassTypeAttribute)attr;
							else
								if (attr is ORFieldMappingAttribute)
									result.FieldMapping = (ORFieldMappingAttribute)attr;
								else
									if (attr is SqlBehaviorAttribute)
										result.SqlBehavior = (SqlBehaviorAttribute)attr;
			}

			return result;
		}
		#endregion ˽�з���
	}
}