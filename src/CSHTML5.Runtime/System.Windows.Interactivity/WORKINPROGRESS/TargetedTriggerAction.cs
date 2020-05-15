#if WORKINPROGRESS

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
	public abstract partial class TargetedTriggerAction : TriggerAction
	{
		public static readonly DependencyProperty TargetNameProperty =
			DependencyProperty.Register("TargetName",
										typeof(string),
										typeof(TargetedTriggerAction),
										null);
		public static readonly DependencyProperty TargetObjectProperty =
			DependencyProperty.Register("TargetObject",
										typeof(object),
										typeof(TargetedTriggerAction),
										null);

		public string TargetName
		{
			get { return (string)GetValue(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}
		public object TargetObject
		{
			get { return (object)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}
		protected sealed override Type AssociatedObjectTypeConstraint { get; }
		protected object Target { get; }
		protected Type TargetTypeConstraint { get; }

		protected override void OnAttached()
		{
			
		}

		protected override void OnDetaching()
		{
			
		}
	}
}

#endif