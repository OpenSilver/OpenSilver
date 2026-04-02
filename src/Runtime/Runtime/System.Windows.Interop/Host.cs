
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

using System.Collections.Generic;
using System.Diagnostics;
using OpenSilver.Internal;

namespace System.Windows.Interop;

public class Host : IResizeObserverListener
{
    private readonly Application _app;
    private readonly IDisposable _resizeObserver;
    private Content _content;
    private Settings _settings;
    private string _navigationState;
    private Dictionary<string, string> _initParams;

    internal Host(Application app)
    {
        Debug.Assert(app is not null);

        _app = app;
        _navigationState = GetBrowserNavigationState();
        _resizeObserver = ResizeObserver.Observe(app.GetRootDiv(), this);
        DOMEvents.Window.AddEventListener("hashchange", OnNavigationChanged);
        DOMEvents.Document.AddEventListener("fullscreenchange", OnFullScreenChanged);
    }

    /// <summary>
    /// Gets the "Content" sub-object of this Host.
    /// </summary>
    public Content Content
    {
        get
        {
            if (_content is null)
            {
                _content = new Content();

                if (ResizeObserver.GetCurrentSize(_app.GetRootDiv()) is Size size)
                {
                    _content.SetSize(size);
                }
            }

            return _content;
        }
    }

    /// <summary>
    /// Gets the "Settings" sub-object of this tHost.
    /// </summary>
    public Settings Settings => _settings ??= new Settings();

    /// <summary>
    /// Gets the URI of the package or XAML file that specifies the XAML content
    /// to render.
    /// </summary>
    /// <returns>
    /// The URI of the package, XAML file, or XAML scripting tag that contains the
    /// content to load into the Silverlight plug-in.
    /// </returns>
    public Uri Source => new Uri(OpenSilver.Interop.ExecuteJavaScriptString("osjs.host.origin", false));

    /// <summary>
    /// Gets or sets a URI fragment that represents the current navigation state.
    /// </summary>
    /// <returns>
    /// A URI fragment that represents the current navigation state.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// When setting this property, the specified value is null.
    /// </exception>
    public string NavigationState
    {
        get => _navigationState;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"osjs.host.navigationState = {OpenSilver.Interop.GetVariableStringForJS(value)}");
        }
    }

    /// <summary>
    /// Occurs when the <see cref="NavigationState"/> property
    /// changes value.
    /// </summary>
    public event EventHandler<NavigationStateChangedEventArgs> NavigationStateChanged;

    private void OnNavigationChanged()
    {
        string state = GetBrowserNavigationState();

        string previousNavigationState = _navigationState;
        _navigationState = state;

        NavigationStateChanged?.Invoke(this, new NavigationStateChangedEventArgs(previousNavigationState, state));
    }

    private string GetBrowserNavigationState() => OpenSilver.Interop.ExecuteJavaScriptString("osjs.host.navigationState");

    /// <summary>
    /// Gets the initialization parameters that were passed as part of HTML initialization
    /// of a Silverlight plug-in.
    /// </summary>
    /// <returns>
    /// The set of initialization parameters, as a dictionary with key strings and value
    /// strings.
    /// </returns>
    public IDictionary<string, string> InitParams => _initParams ??= ParseInitParams();

    private Dictionary<string, string> ParseInitParams()
    {
        const string InitParamsName = "InitParams";

        var initParams = new Dictionary<string, string>();

        if (_app.AppParams.TryGetValue(InitParamsName, out string initParamsString))
        {
            foreach (string p in initParamsString.Split(','))
            {
                string key;
                string value;

                int idx = p.IndexOf('=');
                if (idx > -1)
                {
                    key = p.Substring(0, idx).Trim();
                    value = p.Substring(idx + 1).Trim();
                }
                else
                {
                    key = p.Trim();
                    value = string.Empty;
                }

                if (!string.IsNullOrEmpty(key))
                {
                    initParams[key] = value;
                }
            }
        }

        return initParams;
    }

    private void OnFullScreenChanged() => Content.FireFullScreenChanged();

    void IResizeObserverListener.OnSizeChanged(Size size)
    {
        Content.SetSize(size);
        Content.FireResized();
    }
}
