
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

using System.Collections.Generic;

namespace System.Windows.Markup;

/// <summary>
/// Unifies enumerable, collection, and dictionary support that are useful for exposing a dictionary of names in a XAML namescope.
/// </summary>
public interface INameScopeDictionary : INameScope, IDictionary<string, object>
{
}
