
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

#if !CSHTML5BLAZOR
namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for a cancelable event.
    /// </summary>
    public class CancelEventArgs : EventArgs
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