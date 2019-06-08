// Copyright(c) 2019-2020, Siddharth R Barman
// Web : https://sbytestream.pythonanywhere.com
// Mail: sbytestream@outlook.com 

using Siddharth.CodeGen;
using System.Management.Automation;

namespace Siddharth.SqlDevCmdlets {
	[Cmdlet(VerbsCommon.Get, "TableSql")]
	public class TableGeneratorCmdlet : TableGeneratorCmdletBase {
		protected override void ProcessRecord() {
			base.ProcessRecord();
			SqlSchemaGenerator generator = new SqlSchemaGenerator(ConnectionString);
			GenerateOptions options = new GenerateOptions {
				GenerateKeys = this.GeneratePrimaryKey,
				PrimaryKeyName = this.PrimaryKeyName
			};
			string output = generator.GenerateTableSql(OutputName, Query, options);			
			WriteObject(output);
		}
	}
}
