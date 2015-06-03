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
    this.adminscreen.AddPanel = function (index, text, onTransition) {
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

    TFramework.Net.GetModules(function (modules) {
        TFramework.Debug.Raw(modules);
        if (modules) {
            for (var x = 0; x < modules.length; x++) {
                TFramework.LoadDependency(modules[x]);
            }
        }
    });

    return this.modal({
        html: 'Loading admin interface, please wait...'
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


var TUI = {
    Percentage_Tile: function (className) {
        var inner = '';

        inner += '<canvas width=110 height=110></canvas>'
        inner += '<div class="ui-p-text"></div>'

        return '<div class="ui-progress-circle ' + className + '">' + inner + '</div>';
    }
};

function AnimateCanvasCircle(canvas, percentage, circleText) {
    var context = canvas.getContext('2d');
    var radius = 50;

    //Reset
    context.setTransform(1, 0, 0, 1, 0, 0);
    context.clearRect(0, 0, canvas.width, canvas.height);

    context.translate(canvas.width / 2.0, canvas.height / 2.0);
    context.rotate(-Math.PI / 2);

    context.beginPath();
    context.arc(0, 0, radius, 0, (2 * percentage) * Math.PI, false);
    context.lineWidth = 5;

    context.strokeStyle = '#00aa00';
    context.stroke();

    context.rotate(Math.PI / 2);
    context.font = '25px Calibri';
    context.textBaseline = 'middle';
    context.textAlign = 'center';
    context.fillStyle = 'black';
    context.fillText(circleText, 0, 0);
};

function SetProgress(className, progress, circleText, subText) {
    var el = $('DIV.ui-progress-circle.' + className);
    if (el && el.length > 0) {
        el = el.first();
        var canvas = el.find('CANVAS').first()[0];

        AnimateCanvasCircle(canvas, progress, circleText);

        el.find('DIV.ui-p-text').html(subText);
    }
};