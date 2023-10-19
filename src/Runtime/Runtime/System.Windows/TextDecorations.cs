
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
            underline = new TextDecorationCollection();
            underline.Decoration = new TextDecoration(TextDecorationLocation.Underline);

            // Striketrough
            strikethrough = new TextDecorationCollection();
            strikethrough.Decoration = new TextDecoration(TextDecorationLocation.Strikethrough);

            // Overline
            overline = new TextDecorationCollection();
            overline.Decoration = new TextDecoration(TextDecorationLocation.OverLine);

            ////Baseline
            //baseline = new TextDecorationCollection();
            //baseline.Decoration = new TextDecoration(TextDecorationLocation.Baseline);
        }

        /// <summary>
        /// Specifies an underlined text decoration.
        /// </summary>
        public static TextDecorationCollection Underline
        {
            get { return underline; }
        }

        /// <summary>
        /// Specifies a strikethrough text decoration
        /// </summary>
        public static TextDecorationCollection Strikethrough
        {
            get { return strikethrough; }
        }

        /// <summary>
        /// Specifies an overlined text decoration.
        /// </summary>
        public static TextDecorationCollection OverLine
        {
            get { return overline; }
        }

        //public static TextDecorationCollection Baseline
        //{
        //    get { return baseline; }
        //}

        private static readonly TextDecorationCollection underline;
        private static readonly TextDecorationCollection strikethrough;
        private static readonly TextDecorationCollection overline;
        //private static readonly TextDecorationCollection baseline;
    }

    /// <summary>
    ///     TextDecorationLocation - Referenced localization of the text decoration
    /// </summary>
    internal enum TextDecorationLocation
    {
        /// <summary>
        ///     Underline - Underline position
        /// </summary>
        Underline = 0,

        /// <summary>
        ///     OverLine - OverLine position
        /// </summary>
        OverLine = 1,

        /// <summary>
        ///     Strikethrough - Strikethrough position
        /// </summary>
        Strikethrough = 2,

        ///// <summary>
        /////     Baseline - Baseline position
        ///// </summary>
        //Baseline = 3,
    }
}
