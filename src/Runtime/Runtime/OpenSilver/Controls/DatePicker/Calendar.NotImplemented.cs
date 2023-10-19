
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpenSilver.Controls
{
    /// <summary>
    /// Represents a control that enables a user to select a date by using a visual calendar display.
    /// </summary>
    public partial class Calendar
    {
        /// <summary>
        /// Gets or sets a collection of dates that are marked as not selectable.
        /// </summary>
        /// <returns>
        /// A collection of dates that cannot be selected. The default value is an empty collection.
        /// </returns>
        [NotImplemented]
        public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

        /// <summary>
        /// Identifies the <see cref="DisplayDate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="DisplayDate" /> dependency property.
        /// </returns>
        [NotImplemented]
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.Today));

        /// <summary>Occurs when the <see cref="DisplayDate" /> property is changed.</summary>     
        [NotImplemented]
        public event EventHandler<CalendarDateChangedEventArgs> DisplayDateChanged;

        [NotImplemented]
        public Style CalendarDayButtonStyle { get; set; }

        [NotImplemented]
        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.Register(nameof(IsTabStop), typeof(bool), typeof(Calendar), new PropertyMetadata(true));

        [NotImplemented]
        public bool IsTabStop
        {
            get { return (bool)GetValue(IsTabStopProperty); }
            set { SetValue(IsTabStopProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(Calendar), new PropertyMetadata((object)null));

        [NotImplemented]
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(Calendar), new PropertyMetadata(new Thickness()));

        [NotImplemented]
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(Calendar), new PropertyMetadata(new Thickness()));

        [NotImplemented]
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(Calendar), new PropertyMetadata((object)null));

        [NotImplemented]
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register(nameof(Template), typeof(ControlTemplate), typeof(Calendar), new PropertyMetadata((object)null));

        [NotImplemented]
        public ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        /// <remarks>
        /// The brush that paints the foreground of the control.
        /// The default value is System.Windows.Media.Colors.Black.
        /// </remarks>
        [NotImplemented]
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Calendar), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// </summary>
        /// <remarks>
        /// The font used to display text in the control. The default is the "Portable User Interface".
        /// </remarks>
        [NotImplemented]
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [NotImplemented]
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Calendar), new PropertyMetadata(new FontFamily("Portable User Interface")));

        [NotImplemented]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.FontSize"/> dependency property.
        /// </summary>
        [NotImplemented]
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(Calendar), new PropertyMetadata(11d));

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is displayed in months, years, or decades.
        /// </summary>
        /// <returns>
        /// A value indicating what length of time the <see cref="Calendar" /> should display.
        /// </returns>        
        [NotImplemented]
        public CalendarMode DisplayMode
        {
            get
            {
                return (CalendarMode)GetValue(DisplayModeProperty);
            }
            set
            {
                SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DisplayMode" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="DisplayMode" /> dependency property.
        /// </returns>
        [NotImplemented]
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
                nameof(DisplayMode),
                typeof(CalendarMode),
                typeof(Calendar),
                new PropertyMetadata(CalendarMode.Month));

        /// <summary>
        /// Occurs when the <see cref="DisplayMode" /> property is changed. 
        /// </summary>       
        [NotImplemented]
        public event EventHandler<CalendarModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Gets or sets a value that indicates what kind of selections are allowed.
        /// </summary>
        /// <returns>
        /// A value that indicates the current selection mode. The default is <see cref="CalendarSelectionMode.SingleDate" />.
        /// </returns>
        [NotImplemented]
        public CalendarSelectionMode SelectionMode
        {
            get
            {
                return (CalendarSelectionMode)GetValue(SelectionModeProperty);
            }
            set
            {
                SetValue(SelectionModeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectionMode" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="SelectionMode" /> dependency property.
        /// </returns>
        [NotImplemented]
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                nameof(SelectionMode),
                typeof(CalendarSelectionMode),
                typeof(Calendar),
                new PropertyMetadata(CalendarSelectionMode.SingleDate));

        /// <summary>
        /// Gets a collection of selected dates.
        /// </summary>
        /// <returns>
        /// A <see cref="SelectedDatesCollection"/> object that contains the currently
        /// selected dates. The default is an empty collection.
        /// </returns>
        [NotImplemented]
        public SelectedDatesCollection SelectedDates { get; private set; }
    }
}
