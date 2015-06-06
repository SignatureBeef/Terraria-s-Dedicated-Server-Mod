//Create our listener
(window.OnServerOverviewReady = function () {
    //Test to ensure our functionality is ready (this is an example, as we already know these are created otherwise we wouldn't be loaded)
    if ('TInterface' in window && 'adminscreen' in TInterface && 'TUI' in window) {
        StartMonitoring();
        FetchInfo();

        var html = ''; //<div class="ui-serveroverview">';

        html += '<div class="progress-tiles">';
        html += TUI.Percentage_Tile('ui-players');
        html += TUI.Percentage_Tile('ui-cpu');
        html += TUI.Percentage_Tile('ui-mem');
        html += '</div>';

        html += '<section class="ui-serveroverview">';
        html += '<h4 class="uid-serverstatus"></h4>';
        html += '<div class="uid-worldname"><h5>World name:</h5><input type=text disabled/></div>';

        html += '<div class="uid-worldsize"><h5>World size:</h5>';
        html += '<span class="uid-filesize"></span>';
        html += '<span class="uid-tilex"></span>';
        html += '<span class="uid-tiley"></span>';
        html += '</div>';

        html += '<div class="uid-heart-enabled"><h5>Heartbeat Enabled:</h5><input type=checkbox disabled /></div>';
        html += '<div class="uid-heart-publish"><h5>Publish to server list:</h5><input type=checkbox disabled /></div>';

        html += '<div class="uid-servername"><h5>Server name:</h5><input type=text disabled /></div>';
        html += '<div class="uid-serverdesc"><h5>Server Description:</h5><textarea disabled></textarea></div>';
        html += '<div class="uid-serverdom"><h5>Server Domain:</h5><input type=text disabled /></div>';
        html += '</section>';

        //html += '</div>';

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

function FetchInfo() {
    GetUrlReader(TFramework.Net.baseUrl + '/api/admin/info', function (reader) {
        if (reader) {
            var inf = {
                WorldName: reader.ReadString(),

                TileSizeX: reader.ReadInt32(),
                TileSizeY: reader.ReadInt32(),

                WorldByteSize: reader.ReadDouble(),

                HeartbeatEnabled: reader.ReadBoolean(),
                ServerName: reader.ReadString(),
                ServerDescription: reader.ReadString(),
                ServerDomain: reader.ReadString(),
                PublishToList: reader.ReadBoolean()
            };

            if (String.isNullOrEmpty(inf.WorldName)) {
                //Keep going until the world is loaded
                setTimeout(FetchInfo, 1000);
            }

            //Set what we actually have
            if (!String.isNullOrEmpty(inf.ServerName)) $('.uid-servername').find('INPUT').val(inf.ServerName).removeAttr('disabled');
            if (!String.isNullOrEmpty(inf.WorldName)) $('.uid-worldname').find('INPUT').val(inf.WorldName).removeAttr('disabled');
            if (!String.isNullOrEmpty(inf.ServerDescription)) $('.uid-serverdesc').find('textarea').val(inf.ServerDescription).removeAttr('disabled');
            if (!String.isNullOrEmpty(inf.ServerDomain)) $('.uid-serverdom').find('INPUT').val(inf.ServerDomain).removeAttr('disabled');

            $('SPAN.uid-filesize').html('<h6>File Size:</h6>' + GetFormattedByteSize(inf.WorldByteSize).toString());
            $('SPAN.uid-tilex').html('<h6>Width:</h6>' + inf.TileSizeX);
            $('SPAN.uid-tiley').html('<h6>Height:</h6>' + inf.TileSizeY);

            $('.uid-heart-enabled').find('INPUT').prop('checked', true == inf.HeartbeatEnabled).removeAttr('disabled');
            $('.uid-heart-publish').find('INPUT').prop('checked', true == inf.PublishToList).removeAttr('disabled');
        }
    });
};

String.isNullOrEmpty = function (v) {
    return v == null || v == undefined || v == '';
};

function StartMonitoring() {
    if (undefined == window._serverOverview) {
        function FetchChanges() {
            GetUrlReader(TFramework.Net.baseUrl + '/api/server/overview', function (reader) {
                if (reader) {
                    var hasData = reader.ReadBoolean();
                    if (hasData) {
                        OnMonitorResult({
                            CPU: reader.ReadDouble(),
                            CPUTimeMs: reader.ReadDouble(),
                            CPUAverage: reader.ReadDouble(),
                            CPUMax: reader.ReadDouble(),

                            MaxMemory: reader.ReadDouble(),

                            Virtual: reader.ReadDouble(),
                            VirtualMax: reader.ReadDouble(),
                            VirtualAverage: reader.ReadDouble(),

                            Working: reader.ReadDouble(),
                            WorkingMax: reader.ReadDouble(),
                            WorkingAverage: reader.ReadDouble(),

                            ConnectedPlayers: reader.ReadInt32(),
                            MaxPlayers: reader.ReadInt32(),

                            ServerUp: reader.ReadBoolean(),
                            AcceptNewConnections: reader.ReadBoolean()
                        });
                    }
                }
            });
        }
        FetchChanges();
        window._serverOverview = window.setInterval(FetchChanges, 3000);
    }
};

function OnMonitorResult(data) {
    console.log(data);

    SetProgress('ui-players', data.ConnectedPlayers / data.MaxPlayers,
        data.ConnectedPlayers,
        '<h6>Connected Players</h6>' +
        '<div><b>Max players</b>: ' + data.MaxPlayers + '</div>');

    SetProgress('ui-cpu', data.CPU / 100.0,
        Math.round(data.CPU) + '%',
        '<h6>CPU Usage</h6>' +
        '<div><b>Average</b>: ' + Math.roundTo(data.CPUAverage, 2) + '%</div>' +
        '<div><b>Max</b>: ' + Math.roundTo(data.CPUMax, 2) + '%</div>');

    SetProgress('ui-mem', data.Working / data.MaxMemory / 100.0,
        //GetFormattedByteSize(data.Working, true).toString(),
        Math.roundTo(data.Working / data.MaxMemory, 2) + '%',
        '<h6>Memory Usage</h6>' +
        '<div><b>Current</b>: ' + GetFormattedByteSize(data.Working).toString() + '</div>' +
        '<div><b>Average</b>: ' + GetFormattedByteSize(data.WorkingAverage).toString() + '</div>' +
        '<div><b>Max</b>: ' + GetFormattedByteSize(data.MaxMemory).toString() + '</div>');

    var notice = 'The server is ';
    if (data.ServerUp) {
        notice += 'online';
    }
    else {
        notice += 'offline';
    }
    notice += ' and is ';
    if (false == data.AcceptNewConnections) {
        notice += 'not ';
    }
    notice += 'accepting new connections.';

    $('.uid-serverstatus').html(notice);
};

var DataSizes = {
    Byte: 'byte',
    Kilobyte: 'kb',
    Megabyte: 'mb',
    Gigabyte: 'gb',
    Terabyte: 'tb',
    Petabyte: 'pb',
    Exabyte: 'eb',
    Zettabyte: 'zb',
    Yottabyte: 'yb'
};
function GetNextDataSize(current) {
    var hit = false;

    for (var x in DataSizes) {
        if (hit) {
            current = DataSizes[x];
            break;
        }
        if (DataSizes[x] == current) {
            hit = true;
        }
    }

    return current;
}

function GetFormattedByteSize(value, noDataSize) {
    var current = DataSizes.Byte;

    while (value > 1024.0) {
        value /= 1024.0;

        if (true != noDataSize) {
            current = GetNextDataSize(current);
        }
    }

    if (true == noDataSize) {
        current = undefined;
    }

    return {
        value: value,
        size: current,
        toString: function () {
            var rnd = Math.roundTo(this.value, 2);
            //return rnd + (this.size !== undefined ? (' ' + this.size + (rnd > 1 ? 's' : '')) : '');
            return rnd + (this.size !== undefined ? (' ' + this.size) : '');
        }
    };
};