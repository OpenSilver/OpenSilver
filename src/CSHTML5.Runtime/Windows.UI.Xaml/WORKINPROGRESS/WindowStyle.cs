#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public enum WindowStyle
    {
        //
        // Summary:
        //     The window displays a title bar and border.
        SingleBorderWindow = 0,
        //
        // Summary:
        //     The window does not display a title bar or border.
        None = 1,
        //
        // Summary:
        //     The window does not display a title bar or border, and the window corners are
        //     rounded.
        BorderlessRoundCornersWindow = 2
    }
}
#endif