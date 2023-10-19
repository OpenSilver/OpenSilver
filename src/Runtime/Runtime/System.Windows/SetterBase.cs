

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

namespace System.Windows
{
    /// <summary>
    /// Represents the base class for value setters.
    /// </summary>
    public abstract partial class SetterBase : DependencyObject
    {
        /// <summary>
        ///     SetterBase construction
        /// </summary>
        internal SetterBase()
        {
        }

        /// <summary>
        ///     Returns the sealed state of this object.  If true, any attempt
        /// at modifying the state of this object will trigger an exception.
        /// </summary>
        public bool IsSealed
        {
            get
            {
                return _sealed;
            }
        }

        internal virtual void Seal()
        {
            _sealed = true;
        }

        /// <summary>
        ///  Subclasses need to call this method before any changes to their state.
        /// </summary>
        private protected void CheckSealed()
        {
            if (_sealed)
            {
                throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", "SetterBase"));
            }
        }

        // Derived
        private bool _sealed;
    }
}
