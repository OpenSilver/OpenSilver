
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CSHTML5;
using CSHTML5.Internal;

namespace OpenSilver;

public static partial class Interop
{
    private static readonly WebResourcesManager _webResourcesManager = new();

    private static readonly HashSet<string> _cssFileKeys = new();
    private static readonly HashSet<string> _jsScriptFileKeys = new();

    /// <summary>
    /// Adds a 'script' tag to the HTML page and waits for the script to finish loading.
    /// </summary>
    /// <param name="url">The URL of the JavaScript file, with the syntax ms-appx:///AssemblyName/Folder/FileName.js or /AssemblyName;component/Folder/FileName.js</param>
    /// <returns>Nothing.</returns>
    public static Task<object> LoadJavaScriptFile(string url)
    {
        // Get the assembly name of the calling method: 
        //IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
        //that is executed immediately after the one where the URI is defined! Be careful
        //when moving the following line of code.
        string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;

        var t = new TaskCompletionSource<object>();
        _webResourcesManager.LoadJavaScriptFile(
            url,
            callerAssemblyName,
            () => t.SetResult(null), () => t.SetException(new Exception("Could not load file: \"" + url + "\"."))
        );
        return t.Task;
    }

    public static Task<object> LoadJavaScriptFile(ResourceFile resourceFile)
    {
        if (!_jsScriptFileKeys.Contains(resourceFile.Key))
        {
            // Get the assembly name of the calling method: 
            // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
            // that is executed immediately after the one where the URI is defined! Be careful
            // when moving the following line of code.
            string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;

            var t = new TaskCompletionSource<object>();
            _webResourcesManager.LoadJavaScriptFile(resourceFile.Url, callerAssemblyName,
                () =>
                {
                    t.SetResult(null);
                    _jsScriptFileKeys.Add(resourceFile.Key); //we only set this when it successfully loaded so that the next time we try to load a file with the same key, finding the key in the HashSet means that it was already SUCCESSFULLY loaded.
                },
                () => t.SetException(new Exception("Could not load file: \"" + resourceFile.Url + "\".")));

            return t.Task;
        }

        var task = new TaskCompletionSource<object>();
        task.SetResult(null);
        return task.Task;
    }

    /// <summary>
    /// Loads a list of JavaScript files from either an online location (http/https) or the local project. Note: This method will stop at the first script it cannot load, meaning that all subsequent scripts will not be loaded.
    /// </summary>
    /// <param name="urls">The URLs of the JavaScript files, with the syntax ms-appx:///AssemblyName/Folder/FileName.js or /AssemblyName;component/Folder/FileName.js or https://someAddress/FileName.js
    /// </param>
    /// <param name="callback">The method that is called when all the files have successfully finished loading.</param>
    /// <param name="callbackOnError">The method that is called when one of the files could not be loaded.</param>
    public static void LoadJavaScriptFilesAsync(IEnumerable<string> urls, Action callback, Action callbackOnError = null)
    {
        // Get the assembly name of the calling method: 
        // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
        // that is executed immediately after the one where the URI is defined! Be careful
        // when moving the following line of code.
        string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
        List<string> urlsAsList = (urls is List<string> ? (List<string>)urls : new List<string>(urls));
        _webResourcesManager.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
    }

    public static void LoadJavaScriptFilesAsync(IEnumerable<ResourceFile> resourceFiles, Action callback, Action callbackOnError = null)
    {
        // Get the assembly name of the calling method: 
        // IMPORTANT: the call to the "GetCallingAssembly" method must be done in the method
        // that is executed immediately after the one where the URI is defined! Be careful
        // when moving the following line of code.
        string callerAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
        List<string> urlsAsList = new List<string>();
        foreach (var resourceFile in resourceFiles)
        {
            //add the key to the dictionary and add the url to the list:
            if (!_jsScriptFileKeys.Contains(resourceFile.Key))
            {
                _jsScriptFileKeys.Add(resourceFile.Key);
                urlsAsList.Add(resourceFile.Url);
            }
        }
        _webResourcesManager.LoadJavaScriptFiles(urlsAsList, callerAssemblyName, callback, callbackOnError);
    }

    /// <summary>
    /// Adds a 'link' tag to the HTML page and waits for the CSS file to finish loading.
    /// </summary>
    /// <param name="url">The URL of the CSS file, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
    /// <returns>Nothing</returns>
    public static Task<object> LoadCssFile(string url)
    {
        var t = new TaskCompletionSource<object>();
        _webResourcesManager.LoadCssFile(url, () => t.SetResult(null));
        return t.Task;
    }

    public static Task<object> LoadCssFile(ResourceFile resourceFile)
    {
        if (!_cssFileKeys.Contains(resourceFile.Key))
        {
            var t = new TaskCompletionSource<object>();
            _webResourcesManager.LoadCssFile(resourceFile.Url, () => t.SetResult(null));
            _cssFileKeys.Add(resourceFile.Key);
            return t.Task;
        }

        var task = new TaskCompletionSource<object>();
        task.SetResult(null);
        return task.Task;
    }

    /// <summary>
    /// Adds 'link' tags to the HTML page and waits for the CSS files to finish loading.
    /// </summary>
    /// <param name="urls">The URL of the CSS files, with the syntax ms-appx:///AssemblyName/Folder/FileName.css or /AssemblyName;component/Folder/FileName.css</param>
    /// <param name="callback">The method that is called when the CSS files have finished loading.</param>
    public static void LoadCssFilesAsync(IEnumerable<string> urls, Action callback)
    {
        List<string> urlsAsList = (urls is List<string> ? (List<string>)urls : new List<string>(urls));
        _webResourcesManager.LoadCssFiles(urlsAsList, callback);
    }

