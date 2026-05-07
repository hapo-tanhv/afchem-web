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
   public class ProjectDataEnergy
    {
        private MySQLConnect connector;
        public string starttime;
        public string endtime;
        public ProjectDataEnergy()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=thanglong3;Uid=root;Pwd=101101;"
            };
        }
        public ProjectDataEnergy(MySQLConnect connector)
        {
            this.connector = connector;
        }

        //Get data for Energy (for Project page)
        public List<ProjectEnergyParameter> GetProjectSolarEnergy(string ProjectName, int TimeUnit, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_energy";            
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit = 3,
                    selectProc = TimeUnit

                });

                if (dataTable is null) return null;

                var list = new List<ProjectEnergyParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var startTime = row.Field<DateTime>("StartTime");
                    var solar = row["Solar"].ToString();
                    
                    if (float.TryParse(solar, out float solarvalue))
                    {
                        list.Add(new ProjectEnergyParameter()
                        {
                            DateTime = startTime,
                            Value = solarvalue,
                            
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

        // Get data for Power (for Project page)
        public List<ProjectPowerParameter> GetProjectSolarPower(string datetime, string ProjectName)
        {
            try
            {
                if (datetime == null)
                {
                    datetime = "2022-06-23 00:00:00";
                }
                var procName = $"proc_{ProjectName}_power";

                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_DateTime = datetime
                });

                if (dataTable is null) return null;

                var list = new List<ProjectPowerParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var time = row.Field<DateTime>("DateTime");
                    var solarvalue = row["Solar"].ToString();
                   
                    if (float.TryParse(solarvalue, out float SolarValue))
                    {
                        list.Add(new ProjectPowerParameter()
                        {
                            DateTime = time,
                            Value = SolarValue,                           
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
