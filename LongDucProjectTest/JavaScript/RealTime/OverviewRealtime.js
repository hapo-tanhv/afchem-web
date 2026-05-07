var activepower;
document.addEventListener("DOMContentLoaded", function () {
    var atscadaTask = document.querySelector('atscada-task');
    var dataTask = atscadaTask.dataTask;
    var dataCollection = dataTask.dataCollection;
    AcivePowerChart();

    //Add tag solar information
    
    dataCollection.add(`ITNCommonSolar.ActivePower`);
    dataCollection.add(`ITNCommonSolar.DailyEnergy`);
    dataCollection.add(`ITNCommonSolar.MonthlyEnergy`);
    dataCollection.add(`ITNCommonSolar.YearlyEnergy`);
    dataCollection.add(`ITNCommonSolar.TotalEnergy`);
    dataCollection.add(`ITNCommonSolar.ReactivePower`);
    dataCollection.add(`ITNCommonSolar.CO2Reduction`);
    dataCollection.add(`ITNCommonSolar.Performance`);
   // ITNCommon.Solar.ReactivePower


    //Add tag weather information
    dataCollection.add(`WetherStationRF1.AmbientTemp`);
    dataCollection.add(`WetherStationRF1.Irradiation`);

    //Add tag plant information.
    dataCollection.add(`ITNCommonSolar.Tree`);
    dataCollection.add(`ITNCommonSolar.TotalProjectActive`);
    dataCollection.add(`ITNCommonSolar.TotalProjectInActive`);
    dataCollection.add(`ITNCommonSolar.TotalInverterON`);
    dataCollection.add(`ITNCommonSolar.TotalInverterOFF`);
    dataCollection.add(`ITNCommonSolar.TotalInverterFault`);
    dataCollection.add(`ITNCommonSolar.TotalInverterOther`);
    dataCollection.add(`ITNCommonSolar.TotalInverterStandby`);

    
    //Add ACB status
    dataCollection.add(`Project1SolarPanelMainCB.ON`);
    dataCollection.add(`Project1SolarPanelMainCB.OFF`);
    dataCollection.add(`Project1SolarPanelMainCB.Trip`);
    dataCollection.add(`Project1SolarPanelMainCB.Emergency`);

    dataCollection.add(`Project2SolarPanelMainCB.ON`);
    dataCollection.add(`Project2SolarPanelMainCB.OFF`);
    dataCollection.add(`Project2SolarPanelMainCB.Trip`);
    dataCollection.add(`Project2SolarPanelMainCB.Emergency`);
   
    dataCollection.add(`Project3SolarPanelMainCB1.ON`);
    dataCollection.add(`Project3SolarPanelMainCB1.OFF`);
    dataCollection.add(`Project3SolarPanelMainCB1.Trip`);
    dataCollection.add(`Project3SolarPanelMainCB1.Emergency`);
    dataCollection.add(`Project3SolarPanelMainCB2.ON`);
    dataCollection.add(`Project3SolarPanelMainCB2.OFF`);
    dataCollection.add(`Project3SolarPanelMainCB2.Trip`);
    dataCollection.add(`Project3SolarPanelMainCB2.Emergency`);

    dataCollection.add(`Project4SolarPanelMainCB.ON`);
    dataCollection.add(`Project4SolarPanelMainCB.OFF`);
    dataCollection.add(`Project4SolarPanelMainCB.Trip`);
    dataCollection.add(`Project4SolarPanelMainCB.Emergency`);

    dataCollection.add(`Project5SolarPanelMainCB.ON`);
    dataCollection.add(`Project5SolarPanelMainCB.OFF`);
    dataCollection.add(`Project5SolarPanelMainCB.Trip`);
    dataCollection.add(`Project5SolarPanelMainCB.Emergency`);

    dataCollection.add(`Project6SolarPanelMainCB1.ON`);
    dataCollection.add(`Project6SolarPanelMainCB1.OFF`);
    dataCollection.add(`Project6SolarPanelMainCB1.Trip`);
    dataCollection.add(`Project6SolarPanelMainCB1.Emergency`);
    dataCollection.add(`Project6SolarPanelMainCB2.ON`);
    dataCollection.add(`Project6SolarPanelMainCB2.OFF`);
    dataCollection.add(`Project6SolarPanelMainCB2.Trip`);
    dataCollection.add(`Project6SolarPanelMainCB2.Emergency`);

    dataCollection.add(`Project7SolarPanelMainCB.ON`);
    dataCollection.add(`Project7SolarPanelMainCB.OFF`);
    dataCollection.add(`Project7SolarPanelMainCB.Trip`);
    dataCollection.add(`Project7SolarPanelMainCB.Emergency`);

    updateTag1(
        dataCollection.get(`ITNCommonSolar.ActivePower`),
        document.querySelector('#container-speed'));
    updateTag2(
        dataCollection.get(`ITNCommonSolar.DailyEnergy`),
        document.querySelector('#DailyEnergy'));
    updateTag3(
        dataCollection.get(`ITNCommonSolar.MonthlyEnergy`),
        document.querySelector('#MonthlyEnergy'));
    updateTag4(
        dataCollection.get(`ITNCommonSolar.YearlyEnergy`),
        document.querySelector('#YearlyEnergy'));
    updateTag5(
        dataCollection.get(`ITNCommonSolar.TotalEnergy`),
        document.querySelector('#TotalEnergy'));
    updateTag6(
        dataCollection.get(`ITNCommonSolar.ReactivePower`),
        document.querySelector('#ReactivePower'));
    updateTag7(
        dataCollection.get(`ITNCommonSolar.CO2Reduction`),
        document.querySelector('#CO2Reduction'));
    updateTag8(
        dataCollection.get(`ITNCommonSolar.Performance`),
        document.querySelector('#Performance'));

    updateTag9(
        dataCollection.get(`WetherStationRF1.AmbientTemp`),
        document.querySelector('#AmbientTemp'));
    updateTag10(
        dataCollection.get(`WetherStationRF1.Irradiation`),
        document.querySelector('#Irradiation'));

    updateTag13(
        dataCollection.get(`ITNCommonSolar.TotalProjectActive`),
        document.querySelector('#TotalProjectActive'));
    updateTag14(
        dataCollection.get(`ITNCommonSolar.TotalProjectInActive`),
        document.querySelector('#TotalProjectInActive'));
    updateTag15(
        dataCollection.get(`ITNCommonSolar.TotalInverterON`),
        document.querySelector('#TotalInverterON'));
    updateTag16(
        dataCollection.get(`ITNCommonSolar.TotalInverterOFF`),
        document.querySelector('#TotalInverterOFF'));
    updateTag17(
        dataCollection.get(`ITNCommonSolar.TotalInverterFault`),
        document.querySelector('#TotalInverterFault'));
    updateTag18(
        dataCollection.get(`ITNCommonSolar.TotalInverterOther`),
        document.querySelector('#TotalInverterOther'));
    updateTag19(
        dataCollection.get(`ITNCommonSolar.Tree`),
        document.querySelector('#tree'));
    updateTag20(
        dataCollection.get(`ITNCommonSolar.TotalInverterStandby`),
        document.querySelector('#TotalInverterStandby'));

    updateTag21(
        dataCollection.get(`Project1SolarPanelMainCB.ON`),
        document.querySelector('#Project1ACB'));
    updateTag22(
        dataCollection.get(`Project1SolarPanelMainCB.OFF`),
        document.querySelector('#Project1ACB'));
    updateTag23(
        dataCollection.get(`Project1SolarPanelMainCB.Trip`),
        document.querySelector('#Project1ACB'));
    updateTag24(
        dataCollection.get(`Project1SolarPanelMainCB.Emergency`),
        document.querySelector('#Project1ACB'));

    updateTag25(
        dataCollection.get(`Project2SolarPanelMainCB.ON`),
        document.querySelector('#Project2ACB'));
    updateTag26(
        dataCollection.get(`Project2SolarPanelMainCB.OFF`),
        document.querySelector('#Project2ACB'));
    updateTag27(
        dataCollection.get(`Project2SolarPanelMainCB.Trip`),
        document.querySelector('#Project2ACB'));
    updateTag28(
        dataCollection.get(`Project2SolarPanelMainCB.Emergency`),
        document.querySelector('#Project2ACB'));

    updateTag29(
        dataCollection.get(`Project3SolarPanelMainCB1.ON`),
        document.querySelector('#Project3ACB1'));
    updateTag30(
        dataCollection.get(`Project3SolarPanelMainCB1.OFF`),
        document.querySelector('#Project3ACB1'));
    updateTag31(
        dataCollection.get(`Project3SolarPanelMainCB1.Trip`),
        document.querySelector('#Project3ACB1'));
    updateTag32(
        dataCollection.get(`Project3SolarPanelMainCB1.Emergency`),
        document.querySelector('#Project3ACB1'));

    updateTag33(
        dataCollection.get(`Project3SolarPanelMainCB2.ON`),
        document.querySelector('#Project3ACB2'));
    updateTag34(
        dataCollection.get(`Project3SolarPanelMainCB2.OFF`),
        document.querySelector('#Project3ACB2'));
    updateTag35(
        dataCollection.get(`Project3SolarPanelMainCB2.Trip`),
        document.querySelector('#Project3ACB2'));
    updateTag36(
        dataCollection.get(`Project3SolarPanelMainCB2.Emergency`),
        document.querySelector('#Project3ACB2'));

    updateTag37(
        dataCollection.get(`Project4SolarPanelMainCB.ON`),
        document.querySelector('#Project4ACB'));
    updateTag38(
        dataCollection.get(`Project4SolarPanelMainCB.OFF`),
        document.querySelector('#Project4ACB'));
    updateTag39(
        dataCollection.get(`Project4SolarPanelMainCB.Trip`),
        document.querySelector('#Project4ACB'));
    updateTag40(
        dataCollection.get(`Project4SolarPanelMainCB.Emergency`),
        document.querySelector('#Project4ACB'));

    updateTag41(
        dataCollection.get(`Project5SolarPanelMainCB.ON`),
        document.querySelector('#Project5ACB'));
    updateTag42(
        dataCollection.get(`Project5SolarPanelMainCB.OFF`),
        document.querySelector('#Project5ACB'));
    updateTag43(
        dataCollection.get(`Project5SolarPanelMainCB.Trip`),
        document.querySelector('#Project5ACB'));
    updateTag44(
        dataCollection.get(`Project5SolarPanelMainCB.Emergency`),
        document.querySelector('#Project5ACB'));

    updateTag45(
        dataCollection.get(`Project6SolarPanelMainCB1.ON`),
        document.querySelector('#Project6ACB1'));
    updateTag46(
        dataCollection.get(`Project6SolarPanelMainCB1.OFF`),
        document.querySelector('#Project6ACB1'));
    updateTag47(
        dataCollection.get(`Project6SolarPanelMainCB1.Trip`),
        document.querySelector('#Project6ACB1'));
    updateTag48(
        dataCollection.get(`Project6SolarPanelMainCB1.Emergency`),
        document.querySelector('#Project6ACB1'));


    updateTag49(
        dataCollection.get(`Project6SolarPanelMainCB2.ON`),
        document.querySelector('#Project6ACB2'));
    updateTag50(
        dataCollection.get(`Project6SolarPanelMainCB2.OFF`),
        document.querySelector('#Project6ACB2'));
    updateTag51(
        dataCollection.get(`Project6SolarPanelMainCB2.Trip`),
        document.querySelector('#Project6ACB2'));
    updateTag52(
        dataCollection.get(`Project6SolarPanelMainCB2.Emergency`),
        document.querySelector('#Project6ACB2'));


    updateTag53(
        dataCollection.get(`Project7SolarPanelMainCB.ON`),
        document.querySelector('#Project7ACB'));
    updateTag54(
        dataCollection.get(`Project7SolarPanelMainCB.OFF`),
        document.querySelector('#Project7ACB'));
    updateTag55(
        dataCollection.get(`Project7SolarPanelMainCB.Trip`),
        document.querySelector('#Project7ACB'));
    updateTag56(
        dataCollection.get(`Project7SolarPanelMainCB.Emergency`),
        document.querySelector('#Project7ACB'));
    dataTask.start();
});

