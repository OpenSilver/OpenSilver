

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



//------------------------------
// CHECK BROWSER COMPATIBILITY
//------------------------------


function getInternetExplorerVersion() {
    var rv = false;
    if (navigator.appName == 'Microsoft Internet Explorer') {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    else if (navigator.appName == 'Netscape') {
        var ua = navigator.userAgent;
        var re = new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
        var re2 = new RegExp("Edge\/([0-9]{1,}[\.0-9]{0,})");
        if (re2.exec(ua) != null) {
            rv = parseFloat(RegExp.$1);
            window.IS_EDGE = true;
        }
    }
    return rv;
}

var userAgentLowercase = navigator.userAgent.toLowerCase();

window.IE_VERSION = getInternetExplorerVersion();
window.ANDROID_VERSION = (userAgentLowercase.indexOf('android') != -1) ? parseInt(userAgentLowercase.split('android')[1]) : false;
window.FIREFOX_VERSION = ((index = userAgentLowercase.indexOf('firefox')) != -1) ? parseInt(userAgentLowercase.substring(index + 8)) : false;

// Current version does not support IE < 11:
if (window.IE_VERSION && window.IE_VERSION < 10)
    alert("This version of Internet Explorer is not supported yet. Please use Internet Explorer 10 (or newer), Chrome 35 (or newer), Firefox 27 (or newer), Safari 8 (or newer), Safari Mobile iOS 8 (or newer), Opera 24 (or newer), or Android 4.x Browser (or newer). More browsers will be supported in the future.");

// Current version does not support Android < 4:
if (window.ANDROID_VERSION && window.ANDROID_VERSION < 4)
    alert("This version of Android is not supported yet. Please use Android 4.x (or newer), Internet Explorer 11 (or newer), Chrome 35 (or newer), Firefox 27 (or newer), Safari 8 (or newer), Safari Mobile iOS 8 (or newer), or Opera 24 (or newer). More browsers will be supported in the future.");


//------------------------------
// CHECK BROWSER GRID SUPPORT
//------------------------------

var div = document.createElement("div");
div.style.display = "grid";
if (div.style.display == "grid") {
    document.isGridSupported = true;
    document.isMSGrid = false;
} else {
    div.style.display = "-ms-grid";
    if (div.style.display == "-ms-grid") {
        document.isGridSupported = true;
        document.isMSGrid = true;
    } else {
        document.gridSupport = "none";
        document.isGridSupported = false;
        document.isMSGrid = false;
    }
}

//------------------------------
// DEFINE OTHER SCRIPTS
//------------------------------

document.getAppParams = function (element) {
    if (element) {
        return JSON.stringify(
            Array.from(
                element.getElementsByTagName("param"),
                function (p) { return { Name: p.name, Value: p.value }; }
            )
        );
    }

    return JSON.stringify([]);
}

document.ResXFiles = {};

document.jsObjRef = {};
document.callbackCounterForSimulator = 0;

document.performanceCounters = [];

document.addToPerformanceCounters = function (name, initialTime) {
    var elapsedTime = performance.now() - initialTime;
    var counter = document.performanceCounters[name];
    if (counter === undefined) {
        counter = {};
        counter.time = 0;
        counter.count = 0;
        document.performanceCounters[name] = counter;
    }
    counter.time += elapsedTime;
    counter.count += 1;
}

document.interopErrors = {};

document.getElementByIdSafe = function (id) {
    let element = document.getElementById(id);
    if (element == null) {
        element = document.createElement("div");
        if (!document.interopErrors[id]) {
            document.interopErrors[id] = 0;
        }
        document.interopErrors[id]++;
    }
    return element;
}

document.setGridCollapsedDuetoHiddenColumn = function (id) {
    const element = document.getElementById(id);
    if (!element)
        return;

    if (element.getAttribute('data-isCollapsedDueToHiddenColumn' == true)) {
        element.style.overflow = 'visible';
        element.setAttribute('data-isCollapsedDueToHiddenColumn', false);
    }
}

document.getActualWidthAndHeight = function (element) {
    return (typeof element === 'undefined' || element === null) ? '0|0' : element['offsetWidth'].toFixed(3) + '|' + element['offsetHeight'].toFixed(3);
}

document.createElementSafe = function (tagName, id, parentElement, index) {
    const newElement = document.createElement(tagName);

    newElement.setAttribute('id', id);
    newElement.setAttribute('xamlid', id);

    if (typeof parentElement == 'string') {
        parentElement = document.getElementById(parentElement);
    }

    if (parentElement == null) {
        console.log('createElement is failed becaused of the removed parent.');
        return null;
    }

    if (index < 0 || index >= parentElement.children.length) {
        parentElement.appendChild(newElement);
    }
    else {
        var nextSibling = parentElement.children[index];
        parentElement.insertBefore(newElement, nextSibling);
    }

    Object.defineProperty(newElement, 'dump', {
        get() { return document.dumpProperties(id); }
    });

    return newElement;
}

document.dumpProperties = function (id, ...names) {
    if (DotNet && DotNet.invokeMethod) {
        return DotNet.invokeMethod('OpenSilver', 'DumpProperties', id, names);
    }
    return null;
};

document.createTextBlockElement = function (id, parentElement, wrap) {
    const newElement = document.createElementSafe('div', id, parentElement, -1);

    if (newElement) {
        newElement.style['overflow'] = 'hidden';
        newElement.style['textAlign'] = 'start';
        newElement.style['boxSizing'] = 'border-box';
        if (wrap) {
            newElement.style['overflowWrap'] = 'break-word';
            newElement.style['whiteSpace'] = 'pre-wrap';
        } else {
            newElement.style['whiteSpace'] = 'pre';
        }
    }
}

document.createPopupRootElement = function (id, rootElement, pointerEvents) {
    if (!rootElement) return;

    const popupRoot = document.createElement('div');
    popupRoot.setAttribute('id', id);
    popupRoot.setAttribute('xamlid', id);
    popupRoot.style.position = 'absolute';
    popupRoot.style.width = '100%';
    popupRoot.style.height = '100%';
    popupRoot.style.overflowX = 'hidden';
    popupRoot.style.overflowY = 'hidden';
    popupRoot.style.pointerEvents = pointerEvents;
    rootElement.appendChild(popupRoot);
}

document.createCanvasElement = function (id, parentElement) {
    const newElement = document.createElementSafe('div', id, parentElement, -1);

    if (newElement) {
        newElement.style['overflow'] = 'display';
        newElement.style['position'] = 'relative';
    }
}

document.createImageElement = function (id, parentElement) {
    const img = document.createElementSafe('img', id, parentElement, -1);

    if (img) {
        img.setAttribute('alt', ' ');
        img.style.display = 'none';
        img.style.width = 'inherit';
        img.style.height = 'inherit';
        img.style.lineHeight = '0px';
        img.style.objectFit = 'contain';
        img.style.objectPosition = 'left top';
        img.addEventListener('load', function (e) {
            this.style.display = '';
        })
        img.addEventListener('error', function (e) {
            this.style.display = 'none';
        });
    }
}

document.createFrameworkElement = function (id, parentElement, enablePointerEvents) {
    const newElement = document.createElementSafe('div', id, parentElement, -1);

    if (newElement) {
        newElement.style['width'] = '100%';
        newElement.style['height'] = '100%';

        if (enablePointerEvents) {
            newElement.style['pointerEvents'] = 'all';
        }
    }
}

document.createTextElement = function (id, tagName, parent) {
    if (typeof parent === 'string') parent = document.getElementById(parent);
    if (parent === null) return null;

    const textElement = document.createElement(tagName);
    textElement.setAttribute('id', id);

    if (index < 0 || index >= parent.children.length) {
        parent.appendChild(textElement);
    } else {
        parent.insertBefore(textElement, parent.children[index]);
    }
};

document.createShapeOuterElement = function (id, parentElement) {
    const newElement = document.createElementSafe('div', id, parentElement, -1);

    if (newElement) {
        newElement.style['lineHeight'] = '0';     // Line height is not needed in shapes because it causes layout issues.
        newElement.style['fontSize'] = '0';       //this allows this div to be as small as we want (for some reason in Firefox, what contains a canvas has a height of at least about (1 + 1/3) * fontSize)
    }
}

document.createShapeInnerElement = function (id, parentElement) {
    const newElement = document.createElementSafe('canvas', id, parentElement, -1);

    if (newElement) {
        newElement.style['width'] = '0';
        newElement.style['height'] = '0';
    }
}

document.set2dContextProperty = function (id, propertyName, propertyValue) {
    const element = document.getElementById(id);
    if (!element || element.tagName !== 'CANVAS')
        return;

    element.getContext('2d')[propertyName] = propertyValue;
}

document.invoke2dContextMethod = function (id, methodName, args) {
    const element = document.getElementById(id);
    if (!element || element.tagName !== 'CANVAS')
        return undefined;
    return CanvasRenderingContext2D.prototype[methodName].apply(element.getContext('2d'),
        args.split(',')
            .map(Function.prototype.call, String.prototype.trim)
            .filter(i => i.length > 0));
}

document.setDomStyle = function (id, propertyName, value) {
    const element = document.getElementById(id);
    if (!element)
        return;

    element.style[propertyName] = value;
}

document.setDomTransform = function (id, value) {
    const element = document.getElementById(id);
    if (!element)
        return;

    element.style['transform'] = value;
    element.style['msTransform'] = value;
    element.style['WebkitTransform'] = value;
}

document.setDomTransformOrigin = function (id, value) {
    const element = document.getElementById(id);
    if (!element)
        return;

    element.style['transformOrigin'] = value;
    element.style['msTransformOrigin'] = value;
    element.style['WebkitTransformOrigin'] = value;
}

document.setDomAttribute = function (id, propertyName, value) {
    const element = document.getElementById(id);
    if (!element)
        return;

    element.setAttribute(propertyName, value);
}

document.removeEventListenerSafe = function (element, method, func) {
    if (typeof element == 'string') {
        element = document.getElementById(element);
    }
    if (element) {
        element.removeEventListener(method, func);
    }
}

document.addEventListenerSafe = function (element, method, func) {
    if (typeof element == 'string') {
        element = document.getElementById(element);
    }
    if (element) {
        if (method == "touchstart" || method == "wheel" || method == "touchmove") {
            element.addEventListener(method, func, { passive: true });
        }
        else {
            element.addEventListener(method, func);
        }
    }
};

document.setFocus = function (element) {
    if (!element) return;

    setTimeout(function () {
        element.setAttribute('tabindex', 0);
        element.focus({ preventScroll: true });
    });
};

document.createInputManager = function (callback) {
    if (document.inputManager) return;

    // This must remain synchronyzed with the EVENTS enum defined in InputManager.cs.
    // Make sure to change both files if you update this !
    const EVENTS = {
        MOUSE_MOVE: 0,
        MOUSE_LEFT_DOWN: 1,
        MOUSE_LEFT_UP: 2,
        MOUSE_RIGHT_DOWN: 3,
        MOUSE_RIGHT_UP: 4,
        MOUSE_ENTER: 5,
        MOUSE_LEAVE: 6,
        WHEEL: 7,
        KEYDOWN: 8,
        KEYUP: 9,
        FOCUS: 10,
        BLUR: 11,
        KEYPRESS: 12,
        INPUT: 13,
        TOUCH_START: 14,
        TOUCH_END: 15,
        TOUCH_MOVE: 16,
        WINDOW_BLUR: 17,
    };

    const MODIFIERKEYS = {
        NONE: 0,
        CONTROL: 1,
        ALT: 2,
        SHIFT: 4,
        WINDOWS: 8,
    };

    let _modifiers = MODIFIERKEYS.NONE;
    let _mouseCapture = null;
    let _suppressContextMenu = false;
    let _lastTouchEndTimeStamp = 0;

    function setModifiers(e) {
        _modifiers = MODIFIERKEYS.NONE;
        if (e.ctrlKey)
            _modifiers |= MODIFIERKEYS.CONTROL;
        if (e.altKey)
            _modifiers |= MODIFIERKEYS.ALT;
        if (e.shiftKey)
            _modifiers |= MODIFIERKEYS.SHIFT;
        if (e.metaKey)
            _modifiers |= MODIFIERKEYS.WINDOWS;
    };

    function getClosestElementId(element) {
        while (element) {
            if (element.hasAttribute('xamlid')) {
                return element.getAttribute('xamlid');
            }

            element = element.parentElement;
        }

        return '';
    };

    function shouldIgnoreMouseEvent(e) {
        return e.timeStamp - _lastTouchEndTimeStamp < 500;
    };

    function initDom() {
        document.addEventListener('mousedown', function (e) {
            if (!e.isHandled) {
                switch (e.button) {
                    case 0:
                        callback('', EVENTS.MOUSE_LEFT_DOWN, e);
                        break;
                    case 2:
                        callback('', EVENTS.MOUSE_RIGHT_DOWN, e);
                        break;
                }
            }
        });

        document.addEventListener('mouseup', function (e) {
            if (!e.isHandled) {
                const target = _mouseCapture;
                if (target !== null) {
                    switch (e.button) {
                        case 0:
                            callback(getClosestElementId(target), EVENTS.MOUSE_LEFT_UP, e);
                            break;
                        case 2:
                            callback(getClosestElementId(target), EVENTS.MOUSE_RIGHT_UP, e);
                            break;
                    }
                }
            }
        });

        document.addEventListener('mousemove', function (e) {
            if (!e.isHandled) {
                const target = _mouseCapture;
                if (target !== null) {
                    callback(getClosestElementId(target), EVENTS.MOUSE_MOVE, e);
                }
            }
        });

        document.addEventListener('selectstart', function (e) { if (_mouseCapture !== null) e.preventDefault(); });

        document.addEventListener('contextmenu', function (e) {
            if (_suppressContextMenu ||
                (_mouseCapture !== null && this !== _mouseCapture)) {
                _suppressContextMenu = false;
                e.preventDefault();
            }
        });

        document.addEventListener('keydown', function (e) { setModifiers(e); });

        document.addEventListener('keyup', function (e) { setModifiers(e); });        

        window.addEventListener('blur', function (e) {
            callback('', EVENTS.WINDOW_BLUR, e);
            _modifiers = MODIFIERKEYS.NONE;
        });
    };

    initDom();

    document.inputManager = {
        registerRoot: function (root) {
            // Make sure the root div is keyboard focusable, so that we can tab into the app.
            root.tabIndex = Math.max(root.tabIndex, 0);

            root.addEventListener('focusin', function (e) { callback(getClosestElementId(e.target), EVENTS.FOCUS, e); });

            root.addEventListener('mousemove', function (e) {
                if (shouldIgnoreMouseEvent(e)) return;

                e.isHandled = true;
                const target = _mouseCapture || e.target;
                callback(getClosestElementId(target), EVENTS.MOUSE_MOVE, e);
            });

            root.addEventListener('wheel', function (e) {
                e.isHandled = true;
                callback(getClosestElementId(e.target), EVENTS.WHEEL, e);
            });

            root.addEventListener('mousedown', function (e) {
                if (shouldIgnoreMouseEvent(e)) return;

                e.isHandled = true;
                let id = (_mouseCapture === null || e.target === _mouseCapture) ? getClosestElementId(e.target) : '';
                switch (e.button) {
                    case 0:
                        callback(id, EVENTS.MOUSE_LEFT_DOWN, e);
                        break;
                    case 2:
                        callback(id, EVENTS.MOUSE_RIGHT_DOWN, e);
                        break;
                }
            });

            root.addEventListener('mouseup', function (e) {
                if (shouldIgnoreMouseEvent(e)) return;

                e.isHandled = true;
                const target = _mouseCapture || e.target;
                switch (e.button) {
                    case 0:
                        callback(getClosestElementId(target), EVENTS.MOUSE_LEFT_UP, e);
                        break;
                    case 2:
                        callback(getClosestElementId(target), EVENTS.MOUSE_RIGHT_UP, e);
                        break;
                }
            });
        },
        addListeners: function (element, isFocusable) {
            const view = typeof element === 'string' ? document.getElementById(element) : element;
            if (!view) return;

            view.addEventListener('mouseenter', function (e) {
                if (_mouseCapture === null || this === _mouseCapture) {
                    callback(getClosestElementId(this), EVENTS.MOUSE_ENTER, e);
                }
            });

            view.addEventListener('mouseleave', function (e) {
                if (_mouseCapture === null || this === _mouseCapture) {
                    callback(getClosestElementId(this), EVENTS.MOUSE_LEAVE, e);
                }
            });

            if (isTouchDevice()) {
                view.addEventListener('touchstart', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        callback(getClosestElementId(this), EVENTS.TOUCH_START, e);
                    }
                }, { passive: true });

                view.addEventListener('touchend', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        callback(getClosestElementId(this), EVENTS.TOUCH_END, e);
                        _lastTouchEndTimeStamp = e.timeStamp;
                    }
                });

                view.addEventListener('touchmove', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        callback(getClosestElementId(this), EVENTS.TOUCH_MOVE, e);
                    }
                }, { passive: true });
            }

            if (isFocusable) {
                view.addEventListener('keypress', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        callback(getClosestElementId(this), EVENTS.KEYPRESS, e);
                    }
                });

                view.addEventListener('input', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        callback(getClosestElementId(this), EVENTS.INPUT, e);
                    }
                });

                view.addEventListener('keydown', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        setModifiers(e);
                        callback(getClosestElementId(this), EVENTS.KEYDOWN, e);
                    }
                });

                view.addEventListener('keyup', function (e) {
                    if (!e.isHandled) {
                        e.isHandled = true;
                        setModifiers(e);
                        callback(getClosestElementId(this), EVENTS.KEYUP, e);
                    }
                });
            }
        },
        getModifiers: function () {
            return _modifiers;
        },
        captureMouse: function (element) {
            _mouseCapture = element;
        },
        releaseMouseCapture: function () {
            _mouseCapture = null;
        },
        suppressContextMenu: function (value) {
            _suppressContextMenu = value;
        },
    };
};

