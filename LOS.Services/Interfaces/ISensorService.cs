using LOS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services.Interfaces
{
    public interface ISensorService
    {
        Task<IEnumerable<SensorValue>> GetAllValues();

        Task<IEnumerable<SensorValue>> GetAllValuesByFilter(string Sensor, string SensorType, DateTime Since, DateTime To);

    }
}
