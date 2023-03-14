

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

// Load all css and js files
(function (names) {
    names.forEach((name) => {
        var extension = name.split('.').pop();
        var url = name + "?date=" + new Date().toISOString();
        if (extension == 'js') {
            var el = document.createElement('script');
            el.setAttribute('type', 'application/javascript');
            el.setAttribute('src', url);
            document.getElementsByTagName('head')[0].appendChild(el);
        }
        else if (extension == 'css') {
            var el = document.createElement('link');
            el.setAttribute('rel', 'stylesheet');
            el.setAttribute('type', 'text/css');
            el.setAttribute('href', url);
            document.getElementsByTagName('head')[0].appendChild(el);
        }
    });
}(['libs/cshtml5.css',
    'libs/flatpickr.css',
    'libs/quill.core.css',
    'libs/cshtml5.js',
    'libs/velocity.js',
    'libs/flatpickr.js',
    'libs/ResizeSensor.js',
    'libs/ResizeObserver.js',    
    'libs/quill.min.js',
    'libs/html2canvas.js']));

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
    }
    else if (result == null) {
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


/* normally, this function should have 4 args:
window.callJSUnmarshalledHeap = (function () {
    const textDecoder = new TextDecoder('utf-16le');
    return function (arrAddress, length, referenceId, wantsResult) {
        const byteArray = Module.HEAPU8.subarray(arrAddress + 16, arrAddress + 16 + length);
        const javaScriptToExecute = textDecoder.decode(byteArray);
        const result = eval(javaScriptToExecute);
        if (referenceId >= 0)
            document.jsObjRef[referenceId] = result;

        if (!wantsResult) 
            return;
        
        const resultType = typeof result;
        if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
            return BINDING.js_to_mono_obj(result);
        } else if (result == null) {
            return null;
        } else {
            return BINDING.js_to_mono_obj(result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")");
        }
    };
})();

however, due to C#-to-JS InvokeUnmarshalled constraints, I can only have 3 args max.
So I turned referenceId into 2 arguments:
1. if referenceId = 0 -> don't hold the result in jsObjRef and wantsResult=false
2. if referenceId = 1 -> don't hold the result in jsObjRef and wantsResult=true
3. if referenceId > 1 -> hold it in jsObjRef + wantsResult=true
4. If referenceId < -1 -> negate the value and holde it in jsObjRef + wantsResult=true
*/
window.callJSUnmarshalledHeap = (function () {
    const textDecoder = new TextDecoder('utf-16le');
    return function (arrAddress, length, referenceId) {
        const byteArray = Module.HEAPU8.subarray(arrAddress + 16, arrAddress + 16 + length);
        const javaScriptToExecute = textDecoder.decode(byteArray);
        const result = eval(javaScriptToExecute);

        let wantsResult = false;
        if (referenceId == 0)
            wantsResult = false;
        else if (referenceId == 1)
            wantsResult = true;
        else if (referenceId > 1)
            wantsResult = true;
        else {
            referenceId = -referenceId;
            wantsResult = false;
        }
        
        if (referenceId > 1)
            document.jsObjRef[referenceId.toString()] = result;

        if (!wantsResult) 
            return;
        
        const resultType = typeof result;
        if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
            return BINDING.js_to_mono_obj(result);
        } else if (result == null) {
            return null;
        } else {
            return BINDING.js_to_mono_obj(result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")");
        }
    };
})();
