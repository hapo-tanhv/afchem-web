using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hino.DatabaseConnector;
using Hino.Parameter.Common;

namespace Hino.Getdata.Common
{
    public class EventCommon
    {
        private MySQLConnect connector;
        public DateTime occurrencetimeValue;
        public DateTime restoretimeValue;
        public int index = 0;
        public EventCommon()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=scada;Uid=root;Pwd=101101;"
            };
        }
        public EventCommon(MySQLConnect connector)
        {
            this.connector = connector;
        }
        public List<EventParameter> GetEventLog(string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-08-30 00:00:00";
                    endtime = "2022-08-31 23:59:59";
                }
                var dataTable = this.connector.ExecuteStoreProcedure("proc_event", new
                {

                    p_start_time = starttime,
                    p_end_time = endtime,

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
