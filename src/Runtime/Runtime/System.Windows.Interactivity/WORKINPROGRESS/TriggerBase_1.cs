#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
	[OpenSilver.NotImplemented]
	public abstract partial class TriggerBase<T> : TriggerBase where T : DependencyObject
	{
		[OpenSilver.NotImplemented]
		protected TriggerBase()
		{
			
		}

		[OpenSilver.NotImplemented]
		protected T AssociatedObject { get; }
		[OpenSilver.NotImplemented]
		protected sealed override Type AssociatedObjectTypeConstraint { get; }
	}
}
