using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
    public class EnergyParameter
    {
        public DateTime DateTime { get; set; }

        public float SolarValue { get; set; }
        public float GridValue { get; set; }
    }
}
