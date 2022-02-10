

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


using CSHTML5.Internal;
using System;
using System.ComponentModel;
using System.Net;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace System
{
    public partial class Settings
    {

        public Settings()
        {
            // Default values:
            DefaultSoapCredentialsMode = CredentialsMode.Disabled;
            EnableBindingErrorsLogging = false;
            EnableBindingErrorsThrowing = false;
            EnableInvalidPropertyMetadataDefaultValueExceptions = true;
        }

        public CredentialsMode DefaultSoapCredentialsMode { get; set; }

        public bool EnableBindingErrorsLogging { get; set; }

        public bool EnableBindingErrorsThrowing { get; set; }

        public bool EnableWebRequestsLogging { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether or not to use the resizeSensor.js library
        /// instead of the ResizeObserver API to listen for size change event in Html.
        /// </summary>
        /// <remarks>Should probably be removed once we get the ResizeObserver fully working.</remarks>
        public bool UseResizeSensor { get; set; }



        public bool EnableInteropLogging
        {
            get { return INTERNAL_SimulatorExecuteJavaScript.EnableInteropLogging; }
            set { INTERNAL_SimulatorExecuteJavaScript.EnableInteropLogging = value; }
        }

        public bool EnablePerformanceLogging
        {
            get { return INTERNAL_VisualTreeManager.EnablePerformanceLogging; }
            set { INTERNAL_VisualTreeManager.EnablePerformanceLogging = value; }
        }

        /// <summary>
        /// When True, do not apply the CSS properties of the UI elements that are not visible.
        /// Those property are applied later, when the control becomes visible.
        /// Enabling this option results in improved performance.
        /// </summary>
        public bool EnableOptimizationWhereCollapsedControlsAreNotRendered
        {
            get { return INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotRendered; }
            set { INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotRendered = value; }
        }

        /// <summary>
        /// When True, do not create the DOM elements for the UI elements that are not visible.
        /// The DOM elements are created later, when the control becomes visible.
        /// Enabling this option results in significantly improved performance if there are many hidden elements.
        /// </summary>
        public bool EnableOptimizationWhereCollapsedControlsAreNotLoaded
        {
            get { return INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotLoaded; }
            set { INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotLoaded = value; }
        }

        /// <summary>
        /// Wait a few moments before creating the DOM elements for the UI elements that are not visible.
        /// The goal is to give the time to the browser engine to draw the visible ones, in order to improve the perceived performance.
        /// The DOM elements are created immediately after the browser engine has finished drawing (when the UI thread is available).
        /// </summary>
        public bool EnableOptimizationWhereCollapsedControlsAreLoadedLast
        {
            get { return INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreLoadedLast; }
            set { INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreLoadedLast = value; }
        }

        public bool EnableProgressiveRendering
        {
            get { return Panel.INTERNAL_ApplicationWideEnableProgressiveRendering; }
            set { Panel.INTERNAL_ApplicationWideEnableProgressiveRendering = value; }
        }

        public bool EnableInvalidPropertyMetadataDefaultValueExceptions { get; set; }

        /// <summary>
        /// Gets or sets the interval between each refresh of the position an element attached to another (for example Popups or Tooltips attached to a certain element).
        /// Note: A refresh interval too low might negatively impact performances
        /// </summary>
        public TimeSpan PopupMoveDelay
        {
            get { return Window.Current.INTERNAL_PositionsWatcher.INTERNAL_WatchInterval; }
            set { Window.Current.INTERNAL_PositionsWatcher.INTERNAL_WatchInterval = value; }
        }

        [OpenSilver.NotImplemented]
        public bool EnableAutoZoom { get; set; }
        [OpenSilver.NotImplemented]
        public bool Windowless { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the application was launched from the out-of-browser state.
        /// Specified value can be received from <see cref="Application.IsRunningOutOfBrowser"/> property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsRunningOutOfBrowser { get; set; }
    }
}
