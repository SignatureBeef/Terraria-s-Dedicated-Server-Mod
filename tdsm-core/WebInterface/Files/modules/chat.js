//Create our listener
(window.OnChatReady = function () {
    //Test to ensure our functionality is ready (this is an example, as we already know these are created otherwise we wouldn't be loaded)
    if ('TInterface' in window && 'adminscreen' in TInterface) {
        TInterface.adminscreen.AddPanel(2, 'Chat', function () {
            var panel = this.TransitionTo('<div class="chat-box"><textarea disabled></textarea> <input /><button>Send</button></div>', function (panel) {
                //Load information
            });
        });
    }
    else {
        window.setTimeout(window.OnChatReady, 50);
    }
})();