function updateTag1(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {                
                element.innerHTML = data.e.newValue;
                activepower = Number(data.e.newValue);
                AcivePowerChart();
            }
        });
        if (dataTag.Value !== undefined) {            
            element.innerHTML = data.e.newValue;
            activepower = data.e.newValue;
        }
    }
}

function updateTag2(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag3(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag4(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag5(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag6(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag7(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue/1000);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag8(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag9(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag10(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag11(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag12(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}
function updateTag13(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag14(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag15(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag16(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag17(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag18(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag19(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag20(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag21(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }                
                
            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag22(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag23(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag24(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}

function updateTag25(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag26(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag27(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag28(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}

function updateTag29(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag30(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag31(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag32(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}


function updateTag33(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag34(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag35(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag36(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}

function updateTag37(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag38(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag39(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag40(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}

function updateTag41(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag42(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag43(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag44(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}


function updateTag45(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag46(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag47(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag48(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}


function updateTag49(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag50(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag51(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag52(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "EMER.SHUTDOWN";
                    element.style.color = "purple";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "EMER.SHUTDOWN";
                element.style.color = "purple";
            }
        }
    }
}

function updateTag53(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "CLOSE";
                    element.style.color = "red";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "CLOSE";
                element.style.color = "red";
            }
        }
    }
}

function updateTag54(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "OPEN";
                    element.style.color = "lime";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "OPEN";
                element.style.color = "lime";
            }
        }
    }
}

function updateTag55(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}

function updateTag56(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                if (data.e.newValue == "1") {
                    element.innerHTML = "TRIP";
                    element.style.color = "orange";
                }

            }
        });
        if (dataTag.Value !== undefined) {
            if (data.e.newValue == "1") {
                element.innerHTML = "TRIP";
                element.style.color = "orange";
            }
        }
    }
}



var gaugeOptions = {
    chart: {
        type: 'solidgauge'
    },

    title: null,

    pane: {
        //startAngle: -180,
        //endAngle: 180,
        background: {
            backgroundColor:
                Highcharts.defaultOptions.legend.backgroundColor || '#EEE',
            innerRadius: '60%',
            outerRadius: '100%',
        }
    },

    exporting: {
        enabled: false
    },

    tooltip: {
        enabled: false
    },

    // the value axis
    yAxis: {
        stops: [
            [0.1, '#2d8cf0']
        ],
        lineWidth: 0,
        tickWidth: 0,
        minorTickInterval: null,
        tickAmount: 2,
        title: {
            y: -70,

        },
        labels: {
            y: 16
        }
    },

    plotOptions: {
        solidgauge: {
            dataLabels: {
                y: -20,
                color: '#2d8cf0',
                borderWidth: 0,
                useHTML: true
            }
        }
    }
};



function AcivePowerChart() {
    var chartSpeed = Highcharts.chart('container-speed', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 5170,
            visible: false,
        },

        credits: {
            enabled: false
        },

        series: [{
            animation: false,
            name: 'Speed',
             data: [activepower],
            dataLabels: {
                format:
                    '<div style="text-align:center">' +
                    '<span style="font-size:25px">{y}</span><br/>' +
                    '<span style="font-size:12px">kW</span>' +
                    '</div>'
            },
            tooltip: {
                valueSuffix: ' kW'
            }
        }]

    }));
    
}