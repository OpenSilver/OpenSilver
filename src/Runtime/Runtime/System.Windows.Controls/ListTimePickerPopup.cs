// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

#if MIGRATION
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Automation;
#else
using Windows.System;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Automation;
using MouseButtonEventArgs = Windows.UI.Xaml.Input.PointerRoutedEventArgs;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#endif

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
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = ContainedStateName, GroupName = ContainedByPickerGroupName)]
    [TemplateVisualState(Name = NotContainedStateName, GroupName = ContainedByPickerGroupName)]

    [TemplateVisualState(Name = AllowSecondsAndDesignatorsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = AllowTimeDesignatorsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = AllowSecondsSelectionStateName, GroupName = PopupModeGroupName)]
    [TemplateVisualState(Name = HoursAndMinutesOnlyStateName, GroupName = PopupModeGroupName)]

    [TemplatePart(Name = ListBoxPartName, Type = typeof(ListBox))]

    [StyleTypedProperty(Property = "ListBoxStyle", StyleTargetType = typeof(ListBox))]
    [StyleTypedProperty(Property = "ListBoxItemStyle", StyleTargetType = typeof(ListBoxItem))]
    public class ListTimePickerPopup : TimePickerPopup
    {
#region Template Part Names
        /// <summary>
        /// The name of the ListBox TemplatePart.
        /// </summary>
        internal const string ListBoxPartName = "ListBox";
#endregion Template Part Names

#region TemplateParts

        /// <summary>
        /// Gets the ListBox part.
        /// </summary>
        internal ListBox ListBoxPart
        {
            get { return _listBoxPart; }
            private set
            {
                if (_listBoxPart != null)
                {
#if MIGRATION
                    _listBoxPart.MouseLeftButtonUp -= ItemSelectedByMouse;
#else
                    _listBoxPart.PointerReleased -= ItemSelectedByMouse;
#endif
                    _listBoxPart.SelectionChanged -= RaiseAutomationPeerSelectionChanged;
                }

                _listBoxPart = value;

                if (_listBoxPart != null)
                {
#if MIGRATION
                    _listBoxPart.MouseLeftButtonUp += ItemSelectedByMouse;
#else
                    _listBoxPart.PointerReleased += ItemSelectedByMouse;
#endif
                    _listBoxPart.SelectionChanged += RaiseAutomationPeerSelectionChanged;
                }
            }
        }

        /// <summary>
        /// BackingField for ListBoxPart.
        /// </summary>
        private ListBox _listBoxPart;
#endregion TemplateParts

        /// <summary>
        /// Determines whether the value changed because SelectedItem in the
        /// ListBox was changed.
        /// </summary>
        private bool _isValueChangeCausedBySelection;

#region public Style ListBoxStyle
        /// <summary>
        /// Gets or sets the Style applied to the ListBox portion the 
        /// ListTimePickerPopup control.
        /// </summary>
        public Style ListBoxStyle
        {
            get { return GetValue(ListBoxStyleProperty) as Style; }
            set { SetValue(ListBoxStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ListBoxStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ListBoxStyleProperty =
            DependencyProperty.Register(
                "ListBoxStyle",
                typeof(Style),
                typeof(ListTimePickerPopup),
                new PropertyMetadata(OnListBoxStylePropertyChanged));

        /// <summary>
        /// ListBoxStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">ListTimePickerPopup that changed its ListBoxStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnListBoxStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public Style ListBoxStyle

#region public Style ListBoxItemStyle
        /// <summary>
        /// Gets or sets the Style applied to the ListBoxItems in the 
        /// ListTimePickerPopup control.
        /// </summary>
        public Style ListBoxItemStyle
        {
            get { return GetValue(ListBoxItemStyleProperty) as Style; }
            set { SetValue(ListBoxItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ListBoxItemStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ListBoxItemStyleProperty =
            DependencyProperty.Register(
                "ListBoxItemStyle",
                typeof(Style),
                typeof(ListTimePickerPopup),
                new PropertyMetadata(null, OnListBoxItemStylePropertyChanged));

        /// <summary>
        /// ListBoxItemStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">ListTimePickerPopup that changed its ListBoxItemStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnListBoxItemStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
#endregion public Style ListBoxItemStyle

#region public ItemSelectionHelper<KeyValuePair<string, DateTime?>> TimeItemsSelection
        /// <summary>
        /// Indicates whether it is allowed to set the TimeItemsSelection property.
        /// </summary>
        private bool _allowWritingTimeItemsSelection;

        /// <summary>
        /// Gets the collection of times used in the ListBox portion of the 
        /// ListTimePickerPopup control.
        /// </summary>
        /// <value>The time items selection.</value>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Value is a nullable DateTime.")]
        public ItemSelectionHelper<KeyValuePair<string, DateTime?>> TimeItemsSelection
        {
            get { return (ItemSelectionHelper<KeyValuePair<string, DateTime?>>)GetValue(TimeItemsSelectionProperty); }
            private set
            {
                _allowWritingTimeItemsSelection = true;
                SetValue(TimeItemsSelectionProperty, value);
                _allowWritingTimeItemsSelection = false;
            }
        }

        /// <summary>
        /// Identifies the TimeItemsSelection dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeItemsSelectionProperty =
            DependencyProperty.Register(
                "TimeItemsSelection",
                typeof(ItemSelectionHelper<KeyValuePair<string, DateTime?>>),
                typeof(ListTimePickerPopup),
                new PropertyMetadata(null, OnTimeItemsSelectionPropertyChanged));

        /// <summary>
        /// TimeItemsSelectionProperty property changed handler.
        /// </summary>
        /// <param name="d">ListTimePickerPopup that changed its TimeItemsSelection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTimeItemsSelectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListTimePickerPopup source = (ListTimePickerPopup)d;
            ItemSelectionHelper<KeyValuePair<string, DateTime?>> oldValue = e.OldValue as ItemSelectionHelper<KeyValuePair<string, DateTime?>>;
            ItemSelectionHelper<KeyValuePair<string, DateTime?>> newValue = e.NewValue as ItemSelectionHelper<KeyValuePair<string, DateTime?>>;

            if (!source._allowWritingTimeItemsSelection)
            {
                // set back original value
                source.TimeItemsSelection = oldValue;

                throw new ArgumentException("Cannot set read-only property TimeItemsSelection.");
            }

            source.TimeItemsSelectionPropertyChanged(oldValue, newValue);
        }
#endregion public ItemSelectionHelper<KeyValuePair<string, DateTime?>> TimeItemsSelection

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTimePickerPopup"/> class.
        /// </summary>
        public ListTimePickerPopup() : base()
        {
            TimeItemsSelection = new ItemSelectionHelper<KeyValuePair<string, DateTime?>>
            {
                Items = new ObservableCollection<KeyValuePair<string, DateTime?>>()
            };

            DefaultStyleKey = typeof(ListTimePickerPopup);
        }

        /// <summary>
        /// Builds the visual tree for the ListTimePickerPopup control when a new 
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            ListBoxPart = GetTemplateChild(ListBoxPartName) as ListBox;
        }

        /// <summary>
        /// Raises the ValueChanged event when Value property has changed.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            base.OnValueChanged(e);

            if ((e.OldValue.HasValue && e.NewValue.HasValue && e.OldValue.Value.Date != e.NewValue.Value.Date) ||
                (!e.OldValue.HasValue && e.NewValue.HasValue))
            {
                // generate a list based on the new datepart of this value.
                RegenerateTimeItems();
            }

            // only select and scroll when not contained within TimePicker.
            // TimePicker will call the OnOpened method where initialization can 
            // be performed.
            if (TimePickerParent == null && !_isValueChangeCausedBySelection)
            {
                SelectValue();
                ScrollToSelectedValue();
            }
        }

        /// <summary>
        /// Called when the Minimum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Minimum property.</param>
        /// <param name="newValue">New value of the Minimum property.</param>
        protected override void OnMinimumChanged(DateTime? oldValue, DateTime? newValue)
        {
            base.OnMinimumChanged(oldValue, newValue);

            RegenerateTimeItems();
        }

        /// <summary>
        /// Called when the Maximum property value has changed.
        /// </summary>
        /// <param name="oldValue">Old value of the Maximum property.</param>
        /// <param name="newValue">New value of the Maximum property.</param>
        protected override void OnMaximumChanged(DateTime? oldValue, DateTime? newValue)
        {
            base.OnMaximumChanged(oldValue, newValue);

            RegenerateTimeItems();
        }

        /// <summary>
        /// Called when the culture changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            base.OnCultureChanged(oldValue, newValue);

            RegenerateTimeItems();
        }

        /// <summary>
        /// Called when format changed.
        /// </summary>
        /// <param name="oldValue">The old format.</param>
        /// <param name="newValue">The new format.</param>
        protected override void OnFormatChanged(ITimeFormat oldValue, ITimeFormat newValue)
        {
            base.OnFormatChanged(oldValue, newValue);

            RegenerateTimeItems();
        }

        /// <summary>
        /// Called when the popup minutes interval changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPopupMinutesIntervalChanged(int oldValue, int newValue)
        {
            base.OnPopupMinutesIntervalChanged(oldValue, newValue);

            RegenerateTimeItems();
        }

        /// <summary>
        /// Called when the popup seconds interval changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPopupSecondsIntervalChanged(int oldValue, int newValue)
        {
            base.OnPopupSecondsIntervalChanged(oldValue, newValue);

            RegenerateTimeItems();
        }

        /// <summary>
        /// Called when the time selection mode is changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnPopupTimeSelectionModeChanged(PopupTimeSelectionMode oldValue, PopupTimeSelectionMode newValue)
        {
            base.OnPopupTimeSelectionModeChanged(oldValue, newValue);

            if (newValue != PopupTimeSelectionMode.HoursAndMinutesOnly)
            {
                // revert to old value
                SetValue(PopupTimeSelectionModeProperty, oldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Invalid PopupTimeSelectionMode for this popup, value '{0}'.",
                    newValue);

                throw new ArgumentOutOfRangeException("newValue", message);
            }
        }

        /// <summary>
        /// Regenerates the time items.
        /// </summary>
        private void RegenerateTimeItems()
        {
            TimeItemsSelection.Items.Clear();

            // validate
            if (PopupMinutesInterval == 0)
            {
                // do not allow a zero interval.
                return;
            }

            DateTime runningTime = Value.HasValue ? Value.Value.Date : new DateTime(1900, 1, 1);
            DateTime startTime = Minimum.HasValue
                                     ? runningTime.Add(Minimum.Value.TimeOfDay)
                                     : runningTime;

            DateTime endTime = Maximum.HasValue
                                   ? runningTime.Add(Maximum.Value.TimeOfDay)
                                   : runningTime.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1));

            // ListTimePickerPopup will disregard seconds.
            TimeSpan increment = new TimeSpan(0, 0, PopupMinutesInterval, 0);

            while (runningTime <= endTime)
            {
                if (runningTime >= startTime)
                {
                    TimeItemsSelection.Items.Add(new KeyValuePair<string, DateTime?>(ActualTimeGlobalizationInfo.FormatTime(runningTime, ActualFormat), runningTime));
                }
                runningTime += increment;
            }
        }

        /// <summary>
        /// Select a value based on the current value. This will 'snap' the 
        /// Value to the closest possible Time based on the interval.
        /// </summary>
        private void SelectValue()
        {
            if (Value.HasValue)
            {
                // select the item, or the closest to that item
                KeyValuePair<string, DateTime?> scrollToItem = (from item in TimeItemsSelection.Items
                                                                where
                                                                    item.Value.Value.TimeOfDay <=
                                                                    Value.Value.TimeOfDay
                                                                orderby item.Value.Value.TimeOfDay descending
                                                                select item).FirstOrDefault();
                // select this item.
                TimeItemsSelection.SelectedItem = scrollToItem;
            }
        }

        /// <summary>
        /// Scrolls to a value in the list, or closest.
        /// </summary>
        private void ScrollToSelectedValue()
        {
            if (ListBoxPart == null)
            {
                return;
            }

            if (ListBoxPart.Items.Contains(TimeItemsSelection.SelectedItem))
            {
                // scroll to last item
                ListBoxPart.UpdateLayout();
                ListBoxPart.ScrollIntoView(ListBoxPart.Items[ListBoxPart.Items.Count - 1]);

                // scroll to that item
                // because we've already scrolled to the last item, 
                // this scroll will put the item at the top of the ListBox
                ListBoxPart.UpdateLayout();
                ListBoxPart.ScrollIntoView(TimeItemsSelection.SelectedItem);
            }
        }

        /// <summary>
        /// Called when the TimeItems object is set.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void TimeItemsSelectionPropertyChanged(INotifyPropertyChanged oldValue, INotifyPropertyChanged newValue)
        {
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= TimeItemsPropertyChanged;
            }

            if (newValue != null)
            {
                newValue.PropertyChanged += TimeItemsPropertyChanged;
            }
        }

        /// <summary>
        /// Called by any property change on the TimeItems object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void TimeItemsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ItemSelectionHelper<KeyValuePair<string, DateTime?>>.SelectedItemName)
            {
                if (TimeItemsSelection.Items.Contains(TimeItemsSelection.SelectedItem))
                {
                    _isValueChangeCausedBySelection = true;
                    try
                    {
                        Value = TimeItemsSelection.SelectedItem.Value;
                    }
                    finally
                    {
                        _isValueChangeCausedBySelection = false;
                    }
                }
            }
        }

        /// <summary>
        /// Called when TimePicker opened the popup.
        /// </summary>
        /// <remarks>Called before the TimePicker reacts to value changes.
        /// This is done so that the Popup can 'snap' to a specific value without
        /// changing the selected value in the TimePicker.</remarks>
        public override void OnOpened()
        {
            base.OnOpened();

            UpdateLayout();
            RegenerateTimeItems();
            SelectValue();
            ScrollToSelectedValue();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the ListBoxPart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ItemSelectedByMouse(object sender, MouseButtonEventArgs e)
        {
            // there is a scenario where timepickerParent has never received the 
            // valuechanged event: when the picker has a value that is not
            // in the list and the user is selecting the first value in the list
            // which was pre-selected.
            // scenario: 4:15 was entered, popup opened and preselected
            // 4:00 (closest). User picks 4:00.
            if (TimePickerParent != null)
            {
                if (TimePickerParent.Value.HasValue == false ||
                    (TimePickerParent.Value.HasValue && TimePickerParent.Value.Value.TimeOfDay != Value.Value.TimeOfDay))
                {
                    TimePickerParent.Value = Value;
                }
            }

            DoCommit();
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.Key)
            {
#if MIGRATION
                case Key.Enter:
#else
                case VirtualKey.Enter:
#endif
                    {
                        DoCommit();
                        break;
                    }
#if MIGRATION
                case Key.Escape:
#else
                case VirtualKey.Escape:
#endif
                    {
                        DoCancel();
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the valid popup time selection modes.
        /// </summary>
        /// <returns>
        /// An array of PopupTimeSelectionModes that are supported by
        /// the Popup.
        /// </returns>
        internal override PopupTimeSelectionMode[] GetValidPopupTimeSelectionModes()
        {
            return new[] { PopupTimeSelectionMode.HoursAndMinutesOnly };
        }

        /// <summary>
        /// Creates the automation peer.
        /// </summary>
        /// <returns>The ListTimePickerPopupAutomationPeer for this instance.</returns>
        protected override TimePickerPopupAutomationPeer CreateAutomationPeer()
        {
            return new ListTimePickerPopupAutomationPeer(this);
        }

        /// <summary>
        /// Raises the automation peer selection changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void RaiseAutomationPeerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // the selection has changed, let automation know
            AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
            if (peer != null)
            {
                // we are single select
                object oldValue = e.RemovedItems.Count == 1 ? e.RemovedItems[0] : String.Empty;
                object newValue = e.AddedItems.Count == 1 ? e.AddedItems[0] : String.Empty;
                peer.RaisePropertyChangedEvent(SelectionPatternIdentifiers.SelectionProperty, oldValue, newValue);
            }
        }
    }
}
