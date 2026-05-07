document.addEventListener("DOMContentLoaded", function () {
    var atscadaTask = document.querySelector('atscada-task');
    var dataTask = atscadaTask.dataTask;
    var dataCollection = dataTask.dataCollection;
    //Khai báo các tag
    dataCollection.add(`Project2PowerMeter.ActivePower`);
    dataCollection.add(`Project2PowerMeter.ReactivePower`);
    dataCollection.add(`ITNProject2Common.DailyEnergy`);
    dataCollection.add(`ITNProject2Common.MonthlyEnergy`);
    dataCollection.add(`ITNProject2Common.YearlyEnergy`);
    dataCollection.add(`Project2PowerMeter.TotalEnergy`);
    dataCollection.add(`ITNProject2Common.TotalInverterON`);
    dataCollection.add(`ITNProject2Common.TotalInverterOFF`);
    dataCollection.add(`ITNProject2Common.TotalInverterFault`);
    dataCollection.add(`ITNProject2Common.TotalInverterStandby`);

    dataCollection.add(`Project2Inverter1.OutputActivePower`);
    dataCollection.add(`Project2Inverter2.OutputActivePower`);
    dataCollection.add(`Project2Inverter3.OutputActivePower`);
    dataCollection.add(`Project2Inverter4.OutputActivePower`);
    dataCollection.add(`Project2Inverter5.OutputActivePower`);
    dataCollection.add(`Project2Inverter6.OutputActivePower`);
    dataCollection.add(`Project2Inverter7.OutputActivePower`);
    dataCollection.add(`Project2Inverter8.OutputActivePower`);
    dataCollection.add(`Project2Inverter9.OutputActivePower`);
    //Get tag
    //Project Information
    updateTag1(
        dataCollection.get(`Project2PowerMeter.ActivePower`),
        document.querySelector('#ActivePower'));
    updateTag2(
        dataCollection.get(`ITNProject2Common.DailyEnergy`),
        document.querySelector('#DailyEnergy'));
    updateTag3(
        dataCollection.get(`ITNProject2Common.MonthlyEnergy`),
        document.querySelector('#MonthlyEnergy'));
    updateTag4(
        dataCollection.get(`ITNProject2Common.YearlyEnergy`),
        document.querySelector('#YearlyEnergy'));
    updateTag5(
        dataCollection.get(`Project2PowerMeter.TotalEnergy`),
        document.querySelector('#TotalEnergy'));
    updateTag6(
        dataCollection.get(`ITNProject2Common.TotalInverterON`),
        document.querySelector('#TotalInverterON'));
    updateTag7(
        dataCollection.get(`ITNProject2Common.TotalInverterOFF`),
        document.querySelector('#TotalInverterOFF'));
    updateTag8(
        dataCollection.get(`ITNProject2Common.TotalInverterFault`),
        document.querySelector('#TotalInverterFault'));
    updateTag9(
        dataCollection.get(`ITNProject2Common.TotalInverterStandby`),
        document.querySelector('#TotalInverterStandby'));
    //Inverter information
    updateTag10(
        dataCollection.get(`Project2Inverter1.OutputActivePower`),
        document.querySelector('#INV1OutputActivePower'));
    updateTag11(
        dataCollection.get(`Project2Inverter2.OutputActivePower`),
        document.querySelector('#INV2OutputActivePower'));
    updateTag12(
        dataCollection.get(`Project2Inverter3.OutputActivePower`),
        document.querySelector('#INV3OutputActivePower'));
    updateTag13(
        dataCollection.get(`Project2Inverter4.OutputActivePower`),
        document.querySelector('#INV4OutputActivePower'));
    updateTag14(
        dataCollection.get(`Project2Inverter5.OutputActivePower`),
        document.querySelector('#INV5OutputActivePower'));
    updateTag15(
        dataCollection.get(`Project2Inverter6.OutputActivePower`),
        document.querySelector('#INV6OutputActivePower'));
    updateTag16(
        dataCollection.get(`Project2Inverter7.OutputActivePower`),
        document.querySelector('#INV7OutputActivePower'));
    updateTag17(
        dataCollection.get(`Project2Inverter8.OutputActivePower`),
        document.querySelector('#INV8OutputActivePower'));
    updateTag18(
        dataCollection.get(`Project2Inverter9.OutputActivePower`),
        document.querySelector('#INV9OutputActivePower'));
    updateTag19(
        dataCollection.get(`Project2PowerMeter.ReactivePower`),
        document.querySelector('#ReactivePower'));
    updateTag21(
        dataCollection.get(`Project2PowerMeter.ActivePower`),
        document.querySelector('#ExcessEnergy'));
    dataTask.start();
});

function updateTag1(dataTag, element) {
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

//function updateTag2(dataTag, element) {
//    if (dataTag && element) {
//        dataTag.dispatcher.on('valueChanged', (data) => {
//            if (data.e.newValue !== undefined) {
//                element.innerText = data.e.newValue * 1000;
//            }
//        });
//        if (dataTag.Value !== undefined) {
//            element.innerText = data.e.newValue * 1000;
//        }
//    }
//}

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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}