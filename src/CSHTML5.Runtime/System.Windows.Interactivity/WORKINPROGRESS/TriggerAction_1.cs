#if WORKINPROGRESS
#if MIGRATION

namespace System.Windows.Interactivity
{
    public abstract partial class TriggerAction<T> : TriggerAction where T : DependencyObject
    {
        protected TriggerAction()
        {

        }

        protected new T AssociatedObject { get; private set; }

        //protected sealed override Type AssociatedObjectTypeConstraint { get; }
    }
}

#endif
#endif
