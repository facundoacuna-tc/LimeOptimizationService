using LOS.Common.Entities;
using LOS.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Data.Repository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly IServiceScope _scope;
        private readonly AppDbContext _databaseContext;
        public SensorRepository(IServiceProvider services) 
        {

            _scope = services.CreateScope();
            _databaseContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();            
            
        }

        
        public async Task<IEnumerable<SensorValue>> GetAll()
        {
            var result = await _databaseContext.SensorValue.ToListAsync();

            return result;

        }


        private void SetData() 
        {
            //var sensorType = new SensorType { Id = 1, Name = "Flow" };

            //var sensor = new Sensor
            //{
            //    Id = 1,
            //    Name = "Sensor",
            //    Description = "Description",
            //    SensorTypeId = sensorType.Id
            //};

            //var sensorValue = new SensorValue
            //{
            //    SensorId = sensor.Id,
            //    Date = DateTime.Now,
            //    Value = "0.01"
            //};


            //_context.Add(sensorType);
            //_context.Add(sensor);
            //_context.Add(sensor);

            //_context.SaveChanges();

        }
    }
}
