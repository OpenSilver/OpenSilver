#if WORKINPROGRESS

namespace System.Windows.Interactivity
{
	public abstract partial class TriggerBase
	{
		protected virtual Type AssociatedObjectTypeConstraint { get; }

		protected virtual void OnDetaching()
		{
			
		}
	}
}

#endif