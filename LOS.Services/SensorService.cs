using LOS.Common.Entities;
using LOS.Data.Repository;
using LOS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services
{
    public class SensorService : ISensorService
    {
        private readonly ISensorRepository _sensorRepository;

        public SensorService(ISensorRepository sensorRepository) 
        { 
            _sensorRepository = sensorRepository;
        }

        public async Task<IEnumerable<SensorValue>> GetAllValues()
        {
            var result = await _sensorRepository.GetAll();
            return result;
        }

        public async Task<IEnumerable<SensorValue>> GetAllValuesByFilter(string Sensor, string SensorType, DateTime Since, DateTime To)
        {
            var result = await _sensorRepository.GetAll();
            return result;

        }

        
    }
}
