

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

window._openSilverRuntime = (function () {
    const _promises = [];
    const _textDecoder = new TextDecoder('utf-16le');

    (function () {
        const styleheets = ['libs/cshtml5.css', 'libs/quill.core.css'];
        const scripts = ['libs/cshtml5.js', 'libs/quill.min.js', 'libs/html2canvas.js', 'libs/FileSaver.min.js'];
        const timestamp = '?date=' + new Date().toISOString();

        styleheets.forEach((name) => {
            _promises.push(new Promise((resolve, reject) => {
                const url = name + timestamp;
                const stylesheet = document.createElement('link');
                stylesheet.setAttribute('rel', 'stylesheet');
                stylesheet.setAttribute('type', 'text/css');
                stylesheet.setAttribute('href', url);
                stylesheet.onload = () => { resolve(url); };
                stylesheet.onerror = () => { reject(url); };
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
                script.onerror = () => { reject(url); };
                document.getElementsByTagName('head')[0].appendChild(script);
            }));
        });
    })();

    return {
        startAsync: async function () {
            try {
                await Promise.all(_promises);
                return true;
            } catch (error) {
                console.error(error);
                return false;
            }
        },
        invokeJS: function (javaScriptToExecute, referenceId) {
            const result = eval(javaScriptToExecute);

            if (referenceId >= 0) {
                document.jsObjRef[referenceId.toString()] = result;
            }

            const resultType = typeof result;
            if (resultType == 'string' || resultType == 'number' || resultType == 'boolean') {
                return result;
            } else if (result == null) {
                return null;
            } else {
                return result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")";
            }
        },
        invokeJSVoid: function (javaScriptToExecute) {
            eval(javaScriptToExecute);
        },
        invokePendingJS: function (span) {
            const view = span._unsafe_create_view();
            const javaScriptToExecute = _textDecoder.decode(view);
            eval(javaScriptToExecute);
        },
        WBM: (function () {
            let _tempPixelsData;

            function smoothCanvasContext(ctx) {
                ctx.imageSmoothingEnabled = true;
                ctx.webkitImageSmoothingEnabled = true;
                ctx.mozImageSmoothingEnabled = true;
                ctx.msImageSmoothingEnabled = true;
            }

            return {
                createFromBitmapSource: function (data, callback) {
                    const img = new Image();
                    img.src = data;
                    img.onload = function () {
                        try {
                            const canvas = document.createElement('canvas');
                            canvas.height = img.height;
                            canvas.width = img.width;
                            const ctx = canvas.getContext('2d');
                            smoothCanvasContext(ctx);
                            ctx.drawImage(img, 0, 0);
                            const imgData = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
                            _tempPixelsData = new Int32Array(imgData.data.buffer);
                            callback(imgData.data.length, imgData.width, imgData.height);
                        } catch (err) {
                            console.error(err);
                            callback(err.message);
                        }
                    }
                },
                renderUIElement: function (id, width, height, transform, callback) {
                    const element = document.getElementById(id);
                    const currentTransform = element.style.transform;
                    element.style.transform = transform;
                    html2canvas(element, { scale: 1 }).then(function (canvas) {
                        try {
                            const ctx = canvas.getContext('2d');
                            smoothCanvasContext(ctx);
                            const w = width > -1 ? width : canvas.width;
                            const h = height > -1 ? height : canvas.height;
                            const imgData = ctx.getImageData(0, 0, w, h);
                            _tempPixelsData = new Int32Array(imgData.data.buffer);
                            callback(imgData.data.length, imgData.width, imgData.height);
                        } catch (err) {
                            console.error(err);
                            callback(err.message);
                        }
                    });
                    element.style.transform = currentTransform;
                },
                fillInt32Buffer: function (buffer) {
                    buffer.set(new Int32Array(_tempPixelsData), 0);
                },
            }
        })(),
    }
})();
