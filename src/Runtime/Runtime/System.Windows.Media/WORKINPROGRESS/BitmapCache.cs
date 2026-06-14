
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

namespace System.Windows.Media;

/// <summary>
/// Represents the behavior of caching a visual element or tree of elements as bitmap surfaces.
/// This can yield significant performance improvements for some scenarios.
/// </summary>
[OpenSilver.NotImplemented]
public sealed class BitmapCache : CacheMode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapCache"/> class.
    /// </summary>
    public BitmapCache() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapCache"/> class with the specified render scale.
    /// </summary>
    /// <param name="renderAtScale">
    /// The scale at which the object is rendered on the cached bitmap surface.
    /// </param>
    public BitmapCache(double renderAtScale)
    {
        RenderAtScale = renderAtScale;
    }

    /// <summary>
    /// Identifies the <see cref="RenderAtScale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RenderAtScaleProperty =
        DependencyProperty.Register(
            nameof(RenderAtScale),
            typeof(double),
            typeof(BitmapCache),
            new PropertyMetadata(1.0));

    /// <summary>
    /// Gets or sets the scale at which the object is rendered on the cached bitmap surface.
    /// Use this property for cached objects that are scaled to improve performance.
    /// </summary>
    /// <returns>
    /// The scale at which the object is rendered as a cached bitmap. If you specify a negative 
    /// number, an error will be thrown. The default is 1.
    /// </returns>
    public double RenderAtScale
    {
        get => (double)GetValue(RenderAtScaleProperty);
        set => SetValueInternal(RenderAtScaleProperty, value);
    }
}
