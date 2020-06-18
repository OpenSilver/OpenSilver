

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
namespace System.Windows
{
    public static class TextDecorations
    {
        private static readonly TextDecorationCollection none = new TextDecorationCollection(new TextDecoration(0));
        //private static readonly TextDecorationCollection baseline = new TextDecorationCollection(new TextDecoration(1));
        private static readonly TextDecorationCollection overline = new TextDecorationCollection(new TextDecoration(2));
        private static readonly TextDecorationCollection strikethrough = new TextDecorationCollection(new TextDecoration(3));
        private static readonly TextDecorationCollection underline = new TextDecorationCollection(new TextDecoration(4));

        //public static TextDecorationCollection Baseline
        //{
        //    get { return baseline; }
        //}

        public static TextDecorationCollection OverLine
        {
            get { return overline; }
        }

        public static TextDecorationCollection Strikethrough
        {
            get { return strikethrough; }
        }

        public static TextDecorationCollection Underline
        {
            get { return underline; }
        }

        internal static TextDecorationCollection None
        {
            get { return none; }
        }
    }
}
#else
namespace Windows.UI.Text
{
    /// <summary>
    /// Provides a set of predefined text decorations.
    /// </summary>
    public enum TextDecorations
    {
        /// <summary>
        /// Defines a line above the text
        /// </summary>
        None = 0,

        ///// <summary>
        ///// Defines a line below the text
        ///// </summary>
        //Baseline = 1,

        /// <summary>
        /// Defines a line above the text
        /// </summary>
        OverLine = 2,

        /// <summary>
        /// Defines a line through the text
        /// </summary>
        Strikethrough = 3,

        /// <summary>
        /// Defines a line below the text
        /// </summary>
        Underline = 4
    }
}
#endif