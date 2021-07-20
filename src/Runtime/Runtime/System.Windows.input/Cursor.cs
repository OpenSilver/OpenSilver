

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


namespace System.Windows.Input
{
    /// <summary>
    /// Represents the image used for the mouse pointer.
    /// </summary>
    public sealed partial class Cursor : IDisposable
    {
        private static string[] HtmlCursors { get; }

        static Cursor()
        {
            HtmlCursors = new string[Cursors._cursorTypeCount];
            HtmlCursors[(int)CursorType.None] = "none";
            HtmlCursors[(int)CursorType.No] = "not-allowed";
            HtmlCursors[(int)CursorType.Arrow] = "default";
            HtmlCursors[(int)CursorType.AppStarting] = "progress";
            HtmlCursors[(int)CursorType.Cross] = "crosshair";
            HtmlCursors[(int)CursorType.Help] = "help";
            HtmlCursors[(int)CursorType.IBeam] = "text";
            HtmlCursors[(int)CursorType.SizeAll] = "move";
            HtmlCursors[(int)CursorType.SizeNESW] = "nesw-resize";
            HtmlCursors[(int)CursorType.SizeNS] = "ns-resize";
            HtmlCursors[(int)CursorType.SizeNWSE] = "nwse-resize";
            HtmlCursors[(int)CursorType.SizeWE] = "ew-resize";
            HtmlCursors[(int)CursorType.UpArrow] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.Wait] = "wait";
            HtmlCursors[(int)CursorType.Hand] = "pointer";
            HtmlCursors[(int)CursorType.Pen] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollNS] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollWE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollAll] = "all-scroll";
            HtmlCursors[(int)CursorType.ScrollN] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollS] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollW] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollNW] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollNE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollSW] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollSE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ArrowCD] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.Stylus] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.Eraser] = "auto"; // not implemented
        }

        /// <summary>
        /// Constructor for Standard Cursors, needn't be public as Stock Cursors
        /// are exposed in Cursors class.
        /// </summary>
        internal Cursor(CursorType cursorType)
        {
            if (IsValidCursorType(cursorType))
            {
                _cursorType = cursorType;
            }
            else
            {
                throw new ArgumentException(string.Format("'{0}' cursor type is not valid.", cursorType));
            }
        }

        /// <summary>
        /// CursorType - Cursor Type Enumeration
        /// </summary>
        /// <value></value>
        internal CursorType CursorType
        {
            get
            {
                return _cursorType;
            }
        }

        private static bool IsValidCursorType(CursorType cursorType)
        {
            return ((int)cursorType >= (int)CursorType.None && (int)cursorType <= (int)CursorType.Eraser);
        }

        /// <summary>
        /// Releases the resources used by the <see cref="Cursor"/> class.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Returns the string representation of the <see cref="Cursor"/>.
        /// </summary>
        /// <returns>
        /// The string representation of the cursor. This corresponds to the active <see cref="Cursors"/>
        /// property name.
        /// </returns>
        public override string ToString()
        {
            // Get the string representation fo the cursor type enumeration.
            return Enum.GetName(typeof(CursorType), _cursorType);
        }

        internal string ToHtmlString()
        {
            return HtmlCursors[(int)_cursorType];
        }

        public override bool Equals(object obj)
        {
            return obj is Cursor cursor && CursorType == cursor.CursorType && _cursorType == cursor._cursorType;
        }

        public override int GetHashCode()
        {
            int hashCode = -1140138637;
            hashCode = hashCode * -1521134295 + CursorType.GetHashCode();
            hashCode = hashCode * -1521134295 + _cursorType.GetHashCode();
            return hashCode;
        }

        private CursorType _cursorType = CursorType.None;
    }
}