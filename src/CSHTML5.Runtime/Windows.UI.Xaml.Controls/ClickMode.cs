
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
