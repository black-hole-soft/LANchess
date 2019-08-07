using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace Ajedrez
{
    public class My_SQL
    {
		private OdbcCommand odbcCommand1,odbcCommand2;
        private OdbcConnection odbcConnection1, odbcConnection2;

        public My_SQL()
        {
            string cadCxn = "DSN=chess;User=root;PWD=Gera1910;Database=chess";
			odbcConnection1 = new OdbcConnection();
			odbcConnection1.ConnectionString = cadCxn;
			odbcConnection2 = new OdbcConnection();
			odbcConnection2.ConnectionString = cadCxn;
			odbcCommand1 = new OdbcCommand();
			odbcCommand2 = new OdbcCommand();
			odbcCommand1.Connection = odbcConnection1;
			odbcCommand2.Connection = odbcConnection2;
			odbcConnection1.Open();
			odbcConnection2.Open();
		}
		public OdbcDataReader hazConsulta(string consulta)
        {
			odbcCommand1.CommandText = consulta;
			return odbcCommand1.ExecuteReader();
		}
		public int hazNoConsulta(string consulta)
        {
			odbcCommand2.CommandText = consulta;
			return odbcCommand2.ExecuteNonQuery();	
		}
	}
}
