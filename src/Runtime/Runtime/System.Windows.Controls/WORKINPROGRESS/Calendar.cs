

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
        /// Identifies the <see cref="P:System.Windows.Controls.Calendar.SelectedDate" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.Calendar.SelectedDate" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(Calendar), new PropertyMetadata(OnSelectedDateChanged));

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// <returns>
        /// The date currently selected. The default is null.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The given date is outside the range specified by <see cref="P:System.Windows.Controls.Calendar.DisplayDateStart" /> and <see cref="P:System.Windows.Controls.Calendar.DisplayDateEnd" />
        /// -or-The given date is in the <see cref="P:System.Windows.Controls.Calendar.BlackoutDates" /> collection.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// If set to anything other than null when <see cref="P:System.Windows.Controls.Calendar.SelectionMode" /> is set to <see cref="F:System.Windows.Controls.CalendarSelectionMode.None" />.
        /// </exception>
        [OpenSilver.NotImplemented]
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        private static void OnSelectedDateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

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

        [OpenSilver.NotImplemented]
        public Style CalendarDayButtonStyle { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.IsTabStop dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.IsTabStop dependency property.
        public static readonly DependencyProperty IsTabStopProperty = DependencyProperty.Register(nameof(IsTabStop), typeof(bool), typeof(Calendar), new PropertyMetadata());

        //
        // Summary:
        //     Gets or sets a value that indicates whether a control is included in tab navigation.
        //
        // Returns:
        //     true if the control is included in tab navigation; otherwise, false. The default
        //     is true.
        public bool IsTabStop { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.Background dependency property
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.Background dependency
        //     property.
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(Calendar), new PropertyMetadata());

        //
        // Summary:
        //     Gets or sets a brush that provides the background of the control.
        //
        // Returns:
        //     The brush that provides the background of the control. The default is null.
        public Brush Background { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.Padding dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.Padding dependency property.
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(Calendar), new PropertyMetadata());

        //
        // Summary:
        //     Gets or sets the padding inside a control.
        //
        // Returns:
        //     The amount of space between the content of a System.Windows.Controls.Control
        //     and its System.Windows.FrameworkElement.Margin or System.Windows.Controls.Border.
        //     The default is a thickness of 0 on all four sides.
        public Thickness Padding { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.BorderThickness dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.BorderThickness dependency
        //     property.
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(nameof(BorderThickness), typeof(Thickness), typeof(Calendar), new PropertyMetadata());

        //
        // Summary:
        //     Gets or sets the border thickness of a control.
        //
        // Returns:
        //     A thickness value; the default is a thickness of 0 on all four sides.
        public Thickness BorderThickness { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.BorderBrush dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.BorderBrush dependency
        //     property.
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(Calendar), new PropertyMetadata());

        //
        // Summary:
        //     Gets or sets a brush that describes the border background of a control.
        //
        // Returns:
        //     The brush that is used to fill the control's border; the default is null.
        public Brush BorderBrush { get; set; }

        //
        // Summary:
        //     Identifies the System.Windows.Controls.Control.Template dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.Control.Template dependency property.
        public static readonly DependencyProperty TemplateProperty = DependencyProperty.Register(nameof(Template), typeof(ControlTemplate), typeof(Calendar), new PropertyMetadata());

        //
        // Summary:
        //     Gets or sets a control template.
        //
        // Returns:
        //     The template that defines the appearance of the System.Windows.Controls.Control.
        public ControlTemplate Template { get; set; }
    }
}
