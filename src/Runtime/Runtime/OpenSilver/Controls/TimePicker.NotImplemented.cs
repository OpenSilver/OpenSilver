

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
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Controls
{
    /// <summary>
    /// Represents a control that allows the user to select a time.
    /// </summary>
    public partial class TimePicker
    {
#if MIGRATION
        /// <summary>
        /// Occurs when Value property has changed.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged;
#endif

#region public DateTime? Minimum
        /// <summary>
        /// Gets or sets the minimum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the minimum property is applicable for the following
        /// features: Selecting a value through a popup, Parsing a new value from
        /// the textbox, spinning a new value and programmatically specifying a value.</remarks>
        [OpenSilver.NotImplemented]
        public DateTime? Minimum
        {
            get => (DateTime?)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(DateTime?),
                typeof(TimePicker),
                new PropertyMetadata(null, OnMinimumPropertyChanged));

        /// <summary>
        /// MinimumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Minimum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
#endregion public DateTime? Minimum

#region public DateTime? Maximum
        /// <summary>
        /// Gets or sets the maximum time considered valid by the control.
        /// </summary>
        /// <remarks>Setting the Maximum property is applicable for the following
        /// features: Selecting a value through a popup, Parsing a new value
        /// from the textbox, spinning a new value and programmatically specifying
        /// a value. </remarks>
        [OpenSilver.NotImplemented]
        public DateTime? Maximum
        {
            get => (DateTime?)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(DateTime?),
                typeof(TimePicker),
                new PropertyMetadata(null, OnMaximumPropertyChanged));

        /// <summary>
        /// MaximumProperty property changed handler.
        /// </summary>
        /// <param name="d">TimeUpDown that changed its Maximum.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        [OpenSilver.NotImplemented]
        protected virtual void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
        }
#endregion public DateTime? Maximum

#region public ITimeFormat Format
        /// <summary>
        /// Gets or sets the Format used by the control. From XAML Use either
        /// "Short", "Long" or a custom format.
        /// Custom formats can only contain "H", "h", "m", "s" or "t".
        /// For example: use 'hh:mm:ss' is used to format time as "13:45:30".
        /// </summary>
        [OpenSilver.NotImplemented]
        public ITimeFormat Format
        {
            get => GetValue(FormatProperty) as ITimeFormat;
            set => SetValue(FormatProperty, value);
        }

        /// <summary>
        /// Identifies the Format dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(
                "Format",
                typeof(ITimeFormat),
                typeof(TimePicker),
                new PropertyMetadata(OnFormatPropertyChanged));

        /// <summary>
        /// FormatProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its Format.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// BackingField for ActualFormat.
        /// </summary>
        private ITimeFormat _actualFormat;
#endregion public ITimeFormat Format

#region public TimePickerPopup Popup
        /// <summary>
        /// Gets or sets the TimePickerPopup that will be shown to the user by the
        /// TimePicker control. This property may not be styled. To style a
        /// TimePicker with a Popup, please use the PopupTemplate property.
        /// When both PopupTemplate and Popup are set, Popup will be used.
        /// </summary>
        /// <remark>
        /// This property might be null, since a template can be used.
        /// </remark>
        [OpenSilver.NotImplemented]
        public TimePickerPopup Popup
        {
            get => GetValue(PopupProperty) as TimePickerPopup;
            set => SetValue(PopupProperty, value);
        }

        /// <summary>
        /// Identifies the Popup dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register(
                "Popup",
                typeof(TimePickerPopup),
                typeof(TimePicker),
                new PropertyMetadata(OnPopupPropertyChanged));

        /// <summary>
        /// PopupProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TimePicker that changed its Popup.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private static void OnPopupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public TimePickerPopup Popup

#region public TimePickerPopupTemplate PopupTemplate
        /// <summary>
        /// Gets or sets the template used as Popup. A Popup can also be set
        /// directly on the Popup property. When both PopupTemplate and Popup
        /// are set, Popup will be used. 
        /// </summary>
        [OpenSilver.NotImplemented]
        public TimePickerPopupTemplate PopupTemplate
        {
            get { return GetValue(PopupTemplateProperty) as TimePickerPopupTemplate; }
            set { SetValue(PopupTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the PopupTemplate dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupTemplateProperty =
            DependencyProperty.Register(
                "PopupTemplate",
                typeof(TimePickerPopupTemplate),
                typeof(TimePicker),
                new PropertyMetadata(null, OnPopupTemplatePropertyChanged));

        /// <summary>
        /// PopupTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupTemplate.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// BackingField for InstantiatedPopupFromTemplate.
        /// </summary>
        private TimePickerPopup _instantiatedPopupFromTemplate;
#endregion public TimePickerPopupTemplate PopupTemplate

#region public PopupTimeSelectionMode PopupTimeSelectionMode
        /// <summary>
        /// Gets or sets the whether the TimePickerPopup supports selecting
        /// designators and/or seconds.
        /// </summary>
        [OpenSilver.NotImplemented]
        public PopupTimeSelectionMode PopupTimeSelectionMode
        {
            get => (PopupTimeSelectionMode)GetValue(PopupTimeSelectionModeProperty);
            set => SetValue(PopupTimeSelectionModeProperty, value);
        }

        /// <summary>
        /// Identifies the PopupTimeSelectionMode dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupTimeSelectionModeProperty =
            DependencyProperty.Register(
                "PopupTimeSelectionMode",
                typeof(PopupTimeSelectionMode),
                typeof(TimePicker),
                new PropertyMetadata(PopupTimeSelectionMode.HoursAndMinutesOnly, OnPopupTimeSelectionModePropertyChanged));

        /// <summary>
        /// PopupTimeSelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">TimePicker that changed its PopupTimeSelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupTimeSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
#endregion public PopupTimeSelectionMode PopupTimeSelectionMode

#region public ClickMode PopupButtonMode
        /// <summary>
        /// Gets or sets the button event that causes the popup portion of the
        /// Picker control to open.
        /// </summary>
        [OpenSilver.NotImplemented]
        public ClickMode PopupButtonMode
        {
            get => (ClickMode)GetValue(PopupButtonModeProperty);
            set => SetValue(PopupButtonModeProperty, value);
        }

        /// <summary>
        /// Identifies the PopupButtonMode dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupButtonModeProperty =
            DependencyProperty.Register(
                "PopupButtonMode",
                typeof(ClickMode),
                typeof(TimePicker),
                new PropertyMetadata(ClickMode.Release, OnPopupButtonModePropertyChanged));

        /// <summary>
        /// PopupButtonModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Picker that changed its PopupButtonMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupButtonModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public ClickMode PopupButtonMode
    }
}
