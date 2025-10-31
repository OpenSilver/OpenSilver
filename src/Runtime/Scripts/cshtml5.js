
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

    const element = document._createElement(tagName, id, parent.windowid);

    if (index < 0 || index >= parent.children.length) {
        parent.appendChild(element);
    } else {
        parent.insertBefore(element, parent.children[index]);
    }

    return element;
};

document._createElement = function (tagName, id, windowid) {
    const element = document.createElement(tagName);
    element.setAttribute('id', id);
    Object.defineProperty(element, 'xamlid', {
        value: id,
        writable: false,
    });
    Object.defineProperty(element, 'windowid', {
        value: windowid,
        writable: false,
    });
    Object.defineProperty(element, 'dump', {
        get() { return document.dumpProperties(id); }
    });
    return element;
};

document.createLayout = function (tagName, id, parentId, isKeyboardFocusable) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const element = document._createLayout(tagName, id, parent.windowid, isKeyboardFocusable);
    parent.appendChild(element);
};

document._createLayout = function (tagName, id, windowid, isKeyboardFocusable) {
    const element = document._createElement(tagName, id, windowid);
    element.classList.add('opensilver-uielement', 'uielement-unarranged');
    document.inputManager.addListeners(element, isKeyboardFocusable);
    return element;
};

document.dumpProperties = function (id, ...names) {
    if (DotNet && DotNet.invokeMethod) {
        return DotNet.invokeMethod('OpenSilver', 'DumpProperties', id, names);
    }
    return null;
};

document.createTextBlock = function (id, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const element = document._createLayout('div', id, parent.windowid, false);
    element.classList.add('opensilver-textblock');

    parent.appendChild(element);
};

document.createBorder = function (id, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const element = document._createLayout('div', id, parent.windowid, false);
    element.classList.add('opensilver-border');

    parent.appendChild(element);
};

document.createInkPresenter = function (id, canvasId, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const element = document._createLayout('div', id, parent.windowid, false);
    const canvas = document._createElement('canvas', canvasId, parent.windowid);
    canvas.classList.add('opensilver-inkpresenter');

    element.appendChild(canvas);
    parent.appendChild(element);
};

document.createWindow = function (id, rootElementId) {
    const rootElement = document.getElementById(rootElementId);
    if (!rootElement) return;

    // Set the window on the root element, used by popups
    Object.defineProperty(rootElement, 'windowid', {
        value: id,
        writable: false,
        configurable: true,
    });

    const w = document._createElement('div', id, id);
    w.classList.add('opensilver-window');

    rootElement.appendChild(w);
};

document.createPopupRoot = function (id, rootElementId, pointerEvents) {
    const rootElement = document.getElementById(rootElementId);
    if (!rootElement) return;

    const popupRoot = document._createElement('div', id, rootElement.windowid);
    popupRoot.classList.add('opensilver-popup');
    popupRoot.style.pointerEvents = pointerEvents;

    rootElement.appendChild(popupRoot);
};

document.createImageManager = function (loadCallback, errorCallback) {
    if (document.imgManager) return;

    document.imgManager = {
        create: function (id, imgId, parentId) {
            const parent = document.getElementById(parentId);
            if (!parent) return;

            const element = document._createLayout('div', id, parent.windowid, false);
            element.style.lineHeight = '0px';

            const img = document._createElement('img', imgId, parent.windowid);
            img.setAttribute('draggable', false);
            img.setAttribute('alt', ' ');
            img.style.display = 'none';
            img.style.width = 'inherit';
            img.style.height = 'inherit';
            img.style.lineHeight = '0px';
            img.style.objectFit = 'contain';
            img.style.objectPosition = 'left top';
            img.addEventListener('load', function (e) {
                this.style.display = '';
                loadCallback(id);
            })
            img.addEventListener('error', function (e) {
                this.style.display = 'none';
                errorCallback(id);
            });

            element.appendChild(img);
            parent.appendChild(element);
        },
        getNaturalWidth: function (img) {
            if (img) {
                return img.naturalWidth;
            }
            return 0.0;
        },
        getNaturalHeight: function (img) {
            if (img) {
                return img.naturalHeight;
            }
            return 0.0;
        },
    };
};

document.createInline = function (tagName, id, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const inline = document.createElement(tagName);
    inline.setAttribute('id', id);
    inline.classList.add('opensilver-inline');

    parent.appendChild(inline);
};

document.createBlock = function (tagName, id, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const block = document.createElement(tagName);
    block.setAttribute('id', id);
    block.classList.add('opensilver-block');

    parent.appendChild(block);
};

