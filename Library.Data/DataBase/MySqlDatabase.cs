using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using System.Collections.Generic;

using WD.Library.Data.Properties;
using MySql.Data.MySqlClient;
using WD.Library.Data.SqlServer;

namespace WD.Library.Data.MySql
{
	public class MySqlDatabase : SqlDatabase
    {
		public MySqlDatabase(string name)
			: base(name)
		{
			this.factory = MySqlClientFactory.Instance;
		}
	}
}
