

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


using System;

namespace System.Windows
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
        /// <summary>
        /// The dialog box return value is
        /// Yes (usually sent from a button labeled Yes).
        /// </summary>
        [OpenSilver.NotImplemented]
        Yes,
        /// <summary>
        /// The dialog box return value is
        /// No (usually sent from a button labeled No).
        /// </summary>
        [OpenSilver.NotImplemented]
        No,
    }
}
