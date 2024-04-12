using LOS.Common.Configurations;
using LOS.Data.ADO.Interface;
using LOS.Services.ImporterManager.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services.ImporterManager.Implementation
{
    public class CSVDataImporterManager : ICSVDataImporter
    {

        private ICSVDataReader _reader;
        private IDatabaseConnection _connection;
        protected readonly ILogger<CSVDataImporterManager> _logger;

        public CSVDataImporterManager(ICSVDataReader reader, IDatabaseConnection connection, ILoggerFactory loggerFactory)
        {
            _reader = reader;
            _connection = connection;
            _logger = loggerFactory.CreateLogger<CSVDataImporterManager>();
        }


        /// <summary>
        /// Allows import CSV information from the configured CSV to the destination database
        /// </summary>
        /// <param name="settings">settings with configuration parameters</param>
        /// <returns></returns>
        public async Task ImportCsvFile(AppSettings settings)
        {
            _logger.LogInformation("Preparing import CSV rows", "ImportCsvFile");

            try
            {
                _logger.LogInformation("Connect to input file", "ImportCsvFile");
                _reader.DatareaderConfiguration(settings.CsvFilePath, settings.CsvDelimiter[0], settings.CsvFileType);

                _logger.LogInformation("Connect to output database", "ImportCsvFile");
                _connection.DatabaseConnectionConfiguration(settings.ConnectionString, settings.TableName);

                _logger.LogInformation("Execute import CSV rows", "ImportCsvFile");
                await _connection.InsertReaderIntoDatabase(_reader);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
