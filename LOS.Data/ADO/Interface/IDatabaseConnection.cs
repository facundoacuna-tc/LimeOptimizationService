using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Data.ADO.Interface
{
    public interface IDatabaseConnection
    {
        void DatabaseConnectionConfiguration(string connectionString, string tablename);

        Task InsertReaderIntoDatabase(IDataReader reader);
    }
}
