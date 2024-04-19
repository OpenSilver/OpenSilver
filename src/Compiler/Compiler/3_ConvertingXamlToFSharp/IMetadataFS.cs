
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
    internal static class MetadatasFS
    {
        public static IMetadata Silverlight { get; } = new SLMetadataFS();
    }

    internal class SLMetadataFS : IMetadata
    {
        public string FieldModifier { get; } = "let mutable";

        public string SystemWindowsDLL { get; } = "System.Windows";

        public string SystemWindowsNS { get; } = "System.Windows";
        public string SystemWindowsDataNS { get; } = "System.Windows.Data";
        public string SystemWindowsDocumentsNS { get; } = "System.Windows.Documents";
        public string SystemWindowsControlsNS { get; } = "System.Windows.Controls";
        public string SystemWindowsMediaNS { get; } = "System.Windows.Media";
        public string SystemWindowsMediaAnimationNS { get; } = "System.Windows.Media.Animation";
    }
}
