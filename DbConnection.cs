namespace SRT.SqlClient
{
    public static class DbConnection
    {
        private static string? _connection = null;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connection))
                    throw new InvalidOperationException("Connection string is not set.");
                return _connection;
            }
            set { _connection = value; }
        }
    }
}
