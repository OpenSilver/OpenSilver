#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    //
    // Summary:
    //     Represents the method that will handle the System.Windows.UIElement.TextInput
    //     routed event.
    //
    // Parameters:
    //   sender:
    //     The object where the event handler is attached.
    //
    //   e:
    //     Event data for the event.
    public delegate void TextCompositionEventHandler(object sender, TextCompositionEventArgs e);
}
#endif