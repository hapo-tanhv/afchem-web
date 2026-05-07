using Hino.DatabaseConnector;
using Hino.Parameter.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hino.GetData.Common
{
    public class EnergyPowerData
    {
        private MySQLConnect connector;
        public string starttime;
        public string endtime;
        public EnergyPowerData()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=longduc;Uid=root;Pwd=101101;"
            };
        }
        public EnergyPowerData(MySQLConnect connector)
        {
            this.connector = connector;
        }

        //Get data for Energy (for Overview page)
        public List<EnergyParameter> GetSolarEnergy(int TimeUnit, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = "";
                switch(TimeUnit)
                {
                    case 1:
                        procName = "proc_total_energy_day";
                        break;
                    case 2:
                        procName = "proc_total_energy_month";
                        break;
                    case 3:
                        procName = "proc_total_energy_year";
                        break;
                }
                
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit = 3,
                    

                });

                if (dataTable is null) return null;

                var list = new List<EnergyParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    
                    var startTime = row.Field<DateTime>("StartTime");
                    var solar = row["Solar"].ToString();
                    var grid = row["Grid"].ToString();
                    if (float.TryParse(solar, out float solarvalue) && float.TryParse(grid, out float gridvalue))
                    {
                        list.Add(new EnergyParameter()
                        {
                            DateTime = startTime,
                            SolarValue = solarvalue,
                            GridValue = gridvalue
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

        // Get data for Power (for Overview page)
        public List<PowerParameter> GetSolarPower(string datetime)
        {
            try
            {
                if (datetime == null)
                {
                    datetime = "2022-06-23 00:00:00";
                }
                var procName = "proc_total_power";

                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_DateTime = datetime
                });

                if (dataTable is null) return null;

                var list = new List<PowerParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var time = row.Field<DateTime>("DateTime");
                    var solarvalue = row["Solar"].ToString();
                    
                    if (float.TryParse(solarvalue, out float SolarValue))
                    {
                        list.Add(new PowerParameter()
                        {
                            DateTime = time,
                            SolarValue = SolarValue,                           
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


