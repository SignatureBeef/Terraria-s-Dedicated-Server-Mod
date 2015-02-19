function ModularFramework() {
    this.Version = '1';
    this.Net = null;
};

ModularFramework.prototype.Debug = {
    Log: function (fmt) {
        //TODO: apply args
        console.log('[Debug] ' + fmt);
    }
};

ModularFramework.prototype.Load = function () {
    if ('$' in window && 'TDSMNetworking' in window) {
        var port = this.GetPort();
        if (typeof port == 'number') {
            this.Net = new TDSMNetworking(port);
            if ('OnReady' in window) window.OnReady();
        }
        else alert('Server setup does not specify the TDSM port!');
    }
    else {
        var self = this;
        window.setTimeout(self.Load.bind(this), 10, self);
    }

    return this;
};

ModularFramework.prototype.GetPort = function () {
    var res = $('META[name="tdsm:port"]').attr('content');
    if (res) {
        var port = parseInt(res);
        if (!isNaN(port)) return port;
    }
    return false;
};

//var Framework = (function () {
//    var fw = new ModularFramework();
//    fw.Load();
//    return fw;
//})();

window.TFramework = (new ModularFramework).Load();