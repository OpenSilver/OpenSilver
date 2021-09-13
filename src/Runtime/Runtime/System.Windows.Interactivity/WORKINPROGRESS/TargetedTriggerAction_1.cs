namespace System.Windows.Interactivity
{
	[OpenSilver.NotImplemented]
	public abstract partial class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
	{
		[OpenSilver.NotImplemented]
		protected TargetedTriggerAction()
		{
			
		}

		[OpenSilver.NotImplemented]
		protected T Target { get; }

		[OpenSilver.NotImplemented]
		protected virtual void OnTargetChanged(T oldTarget, T newTarget)
		{
			
		}
	}
}
