using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
   public class ProjectCommonParameter
    {
        public DateTime DateTime { get; set; }
        public float TotalEnergyFromINV { get; set; }
        public float TotalEnergy { get; set; }
        public float SurplusPower { get; set; }
        public float PurchasedPower { get; set; }
        public float VoltageA { get; set; }
        public float CurrentA { get; set; }
        public float ActivePower { get; set; }
        public float ReactivePower { get; set; }
        public float PowerFactor { get; set; }
        public float AmbientAirTemperature { get; set; }
        public float Irradiance { get; set; }
        public float BackOfModuleTemperature1 { get; set; }
        public float BackOfModuleTemperature2 { get; set; }
    }
}
