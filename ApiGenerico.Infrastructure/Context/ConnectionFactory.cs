using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using ApiGenerico.Utils.Security;

namespace ApiGenerico.Infrastructure.Context
{
    public class ConnectionFactory
    {
        public DbConnection GetOpenConnection(string cnx, bool openConnection = true)
        {
            var connection = new SqlConnection(new EncryptionService().Decrypt(cnx));
            SetNameApp(connection);

            if (openConnection)
                connection.Open();
            return connection;
        }

        private static void SetNameApp(DbConnection _connection)
        {
            if (_connection != null)
            {
                var stringConnection = _connection.ConnectionString;
                var idx = stringConnection.IndexOf("App=");
                if (idx == -1)
                {
                    stringConnection += "App=ApiTableroPowerBiFepep;Trust Server Certificate=true;";
                }
            }
        }
    }
}
