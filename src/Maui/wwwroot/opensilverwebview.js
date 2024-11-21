document.addEventListener('DOMContentLoaded', function () {
    if (typeof __openSilverBridge !== 'undefined') {
        if (__openSilverBridge._domContentLoaded) {
            __openSilverBridge._domContentLoaded();
        }
    }
});

window.onCallBack = (function () {
    const _openSilverCallback = (typeof __openSilverBridge !== 'undefined') ? __openSilverBridge : window.chrome.webview;

    return {
        OnCallbackFromJavaScript: function (callbackId, idWhereCallbackArgsAreStored, callbackArgsObject, returnValue) {
            _openSilverCallback.postMessage(JSON.stringify(
                {
                    methodName: "OnCallbackFromJavaScript", callbackId, idWhereCallbackArgsAreStored, callbackArgsObject, returnValue
                }
            ));
        },
        OnCallbackFromJavaScriptWithResult: function () { },
        OnCallbackFromJavaScriptError: function () { }
    };
})();