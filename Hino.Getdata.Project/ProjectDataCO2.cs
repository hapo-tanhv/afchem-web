using Hino.DatabaseConnector;
using Hino.Parameter.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.Getdata.Project
{
   public class ProjectDataCO2
    {
        private MySQLConnect connector;
        public string starttime;
        public string endtime;

        public ProjectDataCO2()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=192.168.2.11;Database=longduc;Uid=root;Pwd=101101;"
            };
        }

        public ProjectDataCO2(MySQLConnect connector)
        {
            this.connector = connector;
        }

        public List<ProjectEnergyParameter> GetProjectCO2(string ProjectName, int TimeUnit, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_CO2_day";
                if(TimeUnit==1)
                {
                    procName= $"proc_{ProjectName}_CO2_day";
                }
                else if (TimeUnit == 2)
                {
                    procName = $"proc_{ProjectName}_CO2_month";
                }
                else if (TimeUnit == 3)
                {
                    procName = $"proc_{ProjectName}_CO2_year";
                }
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit = 3                    
                });

                if (dataTable is null) return null;

                var list = new List<ProjectEnergyParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var startTime = row.Field<DateTime>("StartTime");
                    var co2 = row["Value"].ToString();

                    if (float.TryParse(co2, out float co2value))
                    {
                        list.Add(new ProjectEnergyParameter()
                        {
                            DateTime = startTime,
                            Value = co2value,

                        });
                    }
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
