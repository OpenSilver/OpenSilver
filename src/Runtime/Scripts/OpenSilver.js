

/*===================================================================================
*
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*
*   This file is part of both the OpenSilver Runtime (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Runtime (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/

window.getOSFilesLoadedPromise = (function () {
    const styleheets = ['libs/cshtml5.css', 'libs/quill.core.css'];
    const scripts = ['libs/cshtml5.js', 'libs/ResizeObserver.js', 'libs/quill.min.js', 'libs/html2canvas.js'];

    const _promises = [];
    const _timestamp = '?date=' + new Date().toISOString();

    styleheets.forEach((name) => {
        _promises.push(new Promise((resolve, reject) => {
            const url = name + _timestamp;
            const stylesheet = document.createElement('link');
            stylesheet.setAttribute('rel', 'stylesheet');
            stylesheet.setAttribute('type', 'text/css');
            stylesheet.setAttribute('href', url);
            stylesheet.onload = function () { resolve(url); }
            stylesheet.onerror = function () {
                console.error('Failed to load ' + name);
                reject(url);
            }
            document.getElementsByTagName('head')[0].appendChild(stylesheet);
        }));
    });

    scripts.forEach((name) => {
        _promises.push(new Promise((resolve, reject) => {
            const url = name + _timestamp;
            const script = document.createElement('script');
            script.setAttribute('type', 'application/javascript');
            script.setAttribute('src', url);
            script.onload = function () { resolve(url); };
            script.onerror = function () {
                console.error('Failed to load ' + name);
                reject(url);
            };
            document.getElementsByTagName('head')[0].appendChild(script);
        }));
    });

    return async function () {
        try {
            await Promise.all(_promises);
            return true;
        } catch (error) {
            return false;
        }
    };
})();

window.onCallBack = (function () {
    const opensilver = "OpenSilver";
    const opensilver_js_callback = "OnCallbackFromJavaScriptBrowser";
    const opensilver_js_error_callback = "OnCallbackFromJavaScriptError";

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
            let formattedArgs = prepareCallbackArgs(callbackArgsObject);
            const res = DotNet.invokeMethod(opensilver, opensilver_js_callback, callbackId, idWhereCallbackArgsAreStored, formattedArgs, returnValue || false);
            if (returnValue) {
                return res;
            }
        },

        OnCallbackFromJavaScriptError: function (idWhereCallbackArgsAreStored) {
            DotNet.invokeMethod(opensilver, opensilver_js_error_callback, idWhereCallbackArgsAreStored);
        }
    };
})();

window.callJS = function (javaScriptToExecute) {
    var result = eval(javaScriptToExecute);
    var resultType = typeof result;
    if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
       return result;
    } else if (result == null) {
        return null;
    } else {     
        return result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")";
    }
};

window.callJSUnmarshalled = function (javaScriptToExecute) {
    javaScriptToExecute = BINDING.conv_string(javaScriptToExecute);
    var result = eval(javaScriptToExecute);
    var resultType = typeof result;
    if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
        return BINDING.js_to_mono_obj(result);
    }
    else if (result == null) {
        return null;
    } else {
        return BINDING.js_to_mono_obj(result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")");
    }
};

window.callJSUnmarshalled_v2 = function (javaScriptToExecute, referenceId, wantsResult) {
    javaScriptToExecute = BINDING.conv_string(javaScriptToExecute);
    var result = eval(javaScriptToExecute);

    if (referenceId >= 0)
        document.jsObjRef[referenceId.toString()] = result;

    if (!wantsResult) 
        return;

    var resultType = typeof result;
    if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
        return BINDING.js_to_mono_obj(result);
    } else if (result == null) {
        return null;
    } else {
        return BINDING.js_to_mono_obj(result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")");
    }
};

// IMPORTANT: this doesn't return anything (this just executes the pending async JS)
window.callJSUnmarshalledHeap = (function () {
    const textDecoder = new TextDecoder('utf-16le');
    return function (arrAddress, length) {
        const byteArray = Module.HEAPU8.subarray(arrAddress + 16, arrAddress + 16 + length);
        const javaScriptToExecute = textDecoder.decode(byteArray);
        eval(javaScriptToExecute);
    };
})();