    public static void LoadCssFilesAsync(IEnumerable<ResourceFile> resourceFiles, Action callback)
    {
        List<string> urlsAsList = new List<string>();
        foreach (var resourceFile in resourceFiles)
        {
            //add the key to the dictionary and add the url to the list:
            if (!_cssFileKeys.Contains(resourceFile.Key))
            {
                _cssFileKeys.Add(resourceFile.Key);
                urlsAsList.Add(resourceFile.Url);
            }
        }
        _webResourcesManager.LoadCssFiles(urlsAsList, callback);
    }

    internal static void ResetLoadedFilesDictionaries() => _webResourcesManager.ResetLoadedFilesDictionaries();

    public class ResourceFile
    {
        public ResourceFile(string key, string url)
        {
            Key = key;
            Url = url;
        }

        /// <summary>
        /// The Key associated with the file. This is used to avoid loading a file multiple times or loading multiple files with the same purpose (for example JQuery).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The path to the file.
        /// </summary>
        public string Url { get; set; }
    }

    private sealed class WebResourcesManager
    {
        private readonly Dictionary<string, List<(Action, Action)>> _pendingJSFile = new();
        private readonly HashSet<string> _loadedFiles = new();

        public void LoadJavaScriptFile(string url, string callerAssemblyName, Action onSuccess, Action onError = null)
        {
            string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(url);
            if (_loadedFiles.Contains(html5Path))
            {
                onSuccess?.Invoke();
            }
            else if (_pendingJSFile.TryGetValue(html5Path, out List<(Action, Action)> callbacks))
            {
                callbacks.Add((onSuccess, onError));
            }
            else
            {
                _pendingJSFile.Add(html5Path, new List<(Action, Action)> { (onSuccess, onError) });
                string sSuccessAction = GetVariableStringForJS((Action<object>)LoadJavaScriptFileSuccess);
                string sFailureAction = GetVariableStringForJS((Action<object>)LoadJavaScriptFileFailure);
                ExecuteJavaScriptVoid(
    $@"// Add the script tag to the head
var filePath = {GetVariableStringForJS(html5Path)};
var head = document.getElementsByTagName('head')[0];
var script = document.createElement('script');
script.type = 'text/javascript';
script.src = filePath;
// Then bind the event to the callback function
// There are several events for cross browser compatibility.
if(script.onreadystatechange != undefined) {{
script.onreadystatechange = {sSuccessAction};
}} else {{
script.onload = function () {{ {sSuccessAction}(filePath) }};
script.onerror = function () {{ {sFailureAction}(filePath) }};
}}

// Fire the loading
head.appendChild(script);");
            }
        }

        private void LoadJavaScriptFileSuccess(object jsArgument)
        {
            // using an Interop call instead of jsArgument.ToString because it causes errors in OpenSilver.
            string loadedFileName = ExecuteJavaScriptString(GetVariableStringForJS(jsArgument));

            foreach ((Action onSuccess, _) in _pendingJSFile[loadedFileName])
            {
                onSuccess?.Invoke();
            }

            _loadedFiles.Add(loadedFileName);
            _pendingJSFile.Remove(loadedFileName);
        }

        private void LoadJavaScriptFileFailure(object jsArgument)
        {
            // using an Interop call instead of jsArgument.ToString because it causes errors in OpenSilver.
            string loadedFileName = ExecuteJavaScriptString(GetVariableStringForJS(jsArgument));

            foreach ((_, Action onError) in _pendingJSFile[loadedFileName])
            {
                onError?.Invoke();
            }
            _pendingJSFile.Remove(loadedFileName);
        }

        internal void LoadJavaScriptFiles(List<string> urls, string callerAssemblyName, Action onCompleted, Action onError = null)
        {
            if (urls.Count > 0)
            {
                LoadJavaScriptFile(urls[0], callerAssemblyName, () =>
                {
                    urls.Remove(urls[0]);
                    LoadJavaScriptFiles(urls, callerAssemblyName, onCompleted, onError);
                },
                () => onError?.Invoke());
            }
            else
            {
                onCompleted?.Invoke();
            }
        }

        internal void LoadCssFile(string url, Action callback)
        {
            string html5Path = INTERNAL_UriHelper.ConvertToHtml5Path(url);

            string sHtml5Path = GetVariableStringForJS(html5Path);
            string sCallback = GetVariableStringForJS(callback);
            ExecuteJavaScriptVoid(
$@"// Add the link tag to the head
var head = document.getElementsByTagName('head')[0];
var link = document.createElement('link');
link.rel  = 'stylesheet';
link.type = 'text/css';
link.href = {sHtml5Path};
link.media = 'all';

// Fire the loading
head.appendChild(link);

// Some browsers do not support the 'onload' event of the 'link' element,
// therefore we use the 'onerror' event of the 'img' tag instead, which is always triggered:
var img = document.createElement('img');
img.onerror = {sCallback};
img.src = {sHtml5Path};");
        }

        internal void LoadCssFiles(List<string> urls, Action onCompleted)
        {
            if (urls.Count > 0)
            {
                LoadCssFile(urls[0], () =>
                {
                    urls.Remove(urls[0]);
                    LoadCssFiles(urls, onCompleted);
                });
            }
            else
            {
                onCompleted?.Invoke();
            }
        }

        internal void ResetLoadedFilesDictionaries()
        {
            _pendingJSFile.Clear();
            _loadedFiles.Clear();
        }
    }
}