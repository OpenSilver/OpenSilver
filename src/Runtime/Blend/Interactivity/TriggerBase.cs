﻿// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace System.Windows.Interactivity
{
    using System;
    using System.Windows.Markup;
    using System.Globalization;
    using OpenSilver.Internal.Interactivity;

#if MIGRATION
    using System.Windows;
    using System.Windows.Media.Animation;
#else
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Media.Animation;
#endif
    
    /// <summary>
    /// Represents an object that can invoke actions conditionally.
    /// </summary>
    /// <typeparam name="T">The type to which this trigger can be attached.</typeparam>
    /// <remarks>
    ///		TriggerBase is the base class for controlling actions. Override OnAttached() and 
    ///		OnDetaching() to hook and unhook handlers on the AssociatedObject. You may 
    ///		constrain the types that a derived TriggerBase may be attached to by specifying 
    ///		the generic parameter. Call InvokeActions() to fire all Actions associated with 
    ///		this TriggerBase.
    ///	</remarks>
    public abstract class TriggerBase<T> : TriggerBase where T : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerBase&lt;T&gt;"/> class.
        /// </summary>
        protected TriggerBase()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// Gets the object to which the trigger is attached.
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
        /// Gets the type constraint of the associated object.
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

    /// <summary>
    /// Argument passed to PreviewInvoke event. Assigning Cancelling to True will cancel the invoking of the trigger.
    /// </summary>
    /// <remarks>This is an infrastructure class. Behavior attached to a trigger base object can add its behavior as a listener to TriggerBase.PreviewInvoke.</remarks>
    public class PreviewInvokeEventArgs : EventArgs
    {
        public bool Cancelling { get; set; }
    }

    /// <summary>
    /// Represents an object that can invoke Actions conditionally.
    /// </summary>
    /// <remarks>This is an infrastructure class. Trigger authors should derive from Trigger&lt;T&gt; instead of this class.</remarks>
    [ContentProperty("Actions")]
    public abstract class TriggerBase :
#if __WPF__
		Animatable,
#else
        DependencyObject,
#endif
        IAttachedObject
    {
        private DependencyObject associatedObject;
        private Type associatedObjectTypeConstraint;

#if __WPF__
        private static readonly DependencyPropertyKey ActionsPropertyKey = DependencyProperty.RegisterReadOnly("Actions",
                                                                                                            typeof(TriggerActionCollection),
                                                                                                            typeof(TriggerBase),
                                                                                                            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ActionsProperty = ActionsPropertyKey.DependencyProperty;
#else
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register(
                nameof(Actions),
                typeof(TriggerActionCollection),
                typeof(TriggerBase),
                null);
#endif

        internal TriggerBase(Type associatedObjectTypeConstraint)
        {
            this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
            TriggerActionCollection newCollection = new TriggerActionCollection();
#if __WPF__
            this.SetValue(ActionsPropertyKey, newCollection);
#else
            this.SetValue(ActionsProperty, newCollection);
#endif
        }

        /// <summary>
        /// Gets the object to which the trigger is attached.
        /// </summary>
        /// <value>The associated object.</value>
        protected DependencyObject AssociatedObject
        {
            get
            {
#if __WPF__
				this.ReadPreamble();
#endif
                return this.associatedObject;
            }
        }

        /// <summary>
        /// Gets the type constraint of the associated object.
        /// </summary>
        /// <value>The associated object type constraint.</value>
        protected virtual Type AssociatedObjectTypeConstraint
        {
            get
            {
#if __WPF__
				this.ReadPreamble();
#endif
                return this.associatedObjectTypeConstraint;
            }
        }

        /// <summary>
        /// Gets the actions associated with this trigger.
        /// </summary>
        /// <value>The actions associated with this trigger.</value>
        public TriggerActionCollection Actions
        {
            get
            {
                return (TriggerActionCollection)this.GetValue(ActionsProperty);
            }
        }

        /// <summary>
        /// Event handler for registering to PreviewInvoke.
        /// </summary>
        public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;

        /// <summary>
        /// Invoke all actions associated with this trigger.
        /// </summary>
        /// <remarks>Derived classes should call this to fire the trigger.</remarks>
        protected void InvokeActions(object parameter)
        {
            if (this.PreviewInvoke != null)
            {
                // Fire the previewInvoke event 
                PreviewInvokeEventArgs previewInvokeEventArg = new PreviewInvokeEventArgs();
                this.PreviewInvoke(this, previewInvokeEventArg);
                // If a handler has cancelled the event, abort the invoke
                if (previewInvokeEventArg.Cancelling == true)
                {
                    return;
                }
            }

            foreach (TriggerAction action in this.Actions)
            {
                action.CallInvoke(parameter);
            }
        }

        /// <summary>
        /// Called after the trigger is attached to an AssociatedObject.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the trigger is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected virtual void OnDetaching()
        {
        }

#if __WPF__
        /// <summary>
        /// Creates a new instance of the TriggerBase derived class.
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            Type classType = this.GetType();
            return (Freezable)Activator.CreateInstance(classType);
        }
#endif

#region IAttachedObject Members

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

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        /// <exception cref="InvalidOperationException">Cannot host the same trigger on more than one object at a time.</exception>
        /// <exception cref="InvalidOperationException">dependencyObject does not satisfy the trigger type constraint.</exception>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject != this.AssociatedObject)
            {
                if (this.AssociatedObject != null)
                {
                    throw new InvalidOperationException(ExceptionStringTable.CannotHostTriggerMultipleTimesExceptionMessage);
                }

                // Ensure the type constraint is met
                if (dependencyObject != null && !this.AssociatedObjectTypeConstraint.IsAssignableFrom(dependencyObject.GetType()))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                        ExceptionStringTable.TypeConstraintViolatedExceptionMessage,
                                                                        this.GetType().Name,
                                                                        dependencyObject.GetType().Name,
                                                                        this.AssociatedObjectTypeConstraint.Name));
                }

#if __WPF__
                this.WritePreamble();
#endif
                this.associatedObject = dependencyObject;
#if __WPF__
                this.WritePostscript();
#endif

                this.Actions.Attach(dependencyObject);
                this.OnAttached();
            }
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            this.OnDetaching();
#if __WPF__
            this.WritePreamble();
#endif
            this.associatedObject = null;
#if __WPF__
            this.WritePostscript();
#endif
            this.Actions.Detach();
        }

#endregion
    }
}
