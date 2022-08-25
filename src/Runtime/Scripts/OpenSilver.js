

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



//new Element("link",   { rel:"stylesheet", src: "cshtml5.css", type: "text/css" });
var link = document.createElement('link');
link.setAttribute('rel', 'stylesheet');
link.setAttribute('type', 'text/css');
link.setAttribute('href', 'libs/cshtml5.css');
document.getElementsByTagName('head')[0].appendChild(link);

//new Element("link",   { rel:"stylesheet", src: "flatpickr.css", type: "text/css" });
var link = document.createElement('link');
link.setAttribute('rel', 'stylesheet');
link.setAttribute('type', 'text/css');
link.setAttribute('href', 'libs/flatpickr.css');
document.getElementsByTagName('head')[0].appendChild(link);

//new Element("script", { src: "cshtml5.js", type: "application/javascript" });
var cshtml5Script = document.createElement('script');
cshtml5Script.setAttribute('type', 'application/javascript');
cshtml5Script.setAttribute('src', 'libs/cshtml5.js');
document.getElementsByTagName('head')[0].appendChild(cshtml5Script);

//new Element("script", { src: "fastclick.js", type: "application/javascript" });
var fastclickScript = document.createElement('script');
fastclickScript.setAttribute('type', 'application/javascript');
fastclickScript.setAttribute('src', 'libs/fastclick.js');
document.getElementsByTagName('head')[0].appendChild(fastclickScript);

//new Element("script", { src: "velocity.js", type: "application/javascript" });
var velocityScript = document.createElement('script');
velocityScript.setAttribute('type', 'application/javascript');
velocityScript.setAttribute('src', 'libs/velocity.js');
document.getElementsByTagName('head')[0].appendChild(velocityScript);

//new Element("script", { src: "flatpickr.js", type: "application/javascript" });
var velocityScript = document.createElement('script');
velocityScript.setAttribute('type', 'application/javascript');
velocityScript.setAttribute('src', 'libs/flatpickr.js');
document.getElementsByTagName('head')[0].appendChild(velocityScript);


//new Element("script", { src: "ResizeSensor.js", type: "application/javascript" });
var velocityScript = document.createElement('script');
velocityScript.setAttribute('type', 'application/javascript');
velocityScript.setAttribute('src', 'libs/ResizeSensor.js');
document.getElementsByTagName('head')[0].appendChild(velocityScript);

//new Element("script", { src: "ResizeObserver.js", type: "application/javascript" });
var velocityScript = document.createElement('script');
velocityScript.setAttribute('type', 'application/javascript');
velocityScript.setAttribute('src', 'libs/ResizeObserver.js');
document.getElementsByTagName('head')[0].appendChild(velocityScript);


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
                    callbackArgs = args;
                    for (let i = 0; i < args.length; i++) {
                        let itemType = typeof args[i];
                        if (!(args[i] === null || itemType === 'number' || itemType === 'string' || itemType === 'boolean' ||
                            // Check for TypedArray. This is used for reading binary data for FileReader for example
                            (ArrayBuffer.isView(args[i]) && !(args[i] instanceof DataView))
                        )) {
                            callbackArgs = [];
                            break;
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
    //console.log(javaScriptToExecute);

    var result = eval(javaScriptToExecute);
    //console.log(result);
    var resultType = typeof result;
    if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
        //if (typeof result !== 'undefined' && typeof result !== 'function') {
        //console.log("supported");
        return result;
    }
    else {
        //console.log("not supported");
        if (resultType === 'undefined')
            return "[UNDEFINED]";
        else
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
    else {
        if (resultType === 'undefined')
            return BINDING.js_to_mono_obj("[UNDEFINED]");
        else
            return BINDING.js_to_mono_obj(result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")");
    }
}; 