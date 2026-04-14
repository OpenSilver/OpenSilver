
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

namespace OpenSilver.Compiler;

[Flags]
public enum MemberKind
{
    Unknown = 0x00,
    Property = 0x01,
    AttachedPropertyGet = 0x02,
    AttachedPropertySet = 0x04,
    Event = 0x08,
    AttachedEvent = 0x10,
    Field = 0x20,
}
