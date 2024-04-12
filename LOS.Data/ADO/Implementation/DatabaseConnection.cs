using LOS.Data.ADO.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Data.ADO.Implementation
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private string _connectionString;
        private string _tablename;

        SqlConnection connection;
        SqlCommand command;

        protected readonly ILogger<DatabaseConnection> _logger;

        public DatabaseConnection(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DatabaseConnection>();
        }

        /// <summary>
        /// Configuration for setting connection properties
        /// </summary>
        /// <param name="connectionString">connection string for database</param>
        /// <param name="tablename"></param>
        public void DatabaseConnectionConfiguration(string connectionString, string tablename)
        {
            _connectionString = connectionString;
            _tablename = tablename;
        }


        /// <summary>
        /// Inserts datareader information into database
        /// </summary>
        /// <param name="reader">object with data information</param>
        /// <returns>asynchronus operation</returns>
        public async Task InsertReaderIntoDatabase(IDataReader reader)
        {
            if (GetCountRows() > 0)
                DeleteRecordsFromDB();

            try
            {
                SqlBulkCopy bulkcopy = new SqlBulkCopy(_connectionString, SqlBulkCopyOptions.UseInternalTransaction);
                bulkcopy.BatchSize = 1000;
                bulkcopy.DestinationTableName = _tablename;
                bulkcopy.NotifyAfter = 1000;
                bulkcopy.SqlRowsCopied += (sender, e) =>
                {
                    Console.WriteLine("Written: " + e.RowsCopied.ToString());
                };
                await bulkcopy.WriteToServerAsync(reader);
                bulkcopy.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }



        /// <summary>
        /// Deletes all records from table
        /// </summary>
        public void DeleteRecordsFromDB()
        {
            connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand("DELETE FROM " + _tablename, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        /// <summary>
        /// Get the current count of rows from the table
        /// </summary>
        /// <returns>quantity rows</returns>
        public int GetCountRows()
        {
            int count = 0;
            connection = new SqlConnection(_connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand("SELECT COUNT(*) FROM " + _tablename, connection);
                count = (Int32)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return count;
        }
    }
}
