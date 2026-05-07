using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
   public class WeatherParameter
    {
        public DateTime DateTime { get; set; }
        public float Irradiation { get; set; }
        public float AmbientTemperature { get; set; }
        public float Cell1Temperature { get; set; }
        public float Cell2Temperature { get; set; }
    }
}
