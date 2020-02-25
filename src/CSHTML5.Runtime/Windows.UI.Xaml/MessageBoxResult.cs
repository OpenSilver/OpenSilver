
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents a user's response to a message box.
    /// </summary>
    public enum MessageBoxResult
    {
        /// <summary>
        /// This value is not currently used.
        /// </summary>
        None = 0,
        /// <summary>
        /// The user clicked the OK button.
        /// </summary>
        OK = 1,
        /// <summary>
        /// The user clicked the Cancel button or pressed ESC.
        /// </summary>
        Cancel = 2,
#if WORKINPROGRESS
        /// <summary>
        /// The dialog box return value is
        /// Yes (usually sent from a button labeled Yes).
        /// </summary>
        Yes,
        /// <summary>
        /// The dialog box return value is
        /// No (usually sent from a button labeled No).
        /// </summary>
        No,
#endif
    }
}
