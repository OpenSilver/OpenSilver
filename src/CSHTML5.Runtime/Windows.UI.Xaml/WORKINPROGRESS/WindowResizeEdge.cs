#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    //
    // Summary:
    //     Defines constants that represent the edges and corners of a Silverlight out-of-browser
    //     application window.
    public enum WindowResizeEdge
    {
        //
        // Summary:
        //     The left edge of the window.
        Left = 1,
        //
        // Summary:
        //     The right edge of the window.
        Right = 2,
        //
        // Summary:
        //     The upper edge of the window.
        Top = 3,
        //
        // Summary:
        //     The upper-left corner of the window.
        TopLeft = 4,
        //
        // Summary:
        //     The upper-right corner of the window.
        TopRight = 5,
        //
        // Summary:
        //     The lower edge of the window.
        Bottom = 6,
        //
        // Summary:
        //     The lower-left corner of the window.
        BottomLeft = 7,
        //
        // Summary:
        //     The lower-right corner of the window.
        BottomRight = 8
    }
}
#endif