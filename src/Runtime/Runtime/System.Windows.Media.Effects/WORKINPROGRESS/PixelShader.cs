
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

namespace System.Windows.Media.Effects;

/// <summary>
/// Provides a managed wrapper around a High Level Shading Language (HLSL) pixel shader.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class PixelShader : DependencyObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PixelShader"/> class.
    /// </summary>
    public PixelShader() { }

    /// <summary>
    /// Identifies the <see cref="UriSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UriSourceProperty =
        DependencyProperty.Register(
            nameof(UriSource),
            typeof(Uri),
            typeof(PixelShader),
            null);

    /// <summary>
    /// Gets or sets a URI reference to HLSL bytecode in the assembly.
    /// </summary>
    /// <returns>
    /// The URI reference to HLSL bytecode in the assembly.
    /// </returns>
    public Uri UriSource
    {
        get => (Uri)GetValue(UriSourceProperty);
        set => SetValueInternal(UriSourceProperty, value);
    }
}
