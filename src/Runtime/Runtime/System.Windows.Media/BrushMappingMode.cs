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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Specifies the coordinate system used by a System.Windows.Media.Brush.
    /// </summary>
    public enum BrushMappingMode
    {
        /// <summary>
        /// The coordinate system is not relative to a bounding box. Values are interpreted
        /// directly in local space.
        /// </summary>
        Absolute = 0,
       
        /// <summary>
        /// The coordinate system is relative to a bounding box: 0 indicates 0 percent
        /// of the bounding box, and 1 indicates 100 percent of the bounding box. For
        /// example, (0.5, 0.5) describes a point in the middle of the bounding box,
        /// and (1, 1) describes a point at the bottom right of the bounding box.
        /// </summary>
        RelativeToBoundingBox = 1,
    }
}
