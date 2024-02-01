
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

(function () {
    const userAgentLowercase = navigator.userAgent.toLowerCase();

    window.ANDROID_VERSION = (userAgentLowercase.indexOf('android') != -1) ? parseInt(userAgentLowercase.split('android')[1]) : false;
    window.FIREFOX_VERSION = ((index = userAgentLowercase.indexOf('firefox')) != -1) ? parseInt(userAgentLowercase.substring(index + 8)) : false;

    // Current version does not support Android < 4:
    if (window.ANDROID_VERSION && window.ANDROID_VERSION < 4)
        alert("This version of Android is not supported yet. Please use Android 4.x (or newer), Internet Explorer 11 (or newer), Chrome 35 (or newer), Firefox 27 (or newer), Safari 8 (or newer), Safari Mobile iOS 8 (or newer), or Opera 24 (or newer). More browsers will be supported in the future.");
})();

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
};

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
};

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
};

document.createElementSafe = function (tagName, id, parent, index) {
    if (typeof parent === 'string') parent = document.getElementById(parent);
    if (parent == null) return null;

    const element = document.createElement(tagName);

    element.setAttribute('id', id);
    Object.defineProperty(element, 'xamlid', {
        value: id,
        writable: false,
    });

    if (index < 0 || index >= parent.children.length) {
        parent.appendChild(element);
    } else {
        parent.insertBefore(element, parent.children[index]);
    }

    Object.defineProperty(element, 'dump', {
        get() { return document.dumpProperties(id); }
    });

    return element;
};

document.createLayoutElement = function (tagName, id, parent, index) {
    const element = document.createElementSafe(tagName, id, parent, index);
    if (element) element.classList.add('uielement-unarranged');
    return element;
};

document.dumpProperties = function (id, ...names) {
    if (DotNet && DotNet.invokeMethod) {
        return DotNet.invokeMethod('OpenSilver', 'DumpProperties', id, names);
    }
    return null;
};

document.createTextBlockElement = function (id, parent, wrap) {
    const element = document.createLayoutElement('div', id, parent, -1);
    if (element) {
        element.style.overflow = 'hidden';
        element.style.textAlign = 'start';
        element.style.boxSizing = 'border-box';
        if (wrap) {
            element.style.overflowWrap = 'break-word';
            element.style.whiteSpace = 'pre-wrap';
        } else {
            element.style.whiteSpace = 'pre';
        }
    }
};

document.createBorderElement = function (id, parent) {
    const element = document.createLayoutElement('div', id, parent, -1);
    if (element) {
        element.classList.add('opensilver-border');
    }
};

document.createPopupRootElement = function (id, rootElement, pointerEvents) {
    if (!rootElement) return;

    const popupRoot = document.createElement('div');
    popupRoot.setAttribute('id', id);
    Object.defineProperty(popupRoot, 'xamlid', {
        value: id,
        writable: false,
    });
    popupRoot.style.position = 'absolute';
    popupRoot.style.width = '100%';
    popupRoot.style.height = '100%';
    popupRoot.style.overflow = 'clip';
    popupRoot.style.pointerEvents = pointerEvents;
    rootElement.appendChild(popupRoot);
};

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
};

document.createTextElement = function (id, tagName, parent) {
    if (typeof parent === 'string') parent = document.getElementById(parent);
    if (parent === null) return null;

    const textElement = document.createElement(tagName);
    textElement.setAttribute('id', id);

    parent.appendChild(textElement);
};

document.createShapeElement = function (svgId, shapeId, defsId, svgTagName, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg.classList.add('uielement-shape');
    svg.classList.add('uielement-unarranged');
    svg.setAttribute('id', svgId);
    Object.defineProperty(svg, 'xamlid', {
        value: svgId,
        writable: false,
    });
    Object.defineProperty(svg, 'dump', {
        get() { return document.dumpProperties(svgId); }
    });
    const shape = document.createElementNS('http://www.w3.org/2000/svg', svgTagName);
    shape.setAttribute('id', shapeId);
    shape.setAttribute('vector-effect', 'non-scaling-stroke');
    Object.defineProperty(shape, 'xamlid', {
        value: shapeId,
        writable: false,
    });
    svg.appendChild(shape);
    const defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
    defs.setAttribute('id', defsId);
    svg.appendChild(defs);
    parent.appendChild(svg);
};

