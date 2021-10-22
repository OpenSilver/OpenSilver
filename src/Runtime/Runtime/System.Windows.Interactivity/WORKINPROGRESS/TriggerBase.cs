namespace System.Windows.Interactivity
{
	public abstract partial class TriggerBase
	{
		[OpenSilver.NotImplemented]
		protected virtual Type AssociatedObjectTypeConstraint { get; }

		[OpenSilver.NotImplemented]
		protected virtual void OnDetaching()
		{
			
		}
	}
}
