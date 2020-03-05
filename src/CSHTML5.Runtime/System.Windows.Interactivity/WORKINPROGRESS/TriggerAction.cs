#if WORKINPROGRESS

namespace System.Windows.Interactivity
{
    public abstract partial class TriggerAction
    {
        protected virtual Type AssociatedObjectTypeConstraint { get; }
        
        protected virtual void OnAttached()
        {

        }

        protected virtual void OnDetaching()
        {
            
        }
    }
}

#endif