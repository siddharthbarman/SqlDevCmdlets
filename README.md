# SqlDevCmdlets
### **Description**

Bunch of Powershell cmdlets for SQL and .NET developers. As of today, it consists of two PowerShell command Get-TableSql and Get-TableVariableSql. Both the cmdlets allow one to generate a table or table variable definition from the output of a query. For example, its possible to generate the following SQL: 

create table #TempTable
(
        BusinessEntityId int CONSTRAINT MyPK PRIMARY KEY NOT NULL,
        FirstName nvarchar(50) NOT NULL,
        MiddleName nvarchar(50) NULL,
        LastName nvarchar(50) NOT NULL,
        ModifiedDate datetime NOT NULL
)

as a result of output of a select query.



### Compiling the code

The code is written in Visual Studio 2019. It should be possible to load the solution/project in earlier versions like 2017 also. The project requires you to have .NET Framework 4.5 (SDK) installed.



### Using the Cmdlets

Compiling the project results in a .NET assembly named SqlDevCmdlets.dll. Open an instance of PowerShell. CD into to the build folder e.g. D:\SqlDevCmdlets\bin\Debug. Load the cmdlet using `Import-Module .\SqlDevCmdlets.dll`



### Viewing help for the cmdlets

Once the module is loaded, type in: 

Get-Help Get-TableSql -full`

Get-Help Get-TableVariableSql -full`



### Get in touch

For questions, comments, suggestions, email to sbytestream@outlook.com 

You can also visit https://sbytestream.pythonanywhere.com

This software is free. Please see LICENSE file for details on licensing.

Copyright 2019 Siddharth Barman.




=========================================
