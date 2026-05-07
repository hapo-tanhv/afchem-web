document.addEventListener("DOMContentLoaded", function () {
    var atscadaTask = document.querySelector('atscada-task');
    var dataTask = atscadaTask.dataTask;
    var dataCollection = dataTask.dataCollection;
    //Khai báo các tag
    dataCollection.add(`Project1PowerMeter.ActivePower`);
    dataCollection.add(`Project1PowerMeter.ReactivePower`);
    dataCollection.add(`ITNProject1Common.DailyEnergy`);
    dataCollection.add(`ITNProject1Common.MonthlyEnergy`);
    dataCollection.add(`ITNProject1Common.YearlyEnergy`);
    dataCollection.add(`Project1PowerMeter.TotalEnergy`);
    dataCollection.add(`ITNProject1Common.TotalInverterON`);
    dataCollection.add(`ITNProject1Common.TotalInverterOFF`);
    dataCollection.add(`ITNProject1Common.TotalInverterFault`);
    dataCollection.add(`ITNProject1Common.TotalInverterStandby`);

    dataCollection.add(`Project1Inverter1.OutputActivePower`);
    dataCollection.add(`Project1Inverter2.OutputActivePower`);
    dataCollection.add(`Project1Inverter3.OutputActivePower`);
    dataCollection.add(`Project1Inverter4.OutputActivePower`);
    dataCollection.add(`Project1Inverter5.OutputActivePower`);
    dataCollection.add(`Project1Inverter6.OutputActivePower`);
    dataCollection.add(`Project1Inverter7.OutputActivePower`);
    dataCollection.add(`Project1Inverter8.OutputActivePower`);
    //Get tag
    //Project Information
    updateTag1(
        dataCollection.get(`Project1PowerMeter.ActivePower`),
        document.querySelector('#ActivePower'));
    updateTag2(
        dataCollection.get(`ITNProject1Common.DailyEnergy`),
        document.querySelector('#DailyEnergy'));
    updateTag3(
        dataCollection.get(`ITNProject1Common.MonthlyEnergy`),
        document.querySelector('#MonthlyEnergy'));
    updateTag4(
        dataCollection.get(`ITNProject1Common.YearlyEnergy`),
        document.querySelector('#YearlyEnergy'));
    updateTag5(
        dataCollection.get(`Project1PowerMeter.TotalEnergy`),
        document.querySelector('#TotalEnergy'));
    updateTag6(
        dataCollection.get(`ITNProject1Common.TotalInverterON`),
        document.querySelector('#TotalInverterON'));
    updateTag7(
        dataCollection.get(`ITNProject1Common.TotalInverterOFF`),
        document.querySelector('#TotalInverterOFF'));
    updateTag8(
        dataCollection.get(`ITNProject1Common.TotalInverterFault`),
        document.querySelector('#TotalInverterFault'));
    updateTag9(
        dataCollection.get(`ITNProject1Common.TotalInverterStandby`),
        document.querySelector('#TotalInverterStandby'));
    //Inverter information
    updateTag10(
        dataCollection.get(`Project1Inverter1.OutputActivePower`),
        document.querySelector('#INV1OutputActivePower'));
    updateTag11(
        dataCollection.get(`Project1Inverter2.OutputActivePower`),
        document.querySelector('#INV2OutputActivePower'));
    updateTag12(
        dataCollection.get(`Project1Inverter3.OutputActivePower`),
        document.querySelector('#INV3OutputActivePower'));
    updateTag13(
        dataCollection.get(`Project1Inverter4.OutputActivePower`),
        document.querySelector('#INV4OutputActivePower'));
    updateTag14(
        dataCollection.get(`Project1Inverter5.OutputActivePower`),
        document.querySelector('#INV5OutputActivePower'));
    updateTag15(
        dataCollection.get(`Project1Inverter6.OutputActivePower`),
        document.querySelector('#INV6OutputActivePower'));
    updateTag16(
        dataCollection.get(`Project1Inverter7.OutputActivePower`),
        document.querySelector('#INV7OutputActivePower'));
    updateTag17(
        dataCollection.get(`Project1Inverter8.OutputActivePower`),
        document.querySelector('#INV8OutputActivePower'));
    updateTag18(
        dataCollection.get(`Project1PowerMeter.ActivePower`),
        document.querySelector('#ExcessEnergy'));
    updateTag19(
        dataCollection.get(`Project1PowerMeter.ReactivePower`),
        document.querySelector('#ReactivePower'));
    dataTask.start();
});

function updateTag1(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                var x = Number(data.e.newValue);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                element.innerHTML = data.e.newValue;
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
                element.innerHTML = data.e.newValue;
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
                element.innerHTML = data.e.newValue;
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
                element.innerHTML = data.e.newValue;
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}