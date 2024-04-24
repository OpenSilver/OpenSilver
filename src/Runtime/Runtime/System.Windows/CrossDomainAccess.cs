
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

namespace System.Windows;

/// <summary>
/// Defines values that specify the access that cross-domain callers have to a Silverlight-based application.
/// </summary>
public enum CrossDomainAccess
{
    /// <summary>
    /// Cross-domain callers have no access to the Silverlight application.
    /// </summary>
    NoAccess = 0,

    /// <summary>
    /// Cross-domain callers have script access to the Silverlight application.
    /// </summary>
    ScriptableOnly = 2,
}
