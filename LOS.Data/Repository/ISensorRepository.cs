using LOS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Data.Repository
{
    public interface ISensorRepository
    {        
        Task<IEnumerable<SensorValue>> GetAll();

    }
}
