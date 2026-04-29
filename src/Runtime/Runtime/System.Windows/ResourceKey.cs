
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

using System.Reflection;
using System.Windows.Markup;

namespace System.Windows;

/// <summary>
/// Provides an abstract base class for various resource keys.
/// </summary>
[MarkupExtensionReturnType(typeof(ResourceKey))]
public abstract class ResourceKey : MarkupExtension
{
    /// <summary>
    /// Gets an assembly object that indicates which assembly's dictionary to look in for the 
    /// value associated with this key.
    /// </summary>
    /// <returns>
    /// The retrieved assembly, as a reflection class.
    /// </returns>
    public abstract Assembly Assembly { get; }

    /// <summary>
    /// Returns this <see cref="ResourceKey"/>. Instances of this class are typically used as a key in a dictionary.
    /// </summary>
    /// <param name="serviceProvider">
    /// A service implementation that provides the desired value.
    /// </param>
    /// <returns>
    /// Calling this method always returns the instance itself.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
