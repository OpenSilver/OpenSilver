

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

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
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
        [OpenSilver.NotImplemented]
        public CalendarBlackoutDatesCollection BlackoutDates { get; private set; }

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Calendar.DisplayDate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.Calendar.DisplayDate" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(Calendar), new PropertyMetadata(DateTime.Today, OnDisplayDateChanged));

        private static void OnDisplayDateChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>Occurs when the <see cref="DisplayDate" /> property is changed.</summary>     
        [OpenSilver.NotImplemented]
        public event EventHandler<CalendarDateChangedEventArgs> DisplayDateChanged;

        [OpenSilver.NotImplemented]
        public Style CalendarDayButtonStyle { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.IsTabStop dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.IsTabStop dependency property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.Register(nameof(IsTabStop), typeof(bool), typeof(Calendar), new PropertyMetadata(true));

        //
        // Summary:
        //     Gets or sets a value that indicates whether a control is included in tab navigation.
        //
        // Returns:
        //     true if the control is included in tab navigation; otherwise, false. The default
        //     is true.
        [OpenSilver.NotImplemented]
        public bool IsTabStop
        {
            get { return (bool)GetValue(IsTabStopProperty); }
            set { SetValue(IsTabStopProperty, value); }
        }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.Background dependency property
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.Background dependency
        //     property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(Calendar), new PropertyMetadata((object)null));

        //
        // Summary:
        //     Gets or sets a brush that provides the background of the control.
        //
        // Returns:
        //     The brush that provides the background of the control. The default is null.
        [OpenSilver.NotImplemented]
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.Padding dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.Padding dependency property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(Calendar), new PropertyMetadata(new Thickness()));

        //
        // Summary:
        //     Gets or sets the padding inside a control.
        //
        // Returns:
        //     The amount of space between the content of a System.Windows.Controls.Control
        //     and its System.Windows.FrameworkElement.Margin or System.Windows.Controls.Border.
        //     The default is a thickness of 0 on all four sides.
        [OpenSilver.NotImplemented]
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.BorderThickness dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.BorderThickness dependency
        //     property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(Calendar), new PropertyMetadata(new Thickness()));

        //
        // Summary:
        //     Gets or sets the border thickness of a control.
        //
        // Returns:
        //     A thickness value; the default is a thickness of 0 on all four sides.
        [OpenSilver.NotImplemented]
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.BorderBrush dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.BorderBrush dependency
        //     property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(Calendar), new PropertyMetadata((object)null));

        //
        // Summary:
        //     Gets or sets a brush that describes the border background of a control.
        //
        // Returns:
        //     The brush that is used to fill the control's border; the default is null.
        [OpenSilver.NotImplemented]
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.Template dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.Template dependency property.
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register(nameof(Template), typeof(ControlTemplate), typeof(Calendar), new PropertyMetadata((object)null));

        //
        // Summary:
        //     Gets or sets a control template.
        //
        // Returns:
        //     The template that defines the appearance of the System.Windows.Controls.Control.
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Calendar), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// </summary>
        /// <remarks>
        /// The font used to display text in the control. The default is the "Portable User Interface".
        /// </remarks>
        [OpenSilver.NotImplemented]
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Calendar), new PropertyMetadata(new FontFamily("Portable User Interface")));

        [OpenSilver.NotImplemented]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Control.FontSize"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(Calendar), new PropertyMetadata(11d));

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is displayed in months, years, or decades.
        /// </summary>
        /// <returns>
        /// A value indicating what length of time the <see cref="Calendar" /> should display.
        /// </returns>        
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
                nameof(DisplayMode),
                typeof(CalendarMode),
                typeof(Calendar),
                new PropertyMetadata(CalendarMode.Month));

        /// <summary>
        /// Occurs when the <see cref="DisplayMode" /> property is changed. 
        /// </summary>       
        [OpenSilver.NotImplemented]
        public event EventHandler<CalendarModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Gets or sets a value that indicates what kind of selections are allowed.
        /// </summary>
        /// <returns>
        /// A value that indicates the current selection mode. The default is <see cref="CalendarSelectionMode.SingleDate" />.
        /// </returns>
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
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
        [OpenSilver.NotImplemented]
        public SelectedDatesCollection SelectedDates { get; private set; }
    }
}
