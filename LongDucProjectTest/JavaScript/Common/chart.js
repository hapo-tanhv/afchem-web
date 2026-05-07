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

// The speed gauge
async function cycletime() {

    
    AcivePowerChart();

    setTimeout(cycletime, 3000);
}

function AcivePowerChart() {
    var chartSpeed = Highcharts.chart('container-speed', Highcharts.merge(gaugeOptions, {
    yAxis: {
        min: 0,
        max: 200,
    },

    credits: {
        enabled: false
    },

    series: [{
        name: 'Speed',
        data: [150],
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




