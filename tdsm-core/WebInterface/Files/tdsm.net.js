function TDSMNetworking(port) {
    this.baseUrl = window.location.protocol + '//' + window.location.hostname + ':' + port + '/';
};

TDSMNetworking.prototype.Ping = function (onResult) {
    GetUrlReader(this.baseUrl + 'api/info', function (reader) {
        if (reader) {
            var provider = reader.ReadString();
            var apiVers = reader.ReadInt32();
            var coreVers = reader.ReadInt32();
        }
        else onResult(false);
    });
};

function GetUrlReader(url, onResult) {
    var request = new XMLHttpRequest();
    request.open("GET", url, true);
    request.responseType = "arraybuffer";

    request.onload = function (oEvent) {
        var arrayBuffer = this.response;
        if (arrayBuffer) {
            onResult(new ByteReader(new Uint8Array(arrayBuffer)));
        }
        else onResult(false);
    };

    request.send(null);
};

function ByteReader(buffer) {
    this.buffer = buffer;
    this.index = 0;
};

ByteReader.prototype.ReadInt32 = function () {
    return ((this.buffer[this.index++]) |
            (this.buffer[this.index++] << 8) |
            (this.buffer[this.index++] << 16) |
            (this.buffer[this.index++] << 24))
};

ByteReader.prototype.ReadString = function () {
    var length = this.ReadInt32();
    var section = this.buffer.slice(this.index, this.index + length);
    this.index += length;
    return String.fromCharCode.apply(null, section);
};
