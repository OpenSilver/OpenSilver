//******************************************
//           C#/XAML for HTML5
//    Copyright 2019 Userware/CSHTML5
//        http://www.cshtml5.com
//******************************************

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


window.onCallBack = {}
window.onCallBack.OnCallbackFromJavaScript = function (callbackId, idWhereCallbackArgsAreStored, callbackArgsObject) {
	try {
		DotNet.invokeMethod("OpenSilver", "OnCallbackFromJavaScript", callbackId, idWhereCallbackArgsAreStored, "");
	} catch (e)	{
		DotNet.invokeMethod("OpenSilver.UwpCompatible", "OnCallbackFromJavaScript", callbackId, idWhereCallbackArgsAreStored, "");
	}	
};

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
        return result + " [NOT USABLE DIRECTLY IN C#] (" + resultType + ")";
    }
};
