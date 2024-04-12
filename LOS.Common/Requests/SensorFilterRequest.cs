using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Common.Requests
{
    public class SensorFilterRequest
    {
        public string Sensor { get; set; }
        public string SensorType { get; set; }
        public DateTime Since { get; set; }
        public DateTime To { get; set; }
    }
}
