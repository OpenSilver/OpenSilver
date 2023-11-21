
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

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OpenSilver.Internal.Xaml
{
    /// <summary>
    /// Provides data for XAML Designer, primarily intended for internal use.
    /// </summary>
    /// <remarks>
    /// Important: This class is not intended for use in production code and should not be used by consumers of the library.
    /// It is subject to change or removal in future versions, and reliance on its functionality is discouraged.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class XamlDesignerBridge
    {
        private static readonly ConditionalWeakTable<IUIElement, XamlDesignerData> XamlDesignerElements = new();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG")]
        public static void SetFilePath(IUIElement uie, string filePath)
        {
            XamlDesignerElements.GetOrCreateValue(uie).FilePath = filePath;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG")]
        public static void SetPathInXaml(IUIElement uie, string pathInXaml)
        {
            XamlDesignerElements.GetOrCreateValue(uie).PathInXaml = pathInXaml;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetPathInXaml(IUIElement uie)
        {
            return XamlDesignerElements.TryGetValue(uie, out var val) ? val.PathInXaml : null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetFilePath(IUIElement uie)
        {
            return XamlDesignerElements.TryGetValue(uie, out var val) ? val.FilePath : null;
        }

        private sealed class XamlDesignerData
        {
            public string PathInXaml { get; set; }

            public string FilePath { get; set; }
        }
    }
}
