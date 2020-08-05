

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


        private bool _enablePerformanceLogging;
        public bool EnablePerformanceLogging
        {
            get { return _enablePerformanceLogging; }
            set
            {
                _enablePerformanceLogging = value;
                INTERNAL_VisualTreeManager.EnablePerformanceLogging = value;
            }
        }

        /// <summary>
        /// When True, do not apply the CSS properties of the UI elements that are not visible.
        /// Those property are applied later, when the control becomes visible.
        /// Enabling this option results in improved performance.
        /// </summary>
        private bool _enableOptimizationWhereCollapsedControlsAreNotRendered = true;
        public bool EnableOptimizationWhereCollapsedControlsAreNotRendered
        {
            get { return _enableOptimizationWhereCollapsedControlsAreNotRendered; }
            set
            {
                _enableOptimizationWhereCollapsedControlsAreNotRendered = value;
                INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotRendered = value;
            }
        }

        /// <summary>
        /// When True, do not create the DOM elements for the UI elements that are not visible.
        /// The DOM elements are created later, when the control becomes visible.
        /// Enabling this option results in significantly improved performance if there are many hidden elements.
        /// </summary>
        private bool _enableOptimizationWhereCollapsedControlsAreNotLoaded;
        public bool EnableOptimizationWhereCollapsedControlsAreNotLoaded
        {
            get { return _enableOptimizationWhereCollapsedControlsAreNotLoaded; }
            set
            {
                _enableOptimizationWhereCollapsedControlsAreNotLoaded = value;
                INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotLoaded = value;
            }
        }

        /// <summary>
        /// Wait a few moments before creating the DOM elements for the UI elements that are not visible.
        /// The goal is to give the time to the browser engine to draw the visible ones, in order to improve the perceived performance.
        /// The DOM elements are created immediately after the browser engine has finished drawing (when the UI thread is available).
        /// </summary>
        private bool _enableOptimizationWhereCollapsedControlsAreLoadedLast;
        public bool EnableOptimizationWhereCollapsedControlsAreLoadedLast
        {
            get { return _enableOptimizationWhereCollapsedControlsAreLoadedLast; }
            set
            {
                _enableOptimizationWhereCollapsedControlsAreLoadedLast = value;
                INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreLoadedLast = value;
            }
        }

        private bool _enableProgressiveRendering;
        public bool EnableProgressiveRendering
        {
            get { return _enableProgressiveRendering; }
            set
            {
                _enableProgressiveRendering = value;
                Panel.INTERNAL_ApplicationWideEnableProgressiveRendering = value;

            }
        }

        public bool EnableInvalidPropertyMetadataDefaultValueExceptions { get; set; }

#if WORKINPROGRESS
        public bool EnableAutoZoom { get; set; }
        public bool Windowless { get; private set; }
#endif
    }
}
