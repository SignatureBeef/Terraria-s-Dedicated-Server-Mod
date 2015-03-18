function TDSMNetworking(port) {
    this.baseUrl = window.location.protocol + '//' + window.location.hostname + ':' + port;
    this.nonce = '';
};

TDSMNetworking.prototype.GetAuth = function (relativeUrl) {
    var ha1 = window.localStorage.getItem('__UUA');
    var user = window.localStorage.getItem('__UUN');
    var ha2 = md5('auth:' + relativeUrl); //MD5(method:URI)
    var nonce = TFramework.Net.GetRandom();

    return user + '=' + md5(ha1 + ':' + nonce + ':' + ha2) + ',' + window.settings.provider; //username=MD5(HA1:nonce:HA2),realm
};

TDSMNetworking.prototype.Ping = function (onResult) {
    GetUrlReader(this.baseUrl + '/api/info', function (reader) {
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

TDSMNetworking.prototype.Login = function (user, pass, onResult) {
    var relative = '/api/auth';
    var apiUrl = this.baseUrl + relative;
    function GetAuth() {
        var ha1 = md5(user + ':' + window.settings.provider + ':' + pass); //MD5(username:realm:password)
        window.localStorage.setItem('__UUA', ha1);
        window.localStorage.setItem('__UUN', user);

        return TFramework.Net.GetAuth(relative);
    }
    function TryLogin(authentication, callback) {
        GetUrlReader(apiUrl, function (reader) {
            if (reader) {
                onResult(reader.ReadBoolean());
            }
            else onResult(false);
        }, { 'Auth': authentication });
    };
    if (this.nonce == '') {
        GetUrlReader(apiUrl, function (reader) {
            if (reader && TFramework.Net.GetRandom() != '') {
                TryLogin(GetAuth(), onResult);
            }
            else onResult(false);
        }, { 'Auth': GetAuth() });
    }
    else {
        TryLogin(GetAuth(), onResult);
    }
};

TDSMNetworking.prototype.SetRandom = function (nonce) {
    this.nonce = nonce;
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

    request.onload = function (evt) {
        var nonce = this.getResponseHeader("next-nonce");
        if (nonce) {
            TFramework.Net.SetRandom(nonce);
        }

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