document.createSvgElement = function (id, tagName, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const svg = document.createElementNS('http://www.w3.org/2000/svg', tagName);
    svg.setAttribute('id', id);

    parent.appendChild(svg);
};

document.getBBox = function (svgElement) {
    if (svgElement) {
        const bbox = svgElement.getBBox();
        return JSON.stringify({ X: bbox.x, Y: bbox.y, Width: bbox.width, Height: bbox.height, });
    }
    return '{}';
};

document.set2dContextProperty = function (id, propertyName, propertyValue) {
    const element = document.getElementById(id);
    if (!element || element.tagName !== 'CANVAS')
        return;

    element.getContext('2d')[propertyName] = propertyValue;
};

document.invoke2dContextMethod = function (id, methodName, args) {
    const element = document.getElementById(id);
    if (!element || element.tagName !== 'CANVAS')
        return undefined;
    return CanvasRenderingContext2D.prototype[methodName].apply(element.getContext('2d'),
        args.split(',')
            .map(Function.prototype.call, String.prototype.trim)
            .filter(i => i.length > 0));
};

document.setDomStyle = function (id, propertyName, value) {
    const element = document.getElementById(id);
    if (!element)
        return;

    element.style[propertyName] = value;
};

document.setStyleProperty = function (id, propertyName, value, priority) {
    const element = document.getElementById(id);
    if (element) {
        element.style.setProperty(propertyName, value, priority);
    }
};

document.setDomAttribute = function (id, propertyName, value) {
    const element = document.getElementById(id);
    if (!element)
        return;

    element.setAttribute(propertyName, value);
};

