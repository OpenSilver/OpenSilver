
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

using System.IO;
using System.Reflection;

namespace System.Windows;

/// <summary>
/// An assembly part is an assembly that is to be included in a Silverlight-based
/// application package (.xap).
/// </summary>
[OpenSilver.NotImplemented]
public sealed class AssemblyPart : DependencyObject
{
    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source),
            typeof(string),
            typeof(AssemblyPart),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets the <see cref="Uri"/> that identifies an assembly as an assembly part.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that is the assembly, which is identified as an assembly part.
    /// </returns>
    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Converts a <see cref="Stream"/> to an <see cref="Assembly"/> that is subsequently
    /// loaded into the current application domain.
    /// </summary>
    /// <param name="assemblyStream">
    /// The <see cref="Stream"/> to load into the current application domain.
    /// </param>
    /// <returns>
    /// The <see cref="Assembly"/> that is subsequently loaded into the current application domain.
    /// </returns>
    public Assembly Load(Stream assemblyStream) => null;
}
