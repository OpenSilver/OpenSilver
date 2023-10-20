
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

using OpenSilver.Internal;
using System;
using System.ComponentModel;

namespace JSIL
{
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class Verbatim
    {
        public static dynamic Expression(string javascript)
        {
            throw new NotImplementedException();
        }
        public static dynamic Expression(string javascript, params object[] variables)
        {
            throw new NotImplementedException();
        }
    }
}