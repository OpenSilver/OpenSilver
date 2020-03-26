

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

#if !CSHTML5BLAZOR
namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for a cancelable event.
    /// </summary>
    public partial class CancelEventArgs : EventArgs
    {
        private bool _cancel;

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.CancelEventArgs class
        /// with the System.ComponentModel.CancelEventArgs.Cancel property set to false.
        /// </summary>
        public CancelEventArgs() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.CancelEventArgs class
        /// with the System.ComponentModel.CancelEventArgs.Cancel property set to the
        /// given value.
        /// </summary>
        /// <param name="cancel">true to cancel the event; otherwise, false.</param>
        public CancelEventArgs(bool cancel)
        {
            _cancel = cancel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }
    }
}
#endif