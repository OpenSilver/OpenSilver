
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

namespace System.Windows.Input;

/// <summary>
/// Contains transformation data that is accumulated when manipulation events occur.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ManipulationDelta : DependencyObject
{
    public ManipulationDelta(Point translation, Point scale)
    {
        Translation = translation;
        Scale = scale;
    }

    /// <summary>
    /// Identifies the <see cref="Scale"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ScaleProperty =
        DependencyProperty.Register(
            nameof(Scale),
            typeof(Point),
            typeof(ManipulationDelta),
            null);

    /// <summary>
    /// Gets the amount the manipulation has resized as a multiplier.
    /// </summary>
    /// <returns>
    /// The amount the manipulation has resized as a multiplier.
    /// </returns>
    public Point Scale
    {
        get => (Point)GetValue(ScaleProperty);
        private set => SetValueInternal(ScaleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Translation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TranslationProperty =
        DependencyProperty.Register(
            nameof(Translation),
            typeof(Point),
            typeof(ManipulationDelta),
            null);

    /// <summary>
    /// Gets the linear motion of the manipulation.
    /// </summary>
    /// <returns>
    /// The linear motion of the manipulation.
    /// </returns>
    public Point Translation
    {
        get => (Point)GetValue(TranslationProperty);
        private set => SetValueInternal(TranslationProperty, value);
    }
}
