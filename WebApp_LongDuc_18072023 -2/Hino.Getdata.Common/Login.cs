using Hino.DatabaseConnector;
using Hino.Parameter.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Getdata.Common
{
   public class Login
    {
        private MySQLConnect connector;
       
        public Login()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=scada;Uid=root;Pwd=101101;"
            };
        }
        public Login(MySQLConnect connector)
        {
            this.connector = connector;
        }
        public List<LoginParameter> GetDataLogin(string username, string password)
        {
            try
            {
                var dataTable = this.connector.ExecuteQuery("SELECT * FROM scada.account");
                if (dataTable is null) return null;
                var list = new List<LoginParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var Username = row["UserName"].ToString();
                    var Password = row["Password"].ToString();
                    
                        list.Add(new LoginParameter()
                        {
                            UserName = Username,
                            Password = Password
                        });
                    
                }
                return list;
            }
            catch
            {
                return null;
            }
            
        }
    }
}
