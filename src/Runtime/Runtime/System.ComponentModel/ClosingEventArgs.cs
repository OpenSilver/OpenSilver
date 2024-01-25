
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

using System.Windows;
using OpenSilver.Internal;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the <see cref="Window.Closing"/> event.
    /// </summary>
    public class ClosingEventArgs : CancelEventArgs
    {
        private object _jsArgs;
        private bool _showCloseConfirmationDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingEventArgs"/> class.
        /// </summary>
        /// <param name="isCancelable">
        /// Initializes the <see cref="IsCancelable"/> property.
        /// </param>
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
        /// Gets a value that indicates whether you can cancel the <see cref="Window.Closing"/> event.
        /// </summary>
        /// <returns>
        /// true if you can cancel the event; otherwise, false.
        /// </returns>
        public bool IsCancelable { get; }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
                            $"{OpenSilver.Interop.GetVariableStringForJS(_jsArgs)}.returnValue = 'The modifications you made may not be saved.';"); //todo: find a way to change this message at will (changing this only has an impact on IE and maybe Edge).
                    }
                    else
                    {
                        OpenSilver.Interop.ExecuteJavaScriptVoid(
                            $"{OpenSilver.Interop.GetVariableStringForJS(_jsArgs)}.returnValue = '';");
                    }
                }
            }
        }
    }
}