document.eventCallback = function (callbackId, args, sync) {
    const argsArray = args;
    const idWhereCallbackArgsAreStored = "callback_args_" + document.callbackCounterForSimulator++;
    document.jsObjRef[idWhereCallbackArgsAreStored] = argsArray;
    if (sync) {
        const v = window.onCallBack.OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, argsArray, true);
        delete document.jsObjRef[idWhereCallbackArgsAreStored];
        return v;
    } else {
        setTimeout(
            function () {
                window.onCallBack.OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, argsArray, false);
            }, 1);
    }
}

document.getCallbackFunc = function (callbackId, sync, sliceArguments) {
    return function () {
        return document.eventCallback(callbackId,
            (sliceArguments) ? Array.prototype.slice.call(arguments) : arguments,
            sync);
    };
}

document.callScriptSafe = function (referenceId, javaScriptToExecute, errorCallBackId) {
    try {
        document.jsObjRef[referenceId] = eval(javaScriptToExecute);
        return document.jsObjRef[referenceId];
    } catch (error) {
        document.errorCallback(error, errorCallBackId);
    }
}

document.errorCallback = function (error, IndexOfNextUnmodifiedJSCallInList) {
    const idWhereErrorCallbackArgsAreStored = "callback_args_" + document.callbackCounterForSimulator++;
    const argsArr = [];
    argsArr[0] = error.message;
    argsArr[1] = IndexOfNextUnmodifiedJSCallInList;
    document.jsObjRef[idWhereErrorCallbackArgsAreStored] = argsArr;
    window.onCallBack.OnCallbackFromJavaScriptError(idWhereErrorCallbackArgsAreStored);
}

