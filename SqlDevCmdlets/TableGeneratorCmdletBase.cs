// Copyright(c) 2019-2020, Siddharth R Barman
// Web : https://sbytestream.pythonanywhere.com
// Mail: sbytestream@outlook.com 

using Siddharth.CodeGen;
using System;
using System.Management.Automation;

namespace Siddharth.SqlDevCmdlets {
	public class TableGeneratorCmdletBase : Cmdlet {
		[Parameter(ValueFromPipelineByPropertyName = true, Position = 0)]
		public string ConnectionString {
			get;
			set;
		}
	
		[Parameter(ValueFromPipelineByPropertyName = true, Position = 1)]
		public string Query {
			get;
			set;
		}

		[Parameter(ValueFromPipelineByPropertyName = true, Position = 2)]
		public string OutputName {
			get;
			set;
		}

		[Parameter(ValueFromPipelineByPropertyName = true, Position = 3)]
		public SwitchParameter GeneratePrimaryKey {
			get;
			set;
		}

		[Parameter(ValueFromPipelineByPropertyName = true, Position = 4)]
		public string PrimaryKeyName {
			get;
			set;
		}
		protected override void ProcessRecord() {
			if (string.IsNullOrEmpty(ConnectionString)) {
				WriteError(new ErrorRecord(new ApplicationException(), "ConnectionString not set", ErrorCategory.NotSpecified, "ConnectionString"));
				return;
			}			

			if (string.IsNullOrEmpty(Query)) {
				WriteError(new ErrorRecord(new ApplicationException(), "Query not set", ErrorCategory.NotSpecified, "Query"));
				return;
			}

			if (string.IsNullOrEmpty(OutputName)) {
				WriteError(new ErrorRecord(new ApplicationException(), "OutputName not set", ErrorCategory.NotSpecified, "OutputName"));
				return;
			}
		}

		protected string GenerateTableVariable(string connStr, string variableName, string query) {
			SqlSchemaGenerator generator = new SqlSchemaGenerator(connStr);
			GenerateOptions options = new GenerateOptions {
				GenerateKeys = this.GeneratePrimaryKey,
				PrimaryKeyName = this.PrimaryKeyName
			};
			return generator.GenerateTableVariable(variableName, query, options);
		}

	}
}
