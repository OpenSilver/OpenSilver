#if MIGRATION
namespace System.Windows.Automation.Text
#else
namespace Windows.UI.Xaml.Automation.Text
#endif
{
    //
    // Summary:
    //     Represents pre-defined units of text for the purposes of navigation within a
    //     document.
    public enum TextUnit
    {
        //
        // Summary:
        //     Specifies that the text unit is one character in length.
        Character = 0,
        //
        // Summary:
        //     Specifies that the text unit is the length of a single, common format specification,
        //     such as bold, italic, or similar.
        Format = 1,
        //
        // Summary:
        //     Specifies that the text unit is one word in length.
        Word = 2,
        //
        // Summary:
        //     Specifies that the text unit is one line in length.
        Line = 3,
        //
        // Summary:
        //     Specifies that the text unit is one paragraph in length.
        Paragraph = 4,
        //
        // Summary:
        //     Specifies that the text unit is one document-specific page in length.
        Page = 5,
        //
        // Summary:
        //     Specifies that the text unit is an entire document in length.
        Document = 6
    }
}
