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
    public class ProjectDataWeather
    {
        private MySQLConnect connector;
        public string starttime;
        public string endtime;
        public ProjectDataWeather()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=longduc;Uid=root;Pwd=101101;"
            };
        }
        public ProjectDataWeather(MySQLConnect connector)
        {
            this.connector = connector;
        }

        public List<WeatherParameter> GetProjectWeather(string ProjectName, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_weather";
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_DateTime = starttime,
                    

                });

                if (dataTable is null) return null;

                var list = new List<WeatherParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var startTime = row.Field<DateTime>("DateTime");
                    var irradian = row["Irradiation"].ToString();
                    var ambienttemperature = row["AmbientTemperature"].ToString();
                    var cell1temperature = row["ModuleTemperature1"].ToString();
                    var cell2temperature = row["ModuleTemperature2"].ToString();

                    if ((float.TryParse(irradian, out float irradianvalue))&& (float.TryParse(cell1temperature, out float cell1temperaturevalue))&& (float.TryParse(ambienttemperature, out float ambienttemperaturevalue))&& (float.TryParse(cell2temperature, out float cell2temperaturevalue)))
                    {
                        list.Add(new WeatherParameter()
                        {
                            DateTime = startTime,
                            Irradiation = irradianvalue,
                            AmbientTemperature= ambienttemperaturevalue,
                            Cell1Temperature = cell1temperaturevalue,
                            Cell2Temperature = cell2temperaturevalue

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
