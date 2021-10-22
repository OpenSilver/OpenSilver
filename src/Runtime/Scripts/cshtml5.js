

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

document.getXamlRoot = function()
{
	let xamlRoot = document.getElementById("opensilver-root");
	if (!xamlRoot)
	{
		xamlRoot = document.getElementById("cshtml5-root");
	}
	return xamlRoot;
}

document.ResXFiles = {};

document.modifiersPressed = 0;

document.refreshKeyModifiers = function (evt) {
    var value = 0;
    if (evt.ctrlKey) {
        value = value | 1;
    }
    if (evt.altKey) {
        value = value | 2;
    }
    if (evt.shiftKey) {
        value = value | 4;
    }
    document.modifiersPressed = value;
}

document.onkeydown = function (evt) {
    evt = evt || window.event;
    document.refreshKeyModifiers(evt);
};

document.onkeyup = function (evt) {
    evt = evt || window.event;
    document.refreshKeyModifiers(evt);
};

document.jsObjRef = new Array();
document.callbackCounterForSimulator = 0;
document.measureTextBlockElement = null;

document.reroute = function reroute(e, elem, shiftKey) {
    shiftKey = shiftKey || false;
    if (e.rerouted === undefined) {
        var evt;
        if (typeof document.dispatchEvent !== 'undefined') {
            evt = document.createEvent('MouseEvents');
            evt.initMouseEvent(
                e.type				// event type
                , e.bubbles			// can bubble?
                , e.cancelable		// cancelable?
                , window			// the event's abstract view (should always be window)
                , e.detail			// mouse click count (or event "detail")
                , e.screenX			// event's screen x coordinate
                , e.screenY			// event's screen y coordinate
                , e.pageX			// event's client x coordinate
                , e.pageY			// event's client y coordinate
                , e.ctrlKey			// whether or not CTRL was pressed during event
                , e.altKey			// whether or not ALT was pressed during event
                , shiftKey			// whether or not SHIFT was pressed during event
                , e.metaKey			// whether or not the meta key was pressed during event
                , e.button			// indicates which button (if any) caused the mouse event (1 = primary button)
                , e.relatedTarget	// relatedTarget (only applicable for mouseover/mouseout events)
            );
            evt.rerouted = true;
            elem.dispatchEvent(evt);
        }
    }
}

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

	if (element.getAttribute('data-isCollapsedDueToHiddenColumn' == true)){
		element.style.overflow = 'visible';
		element.setAttribute('data-isCollapsedDueToHiddenColumn', false);
	}
}

document.setDisplayTableCell = function (id) {
    const element = document.getElementById(id);
    if (!element || element.tagName == 'SPAN')
        return;

    element.style.display = 'table-cell';
}

document.getActualWidthAndHeight = function (element) {
	return (typeof element === 'undefined' || element === null) ? '0|0' : element['offsetWidth'].toFixed(3) + '|' + element['offsetHeight'].toFixed(3);
}

