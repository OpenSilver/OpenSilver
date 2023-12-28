// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#define SILVERLIGHT

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Resource = OpenSilver.Controls.Input.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// A control that has a rating.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateReadOnly, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RatingItem))]
    public class Rating : ItemsControl, IUpdateVisualState
    {
        #region protected double DisplayValue
        /// <summary>
        /// Gets or sets the actual value of the Rating control.
        /// </summary>
        protected double DisplayValue
        {
            get { return (double)GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayValue dependency property.
        /// </summary>
        protected static readonly DependencyProperty DisplayValueProperty =
            DependencyProperty.Register(
                "DisplayValue",
                typeof(double),
                typeof(Rating),
                new PropertyMetadata(0.0, OnDisplayValueChanged));

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary>
        /// <param name="dependencyObject">Rating that changed its DisplayValue.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private static void OnDisplayValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            Rating source = (Rating)dependencyObject;
            source.OnDisplayValueChanged();
        }

        /// <summary>
        /// DisplayValueProperty property changed handler.
        /// </summary>
        private void OnDisplayValueChanged()
        {
            UpdateDisplayValues();
        }

        #endregion protected double DisplayValue

        /// <summary>
        /// Gets or sets the rating item hovered over.
        /// </summary>
        private RatingItem HoveredRatingItem { get; set; }

        /// <summary>
        /// Gets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        internal InteractionHelper Interaction { get; private set; }

        /// <summary>
        /// Gets or sets the items control helper class.
        /// </summary>
        private ItemsControlHelper ItemsControlHelper { get; set; }

        #region public int ItemCount
        /// <summary>
        /// Gets or sets the number of rating items.
        /// </summary>
        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemCount dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register(
                "ItemCount",
                typeof(int),
                typeof(Rating),
                new PropertyMetadata(0, OnItemCountChanged));

        /// <summary>
        /// ItemCountProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its ItemCount.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = d as Rating;
            int value = (int)e.NewValue;
            source.OnItemCountChanged(value);
        }

        /// <summary>
        /// This method is invoked when the items count property is changed.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnItemCountChanged(int newValue)
        {
            if (newValue < 0)
            {
                throw new ArgumentException(Resource.Rating_SetItemCount_ItemCountMustBeLargerThanOrEqualToZero);
            }

            int amountToAdd = newValue - this.Items.Count;
            if (amountToAdd > 0)
            {
                for (int cnt = 0; cnt < amountToAdd; cnt++)
                {
                    this.Items.Add(new RatingItem());
                }
            }
            else if (amountToAdd < 0)
            {
                for (int cnt = 0; cnt < Math.Abs(amountToAdd); cnt++)
                {
                    this.Items.RemoveAt(this.Items.Count - 1);
                }
            }
        }
        #endregion public int ItemCount

        #region public bool IsReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether the Rating is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(Rating),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its IsReadOnly.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            source.OnIsReadOnlyChanged(oldValue, newValue);
        }

        /// <summary>
        /// IsReadOnlyProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            Interaction.OnIsReadOnlyChanged(newValue);
            foreach (RatingItem ratingItem in GetRatingItems())
            {
                ratingItem.IsReadOnly = newValue;
            }

            UpdateHoverStates();
        }
        #endregion public bool IsReadOnly

#if SILVERLIGHT
        #region public Style ItemContainerStyle
        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        public new Style ItemContainerStyle
        {
            get { return GetValue(ItemContainerStyleProperty) as Style; }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static new readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(
                "ItemContainerStyle",
                typeof(Style),
                typeof(Rating),
                new PropertyMetadata(null, OnItemContainerStyleChanged));

        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its ItemContainerStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            Style newValue = (Style)e.NewValue;
            source.OnItemContainerStyleChanged(newValue);
        }

        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnItemContainerStyleChanged(Style newValue)
        {
            ItemsControlHelper.UpdateItemContainerStyle(newValue);
        }
        #endregion public Style ItemContainerStyle
