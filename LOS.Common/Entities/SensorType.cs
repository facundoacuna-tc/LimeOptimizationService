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
    public class SensorType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
