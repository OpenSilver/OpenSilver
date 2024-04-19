
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
    internal interface IMetadata
    {
        string FieldModifier { get; }

        string SystemWindowsDLL { get; }

        string SystemWindowsNS { get; }
        string SystemWindowsDataNS { get; }
        string SystemWindowsDocumentsNS { get; }
        string SystemWindowsControlsNS { get; }
        string SystemWindowsMediaNS { get; }
        string SystemWindowsMediaAnimationNS { get; }
    }
}
