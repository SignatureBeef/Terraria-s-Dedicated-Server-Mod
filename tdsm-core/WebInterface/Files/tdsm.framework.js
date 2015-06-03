function ModularFramework() {
    this.Version = '1';
    this.Net = null;
};

ModularFramework.prototype.Debug = {
    Log: function (fmt) {
        //TODO: apply args
        console.log('[Debug] ' + fmt);
    },
    Raw: function (value) {
        console.log('[Debug below]');
        console.log(value);
    }
};

ModularFramework.prototype.Load = function () {
    if ('$' in window && 'TDSMNetworking' in window && 'OnPageReady' in window) {
        var port = this.GetPort();
        if (typeof port == 'number') {
            this.Net = new TDSMNetworking(port);
            window.OnPageReady();
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


ModularFramework.prototype.LoadDependency = function (dep) {
    var el;
    switch (dep.type) {
        case ResourceType.Javascript:
            el = document.createElement('SCRIPT');
            el.src = dep.url;
            el.async = true;
            break;
        case ResourceType.Stylesheet:
            el = document.createElement('STYLE');
            el.type = 'text/css';
            el.rel = 'stylesheet';
            el.href = dep.url;
            break;
        default:
            throw 'Invalid dependency';
            break;
    }

    if (el) {
        document.body.appendChild(el);
    }
};

//var Framework = (function () {
//    var fw = new ModularFramework();
//    fw.Load();
//    return fw;
//})();

window.TFramework = new ModularFramework();
window.TFramework.Load();

Math.roundTo = function (value, ep) {
    var v = 10.0 * ep;
    return Math.round(value * v) / v;
};