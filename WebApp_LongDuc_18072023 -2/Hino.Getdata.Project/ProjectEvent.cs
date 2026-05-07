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
        public ProjectEvent()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=scada;Uid=root;Pwd=101101;"
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
                foreach (DataRow row in dataTable.Rows)
                {
                    var tagnoValue = row["TagNo"].ToString();
                    var locationValue = row["Location"].ToString();
                    var descriptionValue = row["Description"].ToString();
                    var statusValue = row["Status"].ToString();
                    if (string.IsNullOrEmpty(row["OccurrenceTime"].ToString()) == false)
                    {
                        occurrencetimeValue = row.Field<DateTime>("OccurrenceTime");
                    }
                    list.Add(new EventParameter()
                    {
                        OccurrenceTime = occurrencetimeValue.ToString("yyyy/MM/dd HH:mm:ss"),
                        TagNo = tagnoValue,
                        Location = locationValue,
                        Description = descriptionValue,
                        Status = statusValue
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
