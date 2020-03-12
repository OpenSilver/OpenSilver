

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
    /// <summary>
    /// Represents an attachable object that encapsulates a unit of functionality.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// This is an infrastructure class. Action authors should derive from TriggerAction&lt;T&gt; instead of this class.
    /// </remarks>
    public abstract partial class TriggerAction : FrameworkElement, IAttachedObject //DependencyObject, IAttachedObject //InteractivityBase
    {
        //Note on this file: see commit 58c52131 of October 30th, 2019 for comments on the modifications from the original source.
        //Based on the code that can be found at https://github.com/jlaanstra/Windows.UI.Interactivity/tree/master/Windows.UI.Interactivity.

        #region added because we changed heritage
        internal DependencyObject _associatedObject = null;
        /// <summary>
        /// Gets the object to which this behavior is attached.
        /// </summary>
        public DependencyObject AssociatedObject { get { return _associatedObject; } }  //todo: was protected but it has to be public because it comes from an interface si I don't really understand.

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if (_associatedObject != null)
            {
                throw new InvalidOperationException("The Behavior is already hosted on a different element.");
            }
            _associatedObject = dependencyObject;
        }
#endregion

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TriggerAction), new PropertyMetadata(true)
        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        private bool isHosted;

        /// <summary>
        /// Gets or sets a value indicating whether this action will run when invoked. This is a dependency property.
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if this action will be run when invoked; otherwise, <c>False</c>.
        /// </value>
        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(TriggerAction.IsEnabledProperty);
            }
            set
            {
                this.SetValue(TriggerAction.IsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is attached.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>True</c> if this instance is attached; otherwise, <c>False</c>.
        /// </value>
        internal bool IsHosted
        {
            get
            {
                return this.isHosted;
            }
            set
            {
                this.isHosted = value;
            }
        }

        internal TriggerAction()//Type associatedObjectTypeConstraint)
        {
            //todo: should probably handle that property
            //this.AssociatedObjectTypeConstraint = associatedObjectTypeConstraint;
        }

        /// <summary>
        /// Attempts to invoke the action.
        /// 
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        internal void CallInvoke(object parameter)
        {
            if (!this.IsEnabled)
            {
                return;
            }
            this.Invoke(parameter);
        }

        /// <summary>
        /// Invokes the action.
        /// 
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected abstract void Invoke(object parameter);

        /// <summary>
        /// Attaches to the specified object.
        /// 
        /// </summary>
        /// <param name="frameworkElement">The object to attach to.</param><exception cref="T:System.InvalidOperationException">Cannot host the same TriggerAction on more than one object at a time.</exception><exception cref="T:System.InvalidOperationException">dependencyObject does not satisfy the TriggerAction type constraint.</exception>
        public void Attach(FrameworkElement frameworkElement) //should be an override but we changed the heritage
        {
            if (frameworkElement == this.AssociatedObject)
            {
                return;
            }
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException("Cannot Host TriggerAction Multiple Times");
            }
            this._associatedObject = frameworkElement;
            Attach((DependencyObject)frameworkElement);
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// 
        /// </summary>
        public void Detach() //should be an override but we changed the heritage
        {
            this._associatedObject = null;
        }
    }
}