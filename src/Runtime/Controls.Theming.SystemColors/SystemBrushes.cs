// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Media;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Implements the Brush forms of System.Windows.SystemColors which only
    /// provides Colors on Silverlight.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class SystemBrushes
    {
        /// <summary>
        /// Gets a Brush for SystemColors.ActiveBorderColor.
        /// </summary>
        public static Brush ActiveBorderBrush
        {
            get
            {
                if (_activeBorderBrush == null)
                {
                    _activeBorderBrush = new SolidColorBrush(SystemColors.ActiveBorderColor);
                }
                return _activeBorderBrush;
            }
        }

        /// <summary>
        /// Backing store for the ActiveBorderBrush property.
        /// </summary>
        private static Brush _activeBorderBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ActiveCaptionColor.
        /// </summary>
        public static Brush ActiveCaptionBrush
        {
            get
            {
                if (_activeCaptionBrush == null)
                {
                    _activeCaptionBrush = new SolidColorBrush(SystemColors.ActiveCaptionColor);
                }
                return _activeCaptionBrush;
            }
        }

        /// <summary>
        /// Backing store for the ActiveCaptionBrush property.
        /// </summary>
        private static Brush _activeCaptionBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ActiveCaptionTextColor.
        /// </summary>
        public static Brush ActiveCaptionTextBrush
        {
            get
            {
                if (_activeCaptionTextBrush == null)
                {
                    _activeCaptionTextBrush = new SolidColorBrush(SystemColors.ActiveCaptionTextColor);
                }
                return _activeCaptionTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the ActiveCaptionTextBrush property.
        /// </summary>
        private static Brush _activeCaptionTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.AppWorkspaceColor.
        /// </summary>
        public static Brush AppWorkspaceBrush
        {
            get
            {
                if (_appWorkspaceBrush == null)
                {
                    _appWorkspaceBrush = new SolidColorBrush(SystemColors.AppWorkspaceColor);
                }
                return _appWorkspaceBrush;
            }
        }

        /// <summary>
        /// Backing store for the AppWorkspaceBrush property.
        /// </summary>
        private static Brush _appWorkspaceBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ControlColor.
        /// </summary>
        public static Brush ControlBrush
        {
            get
            {
                if (_controlBrush == null)
                {
                    _controlBrush = new SolidColorBrush(SystemColors.ControlColor);
                }
                return _controlBrush;
            }
        }

        /// <summary>
        /// Backing store for the ControlBrush property.
        /// </summary>
        private static Brush _controlBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ControlDarkColor.
        /// </summary>
        public static Brush ControlDarkBrush
        {
            get
            {
                if (_controlDarkBrush == null)
                {
                    _controlDarkBrush = new SolidColorBrush(SystemColors.ControlDarkColor);
                }
                return _controlDarkBrush;
            }
        }

        /// <summary>
        /// Backing store for the ControlDarkBrush property.
        /// </summary>
        private static Brush _controlDarkBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ControlDarkDarkColor.
        /// </summary>
        public static Brush ControlDarkDarkBrush
        {
            get
            {
                if (_controlDarkDarkBrush == null)
                {
                    _controlDarkDarkBrush = new SolidColorBrush(SystemColors.ControlDarkDarkColor);
                }
                return _controlDarkDarkBrush;
            }
        }

        /// <summary>
        /// Backing store for the ControlDarkDarkBrush property.
        /// </summary>
        private static Brush _controlDarkDarkBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ControlLightColor.
        /// </summary>
        public static Brush ControlLightBrush
        {
            get
            {
                if (_controlLightBrush == null)
                {
                    _controlLightBrush = new SolidColorBrush(SystemColors.ControlLightColor);
                }
                return _controlLightBrush;
            }
        }

        /// <summary>
        /// Backing store for the ControlLightBrush property.
        /// </summary>
        private static Brush _controlLightBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ControlLightLightColor.
        /// </summary>
        public static Brush ControlLightLightBrush
        {
            get
            {
                if (_controlLightLightBrush == null)
                {
                    _controlLightLightBrush = new SolidColorBrush(SystemColors.ControlLightLightColor);
                }
                return _controlLightLightBrush;
            }
        }

        /// <summary>
        /// Backing store for the ControlLightLightBrush property.
        /// </summary>
        private static Brush _controlLightLightBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ControlTextColor.
        /// </summary>
        public static Brush ControlTextBrush
        {
            get
            {
                if (_controlTextBrush == null)
                {
                    _controlTextBrush = new SolidColorBrush(SystemColors.ControlTextColor);
                }
                return _controlTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the ControlTextBrush property.
        /// </summary>
        private static Brush _controlTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.DesktopColor.
        /// </summary>
        public static Brush DesktopBrush
        {
            get
            {
                if (_desktopBrush == null)
                {
                    _desktopBrush = new SolidColorBrush(SystemColors.DesktopColor);
                }
                return _desktopBrush;
            }
        }

        /// <summary>
        /// Backing store for the DesktopBrush property.
        /// </summary>
        private static Brush _desktopBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.GrayTextColor.
        /// </summary>
        public static Brush GrayTextBrush
        {
            get
            {
                if (_grayTextBrush == null)
                {
                    _grayTextBrush = new SolidColorBrush(SystemColors.GrayTextColor);
                }
                return _grayTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the GrayTextBrush property.
        /// </summary>
        private static Brush _grayTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.HighlightColor.
        /// </summary>
        public static Brush HighlightBrush
        {
            get
            {
                if (_highlightBrush == null)
                {
                    _highlightBrush = new SolidColorBrush(SystemColors.HighlightColor);
                }
                return _highlightBrush;
            }
        }

        /// <summary>
        /// Backing store for the HighlightBrush property.
        /// </summary>
        private static Brush _highlightBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.HighlightTextColor.
        /// </summary>
        public static Brush HighlightTextBrush
        {
            get
            {
                if (_highlightTextBrush == null)
                {
                    _highlightTextBrush = new SolidColorBrush(SystemColors.HighlightTextColor);
                }
                return _highlightTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the HighlightTextBrush property.
        /// </summary>
        private static Brush _highlightTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.InactiveBorderColor.
        /// </summary>
        public static Brush InactiveBorderBrush
        {
            get
            {
                if (_inactiveBorderBrush == null)
                {
                    _inactiveBorderBrush = new SolidColorBrush(SystemColors.InactiveBorderColor);
                }
                return _inactiveBorderBrush;
            }
        }

        /// <summary>
        /// Backing store for the InactiveBorderBrush property.
        /// </summary>
        private static Brush _inactiveBorderBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.InactiveCaptionColor.
        /// </summary>
        public static Brush InactiveCaptionBrush
        {
            get
            {
                if (_inactiveCaptionBrush == null)
                {
                    _inactiveCaptionBrush = new SolidColorBrush(SystemColors.InactiveCaptionColor);
                }
                return _inactiveCaptionBrush;
            }
        }

        /// <summary>
        /// Backing store for the InactiveCaptionBrush property.
        /// </summary>
        private static Brush _inactiveCaptionBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.InactiveCaptionTextColor.
        /// </summary>
        public static Brush InactiveCaptionTextBrush
        {
            get
            {
                if (_inactiveCaptionTextBrush == null)
                {
                    _inactiveCaptionTextBrush = new SolidColorBrush(SystemColors.InactiveCaptionTextColor);
                }
                return _inactiveCaptionTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the InactiveCaptionTextBrush property.
        /// </summary>
        private static Brush _inactiveCaptionTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.InfoColor.
        /// </summary>
        public static Brush InfoBrush
        {
            get
            {
                if (_infoBrush == null)
                {
                    _infoBrush = new SolidColorBrush(SystemColors.InfoColor);
                }
                return _infoBrush;
            }
        }

        /// <summary>
        /// Backing store for the InfoBrush property.
        /// </summary>
        private static Brush _infoBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.InfoTextColor.
        /// </summary>
        public static Brush InfoTextBrush
        {
            get
            {
                if (_infoTextBrush == null)
                {
                    _infoTextBrush = new SolidColorBrush(SystemColors.InfoTextColor);
                }
                return _infoTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the InfoTextBrush property.
        /// </summary>
        private static Brush _infoTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.MenuColor.
        /// </summary>
        public static Brush MenuBrush
        {
            get
            {
                if (_menuBrush == null)
                {
                    _menuBrush = new SolidColorBrush(SystemColors.MenuColor);
                }
                return _menuBrush;
            }
        }

        /// <summary>
        /// Backing store for the MenuBrush property.
        /// </summary>
        private static Brush _menuBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.MenuTextColor.
        /// </summary>
        public static Brush MenuTextBrush
        {
            get
            {
                if (_menuTextBrush == null)
                {
                    _menuTextBrush = new SolidColorBrush(SystemColors.MenuTextColor);
                }
                return _menuTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the MenuTextBrush property.
        /// </summary>
        private static Brush _menuTextBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.ScrollBarColor.
        /// </summary>
        public static Brush ScrollBarBrush
        {
            get
            {
                if (_scrollBarBrush == null)
                {
                    _scrollBarBrush = new SolidColorBrush(SystemColors.ScrollBarColor);
                }
                return _scrollBarBrush;
            }
        }

        /// <summary>
        /// Backing store for the ScrollBarBrush property.
        /// </summary>
        private static Brush _scrollBarBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.WindowColor.
        /// </summary>
        public static Brush WindowBrush
        {
            get
            {
                if (_windowBrush == null)
                {
                    _windowBrush = new SolidColorBrush(SystemColors.WindowColor);
                }
                return _windowBrush;
            }
        }

        /// <summary>
        /// Backing store for the WindowBrush property.
        /// </summary>
        private static Brush _windowBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.WindowFrameColor.
        /// </summary>
        public static Brush WindowFrameBrush
        {
            get
            {
                if (_windowFrameBrush == null)
                {
                    _windowFrameBrush = new SolidColorBrush(SystemColors.WindowFrameColor);
                }
                return _windowFrameBrush;
            }
        }

        /// <summary>
        /// Backing store for the WindowFrameBrush property.
        /// </summary>
        private static Brush _windowFrameBrush;

        /// <summary>
        /// Gets a Brush for SystemColors.WindowTextColor.
        /// </summary>
        public static Brush WindowTextBrush
        {
            get
            {
                if (_windowTextBrush == null)
                {
                    _windowTextBrush = new SolidColorBrush(SystemColors.WindowTextColor);
                }
                return _windowTextBrush;
            }
        }

        /// <summary>
        /// Backing store for the WindowTextBrush property.
        /// </summary>
        private static Brush _windowTextBrush;

        /// <summary>
        /// Gets a Brush for the gradient of a Button.
        /// </summary>
        public static Brush ButtonGradient
        {
            get
            {
                if (_buttonGradient == null)
                {
                    _buttonGradient = new LinearGradientBrush();
                    _buttonGradient.StartPoint = new Point(.5, 0);
                    _buttonGradient.EndPoint = new Point(.5, .65);

                    GradientStop lightColor = new GradientStop();
                    lightColor.Color = SystemColors.ControlLightLightColor;
                    lightColor.Offset = 0;
                    _buttonGradient.GradientStops.Add(lightColor);

                    GradientStop darkColor = new GradientStop();
                    darkColor.Color = SystemColors.ControlColor;
                    darkColor.Offset = 1;
                    _buttonGradient.GradientStops.Add(darkColor);
                }
                return _buttonGradient;
            }
        }

        /// <summary>
        /// Backing store for the ButtonGradient property.
        /// </summary>
        private static LinearGradientBrush _buttonGradient;

        /// <summary>
        /// Gets a Brush for the reversed gradient of a Button.
        /// </summary>
        public static Brush ButtonGradientReverse
        {
            get
            {
                if (_buttonGradientReverse == null)
                {
                    _buttonGradientReverse = new LinearGradientBrush();
                    _buttonGradientReverse.StartPoint = new Point(.5, 0);
                    _buttonGradientReverse.EndPoint = new Point(.5, 1);

                    GradientStop lightColor = new GradientStop();
                    lightColor.Color = SystemColors.ControlLightLightColor;
                    lightColor.Offset = 1;
                    _buttonGradientReverse.GradientStops.Add(lightColor);

                    GradientStop darkColor = new GradientStop();
                    darkColor.Color = SystemColors.ControlColor;
                    darkColor.Offset = 0;
                    _buttonGradientReverse.GradientStops.Add(darkColor);
                }
                return _buttonGradientReverse;
            }
        }

        /// <summary>
        /// Backing store for the ButtonGradientReverse property.
        /// </summary>
        private static LinearGradientBrush _buttonGradientReverse;

        /// <summary>
        /// Gets a Brush for the horizontal gradient of a Button.
        /// </summary>
        public static Brush ButtonGradientHorizontal
        {
            get
            {
                if (_buttonGradientHorizontal == null)
                {
                    _buttonGradientHorizontal = new LinearGradientBrush();
                    _buttonGradientHorizontal.StartPoint = new Point(0, .5);
                    _buttonGradientHorizontal.EndPoint = new Point(.75, .5);

                    GradientStop lightColor = new GradientStop();
                    lightColor.Color = SystemColors.ControlLightLightColor;
                    lightColor.Offset = 0;
                    _buttonGradientHorizontal.GradientStops.Add(lightColor);

                    GradientStop darkColor = new GradientStop();
                    darkColor.Color = SystemColors.ControlColor;
                    darkColor.Offset = 1;
                    _buttonGradientHorizontal.GradientStops.Add(darkColor);
                }
                return _buttonGradientHorizontal;
            }
        }

        /// <summary>
        /// Backing store for the ButtonGradientHorizontal property.
        /// </summary>
        private static LinearGradientBrush _buttonGradientHorizontal;

        /// <summary>
        /// Gets a Brush for the gradient of a Calendar.
        /// </summary>
        public Brush CalendarGradient
        {
            get
            {
                if (_calendarGradient == null)
                {
                    _calendarGradient = new LinearGradientBrush();
                    _calendarGradient.StartPoint = new Point(.5, 0);
                    _calendarGradient.EndPoint = new Point(.5, 1);

                    // Create and add Gradient stops
                    GradientStop faintColor = new GradientStop();
                    faintColor.Color = SystemColors.ControlColor;
                    faintColor.Offset = 0;
                    _calendarGradient.GradientStops.Add(faintColor);

                    // Create and add Gradient stops
                    GradientStop faintColor2 = new GradientStop();
                    faintColor2.Color = SystemColors.ControlColor;
                    faintColor2.Offset = .16;
                    _calendarGradient.GradientStops.Add(faintColor2);

                    GradientStop noColor = new GradientStop();
                    noColor.Color = SystemColors.WindowColor;
                    noColor.Offset = .16;
                    _calendarGradient.GradientStops.Add(noColor);

                    GradientStop noColor2 = new GradientStop();
                    noColor2.Color = SystemColors.WindowColor;
                    noColor2.Offset = 1;
                    _calendarGradient.GradientStops.Add(noColor2);
                }
                return _calendarGradient;
            }
        }

        /// <summary>
        /// Backing store for the CalendarGradient property.
        /// </summary>
        private static LinearGradientBrush _calendarGradient;
    }
}
