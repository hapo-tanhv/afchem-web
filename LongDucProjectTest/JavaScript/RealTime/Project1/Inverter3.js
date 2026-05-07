document.addEventListener("DOMContentLoaded", function () {
    var atscadaTask = document.querySelector('atscada-task');
    var dataTask = atscadaTask.dataTask;
    var dataCollection = dataTask.dataCollection;

    //Khai báo, add thêm tag
    //Inverter information
    dataCollection.add(`ITNProject1Energy.DailyEnergyINV3`);
    dataCollection.add(`Project1Inverter3.TotalEnergy`);
    dataCollection.add(`Project1Inverter3.GridVoltage`);
    dataCollection.add(`Project1Inverter3.GridCurrent`);
    dataCollection.add(`Project1Inverter3.OutputActivePower`);
    dataCollection.add(`Project1Inverter3.OutputReactivePower`);
    dataCollection.add(`Project1Inverter3.PowerFactor`);
    dataCollection.add(`Project1Inverter3.GridFrequency`);
    dataCollection.add(`Project1Inverter3.InverterTemperature`);

    //Inverter PV Details Voltage
    dataCollection.add(`Project1Inverter3.MPPT1Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT2Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT3Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT4Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT5Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT6Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT7Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT8Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT9Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT10Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT11Voltage`);
    dataCollection.add(`Project1Inverter3.MPPT12Voltage`);

    //Inverter PV Details Current
    dataCollection.add(`Project1Inverter3.MPPT1Current`);
    dataCollection.add(`Project1Inverter3.MPPT2Current`);
    dataCollection.add(`Project1Inverter3.MPPT3Current`);
    dataCollection.add(`Project1Inverter3.MPPT4Current`);
    dataCollection.add(`Project1Inverter3.MPPT5Current`);
    dataCollection.add(`Project1Inverter3.MPPT6Current`);
    dataCollection.add(`Project1Inverter3.MPPT7Current`);
    dataCollection.add(`Project1Inverter3.MPPT8Current`);
    dataCollection.add(`Project1Inverter3.MPPT9Current`);
    dataCollection.add(`Project1Inverter3.MPPT10Current`);
    dataCollection.add(`Project1Inverter3.MPPT11Current`);
    dataCollection.add(`Project1Inverter3.MPPT12Current`);

    //Inverter information update
    dataCollection.add(`Project1Inverter3.GridVoltageA`);
    dataCollection.add(`Project1Inverter3.GridVoltageB`);
    dataCollection.add(`Project1Inverter3.GridVoltageC`);
    dataCollection.add(`Project1Inverter3.GridVoltageAB`);
    dataCollection.add(`Project1Inverter3.GridVoltageBC`);
    dataCollection.add(`Project1Inverter3.GridVoltageCA`);
    dataCollection.add(`Project1Inverter3.GridCurrentA`);
    dataCollection.add(`Project1Inverter3.GridCurrentB`);
    dataCollection.add(`Project1Inverter3.GridCurrentC`);
    dataCollection.add(`Project1Inverter3.DCPower`);

    //Inverter update tag
    //Inverter Information
    updateTag1(
        dataCollection.get(`Project1Inverter3.TotalEnergy`),
        document.querySelector('#TotalEnergy'));
    updateTag2(
        dataCollection.get(`Project1Inverter3.GridVoltage`),
        document.querySelector('#GridVoltage'));
    updateTag3(
        dataCollection.get(`Project1Inverter3.GridCurrent`),
        document.querySelector('#GridCurrent'));
    updateTag4(
        dataCollection.get(`Project1Inverter3.OutputActivePower`),
        document.querySelector('#OutputActivePower'));
    updateTag5(
        dataCollection.get(`Project1Inverter3.OutputReactivePower`),
        document.querySelector('#OutputReactivePower'));
    updateTag6(
        dataCollection.get(`Project1Inverter3.PowerFactor`),
        document.querySelector('#PowerFactor'));
    updateTag7(
        dataCollection.get(`Project1Inverter3.GridFrequency`),
        document.querySelector('#GridFrequency'));
    updateTag8(
        dataCollection.get(`Project1Inverter3.InverterTemperature`),
        document.querySelector('#InverterTemperature'));
    updateTag37(
        dataCollection.get(`ITNProject1Energy.DailyEnergyINV3`),
        document.querySelector('#DailyEnergy'));

    //Inverter PV Details Voltage
    updateTag9(
        dataCollection.get(`Project1Inverter3.MPPT1Voltage`),
        document.querySelector('#MPPT1Voltage'));
    updateTag10(
        dataCollection.get(`Project1Inverter3.MPPT2Voltage`),
        document.querySelector('#MPPT2Voltage'));
    updateTag11(
        dataCollection.get(`Project1Inverter3.MPPT3Voltage`),
        document.querySelector('#MPPT3Voltage'));
    updateTag12(
        dataCollection.get(`Project1Inverter3.MPPT4Voltage`),
        document.querySelector('#MPPT4Voltage'));
    updateTag13(
        dataCollection.get(`Project1Inverter3.MPPT5Voltage`),
        document.querySelector('#MPPT5Voltage'));
    updateTag14(
        dataCollection.get(`Project1Inverter3.MPPT6Voltage`),
        document.querySelector('#MPPT6Voltage'));
    updateTag15(
        dataCollection.get(`Project1Inverter3.MPPT7Voltage`),
        document.querySelector('#MPPT7Voltage'));
    updateTag16(
        dataCollection.get(`Project1Inverter3.MPPT8Voltage`),
        document.querySelector('#MPPT8Voltage'));
    updateTag17(
        dataCollection.get(`Project1Inverter3.MPPT9Voltage`),
        document.querySelector('#MPPT9Voltage'));
    // Update thêm 3 string từ 10-12 
    updateTag38(
        dataCollection.get(`Project1Inverter3.MPPT10Voltage`),
        document.querySelector('#MPPT10Voltage'));
    updateTag39(
        dataCollection.get(`Project1Inverter3.MPPT11Voltage`),
        document.querySelector('#MPPT11Voltage'));
    updateTag40(
        dataCollection.get(`Project1Inverter3.MPPT12Voltage`),
        document.querySelector('#MPPT12Voltage'));

    //Inverter PV Details Current
    updateTag18(
        dataCollection.get(`Project1Inverter3.MPPT1Current`),
        document.querySelector('#MPPT1Current'));
    updateTag19(
        dataCollection.get(`Project1Inverter3.MPPT2Current`),
        document.querySelector('#MPPT2Current'));
    updateTag20(
        dataCollection.get(`Project1Inverter3.MPPT3Current`),
        document.querySelector('#MPPT3Current'));
    updateTag21(
        dataCollection.get(`Project1Inverter3.MPPT4Current`),
        document.querySelector('#MPPT4Current'));
    updateTag22(
        dataCollection.get(`Project1Inverter3.MPPT5Current`),
        document.querySelector('#MPPT5Current'));
    updateTag23(
        dataCollection.get(`Project1Inverter3.MPPT6Current`),
        document.querySelector('#MPPT6Current'));
    updateTag24(
        dataCollection.get(`Project1Inverter3.MPPT7Current`),
        document.querySelector('#MPPT7Current'));
    updateTag25(
        dataCollection.get(`Project1Inverter3.MPPT8Current`),
        document.querySelector('#MPPT8Current'));
    updateTag26(
        dataCollection.get(`Project1Inverter3.MPPT9Current`),
        document.querySelector('#MPPT9Current'));
    //Update 3 string từ 10-12
    updateTag41(
        dataCollection.get(`Project1Inverter3.MPPT10Current`),
        document.querySelector('#MPPT10Current'));
    updateTag42(
        dataCollection.get(`Project1Inverter3.MPPT11Current`),
        document.querySelector('#MPPT11Current'));
    updateTag43(
        dataCollection.get(`Project1Inverter3.MPPT12Current`),
        document.querySelector('#MPPT12Current'));

    //Inverter Information (Add Update)
    updateTag27(
        dataCollection.get(`Project1Inverter3.GridVoltageA`),
        document.querySelector('#GridVoltageA'));
    updateTag28(
        dataCollection.get(`Project1Inverter3.GridVoltageB`),
        document.querySelector('#GridVoltageB'));
    updateTag29(
        dataCollection.get(`Project1Inverter3.GridVoltageC`),
        document.querySelector('#GridVoltageC'));
    updateTag30(
        dataCollection.get(`Project1Inverter3.GridVoltageAB`),
        document.querySelector('#GridVoltageAB'));
    updateTag31(
        dataCollection.get(`Project1Inverter3.GridVoltageBC`),
        document.querySelector('#GridVoltageBC'));
    updateTag32(
        dataCollection.get(`Project1Inverter3.GridVoltageCA`),
        document.querySelector('#GridVoltageCA'));
    updateTag33(
        dataCollection.get(`Project1Inverter3.GridCurrentA`),
        document.querySelector('#GridCurrentA'));
    updateTag34(
        dataCollection.get(`Project1Inverter3.GridCurrentB`),
        document.querySelector('#GridCurrentB'));
    updateTag35(
        dataCollection.get(`Project1Inverter3.GridCurrentC`),
        document.querySelector('#GridCurrentC'));
    updateTag36(
        dataCollection.get(`Project1Inverter3.DCPower`),
        document.querySelector('#DCPower'));
    dataTask.start();
});

