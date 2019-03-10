
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


using CSHTML5;
using System;

#if MIGRATION
namespace System.Windows // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#else
namespace Windows.UI.Xaml // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
{
    public class Content
    {
        /// <summary>
        /// Gets the browser-determined height of the content area.
        /// </summary>
        public double ActualHeight
        {
            get
            {
                return Application.Current.MainWindow.ActualHeight;
            }
        }

        /// <summary>
        /// Gets the browser-determined width of the Silverlight content area.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                return Application.Current.MainWindow.ActualWidth;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the page is displaying in full-screen mode.
        /// </summary>
        public bool IsFullScreen
        {
            get
            {
                return Convert.ToBoolean(Interop.ExecuteJavaScript(@"
(function(){
    if (window.IE_VERSION)
    {
        // Internet Explorer:
        return (window.screenTop == 0);
    }
    else
    {
        // Other browsers:
        return (window.innerHeight == screen.height);
    }
}())"));
            }
            set
            {
                if (value)
                {
                    if (!Interop.IsRunningInTheSimulator)
                    {
                        Interop.ExecuteJavaScript(@"
var element = document.body;
var requestMethod = element.requestFullScreen || element.webkitRequestFullScreen || element.mozRequestFullScreen || element.msRequestFullScreen;
if (requestMethod) { // Native full screen.
    requestMethod.call(element);
} else if (typeof window.ActiveXObject !== 'undefined') { // Older IE (< 11)
    var isFullScreen = (window.screenTop == 0);
    if (!isFullScreen)
    {
        var wscript = new ActiveXObject('WScript.Shell');
        if (wscript !== null) {
            wscript.SendKeys('{F11}');
        }
    }
}");
                    }
                    else
                    {
                        MessageBox.Show("Full-screen mode is not supported when running inside the Simulator. Please launch the application in the browser instead.");
                    }
                }
                else
                {
                    Interop.ExecuteJavaScript(@"
var requestMethod = document.exitFullScreen || document.webkitExitFullScreen || document.webkitCancelFullScreen || document.mozCancelFullScreen || document.msExitFullScreen || document.msCancelFullScreen;
if (requestMethod) { // Native exit full screen.
    requestMethod.call(document);
} else if (typeof window.ActiveXObject !== 'undefined') { // Older IE (< 11)
    var isFullScreen = (window.screenTop == 0);
    if (isFullScreen)
    {
        var wscript = new ActiveXObject('WScript.Shell');
        if (wscript !== null) {
            wscript.SendKeys('{F11}');
        }
    }
}");
                }
            }
        }

#if WORKINPROGRESS
        #region Not supported yet

        // Summary:
        //     Gets or sets a value that indicates the behavior of full-screen mode.
        //
        // Returns:
        //     A value that indicates the behavior of full-screen mode.
        //public FullScreenOptions FullScreenOptions { get; set; }

        /// <summary>
        /// Gets the factor by which the current browser window resizes its contents.
        /// </summary>
        /// <returns> 
        /// The zoom setting for the current browser window.
        /// </returns>
        public double ZoomFactor
        {
            get { return 0; }
        }

        // Summary:
        //     Occurs when the hosted Silverlight plug-in either enters or exits full-screen
        //     mode.
        //public event EventHandler FullScreenChanged;

         /// <summary>
         /// Occurs when the System.Windows.Interop.Content.ActualHeight or the System.Windows.Interop.Content.ActualWidth 
         /// of the Silverlight plug-in change.
         /// </summary>
             
        public event EventHandler Resized;

        /// <summary>
        //  Occurs when the zoom setting in the host browser window changes or is initialized.
        /// </summary>
        public event EventHandler Zoomed;
        #endregion
#endif
    }
}
