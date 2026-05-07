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
        public float ReflectedIrradiation { get; set; }
        public float AmbientTemperature { get; set; }
    }
}
