
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

using OpenSilver.Internal;
using System.Reflection;

namespace System.Windows;

/// <summary>
/// The unique IDs for the system resource keys
/// </summary>
internal enum SystemResourceKeyID
{
    // ---- Colors and Brushes section ----
    InternalSystemColorsStart = 0,

    ActiveBorderBrush,
    InactiveBorderBrush,
    InactiveCaptionBrush,
    InactiveCaptionTextBrush,
    InfoBrush,
    InfoTextBrush,
    MenuBrush,
    MenuTextBrush,
    ScrollBarBrush,
    WindowBrush,
    WindowFrameBrush,
    HighlightTextBrush,
    HighlightBrush,
    ActiveCaptionBrush,
    ActiveCaptionTextBrush,
    AppWorkspaceBrush,
    ControlBrush,
    ControlDarkBrush,
    WindowTextBrush,
    ControlLightBrush,
    ControlLightLightBrush,
    ControlTextBrush,
    DesktopBrush,
    GrayTextBrush,
    ControlDarkDarkBrush,
    ActiveBorderColor,
    InactiveBorderColor,
    InactiveCaptionColor,
    InactiveCaptionTextColor,
    InfoColor,
    InfoTextColor,
    MenuColor,
    MenuTextColor,
    ScrollBarColor,
    WindowColor,
    WindowFrameColor,
    HighlightTextColor,
    HighlightColor,
    ActiveCaptionColor,
    ActiveCaptionTextColor,
    AppWorkspaceColor,
    ControlColor,
    ControlDarkColor,
    WindowTextColor,
    ControlLightColor,
    ControlLightLightColor,
    ControlTextColor,
    DesktopColor,
    GrayTextColor,
    ControlDarkDarkColor,

    InternalSystemColorsEnd,

    // ---- SystemParameters section ---
    InternalSystemParametersStart,

    MenuFade,
    MenuAnimation,
    MenuPopupAnimation,

    // ---- SystemThemeStyle section ---
    InternalSystemThemeStylesStart,

    FocusVisualStyle,

    InternalSystemParametersEnd,

    MenuItemSeparatorStyle,

    GridViewScrollViewerStyle,
    GridViewStyle,
    GridViewItemContainerStyle,

    InternalSystemThemeStylesEnd,
}

internal sealed class SystemResourceKey : ResourceKey
{
    /// <summary>
    /// Constructs a new instance of the key with the given ID.
    /// </summary>
    /// <param name="id">The internal, unique ID of the system resource.</param>
    internal SystemResourceKey(SystemResourceKeyID id)
    {
        ResourceKey = id;
    }

    internal SystemResourceKeyID ResourceKey { get; }

