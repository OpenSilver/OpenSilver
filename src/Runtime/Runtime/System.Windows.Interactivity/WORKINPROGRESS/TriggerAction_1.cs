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

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents an attachable object that encapsulates a unit of functionality.
    /// </summary>
    /// <typeparam name="T">The type to which this action can be attached.</typeparam>
    public abstract class TriggerAction<T> : TriggerAction where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerAction&lt;T&gt;"/> class.
        /// </summary>
        protected TriggerAction()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Gets the object to which this <see cref="TriggerAction&lt;T&gt;"/> is attached.
        /// </summary>
        /// <value>The associated object.</value>
        protected new T AssociatedObject
        {
            get
            {
                return (T)base.AssociatedObject;
            }
        }

        /// <summary>
        /// Gets the associated object type constraint.
        /// </summary>
        /// <value>The associated object type constraint.</value>
        protected sealed override Type AssociatedObjectTypeConstraint
        {
            get
            {
                return base.AssociatedObjectTypeConstraint;
            }
        }
    }
}