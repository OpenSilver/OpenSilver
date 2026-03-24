/*====================================================================================
* OpenSilver — WPF compatibility enum for <see cref="Popup.PopupAnimation"/>.
\*====================================================================================*/

namespace System.Windows.Controls.Primitives;

/// <summary>
/// Describes how to animate a <see cref="Popup"/> when it opens or closes (WPF compatibility).
/// </summary>
public enum PopupAnimation
{
    None = 0,
    Fade = 1,
    Slide = 2,
    Scroll = 3,
}
