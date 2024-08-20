
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

using System;

namespace OpenSilver.Internal.Data;

internal interface IPropertyPathNode
{
    object Value { get; set; }

    object Source { get; }

    bool IsBroken { get; }

    Type Type { get; }

    string PropertyName { get; }

    IPropertyPathNode Next { get; set; }

    void SetSource(object source, bool transferValue);

    void SetValue(object value);
}
