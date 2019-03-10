
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;

#if MIGRATION
namespace System.Windows // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#else
namespace Windows.UI.Xaml // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
{
    public class Host
    {
        Content _content;

        Settings _settings;

        /// <summary>
        /// Gets the "Content" sub-object of this Host.
        /// </summary>
        public Content Content
        {
            get
            {
                if (_content == null)
                    _content = new Content();
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

#if WORKINPROGRESS
        #region Not supported yet
        
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
            get { return null; }
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
     
        #endregion
#endif
    }
}
