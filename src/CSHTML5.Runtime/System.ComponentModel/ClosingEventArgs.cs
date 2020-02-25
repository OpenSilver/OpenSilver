
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
