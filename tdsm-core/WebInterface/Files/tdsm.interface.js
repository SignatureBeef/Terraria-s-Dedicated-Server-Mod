function Interface() {
};

Interface.prototype.init = function () {
    this.body = $('BODY');
    this.header = $('HEADER');
    this.window = $('SECTION#InterfaceWindow');
    this.loginscreen = $('SECTION#LoginScreen');
    this.adminscreen = $('SECTION#AdminInterface');

    this.header.hide();
    this.loginscreen.hide();
    this.adminscreen.hide();
};

Interface.prototype.welcome = function () {
    if (this.window.is(':visible')) this.window.fadeOut();
    if (this.loginscreen.is(':visible')) this.loginscreen.fadeOut();

    return this.modal({
        html: 'Loading interface, please wait...'
    });
};

Interface.prototype.admin = function () {
    if (this.window.is(':visible')) this.window.fadeOut();
    if (this.loginscreen.is(':visible')) this.loginscreen.fadeOut();

    this.adminscreen.fadeIn();
    this.current = this.adminscreen;

    this.adminscreen.find('NAV > UL').html('');
    this.adminscreen.AddPanel = function (text, onTransition) {
        var li = $('<li><a>' + text + '</a></li>');
        this.find('NAV > UL').append(li);
        li[0].onTransition = onTransition.bind(this);
        li.click(function (evt) {
            li[0].onTransition();
        });

        return li;
    };
    this.adminscreen.TransitionTo = function (to, onTransitioned) {
        var $to = 'object' == typeof (to) && 'jquery' in to && 'selector' in to ? to : $(to);
        if (undefined !== this.current) this.current.fadeOut(function () {
            var parent = $(this).parent();
            $(this).remove();
            $to.hide();
            parent.append($to);
            $to.fadeIn();
            if (onTransitioned) onTransitioned($to);
        });
        else {
            $to.hide();
            this.find(' > DIV').append($to);
            $to.fadeIn();
            if (onTransitioned) onTransitioned($to);
        }

        this.current = $to;
        return $to;
    };

    this.adminscreen.AddPanel('Server Overview', function () {
        this.TransitionTo('<div><table><tr><td>World Name</td><td class="cfg-worldname"></td></tr></table></div>', function (panel) {
            //Load information
        });
    }).click();
    this.adminscreen.AddPanel('Chat', function () {
        var panel = this.TransitionTo('<div class="chat-box"><textarea disabled></textarea> <input /><button>Send</button></div>', function (panel) {
            //Load information
        });
    });
    this.adminscreen.AddPanel('Configuration', function () {
        var panel = this.TransitionTo('<div>CONFIG</div>', function (panel) {
            //Load information
        });
    });
    this.adminscreen.AddPanel('File Browser', function () {
        var panel = this.TransitionTo('<div>...</div>', function (panel) {
            //Load information
        });
    });

    return this.modal({
        html: 'Loading interface, please wait...'
    });
};

Interface.prototype.login = function () {
    this.loginscreen.fadeIn();
    var btn = this.loginscreen.find('BUTTON');

    btn[0].username = this.loginscreen.find('INPUT#LoginUserName');
    btn[0].password = this.loginscreen.find('INPUT#LoginPassword');

    this.current = this.loginscreen;
    btn.click(function (evt) {
        evt.preventDefault();
        this.disabled = true;

        var user = this.username.val();
        var pass = this.password.val();

        var minUser = 4, minPass = 4;

        if (user.length >= minUser) {
            if (pass.length >= minPass) {
                var overlay = TInterface.modal({
                    html: 'Signing in as ' + user + '...'
                });
                overlay.show();
                TFramework.Net.Login(user, pass, function (info) {
                    overlay.remove();
                    btn[0].disabled = false;
                    console.log(info);
                    if (info) {
                        TInterface.current.find('.ui-error').html('');
                        TInterface.setHeader(window.settings.provider + ' control panel');
                        TInterface.admin();
                    }
                    else {
                        TInterface.current.find('.ui-error').html('Invalid login.');
                    }
                });
            }
            else alert('Invalid password length, minimum ' + minPass);
        }
        else alert('Invalid username length, minimum ' + minUser);
    });
};

Interface.prototype.setHeader = function (provider) {
    this.header.html(provider);
    this.header.fadeIn();
};

Interface.prototype.modal = function (options) {
    //TODO queue
    var mdl = new TModal(options);

    mdl.appendTo(this.body);

    return mdl;
};

Interface.prototype.abort = function (text) {
    var mdl = new TModal({ html: text });
    mdl.appendTo(this.body);
    mdl.show();
};

Interface.prototype.alert = function (text) {

};

Interface.prototype.confirm = function (text) {

};

function TModal(options) {
    this.options = options;
};

TModal.prototype.show = function (onShown) {
    this.content.fadeIn(500, function () {
        if (onShown) onShown();
    });
};
TModal.prototype.remove = function (fade, onRemoved) {
    if (this.content) {
        if (false !== fade) {
            this.content.fadeOut(500, function () {
                this.remove();
                if (onRemoved) onRemoved();
            });
        }
        else {
            this.content.remove();
        }
    }
};
TModal.prototype.appendTo = function (jqElement) {
    var html = '';
    if ('html' in this.options) {
        html = this.options.html;
    }

    if (html && html.length > 0) {
        html = '<div><div class="ui-modal-bg ui-window"></div><div class="ui-modal ui-window"><div>' + html + '</div></div></div>'

        this.content = $(html);
        this.content.hide();

        return jqElement.append(this.content);
    }
};

var TInterface = new Interface();
window.settings = {

};

function OnPageReady() {
    TFramework.Debug.Log('Ready to begin');

    //Initialise the interface
    TInterface.init();

    var overlay = TInterface.welcome();
    overlay.show(function () {
        TFramework.Net.Ping(function (info) {
            overlay.remove();
            if (info) {
                window.settings = {
                    api: info.api,
                    core: info.core,
                    provider: info.provider
                };
                TInterface.setHeader(window.settings.provider + ' server login');
                TFramework.Debug.Log('Settings aquired');

                TInterface.login();
            }
            else TInterface.abort('Failed to contact the server.');
        });
    });
};