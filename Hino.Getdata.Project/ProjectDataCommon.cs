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
    
    public class ProjectDataCommon
    {
        private MySQLConnect connector;
        public string starttime;
        public string endtime;

        public ProjectDataCommon()
        {
            this.connector = new MySQLConnect()
            {
                ConnectionString = $"Server=192.168.2.11;Database=longduc;Uid=root;Pwd=101101;"
            };
        }
        public ProjectDataCommon(MySQLConnect connector)
        {
            this.connector = connector;
        }

        public List<ProjectCommonParameter> GetProjectDataCommon(string starttime, string endtime, string ProjectName)
        {
            try
            {
                if (starttime == null)
                {
                    starttime = "2022-06-23 00:00:00";
                    endtime = "2022-06-23 23:00:00";
                }
                var procName = $"proc_{ProjectName}_common";
                var dataTable = this.connector.ExecuteStoreProcedure(procName, new
                {
                    p_start_time = starttime,
                    p_end_time = endtime,
                    p_time_unit=2,
                });

                if (dataTable is null) return null;

                var list = new List<ProjectCommonParameter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var DateTime = row.Field<DateTime>("DateTime");
                    var TotalEnergyFromINV = row["TotalEnergyFromINV"].ToString();
                    var TotalEnergy = row["TotalEnergy"].ToString();
                    var SurplusPower = row["SurplusPower"].ToString();
                    var PurchasedPower = row["PurchasedPower"].ToString();
                    var VoltageA = row["VoltageA"].ToString();
                    var VoltageB = row["VoltageB"].ToString();
                    var VoltageC = row["VoltageC"].ToString();
                    var CurrentA = row["CurrentA"].ToString();
                    var CurrentB = row["CurrentB"].ToString();
                    var CurrentC = row["CurrentC"].ToString();
                    var Frequency = row["Frequency"].ToString();
                    var ActivePower = row["ActivePower"].ToString();
                    var ReactivePower = row["ReactivePower"].ToString();
                    var PowerFactor = row["PowerFactor"].ToString();
                    var AmbientAirTemperature = row["AmbientAirTemperature"].ToString();
                    var Irradiance = row["Irradiance"].ToString();
                    var BackOfModuleTemperature1 = row["BackOfModuleTemperature1"].ToString();
                    var BackOfModuleTemperature2 = row["BackOfModuleTemperature2"].ToString();
                    var SolarPeak = row["SolarPeak"].ToString();
                    var SolarOffPeak = row["SolarOffPeak"].ToString();
                    var SolarNormal = row["SolarNormal"].ToString();
                    var GridPeakIM = row["GridPeakIM"].ToString();
                    var GridOffPeakIM = row["GridOffPeakIM"].ToString();
                    var GridNormalIM = row["GridNormalIM"].ToString();
                    var GridPeakEX = row["GridPeakEX"].ToString();
                    var GridOffPeakEX = row["GridOffPeakEX"].ToString();
                    var GridNormalEX = row["GridNormalEX"].ToString();

                    float.TryParse(TotalEnergyFromINV, out float TotalEnergyFromINVValue);
                    float.TryParse(TotalEnergy, out float TotalEnergyValue);
                    float.TryParse(SurplusPower, out float SurplusPowerValue);
                    float.TryParse(PurchasedPower, out float PurchasedPowerValue);
                    float.TryParse(VoltageA, out float VoltageAValue);
                    float.TryParse(VoltageB, out float VoltageBValue);
                    float.TryParse(VoltageC, out float VoltageCValue);
                    float.TryParse(CurrentA, out float CurrentAValue);
                    float.TryParse(CurrentB, out float CurrentBValue);
                    float.TryParse(CurrentC, out float CurrentCValue);
                    float.TryParse(Frequency, out float FrequencyValue);
                    float.TryParse(ActivePower, out float ActivePowerValue);
                    float.TryParse(ReactivePower, out float ReactivePowerAValue);
                    float.TryParse(PowerFactor, out float PowerFactorValue);
                    float.TryParse(AmbientAirTemperature, out float AmbientAirTemperatureValue);
                    float.TryParse(Irradiance, out float IrradianceAValue);
                    float.TryParse(BackOfModuleTemperature1, out float BackOfModuleTemperature1Value);
                    float.TryParse(BackOfModuleTemperature2, out float BackOfModuleTemperature2Value);
                    float.TryParse(SolarPeak, out float SolarPeakValue);
                    float.TryParse(SolarOffPeak, out float SolarOffPeakValue);
                    float.TryParse(SolarNormal, out float SolarNormalValue);
                    float.TryParse(GridPeakIM, out float GridPeakIMValue);
                    float.TryParse(GridOffPeakIM, out float GridOffPeakIMValue);
                    float.TryParse(GridNormalIM, out float GridNormalIMValue);
                    float.TryParse(GridPeakEX, out float GridPeakEXValue);
                    float.TryParse(GridOffPeakEX, out float GridOffPeakEXValue);
                    float.TryParse(GridNormalEX, out float GridNormalEXValue);


                    list.Add(new ProjectCommonParameter()
                        {
                            DateTime = DateTime,
                            TotalEnergyFromINV = TotalEnergyFromINVValue,
                            TotalEnergy = TotalEnergyValue,
                            SurplusPower = SurplusPowerValue,
                            PurchasedPower = PurchasedPowerValue,
                            VoltageA = VoltageAValue,
                            VoltageB = VoltageBValue,
                            VoltageC = VoltageCValue,
                            CurrentA = CurrentAValue,
                            CurrentB = CurrentBValue,
                            CurrentC = CurrentCValue,
                            Frequency = FrequencyValue,
                            ActivePower = ActivePowerValue,
                            ReactivePower = ReactivePowerAValue,
                            PowerFactor = PowerFactorValue,
                            AmbientAirTemperature = AmbientAirTemperatureValue,
                            Irradiance = IrradianceAValue,
                            BackOfModuleTemperature1 = BackOfModuleTemperature1Value,
                            BackOfModuleTemperature2 = BackOfModuleTemperature2Value,
                            SolarPeak = SolarPeakValue,
                            SolarOffPeak = SolarOffPeakValue,
                            SolarNormal = SolarNormalValue,
                            GridPeakIM = GridPeakIMValue,
                            GridOffPeakIM = GridOffPeakIMValue,
                            GridNormalIM = GridNormalIMValue,
                            GridPeakEX = GridPeakEXValue,
                            GridOffPeakEX = GridOffPeakEXValue,
                            GridNormalEX = GridNormalEXValue,

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
