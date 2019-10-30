using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows.Markup;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents an object that can invoke Actions conditionally.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// This is an infrastructure class. Trigger authors should derive from Trigger&lt;T&gt; instead of this class.
    /// </remarks>
    [ContentProperty(Name = "Actions")]
    public abstract class TriggerBase : DependencyObject, IAttachedObject//InteractivityBase
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
            if (dependencyObject == this.AssociatedObject)
            {
                return;
            }
            if (_associatedObject != null)
            {
                throw new InvalidOperationException("The Behavior is already hosted on a different element.");
            }
            _associatedObject = dependencyObject;
            OnAttached();
        }

        protected virtual void OnAttached()
        {
            this.Actions.Attach(_associatedObject);

        }
#endregion

        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(TriggerActionCollection), typeof(TriggerBase), null);

        /// <summary>
        /// Gets the actions associated with this trigger.
        /// </summary>
        /// 
        /// <value>
        /// The actions associated with this trigger.
        /// </value>
        public TriggerActionCollection Actions
        {
            get
            {
                TriggerActionCollection actions = (TriggerActionCollection)this.GetValue(TriggerBase.ActionsProperty);
                if (actions == null)
                {
                    actions = new TriggerActionCollection();
                    this.SetValue(TriggerBase.ActionsProperty, actions);
                }
                return actions;
            }
        }

        /// <summary>
        /// Invoke all actions associated with this trigger.
        /// </summary>
        /// 
        /// <remarks>
        /// Derived classes should call this to fire the trigger.
        /// </remarks>
        protected void InvokeActions(object parameter)
        {
            foreach (TriggerAction triggerAction in this.Actions)
            {
                triggerAction.CallInvoke(parameter);
            }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="frameworkElement">The object to attach to.</param><exception cref="T:System.InvalidOperationException">Cannot host the same trigger on more than one object at a time.</exception><exception cref="T:System.InvalidOperationException">dependencyObject does not satisfy the trigger type constraint.</exception>
        public void Attach(FrameworkElement frameworkElement) //should be an override but we changed the heritage
        {
            //Note: I don't think this method is ever called.

            if (frameworkElement == this.AssociatedObject)
            {
                return;
            }
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException("Cannot Host Trigger Multiple Times");
            }
           
            this._associatedObject = frameworkElement;
            Attach((DependencyObject)frameworkElement);
            this.Actions.Attach(frameworkElement);
            this.OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()//should be an override but we changed the heritage
        {
            this._associatedObject = null;
            this.Actions.Detach();
        }
    }
}