document.createElementSafe = function (tagName, id, parentElement, index) {
	const newElement = document.createElement(tagName);

	newElement.setAttribute("id", id);

    if (typeof parentElement == 'string') {
        parentElement = document.getElementById(parentElement);
    }

    if (parentElement == null) {
        console.log('createElement is failed becaused of the removed parent.');
        return;
    }

	if(index < 0 || index >= parentElement.children.length)	{
		parentElement.appendChild(newElement);
	}
	else {
		var nextSibling = parentElement.children[index];
		parentElement.insertBefore(newElement, nextSibling);
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
	if (element){
		element.removeEventListener(method, func);
	}
}

document.addEventListenerSafe = function (element, method, func) {
    if (typeof element == 'string') {
        element = document.getElementById(element);
    }
	if (element){
		element.addEventListener(method, func);
	}
}

document.eventCallback = function (callbackId, arguments, sync) {
	const argsArray = arguments;
	const idWhereCallbackArgsAreStored = "callback_args_" + document.callbackCounterForSimulator++;
	document.jsObjRef[idWhereCallbackArgsAreStored] = argsArray;
	if (sync) {
		return window.onCallBack.OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, argsArray, true);
	} else {
		setTimeout(
			function()
			{{
				window.onCallBack.OnCallbackFromJavaScript(callbackId, idWhereCallbackArgsAreStored, argsArray, false);
			}}
			, 1);
	}
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

document.rerouteMouseEvents = function (id) {
    document.onmouseup = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.onmouseover = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.onmousedown = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.onmouseout = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.onmousemove = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.onclick = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.oncontextmenu = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
    document.ondblclick = function(e) {
        if(e.doNotReroute == undefined)
        {
            var element = document.getElementById(id);
            if (element) {
                document.reroute(e, element);
            }
        }
    }
}

document.setVisualBounds = function(id, left, top, width, height, bSetAbsolutePosition, bSetZeroMargin, bSetZeroPadding) {
    var element = document.getElementById(id);
    if (element)
    {
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

document.setPosition = function(id, left, top, bSetAbsolutePosition, bSetZeroMargin, bSetZeroPadding) {
    var element = document.getElementById(id);
    if (element)
    {
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

document.measureTextBlock = function(text, fontSize, fontFamily, fontStyle, fontWeight, textWrapping, padding, width, maxWidth) {
    var element = document.measureTextBlockElement;
    if (element)
    {
        var runElement = element.firstElementChild;
        if (runElement != null) {
            runElement.innerText = text;
            runElement.style.fontSize = fontSize;
            runElement.style.fontWeight = fontWeight;
        }

        if (fontSize.length > 0) {
            element.style.fontSize = fontSize;
        }
        if (fontFamily.length > 0) {
            if (fontFamily === "-") {
                fontFamily = "";
            }
            element.style.fontFamily = fontFamily;
        }
        if (fontStyle.length > 0) {
            element.style.fontStyle = fontStyle;
        }
        if (fontWeight.length > 0) {
            element.style.fontWeight = fontWeight;
        }
        if (textWrapping.length > 0) {
            element.style.whiteSpace = textWrapping;
        }
        if (padding.length > 0) {
            element.style.boxSizing = "border-box";
            element.style.padding = padding;
        }

        element.style.width = width;
        element.style.maxWidth = maxWidth;

        return element.offsetWidth + "|" + element.offsetHeight;
    }

    return "0|0";
}

document.setContentString = function(id, text, removeTextWrapping) {
    var el = document.getElementById(id);
    if (el)
    {
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
                    resultString += "\r\n";
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
                    textToAdd = textToAdd.replace(new RegExp("\n", 'g'), "\r\n");
                }
                resultString += textToAdd;
                forceNewLineFirst = false;
            }
        }
        else {
            var nodeName = currentNode.nodeName;
            if (nodeName == "BR") {
                if (forceNewLineFirst) {
                    resultString += "\r\n";
                }
                forceNewLineFirst = true;
            }
            else //we consider it's a <div> or a <p>:
            {
                if (forceNewLineFirst) {
                    resultString += "\r\n";
                    forceNewLineFirst = false;
                }
                if (currentNode.previousSibling != undefined && !resultString.endsWith("\r\n")) {
                    //The element is not the first in its parent and there is no new line to put it, so we add one:
                    resultString += "\r\n";
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


//this counts the amount of characters before the ones (start and end) defined by the range object.
// it does so by defining:
//      -globalIndexes.startIndex and globalIndexes.endIndex: the indexes as in c# of the positions defined by the range
//      -globalIndexes.isStartFound and globalIndexes.isEndFound: a boolean stating whether the index has been found yet ot not (used within this method to know when to stop changing the indexes).
document.getRangeGlobalStartAndEndIndexes = function getRangeGlobalStartAndEndIndexes(currentParent, isFirstChild, charactersWentThrough, range, globalIndexes) {
    //we go down the tree until we find multiple children or until there is no children left:
    while (currentParent.hasChildNodes()) {
        //a div/p/br tag means a new line if it is not the first child of its parent:
        if (!isFirstChild && (currentParent.tagName == 'DIV' || currentParent.tagName == 'P' || currentParent.tagName == 'BR')) {
            charactersWentThrough += 2; //corresponds to counting the characters for the new line (that are not otherwise included in the count)
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
            charactersWentThrough = getRangeGlobalStartAndEndIndexes(temp, i == 0, charactersWentThrough, range, globalIndexes);
            if (globalIndexes.isStartFound && globalIndexes.isEndFound) {
                break;
            }
        }
    }
    else {
        //we stopped because we are at the end of a branch in the tree view --> count the characters in the line:
        //if the end of the branch is a new line, count it:
        if (!isFirstChild && (currentParent.tagName == 'DIV' || currentParent.tagName == 'P' || currentParent.tagName == 'BR')) {
            charactersWentThrough += 2;
        }
        else {
            if (currentParent.length) {
                //we get the basic informations about the text:
                var textContent = currentParent.textContent;
                var splittedText = textContent.split("\n"); //this is to know if the piece of text contains one or more lines.
                var firstText = splittedText[0];
                var splittedLength = splittedText.length;
                var needsCompensation = firstText[firstText.length - 1] != "\r"; //this allows to compensate the cases where new lines are written as "\n" only instead of "\r\n" (1 character instead of 2)
                var wholeLength = textContent.length; //this will be the length of the whole text in this dom element, including possible compensation for the amount of characters used for a new line.
                if (needsCompensation && splittedLength > 1) {
                    wholeLength += splittedLength - 1;
                }
                if (currentParent == range.startContainer) {
                    if (splittedText.length > 1 && needsCompensation) {
                        //when we are at the element containing the position defined in the range and we need adjustments due to the new lines, count the amount of adjustments neede:
                        var i = 0; //this will  be the amount of new lines before the given position.
                        var actualLengthToRangeOffset = 0; //this is the length of the text as is in the browser.
                        //go through the different lines of text while counting the characters and amount of new lines:
                        for (i = 0; i < splittedLength; ++i) {
                            if (actualLengthToRangeOffset + splittedText[i].length >= range.startOffset) {
                                break;
                            }
                            actualLengthToRangeOffset += 1 + splittedText[i].length; //1 --> "\n" that was removed when splitting
                        }
                        globalIndexes.startIndex = charactersWentThrough + range.startOffset + i; //Note: + i because we count 1 additional character ('\r') per new line met before reaching the offset.
                    }
                    else { //if there is no need for compensation on the length taken by the new lines, simply add the offset defined in the range:
                        globalIndexes.startIndex = charactersWentThrough + range.startOffset; //no need to compensate for a new line since it already is 2 characters or there is only one line.
                    }
                    //remember that the index was found:
                    globalIndexes.isStartFound = true;
                }
                if (currentParent == range.endContainer) {
                    if (splittedText.length > 1 && needsCompensation) {
                        var i = 0;
                        var actualLengthToRangeOffset = 0; //this is the length of the text as is in the browser.
                        for (i = 0; i < splittedLength; ++i) {
                            if (actualLengthToRangeOffset + splittedText[i].length >= range.endOffset) {
                                break;
                            }
                            actualLengthToRangeOffset += 1 + splittedText[i].length; //1 --> \n that was removed
                        }
                        globalIndexes.endIndex = charactersWentThrough + range.endOffset + i; //Note: + i because we count 1 additional character ('\r') per new line met before reaching the offset.
                    }
                    else {
                        globalIndexes.endIndex = charactersWentThrough + range.endOffset; //no need to compensate for a new line since it already is 2 characters or there is only one line.
                    }
                    globalIndexes.isEndFound = true;
                }
                charactersWentThrough += wholeLength; //move forward in the count of characters.
            }
        }
        return charactersWentThrough;
    }
}

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
    if(window.chrome && correctionDueToNewLines != 0)
    {
        --correctionDueToNewLines; //on chrome, we have a \n right at the end for some reason.
    }
    else if (window.IE_VERSION)
    {
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

(function() {

    function createShiftArr(step) {

        var space = '    ';

        if ( isNaN(parseInt(step)) ) {  // argument is string
            space = step;
        } else { // argument is integer
            switch(step) {
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
        for(var ix=0;ix<100;ix++){
            shift.push(shift[ix]+space); 
        }
        return shift;
    }

    function vkbeautify(){
        this.step = '    '; // 4 spaces
        this.shift = createShiftArr(this.step);
    }

    vkbeautify.prototype.xml = function(text,step) {

        var ar = text.replace(/>\s*</g,"><")
                     .replace(/</g,"~::~<")
                     .replace(/\s*xmlns:/g,"~::~xmlns:")
                     .replace(/\s*xmlns=/g,"~::~xmlns=")
                     .split('~::~'),
            len = ar.length,
            inComment = false,
            deep = 0,
            str = '',
            ix,
            shift = step ? createShiftArr(step) : this.shift,
            withNamespace = 0;

        for(ix=0;ix<len;ix++) {
            // start comment or <![CDATA[...]]> or <!DOCTYPE //
            if(ar[ix].search(/<!/) > -1) { 
                str += shift[deep]+ar[ix];
                inComment = true; 
                // end comment  or <![CDATA[...]]> //
                if(ar[ix].search(/-->/) > -1 || ar[ix].search(/]>/) > -1 || ar[ix].search(/!DOCTYPE/) > -1 ) {
                    inComment = false; 
                }
            } else 
                // end comment  or <![CDATA[...]]> //
                if(ar[ix].search(/-->/) > -1 || ar[ix].search(/]>/) > -1) {
                    str += ar[ix];
                    inComment = false; 
                } else 
                    // <elm></elm> //
                    if( /^<\w/.exec(ar[ix-1]) && /^<\/\w/.exec(ar[ix]) &&
                        // This comparison will eventually compare an array with a single string item to another string
                        // so we voluntarily use '=='
                        /^<[\w:\-\.,]+/.exec(ar[ix-1]) == /^<\/[\w:\-\.,]+/.exec(ar[ix])[0].replace('/','')) { // jshint ignore:line
                        str += ar[ix];
                        if(!inComment) {
                            deep--;
                        }
                    } else
                        // <elm> //
                        if(ar[ix].search(/<\w/) > -1 && ar[ix].search(/<\//) === -1 && ar[ix].search(/\/>/) === -1 ) {
                            str = !inComment ? str += shift[deep++]+ar[ix] : str += ar[ix];
                        } else 
                            // <elm>...</elm> //
                            if(ar[ix].search(/<\w/) > -1 && ar[ix].search(/<\//) > -1) {
                                str = !inComment ? str += shift[deep]+ar[ix] : str += ar[ix];
                            } else 
                                // </elm> //
                                if(ar[ix].search(/<\//) > -1) { 
                                    --deep;
                                    str = !inComment && !withNamespace? str += shift[deep] + ar[ix] : str += ar[ix];
                                } else 
                                    // <elm/> //
                                    if(ar[ix].search(/\/>/) > -1 ) { 
                                        str = !inComment ? str += shift[deep]+ar[ix] : str += ar[ix];
                                        if (ar[ix].search(/xmlns\:/) > -1  || ar[ix].search(/xmlns\=/) > -1)
                                        deep--;
                                    } else 
                                        // <? xml ... ?> //
                                        if(ar[ix].search(/<\?/) > -1) { 
                                            str += shift[deep]+ar[ix];
                                        } else 
                                            // xmlns //
                                            if( ar[ix].search(/xmlns:/) > -1  || ar[ix].search(/xmlns=/) > -1) {
                                                str += shift[deep]+ar[ix];
                                                withNamespace = 2;
                                            }
                                            else {
                                                str += ar[ix];
                                            }
                                            if (withNamespace)
                                                withNamespace--;
        }

        return  (str.charAt(0) === '\n') ? str.slice(1) : str;
    };

    window.vkbeautify = new vkbeautify();

})();


//------------------------------
// INITIALIZE
//------------------------------


window.addEventListener('load', function () {
    if (typeof FastClick !== 'undefined') {
        new FastClick(document.body);
    }
}, false);


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