document.setVisualBounds = function (id, left, top, width, height, bSetAbsolutePosition, bSetZeroMargin, bSetZeroPadding) {
    var element = document.getElementById(id);
    if (element) {
        element.style.left = left + "px";
        element.style.top = top + "px";
        element.style.width = width + "px";
        element.style.height = height + "px";

        if (bSetAbsolutePosition) {
            element.style.position = "absolute";
        }
        if (bSetZeroMargin) {
            element.style.margin = "0";
        }
        if (bSetZeroPadding) {
            element.style.padding = "0";
        }
    }
}

document.setPosition = function (id, left, top, bSetAbsolutePosition, bSetZeroMargin, bSetZeroPadding) {
    var element = document.getElementById(id);
    if (element) {
        element.style.left = left + "px";
        element.style.top = top + "px";

        if (bSetAbsolutePosition) {
            element.style.position = "absolute";
        }
        if (bSetZeroMargin) {
            element.style.margin = "0";
        }
        if (bSetZeroPadding) {
            element.style.padding = "0";
        }
    }
}

document.measureTextBlock = function (measureElementId, uid, whiteSpace, overflowWrap, padding, maxWidth, emptyVal) {
    var element = document.getElementById(measureElementId);
    var elToMeasure = document.getElementById(uid);
    if (element && elToMeasure) {
        var computedStyle = getComputedStyle(elToMeasure);

        element.innerHTML = elToMeasure.innerHTML.length == 0 ? emptyVal : elToMeasure.innerHTML;

        element.style.fontSize = computedStyle.fontSize;
        element.style.fontWeight = computedStyle.fontWeight;
        element.style.fontFamily = computedStyle.fontFamily;
        element.style.fontStyle = computedStyle.fontStyle;

        if (whiteSpace.length > 0)
            element.style.whiteSpace = whiteSpace;
        element.style.overflowWrap = overflowWrap;
        if (padding.length > 0) {
            element.style.boxSizing = "border-box";
            element.style.padding = padding;
        }

        element.style.maxWidth = maxWidth;

        const size = element.offsetWidth + "|" + element.offsetHeight;

        element.innerHTML = '';

        return size;
    }

    return "0|0";
}

