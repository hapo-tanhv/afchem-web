document.addEventListener("DOMContentLoaded", function () {
    var atscadaTask = document.querySelector('atscada-task');
    var dataTask = atscadaTask.dataTask;
    var dataCollection = dataTask.dataCollection;

    //Khai báo, add thêm tag
    //Inverter information
    dataCollection.add(`Project4SolarPowerMeter.ActivePower`);
    dataCollection.add(`Project4SolarPowerMeter.ActivePower`);
    dataCollection.add(`Project4ExistingPowerMeter.TotalPurchasedEnergy`);
    dataCollection.add(`Project4ExistingPowerMeter.TotalSurplusEnergy`)

    dataCollection.add(`ITNProject4Common.DailyEnergy`);
    dataCollection.add(`ITNProject4Common.MonthlyEnergy`);
    dataCollection.add(`ITNProject4Common.YearlyEnergy`);
    dataCollection.add(`ITNProject4Energy.SelfConsumption`);

    dataCollection.add(`ITNProject4Common.DailyCO2`);
    dataCollection.add(`ITNProject4Common.MonthlyCO2`);
    dataCollection.add(`ITNProject4Common.YearlyCO2`);
    dataCollection.add(`ITNProject4Common.CO2Reduce`);


    //Inverter update tag
    //Inverter Information
    updateTag1(
        dataCollection.get(`Project4SolarPowerMeter.ActivePower`),
        document.querySelector('#PowerGeneration'));
    updateTag2(
        dataCollection.get(`Project4SolarPowerMeter.ActivePower`),
        document.querySelector('#SelfConsumption'));
    updateTag3(
        dataCollection.get(`Project4ExistingPowerMeter.TotalPurchasedEnergy`),
        document.querySelector('#PurchasedPower'));
    updateTag4(
        dataCollection.get(`Project4ExistingPowerMeter.TotalSurplusEnergy`),
        document.querySelector('#SurplusPower'));

    updateTag5(
        dataCollection.get(`ITNProject4Common.DailyEnergy`),
        document.querySelector('#DailySelf'));
    updateTag6(
        dataCollection.get(`ITNProject4Common.MonthlyEnergy`),
        document.querySelector('#MonthlySelf'));
    updateTag7(
        dataCollection.get(`ITNProject4Common.YearlyEnergy`),
        document.querySelector('#YearlySelf'));
    updateTag8(
        dataCollection.get(`ITNProject4Energy.SelfConsumption`),
        document.querySelector('#TotalSelf'));

    updateTag9(
        dataCollection.get(`ITNProject4Common.DailyCO2`),
        document.querySelector('#DailyCO2'));
    updateTag10(
        dataCollection.get(`ITNProject4Common.MonthlyCO2`),
        document.querySelector('#MonthlyCO2'));
    updateTag11(
        dataCollection.get(`ITNProject4Common.YearlyCO2`),
        document.querySelector('#YearlyCO2'));
    updateTag12(
        dataCollection.get(`ITNProject4Common.CO2Reduce`),
        document.querySelector('#TotalCO2'));
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                var x = Number(data.e.newValue / 1000);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                var x = Number(data.e.newValue / 1000);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                var x = Number(data.e.newValue / 1000);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
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
                var x = Number(data.e.newValue / 1000);
                element.innerHTML = x.toLocaleString(undefined, { minimumFractionDigits: 1, maximumFractionDigits: 1 }); // Hiển thị số hàng nghìn ngăn cách bằng dấu phẩy và giữ lại một chữ số hàng thập phân
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}
