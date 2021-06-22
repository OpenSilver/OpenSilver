

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
    /// Defines a set of default mouse pointer images.
    /// </summary>
    public static class Cursors
    {
        /// <summary>
        /// Represents a special <see cref="Cursor"/> that is invisible.
        /// </summary>
        public static Cursor None
        {
            get => EnsureCursor(CursorType.None);
        }

        /// <summary>
        /// Gets a scroll southwest <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollSW
        {
            get => EnsureCursor(CursorType.ScrollSW);
        }

        /// <summary>
        /// Gets a scroll northeast <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollNE
        {
            get => EnsureCursor(CursorType.ScrollNE);
        }

        /// <summary>
        /// Gets a scroll northwest <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollNW
        {
            get => EnsureCursor(CursorType.ScrollNW);
        }

        /// <summary>
        /// Gets the scroll east <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollE
        {
            get => EnsureCursor(CursorType.ScrollE);
        }

        /// <summary>
        /// Gets the scroll west <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollW
        {
            get => EnsureCursor(CursorType.ScrollW);
        }

        /// <summary>
        /// Gets the scroll south <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollS
        {
            get => EnsureCursor(CursorType.ScrollS);
        }

        /// <summary>
        /// Gets the scroll north <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollN
        {
            get => EnsureCursor(CursorType.ScrollN);
        }

        /// <summary>
        /// Gets the scroll all <see cref="Cursor"/>.
        /// </summary>
        public static Cursor ScrollAll
        {
            get => EnsureCursor(CursorType.ScrollAll);
        }

        /// <summary>
        /// Gets a west/east scrolling <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollWE
        {
            get => EnsureCursor(CursorType.ScrollWE);
        }

        /// <summary>
        /// Gets the scroll north/south <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollNS
        {
            get => EnsureCursor(CursorType.ScrollNS);
        }

        /// <summary>
        /// Gets a pen <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor Pen
        {
            get => EnsureCursor(CursorType.Pen);
        }

        /// <summary>
        /// Represents a Hand <see cref="Cursor"/>.
        /// </summary>
        public static Cursor Hand
        {
            get => EnsureCursor(CursorType.Hand);
        }

        /// <summary>
        /// Represents a Wait <see cref="Cursor"/>.
        /// </summary>
        public static Cursor Wait
        {
            get => EnsureCursor(CursorType.Wait);
        }

        /// <summary>
        /// Gets an up arrow <see cref="Cursor"/>, which is typically used to identify
        /// an insertion point.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor UpArrow
        {
            get => EnsureCursor(CursorType.UpArrow);
        }

        /// <summary>
        /// Represents a SizeWE <see cref="Cursor"/>.
        /// </summary>
        public static Cursor SizeWE
        {
            get => EnsureCursor(CursorType.SizeWE);
        }

        /// <summary>
        /// Represents a SizeNWSE <see cref="Cursor"/>.
        /// </summary>
        public static Cursor SizeNWSE
        {
            get => EnsureCursor(CursorType.SizeNWSE);
        }

        /// <summary>
        /// Represents a SizeNS <see cref="Cursor"/>.
        /// </summary>
        public static Cursor SizeNS
        {
            get => EnsureCursor(CursorType.SizeNS);
        }

        /// <summary>
        /// Represents a SizeNESW <see cref="Cursor"/>.
        /// </summary>
        public static Cursor SizeNESW
        {
            get => EnsureCursor(CursorType.SizeNESW);
        }

        /// <summary>
        /// Gets a four-headed sizing <see cref="Cursor"/>, which consists of
        /// four joined arrows that point north, south, east, and west.
        /// </summary>
        public static Cursor SizeAll
        {
            get => EnsureCursor(CursorType.SizeAll);
        }

        /// <summary>
        /// Represents an IBeam <see cref="Cursor"/>, which is typically used to show
        /// where the text cursor appears when the mouse is clicked.
        /// </summary>
        public static Cursor IBeam
        {
            get => EnsureCursor(CursorType.IBeam);
        }

        /// <summary>
        ///  Gets a help <see cref="Cursor"/> which is a combination of an arrow
        ///  and a question mark.
        ///  </summary>
        public static Cursor Help
        {
            get => EnsureCursor(CursorType.Help);
        }

        /// <summary>
        /// Gets the crosshair <see cref="Cursor"/>.
        /// </summary>
        public static Cursor Cross
        {
            get => EnsureCursor(CursorType.Cross);
        }

        /// <summary>
        /// Gets the <see cref="Cursor"/> that appears when an application is
        /// starting.
        /// </summary>
        public static Cursor AppStarting
        {
            get => EnsureCursor(CursorType.AppStarting);
        }

        /// <summary>
        /// Represents an Arrow <see cref="Cursor"/>.
        /// </summary>
        public static Cursor Arrow
        {
            get => EnsureCursor(CursorType.Arrow);
        }

        /// <summary>
        /// Gets a <see cref="Cursor"/> with which indicates that a particular
        /// region is invalid for a given operation.
        /// </summary>
        public static Cursor No
        {
            get => EnsureCursor(CursorType.No);
        }

        /// <summary>
        /// Gets a scroll southeast <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ScrollSE
        {
            get => EnsureCursor(CursorType.ScrollSE);
        }

        /// <summary>
        /// Gets the arrow with a compact disk <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor ArrowCD
        {
            get => EnsureCursor(CursorType.ArrowCD);
        }

        /// <summary>
        /// Represents an Eraser <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor Eraser
        {
            get => EnsureCursor(CursorType.Eraser);
        }

        /// <summary>
        /// Represents a Stylus <see cref="Cursor"/>.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static Cursor Stylus
        {
            get => EnsureCursor(CursorType.Stylus);
        }

        internal static Cursor EnsureCursor(CursorType cursorType)
        {
            if (_stockCursors[(int)cursorType] == null)
            {
                _stockCursors[(int)cursorType] = new Cursor(cursorType);
            }
            return _stockCursors[(int)cursorType];
        }

        internal const int _cursorTypeCount = ((int)CursorType.Eraser) + 1;

        private static Cursor[] _stockCursors = new Cursor[_cursorTypeCount];  //CursorType.Eraser = 29
    }

    /// <summary>
    ///     An enumeration of the supported cursor types.
    /// </summary>
    internal enum CursorType : int
    {
        /// <summary>
        ///     a value indicating that no cursor should be displayed at all.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Standard No Cursor.
        /// </summary>
        No = 1,

        /// <summary>
        ///     Standard arrow cursor.
        /// </summary>
        Arrow = 2,

        /// <summary>
        ///     Standard arrow with small hourglass cursor.
        /// </summary>
        AppStarting = 3,

        /// <summary>
        ///     Crosshair cursor.
        /// </summary>        
        Cross = 4,

        /// <summary>
        ///     Help cursor.
        /// </summary>        	
        Help = 5,

        /// <summary>
        ///     Text I-Beam cursor.
        /// </summary>
        IBeam = 6,

        /// <summary>
        ///     Four-way pointing cursor.
        /// </summary>
        SizeAll = 7,

        /// <summary>
        ///     Double arrow pointing NE and SW.
        /// </summary>
        SizeNESW = 8,

        /// <summary>
        ///     Double arrow pointing N and S.
        /// </summary>
        SizeNS = 9,

        /// <summary>
        ///     Double arrow pointing NW and SE.
        /// </summary>
        SizeNWSE = 10,

        /// <summary>
        ///     Double arrow pointing W and E.
        /// </summary>
        SizeWE = 11,

        /// <summary>
        ///     Vertical arrow cursor.
        /// </summary>
        UpArrow = 12,

        /// <summary>
        ///     Hourglass cursor.
        /// </summary>
        Wait = 13,

        /// <summary>
        ///     Hand cursor.
        /// </summary>
        Hand = 14,

        /// <summary>
        /// PenCursor
        /// </summary>
        Pen = 15,

        /// <summary>
        /// ScrollNSCursor
        /// </summary>
        ScrollNS = 16,

        /// <summary>
        /// ScrollWECursor
        /// </summary>
        ScrollWE = 17,

        /// <summary>
        /// ScrollAllCursor
        /// </summary>
        ScrollAll = 18,

        /// <summary>
        /// ScrollNCursor
        /// </summary>
        ScrollN = 19,

        /// <summary>
        /// ScrollSCursor
        /// </summary>
        ScrollS = 20,

        /// <summary>
        /// ScrollWCursor
        /// </summary>
        ScrollW = 21,

        /// <summary>
        /// ScrollECursor
        /// </summary>
        ScrollE = 22,

        /// <summary>
        /// ScrollNWCursor
        /// </summary>
        ScrollNW = 23,

        /// <summary>
        /// ScrollNECursor
        /// </summary>
        ScrollNE = 24,

        /// <summary>
        /// ScrollSWCursor
        /// </summary>
        ScrollSW = 25,

        /// <summary>
        /// ScrollSECursor
        /// </summary>
        ScrollSE = 26,

        /// <summary>
        /// ArrowCDCursor
        /// </summary>
        ArrowCD = 27,

        /// <summary>
        /// StylusCursor
        /// </summary>
        Stylus = 28,

        /// <summary>
        /// EraserCursor
        /// </summary>
        Eraser = 29,

        // Update the count in Cursors class and the HtmlCursors array
        // in the Cursor class if there is a new addition here.
    }
}