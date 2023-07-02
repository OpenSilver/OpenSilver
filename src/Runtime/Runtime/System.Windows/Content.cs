

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
using CSHTML5;
using CSHTML5.Internal;

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
        private readonly JavaScriptCallback _fullscreenchangeCallback;
        private readonly JavaScriptCallback _windowresizeCallback;

        public Content() : this(null)
        {
        }

        internal Content(Application app)
        {
            if (app is not null)
            {
                _fullscreenchangeCallback = JavaScriptCallback.Create(FullScreenChangedCallback, true);
                _windowresizeCallback = JavaScriptCallback.Create(WindowResizeCallback, true);

                // Hooks the FullScreenChanged event
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"document.addEventListener('fullscreenchange', {INTERNAL_InteropImplementation.GetVariableStringForJS(_fullscreenchangeCallback)})");

                // Hooks the Resized event
                string sRoot = INTERNAL_InteropImplementation.GetVariableStringForJS(app.GetRootDiv());
                OpenSilver.Interop.ExecuteJavaScriptVoid(
                    $"new ResizeSensor({sRoot}, {INTERNAL_InteropImplementation.GetVariableStringForJS(_windowresizeCallback)})");

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
                return OpenSilver.Interop.ExecuteJavaScriptBoolean(
@"(function() {
 if (window.IE_VERSION) return (window.screenTop == 0);
 else return (window.innerHeight == screen.height);
}())");
            }
            set
            {
                if (value)
                {
                    if (!OpenSilver.Interop.IsRunningInTheSimulator)
                    {
                        OpenSilver.Interop.ExecuteJavaScriptVoid(@"
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
                    OpenSilver.Interop.ExecuteJavaScriptVoid(@"
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
        public double ZoomFactor => OpenSilver.Interop.ExecuteJavaScriptDouble("window.devicePixelRatio", false);


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
            FullScreenChanged?.Invoke(Application.Current?.RootVisual, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the window gets resized.
        /// Fires the <see cref="Resized"/> event.
        /// </summary>
        private void WindowResizeCallback()
        {
            Resized?.Invoke(Application.Current?.RootVisual, EventArgs.Empty);
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
