#if WORKINPROGRESS
using System.Globalization;

namespace System.ServiceModel
{
    //
    // Summary:
    //     Represents the text of the reason of a SOAP fault.
    public partial class FaultReasonText
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.FaultReasonText class that
        //     uses a specified fault text.
        //
        // Parameters:
        //   text:
        //     The text that is the SOAP fault reason.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     text is null.
        public FaultReasonText(string text)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.FaultReasonText class that
        //     uses the specified fault text.
        //
        // Parameters:
        //   text:
        //     The text that is the SOAP fault reason.
        //
        //   xmlLang:
        //     The language of the fault.
        public FaultReasonText(string text, string xmlLang)
        {

        }

        //
        // Summary:
        //     Gets the text of the SOAP fault reason.
        //
        // Returns:
        //     The text of the SOAP fault reason.
        public string Text { get; }
        //
        // Summary:
        //     Gets the language of the SOAP fault reason.
        //
        // Returns:
        //     The language of the SOAP fault reason.
        public string XmlLang { get; }

        //
        // Summary:
        //     Returns a value that indicates whether the language of the description matches
        //     the provided System.Globalization.CultureInfo object.
        //
        // Parameters:
        //   cultureInfo:
        //     The System.Globalization.CultureInfo object to compare with the language of the
        //     description.
        //
        // Returns:
        //     true if the specified System.Globalization.CultureInfo matches; otherwise, false.
        public bool Matches(CultureInfo cultureInfo)
        {
            return false;
        }
    }
}
#endif