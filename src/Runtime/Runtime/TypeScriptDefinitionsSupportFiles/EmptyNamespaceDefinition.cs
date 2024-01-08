
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

namespace AnonymousTypes
{
    // This class allow the AnonymousTypes namespace to be visible
    // even if there are no AnonymousTypes defined in the typescript project
    // if this namespace doesn't exists the typescript will not compile
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class AnonymousType0
    {

    }
}