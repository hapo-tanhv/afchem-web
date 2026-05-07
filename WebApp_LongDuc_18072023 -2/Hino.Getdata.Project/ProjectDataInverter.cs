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
   public class ProjectDataInverter
    {
        private MySQLConnect connector;
        public string starttime;
        public string endtime;
        public ProjectDataInverter()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=localhost;Database=longduc;Uid=root;Pwd=101101;"
            };
        }
        public ProjectDataInverter(MySQLConnect connector)
        {
            this.connector = connector;
        }

        //Get data for Energy Inverter (for Inverter Page)
        public List<InverterEnergyParameter> GetInverterSolarEnergy(string ProjectName, string InverterName, int TimeUnit, string starttime, string endtime)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_{InverterName}_energy";
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit = 3,
                    selectProc = TimeUnit

                });

                if (dataTable is null) return null;

                var list = new List<InverterEnergyParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var startTime = row.Field<DateTime>("StartTime");
                    var solar = row["Solar"].ToString();

                    if (float.TryParse(solar, out float solarvalue))
                    {
                        list.Add(new InverterEnergyParameter()
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

        public List<InverterPowerParameter> GetInverterSolarPower(string datetime, string ProjectName, string InverterName)
        {
            try
            {
                if (datetime == null)
                {
                    datetime = "2022-06-23 00:00:00";
                }
                var procName = $"proc_{ProjectName}_{InverterName}_power";

                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_DateTime = datetime
                });

                if (dataTable is null) return null;

                var list = new List<InverterPowerParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var time = row.Field<DateTime>("DateTime");
                    var solarvalue = row["Solar"].ToString();

                    if (float.TryParse(solarvalue, out float SolarValue))
                    {
                        list.Add(new InverterPowerParameter()
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

        public List<InverterElectricalParameter> GetInverterSolarElectrical(string starttime, string endtime, string ProjectName, string InverterName)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_{InverterName}_electrical";

                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                });

                if (dataTable is null) return null;

                var list = new List<InverterElectricalParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var DateTimeValue = row.Field<DateTime>("DateTime");
                    var DailyEnergyValue = row["DailyEnergy"].ToString();
                    var TotalEnergyValue = row["TotalEnergy"].ToString();
                    var GridVoltagePhaseAValue = row["GridVoltagePhaseA"].ToString();
                    var GridVoltagePhaseBValue = row["GridVoltagePhaseB"].ToString();
                    var GridVoltagePhaseCValue = row["GridVoltagePhaseC"].ToString();
                    var GridCurrentPhaseAValue = row["GridCurrentPhaseA"].ToString();
                    var GridCurrentPhaseBValue = row["GridCurrentPhaseB"].ToString();
                    var GridCurrentPhaseCValue = row["GridCurrentPhaseC"].ToString();

                    var OutputActivePowerValue = row["OutputActivePower"].ToString();
                    var OutputReactivePowerValue = row["OutputReactivePower"].ToString();
                    var PowerFactorValue = row["PowerFactor"].ToString();
                    var GridFrequencyValue = row["GridFrequency"].ToString();
                    var TemperatureValue = row["Temperature"].ToString();

                    var MPPT1VoltageValue = row["MPPT1Voltage"].ToString();
                    var MPPT2VoltageValue = row["MPPT2Voltage"].ToString();
                    var MPPT3VoltageValue = row["MPPT3Voltage"].ToString();
                    var MPPT4VoltageValue = row["MPPT4Voltage"].ToString();
                    var MPPT5VoltageValue = row["MPPT5Voltage"].ToString();
                    var MPPT6VoltageValue = row["MPPT6Voltage"].ToString();
                    var MPPT7VoltageValue = row["MPPT7Voltage"].ToString();
                    var MPPT8VoltageValue = row["MPPT8Voltage"].ToString();
                    var MPPT9VoltageValue = row["MPPT9Voltage"].ToString();
                    var MPPT10VoltageValue = row["MPPT10Voltage"].ToString();
                    var MPPT11VoltageValue = row["MPPT11Voltage"].ToString();
                    var MPPT12VoltageValue = row["MPPT12Voltage"].ToString();

                    var MPPT1CurrentValue = row["MPPT1Current"].ToString();
                    var MPPT2CurrentValue = row["MPPT2Current"].ToString();
                    var MPPT3CurrentValue = row["MPPT3Current"].ToString();
                    var MPPT4CurrentValue = row["MPPT4Current"].ToString();
                    var MPPT5CurrentValue = row["MPPT5Current"].ToString();
                    var MPPT6CurrentValue = row["MPPT6Current"].ToString();
                    var MPPT7CurrentValue = row["MPPT7Current"].ToString();
                    var MPPT8CurrentValue = row["MPPT8Current"].ToString();
                    var MPPT9CurrentValue = row["MPPT9Current"].ToString();
                    var MPPT10CurrentValue = row["MPPT10Current"].ToString();
                    var MPPT11CurrentValue = row["MPPT11Current"].ToString();
                    var MPPT12CurrentValue = row["MPPT12Current"].ToString();

                    //convert dữ liệu từ string sang float;
                    float.TryParse(DailyEnergyValue, out float dailyEnergyValue);
                    float.TryParse(TotalEnergyValue, out float totalEnergyValue);
                    float.TryParse(GridVoltagePhaseAValue, out float gridVoltagePhaseAValue);
                    float.TryParse(GridVoltagePhaseBValue, out float gridVoltagePhaseBValue);
                    float.TryParse(GridVoltagePhaseCValue, out float gridVoltagePhaseCValue);
                    float.TryParse(GridCurrentPhaseAValue, out float gridCurrentPhaseAValue);
                    float.TryParse(GridCurrentPhaseBValue, out float gridCurrentPhaseBValue);
                    float.TryParse(GridCurrentPhaseCValue, out float gridCurrentPhaseCValue);

                    float.TryParse(OutputActivePowerValue, out float outputActivePowerValue);
                    float.TryParse(OutputReactivePowerValue, out float outputReactivePowerValue);
                    float.TryParse(PowerFactorValue, out float powerFactorValue);
                    float.TryParse(GridFrequencyValue, out float gridFrequencyValue);
                    float.TryParse(TemperatureValue, out float temperatureValue);

                    float.TryParse(MPPT1VoltageValue, out float mppt1VoltageValue);
                    float.TryParse(MPPT2VoltageValue, out float mppt2VoltageValue);
                    float.TryParse(MPPT3VoltageValue, out float mppt3VoltageValue);
                    float.TryParse(MPPT4VoltageValue, out float mppt4VoltageValue);
                    float.TryParse(MPPT5VoltageValue, out float mppt5VoltageValue);
                    float.TryParse(MPPT6VoltageValue, out float mppt6VoltageValue);
                    float.TryParse(MPPT7VoltageValue, out float mppt7VoltageValue);
                    float.TryParse(MPPT8VoltageValue, out float mppt8VoltageValue);
                    float.TryParse(MPPT9VoltageValue, out float mppt9VoltageValue);
                    float.TryParse(MPPT10VoltageValue, out float mppt10VoltageValue);
                    float.TryParse(MPPT11VoltageValue, out float mppt11VoltageValue);
                    float.TryParse(MPPT12VoltageValue, out float mppt12VoltageValue);

                    float.TryParse(MPPT1CurrentValue, out float mppt1CurrentValue);
                    float.TryParse(MPPT2CurrentValue, out float mppt2CurrentValue);
                    float.TryParse(MPPT3CurrentValue, out float mppt3CurrentValue);
                    float.TryParse(MPPT4CurrentValue, out float mppt4CurrentValue);
                    float.TryParse(MPPT5CurrentValue, out float mppt5CurrentValue);
                    float.TryParse(MPPT6CurrentValue, out float mppt6CurrentValue);
                    float.TryParse(MPPT7CurrentValue, out float mppt7CurrentValue);
                    float.TryParse(MPPT8CurrentValue, out float mppt8CurrentValue);
                    float.TryParse(MPPT9CurrentValue, out float mppt9CurrentValue);
                    float.TryParse(MPPT10CurrentValue, out float mppt10CurrentValue);
                    float.TryParse(MPPT11CurrentValue, out float mppt11CurrentValue);
                    float.TryParse(MPPT12CurrentValue, out float mppt12CurrentValue);

                    list.Add(new InverterElectricalParameter()
                        {
                            DateTime = DateTimeValue,
                            DailyEnergy = dailyEnergyValue,
                            TotalEnergy= totalEnergyValue,
                            GridVoltagePhaseA = gridVoltagePhaseAValue,
                            GridVoltagePhaseB = gridVoltagePhaseBValue,
                            GridVoltagePhaseC = gridVoltagePhaseCValue,
                            GridCurrentPhaseA = gridCurrentPhaseAValue,
                            GridCurrentPhaseB = gridCurrentPhaseBValue,
                            GridCurrentPhaseC = gridCurrentPhaseCValue,
                            OutputActivePower = outputActivePowerValue,
                            OutputReactivePower = outputReactivePowerValue,
                            PowerFactor = powerFactorValue,
                            GridFrequency = gridFrequencyValue,
                            Temperature = temperatureValue,
                            MPPT1Voltage = mppt1VoltageValue,
                            MPPT2Voltage = mppt2VoltageValue,
                            MPPT3Voltage = mppt3VoltageValue,
                            MPPT4Voltage = mppt4VoltageValue,
                            MPPT5Voltage = mppt5VoltageValue,
                            MPPT6Voltage = mppt6VoltageValue,
                            MPPT7Voltage = mppt7VoltageValue,
                            MPPT8Voltage = mppt8VoltageValue,
                            MPPT9Voltage = mppt9VoltageValue,
                            MPPT10Voltage = mppt10VoltageValue,
                            MPPT11Voltage = mppt11VoltageValue,
                            MPPT12Voltage = mppt12VoltageValue,
                            MPPT1Current = mppt1CurrentValue,
                            MPPT2Current = mppt2CurrentValue,
                            MPPT3Current = mppt3CurrentValue,
                            MPPT4Current = mppt4CurrentValue,
                            MPPT5Current = mppt5CurrentValue,
                            MPPT6Current = mppt6CurrentValue,
                            MPPT7Current = mppt7CurrentValue,
                            MPPT8Current = mppt8CurrentValue,
                            MPPT9Current = mppt9CurrentValue,
                            MPPT10Current = mppt10CurrentValue,
                            MPPT11Current = mppt11CurrentValue,
                            MPPT12Current = mppt12CurrentValue,

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
