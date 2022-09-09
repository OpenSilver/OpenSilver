
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

#if MIGRATION
namespace System.Windows.Automation.Text
#else
namespace Windows.UI.Xaml.Automation.Text
#endif
{
    /// <summary>
    /// Represents pre-defined units of text for the purposes of navigation within a document.
    /// </summary>
    public enum TextUnit
    {
        /// <summary>
        /// Specifies that the text unit is one character in length.
        /// </summary>
        Character,
        /// <summary>
        /// Specifies that the text unit is the length of a single, common format specification, 
        /// such as bold, italic, or similar.
        /// </summary>
        Format,
        /// <summary>
        /// Specifies that the text unit is one word in length.
        /// </summary>
        Word,
        /// <summary>
        /// Specifies that the text unit is one line in length.
        /// </summary>
        Line,
        /// <summary>
        /// Specifies that the text unit is one paragraph in length.
        /// </summary>
        Paragraph,
        /// <summary>
        /// Specifies that the text unit is one document-specific page in length.
        /// </summary>
        Page,
        /// <summary>
        /// Specifies that the text unit is an entire document in length.
        /// </summary>
        Document,
    }
}
