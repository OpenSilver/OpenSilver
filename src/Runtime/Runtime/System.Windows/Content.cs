

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

using CSHTML5;
using System;

#if !MIGRATION
using Windows.UI.Xaml.Interop;
#endif

#if MIGRATION
namespace System.Windows.Interop
#else
namespace Windows.UI.Xaml // Note: we didn't use the "Interop" namespace to avoid conflicts with CSHTML5.Interop
#endif
{
    public partial class Content
    {
        public Content() : this(false)
        {
        }

        internal Content(bool hookupEvents)
        {
            if (hookupEvents)
            {
                // Hooks the FullScreenChanged event
                CSHTML5.Interop.ExecuteJavaScript($"document.addEventListener('fullscreenchange', {INTERNAL_InteropImplementation.GetVariableStringForJS(new Action(FullScreenChangedCallback))})");

                // Hooks the Resized event
                CSHTML5.Interop.ExecuteJavaScript($"new ResizeSensor(document.getXamlRoot(), {INTERNAL_InteropImplementation.GetVariableStringForJS(new Action(WindowResizeCallback))})");

                // WORKINPROGRESS
                // Add Zoomed event
            }
        }

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
#if OPENSILVER
                    if (!CSHTML5.Interop.IsRunningInTheSimulator_WorkAround)
#else
                    if (!CSHTML5.Interop.IsRunningInTheSimulator)
#endif
                    {
                        CSHTML5.Interop.ExecuteJavaScript(@"
var element = document.body;
var requestMethod = element.requestFullScreen || element.webkitRequestFullScreen || element.mozRequestFullScreen || element.msRequestFullScreen;
if (requestMethod) {
    requestMethod.call(element);
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
if (requestMethod) {
    requestMethod.call(document);
}");
                }
            }
        }

        /// <summary>
        /// Gets the factor by which the current browser window resizes its contents.
        /// </summary>
        /// <returns> 
        /// The zoom setting for the current browser window.
        /// </returns>
        public double ZoomFactor => Convert.ToDouble(CSHTML5.Interop.ExecuteJavaScript("window.devicePixelRatio"));


        /// <summary>
        /// Occurs when the browser enters or exits full-screen mode.
        /// </summary>
        public event EventHandler FullScreenChanged;

        /// <summary>
        /// Occurs when the <see cref="Window"/> gets resized.
        /// </summary>
        public event EventHandler Resized;

        /// <summary>
        /// Called when the full screen mode changes.
        /// Fires the <see cref="FullScreenChanged"/> event.
        /// </summary>
        private void FullScreenChangedCallback()
        {
            FullScreenChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the window gets resized.
        /// Fires the <see cref="Resized"/> event.
        /// </summary>
        private void WindowResizeCallback()
        {
            Resized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets or sets a value that indicates the behavior of full-screen mode.
        /// </summary>
        [OpenSilver.NotImplemented]
        public FullScreenOptions FullScreenOptions { get; set; }

        /// <summary>
        /// Occurs when the zoom setting in the host browser window changes or is initialized.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler Zoomed;
    }
}
