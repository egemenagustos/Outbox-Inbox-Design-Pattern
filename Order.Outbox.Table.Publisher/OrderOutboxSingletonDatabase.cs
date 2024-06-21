using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Order.Outbox.Table.Publisher
{
    public static class OrderOutboxSingletonDatabase
    {
        static IDbConnection _connection;
        static bool _dataReaderState = true;

        public static async Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery)
            => await _connection.QueryAsync<T>(sqlQuery);

        public static async Task<int> ExecuteAsync(string sqlQuery)
            => await _connection.ExecuteAsync(sqlQuery);

        public static IDbConnection Connection
        {
            get
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                return _connection;
            }
        }

        public static void DataReaderReady()
            => _dataReaderState = true;

        public static void DataReaderBusy()
            => _dataReaderState = false;

        public static bool DataReaderState => _dataReaderState;

        static OrderOutboxSingletonDatabase()
            => _connection = new SqlConnection("Data Source=DESKTOP-6LCK92O;Initial Catalog=Order.Api;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }

}
