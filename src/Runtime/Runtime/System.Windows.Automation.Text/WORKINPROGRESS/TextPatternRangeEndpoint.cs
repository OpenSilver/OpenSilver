#if MIGRATION
namespace System.Windows.Automation.Text
#else
namespace Windows.UI.Xaml.Automation.Text
#endif
{
    //
    // Summary:
    //     Identifies text range endpoints for methods of System.Windows.Automation.Provider.ITextRangeProvider.
    public enum TextPatternRangeEndpoint
    {
        //
        // Summary:
        //     The start point of the range.
        Start = 0,
        //
        // Summary:
        //     The endpoint of the range.
        End = 1
    }
}
