
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

#if MIGRATION
namespace System.Windows // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#else
namespace Windows.UI.Xaml // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
{
    public partial class Host
    {
        private readonly bool _hookupEvents;
        private Content _content;
        private Settings _settings;
        private string _navigationState;
        private Dictionary<string, string> _initParams;

        public Host() : this(false) { }

        internal Host(bool hookupEvents)
        {
            _hookupEvents = hookupEvents;
            _content = new Content(_hookupEvents);

            _navigationState = GetBrowserNavigationState();
            OpenSilver.Interop.ExecuteJavaScript("window.addEventListener('hashchange', $0, false)", (Action)OnNavigationChanged);
        }

        /// <summary>
        /// Gets the "Content" sub-object of this Host.
        /// </summary>
        public Content Content
        {
            get
            {
                if (_content == null)
                {
                    _content = new Content(_hookupEvents);
                }

                return _content;
            }
        }

        /// <summary>
        /// Gets the "Settings" sub-object of this tHost.
        /// </summary>
        public Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }

                return _settings;
            }
        }

        /// <summary>
        /// Gets the URI of the package or XAML file that specifies the XAML content
        /// to render.
        /// </summary>
        /// <returns>
        /// The URI of the package, XAML file, or XAML scripting tag that contains the
        /// content to load into the Silverlight plug-in.
        /// </returns>
        public Uri Source => new Uri(OpenSilver.Interop.ExecuteJavaScript("window.location.origin").ToString());

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

                OpenSilver.Interop.ExecuteJavaScript("window.location.hash = $0", value);
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
            string state = HttpUtility.UrlDecode(Convert.ToString(OpenSilver.Interop.ExecuteJavaScript("location.hash"))) ?? string.Empty;

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

        //// Summary:
        ////     Gets the background color value that was applied to the Silverlight plug-in
        ////     as part of instantiation settings.
        ////
        //// Returns:
        ////     The background color for the Silverlight plug-in.
        //public Color Background { get; }
        ////
        //// Summary:
        ////     Gets a value that indicates whether the hosted Silverlight plug-in has finished
        ////     loading.
        ////
        //// Returns:
        ////     true if the plug-in has finished loading; otherwise, false.
        //public bool IsLoaded { get; }
        //// Summary:
        ////     Returns a value that indicates whether the installed Silverlight plug-in
        ////     supports the specified version.
        ////
        //// Parameters:
        ////   versionStr:
        ////     The version to check, in the form of major.minor.build.revision See Remarks
        ////     for more information about the string form.
        ////
        //// Returns:
        ////     true if the version can be supported by the installation; otherwise, false.
        //public bool IsVersionSupported(string versionStr);
    }
}