document.createShape = function (svgTagName, svgId, shapeId, defsId, parentId) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg.classList.add('opensilver-uielement', 'opensilver-shape', 'uielement-unarranged');
    document.inputManager.addListeners(svg, false);
    svg.setAttribute('id', svgId);
    Object.defineProperty(svg, 'xamlid', {
        value: svgId,
        writable: false,
    });
    Object.defineProperty(svg, 'windowid', {
        value: parent.windowid,
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
    Object.defineProperty(shape, 'windowid', {
        value: parent.windowid,
        writable: false,
    });
    svg.appendChild(shape);
    const defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
    defs.setAttribute('id', defsId);
    svg.appendChild(defs);
    parent.appendChild(svg);
};

document.createSvg = function (id, parentId, tagName) {
    const parent = document.getElementById(parentId);
    if (!parent) return;

    const svg = document.createElementNS('http://www.w3.org/2000/svg', tagName);
    svg.setAttribute('id', id);
    parent.appendChild(svg);
};

document.drawSvgLinearGradient = function (id, x1, y1, x2, y2, units, spreadMethod, transform, opacity, ...stops) {
    const linearGradient = document.getElementById(id);
    if (linearGradient) {
        linearGradient.setAttribute('x1', x1);
        linearGradient.setAttribute('y1', y1);
        linearGradient.setAttribute('x2', x2);
        linearGradient.setAttribute('y2', y2);
        linearGradient.setAttribute('gradientUnits', units);
        linearGradient.setAttribute('spreadMethod', spreadMethod);
        linearGradient.setAttribute('gradientTransform', transform);

        linearGradient.innerHTML = '';
        for (let i = 0; i < stops.length; i += 2) {
            const stop = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
            stop.setAttribute('offset', stops[i]);
            stop.style.stopColor = stops[i + 1];
            stop.style.stopOpacity = opacity;
            linearGradient.appendChild(stop);
        }
    }
};

document.drawSvgRadialGradient = function (id, cx, cy, r, units, spreadMethod, transform, opacity, ...stops) {
    const radialGradient = document.getElementById(id);
    if (radialGradient) {
        radialGradient.setAttribute('cx', cx);
        radialGradient.setAttribute('cy', cy);
        radialGradient.setAttribute('r', r);
        radialGradient.setAttribute('gradientUnits', units);
        radialGradient.setAttribute('spreadMethod', spreadMethod);
        radialGradient.setAttribute('gradientTransform', transform);

        radialGradient.innerHTML = '';
        for (let i = 0; i < stops.length; i += 2) {
            const stop = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
            stop.setAttribute('offset', stops[i]);
            stop.style.stopColor = stops[i + 1];
            stop.style.stopOpacity = opacity;
            radialGradient.appendChild(stop);
        }
    }
};

document.setSvgPatternNaturalSize = function (patternId, imageId, renderTargetId, alignX, alignY) {
    const pattern = document.getElementById(patternId);
    const image = document.getElementById(imageId);
    const renderTarget = document.getElementById(renderTargetId);

    if (!pattern || !image || !renderTarget) return;

    const img = document.createElement('img');
    img.src = image.getAttribute('href');
    img.onload = function () {
        const naturalWidth = img.naturalWidth;
        const naturalHeight = img.naturalHeight;

        image.setAttribute('width', naturalWidth);
        image.setAttribute('height', naturalHeight);

        const bounds = renderTarget.getBoundingClientRect();
        const width = bounds.width;
        const height = bounds.height;

        switch (alignX) {
            case 0: // AlignmentX.Left
                viewBoxAlignX = 0;
                break;

            case 1: // AlignmentX.Center
                viewBoxAlignX = (naturalWidth - width) / 2;
                break;

            case 2: // AlignmentX.Right
                viewBoxAlignX = naturalWidth - width;
                break;

            default:
                viewBoxAlignX = 0;
                break;
        }

        let viewBoxAlignY;
        switch (alignY) {
            case 0: // AlignmentY.Top
                viewBoxAlignY = 0;
                break;

            case 1: // AlignmentY.Center
                viewBoxAlignY = (naturalHeight - height) / 2;
                break;

            case 2: // AlignmentY.Bottom
                viewBoxAlignY = naturalHeight - height;
                break;

            default:
                viewBoxAlignY = 0;
                break;
        }

        pattern.setAttribute('viewBox', `${viewBoxAlignX} ${viewBoxAlignY} ${width} ${height}`);
    };
};

document.arrangeRectangle = function (id, x, y, width, height) {
    const rect = document.getElementById(id);
    if (rect) {
        rect.setAttribute('x', x);
        rect.setAttribute('y', y);
        rect.setAttribute('width', width);
        rect.setAttribute('height', height);
    }
};

document.arrangeEllipse = function (id, rx, ry, penThickness) {
    const ellipse = document.getElementById(id);
    if (ellipse) {
        ellipse.setAttribute('rx', rx);
        ellipse.setAttribute('ry', ry);
        ellipse.setAttribute('cx', rx + penThickness / 2.0);
        ellipse.setAttribute('cy', ry + penThickness / 2.0);
    }
};

document.getBBox = function (svgElement) {
    if (svgElement && svgElement instanceof SVGElement) {
        const bbox = svgElement.getBBox();
        return JSON.stringify({ X: bbox.x, Y: bbox.y, Width: bbox.width, Height: bbox.height, });
    }
    return '{}';
};

document.setCSS = function (id, cssPropertyName, value) {
    const element = document.getElementById(id);
    if (element) {
        element.style[cssPropertyName] = value;
    }
};

document.setCSSProperty = function (id, propertyName, value, priority) {
    const element = document.getElementById(id);
    if (element) {
        element.style.setProperty(propertyName, value, priority);
    }
};

document.setAttr = function (id, attributeName, value) {
    const element = document.getElementById(id);
    if (element) {
        element.setAttribute(attributeName, value);
    }
};

document.unsetAttr = function (id, attributeName) {
    const element = document.getElementById(id);
    if (element) {
        element.removeAttribute(attributeName);
    }
};

document.setProp = function (id, propertyName, value) {
    const element = document.getElementById(id);
    if (element) {
        element[propertyName] = value;
    }
};

document.detachView = function (id) {
    const element = document.getElementById(id);
    if (element) {
        element.remove();
    }
};

document.setFocus = function (element) {
    if (!element) return;

    setTimeout(function () {
        element.setAttribute('tabindex', 0);
        element.focus({ preventScroll: true });
    });
};

document.createInputManager = function (callback, pointerCallback) {
    if (document.inputManager) return;

    // This must remain synchronyzed with the EVENTS enum defined in InputManager.cs.
    // Make sure to change both files if you update this !
    const EVENTS = {
        POINTER_MOVE: 0,
        POINTER_LEFT_DOWN: 1,
        POINTER_LEFT_UP: 2,
        POINTER_RIGHT_DOWN: 3,
        POINTER_RIGHT_UP: 4,
        POINTER_MIDDLE_DOWN: 5,
        POINTER_MIDDLE_UP: 6,
        POINTER_ENTER: 7,
        POINTER_LEAVE: 8,
        WHEEL: 9,
        KEYDOWN: 10,
        KEYUP: 11,
        KEYPRESS: 12,
        FOCUS_UNMANAGED: 13,
        WINDOW_FOCUS: 14,
        WINDOW_BLUR: 15,
    };

    const MODIFIERKEYS = {
        NONE: 0,
        CONTROL: 1,
        ALT: 2,
        SHIFT: 4,
        WINDOWS: 8,
    };

    const FocusManager = (function () {
        let _isManagedFocusUpdate = false;

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

                return document.activeElement === element;
            },
        };
    })();

    let _modifiers = MODIFIERKEYS.NONE;
    let _pointerCapture = null;
    let _suppressContextMenu = false;

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

    function getClosestElement(element) {
        while (element) {
            if (element.xamlid) {
                return element;
            }

            element = element.parentElement;
        }

        return null;
    }

    function getClosestElementId(element) {
        const e = getClosestElement(element);
        if (e) {
            return e.xamlid;
        }
        return '';
    }

    function invokePointerCallback(element, type, e) {
        if (!element) {
            callback('', type, e);
            return;
        }

        let pageX = e.pageX;
        let pageY = e.pageY;

        const parentWindow = document.getElementById(element.windowid);
        if (parentWindow) {
            const windowRect = parentWindow.getBoundingClientRect();
            const bodyRect = document.body.getBoundingClientRect();
            pageX -= (windowRect.left - bodyRect.left);
            pageY -= (windowRect.top - bodyRect.top);
        }

        pointerCallback(getClosestElementId(element), type, e, e.pointerType === 'touch', pageX, pageY, _modifiers);
    }

    function initDom() {
        document.addEventListener('pointerdown', function (e) {
            if (!e.isHandled) {
                switch (e.button) {
                    case 0:
                        callback('', EVENTS.POINTER_LEFT_DOWN, e);
                        break;
                    case 1:
                        callback('', EVENTS.POINTER_MIDDLE_DOWN, e);
                        break;
                    case 2:
                        callback('', EVENTS.POINTER_RIGHT_DOWN, e);
                        break;
                }
            }
        });

        document.addEventListener('pointerup', function (e) {
            if (!e.isHandled) {
                const target = _pointerCapture;
                switch (e.button) {
                    case 0:
                        invokePointerCallback(getClosestElement(target), EVENTS.POINTER_LEFT_UP, e);
                        break;
                    case 1:
                        invokePointerCallback(getClosestElement(target), EVENTS.POINTER_MIDDLE_UP, e);
                        break;
                    case 2:
                        invokePointerCallback(getClosestElement(target), EVENTS.POINTER_RIGHT_UP, e);
                        break;
                }
            }
        });

        document.addEventListener('pointermove', function (e) {
            if (!e.isHandled) {
                setModifiers(e);
                const target = _pointerCapture;
                if (target !== null) {
                    invokePointerCallback(getClosestElement(target), EVENTS.POINTER_MOVE, e);
                }
            }
        });

        document.addEventListener('contextmenu', function (e) {
            if (_suppressContextMenu ||
                (_pointerCapture !== null && this !== _pointerCapture)) {
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

            root.classList.add('opensilver-root-element');

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

            root.addEventListener('pointermove', function (e) {
                e.isHandled = true;
                setModifiers(e);
                const target = _pointerCapture || e.target;
                invokePointerCallback(getClosestElement(target), EVENTS.POINTER_MOVE, e);
            });

            root.addEventListener('wheel', function (e) {
                // Zoom in/out request, takes priority over opensilver
                if (e.ctrlKey) return;
                // Only support vertical scroll
                if (e.deltaY === 0) return;
                e.isHandled = true;
                setModifiers(e);
                const target = _pointerCapture || e.target;
                invokePointerCallback(getClosestElement(target), EVENTS.WHEEL, e);
            });

            root.addEventListener('pointerdown', function (e) {
                e.isHandled = true;
                setModifiers(e);
                const element = (_pointerCapture === null || e.target === _pointerCapture) ? getClosestElement(e.target) : null;
                switch (e.button) {
                    case 0:
                        invokePointerCallback(element, EVENTS.POINTER_LEFT_DOWN, e);
                        break;
                    case 1:
                        invokePointerCallback(element, EVENTS.POINTER_MIDDLE_DOWN, e);
                        break;
                    case 2:
                        invokePointerCallback(element, EVENTS.POINTER_RIGHT_DOWN, e);
                        break;
                }
            });

            root.addEventListener('pointerup', function (e) {
                e.isHandled = true;
                const target = _pointerCapture || e.target;
                switch (e.button) {
                    case 0:
                        invokePointerCallback(getClosestElement(target), EVENTS.POINTER_LEFT_UP, e);
                        break;
                    case 1:
                        invokePointerCallback(getClosestElement(target), EVENTS.POINTER_MIDDLE_UP, e);
                        break;
                    case 2:
                        invokePointerCallback(getClosestElement(target), EVENTS.POINTER_RIGHT_UP, e);
                        break;
                }
            });
        },
        addListeners: function (view, isFocusable) {
            if (!view) return;

            view.addEventListener('pointerenter', function (e) {
                if (_pointerCapture === null || this === _pointerCapture) {
                    setModifiers(e);
                    invokePointerCallback(getClosestElement(this), EVENTS.POINTER_ENTER, e);
                }
            });

            view.addEventListener('pointerleave', function (e) {
                if (_pointerCapture === null || this === _pointerCapture) {
                    setModifiers(e);
                    invokePointerCallback(getClosestElement(this), EVENTS.POINTER_LEAVE, e);
                }
            });

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
        capturePointer: function (element) {
            _pointerCapture = element;
            document.body.classList.add('opensilver-pointer-captured');
        },
        releasePointerCapture: function () {
            _pointerCapture = null;
            document.body.classList.remove('opensilver-pointer-captured');
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

document.setVisible = function (id) {
    const element = document.getElementById(id);
    if (element) {
        element.classList.remove('uielement-collapsed', 'uielement-hidden');
    }
};

document.setCollapsed = function (id) {
    const element = document.getElementById(id);
    if (element) {
        element.classList.remove('uielement-hidden');
        element.classList.add('uielement-collapsed');
    }
};

document.setHidden = function (id) {
    const element = document.getElementById(id);
    if (element) {
        element.classList.remove('uielement-collapsed');
        element.classList.add('uielement-hidden');
    }
};

document.arrange = function (id, left, top, width, height, clip, clipLeft, clipTop, clipRight, clipBottom) {
    const element = document.getElementById(id);
    if (element) {
        element.style.left = left + 'px';
        element.style.top = top + 'px';
        element.style.width = width + 'px';
        element.style.height = height + 'px';
        if (clip) {
            element.style.clip = `rect(${clipTop}px ${clipRight}px ${clipBottom}px ${clipLeft}px)`;
        } else {
            element.style.clip = '';
        }
        element.classList.remove('uielement-unarranged');
    }
};

document.attachMeasurementService = function (owner) {
    if (!owner || owner._measurementService) return;

    const htmlMeasurer = document.createElement('div');
    htmlMeasurer.style.position = 'absolute';
    htmlMeasurer.style.visibility = 'hidden';
    htmlMeasurer.style.height = '';
    htmlMeasurer.style.width = '';
    htmlMeasurer.style.boxSizing = 'border-box';
    htmlMeasurer.style.whiteSpace = 'pre';
    htmlMeasurer.style.left = '-100000px';
    htmlMeasurer.style.top = '-100000px';
    htmlMeasurer.style.textAlign = 'left';
    owner.appendChild(htmlMeasurer);

    const canvasMeasurer = document.createElement('canvas').getContext('2d');

    owner._measurementService = {
        measureTextView: function (element, whiteSpace, overflowWrap, maxWidth, emptyVal) {
            if (element instanceof HTMLTextAreaElement) {
                let text = element.value.length == 0 ? emptyVal : element.value;
                // if the text ends with a new line, we need to add one more or it will not be measured
                if (text.endsWith('\n')) text += '\n';
                htmlMeasurer.textContent = text;
            } else {
                htmlMeasurer.innerHTML = element.innerHTML.length == 0 ? emptyVal : element.innerHTML;
            }

            htmlMeasurer.style.fontSize = element.style.fontSize;
            htmlMeasurer.style.fontWeight = element.style.fontWeight;
            htmlMeasurer.style.fontFamily = element.style.fontFamily;
            htmlMeasurer.style.fontStyle = element.style.fontStyle;
            htmlMeasurer.style.letterSpacing = element.style.letterSpacing;
            htmlMeasurer.style.lineHeight = element.style.lineHeight;
            htmlMeasurer.style.setProperty('--line-stacking-strategy', element.style.getPropertyValue('--line-stacking-strategy'));

            htmlMeasurer.style.whiteSpace = whiteSpace;
            htmlMeasurer.style.overflowWrap = overflowWrap;
            htmlMeasurer.style.maxWidth = maxWidth;

            const rect = htmlMeasurer.getBoundingClientRect();
            const size = Math.ceil(rect.width) + '|' + Math.ceil(rect.height);

            htmlMeasurer.innerHTML = '';

            return size;
        },
        measureTextBlock: function (innerHTML, whiteSpace, overflowWrap, lineHeight, lineStackingStrategy, maxWidth) {
            htmlMeasurer.innerHTML = innerHTML;
            htmlMeasurer.style.fontSize = '';
            htmlMeasurer.style.fontWeight = '';
            htmlMeasurer.style.fontFamily = '';
            htmlMeasurer.style.fontStyle = '';
            htmlMeasurer.style.lineHeight = lineHeight;
            htmlMeasurer.style.setProperty('--line-stacking-strategy', lineStackingStrategy);
            htmlMeasurer.style.letterSpacing = '';
            htmlMeasurer.style.whiteSpace = whiteSpace;
            htmlMeasurer.style.overflowWrap = overflowWrap;
            htmlMeasurer.style.maxWidth = maxWidth;

            const rect = htmlMeasurer.getBoundingClientRect();
            const size = rect.width + '|' + rect.height;

            htmlMeasurer.innerHTML = '';

            return size;
        },
        measureBaseline: function (fonts) {
            let baselineOffset = 0.0;
            for (const font of fonts) {
                canvasMeasurer.font = font;
                baselineOffset = Math.max(baselineOffset, canvasMeasurer.measureText('').fontBoundingBoxAscent);
            }
            return baselineOffset;
        },
    };
};

document.measureTextBlock = function (measurerId, innerHTML, whiteSpace, overflowWrap, lineHeight, lineStackingStrategy, maxWidth) {
    const owner = document.getElementById(measurerId);
    if (owner && owner._measurementService) {
        return owner._measurementService.measureTextBlock(innerHTML, whiteSpace, overflowWrap, lineHeight, lineStackingStrategy, maxWidth);
    }
    return '0|0';
};

document.measureTextView = function (measurerId, id, whiteSpace, overflowWrap, maxWidth, emptyVal) {
    const owner = document.getElementById(measurerId);
    if (owner && owner._measurementService) {
        const element = document.getElementById(id);
        if (element) {
            return owner._measurementService.measureTextView(element, whiteSpace, overflowWrap, maxWidth, emptyVal);
        }
    }
    return '0|0';
};

document.measureBaseline = function (measurerId, ...fonts) {
    const owner = document.getElementById(measurerId);
    if (owner && owner._measurementService) {
        return owner._measurementService.measureBaseline(fonts);
    }
    return 0.0;
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

document.createTextviewManager = function (inputCallback, scrollCallback, selectionChangeCallback) {
    if (document.textviewManager) return;

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

    document.textviewManager = {
        createTextView: function (id, parentId) {
            const parent = document.getElementById(parentId);
            if (!parent) return;

            const view = document._createLayout('textarea', id, parent.windowid, true);
            view.classList.add('opensilver-textboxview');

            view.setAttribute('tabindex', -1);

            view.addEventListener('input', function (e) {
                inputCallback(id);
            });

            view.addEventListener('scroll', function (e) {
                scrollCallback(id);
            });

            view.addEventListener('selectionchange', function (e) {
                selectionChangeCallback(id);
            });

            view.addEventListener('paste', function (e) {
                if (this.getAttribute('data-acceptsreturn') === 'false') {
                    const text = e.clipboardData.getData('text/plain');

                    if (text.indexOf('\n') !== -1 || text.indexOf('\r') !== -1) {
                        e.preventDefault();

                        const newText = text.replace(/[\r\n]+/g, '');
                        document.execCommand('insertText', false, newText);

                        // Scroll to the cursor position
                        this.blur();
                        this.focus();
                    }
                }
            });

            parent.appendChild(view);
        },
        createPasswordView: function (id, parentId) {
            const parent = document.getElementById(parentId);
            if (!parent) return;

            const view = document._createLayout('input', id, parent.windowid, true);
            view.classList.add('opensilver-passwordboxview');

            view.setAttribute('type', 'password');
            view.setAttribute('tabindex', -1);

            view.addEventListener('input', function (e) {
                inputCallback(id);
            });

            view.addEventListener('scroll', function (e) {
                scrollCallback(id);
            });

            parent.appendChild(view);
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
        handleKeyDownFromSimulator: function (view) {
            if (!view) return;
            view.addEventListener('keydown', function (e) {
                const acceptsReturn = this.getAttribute('data-acceptsreturn');
                var maxLength = this.getAttribute('maxlength');
                const acceptsTab = this.getAttribute('data-acceptstab');

                if (maxLength == null) maxLength = 0;
                if (e.keyCode == 13) {
                    if (acceptsReturn != "true") {
                        e.preventDefault();
                        return false;
                    }
                }

                const isAddingTabulation = e.keyCode == 9 && acceptsTab == 'true';
                if ((isAddingTabulation || e.keyCode == 13 || e.keyCode == 32 || e.keyCode > 47) && maxLength != 0) {
                    let text = this.value;
                    if (!acceptsReturn) {
                        text = text.replace('\n', '').replace('\r', '');
                    }

                    let correctionDueToNewLines = 0;
                    if (e.keyCode == 13) {
                        ++correctionDueToNewLines; //because adding a new line takes 2 characters instead of 1.
                    }
                    if (text.length + correctionDueToNewLines >= maxLength) {
                        if (!window.getSelection().toString()) {
                            e.preventDefault();
                            return false;
                        }
                    }
                }

                if (isAddingTabulation) {
                    //we need to add '\t' where the cursor is, prevent the event (which would change the focus) and dispatch the event for the text changed:
                    let sel, range;
                    if (window.getSelection) {
                        sel = window.getSelection();
                        if (sel.rangeCount) {
                            range = sel.getRangeAt(0);
                            range.deleteContents();
                            range.insertNode(document.createTextNode('\t'));
                            sel.collapseToEnd();
                            range.collapse(false); //for IE
                        }
                    } else if (document.selection && document.selection.createRange) {
                        range = document.selection.createRange();
                        range.text = '\t';
                        document.selection.collapseToEnd();
                    }

                    e.preventDefault();
                    return false;
                }
            }, false);
        },
        getSelectionStart: function (view) {
            if (view) {
                return view.selectionStart;
            }
            return 0;
        },
        setSelectionStart: function (view, start) {
            if (view) {
                view.setSelectionRange(start, start + view.selectionEnd - view.selectionStart, 'forward');
            }
        },
        getSelectionLength: function (view) {
            if (view) {
                return view.selectionEnd - view.selectionStart;
            }
            return 0;
        },
        setSelectionLength: function (view, length) {
            if (view) {
                view.setSelectionRange(view.selectionStart, view.selectionStart + length, 'forward');
            }
        },
        getSelectedText: function (view) {
            if (view) {
                return view.value.substring(view.selectionStart, view.selectionEnd);
            }
            return '';
        },
        setSelectedText: function (view, text) {
            if (view) {
                view.setRangeText(text, view.selectionStart, view.selectionEnd, 'end');
            }
        },
    };
};

document.createRichTextViewManager = function (selectionChangedCallback, contentChangedCallback, scrollCallback) {
    if (document.richTextViewManager) return;

    const ACCEPTS_TAB_ATTR = 'data-acceptstab';
    const ACCEPTS_RETURN_ATTR = 'data-acceptsreturn';
    const Options = createOptions();
    const instances = new Map();

    function createOptions() {
        const Parchment = Quill.import('parchment');
        const Delta = Quill.import('delta');
        const Keyboard = Quill.import('modules/keyboard');

        // Essential formats
        const Block = Quill.import('blots/block');
        const Break = Quill.import('blots/break');
        const Container = Quill.import('blots/container');
        const Cursor = Quill.import('blots/cursor');
        const Inline = Quill.import('blots/inline');
        const Scroll = Quill.import('blots/scroll');
        const Text = Quill.import('blots/text');

        const IMAGE_ORIG_SRC = 'data-originalsrc';
        const IMAGE_OBJECT_FIT = 'data-objectfit';
        const _originalSourceMap = new WeakMap();
        class ExtendedImage extends Quill.import('formats/image') {
            static blotName = 'image';
            static tagName = 'IMG';

            static formats(domNode) {
                const formats = super.formats(domNode);
                formats[IMAGE_OBJECT_FIT] = domNode.style.objectFit;
                return formats;
            }

            static value(domNode) {
                if (_originalSourceMap.has(domNode)) {
                    return _originalSourceMap.get(domNode);
                }
                return super.value(domNode);
            }

            format(name, value) {
                if (name === IMAGE_ORIG_SRC) {
                    if (value) {
                        _originalSourceMap.set(this.domNode, value);
                    } else {
                        _originalSourceMap.delete(this.domNode);
                    }
                } else if (name === IMAGE_OBJECT_FIT) {
                    if (value) {
                        this.domNode.style.objectFit = value;
                    } else {
                        this.domNode.style.objectFit = '';
                    }
                } else {
                    super.format(name, value);
                }
            }
        }

        // TextElement properties
        const Spacing = new Parchment.StyleAttributor('spacing', 'letter-spacing', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE
        });
        const Font = new Parchment.StyleAttributor('font', 'font-family', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE
        });
        const Size = new Parchment.StyleAttributor('size', 'font-size', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE
        });
        const Style = new Parchment.StyleAttributor('style', 'font-style', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE,
            whitelist: ['normal', 'oblique', 'italic']
        });
        const Weight = new Parchment.StyleAttributor('weight', 'font-weight', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE,
            whitelist: ['100', '200', '300', '350', '400', '500', '600', '700', '800', '900', '950']
        });
        const Color = new Parchment.StyleAttributor('color', 'color', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE
        });
        const Decoration = new Parchment.StyleAttributor('decoration', 'text-decoration', {
            scope: Parchment.Scope.INLINE_ATTRIBUTE,
            whitelist: ['none', 'underline', 'line-through', 'overline']
        });

        // Block properties
        const Height = new Parchment.StyleAttributor('lineheight', 'line-height', {
            scope: Parchment.Scope.BLOCK
        });
        const Align = new Parchment.StyleAttributor('align', 'text-align', {
            scope: Parchment.Scope.BLOCK,
            whitelist: ['start', 'center', 'end', 'justify']
        });

        const registry = new Parchment.Registry();
        registry.register(
            Scroll,
            Block,
            Break,
            Container,
            Cursor,
            Inline,
            Text,
            ExtendedImage,
            Spacing,
            Font,
            Size,
            Style,
            Weight,
            Color,
            Decoration,
            Height,
            Align
        );

        return {
            registry,
            modules: {
                keyboard: {
                    bindings: {
                        tab: {
                            key: 'Tab',
                            handler: function (range, context) {
                                if (acceptsTab(this.quill.container)) {
                                    return Keyboard.DEFAULTS.bindings['tab'].handler.apply(this, [range, context]);
                                }
                                return false;
                            },
                        },
                        'remove tab': {
                            key: 'Tab',
                            shiftKey: true,
                            handler: function (range, context) {
                                if (acceptsTab(this.quill.container)) {
                                    return Keyboard.DEFAULTS.bindings['remove tab'].handler.apply(this, [range, context]);
                                }
                                return false;
                            },
                        },
                        enter: {
                            key: 'Enter',
                            shiftKey: null,
                            handler: function (range, context) {
                                if (acceptsReturn(this.quill.container)) {
                                    const lineFormats = Object.keys(context.format).reduce(
                                        (formats, format) => {
                                            if (this.quill.scroll.query(format, Parchment.Scope.BLOCK) &&
                                                !Array.isArray(context.format[format])) {
                                                formats[format] = context.format[format];
                                            }
                                            return formats;
                                        },
                                        {});
                                    const delta = new Delta()
                                        .retain(range.index)
                                        .delete(range.length)
                                        .insert('\n', lineFormats);
                                    this.quill.updateContents(delta, Quill.sources.USER);
                                    this.quill.setSelection(range.index + 1, Quill.sources.SILENT);
                                    this.quill.focus();

                                    Object.keys(context.format).forEach(name => {
                                        if (lineFormats[name] != null) return;
                                        if (Array.isArray(context.format[name])) return;
                                        if (name === 'code' || name === 'link') return;
                                        this.quill.format(name, context.format[name], Quill.sources.USER);
                                    });
                                }
                                return false;
                            },
                        },
                        bold: {
                            key: 'b',
                            ctrlKey: true,
                            handler: function (range, context) {
                                this.quill.format('weight', this.quill.getFormat().weight > 600 ? '' : '700');
                            },
                        },
                        italic: {
                            key: 'i',
                            ctrlKey: true,
                            handler: function (range, context) {
                                this.quill.format('style', this.quill.getFormat().style === 'italic' ? '' : 'italic');
                            },
                        },
                        underline: {
                            key: 'u',
                            ctrlKey: true,
                            handler: function (range, context) {
                                this.quill.format('decoration', this.quill.getFormat().decoration === 'underline' ? '' : 'underline');
                            },
                        },
                    }
                }
            }
        };
    }

    function acceptsTab(view) { return view.getAttribute(ACCEPTS_TAB_ATTR) === 'true'; }

    function acceptsReturn(view) { return view.getAttribute(ACCEPTS_RETURN_ATTR) === 'true'; }

    function isNewLineChar(c) { return c === '\n' || c === '\r'; };

    function getView(id) { return instances.get(id); }

    function getLength(ql) { return Math.max(0, ql.getLength() - 1); }

    function getSelectionLength(ql) {
        const selection = ql.getSelection();
        if (selection) {
            return selection.length;
        }
        return 0;
    }

    function getSelectionDirection() {
        const selection = document.getSelection();
        const position = selection.anchorNode.compareDocumentPosition(selection.focusNode);
        if (position === 0) {
            return selection.anchorOffset > selection.focusOffset ? 'backward' : 'forward';
        } else if (position === Node.DOCUMENT_POSITION_PRECEDING) {
            return 'backward';
        } else {
            return 'forward';
        }
    }

    function getCaretPosition(ql) {
        const selection = ql.getSelection();
        if (selection) {
            if (selection.length === 0) {
                return selection.index;
            }
            return getSelectionDirection() === 'forward' ? selection.index + selection.length : selection.index;
        }
        return 0;
    }

    function navigateInDirection(ql, e) {
        if (!e.shiftKey && !e.ctrlKey && getSelectionLength(ql) > 0) return true;

        switch (e.key) {
            case 'ArrowUp':
                return getCaretPosition(ql) > 0;
            case 'ArrowDown':
                return getCaretPosition(ql) < getLength(ql);
            case 'ArrowLeft':
                return window.getComputedStyle(ql.container).direction === 'ltr' ?
                    (getCaretPosition(ql) > 0) :
                    (getCaretPosition(ql) < getLength(ql));
            case 'ArrowRight':
                return window.getComputedStyle(ql.container).direction === 'ltr' ?
                    (getCaretPosition(ql) < getLength(ql)) :
                    (getCaretPosition(ql) > 0);
            default:
                return false;
        }
    }

    function navigateByPage(ql, e) {
        if (e.ctrlKey) return false;

        if (e.key === 'PageDown') {
            if (getCaretPosition(ql) < getLength(ql) || (!e.shiftKey && getSelectionLength(ql) > 0)) {
                return true;
            }
        } else {
            if (getCaretPosition(ql) > 0 || (!e.shiftKey && getSelectionLength(ql) > 0)) {
                return true;
            }
        }

        return false;
    }

    function navigateToStart(ql, e) {
        if (!e.shiftKey && getSelectionLength(ql) > 0) {
            return true;
        }

        const caretIndex = getCaretPosition(ql);
        return caretIndex > 0 && (e.ctrlKey || !isNewLineChar(ql.getText(caretIndex - 1, 1)));
    }

    function navigateToEnd(ql, e) {
        if (!e.shiftKey && getSelectionLength(ql) > 0) {
            return true;
        }

        const caretIndex = getCaretPosition(ql);
        return caretIndex < getLength(ql) && (e.ctrlKey || !isNewLineChar(ql.getText(caretIndex, 1)));
    }

    document.richTextViewManager = {
        createView: function (id, parentId) {
            const parent = document.getElementById(parentId);
            if (!parent) return;

            const view = document._createLayout('div', id, parent.windowid, true);
            instances.set(id, view);

            view.addEventListener('scroll', function (e) { scrollCallback(this.id); });
            view.addEventListener('focus', function (e) {
                setTimeout(function (thisArg) {
                    if (document.activeElement === thisArg) {
                        const ql = Quill.find(thisArg);
                        if (ql) {
                            ql.focus();
                        }
                    }
                }, 0, this);
            });

            const ql = new Quill(view, Options);

            // we can't use the 'selection-change' event because it does not fire when the user types in the editor
            ql.on('editor-change', function (eventName, ...args) {
                if (eventName === 'selection-change') {
                    const range = args[0];
                    if (range) {
                        selectionChangedCallback(id, range.index, range.length);
                    }
                }
            });
            ql.on('text-change', function (delta, oldDelta, source) {
                if (source === Quill.sources.USER) {
                    contentChangedCallback(id);
                }
            });

            parent.appendChild(view);
        },
        deleteView: function (id) {
            instances.delete(id);
        },
        setAcceptsTab: function (id, value) {
            const view = document.getElementById(id);
            if (view) {
                view.setAttribute(ACCEPTS_TAB_ATTR, value);
            }
        },
        setAcceptsReturn: function (id, value) {
            const view = document.getElementById(id);
            if (view) {
                view.setAttribute(ACCEPTS_RETURN_ATTR, value);
            }
        },
        measureView: function (id, maxWidth, maxHeight) {
            const ql = Quill.find(getView(id));
            if (!ql) return '0|0';

            const root = ql.root;

            root.style.width = 'max-content';
            root.style.height = 'auto';
            if (maxWidth >= 0) {
                root.style.maxWidth = maxWidth + 'px';
            }
            if (maxHeight >= 0) {
                root.style.maxHeight = maxHeight + 'px';
            }

            const size = root.scrollWidth + '|' + root.scrollHeight;

            root.style.width = '';
            root.style.height = '';
            root.style.maxWidth = '';
            root.style.maxHeight = '';

            return size;
        },
        onKeyDownNative: function (view, e) {
            const ql = Quill.find(view);
            if (!ql) return false;

            switch (e.key.toLowerCase()) {
                case 'arrowleft':
                case 'arrowright':
                case 'arrowdown':
                case 'arrowup':
                    return navigateInDirection(ql, e);
                case 'pagedown':
                case 'pageup':
                    return navigateByPage(ql, e);
                case 'home':
                    return navigateToStart(ql, e);
                case 'end':
                    return navigateToEnd(ql, e);
                case 'delete':
                    return getCaretPosition(ql) < getLength(ql) || getSelectionLength(ql) > 0;
                case 'backspace':
                    return getCaretPosition(ql) > 0 || getSelectionLength(ql) > 0;
                case 'c':
                case 'x':
                    return e.ctrlKey && getSelectionLength(ql) > 0;
                case 'a':
                    return e.ctrlKey && getSelectionLength(ql) < getLength(ql);
                case 'v':
                case 'y':
                case 'z':
                    return e.ctrlKey;
                case 'tab':
                    return acceptsTab(view);
                default:
                    return false;
            }
        },
        getContentLength: function (id) {
            const ql = Quill.find(getView(id));
            if (!ql) return 0;

            return ql.getLength();
        },
        getSelectedText: function (id) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            const selection = ql.getSelection();
            if (selection) {
                return ql.getText(selection);
            }
            return '';
        },
        setSelectedText: function (id, text) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            const selection = ql.getSelection();
            if (selection) {
                if (text.length > 0) {
                    if (selection.length > 0) {
                        ql.deleteText(selection.index, selection.length, Quill.sources.SILENT);
                    }
                    ql.insertText(selection.index, text, Quill.sources.API);
                } else if (selection.length > 0) {
                    ql.deleteText(selection.index, selection.length, Quill.sources.API);
                }
            } else if (text.length > 0) {
                ql.insertText(0, text, Quill.sources.API);
            }
        },
        select: function (id, start, length) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            ql.setSelection(start, length, Quill.sources.API);
        },
        selectAll: function (id) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            ql.setSelection(0, ql.getLength(), Quill.sources.API);
        },
        getContents: function (id, start, length) {
            const ql = Quill.find(getView(id));
            if (!ql) return '[]';

            const contents = ql.getContents(start, length);
            return JSON.stringify(contents.ops);
        },
        setContents: function (id, delta) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            ql.setContents(delta, Quill.sources.API);
        },
        updateContents: function (id, delta) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            ql.updateContents(delta, Quill.sources.API);
        },
        enable: function (id, enable) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            ql.enable(enable);
        },
        format: function (id, property, value) {
            const ql = Quill.find(getView(id));
            if (!ql) return;

            ql.format(property, value, Quill.sources.API);
        },
        getFormat: function (id, property) {
            const ql = Quill.find(getView(id));
            if (!ql) return null;

            const format = ql.getFormat();
            if (!format) return null;

            const f = format[property];
            if (typeof f === 'string') {
                return f;
            }
            return null;
        },
    };
};

