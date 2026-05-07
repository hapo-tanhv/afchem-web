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
    public enum Role
    {
        None,
        Admin,
        Project1,
        Project2,
        Project3,
        Project4,
        Project5,
        Project6,
        Project7,
        SolEnergy,
        JGC,
        Hino,
        LDIP
    }
    public class Authentication
    {
        private MySQLConnect connector;
       
        public Authentication()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=127.0.0.1;Database=scada;Uid=root;Pwd=101101;"
            };
        }
        public Authentication(MySQLConnect connector)
        {
            this.connector = connector;
        }

        public Role CheckUser(string username, string password)
        {
            try
            {
                var query = $"select `Role` from `account` where binary `UserName` = '{username}' and binary `Password` = '{password}' limit 1";
                var role = this.connector.ExecuteScalarQuery(query);
                return (Role)Convert.ToInt32(role);
            }
            catch
            {
                return Role.None;
            }
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                var query = $"update  `account` set `Password` = '{newPassword}' where " +
                    $"`ID` > 0  and " +
                    $"`UserName` = '{userName}' and " +
                    $"`Password` = '{oldPassword}'";
                var result = this.connector.ExecuteNonQuery(query);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
       
    }
}
