#if WORKINPROGRESS

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
	public abstract partial class TargetedTriggerAction : TriggerAction
	{
		public static readonly DependencyProperty TargetNameProperty;
		public static readonly DependencyProperty TargetObjectProperty;

		public string TargetName { get; set; }
		public object TargetObject { get; set; }
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