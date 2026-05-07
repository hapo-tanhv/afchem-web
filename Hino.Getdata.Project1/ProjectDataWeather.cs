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
                ConnectionString = $"Server=localhost;Database=thanglong3;Uid=root;Pwd=101101;"
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
                    p_start_time = starttime,
                    p_end_time = endtime

                });

                if (dataTable is null) return null;

                var list = new List<WeatherParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var startTime = row.Field<DateTime>("StartTime");
                    var irradian = row["Irradiation"].ToString();
                    var reflectedirradiation = row["ReflectedIrradiation"].ToString();
                    var ambienttemperature = row["AmbientTemperature"].ToString();

                    if ((float.TryParse(irradian, out float irradianvalue))&& (float.TryParse(reflectedirradiation, out float reflectedirradiationvalue))&& (float.TryParse(ambienttemperature, out float ambienttemperaturevalue)))
                    {
                        list.Add(new WeatherParameter()
                        {
                            DateTime = startTime,
                            Irradiation = irradianvalue,
                            ReflectedIrradiation=reflectedirradiationvalue,
                            AmbientTemperature=ambienttemperaturevalue

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
