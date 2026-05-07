document.addEventListener("DOMContentLoaded", function () {
    var atscadaTask = document.querySelector('atscada-task');
    var dataTask = atscadaTask.dataTask;
    var dataCollection = dataTask.dataCollection;

    dataCollection.add(`WetherStationPR3.AmbientTemp`);
    dataCollection.add(`WetherStationPR3.Irradiation`);
    dataCollection.add(`WetherStationPR3.PVmoduleTemp1`);
    dataCollection.add(`WetherStationPR3.PVmoduleTemp2`);

    updateTag1(
        dataCollection.get(`WetherStationPR3.AmbientTemp`),
        document.querySelector('#AmbientAirTemperature'));
    updateTag2(
        dataCollection.get(`WetherStationPR3.Irradiation`),
        document.querySelector('#Irradiance'));
    updateTag3(
        dataCollection.get(`WetherStationPR3.PVmoduleTemp1`),
        document.querySelector('#BackOfModuleTemperature1'));
    updateTag4(
        dataCollection.get(`WetherStationPR3.PVmoduleTemp2`),
        document.querySelector('#BackOfModuleTemperature2'));
    dataTask.start();
});

function updateTag1(dataTag, element) {
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

function updateTag2(dataTag, element) {
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

function updateTag3(dataTag, element) {
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

function updateTag4(dataTag, element) {
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

