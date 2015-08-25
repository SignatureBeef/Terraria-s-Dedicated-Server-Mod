//Create our listener
(window.OnConfigReady = function () {
    //Test to ensure our functionality is ready (this is an example, as we already know these are created otherwise we wouldn't be loaded)
    if ('TInterface' in window && 'adminscreen' in TInterface) {
        TInterface.adminscreen.AddPanel(3, 'Configuration', function () {
            var panel = this.TransitionTo('<div>CONFIG</div>', function (panel) {
                //Load information
            });
        });
    }
    else {
        window.setTimeout(window.OnConfigReady, 50);
    }
})();