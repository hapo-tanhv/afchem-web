using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
    public class ProjectSignageParameter
    {
        public DateTime DateTime { get; set; }

        public float SolarPower { get; set; }
        public float SelfConsumption { get; set; }
        public float PurchasedPower { get; set; }
    }
}
