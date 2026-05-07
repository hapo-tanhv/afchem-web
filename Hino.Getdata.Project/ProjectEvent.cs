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
   public class ProjectEvent
    {
        private MySQLConnect connector;
        public DateTime occurrencetimeValue;
        public DateTime restoretimeValue;
        public int index = 0;
        public ProjectEvent()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=192.168.2.11;Database=scada;Uid=root;Pwd=101101;"
            };
        }
        public ProjectEvent(MySQLConnect connector)
        {
            this.connector = connector;
        }

        public List<EventParameter> GetEventProject(string starttime, string endtime, string projectname, string invertername)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-08-30 00:00:00";
                    endtime = "2022-08-31 23:59:59";
                }
                var dataTable = this.connector.ExecuteStoreProcedure("proc_event_web", new
                {

                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_project = projectname,
                    p_inverter = invertername

                });

                if (dataTable is null) return null;

                var list = new List<EventParameter>();
                index = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    var locationValue = row["Location"].ToString();
                    var descriptionValue = row["Description"].ToString();
                    var statusValue = row["Status"].ToString();
                    if (string.IsNullOrEmpty(row["OccurrenceTime"].ToString()) == false)
                    {
                        occurrencetimeValue = row.Field<DateTime>("OccurrenceTime");
                    }
                    list.Add(new EventParameter()
                    {
                        count = index,
                        OccurrenceTime = occurrencetimeValue.ToString("yyyy/MM/dd HH:mm:ss"),
                        Location = locationValue,
                        Description = descriptionValue,
                        Status = statusValue
                    });
                    index++;
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
