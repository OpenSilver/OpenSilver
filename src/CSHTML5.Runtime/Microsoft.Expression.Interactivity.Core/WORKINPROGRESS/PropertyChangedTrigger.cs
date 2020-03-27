#if WORKINPROGRESS
#if MIGRATION

using System.Windows.Interactivity;

using System.Windows;

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     Represents a trigger that performs actions when the bound data have changed.
	public partial class PropertyChangedTrigger : TriggerBase<DependencyObject>
	{
		public static readonly DependencyProperty BindingProperty;

		public PropertyChangedTrigger()
		{
			
		}

		//
		// Summary:
		//     A binding object that the trigger will listen to, and that causes the trigger
		//     to fire when it changes.
		public object Binding { get; set; }

		//
		// Summary:
		//     Called when the binding property has changed. UA_REVIEW:chabiss
		//
		// Parameters:
		//   args:
		//     System.Windows.DependencyPropertyChangedEventArgs argument.
		protected virtual void EvaluateBindingChange(object args)
		{
			
		}
		//
		// Summary:
		//     Called after the trigger is attached to an AssociatedObject. UA_REVIEW:chabiss
		protected override void OnAttached()
		{
			
		}
		//
		// Summary:
		//     Called when the trigger is being detached from its AssociatedObject, but before
		//     it has actually occurred. UA_REVIEW:chabiss
		protected override void OnDetaching()
		{
			
		}
	}
}

#endif
#endif