document.setContentString = function (id, text, removeTextWrapping) {
    var el = document.getElementById(id);
    if (el) {
        el.innerText = text;
        if (removeTextWrapping)
            el.style.whiteSpace = "nowrap";
    };
}

window.ViewInteropErrors = function () {
    for (var key in document.interopErrors) {
        console.log(`Unable to find element with id '${key}' (${document.interopErrors[key]} time(s)).`);
    }
}

window.ViewProfilerResults = function () {
    if (Object.keys(document.performanceCounters).length > 0) {
        var sortedPerformanceCountersNames = [];
        for (var name in document.performanceCounters) {
            sortedPerformanceCountersNames[sortedPerformanceCountersNames.length] = name;
        }
        sortedPerformanceCountersNames.sort();
        var i;
        for (i = 0; i < sortedPerformanceCountersNames.length; i++) {
            var name = sortedPerformanceCountersNames[i];
            var counter = document.performanceCounters[name];
            console.log('=== ' + name + ' ===');
            console.log('Total time: ' + counter.time + 'ms');
            console.log('Number of calls: ' + counter.count);
            if (counter.count > 0)
                console.log('Average time per call: ' + (counter.time / counter.count) + 'ms');
            console.log('');
        }
        console.log('### RESULTS IN CSV FORMAT: ###');
        var s = 'Description,Total time in ms, Number of calls' + '\n';
        for (i = 0; i < sortedPerformanceCountersNames.length; i++) {
            var name = sortedPerformanceCountersNames[i];
            var counter = document.performanceCounters[name];
            s += name + ',' + counter.time + ',' + counter.count + '\n';
        }
        console.log(s);
    }
}

//todo: see if this does not break something else that would define String.endsWith in a better way.
var defineStringEndsWith = function () {
    if (!String.prototype.endsWith) { //IE doesn't know string.endsWith so we add it:
        String.prototype.endsWith = function (search, this_len) {
            if (this_len === undefined || this_len > this.length) {
                this_len = this.length;
            }
            return this.substring(this_len - search.length, this_len) === search;
        };
    }
}
defineStringEndsWith();

//gets the text inside a textArea as it actually is (domElement.innerText returns an incorrect result).
getTextAreaInnerText = function (domElement, forceNewLineFirst) {
    //logic here:   - br prepares a new line and we only add it to the text if there is more to add afterwards.
    //              - text means text
    //              - div want their own line so new line before if none, new line after.
    //                  no new line before if first element, no new line after if last element.
    var resultString = "";
    var currentNode = domElement.childNodes[0];
    while (currentNode != undefined) {
        if (currentNode.nodeType == Node.TEXT_NODE) {
            if (currentNode.textContent != "") { //Note: we added this test because IE sometimes adds an empty text node which should not count for anything as far as I know.
                if (forceNewLineFirst) {
                    resultString += "\n";
                }
                var textToAdd = currentNode.textContent;
                if (textToAdd.endsWith("\n")) {
                    //We need this because Edge sometimes decides to add the newline '\n' in the Text instead of adding a dom element (when pressing enter at the end of a line?) and
                    //  when it does this, it adds an additional '\n' because why not... So we remove that additional one.
                    //Note: It adds a total of one '\n' so if you go to the end of the TextArea and press enter five times, there will be six '\n' in the TextNode.
                    if (currentNode.nextSibling == undefined) {
                        //Only if currentNode.nextSibling is undefined, because apparently, it only adds one '\n' too much when it is the last node.
                        textToAdd = textToAdd.substring(0, textToAdd.length - 1);
                    }
                    //We also replace the '\n' with "\r\n":
                    textToAdd = textToAdd.replace(new RegExp("\n", 'g'), "\n");
                }
                resultString += textToAdd;
                forceNewLineFirst = false;
            }
        }
        else {
            var nodeName = currentNode.nodeName;
            if (nodeName == "BR") {
                if (forceNewLineFirst) {
                    resultString += "\n";
                }
                forceNewLineFirst = true;
            }
            else //we consider it's a <div> or a <p>:
            {
                if (forceNewLineFirst) {
                    resultString += "\n";
                    forceNewLineFirst = false;
                }
                if (currentNode.previousSibling != undefined && !resultString.endsWith("\n")) {
                    //The element is not the first in its parent and there is no new line to put it, so we add one:
                    resultString += "\n";
                }

                resultString += getTextAreaInnerText(currentNode, forceNewLineFirst);
                if (currentNode.nextSibling != undefined) {
                    //the element is not the last in its parent and there is no new line to put the following so we add one:
                    forceNewLineFirst = true;
                    //resultString += "\r\n";
                }
            }
        }
        currentNode = currentNode.nextSibling;
    }
    return resultString;
}

document.getTextBoxSelection = (function () {
    //this counts the amount of characters before the ones (start and end) defined by the range object.
    // it does so by defining:
    //      -globalIndexes.startIndex and globalIndexes.endIndex: the indexes as in c# of the positions defined by the range
    //      -globalIndexes.isStartFound and globalIndexes.isEndFound: a boolean stating whether the index has been found yet ot not (used within this method to know when to stop changing the indexes).
    function getRangeGlobalStartAndEndIndexes(currentParent, isFirstChild, charactersWentThrough, selection, range, globalIndexes) {
        //we go down the tree until we find multiple children or until there is no children left:
        while (currentParent.hasChildNodes()) {
            //a div/p/br tag means a new line if it is not the first child of its parent:
            if (!isFirstChild && (currentParent.tagName == 'DIV' || currentParent.tagName == 'P' || currentParent.tagName == 'BR')) {
                charactersWentThrough += 1; //corresponds to counting the characters for the new line (that are not otherwise included in the count)
                isFirstChild = true;
            }
            //we stop as soon as we find an element that has more than one childNode, so we can go through all the children afterwards.
            if (currentParent.childNodes.length > 1) {
                break;
            }
            currentParent = currentParent.childNodes[0];
        }
        if (currentParent.hasChildNodes()) { //this being true means that we stopped in the previous loop because it had multiple children. We need to go through them for the count of characters:
            var i = 0;
            var amountOfChildren = currentParent.childNodes.length;
            //recursively go through the children:
            for (i = 0; i < amountOfChildren; ++i) {
                var temp = currentParent.childNodes[i];
                charactersWentThrough = getRangeGlobalStartAndEndIndexes(temp, i == 0, charactersWentThrough, selection, range, globalIndexes);
                if (globalIndexes.isCaretFound && globalIndexes.isStartFound && globalIndexes.isEndFound) {
                    break;
                }
            }
        } else {
            //we stopped because we are at the end of a branch in the tree view --> count the characters in the line:
            //if the end of the branch is a new line, count it:
            if (!isFirstChild && (currentParent.tagName == 'DIV' || currentParent.tagName == 'P' || currentParent.tagName == 'BR')) {
                charactersWentThrough += 1;
            }
            else {
                if (currentParent.length) {
                    //we get the basic informations about the text:
                    var textContent = currentParent.textContent;
                    var wholeLength = textContent.length;
                    if (currentParent === selection.focusNode) {
                        globalIndexes.caretIndex = charactersWentThrough + selection.focusOffset;
                        globalIndexes.isCaretFound = true;
                    }
                    if (currentParent === range.startContainer) {
                        globalIndexes.startIndex = charactersWentThrough + range.startOffset;
                        globalIndexes.isStartFound = true;
                    }
                    if (currentParent === range.endContainer) {
                        globalIndexes.endIndex = charactersWentThrough + range.endOffset;
                        globalIndexes.isEndFound = true;
                    }
                    charactersWentThrough += wholeLength; //move forward in the count of characters.
                } else {
                    if (currentParent.tagName === 'BR') {
                        if (currentParent.parentElement === selection.focusNode) {
                            globalIndexes.caretIndex = charactersWentThrough;
                            globalIndexes.isCaretFound = true;
                        }
                        if (currentParent.parentElement === range.startContainer) {
                            globalIndexes.startIndex = charactersWentThrough;
                            globalIndexes.isStartFound = true;
                        }
                        if (currentParent.parentElement === range.endContainer) {
                            globalIndexes.endIndex = charactersWentThrough;
                            globalIndexes.isEndFound = true;
                        }
                    }
                }
            }
            return charactersWentThrough;
        }
    }

    return function (element) {
        const sel = window.getSelection();
        const gi = {};
        if (sel.rangeCount === 0) {
            gi.caretIndex = 0;
            gi.startIndex = 0;
            gi.endIndex = 0;
        } else {
            getRangeGlobalStartAndEndIndexes(element, true, 0, sel, sel.getRangeAt(0), gi);
        }
        return JSON.stringify(gi);
    }
})();

