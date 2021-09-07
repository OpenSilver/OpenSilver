#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
	[OpenSilver.NotImplemented]
	public abstract partial class TargetedTriggerAction : TriggerAction
	{
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty TargetNameProperty =
			DependencyProperty.Register("TargetName",
										typeof(string),
										typeof(TargetedTriggerAction),
										null);
		[OpenSilver.NotImplemented]
		public static readonly DependencyProperty TargetObjectProperty =
			DependencyProperty.Register("TargetObject",
										typeof(object),
										typeof(TargetedTriggerAction),
										null);

		[OpenSilver.NotImplemented]
		public string TargetName
		{
			get { return (string)GetValue(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}
		[OpenSilver.NotImplemented]
		public object TargetObject
		{
			get { return (object)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}
		[OpenSilver.NotImplemented]
		protected sealed override Type AssociatedObjectTypeConstraint { get; }
		[OpenSilver.NotImplemented]
		protected object Target { get; }
		[OpenSilver.NotImplemented]
		protected Type TargetTypeConstraint { get; }

		[OpenSilver.NotImplemented]
		protected override void OnAttached()
		{
			
		}

		[OpenSilver.NotImplemented]
		protected override void OnDetaching()
		{
			
		}
	}
}
