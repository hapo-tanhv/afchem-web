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
    public class AlarmCommon
    {
        private MySQLConnect connector;
        public DateTime occurrencetimeValue;
        public DateTime restoretimeValue;
        public int index = 0;
        public AlarmCommon()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=scada;Uid=root;Pwd=101101;"
            };
        }
        public AlarmCommon(MySQLConnect connector)
        {
            this.connector = connector;
        }
        public List<AlarmParameter> GetAlarmLog(string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-08-30 00:00:00";
                    endtime = "2022-08-31 23:59:59";
                }
                var dataTable = this.connector.ExecuteStoreProcedure("proc_alarm", new
                {

                    p_start_time = starttime,
                    p_end_time = endtime,

                });

                if (dataTable is null) return null;

                var list = new List<AlarmParameter>();
                index = 1;
                foreach (DataRow row in dataTable.Rows)
                {                    
                    var tagnoValue = row["TagNo"].ToString();
                    var locationValue = row["Location"].ToString();
                    var descriptionValue = row["Description"].ToString();
                    var statusValue = row["Status"].ToString();
                    if (string.IsNullOrEmpty(row["RestoreTime"].ToString()) == false)
                    {
                        restoretimeValue = row.Field<DateTime>("RestoreTime");
                    }
                    else
                    {
                        restoretimeValue = row.Field<DateTime>("OccurrenceTime");
                    }

                    if (string.IsNullOrEmpty(row["OccurrenceTime"].ToString()) == false)
                    {
                        occurrencetimeValue = row.Field<DateTime>("OccurrenceTime");
                    }                   

                    list.Add(new AlarmParameter()
                    {
                        count = index,
                        OccurrenceTime = occurrencetimeValue.ToString("yyyy/MM/dd HH:mm:ss"),
                        RestoreTime = restoretimeValue.ToString("yyyy/MM/dd HH:mm:ss"),
                        TagNo = tagnoValue,
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
