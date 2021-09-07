#if MIGRATION

using System.Windows.Interactivity;

using System.Windows;

namespace Microsoft.Expression.Interactivity.Core
{
	//
	// Summary:
	//     Represents a trigger that performs actions when the bound data have changed.
	[OpenSilver.NotImplemented]
	public partial class PropertyChangedTrigger : TriggerBase<DependencyObject>
	{
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty BindingProperty =
			DependencyProperty.Register("Binding",
										typeof(object),
										typeof(PropertyChangedTrigger),
										null);

		[OpenSilver.NotImplemented]
		public PropertyChangedTrigger()
		{
			
		}

		//
		// Summary:
		//     A binding object that the trigger will listen to, and that causes the trigger
		//     to fire when it changes.
		[OpenSilver.NotImplemented]
		public object Binding
		{
			get { return (object)GetValue(BindingProperty); }
			set { SetValue(BindingProperty, value); }
		}

		//
		// Summary:
		//     Called when the binding property has changed. UA_REVIEW:chabiss
		//
		// Parameters:
		//   args:
		//     System.Windows.DependencyPropertyChangedEventArgs argument.
		[OpenSilver.NotImplemented]
		protected virtual void EvaluateBindingChange(object args)
		{
			
		}
		//
		// Summary:
		//     Called after the trigger is attached to an AssociatedObject. UA_REVIEW:chabiss
		[OpenSilver.NotImplemented]
		protected override void OnAttached()
		{
			
		}
		//
		// Summary:
		//     Called when the trigger is being detached from its AssociatedObject, but before
		//     it has actually occurred. UA_REVIEW:chabiss
		[OpenSilver.NotImplemented]
		protected override void OnDetaching()
		{
			
		}
	}
}

#endif
