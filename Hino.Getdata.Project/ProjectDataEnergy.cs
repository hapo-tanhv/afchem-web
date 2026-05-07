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
                ConnectionString = $"Server=192.168.2.11;Database=longduc;Uid=root;Pwd=101101;"
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
                    var grid = row["Grid"].ToString();
                    if (float.TryParse(solar, out float solarValue) && float.TryParse(grid, out float gridValue))
                    {
                        list.Add(new ProjectEnergyParameter()
                        {
                            DateTime = startTime,
                            Value = solarValue,
                            GridValue = gridValue
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

        // Get data for Electrical Export (for Project page)
        public List<ProjectElectricalParameter> GetProjectSolarElectrical(string datetime, string ProjectName)
        {
            try
            {
                if (datetime == null)
                {
                    datetime = "2022-06-23 00:00:00";
                }
                var procName = $"proc_{ProjectName}_electrical";

                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_DateTime = datetime
                });

                if (dataTable is null) return null;

                var list = new List<ProjectElectricalParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var DateTimeValue = row.Field<DateTime>("DateTime");
                    var ActivePowerValue = row["ActivePower"].ToString();
                    var ReactivePowerValue = row["ReactivePower"].ToString();
                    var PowerFactorValue = row["PowerFactor"].ToString();
                    var FrequencyValue = row["Frequency"].ToString();
                    var VoltageAValue = row["VoltageA"].ToString();
                    var VoltageBValue = row["VoltageB"].ToString();
                    var VoltageCValue = row["VoltageC"].ToString();
                    var CurrentAValue = row["CurrentA"].ToString();
                    var CurrentBValue = row["CurrentB"].ToString();
                    var CurrentCValue = row["CurrentC"].ToString();

                    //convert dữ liệu từ string sang float;
                    float.TryParse(ActivePowerValue, out float activePowerValue);
                    float.TryParse(ReactivePowerValue, out float reactivePowerValue);
                    float.TryParse(PowerFactorValue, out float powerFactorValue);
                    float.TryParse(FrequencyValue, out float frequencyValue);
                    float.TryParse(VoltageAValue, out float voltageAValue);
                    float.TryParse(VoltageBValue, out float voltageBValue);
                    float.TryParse(VoltageCValue, out float voltageCValue);
                    float.TryParse(CurrentAValue, out float currentAValue);
                    float.TryParse(CurrentBValue, out float currentBValue);
                    float.TryParse(CurrentCValue, out float currentCValue);



                    list.Add(new ProjectElectricalParameter()
                    {
                        DateTime = DateTimeValue,
                        ActivePower = activePowerValue,
                        ReactivePower = reactivePowerValue,
                        PowerFactor = powerFactorValue,
                        Frequency = frequencyValue,
                        VoltageA = voltageAValue,
                        VoltageB = voltageBValue,
                        VoltageC = voltageCValue,
                        CurrentA = currentAValue,
                        CurrentB = currentBValue,
                        CurrentC = currentCValue,


                    });
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        //Get data for Project Signage (for each Signage page)
        public List<ProjectSignageParameter> GetProjectSignageData(string ProjectName, int TimeUnit, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_power_generation";
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit = 3,

                });

                if (dataTable is null) return null;

                var list = new List<ProjectSignageParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var DateTime = row.Field<DateTime>("StartTime");
                    var SolarPower = row["Solar"].ToString();
                    var SelfConsumption = row["SelfConsumption"].ToString();
                    var PurchasedPower = row["Purchased"].ToString();

                    if ((float.TryParse(SolarPower, out float SolarPowerValue)) && (float.TryParse(SelfConsumption, out float SelfConsumptionValue)) &&
                        (float.TryParse(PurchasedPower, out float PurchasedPowerValue)))
                    {
                        list.Add(new ProjectSignageParameter()
                        {
                            DateTime = DateTime,
                            SolarPower = SolarPowerValue,
                            SelfConsumption = SelfConsumptionValue,
                            PurchasedPower = PurchasedPowerValue

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


        //Get data for Project Daily Self Consumption (for each Signage page)
        public List<ProjectSignageParameter> GetProjectSelfConsumption(string ProjectName, int selectproc, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_selfconsumption";
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit = 3,
                    selectProc = selectproc,

                });

                if (dataTable is null) return null;

                var list = new List<ProjectSignageParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var DateTime = row.Field<DateTime>("StartTime");
                    var SelfConsumption = row["Solar"].ToString();

                    if (float.TryParse(SelfConsumption, out float SelfConsumptionValue))
                    {
                        list.Add(new ProjectSignageParameter()
                        {
                            DateTime = DateTime,
                            SelfConsumption = SelfConsumptionValue,
                        });
                    }
                }
                return list;
            }
            catch(Exception ẽ)
            {
                return null;
            }
        }
    }
}