//this method goes through every branch of the visual tree starting from the given element and counts the characters until the given indexes are met.
//It then fills nodesAndOffsets with the nodes and offsets to apply on the range defined in the calling method.
document.getRangeStartAndEnd = function getRangeStartAndEnd(currentParent, isFirstChild, charactersWentThrough, startLimitIndex, endLimitIndex, nodesAndOffsets, isStartFound, isEndFound) {
    //we go down the tree until we find multiple children or until there is no children left:
    while (currentParent.hasChildNodes()) {
        //a div/p/br tag means a new line if it is not the first child of its parent:
        if (!isFirstChild && (currentParent.tagName == 'DIV' || currentParent.tagName == 'P' || currentParent.tagName == 'BR')) {
            charactersWentThrough += 2;
            isFirstChild = true;
        }
        if (currentParent.childNodes.length > 1) {
            break;
        }
        currentParent = currentParent.childNodes[0];
    }
    if (currentParent.hasChildNodes()) {
        //this being true means that the currentParent has multiple children through which we need to go to count the characters.
        //We therefore recursively call this same method on them and update the count of characters went through:
        var i = 0;
        var amountOfChildren = currentParent.childNodes.length;

        for (i = 0; i < amountOfChildren; ++i) {
            var temp = currentParent.childNodes[i];
            charactersWentThrough = getRangeStartAndEnd(temp, i == 0, charactersWentThrough, startLimitIndex, endLimitIndex, nodesAndOffsets, isStartFound, isEndFound);
            if ((!(temp.tagName == 'BR')) && charactersWentThrough >= startLimitIndex) {
                isStartFound = true;
            }
            if ((!(temp.tagName == 'BR')) && charactersWentThrough >= endLimitIndex) {
                isEndFound = true;
            }
            if (isStartFound && isEndFound) {
                break;
            }
        }
    }
    else {
        //handle new lines at the end of the branches:
        if (!isFirstChild && (currentParent.tagName == 'DIV' || currentParent.tagName == 'P' || currentParent.tagName == 'BR')) {
            charactersWentThrough += 2;
        }
        else {
            if (currentParent.length) {
                var textContent = currentParent.textContent;
                var splittedText = textContent.split("\n");
                var wholeLength = currentParent.length; //this will be the length of the whole text in this dom element, including possible compensation for the amount of characters used for a new line.
                var newLineCompensation = 0;
                if (splittedText.length > 1) {
                    var firstText = splittedText[0];
                    if (firstText[firstText.length - 1] != "\r") {
                        wholeLength += splittedText.length - 1; //for n lines, n-1 new lines
                        newLineCompensation = 1; //if newLine can be different than 0 or 1, change the places where commented to do so.
                    }
                }
                if (!isStartFound) {
                    if (charactersWentThrough + wholeLength > startLimitIndex) { //read this as "if we reach the given index in the text of this dom element"
                        //here, we need the offset in the text, while compensating the possible new lines (basically, if the new line in the text is represented as only '\n', consider -1 on the offset to put in the range for each new line met before reaching the offset).
                        var startOffset;
                        if (newLineCompensation != 0) {
                            var i = 0;
                            var remainingOffset = startLimitIndex - charactersWentThrough; //this is the offset considering each new line as 2 characters.
                            var charactersWentThroughInThisLine = 0;
                            //the following loop's only purpose is to get the amount of new lines before the remaining offset.
                            for (i = 0; i < splittedText.length; ++i) {
                                if (charactersWentThroughInThisLine + splittedText[i].length >= remainingOffset) {
                                    break;
                                }
                                //advance through the text while including 2 characters for each new line to keep things constant with the definition of remainingOffset:
                                charactersWentThroughInThisLine += splittedText[i].length + 1 + newLineCompensation; //note: it's OK to not include the newLineCompensation in the "if" above since we would want to go to the next line anyway.
                            }
                            startOffset = remainingOffset - i; //line to change if newLineCompensation can be different than 1 (i * newLineCompensation).
                            if (charactersWentThroughInThisLine - remainingOffset == 1) {
                                ++startOffset; //this means we were between '\r' and '\n'
                            }
                        }
                        else {
                            //no need for compensation on the offset to put in the range since new lines are already 2 characters.
                            startOffset = startLimitIndex - charactersWentThrough;
                        }
                        if (startOffset < 0) {
                            startOffset = 0; //case where the index lead to a position between '\r' and '\n' in a new line
                        }
                        nodesAndOffsets['startOffset'] = startOffset;
                    }
                    else {
                        nodesAndOffsets['startOffset'] = currentParent.length; //case where the index given is bigger than the length of the text.
                    }
                    nodesAndOffsets['startParent'] = currentParent;
                }
                if (!isEndFound) {
                    if (charactersWentThrough + wholeLength > endLimitIndex) {
                        var endOffset;
                        if (newLineCompensation != 0) {
                            var i = 0;
                            var remainingOffset = endLimitIndex - charactersWentThrough; //this is the offset considering each new line as 2 characters.
                            var charactersWentThroughInThisLine = 0;
                            for (i = 0; i < splittedText.length; ++i) {
                                if (charactersWentThroughInThisLine + splittedText[i].length >= remainingOffset) {
                                    break;
                                }
                                charactersWentThroughInThisLine += splittedText[i].length + 1 + newLineCompensation; //note: it's OK to not include the newLineCompensation in the "if" above since we would want to go to the next line anyway.
                            }
                            endOffset = remainingOffset - i; //line to change if newLineCompensation can be different than 1 (i * newLineCompensation).
                            if (charactersWentThroughInThisLine - remainingOffset == 1) {
                                ++endOffset; //this means we were between '\r' and '\n'
                            }
                        }
                        else {
                            endOffset = endLimitIndex - charactersWentThrough;
                        }
                        if (endOffset < 0) {
                            endOffset = 0;
                        }
                        nodesAndOffsets['endOffset'] = endOffset;
                    }
                    else {
                        nodesAndOffsets['endOffset'] = currentParent.length; //case where the index given is bigger than the length of the text.
                    }
                    nodesAndOffsets['endParent'] = currentParent;
                }
                return charactersWentThrough + wholeLength;
            }
            else {
                //case where the element is basically empty
                nodesAndOffsets['startParent'] = currentParent;
                nodesAndOffsets['startOffset'] = 0;
                nodesAndOffsets['endParent'] = currentParent;
                nodesAndOffsets['endOffset'] = 0;
            }
        }
    }
    return charactersWentThrough;
}

