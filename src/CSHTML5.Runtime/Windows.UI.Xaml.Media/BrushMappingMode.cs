
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
