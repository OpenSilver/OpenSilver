namespace Microsoft.Expression.Interactivity.Core
{
    using System;
    using System.Windows.Interactivity;
    using OpenSilver.Internal.Expression.Interactivity;

#if MIGRATION
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
#else
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
#endif

    /// <summary>
    /// An action that will remove the targeted element from the tree when invoked.
    /// </summary>
    /// <remarks>
    /// This action may fail. The action understands how to remove elements from common parents but not from custom collections or direct manipulation
    /// of the visual tree.
    /// </remarks>
    public class RemoveElementAction : TargetedTriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null && this.Target != null)
            {
                DependencyObject parent = this.Target.Parent;

                Panel panel = parent as Panel;
                if (panel != null)
                {
                    panel.Children.Remove(this.Target);
                    return;
                }

                ContentControl contentControl = parent as ContentControl;
                if (contentControl != null)
                {
                    if (contentControl.Content == this.Target)
                    {
                        contentControl.Content = null;
                    }
                    return;
                }

                ItemsControl itemsControl = parent as ItemsControl;
                if (itemsControl != null)
                {
                    itemsControl.Items.Remove(this.Target);
                    return;
                }

#if __WPF__
                Page page = parent as Page;
                if (page != null)
                {
                    if (page.Content == this.Target)
                    {
                        page.Content = null;
                    }
                    return;
                }

                Decorator decorator = parent as Decorator;
                if (decorator != null)
                {
                    if (decorator.Child == this.Target)
                    {
                        decorator.Child = null;
                    }
                    return;
                }
#endif

                if (parent != null)
                {
                    throw new InvalidOperationException(ExceptionStringTable.UnsupportedRemoveTargetExceptionMessage);
                }
            }
        }
    }
}