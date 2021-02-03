
#if WORKINPROGRESS
namespace System.Windows.Controls
{
    public class RoutedPropertyChangingEventArgs<T>
    {
        // Summary:
        //     Gets or sets a value that determines whether the property change that originated
        //     the RoutedPropertyChanging event should be cancelled.
        //
        // Returns:
        //     true to cancel the property change; this resets the property to System.Windows.Controls.RoutedPropertyChangingEventArgs<T>.OldValue.
        //     false to not cancel the property change; the value changes to System.Windows.Controls.RoutedPropertyChangingEventArgs<T>.NewValue.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     Attempted to cancel in an instance where System.Windows.Controls.RoutedPropertyChangingEventArgs<T>.IsCancelable
        //     is false.
        public bool Cancel { get; set; }

    }
}
#endif