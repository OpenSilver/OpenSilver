

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

#if MIGRATION
using System.Windows.Interop;
#endif

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

        public Host() : this(false) { }

        internal Host(bool hookupEvents)
        {
            this._hookupEvents = hookupEvents;
            this._content = new Content(this._hookupEvents);
        }

        /// <summary>
        /// Gets the "Content" sub-object of this Host.
        /// </summary>
        public Content Content
        {
            get
            {
                if (_content == null)
                    _content = new Content(this._hookupEvents);
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
                    _settings = new Settings();
                return _settings;
            }
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
        ////     Gets the initialization parameters that were passed as part of HTML initialization
        ////     of a Silverlight plug-in.
        ////
        //// Returns:
        ////     The set of initialization parameters, as a dictionary with key strings and
        ////     value strings.
        //public IDictionary<string, string> InitParams { get; }
        ////
        //// Summary:
        ////     Gets a value that indicates whether the hosted Silverlight plug-in has finished
        ////     loading.
        ////
        //// Returns:
        ////     true if the plug-in has finished loading; otherwise, false.
        //public bool IsLoaded { get; }
        ////
        //// Summary:
        ////     Gets or sets a URI fragment that represents the current navigation state.
        ////
        //// Returns:
        ////     A URI fragment that represents the current navigation state.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     When setting this property, the specified value is null.
        //public string NavigationState { get; set; }
        ////
        
        // Summary:
        //     Gets the URI of the package or XAML file that specifies the XAML content
        //     to render.
        //
        // Returns:
        //     The URI of the package, XAML file, or XAML scripting tag that contains the
        //     content to load into the Silverlight plug-in.
        public Uri Source
        {
            get 
            {
                return new Uri(CSHTML5.Interop.ExecuteJavaScript("window.location.origin").ToString());
            }
        }

        //// Summary:
        ////     Occurs when the System.Windows.Interop.SilverlightHost.NavigationState property
        ////     changes value.
        //public event EventHandler<NavigationStateChangedEventArgs> NavigationStateChanged;

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
