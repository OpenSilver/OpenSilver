
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
            if(_associatedObject != null)
            {
                throw new InvalidOperationException("The Behavior is already hosted on a different element.");
            }
            if (AssociatedType != null && !AssociatedType.IsAssignableFrom(dependencyObject.GetType()))
            {
                throw new InvalidOperationException("dependencyObject does not satisfy the Behavior type constraint.");
            }
            else
            {
                _associatedObject = dependencyObject;
                OnAttached();
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            _associatedObject = null;
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
    }


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
        protected Behavior()
        {
            _associatedType = typeof(T);
        }

        //private T _associatedObject = null;
        /// <summary>
        /// Gets the object to which this System.Windows.Interactivity.Behavior`1 is
        /// attached.
        /// </summary>
        public T AssociatedObject { get { return (T)_associatedObject; } }  //todo: was protected but it has to be public because it comes from an interface si I don't really understand.
    }
}
