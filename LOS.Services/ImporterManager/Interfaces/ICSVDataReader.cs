using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services.ImporterManager.Interfaces
{
    public interface ICSVDataReader : IDataReader, IDisposable
    {
        void DatareaderConfiguration(string filePath, char delimiter, string fileType, bool firstRowHeader = true);

    }
}
