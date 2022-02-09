

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
#if !MIGRATION
using Windows.UI.Xaml;
#endif
namespace System.Windows.Interactivity
{
    // Remarks:
    //     Behavior is the base class for providing attachable state and commands to
    //     an object.  The types the Behavior can be attached to can be controlled by
    //     the generic parameter.  Override OnAttached() and OnDetaching() methods to
    //     hook and unhook any necessary handlers from the AssociatedObject.
    /// <summary>
    /// Encapsulates state information and zero or more ICommands into an attachable
    /// object.
    /// </summary>
    /// <typeparam name="T">The type the System.Windows.Interactivity.Behavior`1 can be attached to.</typeparam>
    public abstract partial class Behavior<T> : Behavior where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Interactivity.Behavior`1
        /// class.
        /// </summary>
        protected Behavior() : base(typeof(T))
        {
        }

        //private T _associatedObject = null;
        /// <summary>
        /// Gets the object to which this System.Windows.Interactivity.Behavior`1 is
        /// attached.
        /// </summary>
        public new T AssociatedObject { get { return (T)base.AssociatedObject; } }
    }
}
