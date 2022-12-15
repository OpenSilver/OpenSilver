
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the System.Windows.Window.Closing event.
    /// </summary>
    public partial class ClosingEventArgs : CancelEventArgs
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
                        OpenSilver.Interop.ExecuteJavaScriptVoid(
                            $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_JSArgs)}.returnValue = 'The modifications you made may not be saved.';"); //todo: find a way to change this message at will (changing this only has an impact on IE and maybe Edge).
                    }
                    else
                    {
                        OpenSilver.Interop.ExecuteJavaScriptVoid(
                            $"{CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_JSArgs)}.returnValue = '';");
                    }
                }
            }
        }
    }
}
