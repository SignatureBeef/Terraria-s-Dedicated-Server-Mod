function OnPageReady() {
    TFramework.Debug.Log('Ready to begin');
    //Setup style
    //Ping the server - returns the basic public info

    TFramework.Net.Ping(function (info) {
        if (info) {

        }
        else alert('Failed to contact the server.');
    });
};

