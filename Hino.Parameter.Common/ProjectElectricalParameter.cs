using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
    public class ProjectElectricalParameter
    {
        public DateTime DateTime { get; set; }
        public float ActivePower { get; set; }
        public float ReactivePower { get; set; }
        public float PowerFactor { get; set; }
        public float Frequency { get; set; }
        public float VoltageA { get; set; }
        public float VoltageB { get; set; }
        public float VoltageC { get; set; }
        public float CurrentA { get; set; }
        public float CurrentB { get; set; }
        public float CurrentC { get; set; }
    }
}
