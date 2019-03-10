
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
    public sealed class GradientStopCollection : PresentationFrameworkCollection<GradientStop>
#else
    public sealed class GradientStopCollection : List<GradientStop>
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