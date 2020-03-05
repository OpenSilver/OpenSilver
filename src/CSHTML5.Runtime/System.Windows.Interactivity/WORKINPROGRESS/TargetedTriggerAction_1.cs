#if WORKINPROGRESS

namespace System.Windows.Interactivity
{
	public abstract partial class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
	{
		protected TargetedTriggerAction()
		{
			
		}

		protected T Target { get; }

		protected virtual void OnTargetChanged(T oldTarget, T newTarget)
		{
			
		}
	}
}

#endif