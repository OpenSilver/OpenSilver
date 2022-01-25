using System;
using System.Windows.Interactivity;
using System.Windows.Markup;
using TriggerBase = System.Windows.Interactivity.TriggerBase;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     A behavior that attaches to a trigger and controls the conditions to fire the
	//     actions.
	[ContentProperty("Condition")]
	public class ConditionBehavior : Behavior<TriggerBase>
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
	}
}