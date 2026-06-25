
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

using System.Threading;
using System.Windows.Controls.Primitives;

namespace System.Windows;

/// <summary>
/// Contains properties that you can use to query system settings.
/// </summary>
public static class SystemParameters
{
    /// <summary>
    /// Gets a value that indicates whether the client computer is in high-contrast mode.
    /// </summary>
    /// <returns>
    /// true if the client computer is in high-contrast mode; otherwise, false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static bool HighContrast => false;

    /// <summary>
    /// Gets a value that indicates the number of lines to scroll vertically in response to mouse wheel events.
    /// </summary>
    /// <returns>
    /// The number of lines to scroll vertically in response to mouse wheel events. This always return 3.
    /// </returns>
    public static int WheelScrollLines => 3;

    private static SystemThemeKey _focusVisualStyleKey;

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the FocusVisualStyle property.
    /// </summary>
    /// <returns>
    /// The resource key.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static ResourceKey FocusVisualStyleKey => MakeResourceKey(ref _focusVisualStyleKey, SystemResourceKeyID.FocusVisualStyle);

    private static SystemResourceKey _menuFadeKey;

    /// <summary>
    /// Gets a value indicating whether menu fade animation is enabled.
    /// </summary>
    /// <returns>
    /// true when fade animation is enabled; otherwise, false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static bool MenuFade => false;

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="MenuFade"/> property.
    /// </summary>
    /// <returns>
    /// A resource key.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static ResourceKey MenuFadeKey => MakeResourceKey(ref _menuFadeKey, SystemResourceKeyID.MenuFade);

    private static SystemResourceKey _menuAnimationKey;

    /// <summary>
    /// Gets a value indicating whether the menu animation feature is enabled.
    /// </summary>
    /// <returns>
    /// true if menu animation is enabled; otherwise, false.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static bool MenuAnimation => false;

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="MenuAnimation"/> property.
    /// </summary>
    /// <returns>
    /// A resource key.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static ResourceKey MenuAnimationKey => MakeResourceKey(ref _menuAnimationKey, SystemResourceKeyID.MenuAnimation);

    private static SystemResourceKey _menuPopupAnimationKey;

    /// <summary>
    /// Gets the system value of the <see cref="Popup.PopupAnimation"/> property for menus.
    /// </summary>
    /// <returns>
    /// The pop-up animation property.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static PopupAnimation MenuPopupAnimation
    {
        get
        {
            if (MenuAnimation)
            {
                if (MenuFade)
                {
                    return PopupAnimation.Fade;
                }
                else
                {
                    return PopupAnimation.Scroll;
                }
            }

            return PopupAnimation.None;
        }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="MenuPopupAnimation"/> property.
    /// </summary>
    /// <returns>
    /// A resource key.
    /// </returns>
    [OpenSilver.NotImplemented]
    public static ResourceKey MenuPopupAnimationKey => MakeResourceKey(ref _menuPopupAnimationKey, SystemResourceKeyID.MenuPopupAnimation);

    private static SystemResourceKey MakeResourceKey(ref SystemResourceKey resourceKey, SystemResourceKeyID id)
    {
        if (resourceKey is null)
        {
            Interlocked.CompareExchange(ref resourceKey, new SystemResourceKey(id), null);
        }

        return resourceKey;
    }

    private static SystemThemeKey MakeResourceKey(ref SystemThemeKey resourceKey, SystemResourceKeyID id)
    {
        if (resourceKey is null)
        {
            Interlocked.CompareExchange(ref resourceKey, new SystemThemeKey(id), null);
        }

        return resourceKey;
    }
}
