using LOS.Common.Configurations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services.ImporterManager.Interfaces
{
    public interface ICSVDataImporter
    {
        Task ImportCsvFile(AppSettings _settings);
    }
}