#endif

        #region public RatingSelectionMode SelectionMode
        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        public RatingSelectionMode SelectionMode
        {
            get { return (RatingSelectionMode) GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(RatingSelectionMode),
                typeof(Rating),
                new PropertyMetadata(RatingSelectionMode.Continuous, OnSelectionModeChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its SelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            RatingSelectionMode oldValue = (RatingSelectionMode)e.OldValue;
            RatingSelectionMode newValue = (RatingSelectionMode)e.NewValue;
            source.OnSelectionModeChanged(oldValue, newValue);
        }

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnSelectionModeChanged(RatingSelectionMode oldValue, RatingSelectionMode newValue)
        {
            UpdateDisplayValues();
        }
        #endregion public RatingSelectionMode SelectionMode

        #region public double? Value
        /// <summary>
        /// Gets or sets the rating value.
        /// </summary>
        [TypeConverter(typeof(NullableConverter<double>))]
        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double?),
                typeof(Rating),
                new PropertyMetadata(new double?(), OnValueChanged));

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="d">Rating that changed its Value.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rating source = (Rating)d;
            double? oldValue = (double?)e.OldValue;
            double? newValue = (double?)e.NewValue;
            source.OnValueChanged(oldValue, newValue);
        }

        /// <summary>
        /// ValueProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
            UpdateValues();
            RoutedPropertyChangedEventHandler<double?> handler = ValueChanged;
            if (handler != null)
            {
                handler(this, new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue));
            }
        }

        /// <summary>
        /// Updates the control when the items change.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            EventHandler layoutUpdated = null;
            layoutUpdated =
                delegate
                {
                    this.LayoutUpdated -= layoutUpdated;
                    UpdateValues();
                    UpdateDisplayValues();
                };
            this.LayoutUpdated += layoutUpdated;

            this.ItemCount = this.Items.Count;

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// This event is raised when the value of the rating is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double?> ValueChanged;

        #endregion public double? Value

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the static members of the ColumnDataPoint class.
        /// </summary>
        static Rating()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Rating), new FrameworkPropertyMetadata(typeof(Rating)));
        }

#endif    
        /// <summary>
        /// Initializes a new instance of the Rating control.
        /// </summary>
        public Rating()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(Rating);
