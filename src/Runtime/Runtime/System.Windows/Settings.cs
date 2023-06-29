﻿

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
using System.ComponentModel;
using System.Net;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

namespace System
{
    public class Settings
    {
        private readonly Lazy<bool> _windowless = new Lazy<bool>(() =>
        {
            const string WindowlessName = "Windowless";

            Application app = Application.Current;
            if (app != null && app.AppParams.TryGetValue(WindowlessName, out string str)
                && bool.TryParse(str.Trim(), out bool windowless))
            {
                return windowless;
            }

            return false;
        });

        private readonly Lazy<bool> _enableHTMLAccess = new Lazy<bool>(() =>
        {
            const string EnableHTMLAccessName = "EnableHTMLAccess";

            Application app = Application.Current;
            if (app != null && app.AppParams.TryGetValue(EnableHTMLAccessName, out string str)
                && bool.TryParse(str.Trim(), out bool enableHTMLAccess))
            {
                return enableHTMLAccess;
            }

            return true;
        });

        public Settings()
        {
            // Default values:
            DefaultSoapCredentialsMode = CredentialsMode.Disabled;
            EnableBindingErrorsLogging = false;
            EnableBindingErrorsThrowing = false;
            EnableInvalidPropertyMetadataDefaultValueExceptions = true;
            ScrollDebounce = TimeSpan.Zero;
            DefaultResourceLookupMode = InheritanceBehavior.Default;
        }

        public InheritanceBehavior DefaultResourceLookupMode { get; set; }

        public TimeSpan ScrollDebounce { get; set; }

        public CredentialsMode DefaultSoapCredentialsMode { get; set; }

        public bool EnableBindingErrorsLogging { get; set; }

        public bool EnableBindingErrorsThrowing { get; set; }

        public bool EnableWebRequestsLogging { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether or not to use the resizeSensor.js library
        /// instead of the ResizeObserver API to listen for size change event in Html.
        /// </summary>
        /// <remarks>Should probably be removed once we get the ResizeObserver fully working.</remarks>
        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool UseResizeSensor { get; set; }

        public bool EnableInteropLogging
        {
            get { return INTERNAL_ExecuteJavaScript.EnableInteropLogging; }
            set { INTERNAL_ExecuteJavaScript.EnableInteropLogging = value; }
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
        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EnableOptimizationWhereCollapsedControlsAreNotLoaded { get; set; }

        /// <summary>
        /// Wait a few moments before creating the DOM elements for the UI elements that are not visible.
        /// The goal is to give the time to the browser engine to draw the visible ones, in order to improve the perceived performance.
        /// The DOM elements are created immediately after the browser engine has finished drawing (when the UI thread is available).
        /// </summary>
        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EnableOptimizationWhereCollapsedControlsAreLoadedLast { get; set; }

        /// <summary>
        /// Gets or sets the number of children in a Panel to render progressively in a batch.
        /// Setting this option can improve performance.
        /// Value of 0 or less means progressive rendering is disabled.
        /// The default value is 0.
        /// </summary>
        /// <remarks>
        /// A value close to 1 can break UI in some cases.
        /// </remarks>
        public int ProgressiveRenderingChunkSize
        {
            get { return Panel.GlobalProgressiveRenderingChunkSize; }
            set { Panel.GlobalProgressiveRenderingChunkSize = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use ProgressiveRenderingChunkSize instead.")]
        public bool EnableProgressiveRendering
        {
            get => Panel.GlobalProgressiveRenderingChunkSize > 0;
            set => Panel.GlobalProgressiveRenderingChunkSize = 1;
        }

        public bool EnableInvalidPropertyMetadataDefaultValueExceptions { get; set; }

        /// <summary>
        /// Gets or sets the interval between each refresh of the position an element attached to another (for example Popups or Tooltips attached to a certain element).
        /// Note: A refresh interval too low might negatively impact performances
        /// </summary>
        public TimeSpan PopupMoveDelay
        {
            get { return PopupService.PositionsWatcher.Interval; }
            set { PopupService.PositionsWatcher.Interval = value; }
        }

        /// <summary>
        /// Gets a value that indicates whether the Silverlight plug-in displays as a windowless
        /// plug-in. (Applies to Windows versions of Silverlight only.)
        /// </summary>
        /// <returns>
        /// true if the Silverlight plug-in displays as a windowless plug-in; otherwise, false.
        /// </returns>
        public bool Windowless => _windowless.Value;

        /// <summary>
        /// Gets or sets a value that indicates whether the application was launched from the out-of-browser state.
        /// Specified value can be received from <see cref="Application.IsRunningOutOfBrowser"/> property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsRunningOutOfBrowser { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the Silverlight plug-in will resize
        /// its content based on the current browser zoom setting.
        /// </summary>
        /// <returns>
        /// true if Silverlight responds to the browser zoom setting; otherwise, false. The
        /// default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool EnableAutoZoom { get; set; } = true;

        /// <summary>
        /// Gets or sets a value that indicates whether to use a non-production analysis
        /// visualization mode, which shows areas of a page that are not being GPU accelerated
        /// with a colored overlay. Do not use in production code.
        /// </summary>
        /// <returns>
        /// true if cache visualization is enabled; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool EnableCacheVisualization { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to display the current frame rate
        /// in the hosting browser's status bar. (Microsoft Internet Explorer only.)
        /// </summary>
        /// <returns>
        /// true if the frames-per-second (fps) of the current rendered Silverlight content
        /// will be displayed in the hosting browser's status bar; otherwise, false. The
        /// default is false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool EnableFrameRateCounter { get; set; }
        
        /// <summary>
        /// Gets a value that indicates whether to use graphics processor unit (GPU) hardware
        /// acceleration for cached compositions, which potentially results in graphics optimization.
        /// </summary>
        /// <returns>
        /// true if hardware acceleration is enabled; otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool EnableGPUAcceleration { get; }

        /// <summary>
        /// Gets a value that indicates whether the Silverlight plug-in allows hosted content
        /// or its runtime to access the HTML DOM.
        /// </summary>
        /// <returns>
        /// true if hosted content can access the browser DOM; otherwise, false.
        /// </returns>
        public bool EnableHTMLAccess => _enableHTMLAccess.Value;

        /// <summary>
        /// Gets or sets a value that indicates whether to show the areas of the Silverlight
        /// plug-in that are being redrawn each frame.
        /// </summary>
        /// <returns>
        /// true if the areas of the plug-in that are being redrawn each frame are shown;
        /// otherwise, false.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool EnableRedrawRegions { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of frames to render per second.
        /// </summary>
        /// <returns>
        /// An integer value that specifies the maximum number of frames to render per second.
        /// The default value is 60.
        /// </returns>
        [OpenSilver.NotImplemented]
        public int MaxFrameRate { get; set; }
    }
}
