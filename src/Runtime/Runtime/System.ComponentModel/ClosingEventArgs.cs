
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
    public class ClosingEventArgs : CancelEventArgs
    {
        private object _jsArgs;
        private bool _showCloseConfirmationDialog;

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.ClosingEventArgs
        /// class.
        /// </summary>
        /// <param name="isCancelable">Initializes the System.ComponentModel.ClosingEventArgs.IsCancelable property.</param>
        public ClosingEventArgs(bool isCancelable)
        {
            IsCancelable = isCancelable;
        }

        internal ClosingEventArgs(bool isCancelable, object jsArgs)
        {
            IsCancelable = isCancelable;
            _jsArgs = jsArgs;
        }

        /// <summary>
        /// Gets a value that indicates whether you can cancel the Window.Closing event.
        /// </summary>
        public bool IsCancelable { get; }

        public bool ShowCloseConfirmationDialog
        {
            get { return _showCloseConfirmationDialog; }
            set
            {
                _showCloseConfirmationDialog = value;
                if (_jsArgs != null)
                {
                    if (value)
                    {
                        OpenSilver.Interop.ExecuteJavaScriptVoid(
                            $"{CSHTML5.InteropImplementation.GetVariableStringForJS(_jsArgs)}.returnValue = 'The modifications you made may not be saved.';"); //todo: find a way to change this message at will (changing this only has an impact on IE and maybe Edge).
                    }
                    else
                    {
                        OpenSilver.Interop.ExecuteJavaScriptVoid(
                            $"{CSHTML5.InteropImplementation.GetVariableStringForJS(_jsArgs)}.returnValue = '';");
                    }
                }
            }
        }
    }
}
