/*====================================================================================
* OpenSilver — WPF compatibility enum for <see cref="Window.ResizeMode"/>.
\*====================================================================================*/

namespace System.Windows;

/// <summary>
/// Specifies whether a window can be resized and, if so, how (WPF compatibility).
/// </summary>
public enum ResizeMode
{
    NoResize = 0,
    CanMinimize = 1,
    CanResize = 2,
    CanResizeWithGrip = 3,
}
