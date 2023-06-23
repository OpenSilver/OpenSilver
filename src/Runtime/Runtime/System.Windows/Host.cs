
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
using System.Windows.Browser;
using System.Windows.Interop;
using CSHTML5.Internal;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public class Host
    {
        private readonly bool _hookupEvents;
        private readonly JavaScriptCallback _hashChangeCallback;
        private Content _content;
        private Settings _settings;
        private string _navigationState;
        private Dictionary<string, string> _initParams;

        public Host() : this(false) { }

        internal Host(bool hookupEvents)
        {
            _hookupEvents = hookupEvents;

            _navigationState = GetBrowserNavigationState();
            _hashChangeCallback = JavaScriptCallback.Create(OnNavigationChanged, true);
            OpenSilver.Interop.ExecuteJavaScriptVoid(
                $"window.addEventListener('hashchange', {CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_hashChangeCallback)}, false)");
        }

        /// <summary>
        /// Gets the "Content" sub-object of this Host.
        /// </summary>
        public Content Content => _content ??= new Content(_hookupEvents);

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
        public Uri Source => new Uri(OpenSilver.Interop.ExecuteJavaScriptString("window.location.origin", false));

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
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                OpenSilver.Interop.ExecuteJavaScriptVoid($"window.location.hash = {CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(value)}");
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

        private string GetBrowserNavigationState()
        {
            string state = HttpUtility.UrlDecode(OpenSilver.Interop.ExecuteJavaScriptString("location.hash")) ?? string.Empty;

            if (state.Length > 0 && state[0] == '#')
            {
                state = state.Substring(1);
            }

            return state;
        }

        /// <summary>
        /// Gets the initialization parameters that were passed as part of HTML initialization
        /// of a Silverlight plug-in.
        /// </summary>
        /// <returns>
        /// The set of initialization parameters, as a dictionary with key strings and value
        /// strings.
        /// </returns>
        public IDictionary<string, string> InitParams => _initParams ?? (_initParams = ParseInitParams());

        private Dictionary<string, string> ParseInitParams()
        {
            const string InitParamsName = "InitParams";

            var initParams = new Dictionary<string, string>();

            Application app = Application.Current;
            if (app != null && app.AppParams.TryGetValue(InitParamsName, out string initParamsString))
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
    }
}
