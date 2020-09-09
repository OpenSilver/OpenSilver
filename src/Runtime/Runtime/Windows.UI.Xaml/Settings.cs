

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CSHTML5.Internal;
#if MIGRATION
using System.Windows.Controls;
#else
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

#if WORKINPROGRESS
        public bool EnableAutoZoom { get; set; }
        public bool Windowless { get; private set; }
#endif
    }
}
