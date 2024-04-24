
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
/// Defines a base class for specifying parts of a Silverlight application that are
/// external to the application package (.xap file).
/// </summary>
[OpenSilver.NotImplemented]
public class ExternalPart : DependencyObject
{
    /// <summary>
    /// Gets or sets the URI of the external part.
    /// </summary>
    /// <returns>
    /// The URI of the external part.
    /// </returns>
    public virtual Uri Source { get; set; }
}
