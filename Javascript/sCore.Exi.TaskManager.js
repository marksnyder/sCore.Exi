
var sCoreExiTaskmgr = {};

sCoreExiTaskmgr = function () {

    var _pub = {};
    var _tasks = [];
    var _initialized = false;
    var _debug = false;

    _pub.initTaskmgr = function () {
        _initialized = true;
        setInterval(function () { }, 100000);
        setInterval(sCoreExiTaskmgr.launchTasks, 1000);
    }

    _pub.scheduleTask = function (id,func,seconds) {
        if (_initialized == false) {
            sCoreExiTaskmgr.initTaskmgr();
        }

        /* Ensure that the same task is not registered more than once */
        for (i = 0; i < _tasks.length; i++) {
            if (_tasks[i]["id"] == id) {
                return;
            }
        }

        _tasks.push({ id: id, func: func, seconds: seconds, ticks: 0, running: false });

        sCoreExiTaskmgr.debugMessage('Registered ' + id);
    }
    
    _pub.markTaskComplete = function (id) {

        sCoreExiTaskmgr.debugMessage('Finished ' + id);

        for (i = 0; i < _tasks.length; i++) {
            if (_tasks[i]["id"] == id) {
                _tasks[i]["running"] = false;
                _tasks[i]["ticks"] = 0;
                return;
            }
        }
    }

    _pub.launchTasks = function()
    {

        for (i = 0; i < _tasks.length; i++) {
            if (_tasks[i]["running"] == false) {

                if (_tasks[i]["ticks"] > _tasks[i]["seconds"]) {
                    _tasks[i]["ticks"] = 0;
                    _tasks[i]["running"] = true;

                    eval(_tasks[i]["func"]);

                    sCoreExiTaskmgr.debugMessage('Launched ' + _tasks[i]["id"]);
                }
                else {
                    _tasks[i]["ticks"] = _tasks[i]["ticks"] + 1;
                }

            }
        }
    }


    _pub.startDebugging = function () {
        _debug = true;
    }

    _pub.debugMessage = function (message) {
        if (_debug) {
            console.log(message);
        }
    };

    return _pub;
}();