using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LOS.Common.Constants;


namespace LOS.Common.Entities
{
    public class SensorValue
    {
        public int SensorId { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }

    }
}
