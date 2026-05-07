using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Parameter.Common
{
   public class LoginParameter
    {
        
        public string UserName { get; set; }
        public string Password { get; set; }
        public static string UserSession { get; set; }

    }
}
