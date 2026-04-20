
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

namespace System.Windows
{
    /// <summary>
    /// Implements a set of predefined text decorations.
    /// </summary>
    public static class TextDecorations
    {
        static TextDecorations()
        {
            // Underline
            Underline = new TextDecorationCollection(TextDecorationLocation.Underline);

            // Striketrough
            Strikethrough = new TextDecorationCollection(TextDecorationLocation.Strikethrough);

            // Overline
            OverLine = new TextDecorationCollection(TextDecorationLocation.OverLine);

            ////Baseline
            Baseline = new TextDecorationCollection(TextDecorationLocation.Baseline);
        }

        /// <summary>
        /// Specifies an underlined text decoration.
        /// </summary>
        public static TextDecorationCollection Underline { get; }

        /// <summary>
        /// Specifies a strikethrough text decoration
        /// </summary>
        public static TextDecorationCollection Strikethrough { get; }

        /// <summary>
        /// Specifies an overlined text decoration.
        /// </summary>
        public static TextDecorationCollection OverLine { get; }

        /// <summary>
        /// Specifies a baseline text decoration.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static TextDecorationCollection Baseline { get; }
    }

    /// <summary>
    /// Specifies the vertical position of a TextDecoration object.
    /// </summary>
    public enum TextDecorationLocation
    {
        /// <summary>
        /// The vertical position of an underline. This is the default value.
        /// </summary>
        Underline = 0,

        /// <summary>
        /// The vertical position of an overline.
        /// </summary>
        OverLine = 1,

        /// <summary>
        /// The vertical position of a strikethrough.
        /// </summary>
        Strikethrough = 2,

        ///// <summary>
        ///// The vertical position of a baseline.
        ///// </summary>
        [OpenSilver.NotImplemented]
        Baseline = 3,
    }
}
