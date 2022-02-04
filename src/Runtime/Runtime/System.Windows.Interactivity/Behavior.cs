

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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif
namespace System.Windows.Interactivity
{
    //Note: this file contains the definition for both Behavior and Behavior<T>

    // Remarks:
    //     This is an infrastructure class. Behavior authors should derive from Behavior<T>
    //     instead of from this class.
    /// <summary>
    /// Encapsulates state information and zero or more ICommands into an attachable
    /// object.
    /// </summary>
    public abstract partial class Behavior : DependencyObject, IAttachedObject
    {
        internal DependencyObject _associatedObject = null;
        /// <summary>
        /// Gets the object to which this behavior is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get { return _associatedObject; } }  //todo: was protected but it has to be public because it comes from an interface si I don't really understand.

        internal Behavior(Type associatedType)
        {
            _associatedType = associatedType;
        }

        internal event EventHandler AssociatedObjectChanged;

        internal Type _associatedType = null;
        /// <summary>
        /// The type to which this behavior can be attached.
        /// </summary>
        protected Type AssociatedType
        {
            get
            {
                return _associatedType;
            }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject != this.AssociatedObject)
            {
                if (this.AssociatedObject != null)
                {
                    throw new InvalidOperationException("An instance of a Behavior cannot be attached to more than one object at a time.");
                }

                // todo jekelly: what do we do if dependencyObject is null?

                // Ensure the type constraint is met
                if (dependencyObject != null && !this.AssociatedType.IsAssignableFrom(dependencyObject.GetType()))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                        "Cannot attach type '{0}' to type '{1}'. Instances of type '{0}' can only be attached to objects of type '{2}'.",
                                                                        this.GetType().Name,
                                                                        dependencyObject.GetType().Name,
                                                                        this.AssociatedType.Name));
                }

                _associatedObject = dependencyObject;

                this.OnAssociatedObjectChanged();

                this.OnAttached();
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            _associatedObject = null;
            this.OnAssociatedObjectChanged();
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected virtual void OnAttached() { } //does nothing?

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but
        /// before it has actually occurred.
        /// </summary>
        protected virtual void OnDetaching() { } //does nothing?

        private void OnAssociatedObjectChanged()
        {
            if (this.AssociatedObjectChanged != null)
            {
                this.AssociatedObjectChanged(this, new EventArgs());
            }
        }

        /// <summary>
		/// Gets the associated object.
		/// </summary>
		/// <value>The associated object.</value>
		DependencyObject IAttachedObject.AssociatedObject
        {
            get
            {
                return this.AssociatedObject;
            }
        }
    }
}