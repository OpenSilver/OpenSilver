
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Specifies the repeat mode that a Windows.UI.Xaml.Media.Animation.RepeatBehavior
    /// raw value represents.
    /// </summary>
    public enum RepeatBehaviorType
    {
        /// <summary>
        /// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        /// the timeline should repeat for a fixed number of complete runs.
        /// </summary>
        Count = 0,
        //// <summary>
        //// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        //// the timeline should repeat for a time duration, which might result in an
        //// animation terminating part way through.
        //// </summary>
        //Duration = 1, //todo: implement the duration for repeatBehavior.
        /// <summary>
        /// The Windows.UI.Xaml.Media.Animation.RepeatBehavior represents a case where
        /// the timeline should repeat indefinitely.
        /// </summary>
        Forever = 2,
    }
}
