
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

namespace OpenSilver.Compiler
{
    internal interface IMetadataVB
    {
        string FieldModifier { get; }

        string SystemWindowsDLL { get; }

        string SystemWindowsNS { get; }
        string SystemWindowsDataNS { get; }
        string SystemWindowsControlsNS { get; }
        string SystemWindowsMediaNS { get; }
        string SystemWindowsMediaAnimationNS { get; }
    }

    internal static class MetadatasVB
    {
        public static SLMetadataVB Silverlight { get; } = new SLMetadataVB();

        public static IMetadataVB UWP { get; } = new UWPMetadataVB();

        internal class SLMetadataVB : IMetadataVB
        {
            public string FieldModifier { get; } = "Friend";

            public string SystemWindowsDLL { get; } = "System.Windows";

            public string SystemWindowsNS { get; } = "System.Windows";
            public string SystemWindowsDataNS { get; } = "System.Windows.Data";
            public string SystemWindowsControlsNS { get; } = "System.Windows.Controls";
            public string SystemWindowsMediaNS { get; } = "System.Windows.Media";
            public string SystemWindowsMediaAnimationNS { get; } = "System.Windows.Media.Animation";
        }

        private class UWPMetadataVB : IMetadataVB
        {
            public string FieldModifier { get; } = "Protected";

            public string SystemWindowsDLL => throw new NotSupportedException();

            public string SystemWindowsNS { get; } = "Windows.UI.Xaml";
            public string SystemWindowsDataNS { get; } = "Windows.UI.Xaml.Data";
            public string SystemWindowsControlsNS { get; } = "Windows.UI.Xaml.Controls";
            public string SystemWindowsMediaNS { get; } = "Windows.UI.Xaml.Media";
            public string SystemWindowsMediaAnimationNS { get; } = "Windows.UI.Xaml.Media.Animation";
        }
    }
}
