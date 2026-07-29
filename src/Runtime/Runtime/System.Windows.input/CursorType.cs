
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

namespace System.Windows.Input;

/// <summary>
/// Specifies the built in cursor types.
/// </summary>
public enum CursorType : int
{
    /// <summary>
    /// A value indicating that no cursor should be displayed.
    /// </summary>
    None = 0,

    /// <summary>
    /// No cursor.
    /// </summary>
    No = 1,

    /// <summary>
    /// A standard arrow cursor.
    /// </summary>
    Arrow = 2,

    /// <summary>
    /// A standard arrow with small hourglass cursor.
    /// </summary>
    AppStarting = 3,

    /// <summary>
    /// A crosshair cursor.
    /// </summary>        
    Cross = 4,

    /// <summary>
    /// A help cursor.
    /// </summary>        	
    Help = 5,

    /// <summary>
    /// A text I-Beam cursor.
    /// </summary>
    IBeam = 6,

    /// <summary>
    /// A cursor with arrows pointing north, south, east, and west.
    /// </summary>
    SizeAll = 7,

    /// <summary>
    /// A cursor with arrows pointing northeast and southwest.
    /// </summary>
    SizeNESW = 8,

    /// <summary>
    /// A cursor with arrows pointing north and south.
    /// </summary>
    SizeNS = 9,

    /// <summary>
    /// A cursor with arrows pointing northwest and southeast.
    /// </summary>
    SizeNWSE = 10,

    /// <summary>
    /// A cursor with arrows pointing west and east.
    /// </summary>
    SizeWE = 11,

    /// <summary>
    /// A vertical arrow cursor.
    /// </summary>
    UpArrow = 12,

    /// <summary>
    /// An hourglass cursor.
    /// </summary>
    Wait = 13,

    /// <summary>
    /// A hand cursor.
    /// </summary>
    Hand = 14,

    /// <summary>
    /// A pen cursor.
    /// </summary>
    Pen = 15,

    /// <summary>
    /// A scrolling cursor with arrows pointing north and south.
    /// </summary>
    ScrollNS = 16,

    /// <summary>
    /// A scrolling cursor with arrows pointing west and east.
    /// </summary>
    ScrollWE = 17,

    /// <summary>
    /// A scrolling cursor with arrows pointing north, south, east, and west.
    /// </summary>
    ScrollAll = 18,

    /// <summary>
    /// A scrolling cursor with an arrow pointing north.
    /// </summary>
    ScrollN = 19,

    /// <summary>
    /// A scrolling cursor with an arrow pointing south.
    /// </summary>
    ScrollS = 20,

    /// <summary>
    /// A scrolling cursor with an arrow pointing west.
    /// </summary>
    ScrollW = 21,

    /// <summary>
    /// A scrolling cursor with an arrow pointing east.
    /// </summary>
    ScrollE = 22,

    /// <summary>
    /// A scrolling cursor with arrows pointing north and west.
    /// </summary>
    ScrollNW = 23,

    /// <summary>
    /// A scrolling cursor with arrows pointing north and east.
    /// </summary>
    ScrollNE = 24,

    /// <summary>
    /// A scrolling cursor with arrows pointing south and west.
    /// </summary>
    ScrollSW = 25,

    /// <summary>
    /// A scrolling cursor with arrows pointing south and east.
    /// </summary>
    ScrollSE = 26,

    /// <summary>
    /// An arrow cd cursor.
    /// </summary>
    ArrowCD = 27,

    /// <summary>
    /// A stylus cursor.
    /// </summary>
    Stylus = 28,

    /// <summary>
    /// An eraser cursor.
    /// </summary>
    Eraser = 29,

    // Update the count in Cursors class and the HtmlCursors array in the Cursor class
    // and the CursorConverter.GetStandardValues method if there is a new addition here.
}