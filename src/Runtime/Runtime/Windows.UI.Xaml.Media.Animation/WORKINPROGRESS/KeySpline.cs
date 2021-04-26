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
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    [OpenSilver.NotImplemented]
    public sealed partial class KeySpline : DependencyObject
    {
        /// <summary>
        /// Gets or sets the first control point used to define a Bezier curve that describes a <see cref="T:System.Windows.Media.Animation.KeySpline" />.
        /// </summary>
        /// <returns>
        /// The first control point used to define a Bezier curve that describes a <see cref="T:System.Windows.Media.Animation.KeySpline" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Point ControlPoint1 { get; set; }

        /// <summary>
        /// Gets or sets the second control point used to define a Bezier curve that describes a <see cref="T:System.Windows.Media.Animation.KeySpline" />.
        /// </summary>
        /// <returns>
        /// The second control point used to define a Bezier curve that describes a <see cref="T:System.Windows.Media.Animation.KeySpline" />.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Point ControlPoint2 { get; set; }
    }
#endif
}
