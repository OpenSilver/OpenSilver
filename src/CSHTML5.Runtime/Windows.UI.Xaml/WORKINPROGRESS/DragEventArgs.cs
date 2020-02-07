#if WORKINPROGRESS

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    //
    // Summary:
    //     Provides data for drag-and-drop events in Silverlight.
    public sealed class DragEventArgs : RoutedEventArgs
    {
        //
        // Summary:
        //     Gets a data object (implements System.Windows.IDataObject) that contains the
        //     data associated with the corresponding drag event. This value is not useful in
        //     all event cases; see Remarks.
        //
        // Returns:
        //     The data object that contains the data that is associated with the corresponding
        //     drag event.
        public IDataObject Data { get; private set; }
        //
        // Summary:
        //     Gets or sets a value that indicates the present state of the event handling for
        //     a routed event as it travels the route.
        //
        // Returns:
        //     true if the event is marked handled; otherwise, false. The default value is false.
        public bool Handled { get; set; }

        //
        // Summary:
        //     Returns a drop point that is relative to a specified System.Windows.UIElement.
        //
        // Parameters:
        //   relativeTo:
        //     The System.Windows.UIElement for which to get a relative drop point.
        //
        // Returns:
        //     A drop point that is relative to the element specified in relativeTo.
        public Point GetPosition(UIElement relativeTo)
        {
            return new Point();
        }
    }
}

#endif
