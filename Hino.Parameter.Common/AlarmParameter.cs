using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
   public class AlarmParameter
    {
        public int count { get; set; }
        public string OccurrenceTime { get; set; }
        public string RestoreTime { get; set; }
        public string TagNo { get; set; }
        public string Location { get; set; }
        public int FaultCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
