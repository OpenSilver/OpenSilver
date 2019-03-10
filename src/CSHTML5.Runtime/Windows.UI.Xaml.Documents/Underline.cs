
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================




#if MIGRATION
#else
using Windows.UI.Text;
#endif

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    /// <summary>
    /// Provides an inline-level content element that causes content to render with a bold font weight.
    /// </summary>
    public sealed class Underline : Span
    {
        /// <summary>
        /// Initializes a new instance of the Underline class.
        /// </summary>
        public Underline()
        {
#if MIGRATION
            this.TextDecorations = System.Windows.TextDecorations.Underline;
#else
            this.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
#endif
        }
    }
}