document.removeEventListenerSafe = function (element, method, func) {
    if (typeof element == 'string') {
        element = document.getElementById(element);
    }
    if (element) {
        element.removeEventListener(method, func);
    }
};

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
        KEYPRESS: 10,
        TOUCH_START: 11,
        TOUCH_END: 12,
        TOUCH_MOVE: 13,
        FOCUS_MANAGED: 14,
        FOCUS_UNMANAGED: 15,
        WINDOW_FOCUS: 16,
        WINDOW_BLUR: 17,
    };

    const MODIFIERKEYS = {
        NONE: 0,
        CONTROL: 1,
        ALT: 2,
        SHIFT: 4,
        WINDOWS: 8,
    };

    const FocusManager = (function () {
        let _timeoutID = null;
        let _isManagedFocusUpdate = false;

        function startTimer() {
            if (_timeoutID === null) {
                _timeoutID = setTimeout(function () {
                    _timeoutID = null;
                    callback('', EVENTS.FOCUS_MANAGED, null);
                });
            }
        };

        return {
            get isManagingFocus() {
                return _isManagedFocusUpdate;
            },
            focus: function (element) {
                if (!element) return false;

                element.setAttribute('tabindex', 0);

                _isManagedFocusUpdate = true;
                element.focus({ preventScroll: true });
                _isManagedFocusUpdate = false;

                if (document.activeElement === element) {
                    startTimer();
                    return true;
                }

                return false;
            },
        };
    })();

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
            const xamlid = element.xamlid;
            if (xamlid) {
                return xamlid;
            }

            element = element.parentElement;
        }

        return '';
    };

    function shouldIgnoreMouseEvent(e) {
        return e.timeStamp - _lastTouchEndTimeStamp < 500;
    };

    function isTouchDevice() {
        return (('ontouchstart' in window) ||
            (navigator.maxTouchPoints > 0) ||
            (navigator.msMaxTouchPoints > 0));
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
                switch (e.button) {
                    case 0:
                        callback(getClosestElementId(target), EVENTS.MOUSE_LEFT_UP, e);
                        break;
                    case 2:
                        callback(getClosestElementId(target), EVENTS.MOUSE_RIGHT_UP, e);
                        break;
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

        document.addEventListener('contextmenu', function (e) {
            if (_suppressContextMenu ||
                (_mouseCapture !== null && this !== _mouseCapture)) {
                _suppressContextMenu = false;
                e.preventDefault();
            }
        });

        document.addEventListener('keydown', function (e) { setModifiers(e); });

        document.addEventListener('keyup', function (e) { setModifiers(e); });        

        window.addEventListener('focus', function (e) { callback('', EVENTS.WINDOW_FOCUS, e); });

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

            root.addEventListener('focusin', function (e) {
                if (FocusManager.isManagingFocus) return;

                // Unrequested focus update, either from user interaction or call to focus()
                // method via interop or external javascript component.
                if (root._ignoreFocus) return;

                // Try to reconnect focused element to a known opensilver element
                const xamlid = getClosestElementId(e.target);
                if (xamlid) {
                    callback(xamlid, EVENTS.FOCUS_UNMANAGED, e);
                } else {
                    // Root element received focus. Check if previous focused element belongs to
                    // the app. If yes, then move focus here again silently.
                    if (getClosestElementId(e.relatedTarget)) {
                        root._ignoreFocus = true;
                        e.relatedTarget.focus({ preventScroll: true });
                        root._ignoreFocus = false;

                        // Make sure that re-focus was successful.
                        if (document.activeElement === e.relatedTarget) return;
                    }

                    callback('', EVENTS.FOCUS_UNMANAGED, e);
                }
            });

            root.addEventListener('mousemove', function (e) {
                if (shouldIgnoreMouseEvent(e)) return;

                e.isHandled = true;
                const target = _mouseCapture || e.target;
                callback(getClosestElementId(target), EVENTS.MOUSE_MOVE, e);
            });

            root.addEventListener('wheel', function (e) {
                // Zoom in/out request, takes priority over opensilver
                if (e.ctrlKey) return;
                e.isHandled = true;
                const target = _mouseCapture || e.target;
                callback(getClosestElementId(target), EVENTS.WHEEL, e);
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
            document.body.classList.add('opensilver-mouse-captured');
        },
        releaseMouseCapture: function () {
            _mouseCapture = null;
            document.body.classList.remove('opensilver-mouse-captured');
        },
        suppressContextMenu: function (value) {
            _suppressContextMenu = value;
        },
        focus: function (element) {
            return FocusManager.focus(element);
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
};

document.getCallbackFunc = function (callbackId, sync) {
    return function () {
        return document.eventCallback(callbackId,
            Array.prototype.slice.call(arguments),
            sync);
    };
};

document.callScriptSafe = function (referenceId, javaScriptToExecute, errorCallBackId) {
    try {
        document.jsObjRef[referenceId] = eval(javaScriptToExecute);
        return document.jsObjRef[referenceId];
    } catch (error) {
        document.errorCallback(error, errorCallBackId);
    }
};

document.errorCallback = function (error, IndexOfNextUnmodifiedJSCallInList) {
    const idWhereErrorCallbackArgsAreStored = "callback_args_" + document.callbackCounterForSimulator++;
    const argsArr = [];
    argsArr[0] = error.message;
    argsArr[1] = IndexOfNextUnmodifiedJSCallInList;
    document.jsObjRef[idWhereErrorCallbackArgsAreStored] = argsArr;
    window.onCallBack.OnCallbackFromJavaScriptError(idWhereErrorCallbackArgsAreStored);
};

document.setVisualBounds = function (id, left, top, width, height, clip, clipLeft, clipTop, clipRight, clipBottom) {
    const element = document.getElementById(id);
    if (element) {
        element.style.left = left + 'px';
        element.style.top = top + 'px';
        element.style.width = width + 'px';
        element.style.height = height + 'px';
        element.style.position = 'absolute';
        if (clip) {
            element.style.clip = `rect(${clipTop}px ${clipRight}px ${clipBottom}px ${clipLeft}px)`;
        } else {
            element.style.clip = '';
        }
        element.classList.remove('uielement-unarranged');
    }
};

document.createMeasurementService = function (parent) {
    if (!parent) return null;
    const measurer = document.createElement('div');
    measurer.id = `${parent.id}-msr`; 
    measurer.style.position = 'absolute';
    measurer.style.visibility = 'hidden';
    measurer.style.height = '';
    measurer.style.width = '';
    measurer.style.boxSizing = 'border-box';
    measurer.style.whiteSpace = 'pre';
    measurer.style.left = '-100000px';
    measurer.style.top = '-100000px';
    measurer.style.textAlign = 'left';
    parent.appendChild(measurer);
    return measurer.id;
};

document.measureTextBlock = function (measureElementId, uid, whiteSpace, overflowWrap, maxWidth, emptyVal) {
    const element = document.getElementById(measureElementId);
    const elToMeasure = document.getElementById(uid);
    if (element && elToMeasure) {
        if (elToMeasure instanceof HTMLTextAreaElement) {
            let text = elToMeasure.value.length == 0 ? emptyVal : elToMeasure.value;
            // if the text ends with a new line, we need to add one more or it will not be measured
            if (text.endsWith('\n')) text += '\n';
            element.textContent = text;
        } else {
            element.innerHTML = elToMeasure.innerHTML.length == 0 ? emptyVal : elToMeasure.innerHTML;
        }

        element.style.fontSize = elToMeasure.style.fontSize;
        element.style.fontWeight = elToMeasure.style.fontWeight;
        element.style.fontFamily = elToMeasure.style.fontFamily;
        element.style.fontStyle = elToMeasure.style.fontStyle;
        element.style.lineHeight = elToMeasure.style.lineHeight;
        element.style.letterSpacing = elToMeasure.style.letterSpacing;

        element.style.whiteSpace = whiteSpace;
        element.style.overflowWrap = overflowWrap;
        element.style.maxWidth = maxWidth;

        const rect = element.getBoundingClientRect();
        const size = Math.ceil(rect.width) + "|" + Math.ceil(rect.height);

        element.innerHTML = '';

        return size;
    }

    return "0|0";
};

document.getBaseLineOffset = (function () {
    const ctx = document.createElement('canvas').getContext('2d');
    return function (element) {
        if (!element) return 0.0;
        ctx.font = getComputedStyle(element).font;
        return ctx.measureText('').fontBoundingBoxAscent;
    };
})();

document.setContentString = function (id, text, removeTextWrapping) {
    var el = document.getElementById(id);
    if (el) {
        el.innerText = text;
        if (removeTextWrapping)
            el.style.whiteSpace = "nowrap";
    };
};

window.ViewInteropErrors = function () {
    for (var key in document.interopErrors) {
        console.log(`Unable to find element with id '${key}' (${document.interopErrors[key]} time(s)).`);
    }
};

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
};

function callScriptableObjectEvent(scriptableObjectName, eventName, passedArgs) {
    var scriptableObj = window[scriptableObjectName];
    if (scriptableObj && scriptableObj[eventName]) {
        scriptableObj[eventName].apply(scriptableObj, passedArgs);
    }
};

document.elementsFromPointOpenSilver = function (x, y) {
    const ids = [];
    const hitTestResults = document.elementsFromPoint(x, y);
    for (const el of hitTestResults) {
        const xamlid = el.xamlid;
        if (xamlid) {
            ids.push(xamlid);
        }
    }
    return JSON.stringify(ids);
};

document.loadFont = async function (family, source, loadedCallback) {
    try {
        const font = new FontFace(family, source);
        await font.load();
        document.fonts.add(font);
        loadedCallback(true);
    } catch (error) {
        loadedCallback(false);
    }
};

document.getSystemColor = function (color) {
    if (CSS.supports('color', color)) {
        const div = document.createElement('div');
        div.style.color = color;
        div.style.display = 'none';
        document.body.appendChild(div);
        const computedColor = window.getComputedStyle(div).color;
        document.body.removeChild(div);
        return computedColor;
    }

    return '';
};

document.textboxHelpers = (function () {
    function getSelectionLength(view) {
        return view.selectionEnd - view.selectionStart;
    };

    function getCaretPosition(view) {
        return view.selectionDirection === 'forward' ? view.selectionEnd : view.selectionStart;
    };

    function isNewLineChar(c) {
        return c === '\n' || c === '\r';
    };

    function navigateInDirection(view, e) {
        if (!e.shiftKey && !e.ctrlKey && getSelectionLength(view) > 0) {
            return true;
        }

        switch (e.key) {
            case 'ArrowUp':
                return getCaretPosition(view) > 0;
            case 'ArrowDown':
                return getCaretPosition(view) < view.value.length;
            case 'ArrowLeft':
                return window.getComputedStyle(view).direction === 'ltr' ?
                    (getCaretPosition(view) > 0) :
                    (getCaretPosition(view) < view.value.length);
            case 'ArrowRight':
                return window.getComputedStyle(view).direction === 'ltr' ?
                    (getCaretPosition(view) < view.value.length) :
                    (getCaretPosition(view) > 0);
            default:
                return false;
        }
    };

    function navigateByPage(view, e) {
        // In Chrome, navigation with PageUp and PageDown does not work when overflow is set to 'hidden',
        // so we manually update the cursor position here.

        if (e.ctrlKey) {
            return false;
        }

        if (e.key === 'PageDown') {
            if (getCaretPosition(view) < view.value.length || (!e.shiftKey && getSelectionLength(view) > 0)) {
                const start = e.shiftKey ? (view.selectionDirection === 'forward' ? view.selectionStart : view.selectionEnd) : view.value.length;
                const end = view.value.length;
                view.setSelectionRange(start, end, 'forward');
                view.scrollTo(view.scrollWidth, view.scrollHeight);
                return true;
            }
        } else {
            if (getCaretPosition(view) > 0 || (!e.shiftKey && getSelectionLength(view) > 0)) {
                const start = 0;
                const end = e.shiftKey ? (view.selectionDirection === 'forward' ? view.selectionStart : view.selectionEnd) : 0;
                view.setSelectionRange(start, end, 'backward');
                view.scrollTo(0, 0);
                return true;
            }
        }

        return false;
    };

    function navigateToStart(view, e) {
        if (!e.shiftKey && getSelectionLength(view) > 0) {
            return true;
        }

        const caretIndex = getCaretPosition(view); 
        return caretIndex > 0 && (e.ctrlKey || !isNewLineChar(view.value[caretIndex - 1]));
    };

    function navigateToEnd(view, e) {
        if (!e.shiftKey && getSelectionLength(view) > 0) {
            return true;
        }

        const caretIndex = getCaretPosition(view); 
        return caretIndex < view.value.length && (e.ctrlKey || !isNewLineChar(view.value[caretIndex]));
    };

    function handleTab(view, e) {
        if (view.getAttribute('data-acceptstab') === 'true' &&
            (getSelectionLength(view) > 0 || view.maxLength < 0 || view.value.length < view.maxLength)) {
            e.preventDefault();
            view.setRangeText('\t', view.selectionStart, view.selectionEnd, 'end');
            return true;
        }

        return false;
    };

    return {
        createView: function (id, parentId) {
            const view = document.createLayoutElement('textarea', id, parentId, -1);
            view.style.fontSize = 'inherit';
            view.style.fontFamily = 'inherit';
            view.style.color = 'inherit';
            view.style.letterSpacing = 'inherit';
            view.style.resize = 'none';
            view.style.outline = 'none';
            view.style.border = 'none';
            view.style.boxSizing = 'border-box';
            view.style.background = 'transparent';
            view.style.cursor = 'text';
            view.style.overflow = 'hidden';
            view.style.tabSize = '4';
            view.style.padding = '0px';

            view.setAttribute('tabindex', -1);

            view.addEventListener('paste', function (e) {
                if (this.getAttribute('data-acceptsreturn') === 'false') {
                    e.preventDefault();
                    let content = (e.originalEvent || e).clipboardData.getData('text/plain');
                    if (content !== undefined) {
                        content = content.replace(/\n/g, '').replace(/\r/g, '');
                    }
                    document.execCommand('insertText', false, content);
                }
            }, false);
        },
        onKeyDownNative: function (view, e) {
            switch (e.key.toLowerCase()) {
                case 'arrowleft':
                case 'arrowright':
                case 'arrowdown':
                case 'arrowup':
                    return navigateInDirection(view, e);
                case 'pagedown':
                case 'pageup':
                    return navigateByPage(view, e);
                case 'home':
                    return navigateToStart(view, e);
                case 'end':
                    return navigateToEnd(view, e);
                case 'delete':
                    return getCaretPosition(view) < view.value.length || getSelectionLength(view) > 0;
                case 'backspace':
                    return getCaretPosition(view) > 0 || getSelectionLength(view) > 0;
                case 'c':
                case 'x':
                    return e.ctrlKey && getSelectionLength(view) > 0;
                case 'a':
                    return e.ctrlKey && getSelectionLength(view) < view.value.length;
                case 'v':
                case 'y':
                case 'z':
                    return e.ctrlKey;
                case 'tab':
                    return handleTab(view, e);
                default:
                    return false;
            }
        },
    };
})();

document.htmlPresenterHelpers = (function () {
    return {
        createView: function (id, contentId, parentId) {
            const view = document.createLayoutElement('div', id, parentId, -1);
            if (view) {
                const content = document.createElement('div');
                content.setAttribute('id', contentId);
                content.attachShadow({ mode: 'open' });
                view.appendChild(content);
            }
        },
        onKeyDownNative: function (view, e) {
            if (!view || !e) return false;

            switch (e.key) {
                case 'ArrowLeft':
                    return view.scrollLeft > 0;
                case 'ArrowRight':
                    return view.scrollLeft < (view.scrollWidth - view.clientWidth);
                case 'ArrowUp':
                case 'PageUp':
                case 'Home':
                    return view.scrollTop > 0;
                case 'ArrowDown':
                case 'PageDown':
                case 'End':
                    return view.scrollTop < (view.scrollHeight - view.clientHeight);
            }

            return false;
        },
        onWheelNative: function (view, e) {
            if (!view || !e || e.deltaY === 0) return false;

            if (e.deltaY > 0) {
                if (e.shiftKey) {
                    return view.scrollLeft < (view.scrollWidth - view.clientWidth);
                } else {
                    return view.scrollTop < (view.scrollHeight - view.clientHeight);
                }
            } else {
                if (e.shiftKey) {
                    return view.scrollLeft > 0;
                } else {
                    return view.scrollTop > 0;
                }
            }
        },
    };
})();

document.createAnimationManager = function (callback) {
    if (document.animationManager) return;

    let _frameDelay = 1 / 60;
    let _isRunning = false;
    let _frameRequestId = null;
    let _previousTimeStamp = 0;

    function scheduleAnimationFrame() {
        if (!_isRunning || _frameRequestId !== null) return;

        _frameRequestId = window.requestAnimationFrame(onFrame);
    };

    function onFrame(timeStamp) {
        _frameRequestId = null;

        if (skipFrame(timeStamp)) {
            scheduleAnimationFrame();
            return;
        }

        _previousTimeStamp = timeStamp;
        try {
            callback();
        } finally {
            scheduleAnimationFrame();
        }
    };

    function skipFrame(timeStamp) {
        const elapsed = timeStamp - _previousTimeStamp;
        return elapsed < _frameDelay;
    };

    document.animationManager = {
        resume: function () {
            if (!_isRunning) {
                _isRunning = true;
                scheduleAnimationFrame();
            }
        },
        pause: function () {
            _isRunning = false;
            if (_frameRequestId) {
                window.cancelAnimationFrame(_frameRequestId);
                _frameRequestId = null;
            }
        },
        setFrameRate: function (frameRate) {
            if (frameRate <= 0) {
                _frameDelay = 0;
            } else {
                _frameDelay = 1000 / frameRate;
            }
        },
    };
};

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