#endif
            this.Interaction = new InteractionHelper(this);

            this.ItemsControlHelper = new ItemsControlHelper(this);
        }

        /// <summary>
        /// Applies control template to the items control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ItemsControlHelper.OnApplyTemplate();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// This method is invoked when the mouse enters the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (Interaction.AllowMouseEnter(e))
            {
                Interaction.UpdateVisualStateBase(true);
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// This method is invoked when the mouse leaves the rating item.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Interaction.AllowMouseLeave(e))
            {
                Interaction.UpdateVisualStateBase(true);
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Provides handling for the Rating's MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Interaction.AllowMouseLeftButtonDown(e))
            {
                Interaction.OnMouseLeftButtonDownBase();
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Provides handling for the Rating's MouseLeftButtonUp event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Interaction.AllowMouseLeftButtonUp(e))
            {
                Interaction.OnMouseLeftButtonUpBase();
            }
            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Updates the values of the rating items.
        /// </summary>
        private void UpdateValues()
        {
            IList<RatingItem> ratingItems = GetRatingItems().ToList();

            RatingItem oldSelectedItem = this.GetSelectedRatingItem();

            IEnumerable<Tuple<RatingItem, double>> itemAndWeights =
                EnumerableFunctions
                    .Zip(
                        ratingItems,
                        ratingItems
                            .Select(ratingItem => 1.0)
                            .GetWeightedValues(Value.GetValueOrDefault()),
                        (item, percent) => Tuple.Create(item, percent));

            foreach (Tuple<RatingItem, double> itemAndWeight in itemAndWeights)
            {
                itemAndWeight.Item1.Value = itemAndWeight.Item2;
            }

            RatingItem newSelectedItem = this.GetSelectedRatingItem();

            // Notify when the selection changes
            if (oldSelectedItem != newSelectedItem)
            {
                if (newSelectedItem != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected))
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(newSelectedItem);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                    }
                }
                if (oldSelectedItem != null && AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(oldSelectedItem);
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                    }
                }
            }

            if (HoveredRatingItem == null)
            {
                DisplayValue = Value.GetValueOrDefault();
            }
        }

        /// <summary>
        /// Updates the value and actual value of the rating items.
        /// </summary>
        private void UpdateDisplayValues()
        {
            IList<RatingItem> ratingItems = GetRatingItems().ToList();

            IEnumerable<Tuple<RatingItem, double>> itemAndWeights =
                EnumerableFunctions
                    .Zip(
                        ratingItems,
                        ratingItems
                            .Select(ratingItem => 1.0)
                            .GetWeightedValues(DisplayValue),
                        (item, percent) => Tuple.Create(item, percent));

            RatingItem selectedItem = null;
            Tuple<RatingItem, double> selectedItemAndWeight = itemAndWeights.LastOrDefault(i => i.Item2 > 0.0);
            if (selectedItemAndWeight != null)
            {
                selectedItem = selectedItemAndWeight.Item1;
            }
            else
            {
                selectedItem = GetSelectedRatingItem();
            }

            foreach (Tuple<RatingItem, double> itemAndWeight in itemAndWeights)
            {
                if (SelectionMode == RatingSelectionMode.Individual && itemAndWeight.Item1 != selectedItem)
                {
                    itemAndWeight.Item1.DisplayValue = 0.0;
                }
                else
                {
                    itemAndWeight.Item1.DisplayValue = itemAndWeight.Item2;
                }
            }
        }

        /// <summary>
        /// Updates the hover states of the rating items.
        /// </summary>
        private void UpdateHoverStates()
        {
            if (HoveredRatingItem != null && !IsReadOnly)
            {
                IList<RatingItem> ratingItems = GetRatingItems().ToList();
                int indexOfItem = ratingItems.IndexOf(HoveredRatingItem);

                double total = ratingItems.Count();
                double filled = indexOfItem + 1;

                this.DisplayValue = filled / total;

                for (int cnt = 0; cnt < ratingItems.Count; cnt++)
                {
                    RatingItem ratingItem = ratingItems[cnt];
                    if (cnt <= indexOfItem && this.SelectionMode == RatingSelectionMode.Continuous)
                    {
                        VisualStates.GoToState(ratingItem, true, VisualStates.StateMouseOver);
                    }
                    else
                    {
                        IUpdateVisualState updateVisualState = (IUpdateVisualState) ratingItem;
                        updateVisualState.UpdateVisualState(true);
                    }
                }
            }
            else
            {
                this.DisplayValue = this.Value.GetValueOrDefault();

                foreach (IUpdateVisualState updateVisualState in GetRatingItems().OfType<IUpdateVisualState>())
                {
                    updateVisualState.UpdateVisualState(true);
                }
            }
        }

        /// <summary>
        /// This method returns a container for the item.
        /// </summary>
        /// <returns>A container for the item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RatingItem();
        }

        /// <summary>
        /// Gets a value indicating whether the item is its own container.
        /// </summary>
        /// <param name="item">The item which may be a container.</param>
        /// <returns>A value indicating whether the item is its own container.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RatingItem;
        }

        /// <summary>
        /// This method prepares a container to host an item.
        /// </summary>
        /// <param name="element">The container.</param>
        /// <param name="item">The item hosted in the container.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            RatingItem ratingItem = (RatingItem)element;
            object defaultForegroundValue = ratingItem.ReadLocalValue(Control.ForegroundProperty);
            if (defaultForegroundValue == DependencyProperty.UnsetValue)
            {
                ratingItem.SetBinding(Control.ForegroundProperty, new Binding("Foreground") { Source = this });
            }

            ratingItem.IsReadOnly = this.IsReadOnly;
            if (ratingItem.Style == null)
            {
                ratingItem.Style = this.ItemContainerStyle;
            }
            ratingItem.Click += RatingItemClick;
            ratingItem.MouseEnter += RatingItemMouseEnter;
            ratingItem.MouseLeave += RatingItemMouseLeave;

            ratingItem.ParentRating = this;
            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// This method clears a container used to host an item.
        /// </summary>
        /// <param name="element">The container that hosts the item.</param>
        /// <param name="item">The item hosted in the container.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            RatingItem ratingItem = (RatingItem)element;
            ratingItem.Click -= RatingItemClick;
            ratingItem.MouseEnter -= RatingItemMouseEnter;
            ratingItem.MouseLeave -= RatingItemMouseLeave;
            ratingItem.ParentRating = null;

            if (ratingItem == HoveredRatingItem)
            {
                HoveredRatingItem = null;
                UpdateDisplayValues();
                UpdateHoverStates();
            }

            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// This method is invoked when a rating item's mouse enter event is
        /// invoked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RatingItemMouseEnter(object sender, MouseEventArgs e)
        {
            HoveredRatingItem = (RatingItem) sender;
            UpdateHoverStates();
        }

        /// <summary>
        /// This method is invoked when a rating item's mouse leave event is
        /// invoked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RatingItemMouseLeave(object sender, MouseEventArgs e)
        {
            HoveredRatingItem = null;
            UpdateDisplayValues();
            UpdateHoverStates();
        }

        /// <summary>
        /// Returns a sequence of rating items.
        /// </summary>
        /// <returns>A sequence of rating items.</returns>
        internal IEnumerable<RatingItem> GetRatingItems()
        {
            return
                Enumerable
                    .Range(0, this.Items.Count)
                    .Select(index => (RatingItem)ItemContainerGenerator.ContainerFromIndex(index))
                    .Where(ratingItem => ratingItem != null);
        }

        /// <summary>
        /// Selects a rating item.
        /// </summary>
        /// <param name="selectedRatingItem">The selected rating item.</param>
        internal void SelectRatingItem(RatingItem selectedRatingItem)
        {
            if (!this.IsReadOnly)
            {
                IList<RatingItem> ratingItems = GetRatingItems().ToList();
                IEnumerable<double> weights = ratingItems.Select(ratingItem => 1.0);
                double total = ratingItems.Count();
                double percent;
                if (total != 0)
                {
                    percent = weights.Take(ratingItems.IndexOf(selectedRatingItem) + 1).Sum() / total;
                    this.Value = percent;
                }
            }
        }

        /// <summary>
        /// This method is raised when a rating item value is selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Information about the event.</param>
        private void RatingItemClick(object sender, RoutedEventArgs e)
        {
            if (!this.IsReadOnly)
            {
                RatingItem item = (RatingItem)sender;
                OnRatingItemValueSelected(item, 1.0);
            }
        }

        /// <summary>
        /// Returns the selected rating item.
        /// </summary>
        /// <returns>The selected rating item.</returns>
        private RatingItem GetSelectedRatingItem()
        {
            return this.GetRatingItems().LastOrDefault(ratingItem => ratingItem.Value > 0.0);
        }

        /// <summary>
        /// This method is invoked when the rating item value is changed.
        /// </summary>
        /// <param name="ratingItem">The rating item that has changed.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnRatingItemValueSelected(RatingItem ratingItem, double newValue)
        {
            List<RatingItem> ratingItems = GetRatingItems().ToList();
            double total = ratingItems.Count();

            double value =
                (ratingItems
                    .Take(ratingItems.IndexOf(ratingItem))
                    .Count() + newValue) / total;

            this.Value = value;
        }

        /// <summary>
        /// Returns a RatingItemAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>A RatingItemAutomationPeer object for the RatingItem.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RatingAutomationPeer(this);
        }

        /// <summary>
        /// Provides handling for the
        /// <see cref="E:System.Windows.UIElement.KeyDown" /> event when a key
        /// is pressed while the control has focus.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains
        /// the event data.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="e " />is null.
        /// </exception>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!Interaction.AllowKeyDown(e))
            {
                return;
            }

            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
            Key invariantKey = InteractionHelper.GetLogicalKey(FlowDirection, e.Key);

            switch (invariantKey)
            {
                case Key.Left:
                    {
#if SILVERLIGHT
                        RatingItem ratingItem = FocusManager.GetFocusedElement() as RatingItem;
#else
                        RatingItem ratingItem = FocusManager.GetFocusedElement(Application.Current.MainWindow) as RatingItem;
#endif
                        if (ratingItem != null)
                        {
                            ratingItem = GetRatingItemAtOffsetFrom(ratingItem, -1);
                        }
                        else
                        {
                            ratingItem = GetRatingItems().FirstOrDefault();
                        }
                        if (ratingItem != null)
                        {
                            if (ratingItem.Focus())
                            {
                                e.Handled = true;
                            }
                        }
                    }
                    break;
                case Key.Right:
                    {
#if SILVERLIGHT
                        RatingItem ratingItem = FocusManager.GetFocusedElement() as RatingItem;
#else
                        RatingItem ratingItem = FocusManager.GetFocusedElement(Application.Current.MainWindow) as RatingItem;
#endif
                        if (ratingItem != null)
                        {
                            ratingItem = GetRatingItemAtOffsetFrom(ratingItem, 1);
                        }
                        else
                        {
                            ratingItem = GetRatingItems().FirstOrDefault();
                        }
                        if (ratingItem != null)
                        {
                            if (ratingItem.Focus())
                            {
                                e.Handled = true;
                            }
                        }                        
                    }
                    break;
                case Key.Add:
                    {
                        if (!this.IsReadOnly)
                        {
                            RatingItem ratingItem = GetSelectedRatingItem();
                            if (ratingItem != null)
                            {
                                ratingItem = GetRatingItemAtOffsetFrom(ratingItem, 1);
                            }
                            else
                            {
                                ratingItem = GetRatingItems().FirstOrDefault();
                            }
                            if (ratingItem != null)
                            {
                                ratingItem.SelectValue();
                                e.Handled = true;
                            }
                        }
                    }
                    break;
                case Key.Subtract:
                    {
                        if (!this.IsReadOnly)
                        {
                            RatingItem ratingItem = GetSelectedRatingItem();
                            if (ratingItem != null)
                            {
                                ratingItem = GetRatingItemAtOffsetFrom(ratingItem, -1);
                            }
                            if (ratingItem != null)
                            {
                                ratingItem.SelectValue();
                                e.Handled = true;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets a rating item at a certain index offset from another 
        /// rating item.
        /// </summary>
        /// <param name="ratingItem">The rating item.</param>
        /// <param name="offset">The rating item at an offset from the 
        /// index of the rating item.</param>
        /// <returns>The rating item at the offset.</returns>
        private RatingItem GetRatingItemAtOffsetFrom(RatingItem ratingItem, int offset)
        {
            IList<RatingItem> ratingItems = GetRatingItems().ToList();
            int index = ratingItems.IndexOf(ratingItem);
            if (index == -1)
            {
                return null;
            }
            index += offset;
            if (index >= 0 && index < ratingItems.Count)
            {
                ratingItem = ratingItems[index];
            }
            else
            {
                ratingItem = null;
            }
            return ratingItem;
        }

        /// <summary>
        /// Updates the visual state.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to use transitions.</param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            Interaction.UpdateVisualStateBase(useTransitions);
        }
    }
}
