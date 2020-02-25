﻿
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

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a collection of GradientStop objects that can be individually
    /// accessed by index.
    /// </summary>
#if WORKINPROGRESS
    public sealed partial class GradientStopCollection : PresentationFrameworkCollection<GradientStop>
#else
    public sealed partial class GradientStopCollection : List<GradientStop>
#endif
    {
#if WORKINPROGRESS
        internal Brush INTERNAL_ParentBrush;


        //// Summary:
        ////     Initializes a new instance of the GradientStopCollection class.
        //public GradientStopCollection();
        internal override void AddInternal(GradientStop value)
        {
            base.AddInternal(value);
            value.INTERNAL_ParentBrush = INTERNAL_ParentBrush;
        }
#endif
    }
}