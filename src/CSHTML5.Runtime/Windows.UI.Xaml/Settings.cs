
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CSHTML5.Internal;

namespace System
{
    public class Settings
    {
        private bool _EnablePerformanceLogging;

        public Settings()
        {
            // Default values:
            DefaultSoapCredentialsMode = CredentialsMode.Disabled;
            EnableBindingErrorsLogging = false;
            EnableBindingErrorsThrowing = false;
        }

        public CredentialsMode DefaultSoapCredentialsMode { get; set; }

        public bool EnableBindingErrorsLogging { get; set; }

        public bool EnableBindingErrorsThrowing { get; set; }

        public bool EnablePerformanceLogging
        {
            get { return _EnablePerformanceLogging; }
            set
            {
                _EnablePerformanceLogging = value;
                INTERNAL_VisualTreeManager.EnablePerformanceLogging = true;

            }
        }

        private bool _enableOptimizationWhereCollapsedControlsAreNotLoaded;

        public bool EnableOptimizationWhereCollapsedControlsAreNotLoaded
        {
            get { return _enableOptimizationWhereCollapsedControlsAreNotLoaded; }
            set
            {
                _enableOptimizationWhereCollapsedControlsAreNotLoaded = value;
                INTERNAL_VisualTreeManager.EnableOptimizationWhereCollapsedControlsAreNotLoaded = true;
            }
        }

    }
}
