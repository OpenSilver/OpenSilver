#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    //
    // Summary:
    //     Defines constants that indicate the state of an out-of-browser application window.
    public enum WindowState
    {
        //
        // Summary:
        //     The application window is in its normal state, occupying screen space based on
        //     its System.Windows.Window.Height and System.Windows.Window.Width values.
        Normal = 0,
        //
        // Summary:
        //     The application window is minimized to the taskbar.
        Minimized = 1,
        //
        // Summary:
        //     The application window is maximized to occupy the entire client area of the screen.
        Maximized = 2
    }
}
#endif
