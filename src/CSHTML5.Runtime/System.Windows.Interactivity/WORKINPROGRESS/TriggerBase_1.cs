#if WORKINPROGRESS

#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
	public abstract partial class TriggerBase<T> : TriggerBase where T : DependencyObject
	{
		protected TriggerBase()
		{
			
		}

		protected T AssociatedObject { get; }
		protected sealed override Type AssociatedObjectTypeConstraint { get; }
	}
}

#endif