document.doesElementInheritDisplayNone = function getRangeStartAndEnd(domElement) {
    // This method will check if the element or one of its ancestors has "display:none".
    while (domElement && domElement.style) {
        if (domElement.style.display == 'none')
            return true;
        domElement = domElement.parentNode;
    }
    return false;
}

document.checkForDivsThatAbsorbEvents = function checkForDivsThatAbsorbEvents(jsEventArgs) {
    var currentElement = jsEventArgs.target;
    var endElement = jsEventArgs.currentTarget;
    while (currentElement && currentElement != endElement) {
        if (currentElement["data-absorb-events"])
            return true;
        currentElement = currentElement.parentNode;
    }
    return false;
}

document.getTextLengthIncludingNewLineCompensation = function (instance) {
    var cshtml5Asm;
    if (document.isSLMigration)
        cshtml5Asm = JSIL.GetAssembly('SLMigration.CSharpXamlForHtml5');
    else
        cshtml5Asm = JSIL.GetAssembly('CSharpXamlForHtml5');
    var htmlDomManager = function () {
        return (htmlDomManager = JSIL.Memoize(cshtml5Asm.CSHTML5.Internal.INTERNAL_HtmlDomManager))();
    };
    var text = htmlDomManager()['GetTextBoxText'](instance);
    if (!instance['get_AcceptsReturn']()) {
        text = (System.String.Replace(System.String.Replace(text, "\n", ""), "\r", ""));
    }
    var correctionDueToNewLines = text.split("\n").length;
    --correctionDueToNewLines; //for n lines, we have n-1 ""\r\n""
    if (window.chrome && correctionDueToNewLines != 0) {
        --correctionDueToNewLines; //on chrome, we have a \n right at the end for some reason.
    }
    else if (window.IE_VERSION) {
        correctionDueToNewLines *= 2; //IE already has 2 characters for new lines but they are doubled: we have ""\r\n\r\n"" instead of ""\r\n"".
    }
    return text.length + correctionDueToNewLines;
}

document.functionToCompareWordForFilter = function (wordToCompare) {
    //this function is used in XContainer.Elements(XName) so that we can give the filter a method that knows the name to compare to.
    return function (node) {
        return node.tagName == wordToCompare;
    }
}

function callScriptableObjectEvent(scriptableObjectName, eventName, passedArgs) {
    var scriptableObj = window[scriptableObjectName];
    if (scriptableObj && scriptableObj[eventName]) {
        scriptableObj[eventName].apply(scriptableObj, passedArgs);
    }
}

//------------------------------
// SCRDOC POLYFILL (cf. https://github.com/jugglinmike/srcdoc-polyfill )
//------------------------------

/*! srcdoc-polyfill - v0.2.0 - 2015-10-02
* http://github.com/jugglinmike/srcdoc-polyfill/
* Copyright (c) 2015 Mike Pennisi; Licensed MIT */
!function (a, b) { var c = window.srcDoc; "function" == typeof define && define.amd ? define(["exports"], function (d) { b(d, c), a.srcDoc = d }) : "object" == typeof exports ? b(exports, c) : (a.srcDoc = {}, b(a.srcDoc, c)) }(this, function (a, b) { var c, d, e = !!("srcdoc" in document.createElement("iframe")), f = { compliant: function (a, b) { b && a.setAttribute("srcdoc", b) }, legacy: function (a, b) { var c; a && a.getAttribute && (b ? a.setAttribute("srcdoc", b) : b = a.getAttribute("srcdoc"), b && (c = "javascript: window.frameElement.getAttribute('srcdoc');", a.setAttribute("src", c), a.contentWindow && (a.contentWindow.location = c))) } }, g = a; if (g.set = f.compliant, g.noConflict = function () { return window.srcDoc = b, g }, !e) for (g.set = f.legacy, d = document.getElementsByTagName("iframe"), c = d.length; c--;) g.set(d[c]) });


//------------------------------
// String.startsWith() POLYFILL (IE11)
//------------------------------

if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.indexOf(searchString, position) === position;
    };
}


//------------------------------
// Array.from POLYFILL (cf. https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/from )
//------------------------------

// Production steps of ECMA-262, Edition 6, 22.1.2.1
if (!Array.from) {
    Array.from = (function () {
        var toStr = Object.prototype.toString;
        var isCallable = function (fn) {
            return typeof fn === 'function' || toStr.call(fn) === '[object Function]';
        };
        var toInteger = function (value) {
            var number = Number(value);
            if (isNaN(number)) { return 0; }
            if (number === 0 || !isFinite(number)) { return number; }
            return (number > 0 ? 1 : -1) * Math.floor(Math.abs(number));
        };
        var maxSafeInteger = Math.pow(2, 53) - 1;
        var toLength = function (value) {
            var len = toInteger(value);
            return Math.min(Math.max(len, 0), maxSafeInteger);
        };

        // The length property of the from method is 1.
        return function from(arrayLike/*, mapFn, thisArg */) {
            // 1. Let C be the this value.
            var C = this;

            // 2. Let items be ToObject(arrayLike).
            var items = Object(arrayLike);

            // 3. ReturnIfAbrupt(items).
            if (arrayLike == null) {
                throw new TypeError('Array.from requires an array-like object - not null or undefined');
            }

            // 4. If mapfn is undefined, then let mapping be false.
            var mapFn = arguments.length > 1 ? arguments[1] : void undefined;
            var T;
            if (typeof mapFn !== 'undefined') {
                // 5. else
                // 5. a If IsCallable(mapfn) is false, throw a TypeError exception.
                if (!isCallable(mapFn)) {
                    throw new TypeError('Array.from: when provided, the second argument must be a function');
                }

                // 5. b. If thisArg was supplied, let T be thisArg; else let T be undefined.
                if (arguments.length > 2) {
                    T = arguments[2];
                }
            }

            // 10. Let lenValue be Get(items, "length").
            // 11. Let len be ToLength(lenValue).
            var len = toLength(items.length);

            // 13. If IsConstructor(C) is true, then
            // 13. a. Let A be the result of calling the [[Construct]] internal method 
            // of C with an argument list containing the single item len.
            // 14. a. Else, Let A be ArrayCreate(len).
            var A = isCallable(C) ? Object(new C(len)) : new Array(len);

            // 16. Let k be 0.
            var k = 0;
            // 17. Repeat, while k < len (also steps a - h)
            var kValue;
            while (k < len) {
                kValue = items[k];
                if (mapFn) {
                    A[k] = typeof T === 'undefined' ? mapFn(kValue, k) : mapFn.call(T, kValue, k);
                } else {
                    A[k] = kValue;
                }
                k += 1;
            }
            // 18. Let putStatus be Put(A, "length", len, true).
            A.length = len;
            // 20. Return A.
            return A;
        };
    }());
}


//------------------------------
// PRETTIFY XML (subset of fork of vkBeautify on 2018.09.12, at: https://github.com/nsoms/vkBeautify/blob/master/vkbeautify.js
// further patched with pull request by bbpennel at: https://github.com/vkiryukhin/vkBeautify/pull/5 )
// See license and copyright at the URL above.
//------------------------------

