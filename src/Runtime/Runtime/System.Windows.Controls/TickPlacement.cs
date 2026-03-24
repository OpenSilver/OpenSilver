/*====================================================================================
* OpenSilver — WPF compatibility enum for <see cref="Slider.TickPlacement"/>.
\*====================================================================================*/

namespace System.Windows.Controls;

/// <summary>
/// Specifies the position of tick marks in a <see cref="Slider"/> (WPF compatibility).
/// </summary>
public enum TickPlacement
{
    None = 0,
    TopLeft = 1,
    BottomRight = 2,
    Both = 3,
}
