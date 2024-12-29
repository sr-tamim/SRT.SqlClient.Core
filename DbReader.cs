using Microsoft.Data.SqlClient;

namespace SRT.SqlClient
{
    public class DbReader : IDisposable
    {
        private bool disposed = false;
        private readonly string connectionString = string.Empty;
        private SqlConnection? connection;

        public DbReader()
        {
            connectionString = DbConnection.ConnectionString;
        }

        public DbReader(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Open the connection
        private void OpenConnection()
        {
            connection ??= new SqlConnection(connectionString);   // Create the connection if it is null
            // Open the connection if it is closed
            if (connection != null && connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        // Close the connection
        private void CloseConnection()
        {
            // Close the connection if it is open
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
                CloseConnection();
            disposed = true;
        }

        public SqlDataReader ExecuteCommand(string query)
        {
            OpenConnection();
            var sql = new SqlCommand(query, connection);
            return sql.ExecuteReader();
        }

        public SqlDataReader ExecuteCommand(string query, List<SqlParameter> parameters)
        {
            OpenConnection();
            var sql = new SqlCommand(query, connection);
            foreach (var param in parameters)
            {
                sql.Parameters.Add(param);
            }
            return sql.ExecuteReader();
        }

        public List<T> GetDataList<T>(string query, List<SqlParameter>? parameters = null)
        {
            OpenConnection();
            var sql = new SqlCommand(query, connection);
            if (parameters != null)
                foreach (var param in parameters)
                {
                    sql.Parameters.Add(param);
                }
            var res = sql.ExecuteReader();
            List<T> list = [];
            while (res.Read())
            {
                var obj = (T)(Activator.CreateInstance(typeof(T)) ?? new object());
                var schemaTable = res.GetSchemaTable();
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (schemaTable.Rows.Cast<System.Data.DataRow>().Any(row => row["ColumnName"].ToString() == prop.Name) == false) continue;
                    if (res[prop.Name] == DBNull.Value) continue;
                    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    {
                        prop.SetValue(obj, res[prop.Name] == DBNull.Value ? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? 0 : (int?)null
                        ) : Convert.ToInt32(res[prop.Name]));
                    }
                    else if (prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                    {
                        prop.SetValue(obj, res[prop.Name] == DBNull.Value ? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? 0 : (long?)null
                        ) : Convert.ToInt64(res[prop.Name]));
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(obj, res[prop.Name].ToString() ?? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? string.Empty : (string?)null
                        ));
                    }
                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        prop.SetValue(obj, res[prop.Name] == DBNull.Value ? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? DateTime.MinValue : (DateTime?)null
                        ) : Convert.ToDateTime(res[prop.Name]));
                    }
                    else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                    {
                        prop.SetValue(obj, res[prop.Name] == DBNull.Value ? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? false : (bool?)null
                        ) : Convert.ToBoolean(res[prop.Name]));
                    }
                    else if (prop.PropertyType == typeof(TimeSpan) || prop.PropertyType == typeof(TimeSpan?))
                    {
                        prop.SetValue(obj, res[prop.Name] == DBNull.Value ? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? TimeSpan.MinValue : (TimeSpan?)null
                        ) : TimeSpan.Parse(res[prop.Name].ToString()!));
                    }
                    else if (prop.PropertyType == typeof(DateTimeOffset) || prop.PropertyType == typeof(DateTimeOffset?))
                    {
                        prop.SetValue(obj, res[prop.Name] == DBNull.Value ? (
                            Nullable.GetUnderlyingType(prop.PropertyType) == null ? DateTimeOffset.MinValue : (DateTimeOffset?)null
                        ) : DateTimeOffset.Parse(res[prop.Name].ToString()!));
                    }
                }
                list.Add(obj);
            }
            res.Close();
            return list;
        }

        // Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Destructor
        ~DbReader()
        {
            Dispose(false);
        }
    }
}