(function () {

    function createShiftArr(step) {

        var space = '    ';

        if (isNaN(parseInt(step))) {  // argument is string
            space = step;
        } else { // argument is integer
            switch (step) {
                case 1: space = ' '; break;
                case 2: space = '  '; break;
                case 3: space = '   '; break;
                case 4: space = '    '; break;
                case 5: space = '     '; break;
                case 6: space = '      '; break;
                case 7: space = '       '; break;
                case 8: space = '        '; break;
                case 9: space = '         '; break;
                case 10: space = '          '; break;
                case 11: space = '           '; break;
                case 12: space = '            '; break;
            }
        }

        var shift = ['\n']; // array of shifts
        for (var ix = 0; ix < 100; ix++) {
            shift.push(shift[ix] + space);
        }
        return shift;
    }

    function vkbeautify() {
        this.step = '    '; // 4 spaces
        this.shift = createShiftArr(this.step);
    }

    vkbeautify.prototype.xml = function (text, step) {

        var ar = text.replace(/>\s*</g, "><")
            .replace(/</g, "~::~<")
            .replace(/\s*xmlns:/g, "~::~xmlns:")
            .replace(/\s*xmlns=/g, "~::~xmlns=")
            .split('~::~'),
            len = ar.length,
            inComment = false,
            deep = 0,
            str = '',
            ix,
            shift = step ? createShiftArr(step) : this.shift,
            withNamespace = 0;

        for (ix = 0; ix < len; ix++) {
            // start comment or <![CDATA[...]]> or <!DOCTYPE //
            if (ar[ix].search(/<!/) > -1) {
                str += shift[deep] + ar[ix];
                inComment = true;
                // end comment  or <![CDATA[...]]> //
                if (ar[ix].search(/-->/) > -1 || ar[ix].search(/]>/) > -1 || ar[ix].search(/!DOCTYPE/) > -1) {
                    inComment = false;
                }
            } else
                // end comment  or <![CDATA[...]]> //
                if (ar[ix].search(/-->/) > -1 || ar[ix].search(/]>/) > -1) {
                    str += ar[ix];
                    inComment = false;
                } else
                    // <elm></elm> //
                    if (/^<\w/.exec(ar[ix - 1]) && /^<\/\w/.exec(ar[ix]) &&
                        // This comparison will eventually compare an array with a single string item to another string
                        // so we voluntarily use '=='
                        /^<[\w:\-\.,]+/.exec(ar[ix - 1]) == /^<\/[\w:\-\.,]+/.exec(ar[ix])[0].replace('/', '')) { // jshint ignore:line
                        str += ar[ix];
                        if (!inComment) {
                            deep--;
                        }
                    } else
                        // <elm> //
                        if (ar[ix].search(/<\w/) > -1 && ar[ix].search(/<\//) === -1 && ar[ix].search(/\/>/) === -1) {
                            str = !inComment ? str += shift[deep++] + ar[ix] : str += ar[ix];
                        } else
                            // <elm>...</elm> //
                            if (ar[ix].search(/<\w/) > -1 && ar[ix].search(/<\//) > -1) {
                                str = !inComment ? str += shift[deep] + ar[ix] : str += ar[ix];
                            } else
                                // </elm> //
                                if (ar[ix].search(/<\//) > -1) {
                                    --deep;
                                    str = !inComment && !withNamespace ? str += shift[deep] + ar[ix] : str += ar[ix];
                                } else
                                    // <elm/> //
                                    if (ar[ix].search(/\/>/) > -1) {
                                        str = !inComment ? str += shift[deep] + ar[ix] : str += ar[ix];
                                        if (ar[ix].search(/xmlns\:/) > -1 || ar[ix].search(/xmlns\=/) > -1)
                                            deep--;
                                    } else
                                        // <? xml ... ?> //
                                        if (ar[ix].search(/<\?/) > -1) {
                                            str += shift[deep] + ar[ix];
                                        } else
                                            // xmlns //
                                            if (ar[ix].search(/xmlns:/) > -1 || ar[ix].search(/xmlns=/) > -1) {
                                                str += shift[deep] + ar[ix];
                                                withNamespace = 2;
                                            }
                                            else {
                                                str += ar[ix];
                                            }
            if (withNamespace)
                withNamespace--;
        }

        return (str.charAt(0) === '\n') ? str.slice(1) : str;
    };

    window.vkbeautify = new vkbeautify();

})();


//------------------------------
// INITIALIZE
//------------------------------

var jsilConfig = {
    printStackTrace: false,
    libraryRoot: "Libraries/",
    onLoadFailure: function (p, e) { alert(e); },
    onLoadFailed: function (e) { alert(e); },
    showProgressBar: true,
    localStorage: true,
    manifests: [
        "index"
    ]
};

window.elementsFromPointOpensilver = function (x, y, element) {
    if (!element) element = document.body;
    const elements = [];
    const walker = document.createTreeWalker(element, NodeFilter.SHOW_ELEMENT, null, false);
    let currentNode = walker.currentNode;
    while (currentNode) {
        const xamlid = currentNode.getAttribute('xamlid');
        if (xamlid && PerformHitTest(x, y, currentNode)) {
            elements.push(xamlid);
        }
        currentNode = walker.nextNode();
    }
    return JSON.stringify(elements.reverse());
};

function PerformHitTest(x, y, element) {
    const rect = element.getBoundingClientRect();
    return rect.x <= x && x <= rect.x + rect.width && rect.y <= y && y <= rect.y + rect.height;
}

//------------------------------
// Just to check if client browser support touch
//------------------------------
const isTouchDevice = () => {
    return (('ontouchstart' in window) ||
        (navigator.maxTouchPoints > 0) ||
        (navigator.msMaxTouchPoints > 0));
}

document.velocityHelpers = (function () {
    const cache = {};

    function addToCache(element, key) {
        const id = element.id;
        if (id in cache && !cache[id].includes(key)) {
            cache[id].push(key);
        } else {
            cache[id] = [element, key];
        }
    }

    function cleanupCache() {
        const keys = Object.keys(cache);
        keys.forEach((key) => {
            const el = document.getElementById(key);
            if (el === null) {
                Velocity.Utilities.removeData(cache[key][0], cache[key].slice(1));
                Velocity.Utilities.removeData(cache[key][0], ['velocity']);
                delete cache[key];
            }
        });
    }

    setInterval(() => { cleanupCache(); }, 10000);

    return {
        setDomStyle: function (element, properties, value) {
            const obj = {};
            for (const property of properties.split(',')) {
                obj[property] = value;
            }

            Velocity(element, obj, { duration: 1, queue: false });
            addToCache(element, 'velocity');
        },

        animate: function (element, fromToValues, options, groupName) {
            Velocity(element, fromToValues, options);
            Velocity.Utilities.dequeue(element, groupName);
            addToCache(element, `${groupName}queue`);
        }
    };
})();

document.browserService = (function () {
    const JSTYPE = {
        ERROR: -1,
        VOID: 0,
        STRING: 1,
        INTEGER: 2,
        DOUBLE: 3,
        BOOLEAN: 4,
        OBJECT: 5,
        HTMLELEMENT: 6,
        HTMLCOLLECTION: 7,
        HTMLDOCUMENT: 8,
        HTMLWINDOW: 9,
    };

    const INTEROP_RESULT = {
        ERROR: 0,
        VOID: 1,
        OBJECT: 2,
        MEMBER: 3,
    };

    let _id = 0;
    const _idToObj = new Map();
    const _objToId = new Map();

    let _isInitialized = false;
    let _getMemberCallback = null;
    let _setPropertyCallback = null;
    let _invokeMethodCallback = null;
    let _addEventListenerCallback = null;

    function checkInitialized() {
        if (!_isInitialized) {
            throw new Error('browserService has not been initialized yet.');
        }
    };

    function createManagedObject(id, isDelegate) {
        const o = isDelegate ? function () { } : {};
        o.id = id;
        Object.defineProperty(o, 'id', { writable: false });

        const handler = {
            get: function (target, prop, receiver) {
                switch (prop) {
                    case 'addEventListener':
                        return function (event, handler) {
                            const r = addDotNetEventListener(target, event, JSON.stringify(conv(handler)));
                            switch (r.type) {
                                case INTEROP_RESULT.ERROR:
                                    throw new Error(r.value);
                            }
                        };
                    case 'removeEventListener':
                        return function (event, handler) {
                            const r = removeDotNetEventListener(target, event, JSON.stringify(conv(handler)));
                            switch (r.type) {
                                case INTEROP_RESULT.ERROR:
                                    throw new Error(r.value);
                            }
                        };
                    default:
                        const r = getDotNetMember(target, prop);
                        switch (r.type) {
                            case INTEROP_RESULT.ERROR:
                                throw new Error(r.value);
                            case INTEROP_RESULT.OBJECT:
                                return r.value;
                            case INTEROP_RESULT.MEMBER:
                                return function (...args) {
                                    const r = invokeDotNetMethod(target, prop, JSON.stringify(args.map(function (x) { return conv(x); })));
                                    switch (r.type) {
                                        case INTEROP_RESULT.ERROR:
                                            throw new Error(r.value);
                                        case INTEROP_RESULT.OBJECT:
                                            return r.value;
                                    }
                                };
                        }
                }
            },
            set: function (target, prop, value, receiver) {
                const r = setDotNetProperty(target, prop, JSON.stringify(conv(value)));
                switch (r.type) {
                    case INTEROP_RESULT.ERROR:
                        throw new Error(r.value);
                }
                return true;
            },
        };

        if (isDelegate) {
            handler.apply = function (target, thisArg, argumentsList) {
                const r = invokeDotNetMethod(target, '', JSON.stringify(argumentsList.map(function (x) { return conv(x); })));
                switch (r.type) {
                    case INTEROP_RESULT.ERROR:
                        throw new Error(r.value);
                    case INTEROP_RESULT.OBJECT:
                        return r.value;
                }
            };
        }

        return new Proxy(o, handler);
    };

    function getDotNetMember(managedObject, name) {
        const str_result = _getMemberCallback(managedObject.id, name);
        return eval(str_result);
    };

    function invokeDotNetMethod(managedObject, name, args) {
        const str_result = _invokeMethodCallback(managedObject.id, name, args);
        return eval(str_result);
    };

    function setDotNetProperty(managedObject, name, value) {
        const str_result = _setPropertyCallback(managedObject.id, name, value);
        return eval(str_result);
    };

    function addDotNetEventListener(managedObject, event, handler) {
        const str_result = _addEventListenerCallback(managedObject.id, event, handler, true);
        return eval(str_result);
    };

    function removeDotNetEventListener(managedObject, event, handler) {
        const str_result = _addEventListenerCallback(managedObject.id, event, handler, false);
        return eval(str_result);
    };

    function getOrCreateId(obj) {
        if (!_objToId.has(obj)) {
            const id = (_id++).toString();
            _objToId.set(obj, id);
            _idToObj.set(id, obj);
        }

        return _objToId.get(obj);
    };

    function isDOMCollection(v) {
        return v instanceof HTMLCollection || v instanceof NodeList;
    };

    function conv(v) {
        if (v instanceof Document) {
            return { Type: JSTYPE.HTMLDOCUMENT };
        } else if (v instanceof Window) {
            return { Type: JSTYPE.HTMLWINDOW };
        } else if (v instanceof HTMLElement) {
            return { Type: JSTYPE.HTMLELEMENT, Value: getOrCreateId(v) };
        } else if (isDOMCollection(v)) {
            return { Type: JSTYPE.HTMLCOLLECTION, Value: getOrCreateId(v) };
        } else if (typeof v === 'number') {
            if (Number.isInteger(v))
                return { Type: JSTYPE.INTEGER, Value: v.toString() };
            else
                return { Type: JSTYPE.DOUBLE, Value: v.toString() };
        } else if (typeof v === 'string') {
            return { Type: JSTYPE.STRING, Value: v };
        } else if (typeof v === 'boolean') {
            return { Type: JSTYPE.BOOLEAN, Value: v.toString() };
        } else if (v === null || v === undefined) {
            return { Type: JSTYPE.VOID };
        } else if (typeof v === 'object' || typeof v === 'function') {
            return { Type: JSTYPE.OBJECT, Value: getOrCreateId(v) };
        } else {
            return { Type: JSTYPE.ERROR, Value: 'An unexpected error occurred' };
        }
    };

    function error(message) {
        return { Type: JSTYPE.ERROR, Value: message };
    };

    return {
        initialize: function (getMemberCallback, setPropertyCallback, invokeMethodCallback, addEventListenerCallback) {
            if (_isInitialized) {
                throw new Error('browserService can only be initialized once.');
            }
            _isInitialized = true;
            _getMemberCallback = getMemberCallback;
            _setPropertyCallback = setPropertyCallback;
            _invokeMethodCallback = invokeMethodCallback;
            _addEventListenerCallback = addEventListenerCallback;
        },
        invoke: function (instance, name, ...args) {
            checkInitialized();
            const m = instance[name];
            if (m) {
                try {
                    const r = m.call(instance, ...args);
                    return JSON.stringify(conv(r));
                } catch (err) {
                    return JSON.stringify(error(err.message));
                }
            } else {
                return JSON.stringify(error(`The method '${name}' is not defined.`));
            }
        },
        invokeSelf: function (f, ...args) {
            checkInitialized();
            if (typeof f === 'function') {
                try {
                    const r = f.call(null, ...args);
                    return JSON.stringify(conv(r));
                } catch (err) {
                    return JSON.stringify(error(err.message));
                }
            } else {
                return JSON.stringify(error("'InvokeSelf' can only be called on a 'function'."));
            }
        },
        getProperty: function (instance, name) {
            checkInitialized();
            try {
                return JSON.stringify(conv(instance[name]));
            } catch (err) {
                return JSON.stringify(error(err.message));
            }
        },
        setProperty: function (instance, name, value) {
            checkInitialized();
            try {
                instance[name] = value;
                return JSON.stringify(conv(undefined));
            } catch (err) {
                return JSON.stringify(error(err.message));
            }
        },
        getObject: function (id) {
            checkInitialized();
            return _idToObj.get(id);
        },
        releaseObject: function (id) {
            checkInitialized();
            if (_idToObj.has(id)) {
                const o = _idToObj.get(id);
                _objToId.delete(o);
                _idToObj.delete(id);
            }
        },
        registerManagedObject: function (isDelegate) {
            checkInitialized();
            const id = (_id++).toString();
            const managedObject = createManagedObject(id, isDelegate);
            _objToId.set(managedObject, id);
            _idToObj.set(id, managedObject);
            return id;
        },
    };
})();