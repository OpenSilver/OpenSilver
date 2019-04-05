
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



#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Specifies when the Click event should be raised for a control.
    /// </summary>
    public enum ClickMode
    {
        // Summary:
        //     Specifies that the System.Windows.Controls.Primitives.ButtonBase.Click event
        //     should be raised when the left mouse button is pressed and released, and
        //     the mouse pointer is over the control. If you are using the keyboard, specifies
        //     that the System.Windows.Controls.Primitives.ButtonBase.Click event should
        //     be raised when the SPACEBAR or ENTER key is pressed and released, and the
        //     control has keyboard focus.
        /// <summary>
        /// Specifies that the Click event
        /// should be raised when the left mouse button is pressed and released, and
        /// the mouse pointer is over the control.
        /// </summary>
        Release = 0,

        /// <summary>
        /// Specifies that the Click event
        /// should be raised when the mouse button is pressed and the mouse pointer is
        /// over the control.
        /// </summary>
        Press = 1,

#if WORKINPROGRESS
        /// <summary>
        /// Specifies that the Click event
        /// should be raised when the mouse pointer moves over the control.
        /// </summary>
        Hover = 2,
#endif

    }
}
