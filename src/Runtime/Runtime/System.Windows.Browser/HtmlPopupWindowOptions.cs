
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

using System.Text;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides options to control pop-up windows.
    /// </summary>
	[OpenSilver.NotImplemented]
    public sealed class HtmlPopupWindowOptions
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HtmlPopupWindowOptions"/> class.
        /// </summary>
        public HtmlPopupWindowOptions() { }

        /// <summary>
        /// Gets or sets a value that determines whether the Internet Explorer links toolbar
        /// or Firefox personal/bookmarks toolbar is shown in the pop-up window.
        /// </summary>
        /// <returns>
        /// true if Internet Explorer links toolbar or Firefox personal/bookmarks toolbar
        /// is shown in the pop-up window; otherwise, false.
        /// </returns>
        public bool Directories { get; set; }

        /// <summary>
        /// Gets or sets the height of a pop-up window.
        /// </summary>
        /// <returns>
        /// Height of the pop-up window, in pixels.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The height is set to a value that is less than zero.
        /// </exception>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the position of the left edge of a pop-up window.
        /// </summary>
        /// <returns>
        /// Horizontal distance from the left edge of the browser's document space to the
        /// left edge of the pop-up window.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The position is set to a value that is less than zero.
        /// </exception>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the URL of a pop-up window.
        /// </summary>
        /// <returns>
        /// The URL of the pop-up window.
        /// </returns>
        public bool Location { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the menu bar is visible in a pop-up
        /// window.
        /// </summary>
        /// <returns>
        /// true if the menu bar is visible in the pop-up window; otherwise, false.
        /// </returns>
        public bool Menubar { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether a pop-up window can be resized.
        /// </summary>
        /// <returns>
        /// true if the pop-up window can be resized; otherwise, false.
        /// </returns>
        public bool Resizeable { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether scroll bars are visible in a pop-up
        /// window.
        /// </summary>
        /// <returns>
        /// true if scroll bars may appear in the pop-up window; otherwise, false.
        /// </returns>
        public bool Scrollbars { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the status bar is visible in a pop-up
        /// window.
        /// </summary>
        /// <returns>
        /// true if the status bar is visible in the pop-up window; otherwise, false.
        /// </returns>
        public bool Status { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the toolbar is visible in a pop-up
        /// window.
        /// </summary>
        /// <returns>
        /// true if the toolbar is visible in the pop-up window; otherwise, false.
        /// </returns>
        public bool Toolbar { get; set; }

        /// <summary>
        /// Gets or sets the position of the top edge of a pop-up window.
        /// </summary>
        /// <returns>
        /// Vertical distance from the top edge of the browser's document space to the top
        /// edge of the pop-up window.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The position is set to a value that is less than zero.
        /// </exception>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the width of a pop-up window.
        /// </summary>
        /// <returns>
        /// Width of the pop-up window, in pixels.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The width is set to a value that is less than zero.
        /// </exception>
        public int Width { get; set; }

        internal string ToFeaturesString()
        {
            var stringBuilder = new StringBuilder(50);
            stringBuilder.Append("width=").Append(this.Width);
            stringBuilder.Append(",height=").Append(this.Height);
            stringBuilder.Append(",left=").Append(this.Left);
            stringBuilder.Append(",top=").Append(this.Top);
            stringBuilder.Append(",directories=").Append(this.BoolToYesNo(this.Directories));
            stringBuilder.Append(",location=").Append(this.BoolToYesNo(this.Location));
            stringBuilder.Append(",menubar=").Append(this.BoolToYesNo(this.Menubar));
            stringBuilder.Append(",resizeable=").Append(this.BoolToYesNo(this.Resizeable));
            stringBuilder.Append(",scrollbars=").Append(this.BoolToYesNo(this.Scrollbars));
            stringBuilder.Append(",status=").Append(this.BoolToYesNo(this.Status));
            stringBuilder.Append(",toolbar=").Append(this.BoolToYesNo(this.Toolbar));
            return stringBuilder.ToString();
        }

        private string BoolToYesNo(bool b) => !b ? "no" : "yes";
    }
}
