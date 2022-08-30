
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Automation.Provider
#else
namespace Windows.UI.Xaml.Automation.Provider
#endif
{
    /// <summary>
    /// Exposes methods and properties to support UI Automation client access to 
    /// controls that contain text.
    /// </summary>
    public interface ITextProvider
    {
        /// <summary>
        /// Retrieves a collection of disjoint text ranges that are associated with the 
        /// current text selection or selections.
        /// </summary>
        /// <returns>
        /// A collection of disjoint text ranges.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If the UI Automation provider does not support text selection.
        /// </exception>
        ITextRangeProvider[] GetSelection();

        /// <summary>
        /// Retrieves an array of disjoint text ranges from a text container. Each text 
        /// range begins with the first partially visible line and ends with the last 
        /// partially visible line.
        /// </summary>
        /// <returns>
        /// The collection of visible text ranges within a container or an empty array. 
        /// This method never returns null.
        /// </returns>
        ITextRangeProvider[] GetVisibleRanges();

        /// <summary>
        /// Retrieves a text range that encloses a child element, such as an image, 
        /// hyperlink, or other embedded object.
        /// </summary>
        /// <param name="childElement">
        /// The enclosed object.
        /// </param>
        /// <returns>
        /// A range that spans the child element.
        /// </returns>
        ITextRangeProvider RangeFromChild(IRawElementProviderSimple childElement);

        /// <summary>
        /// Returns the degenerate (empty) text range that is nearest to the specified 
        /// screen coordinates.
        /// </summary>
        /// <param name="screenLocation">
        /// The location, in screen coordinates.
        /// </param>
        /// <returns>
        /// A degenerate range nearest the specified location. This method never returns null.
        /// </returns>
        ITextRangeProvider RangeFromPoint(Point screenLocation);

        /// <summary>
        /// Gets a text range that encloses the main text of a document.
        /// </summary>
        /// <returns>
        /// A text range that encloses the main text of a document.
        /// </returns>
        ITextRangeProvider DocumentRange { get; }

        /// <summary>
        /// Gets a value that specifies whether a text provider supports selection, and if it 
        /// does, the type of selection that is supported.
        /// </summary>
        /// <returns>
        /// A value of <see cref="Automation.SupportedTextSelection" />.
        /// </returns>
        SupportedTextSelection SupportedTextSelection { get; }
    }
}
