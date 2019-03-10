
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies the visibility of a scrollbar within a ScrollViewer control.
    /// </summary>
    public enum ScrollBarVisibility
    {
        /// <summary>
        /// [DOESN'T WORK YET, BEHAVES LIKE Hidden]
        /// A ScrollBar does not appear even when the viewport cannot display all of
        /// the content. The dimension of the content is set to the corresponding dimension
        /// of the ScrollViewer parent. For a horizontal ScrollBar, the width of the
        /// content is set to the ViewportWidth of the ScrollViewer. For a vertical ScrollBar,
        /// the height of the content is set to the ViewportHeight of the ScrollViewer.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// A ScrollBar appears and the dimension of the ScrollViewer is applied to the
        /// content when the viewport cannot display all of the content. For a horizontal
        /// ScrollBar, the width of the content is set to the ViewportWidth of the ScrollViewer.
        /// For a vertical ScrollBar, the height of the content is set to the ViewportHeight
        /// of the ScrollViewer.
        /// </summary>
        Auto = 1,

        /// <summary>
        /// A ScrollBar does not appear even when the viewport cannot display all of
        /// the content. The dimension of the ScrollViewer is not applied to the content.
        /// </summary>
        Hidden = 2,

        /// <summary>
        /// A ScrollBar always appears. The dimension of the ScrollViewer is applied
        /// to the content. For a horizontal ScrollBar, the width of the content is set
        /// to the ViewportWidth of the ScrollViewer. For a vertical ScrollBar, the height
        /// of the content is set to the ViewportHeight of the ScrollViewer.
        /// </summary>
        Visible = 3,
    }
}
