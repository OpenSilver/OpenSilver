window.onCallBack = (function () {
    function prepareCallbackArgs(args) {
        let callbackArgs;
        switch (typeof args) {
            case 'number':
            case 'string':
            case 'boolean':
                callbackArgs = args;
                break;
            case 'object':
                // if we deal with an array, we need to check
                // that all the items are primitive types.
                if (Array.isArray(args)) {
                    callbackArgs = [];
                    for (let i = 0; i < args.length; i++) {
                        let itemType = typeof args[i];
                        if ((args[i] === null || itemType === 'number' || itemType === 'string' || itemType === 'boolean' ||
                            // Check for TypedArray. This is used for reading binary data for FileReader for example
                            (ArrayBuffer.isView(args[i]) && !(args[i] instanceof DataView))
                        )) {
                            callbackArgs.push(args[i]);
                        } else {
                            callbackArgs.push(undefined);
                        }
                    }
                    break;
                }
            // if args === null, fall to next case.
            case 'undefined':
            default:
                callbackArgs = [];
                break;
        }

        return callbackArgs;
    }

    return {
        OnCallbackFromJavaScript: function (callbackId, idWhereCallbackArgsAreStored, callbackArgsObject, returnValue) {
            callbackArgsObject = prepareCallbackArgs(callbackArgsObject);
            window.external.sendMessage(JSON.stringify({
                type: 'invoke-net',
                callbackId: callbackId,
                idWhereCallbackArgsAreStored: idWhereCallbackArgsAreStored,
                callbackArgsObject: callbackArgsObject
            }));
        },
        OnCallbackFromJavaScriptWithResult: function () { },
        OnCallbackFromJavaScriptError: function (idWhereCallbackArgsAreStored) {
            window.external.sendMessage(JSON.stringify({
                type: 'js-error',
                idWhereCallbackArgsAreStored: idWhereCallbackArgsAreStored
            }));
        }
    };
})();

window._photinoRuntime = (function () {
    const _promises = [];

    (function () {
        const styleheets = ['libs/cshtml5.css', 'libs/quill.core.css'];
        const scripts = ['libs/cshtml5.js', 'libs/quill.min.js', 'libs/htmlToImage.js', 'libs/FileSaver.min.js'];
        const timestamp = '?date=' + new Date().toISOString();

        styleheets.forEach((name) => {
            _promises.push(new Promise((resolve, reject) => {
                const url = name + timestamp;
                const stylesheet = document.createElement('link');
                stylesheet.setAttribute('rel', 'stylesheet');
                stylesheet.setAttribute('type', 'text/css');
                stylesheet.setAttribute('href', url);
                stylesheet.onload = () => { resolve(url); };
                stylesheet.onerror = () => { reject(`Failed to load: ${url}`); };
                document.getElementsByTagName('head')[0].appendChild(stylesheet);
            }));
        });

        scripts.forEach((name) => {
            _promises.push(new Promise((resolve, reject) => {
                const url = name + timestamp;
                const script = document.createElement('script');
                script.setAttribute('type', 'application/javascript');
                script.setAttribute('src', url);
                script.onload = () => { resolve(url); };
                script.onerror = () => { reject(`Failed to load: ${url}`); };
                document.getElementsByTagName('head')[0].appendChild(script);
            }));
        });
    })();

    window.external.receiveMessage(msg => {
        const jsMsg = JSON.parse(msg);
        let result = eval(jsMsg.js);
        const resultType = typeof result;
        if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
            result = result;
        } else if (result == null) {
            result = null;
        } else {
            result = result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")";
        }

        window.external.sendMessage(JSON.stringify({ type: 'response', id: jsMsg.id, result: result}));
    });

    return {
        startAsync: async function () {
            try {
                await Promise.all(_promises);
                return true;
            } catch (error) {
                console.error(error);
                alert(error);
                return false;
            }
        }
    }
})();

window._photinoRuntime.startAsync().then(() => {
    window.external.sendMessage(JSON.stringify({ type: 'start' }));
});
