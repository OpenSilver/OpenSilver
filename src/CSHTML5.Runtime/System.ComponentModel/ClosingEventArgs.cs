
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the System.Windows.Window.Closing event.
    /// </summary>
    public class ClosingEventArgs : CancelEventArgs
    {
        internal object INTERNAL_JSArgs;

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.ClosingEventArgs
        /// class.
        /// </summary>
        /// <param name="isCancelable">Initializes the System.ComponentModel.ClosingEventArgs.IsCancelable property.</param>
        public ClosingEventArgs(bool isCancelable)
        {
            _isCancelable = isCancelable;
        }

        bool _isCancelable = true;
        /// <summary>
        /// Gets a value that indicates whether you can cancel the Window.Closing event.
        /// </summary>
        public bool IsCancelable { get { return _isCancelable; } }

        private bool _showCloseConfirmationDialog;

        public bool ShowCloseConfirmationDialog
        {
            get { return _showCloseConfirmationDialog; }
            set
            {
                _showCloseConfirmationDialog = value;
                if (INTERNAL_JSArgs != null)
                {
                    if (value)
                    {
                        CSHTML5.Interop.ExecuteJavaScript(@"$0.returnValue = ""The modifications you made may not be saved."";", INTERNAL_JSArgs); //todo: find a way to change this message at will (changing this only has an impact on IE and maybe Edge).
                    }
                    else
                    {
                        CSHTML5.Interop.ExecuteJavaScript(@"$0.returnValue = """";", INTERNAL_JSArgs);
                    }
                }
            }
        }
    }
}
