//Create our listener
(window.OnFileBrowserReady = function () {
    //Test to ensure our functionality is ready (this is an example, as we already know these are created otherwise we wouldn't be loaded)
    if ('TInterface' in window && 'adminscreen' in TInterface) {
        TInterface.adminscreen.AddPanel(4, 'File Browser', function () {
            var panel = this.TransitionTo('<div>...</div>', function (panel) {
                //Load information
            });
        });
    }
    else {
        window.setTimeout(window.OnFileBrowserReady, 50);
    }
})();