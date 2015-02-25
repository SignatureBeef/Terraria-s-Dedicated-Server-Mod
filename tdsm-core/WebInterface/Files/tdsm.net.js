function TDSMNetworking(port) {
    this.baseUrl = window.location.protocol + '//' + window.location.hostname + ':' + port + '/';
    this.nonce = '';
};

TDSMNetworking.prototype.Ping = function (onResult) {
    GetUrlReader(this.baseUrl + 'api/info', function (reader) {
        if (reader) {
            onResult({
                provider: reader.ReadString(),
                api: reader.ReadInt32(),
                core: reader.ReadInt32()
            });
        }
        else onResult(false);
    });
};

TDSMNetworking.prototype.Login = function (auth, onResult) {
    GetUrlReader(this.baseUrl + 'api/auth', function (reader) {
        if (reader) {
            onResult(reader.ReadBoolean());
        }
        else onResult(false);
    }, { 'Auth': auth });
};

TDSMNetworking.prototype.GetRandom = function () {
    return this.nonce;
};

function GetUrlReader(url, onResult, headers) {
    var request = new XMLHttpRequest();
    request.open("GET", url, true);
    request.responseType = "arraybuffer";

    if (headers)
        for (var key in headers) {
            request.setRequestHeader(key, headers[key]);
        }

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

ByteReader.prototype.ReadBoolean = function () {
    return this.buffer[this.index++] == 1;
};

ByteReader.prototype.ReadString = function () {
    var length = this.ReadInt32();
    var section = this.buffer.subarray(this.index, this.index + length);
    this.index += length;
    return String.fromCharCode.apply(null, section);
};
