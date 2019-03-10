
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


using System;

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
