// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows;

namespace Microsoft.Expression.Interactivity.Core
{
    /// <summary>
    /// A behavior that attaches to a trigger and controls the conditions
    /// to fire the actions. 
    /// </summary>
    /// 
    [ContentProperty("Condition")]
    public class ConditionBehavior : Behavior<System.Windows.Interactivity.TriggerBase>
    {
        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register("Condition", typeof(ICondition), typeof(ConditionBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the IConditon object on behavior.
        /// </summary>
        /// <value>The name of the condition to change.</value>
        public ICondition Condition
        {
            get { return (ICondition)this.GetValue(ConditionProperty); }
            set { this.SetValue(ConditionProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionBehavior"/> class.
        /// </summary>
        public ConditionBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewInvoke += this.OnPreviewInvoke;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewInvoke -= this.OnPreviewInvoke;
            base.OnDetaching();
        }

        /// <summary>
        /// The event handler that is listening to the preview invoke event that is fired by 
        /// the trigger. Setting PreviewInvokeEventArgs.Cancelling to True will
        /// cancel the invocation.
        /// </summary>
        /// <param name="sender">The trigger base object.</param>
        /// <param name="e">An object of type PreviewInvokeEventArgs where e.Cancelling can be set to True.</param>
        private void OnPreviewInvoke(object sender, PreviewInvokeEventArgs e)
        {
            if (this.Condition != null)
            {
                e.Cancelling = !this.Condition.Evaluate();
            }
        }
    }
}
