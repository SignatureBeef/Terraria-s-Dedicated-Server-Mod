//Create our listener
(window.OnServerOverviewReady = function () {
    //Test to ensure our functionality is ready (this is an example, as we already know these are created otherwise we wouldn't be loaded)
    if ('TInterface' in window && 'adminscreen' in TInterface && 'TUI' in window) {
        StartMonitoring();

        var html = '';

        html += TUI.Percentage_Tile('ui-cpu');
        html += TUI.Percentage_Tile('ui-mem');

        TInterface.adminscreen.AddPanel(1, 'Server Overview', function () {


            this.TransitionTo(html, function (panel) {
                //Load information
            });
        }).click();
    }
    else {
        window.setTimeout(window.OnServerOverviewReady, 50);
    }
})();

function StartMonitoring() {
    if (undefined == window._serverOverview) {
        window._serverOverview = window.setInterval(function () {
            GetUrlReader(TFramework.Net.baseUrl + '/api/server/overview', function (reader) {
                if (reader) {
                    var hasData = reader.ReadBoolean();
                    if (hasData) {
                        OnMonitorResult({
                            MaxMemory: reader.ReadDouble(),

                            CPU: reader.ReadDouble(),
                            CPUTimeMs: reader.ReadDouble(),
                            CPUAverage: reader.ReadDouble(),

                            Virtual: reader.ReadDouble(),
                            VirtualMax: reader.ReadDouble(),
                            VirtualAverage: reader.ReadDouble(),

                            Working: reader.ReadDouble(),
                            WorkingMax: reader.ReadDouble(),
                            WorkingAverage: reader.ReadDouble()
                        });
                    }
                }
            });
        }, 3000);
    }
};

function OnMonitorResult(data) {
    console.log(data);

    SetProgress('ui-cpu', data.CPU / 100.0,
        Math.roundTo(data.CPU, 2) + '%',
        '<h6>CPU Usage</h6>' +
        '<div><b>Average</b>: ' + Math.roundTo(data.CPUAverage, 2) + '</div>')

    SetProgress('ui-mem', data.Working / data.WorkingMax,
        Math.roundTo(data.Working, 2) + 'mb',
        '<h6>Memory Usage</h6>' +
        '<div><b>Average</b>: ' + Math.roundTo(data.WorkingAverage, 2) + 'mb' + '</div>' +
        '<div><b>Max</b>: ' + Math.roundTo(data.MaxMemory, 2) + 'mb' + '</div>')
};
