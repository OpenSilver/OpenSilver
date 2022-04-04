

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a time picker popup that allows choosing time from a ListBox.
    /// </summary>
    /// <remarks>Can also be used independently.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ListBoxStyle", StyleTargetType = typeof(ListBox))]
    [StyleTypedProperty(Property = "ListBoxItemStyle", StyleTargetType = typeof(ListBoxItem))]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowSecondsAndDesignatorsSelection")]
    [TemplateVisualState(GroupName = "ContainedByPickerStates", Name = "Contained")]
    [TemplateVisualState(GroupName = "ContainedByPickerStates", Name = "NotContained")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowSecondsSelection")]
    [TemplatePart(Name = "ListBox", Type = typeof(ListBox))]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "AllowTimeDesignatorsSelection")]
    [TemplateVisualState(GroupName = "PopupModeStates", Name = "HoursAndMinutesOnly")]
    public class ListTimePickerPopup : TimePickerPopup
    {
        /// <summary>Identifies the ListBoxStyle dependency property.</summary>
        public static readonly DependencyProperty ListBoxStyleProperty = DependencyProperty.Register(nameof(ListBoxStyle), typeof(Style), typeof(ListTimePickerPopup), new PropertyMetadata(new PropertyChangedCallback(ListTimePickerPopup.OnListBoxStylePropertyChanged)));
        /// <summary>Identifies the ListBoxItemStyle dependency property.</summary>
        public static readonly DependencyProperty ListBoxItemStyleProperty = DependencyProperty.Register(nameof(ListBoxItemStyle), typeof(Style), typeof(ListTimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(ListTimePickerPopup.OnListBoxItemStylePropertyChanged)));
        /// <summary>
        /// Identifies the TimeItemsSelection dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeItemsSelectionProperty = DependencyProperty.Register(nameof(TimeItemsSelection), typeof(ItemSelectionHelper<KeyValuePair<string, DateTime?>>), typeof(ListTimePickerPopup), new PropertyMetadata((object)null, new PropertyChangedCallback(ListTimePickerPopup.OnTimeItemsSelectionPropertyChanged)));
        /// <summary>The name of the ListBox TemplatePart.</summary>
        internal const string ListBoxPartName = "ListBox";
        /// <summary>BackingField for ListBoxPart.</summary>
        private ListBox _listBoxPart;
        /// <summary>
        /// Determines whether the value changed because SelectedItem in the
        /// ListBox was changed.
        /// </summary>
        private bool _isValueChangeCausedBySelection;
        /// <summary>
        /// Indicates whether it is allowed to set the TimeItemsSelection property.
        /// </summary>
        private bool _allowWritingTimeItemsSelection;

        /// <summary>Gets the ListBox part.</summary>
        internal ListBox ListBoxPart
        {
            get
            {
                return this._listBoxPart;
            }
            private set
            {
                if (this._listBoxPart != null)
                {
                    this._listBoxPart.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ItemSelectedByMouse);
                }
                this._listBoxPart = value;
                if (this._listBoxPart == null)
                    return;
                this._listBoxPart.MouseLeftButtonUp += new MouseButtonEventHandler(this.ItemSelectedByMouse);
            }
        }

        /// <summary>
        /// Gets or sets the Style applied to the ListBox portion the
        /// ListTimePickerPopup control.
        /// </summary>
        public Style ListBoxStyle
        {
            get
            {
                return this.GetValue(ListTimePickerPopup.ListBoxStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ListTimePickerPopup.ListBoxStyleProperty, (object)value);
            }
        }

        /// <summary>ListBoxStyleProperty property changed handler.</summary>
        /// <param name="d">ListTimePickerPopup that changed its ListBoxStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnListBoxStylePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets or sets the Style applied to the ListBoxItems in the
        /// ListTimePickerPopup control.
        /// </summary>
        public Style ListBoxItemStyle
        {
            get
            {
                return this.GetValue(ListTimePickerPopup.ListBoxItemStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ListTimePickerPopup.ListBoxItemStyleProperty, (object)value);
            }
        }

        /// <summary>ListBoxItemStyleProperty property changed handler.</summary>
        /// <param name="d">ListTimePickerPopup that changed its ListBoxItemStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnListBoxItemStylePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the collection of times used in the ListBox portion of the
        /// ListTimePickerPopup control.
        /// </summary>
        /// <value>The time items selection.</value>
        public ItemSelectionHelper<KeyValuePair<string, DateTime?>> TimeItemsSelection
        {
            get
            {
                return (ItemSelectionHelper<KeyValuePair<string, DateTime?>>)this.GetValue(ListTimePickerPopup.TimeItemsSelectionProperty);
            }
            private set
            {
                this._allowWritingTimeItemsSelection = true;
                this.SetValue(ListTimePickerPopup.TimeItemsSelectionProperty, (object)value);
                this._allowWritingTimeItemsSelection = false;
            }
        }

        /// <summary>TimeItemsSelectionProperty property changed handler.</summary>
        /// <param name="d">ListTimePickerPopup that changed its TimeItemsSelection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeItemsSelectionPropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            ListTimePickerPopup listTimePickerPopup = (ListTimePickerPopup)d;
            ItemSelectionHelper<KeyValuePair<string, DateTime?>> oldValue = e.OldValue as ItemSelectionHelper<KeyValuePair<string, DateTime?>>;
            ItemSelectionHelper<KeyValuePair<string, DateTime?>> newValue = e.NewValue as ItemSelectionHelper<KeyValuePair<string, DateTime?>>;
            if (!listTimePickerPopup._allowWritingTimeItemsSelection)
            {
                listTimePickerPopup.TimeItemsSelection = oldValue;
                throw new ArgumentException("Items Read Only");
            }
            listTimePickerPopup.TimeItemsSelectionPropertyChanged((INotifyPropertyChanged)oldValue, (INotifyPropertyChanged)newValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.ListTimePickerPopup" /> class.
        /// </summary>
        public ListTimePickerPopup()
        {            
            this.TimeItemsSelection = new ItemSelectionHelper<KeyValuePair<string, DateTime?>>()
            {
                Items = new ObservableCollection<KeyValuePair<string, DateTime?>>()
            };
            this.DefaultStyleKey = (object)typeof(ListTimePickerPopup);
        }

        /// <summary>
        /// Builds the visual tree for the ListTimePickerPopup control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ListBoxPart = this.GetTemplateChild("ListBox") as ListBox;
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            base.OnValueChanged(e);
            DateTime? nullable;
            int num;
            if (e.OldValue.HasValue)
            {
                nullable = e.NewValue;
                if (nullable.HasValue)
                {
                    nullable = e.OldValue;
                    DateTime date1 = nullable.Value.Date;
                    nullable = e.NewValue;
                    DateTime date2 = nullable.Value.Date;
                    if (date1 != date2)
                    {
                        num = 0;
                        goto label_8;
                    }
                }
            }
            nullable = e.OldValue;
            if (!nullable.HasValue)
            {
                nullable = e.NewValue;
                num = !nullable.HasValue ? 1 : 0;
            }
            else
                num = 1;
            label_8:
            if (num == 0)
                this.RegenerateTimeItems();
            if (this.TimePickerParent != null || this._isValueChangeCausedBySelection)
                return;
            this.SelectValue();
            this.ScrollToSelectedValue();
        }

        /// <summary>Called when the Minimum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected override void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
            base.OnMinimumChanged(oldValue, newValue);
            this.RegenerateTimeItems();
        }

        /// <summary>Called when the Maximum property value has changed.</summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected override void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
            base.OnMaximumChanged(oldValue, newValue);
            this.RegenerateTimeItems();
        }

        /// <summary>Called when the culture changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            base.OnCultureChanged(oldValue, newValue);
            this.RegenerateTimeItems();
        }

        /// <summary>Called when format changed.</summary>
        /// <param name="oldValue">The old format.</param>
        /// <param name="newValue">The new format.</param>
        protected override void OnFormatChanged(ITimeFormat oldValue, ITimeFormat newValue)
        {
            base.OnFormatChanged(oldValue, newValue);
            this.RegenerateTimeItems();
        }

        /// <summary>Called when the popup minutes interval changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPopupMinutesIntervalChanged(int oldValue, int newValue)
        {
            base.OnPopupMinutesIntervalChanged(oldValue, newValue);
            this.RegenerateTimeItems();
        }

        /// <summary>Called when the popup seconds interval changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPopupSecondsIntervalChanged(int oldValue, int newValue)
        {
            base.OnPopupSecondsIntervalChanged(oldValue, newValue);
            this.RegenerateTimeItems();
        }

        /// <summary>Called when the time selection mode is changed.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPopupTimeSelectionModeChanged(
          PopupTimeSelectionMode oldValue,
          PopupTimeSelectionMode newValue)
        {
            base.OnPopupTimeSelectionModeChanged(oldValue, newValue);
            if (newValue != PopupTimeSelectionMode.HoursAndMinutesOnly)
            {
                this.SetValue(TimePickerPopup.PopupTimeSelectionModeProperty, (object)oldValue);
                throw new ArgumentOutOfRangeException(nameof(newValue), string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Invalid {0}", (object)newValue));
            }
        }

        /// <summary>Regenerates the time items.</summary>
        private void RegenerateTimeItems()
        {
            this.TimeItemsSelection.Items.Clear();
            if (this.PopupMinutesInterval == 0)
                return;
            DateTime dateTime1;
            DateTime dateTime2;
            if (!this.Value.HasValue)
            {
                dateTime2 = new DateTime(1900, 1, 1);
            }
            else
            {
                dateTime1 = this.Value.Value;
                dateTime2 = dateTime1.Date;
            }
            DateTime dateTime3 = dateTime2;
            DateTime? nullable = this.Minimum;
            DateTime dateTime4;
            if (!nullable.HasValue)
            {
                dateTime4 = dateTime3;
            }
            else
            {
                ref DateTime local = ref dateTime3;
                nullable = this.Minimum;
                dateTime1 = nullable.Value;
                TimeSpan timeOfDay = dateTime1.TimeOfDay;
                dateTime4 = local.Add(timeOfDay);
            }
            DateTime dateTime5 = dateTime4;
            nullable = this.Maximum;
            DateTime dateTime6;
            if (!nullable.HasValue)
            {
                dateTime1 = dateTime3.AddDays(1.0);
                dateTime6 = dateTime1.Subtract(TimeSpan.FromMilliseconds(1.0));
            }
            else
            {
                ref DateTime local = ref dateTime3;
                nullable = this.Maximum;
                dateTime1 = nullable.Value;
                TimeSpan timeOfDay = dateTime1.TimeOfDay;
                dateTime6 = local.Add(timeOfDay);
            }
            DateTime dateTime7 = dateTime6;
            TimeSpan timeSpan = new TimeSpan(0, 0, this.PopupMinutesInterval, 0);
            while (dateTime3 <= dateTime7)
            {
                if (dateTime3 >= dateTime5)
                    this.TimeItemsSelection.Items.Add(new KeyValuePair<string, DateTime?>(this.ActualTimeGlobalizationInfo.FormatTime(new DateTime?(dateTime3), this.ActualFormat), new DateTime?(dateTime3)));
                dateTime3 += timeSpan;
            }
        }

        /// <summary>
        /// Select a value based on the current value. This will 'snap' the
        /// Value to the closest possible Time based on the interval.
        /// </summary>
        private void SelectValue()
        {
            if (!this.Value.HasValue)
                return;
            this.TimeItemsSelection.SelectedItem = this.TimeItemsSelection.Items.Where<KeyValuePair<string, DateTime?>>((Func<KeyValuePair<string, DateTime?>, bool>)(item =>
            {
                DateTime dateTime = item.Value.Value;
                TimeSpan timeOfDay1 = dateTime.TimeOfDay;
                dateTime = this.Value.Value;
                TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                return timeOfDay1 <= timeOfDay2;
            })).OrderByDescending<KeyValuePair<string, DateTime?>, TimeSpan>((Func<KeyValuePair<string, DateTime?>, TimeSpan>)(item => item.Value.Value.TimeOfDay)).FirstOrDefault<KeyValuePair<string, DateTime?>>();
        }

        /// <summary>Scrolls to a value in the list, or closest.</summary>
        private void ScrollToSelectedValue()
        {
            if (this.ListBoxPart == null || !this.ListBoxPart.Items.Contains((object)this.TimeItemsSelection.SelectedItem))
                return;
            this.ListBoxPart.UpdateLayout();
            this.ListBoxPart.ScrollIntoView(this.ListBoxPart.Items[this.ListBoxPart.Items.Count - 1]);
            this.ListBoxPart.UpdateLayout();
            this.ListBoxPart.ScrollIntoView((object)this.TimeItemsSelection.SelectedItem);
        }

        /// <summary>Called when the TimeItems object is set.</summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void TimeItemsSelectionPropertyChanged(
          INotifyPropertyChanged oldValue,
          INotifyPropertyChanged newValue)
        {
            if (oldValue != null)
                oldValue.PropertyChanged -= new PropertyChangedEventHandler(this.TimeItemsPropertyChanged);
            if (newValue == null)
                return;
            newValue.PropertyChanged += new PropertyChangedEventHandler(this.TimeItemsPropertyChanged);
        }

        /// <summary>
        /// Called by any property change on the TimeItems object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void TimeItemsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == "SelectedItem") || !this.TimeItemsSelection.Items.Contains(this.TimeItemsSelection.SelectedItem))
                return;
            this._isValueChangeCausedBySelection = true;
            try
            {
                this.Value = this.TimeItemsSelection.SelectedItem.Value;
            }
            finally
            {
                this._isValueChangeCausedBySelection = false;
            }
        }

        /// <summary>Called when TimePicker opened the popup.</summary>
        /// <remarks>Called before the TimePicker reacts to value changes.
        /// This is done so that the Popup can 'snap' to a specific value without
        /// changing the selected value in the TimePicker.</remarks>
        public override void OnOpened()
        {
            base.OnOpened();
            this.UpdateLayout();
            this.RegenerateTimeItems();
            this.SelectValue();
            this.ScrollToSelectedValue();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the ListBoxPart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private void ItemSelectedByMouse(object sender, MouseButtonEventArgs e)
        {
            if (this.TimePickerParent != null)
            {
                DateTime? nullable = this.TimePickerParent.Value;
                int num;
                if (nullable.HasValue)
                {
                    nullable = this.TimePickerParent.Value;
                    if (nullable.HasValue)
                    {
                        nullable = this.TimePickerParent.Value;
                        DateTime dateTime = nullable.Value;
                        TimeSpan timeOfDay1 = dateTime.TimeOfDay;
                        nullable = this.Value;
                        dateTime = nullable.Value;
                        TimeSpan timeOfDay2 = dateTime.TimeOfDay;
                        num = !(timeOfDay1 != timeOfDay2) ? 1 : 0;
                    }
                    else
                        num = 1;
                }
                else
                    num = 0;
                if (num == 0)
                    this.TimePickerParent.Value = this.Value;
            }
            this.DoCommit();
        }

        /// <summary>Provides handling for the KeyDown event.</summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Enter:
                    this.DoCommit();
                    break;
                case Key.Escape:
                    this.DoCancel();
                    break;
            }
        }

        /// <summary>Gets the valid popup time selection modes.</summary>
        /// <returns>
        /// An array of PopupTimeSelectionModes that are supported by
        /// the Popup.
        /// </returns>
        internal override PopupTimeSelectionMode[] GetValidPopupTimeSelectionModes()
        {
            return new PopupTimeSelectionMode[1]
            {
        PopupTimeSelectionMode.HoursAndMinutesOnly
            };
        }
    }
}
