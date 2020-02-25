
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5;
using System;

#if MIGRATION
#if WORKINPROGRESS
namespace System.Windows.Interop
#else
namespace System.Windows // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
#else
namespace Windows.UI.Xaml // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
{
    public partial class Content
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
                return Convert.ToBoolean(CSHTML5.Interop.ExecuteJavaScript(@"
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
                    if (!CSHTML5.Interop.IsRunningInTheSimulator)
                    {
                        CSHTML5.Interop.ExecuteJavaScript(@"
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
                    CSHTML5.Interop.ExecuteJavaScript(@"
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

        public event EventHandler FullScreenChanged;
#endif
    }
}
