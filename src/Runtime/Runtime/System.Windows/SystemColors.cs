
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

using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows;

/// <summary>
/// Contains system colors, system brushes, and system resource keys that correspond to system 
/// display elements.
/// </summary>
public static class SystemColors
{
    private static bool _areColorsLoaded;

    private static Color _activeBorderColor;
    private static SolidColorBrush _activeBorderBrush;
    private static SystemResourceKey _activeBorderColorKey;
    private static SystemResourceKey _activeBorderBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the active window's border.
    /// </summary>
    /// <returns>
    /// The color of the active window's border.
    /// </returns>
    public static Color ActiveBorderColor
    {
        get
        {
            EnsureColors();
            return _activeBorderColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush" /> that is the color of the active window's border.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the 
    /// color of the active window's border. The returned brush is frozen, so it cannot be modified.
    /// </returns>
    public static SolidColorBrush ActiveBorderBrush => MakeBrush(ref _activeBorderBrush, ActiveBorderColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of the active window's border.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of the active window's border.
    /// </returns>
    public static ResourceKey ActiveBorderColorKey => MakeResourceKey(ref _activeBorderColorKey, SystemResourceKeyID.ActiveBorderColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> used to paint the 
    /// active window's border.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> used to paint the active window's border.
    /// This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ActiveBorderBrushKey => MakeResourceKey(ref _activeBorderBrushKey, SystemResourceKeyID.ActiveBorderBrush);

    private static Color _activeCaptionColor;
    private static SolidColorBrush _activeCaptionBrush;
    private static SystemResourceKey _activeCaptionColorKey;
    private static SystemResourceKey _activeCaptionBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the background color of the
    /// active window's title bar.
    /// </summary>
    /// <returns>
    /// The background color of the active window's title bar.
    /// </returns>
    public static Color ActiveCaptionColor
    {
        get
        {
            EnsureColors();
            return _activeCaptionColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the background of the active 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the 
    /// background color of the active window's title bar. The returned brush is frozen, so it 
    /// cannot be modified.
    /// </returns>
    public static SolidColorBrush ActiveCaptionBrush => MakeBrush(ref _activeCaptionBrush, ActiveCaptionColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of the active 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of the active window's title bar.
    /// </returns>
    public static ResourceKey ActiveCaptionColorKey => MakeResourceKey(ref _activeCaptionColorKey, SystemResourceKeyID.ActiveCaptionColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> used to paint the 
    /// background of the active window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> used to paint the background of the 
    /// active window's title bar. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ActiveCaptionBrushKey => MakeResourceKey(ref _activeCaptionBrushKey, SystemResourceKeyID.ActiveCaptionBrush);

    private static Color _activeCaptionTextColor;
    private static SolidColorBrush _activeCaptionTextBrush;
    private static SystemResourceKey _activeCaptionTextColorKey;
    private static SystemResourceKey _activeCaptionTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the text in
    /// the active window's title bar.
    /// </summary>
    /// <returns>
    /// The color of the active window's title bar.
    /// </returns>
    public static Color ActiveCaptionTextColor
    {
        get
        {
            EnsureColors();
            return _activeCaptionTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the text in the active window's
    /// title bar.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the 
    /// background color of the color of the text in the active window's title bar. The returned 
    /// brush is frozen, so it cannot be modified.
    /// </returns>
    public static SolidColorBrush ActiveCaptionTextBrush => MakeBrush(ref _activeCaptionTextBrush, ActiveCaptionTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of the text in the active 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of the text in the active window's title bar.
    /// </returns>
    public static ResourceKey ActiveCaptionTextColorKey => MakeResourceKey(ref _activeCaptionTextColorKey, SystemResourceKeyID.ActiveCaptionTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// text in the active window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the text in the active 
    /// window's title bar. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ActiveCaptionTextBrushKey => MakeResourceKey(ref _activeCaptionTextBrushKey, SystemResourceKeyID.ActiveCaptionTextBrush);

    private static Color _appWorkspaceColor;
    private static SolidColorBrush _appWorkspaceBrush;
    private static SystemResourceKey _appWorkspaceColorKey;
    private static SystemResourceKey _appWorkspaceBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the application
    /// workspace.
    /// </summary>
    /// <returns>
    /// The color of the application workspace.
    /// </returns>
    public static Color AppWorkspaceColor
    {
        get
        {
            EnsureColors();
            return _appWorkspaceColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the application workspace.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the 
    /// color of the application workspace. The returned brush is frozen, so it cannot be modified.
    /// </returns>
    public static SolidColorBrush AppWorkspaceBrush => MakeBrush(ref _appWorkspaceBrush, AppWorkspaceColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of the application workspace.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of the application workspace.
    /// </returns>
    public static ResourceKey AppWorkspaceColorKey => MakeResourceKey(ref _appWorkspaceColorKey, SystemResourceKeyID.AppWorkspaceColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// application workspace.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the application workspace. 
    /// This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey AppWorkspaceBrushKey => MakeResourceKey(ref _appWorkspaceBrushKey, SystemResourceKeyID.AppWorkspaceBrush);

    private static Color _controlColor;
    private static SolidColorBrush _controlBrush;
    private static SystemResourceKey _controlColorKey;
    private static SystemResourceKey _controlBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the face color of a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The face color of a three-dimensional display element.
    /// </returns>
    public static Color ControlColor
    {
        get
        {
            EnsureColors();
            return _controlColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the face color of a three-dimensional 
    /// display element.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to 
    /// the face color of a three-dimensional display element. The returned brush is frozen,
    /// so it cannot be modified.
    /// </returns>
    public static SolidColorBrush ControlBrush => MakeBrush(ref _controlBrush, ControlColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the face <see cref="Color"/> of a three-dimensional 
    /// display element.
    /// </summary>
    /// <returns>
    /// The resource key for the face <see cref="Color"/> of a three-dimensional display element.
    /// </returns>
    public static ResourceKey ControlColorKey => MakeResourceKey(ref _controlColorKey, SystemResourceKeyID.ControlColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// face of a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the face of a three-dimensional 
    /// display element. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ControlBrushKey => MakeResourceKey(ref _controlBrushKey, SystemResourceKeyID.ControlBrush);

    private static Color _controlDarkColor;
    private static SolidColorBrush _controlDarkBrush;
    private static SystemResourceKey _controlDarkColorKey;
    private static SystemResourceKey _controlDarkBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the shadow color of a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The shadow color of a three-dimensional display element.
    /// </returns>
    public static Color ControlDarkColor
    {
        get
        {
            EnsureColors();
            return _controlDarkColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the shadow color of a three-dimensional 
    /// display element.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the 
    /// shadow color of a three-dimensional display element. The returned brush is frozen, so it 
    /// cannot be modified.
    /// </returns>
    public static SolidColorBrush ControlDarkBrush => MakeBrush(ref _controlDarkBrush, ControlDarkColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the shadow <see cref="Color"/> of a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The resource key for the shadow <see cref="Color"/> of a three-dimensional display element.
    /// </returns>
    public static ResourceKey ControlDarkColorKey => MakeResourceKey(ref _controlDarkColorKey, SystemResourceKeyID.ControlDarkColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// shadow of a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the shadow of a 
    /// three-dimensional display element. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ControlDarkBrushKey => MakeResourceKey(ref _controlDarkBrushKey, SystemResourceKeyID.ControlDarkBrush);

    private static Color _controlDarkDarkColor;
    private static SolidColorBrush _controlDarkDarkBrush;
    private static SystemResourceKey _controlDarkDarkColorKey;
    private static SystemResourceKey _controlDarkDarkBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the dark shadow color of
    /// a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The dark shadow color of a three-dimensional display element.
    /// </returns>
    public static Color ControlDarkDarkColor
    {
        get
        {
            EnsureColors();
            return _controlDarkDarkColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the dark shadow color of a three-dimensional 
    /// display element.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the 
    /// dark shadow color of a three-dimensional display element. The returned brush is frozen, so 
    /// it cannot be modified.
    /// </returns>
    public static SolidColorBrush ControlDarkDarkBrush => MakeBrush(ref _controlDarkDarkBrush, ControlDarkDarkColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the dark shadow <see cref="Color"/> of the highlight 
    /// color of a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the dark shadow <see cref="Color"/> of a three-dimensional display element.
    /// </returns>
    public static ResourceKey ControlDarkDarkColorKey => MakeResourceKey(ref _controlDarkDarkColorKey, SystemResourceKeyID.ControlDarkDarkColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the dark 
    /// shadow of a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the dark shadow of a 
    /// three-dimensional display element. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ControlDarkDarkBrushKey => MakeResourceKey(ref _controlDarkDarkBrushKey, SystemResourceKeyID.ControlDarkDarkBrush);

    private static Color _controlLightColor;
    private static SolidColorBrush _controlLightBrush;
    private static SystemResourceKey _controlLightColorKey;
    private static SystemResourceKey _controlLightBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the light color of a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The light color of a three-dimensional display element.
    /// </returns>
    public static Color ControlLightColor
    {
        get
        {
            EnsureColors();
            return _controlLightColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the light color of a three-dimensional display 
    /// element.
    /// </summary>
    /// <returns>
    /// A <see cref="SolidColorBrush"/> with its <see cref="SolidColorBrush.Color"/> set to the light 
    /// color of a three-dimensional display element. The returned brush is frozen, so it cannot be 
    /// modified.
    /// </returns>
    public static SolidColorBrush ControlLightBrush => MakeBrush(ref _controlLightBrush, ControlLightColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the highlight <see cref="Color"/> of a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The resource key for the highlight <see cref="Color"/> of a three-dimensional display element.
    /// </returns>
    public static ResourceKey ControlLightColorKey => MakeResourceKey(ref _controlLightColorKey, SystemResourceKeyID.ControlLightColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the light 
    /// area of a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the light area of a 
    /// three-dimensional display element. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ControlLightBrushKey => MakeResourceKey(ref _controlLightBrushKey, SystemResourceKeyID.ControlLightBrush);

    private static Color _controlLightLightColor;
    private static SolidColorBrush _controlLightLightBrush;
    private static SystemResourceKey _controlLightLightColorKey;
    private static SystemResourceKey _controlLightLightBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the highlight color of a
    /// three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The highlight color of a three-dimensional display element.
    /// </returns>
    public static Color ControlLightLightColor
    {
        get
        {
            EnsureColors();
            return _controlLightLightColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the highlight color of a three-dimensional display
    /// element.
    /// </summary>
    /// <returns>
    /// The highlight color of a three-dimensional display element.
    /// </returns>
    public static SolidColorBrush ControlLightLightBrush => MakeBrush(ref _controlLightLightBrush, ControlLightLightColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the highlight <see cref="Color"/> of a three-dimensional 
    /// display element.
    /// </summary>
    /// <returns>
    /// The resource key for the highlight <see cref="Color"/> of a three-dimensional display element.
    /// </returns>
    public static ResourceKey ControlLightLightColorKey => MakeResourceKey(ref _controlLightLightColorKey, SystemResourceKeyID.ControlLightLightColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the highlight 
    /// of a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the highlight of a three-dimensional 
    /// display element. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ControlLightLightBrushKey => MakeResourceKey(ref _controlLightLightBrushKey, SystemResourceKeyID.ControlLightLightBrush);

    private static Color _controlTextColor;
    private static SolidColorBrush _controlTextBrush;
    private static SystemResourceKey _controlTextColorKey;
    private static SystemResourceKey _controlTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of text in a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The color of text in a three-dimensional display element.
    /// </returns>
    public static Color ControlTextColor
    {
        get
        {
            EnsureColors();
            return _controlTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of text in a three-dimensional 
    /// display element.
    /// </summary>
    /// <returns>
    /// The color of text in a three-dimensional display element.
    /// </returns>
    public static SolidColorBrush ControlTextBrush => MakeBrush(ref _controlTextBrush, ControlTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of text in a three-dimensional
    /// display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of text in a three-dimensional display element.
    /// </returns>
    public static ResourceKey ControlTextColorKey => MakeResourceKey(ref _controlTextColorKey, SystemResourceKeyID.ControlTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints text in 
    /// a three-dimensional display element.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints text in a three-dimensional 
    /// display element. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ControlTextBrushKey => MakeResourceKey(ref _controlTextBrushKey, SystemResourceKeyID.ControlTextBrush);

    private static Color _desktopColor;
    private static SolidColorBrush _desktopBrush;
    private static SystemResourceKey _desktopColorKey;
    private static SystemResourceKey _desktopBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the desktop.
    /// </summary>
    /// <returns>
    /// The color of the desktop.
    /// </returns>
    public static Color DesktopColor
    {
        get
        {
            EnsureColors();
            return _desktopColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the desktop.
    /// </summary>
    /// <returns>
    /// The color of the desktop.
    /// </returns>
    public static SolidColorBrush DesktopBrush => MakeBrush(ref _desktopBrush, DesktopColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of the desktop.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of the desktop.
    /// </returns>
    public static ResourceKey DesktopColorKey => MakeResourceKey(ref _desktopColorKey, SystemResourceKeyID.DesktopColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// desktop.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the desktop. This brush
    /// is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey DesktopBrushKey => MakeResourceKey(ref _desktopBrushKey, SystemResourceKeyID.DesktopBrush);

    private static Color _grayTextColor;
    private static SolidColorBrush _grayTextBrush;
    private static SystemResourceKey _grayTextColorKey;
    private static SystemResourceKey _grayTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of disabled text.
    /// </summary>
    /// <returns>
    /// The color of disabled text.
    /// </returns>
    public static Color GrayTextColor
    {
        get
        {
            EnsureColors();
            return _grayTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of disabled text.
    /// </summary>
    /// <returns>
    /// The color of disabled text.
    /// </returns>
    public static SolidColorBrush GrayTextBrush => MakeBrush(ref _grayTextBrush, GrayTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of disabled text.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of disabled text.
    /// </returns>
    public static ResourceKey GrayTextColorKey => MakeResourceKey(ref _grayTextColorKey, SystemResourceKeyID.GrayTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints 
    /// disabled text.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints disabled text. This 
    /// brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey GrayTextBrushKey => MakeResourceKey(ref _grayTextBrushKey, SystemResourceKeyID.GrayTextBrush);

    private static Color _highlightColor;
    private static SolidColorBrush _highlightBrush;
    private static SystemResourceKey _highlightColorKey;
    private static SystemResourceKey _highlightBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the background color of selected
    /// items.
    /// </summary>
    /// <returns>
    /// The background color of selected items.
    /// </returns>
    public static Color HighlightColor
    {
        get
        {
            EnsureColors();
            return _highlightColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that paints the background of selected items.
    /// </summary>
    /// <returns>
    /// The background color of selected items.
    /// </returns>
    public static SolidColorBrush HighlightBrush => MakeBrush(ref _highlightBrush, HighlightColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of selected items.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of selected items.
    /// </returns>
    public static ResourceKey HighlightColorKey => MakeResourceKey(ref _highlightColorKey, SystemResourceKeyID.HighlightColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// background of selected items.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the background of selected 
    /// items. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey HighlightBrushKey => MakeResourceKey(ref _highlightBrushKey, SystemResourceKeyID.HighlightBrush);

    private static Color _highlightTextColor;
    private static SolidColorBrush _highlightTextBrush;
    private static SystemResourceKey _highlightTextColorKey;
    private static SystemResourceKey _highlightTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the text of
    /// selected items.
    /// </summary>
    /// <returns>
    /// The color of the text of selected items.
    /// </returns>
    public static Color HighlightTextColor
    {
        get
        {
            EnsureColors();
            return _highlightTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the text of selected items.
    /// </summary>
    /// <returns>
    /// The color of the text of selected items.
    /// </returns>
    public static SolidColorBrush HighlightTextBrush => MakeBrush(ref _highlightTextBrush, HighlightTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of a selected item's text.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of a selected item's text.
    /// </returns>
    public static ResourceKey HighlightTextColorKey => MakeResourceKey(ref _highlightTextColorKey, SystemResourceKeyID.HighlightTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// text of selected items.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the text of selected 
    /// items. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey HighlightTextBrushKey => MakeResourceKey(ref _highlightTextBrushKey, SystemResourceKeyID.HighlightTextBrush);

    private static Color _inactiveBorderColor;
    private static SolidColorBrush _inactiveBorderBrush;
    private static SystemResourceKey _inactiveBorderColorKey;
    private static SystemResourceKey _inactiveBorderBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of an inactive
    /// window's border.
    /// </summary>
    /// <returns>
    /// The color of an inactive window's border.
    /// </returns>
    public static Color InactiveBorderColor
    {
        get
        {
            EnsureColors();
            return _inactiveBorderColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of an inactive window's border.
    /// </summary>
    /// <returns>
    /// The color of an inactive window's border.
    /// </returns>
    public static SolidColorBrush InactiveBorderBrush => MakeBrush(ref _inactiveBorderBrush, InactiveBorderColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of an inactive window's border.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of an inactive window's border.
    /// </returns>
    public static ResourceKey InactiveBorderColorKey => MakeResourceKey(ref _inactiveBorderColorKey, SystemResourceKeyID.InactiveBorderColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the border 
    /// of an inactive window.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the border of an inactive 
    /// window. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey InactiveBorderBrushKey => MakeResourceKey(ref _inactiveBorderBrushKey, SystemResourceKeyID.InactiveBorderBrush);

    private static Color _inactiveCaptionColor;
    private static SolidColorBrush _inactiveCaptionBrush;
    private static SystemResourceKey _inactiveCaptionColorKey;
    private static SystemResourceKey _inactiveCaptionBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the background color of an
    /// inactive window's title bar.
    /// </summary>
    /// <returns>
    /// The background color of an inactive window's title bar.
    /// </returns>
    public static Color InactiveCaptionColor
    {
        get
        {
            EnsureColors();
            return _inactiveCaptionColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the background color of an inactive 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// The background color of an inactive window's title bar.
    /// </returns>
    public static SolidColorBrush InactiveCaptionBrush => MakeBrush(ref _inactiveCaptionBrush, InactiveCaptionColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of an inactive 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of an inactive window's title bar.
    /// </returns>
    public static ResourceKey InactiveCaptionColorKey => MakeResourceKey(ref _inactiveCaptionColorKey, SystemResourceKeyID.InactiveCaptionColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// background of an inactive window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the background of an 
    /// inactive window's title bar. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey InactiveCaptionBrushKey => MakeResourceKey(ref _inactiveCaptionBrushKey, SystemResourceKeyID.InactiveCaptionBrush);

    private static Color _inactiveCaptionTextColor;
    private static SolidColorBrush _inactiveCaptionTextBrush;
    private static SystemResourceKey _inactiveCaptionTextColorKey;
    private static SystemResourceKey _inactiveCaptionTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the text of
    /// an inactive window's title bar.
    /// </summary>
    /// <returns>
    /// The color of the text of an inactive window's title bar.
    /// </returns>
    public static Color InactiveCaptionTextColor
    {
        get
        {
            EnsureColors();
            return _inactiveCaptionTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the text of an inactive 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// The color of the text of an inactive window's title bar.
    /// </returns>
    public static SolidColorBrush InactiveCaptionTextBrush => MakeBrush(ref _inactiveCaptionTextBrush, InactiveCaptionTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of the text of an inactive 
    /// window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of the text of an inactive window's title bar.
    /// </returns>
    public static ResourceKey InactiveCaptionTextColorKey => MakeResourceKey(ref _inactiveCaptionTextColorKey, SystemResourceKeyID.InactiveCaptionTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the text 
    /// of an inactive window's title bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the text of an inactive 
    /// window's title bar. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey InactiveCaptionTextBrushKey => MakeResourceKey(ref _inactiveCaptionTextBrushKey, SystemResourceKeyID.InactiveCaptionTextBrush);

    private static Color _infoColor;
    private static SolidColorBrush _infoBrush;
    private static SystemResourceKey _infoColorKey;
    private static SystemResourceKey _infoBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the background color for
    /// the <see cref="ToolTip"/> control.
    /// </summary>
    /// <returns>
    /// The background color for the <see cref="ToolTip"/> control.
    /// </returns>
    public static Color InfoColor
    {
        get
        {
            EnsureColors();
            return _infoColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the background color for the 
    /// <see cref="ToolTip"/> control.
    /// </summary>
    /// <returns>
    /// The background color for the <see cref="ToolTip"/> control.
    /// </returns>
    public static SolidColorBrush InfoBrush => MakeBrush(ref _infoBrush, InfoColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of the 
    /// <see cref="ToolTip"/> control.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of the <see cref="ToolTip"/> 
    /// control.
    /// </returns>
    public static ResourceKey InfoColorKey => MakeResourceKey(ref _infoColorKey, SystemResourceKeyID.InfoColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints 
    /// the background of the <see cref="ToolTip"/> control.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the background of 
    /// the <see cref="ToolTip"/> control. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey InfoBrushKey => MakeResourceKey(ref _infoBrushKey, SystemResourceKeyID.InfoBrush);

    private static Color _infoTextColor;
    private static SolidColorBrush _infoTextBrush;
    private static SystemResourceKey _infoTextColorKey;
    private static SystemResourceKey _infoTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the text color for the <see cref="ToolTip"/>
    /// control.
    /// </summary>
    /// <returns>
    /// The text color for the <see cref="ToolTip"/> control.
    /// </returns>
    public static Color InfoTextColor
    {
        get
        {
            EnsureColors();
            return _infoTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the text color for the <see cref="ToolTip"/>
    /// control.
    /// </summary>
    /// <returns>
    /// The text color for the <see cref="ToolTip"/> control.
    /// </returns>
    public static SolidColorBrush InfoTextBrush => MakeBrush(ref _infoTextBrush, InfoTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of the text in a <see cref="ToolTip"/> 
    /// control.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of the text in a <see cref="ToolTip"/> control.
    /// </returns>
    public static ResourceKey InfoTextColorKey => MakeResourceKey(ref _infoTextColorKey, SystemResourceKeyID.InfoTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the text 
    /// in a <see cref="ToolTip"/> control.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the text in a <see cref="ToolTip"/> 
    /// control. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey InfoTextBrushKey => MakeResourceKey(ref _infoTextBrushKey, SystemResourceKeyID.InfoTextBrush);

    private static Color _menuColor;
    private static SolidColorBrush _menuBrush;
    private static SystemResourceKey _menuColorKey;
    private static SystemResourceKey _menuBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of a menu's background.
    /// </summary>
    /// <returns>
    /// The color of a menu's background.
    /// </returns>
    public static Color MenuColor
    {
        get
        {
            EnsureColors();
            return _menuColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of a menu's background.
    /// </summary>
    /// <returns>
    /// The color of a menu's background.
    /// </returns>
    public static SolidColorBrush MenuBrush => MakeBrush(ref _menuBrush, MenuColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of a menu.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of a menu.
    /// </returns>
    public static ResourceKey MenuColorKey => MakeResourceKey(ref _menuColorKey, SystemResourceKeyID.MenuColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// background of a menu.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the background of a 
    /// menu. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey MenuBrushKey => MakeResourceKey(ref _menuBrushKey, SystemResourceKeyID.MenuBrush);

    private static Color _menuTextColor;
    private static SolidColorBrush _menuTextBrush;
    private static SystemResourceKey _menuTextColorKey;
    private static SystemResourceKey _menuTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of a menu's text.
    /// </summary>
    /// <returns>
    /// The color of a menu's text.
    /// </returns>
    public static Color MenuTextColor
    {
        get
        {
            EnsureColors();
            return _menuTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of a menu's text.
    /// </summary>
    /// <returns>
    /// The color of a menu's text.
    /// </returns>
    public static SolidColorBrush MenuTextBrush => MakeBrush(ref _menuTextBrush, MenuTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of a menu's text.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of a menu's text.
    /// </returns>
    public static ResourceKey MenuTextColorKey => MakeResourceKey(ref _menuTextColorKey, SystemResourceKeyID.MenuTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints a 
    /// menu's text.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints a menu's text. This 
    /// brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey MenuTextBrushKey => MakeResourceKey(ref _menuTextBrushKey, SystemResourceKeyID.MenuTextBrush);

    private static Color _scrollBarColor;
    private static SolidColorBrush _scrollBarBrush;
    private static SystemResourceKey _scrollBarColorKey;
    private static SystemResourceKey _scrollBarBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the background color of a
    /// scroll bar.
    /// </summary>
    /// <returns>
    /// The background color of a scroll bar.
    /// </returns>
    public static Color ScrollBarColor
    {
        get
        {
            EnsureColors();
            return _scrollBarColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the background color of a scroll bar.
    /// </summary>
    /// <returns>
    /// The background color of a scroll bar.
    /// </returns>
    public static SolidColorBrush ScrollBarBrush => MakeBrush(ref _scrollBarBrush, ScrollBarColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of a scroll bar.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of a scroll bar.
    /// </returns>
    public static ResourceKey ScrollBarColorKey => MakeResourceKey(ref _scrollBarColorKey, SystemResourceKeyID.ScrollBarColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// background of a scroll bar.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the background of a 
    /// scroll bar. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey ScrollBarBrushKey => MakeResourceKey(ref _scrollBarBrushKey, SystemResourceKeyID.ScrollBarBrush);

    private static Color _windowColor;
    private static SolidColorBrush _windowBrush;
    private static SystemResourceKey _windowColorKey;
    private static SystemResourceKey _windowBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the background color in the
    /// client area of a window.
    /// </summary>
    /// <returns>
    /// The background color in the client area of a window.
    /// </returns>
    public static Color WindowColor
    {
        get
        {
            EnsureColors();
            return _windowColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the background color in the client area of 
    /// a window.
    /// </summary>
    /// <returns>
    /// The background color in the client area of a window.
    /// </returns>
    public static SolidColorBrush WindowBrush => MakeBrush(ref _windowBrush, WindowColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the background <see cref="Color"/> of a window's 
    /// client area.
    /// </summary>
    /// <returns>
    /// The resource key for the background <see cref="Color"/> of a window's client area.
    /// </returns>
    public static ResourceKey WindowColorKey => MakeResourceKey(ref _windowColorKey, SystemResourceKeyID.WindowColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the 
    /// background of a window's client area.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the background of a 
    /// window's client area. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey WindowBrushKey => MakeResourceKey(ref _windowBrushKey, SystemResourceKeyID.WindowBrush);

    private static Color _windowFrameColor;
    private static SolidColorBrush _windowFrameBrush;
    private static SystemResourceKey _windowFrameColorKey;
    private static SystemResourceKey _windowFrameBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of a window frame.
    /// </summary>
    /// <returns>
    /// The color of a window frame.
    /// </returns>
    public static Color WindowFrameColor
    {
        get
        {
            EnsureColors();
            return _windowFrameColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of a window frame.
    /// </summary>
    /// <returns>
    /// The color of a window frame.
    /// </returns>
    public static SolidColorBrush WindowFrameBrush => MakeBrush(ref _windowFrameBrush, WindowFrameColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of a window frame.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of a window frame.
    /// </returns>
    public static ResourceKey WindowFrameColorKey => MakeResourceKey(ref _windowFrameColorKey, SystemResourceKeyID.WindowFrameColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints a 
    /// window frame.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints a window frame. This 
    /// brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey WindowFrameBrushKey => MakeResourceKey(ref _windowFrameBrushKey, SystemResourceKeyID.WindowFrameBrush);

    private static Color _windowTextColor;
    private static SolidColorBrush _windowTextBrush;
    private static SystemResourceKey _windowTextColorKey;
    private static SystemResourceKey _windowTextBrushKey;

    /// <summary>
    /// Gets a <see cref="Color"/> structure that is the color of the text in
    /// the client area of a window.
    /// </summary>
    /// <returns>
    /// The color of the text in the client area of a window.
    /// </returns>
    public static Color WindowTextColor
    {
        get
        {
            EnsureColors();
            return _windowTextColor;
        }
    }

    /// <summary>
    /// Gets a <see cref="SolidColorBrush"/> that is the color of the text in the client area of 
    /// a window.
    /// </summary>
    /// <returns>
    /// The color of the text in the client area of a window.
    /// </returns>
    public static SolidColorBrush WindowTextBrush => MakeBrush(ref _windowTextBrush, WindowTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="Color"/> of text in a window's client area.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="Color"/> of text in a window's client area.
    /// </returns>
    public static ResourceKey WindowTextColorKey => MakeResourceKey(ref _windowTextColorKey, SystemResourceKeyID.WindowTextColor);

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the <see cref="SolidColorBrush"/> that paints the text in
    /// the client area of a window.
    /// </summary>
    /// <returns>
    /// The resource key for the <see cref="SolidColorBrush"/> that paints the text in the client area of 
    /// a window. This brush is frozen, so it cannot be modified.
    /// </returns>
    public static ResourceKey WindowTextBrushKey => MakeResourceKey(ref _windowTextBrushKey, SystemResourceKeyID.WindowTextBrush);

    private static bool IsHighContrast() =>
        OpenSilver.Interop.ExecuteJavaScriptBoolean("window.matchMedia('(forced-colors: active)').matches");

    private static void EnsureColors()
    {
        if (_areColorsLoaded) return;

        _areColorsLoaded = true;

        bool isHighContrast = IsHighContrast();

        _activeBorderColor = GetSystemColor("ActiveBorder") ??
            (isHighContrast ? Color.FromRgb(0, 0, 255) : Color.FromRgb(180, 180, 180));
        _activeCaptionColor = GetSystemColor("ActiveCaption") ??
            (isHighContrast ? Color.FromRgb(0, 0, 255) : Color.FromRgb(153, 180, 209));
        _activeCaptionTextColor = GetSystemColor("CaptionText") ??
            (isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
        _appWorkspaceColor = GetSystemColor("AppWorkspace") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(171, 171, 171));
        _controlColor = GetSystemColor("ButtonFace") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(240, 240, 240));
        _controlTextColor = GetSystemColor("ButtonText") ??
            (isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
        _desktopColor = GetSystemColor("Background") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(0, 0, 0));
        _grayTextColor = GetSystemColor("GrayText") ??
            (isHighContrast ? Color.FromRgb(0, 255, 0) : Color.FromRgb(109, 109, 109));
        _highlightColor = GetSystemColor("Highlight") ??
            (isHighContrast ? Color.FromRgb(0, 128, 0) : Color.FromRgb(0, 120, 215));
        _highlightTextColor = GetSystemColor("HighlightText") ??
            (isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(255, 255, 255));
        _inactiveBorderColor = GetSystemColor("InactiveBorder") ??
            (isHighContrast ? Color.FromRgb(0, 255, 255) : Color.FromRgb(244, 247, 252));
        _inactiveCaptionColor = GetSystemColor("InactiveCaption") ??
            (isHighContrast ? Color.FromRgb(0, 255, 255) : Color.FromRgb(191, 205, 219));
        _inactiveCaptionTextColor = GetSystemColor("InactiveCaptionText") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(0, 0, 0));
        _infoColor = GetSystemColor("InfoBackground") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(255, 255, 225));
        _infoTextColor = GetSystemColor("InfoText") ??
            (isHighContrast ? Color.FromRgb(255, 255, 0) : Color.FromRgb(0, 0, 0));
        _menuColor = GetSystemColor("Menu") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(240, 240, 240));
        _menuTextColor = GetSystemColor("MenuText") ??
            (isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(0, 0, 0));
        _scrollBarColor = GetSystemColor("ScrollBar") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(200, 200, 200));
        _windowColor = GetSystemColor("Window") ??
            (isHighContrast ? Color.FromRgb(0, 0, 0) : Color.FromRgb(255, 255, 255));
        _windowFrameColor = GetSystemColor("WindowFrame") ??
            (isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(100, 100, 100));
        _windowTextColor = GetSystemColor("WindowText") ??
            (isHighContrast ? Color.FromRgb(255, 255, 0) : Color.FromRgb(0, 0, 0));
        _controlDarkColor = isHighContrast ? Color.FromRgb(128, 128, 128) : Color.FromRgb(160, 160, 160);
        _controlDarkDarkColor = isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(105, 105, 105);
        _controlLightColor = isHighContrast ? Color.FromRgb(255, 255, 255) : Color.FromRgb(227, 227, 227);
        _controlLightLightColor = isHighContrast ? Color.FromRgb(192, 192, 192) : Color.FromRgb(255, 255, 255);
    }

    private static Color? GetSystemColor(string color)
    {
        string str = OpenSilver.Interop.ExecuteJavaScriptString($"osjs.getSystemColor('{color}')");
        if (str is not null && str.StartsWith("rgb("))
        {
            str = str.Substring(4, str.Length - 5);
            string[] rgb = str.Split([','], StringSplitOptions.RemoveEmptyEntries);
            if (rgb.Length == 3 &&
                byte.TryParse(rgb[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out byte r) &&
                byte.TryParse(rgb[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out byte g) &&
                byte.TryParse(rgb[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out byte b))
            {
                return Color.FromRgb(r, g, b);
            }
        }

        return null;
    }

    private static SolidColorBrush MakeBrush(ref SolidColorBrush brush, Color color)
    {
        if (brush is null)
        {
            var newBrush = new SolidColorBrush(color);
            newBrush.Seal();
            Interlocked.CompareExchange(ref brush, newBrush, null);
        }

        return brush;
    }

    private static SystemResourceKey MakeResourceKey(ref SystemResourceKey resourceKey, SystemResourceKeyID id)
    {
        if (resourceKey is null)
        {
            Interlocked.CompareExchange(ref resourceKey, new SystemResourceKey(id), null);
        }

        return resourceKey;
    }
}
