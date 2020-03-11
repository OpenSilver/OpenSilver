

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