
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

namespace System.Windows.Controls
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
