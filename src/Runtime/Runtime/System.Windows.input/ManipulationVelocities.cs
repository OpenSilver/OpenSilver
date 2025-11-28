
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
/// Describes the speed at which manipulations occurs.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ManipulationVelocities : DependencyObject
{
    public ManipulationVelocities(Point linear, Point expansion)
    {
        LinearVelocity = linear;
        ExpansionVelocity = expansion;
    }

    /// <summary>
    /// Identifies the <see cref="ExpansionVelocity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionVelocityProperty =
        DependencyProperty.Register(
            nameof(ExpansionVelocity),
            typeof(Point),
            typeof(ManipulationVelocities),
            null);

    /// <summary>
    /// Gets the rate at which the manipulation resized.
    /// </summary>
    /// <returns>
    /// The rate at which the manipulation resized.
    /// </returns>
    public Point ExpansionVelocity
    {
        get => (Point)GetValue(ExpansionVelocityProperty);
        private set => SetValueInternal(ExpansionVelocityProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="LinearVelocity"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinearVelocityProperty =
        DependencyProperty.Register(
            nameof(LinearVelocity),
            typeof(Point),
            typeof(ManipulationVelocities),
            null);

    /// <summary>
    /// Gets the speed of linear motion.
    /// </summary>
    /// <returns>
    /// The speed of linear motion.
    /// </returns>
    public Point LinearVelocity
    {
        get => (Point)GetValue(LinearVelocityProperty);
        private set => SetValueInternal(LinearVelocityProperty, value);
    }
}
