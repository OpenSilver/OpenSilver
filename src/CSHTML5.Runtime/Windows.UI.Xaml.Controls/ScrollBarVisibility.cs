
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