function updateTag1(dataTag, element) {
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

function updateTag2(dataTag, element) {
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

function updateTag3(dataTag, element) {
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
function updateTag4(dataTag, element) {
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

function updateTag5(dataTag, element) {
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
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
                var x = Number(data.e.newValue);
                element.innerHTML = parseFloat(x.toFixed(1));
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

function updateTag22(dataTag, element) {
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

function updateTag23(dataTag, element) {
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

function updateTag24(dataTag, element) {
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

function updateTag25(dataTag, element) {
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

function updateTag26(dataTag, element) {
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

function updateTag27(dataTag, element) {
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
function updateTag28(dataTag, element) {
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

function updateTag29(dataTag, element) {
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

function updateTag30(dataTag, element) {
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

function updateTag31(dataTag, element) {
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

function updateTag32(dataTag, element) {
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

function updateTag33(dataTag, element) {
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

function updateTag34(dataTag, element) {
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

function updateTag35(dataTag, element) {
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

function updateTag36(dataTag, element) {
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

function updateTag37(dataTag, element) {
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

function updateTag38(dataTag, element) {
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

function updateTag39(dataTag, element) {
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

function updateTag40(dataTag, element) {
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

function updateTag41(dataTag, element) {
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

function updateTag42(dataTag, element) {
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

function updateTag43(dataTag, element) {
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