document.htmlPresenterHelpers = (function () {
    return {
        createView: function (id, contentId, parentId, useShadowDom) {
            const parent = document.getElementById(parentId);
            if (!parent) return;

            const view = document._createLayout('div', id, parent.windowid, false);
            const content = document.createElement('div');
            content.setAttribute('id', contentId);
            if (useShadowDom) {
                content.attachShadow({ mode: 'open' });
            }

            view.appendChild(content);
            parent.appendChild(view);
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

document.createUIDispatcher = function (callback) {
    if (document.UIDispatcher) return;

    function computeDelay(tickRate) {
        if (tickRate > 0) {
            return 1000 / tickRate;
        } else if (tickRate === 0) {
            return 1000;
        } else {
            return 1;
        }
    }

    let _delay = computeDelay(60);
    let _intervalID = setInterval(callback, _delay);

    document.UIDispatcher = {
        setTickRate: function (tickRate) {
            const delay = computeDelay(tickRate);
            if (_delay !== delay) {
                _delay = delay;
                clearInterval(_intervalID);
                _intervalID = setInterval(callback, _delay);
            }
        },
    };
};

document.createResizeManager = (function (onresizeCallback) {
    const _observer = new ResizeObserver(onResize);
    const _observedElements = new Map();

    function onResize(entries) {
        for (const entry of entries) {
            const id = entry.target.id;

            if (_observedElements.get(id) !== entry.target) {
                continue;
            }

            onresizeCallback(id, entry.contentRect.width, entry.contentRect.height);
        }
    }

    document.resizeManager = {
        observe: function (element) {
            if (element && element.id) {
                _observedElements.set(element.id, element);
                _observer.observe(element);
            }
        },
        unobserve: function (id) {
            const element = _observedElements.get(id);
            if (_observedElements.delete(id)) {
                _observer.unobserve(element);
            }
        },
    }
});

document.openFileDialog = (function () {
    const _dialogs = new Map();

    return {
        createDialog: function (id, changeCallback, changeCompleteCallback, cancelCallback) {
            if (_dialogs.has(id)) {
                throw new Error(`A dialog with id '${id}' has already been registered.`)
            }

            const dialog = document.createElement('input');
            dialog.type = 'file';

            dialog.addEventListener('change', function () {
                if (dialog.files.length === 0) {
                    changeCompleteCallback();
                    return;
                }

                const reader = new FileReader();

                // Reading each file sequentially, some results were null when running concurrently
                function readNext(i) {
                    const file = dialog.files[i];

                    reader.onload = function () {
                        changeCallback(file.name, reader.result.substr(reader.result.indexOf(',') + 1));

                        if (dialog.files.length > i + 1) {
                            readNext(i + 1);
                        } else {
                            // Triggers finished callback
                            changeCompleteCallback();
                        }
                    };

                    // For performance improvements, readAsArrayBuffer could be used and Uint8Array sent to C#,
                    // this has been optimized in .NET 6. However, this would require changes to the C# callback method,
                    // the array cannot be received as object (must be byte[]).
                    reader.readAsDataURL(file);
                }

                readNext(0);
            });

            dialog.addEventListener('cancel', function () {
                cancelCallback();
            });

            _dialogs.set(id, dialog);
        },
        deleteDialog: function (id) {
            _dialogs.delete(id);
        },
        showDialog: function (id) {
            const dialog = _dialogs.get(id);
            if (!dialog) {
                throw new Error(`Cannot find a dialog associated with id '${id}'.`);
            }

            try {
                dialog.showPicker();
                return '';
            } catch (error) {
                return error.message;
            }
        },
        setMultiple: function (id, value) {
            const dialog = _dialogs.get(id);
            if (!dialog) {
                throw new Error(`Cannot find a dialog associated with id '${id}'.`);
            }

            if (value) {
                dialog.setAttribute('multiple', 'multiple');
            } else {
                dialog.removeAttribute('multiple');
            }
        },
        setAccept: function (id, value) {
            const dialog = _dialogs.get(id);
            if (!dialog) {
                throw new Error(`Cannot find a dialog associated with id '${id}'.`);
            }

            if (value) {
                dialog.setAttribute('accept', value);
            } else {
                dialog.removeAttribute('accept');
            }
        },
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
