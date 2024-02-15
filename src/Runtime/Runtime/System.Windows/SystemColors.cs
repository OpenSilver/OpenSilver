
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
using System.Windows.Media;

namespace System.Windows
{
    /// <summary>
    /// Contains system colors, system brushes, and system resource keys that correspond
    /// to system display elements.
    /// </summary>
    public static class SystemColors
    {
        private static bool _areColorsLoaded;

        private static Color _activeBorderColor;

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

        private static Color _activeCaptionColor;

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

        private static Color _activeCaptionTextColor;
        
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

        private static Color _appWorkspaceColor;

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

        private static Color _controlColor;
        
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

        private static Color _controlDarkColor;

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

        private static Color _controlDarkDarkColor;

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

        private static Color _controlLightColor;

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

        private static Color _controlLightLightColor;

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

        private static Color _controlTextColor;

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

        private static Color _desktopColor;

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

        private static Color _grayTextColor;

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

        private static Color _highlightColor;

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

        private static Color _highlightTextColor;

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

        private static Color _inactiveBorderColor;

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

        private static Color _inactiveCaptionColor;

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

        private static Color _inactiveCaptionTextColor;

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

        private static Color _infoColor;

        /// <summary>
        /// Gets a <see cref="Color"/> structure that is the background color for
        /// the <see cref="Controls.ToolTip"/> control.
        /// </summary>
        /// <returns>
        /// The background color for the <see cref="Controls.ToolTip"/> control.
        /// </returns>
        public static Color InfoColor
        {
            get
            {
                EnsureColors();
                return _infoColor;
            }
        }

        private static Color _infoTextColor;

        /// <summary>
        /// Gets a <see cref="Color"/> structure that is the text color for the <see cref="Controls.ToolTip"/>
        /// control.
        /// </summary>
        /// <returns>
        /// The text color for the <see cref="Controls.ToolTip"/> control.
        /// </returns>
        public static Color InfoTextColor
        {
            get
            {
                EnsureColors();
                return _infoTextColor;
            }
        }

        private static Color _menuColor;

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

        private static Color _menuTextColor;

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

        private static Color _scrollBarColor;

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

        private static Color _windowColor;

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

        private static Color _windowFrameColor;

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

        private static Color _windowTextColor;

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

        private static bool IsHighContrast() =>
            OpenSilver.Interop.ExecuteJavaScriptBoolean("window.matchMedia('(forced-colors: active)').matches;");

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
            string str = OpenSilver.Interop.ExecuteJavaScriptString($"document.getSystemColor('{color}');");
            if (str is not null && str.StartsWith("rgb("))
            {
                str = str.Substring(4, str.Length - 5);
                string[] rgb = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
    }
}