    internal object Resource
    {
        get
        {
            return ResourceKey switch
            {
                SystemResourceKeyID.ActiveBorderBrush => SystemColors.ActiveBorderBrush,
                SystemResourceKeyID.InactiveBorderBrush => SystemColors.InactiveBorderBrush,
                SystemResourceKeyID.InactiveCaptionBrush => SystemColors.InactiveCaptionBrush,
                SystemResourceKeyID.InactiveCaptionTextBrush => SystemColors.InactiveCaptionTextBrush,
                SystemResourceKeyID.InfoBrush => SystemColors.InfoBrush,
                SystemResourceKeyID.InfoTextBrush => SystemColors.InfoTextBrush,
                SystemResourceKeyID.MenuBrush => SystemColors.MenuBrush,
                SystemResourceKeyID.MenuTextBrush => SystemColors.MenuTextBrush,
                SystemResourceKeyID.ScrollBarBrush => SystemColors.ScrollBarBrush,
                SystemResourceKeyID.WindowBrush => SystemColors.WindowBrush,
                SystemResourceKeyID.WindowFrameBrush => SystemColors.WindowFrameBrush,
                SystemResourceKeyID.HighlightTextBrush => SystemColors.HighlightTextBrush,
                SystemResourceKeyID.HighlightBrush => SystemColors.HighlightBrush,
                SystemResourceKeyID.ActiveCaptionBrush => SystemColors.ActiveCaptionBrush,
                SystemResourceKeyID.ActiveCaptionTextBrush => SystemColors.ActiveCaptionTextBrush,
                SystemResourceKeyID.AppWorkspaceBrush => SystemColors.AppWorkspaceBrush,
                SystemResourceKeyID.ControlBrush => SystemColors.ControlBrush,
                SystemResourceKeyID.ControlDarkBrush => SystemColors.ControlDarkBrush,
                SystemResourceKeyID.WindowTextBrush => SystemColors.WindowTextBrush,
                SystemResourceKeyID.ControlLightBrush => SystemColors.ControlLightBrush,
                SystemResourceKeyID.ControlLightLightBrush => SystemColors.ControlLightLightBrush,
                SystemResourceKeyID.ControlTextBrush => SystemColors.ControlTextBrush,
                SystemResourceKeyID.DesktopBrush => SystemColors.DesktopBrush,
                SystemResourceKeyID.GrayTextBrush => SystemColors.GrayTextBrush,
                SystemResourceKeyID.ControlDarkDarkBrush => SystemColors.ControlDarkDarkBrush,
                SystemResourceKeyID.ActiveBorderColor => SystemColors.ActiveBorderColor,
                SystemResourceKeyID.InactiveBorderColor => SystemColors.InactiveBorderColor,
                SystemResourceKeyID.InactiveCaptionColor => SystemColors.InactiveCaptionColor,
                SystemResourceKeyID.InactiveCaptionTextColor => SystemColors.InactiveCaptionTextColor,
                SystemResourceKeyID.InfoColor => SystemColors.InfoColor,
                SystemResourceKeyID.InfoTextColor => SystemColors.InfoTextColor,
                SystemResourceKeyID.MenuColor => SystemColors.MenuColor,
                SystemResourceKeyID.MenuTextColor => SystemColors.MenuTextColor,
                SystemResourceKeyID.ScrollBarColor => SystemColors.ScrollBarColor,
                SystemResourceKeyID.WindowColor => SystemColors.WindowColor,
                SystemResourceKeyID.WindowFrameColor => SystemColors.WindowFrameColor,
                SystemResourceKeyID.HighlightTextColor => SystemColors.HighlightTextColor,
                SystemResourceKeyID.HighlightColor => SystemColors.HighlightColor,
                SystemResourceKeyID.ActiveCaptionColor => SystemColors.ActiveCaptionColor,
                SystemResourceKeyID.ActiveCaptionTextColor => SystemColors.ActiveCaptionTextColor,
                SystemResourceKeyID.AppWorkspaceColor => SystemColors.AppWorkspaceColor,
                SystemResourceKeyID.ControlColor => SystemColors.ControlColor,
                SystemResourceKeyID.ControlDarkColor => SystemColors.ControlDarkColor,
                SystemResourceKeyID.WindowTextColor => SystemColors.WindowTextColor,
                SystemResourceKeyID.ControlLightColor => SystemColors.ControlLightColor,
                SystemResourceKeyID.ControlLightLightColor => SystemColors.ControlLightLightColor,
                SystemResourceKeyID.ControlTextColor => SystemColors.ControlTextColor,
                SystemResourceKeyID.DesktopColor => SystemColors.DesktopColor,
                SystemResourceKeyID.GrayTextColor => SystemColors.GrayTextColor,
                SystemResourceKeyID.ControlDarkDarkColor => SystemColors.ControlDarkDarkColor,
                SystemResourceKeyID.MenuFade => BooleanBoxes.Box(SystemParameters.MenuFade),
                SystemResourceKeyID.MenuAnimation => BooleanBoxes.Box(SystemParameters.MenuAnimation),
                SystemResourceKeyID.MenuPopupAnimation => SystemParameters.MenuPopupAnimation,
                _ => null,
            };
        }
    }

    public override Assembly Assembly => null;

    public override bool Equals(object o) => o is SystemResourceKey key && key.ResourceKey == ResourceKey;

    public override int GetHashCode() => (int)ResourceKey;

    public override string ToString() => ResourceKey.ToString();
}
