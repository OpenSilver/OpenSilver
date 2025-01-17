
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

    const mauiHybrid = 'OpenSilver.MauiHybrid';
    return {
        OnCallbackFromJavaScript: function (callbackId, idWhereCallbackArgsAreStored, callbackArgsObject, returnValue) {
            callbackArgsObject = prepareCallbackArgs(callbackArgsObject);
            DotNet.invokeMethodAsync(
                mauiHybrid,
                'InkoveFromJs',
                callbackId,
                idWhereCallbackArgsAreStored,
                callbackArgsObject
            );
        },
        OnCallbackFromJavaScriptWithResult: function () { },
        OnCallbackFromJavaScriptError: function (idWhereCallbackArgsAreStored) {
            DotNet.invokeMethodAsync(
                mauiHybrid,
                'ErrorFromJs',
                idWhereCallbackArgsAreStored
            );
        }
    };
})();

function jsMauiHybrid(javaScriptToExecute) {
    try {
        const result = eval(javaScriptToExecute);

        const resultType = typeof result;
        if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
            return result;
        } else if (result == null) {
            return null;
        } else {
            return result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")";
        }
    } catch (err) {
        console.error(err);
    }
}