// Copyright(c) 2019-2020, Siddharth R Barman
// Web : https://sbytestream.pythonanywhere.com
// Mail: sbytestream@outlook.com 
/*
 ______   _____      __        ______      _____  __   __   
/ ____/\ / ___ (    /\_\      /_/\___\   /\_____\/_/\ /\_\  
) ) __\// /\_/\ \  ( ( (      ) ) ___/  ( (_____/) ) \ ( (  
 \ \ \ / /_/ (_\ \  \ \_\    /_/ /  ___  \ \__\ /_/   \ \_\ 
 _\ \ \\ \ )_/ / (  / / /__  \ \ \_/\__\ / /__/_\ \ \   / / 
)____) )\ \/_\/ \_\( (_____(  )_)  \/ _/( (_____\)_) \ (_(  
\____\/  \_____\/_/ \/_____/  \_\____/   \/_____/\_\/ \/_/  
*/

using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Siddharth.CodeGen {
	public class GenerateOptions {
		public string PrimaryKeyName;
		public bool GenerateKeys;
	}
	
	public class SqlSchemaGenerator {
		public SqlSchemaGenerator(string connectionString) {
			m_connStr = connectionString;
		}

		public string GenerateTableSql(string variableName, string query, GenerateOptions options) {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("create table {0}\n", variableName);
			sb.Append("(\n");
			sb.Append(GenerateTableBodySql(query, options));
			sb.AppendLine("\n);");
			return sb.ToString();
		}

		public string GenerateTableVariable(string variableName, string query, GenerateOptions options) {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("declare {0} table\n", variableName);
			sb.Append("(\n");
			sb.Append(GenerateTableBodySql(query, options));
			sb.AppendLine("\n);");
			return sb.ToString();
		}

		private string GenerateTableBodySql(string query, GenerateOptions options) {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection conn = new SqlConnection(m_connStr)) {
				conn.Open();
				using (SqlCommand cmd = new SqlCommand(query, conn)) {
					DataTable table = GetSchemaTable(cmd);
					bool compositePK = table.Select("IsKey = 1").Length > 1;
					bool first = true;

					foreach (DataRow row in table.Rows) {
						if (!first)
							sb.Append(",\n");						

						sb.AppendFormat("\t{0}", GetColumnDefinition(row, compositePK, options));
						first = false;
					}

					if (compositePK) {
						sb.AppendFormat(",\n\t{0}", GetCompositePK(table, options.PrimaryKeyName));
					}
				}
			}
			return sb.ToString();
		}

		bool IsSizedColumn(string columnType) {
			return (string.Equals(columnType, "varchar") || string.Equals(columnType, "nvarchar")
				|| string.Equals(columnType, "varbinary"));
		}

		string GetColumnSize(int size) {
			return size == 2147483647 ? "MAX" : size.ToString();
		}

		DataTable GetSchemaTable(SqlCommand cmd) {
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(string));
			table.Columns.Add("DataTypeName", typeof(string));
			table.Columns.Add("Size", typeof(string));
			table.Columns.Add("IsKey", typeof(bool));
			table.Columns.Add("AllowDBNull", typeof(bool));

			DataTable rawInfoTable = null;
			using (var reader = cmd.ExecuteReader()) {
				rawInfoTable = reader.GetSchemaTable();
				foreach (DataRow row in rawInfoTable.Rows) {
					var newrow = table.NewRow();
					table.Rows.Add(newrow);
					newrow["ColumnName"] = row["ColumnName"];
					newrow["DataTypeName"] = row["DataTypeName"];
					newrow["Size"] = IsSizedColumn((string)row["DataTypeName"]) ? GetColumnSize((int)row["ColumnSize"]) : string.Empty;
				}
			}
			table.AcceptChanges();

			DataTable resolvedInfoTable = null;
			using (SqlDataAdapter adapter = new SqlDataAdapter(cmd)) {
				adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
				DataTable tempDataTable = new DataTable();
				adapter.Fill(tempDataTable);

				using (var reader = new DataTableReader(tempDataTable)) {
					resolvedInfoTable = reader.GetSchemaTable();
					foreach (DataRow row in resolvedInfoTable.Rows) {
						string select = string.Format("ColumnName = '{0}'", row["ColumnName"]);
						DataRow resultRow = table.Select(select)[0];
						resultRow["IsKey"] = row["IsKey"];
						resultRow["AllowDBNull"] = row["AllowDBNull"];
					}
				}
			}

			return table;
		}

		string GetColumnDefinition(DataRow row, bool isCompositeKey, GenerateOptions options) {
			StringBuilder sb = new StringBuilder((string)row["ColumnName"]);
			sb.AppendFormat(" {0}", (string)row["DataTypeName"]);

			string size = (string)row["Size"];
			if (!string.IsNullOrEmpty(size)) {
				sb.AppendFormat("({0})", size);
			}

			bool isKey = (bool)row["IsKey"];
			if (!isCompositeKey && options.GenerateKeys && isKey) {
				if (string.IsNullOrEmpty(options.PrimaryKeyName)) {
					sb.Append(" PRIMARY KEY");
				}
				else {
					sb.AppendFormat(" CONSTRAINT {0} PRIMARY KEY", options.PrimaryKeyName);
				}
			}

			sb.AppendFormat(" {0}", (bool)row["AllowDBNull"] ? "NULL" : "NOT NULL");
			return sb.ToString();
		}

		string GetCompositePK(DataTable table, string pkName) {
			var rows = table.Select("IsKey = 1");
			bool firstColumn = true;
			StringBuilder sb = new StringBuilder();
			foreach(DataRow row in rows) {
				if (sb.Length == 0) {
					if (!string.IsNullOrEmpty(pkName)) {
						sb.AppendFormat("constraint {0} primary key (", pkName);
					}
					else {
						sb.Append("primary key (");
					}
				}
				if (!firstColumn) {
					sb.Append(", ");					
				}
				else {
					firstColumn = false;
				}
				sb.Append((string)row["ColumnName"]);				
			}
			sb.Append(")");
			return sb.ToString();
		}

		private string m_connStr;
	}
}
