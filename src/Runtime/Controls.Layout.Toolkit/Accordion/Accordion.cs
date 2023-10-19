// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Properties = OpenSilver.Controls.Properties;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a collection of collapsed and expanded AccordionItem controls.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(AccordionItem))]
    [StyleTypedProperty(Property = AccordionButtonStyleName, StyleTargetType = typeof(AccordionButton))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    public class Accordion : ItemsControl, IUpdateVisualState
    {
        /// <summary>
        /// The items that are currently waiting to perform an action.
        /// </summary>
        /// <remarks>An action can be expanding, resizing or collapsing.</remarks>
        private readonly List<AccordionItem> _scheduledActions;

        /// <summary>
        /// The name used to indicate AccordionButtonStyle property.
        /// </summary>
        private const string AccordionButtonStyleName = "AccordionButtonStyle";

        /// <summary>
        /// Determines whether the SelectedItemsProperty may be written.
        /// </summary>
        private bool _isAllowedToWriteSelectedItems;

        /// <summary>
        /// Determines whether the SelectedIndicesProperty may be written.
        /// </summary>
        private bool _isAllowedToWriteSelectedIndices;

        /// <summary>
        /// Indicates that changes to the SelectedIndices collection should
        /// be ignored.
        /// </summary>
        private bool _isIgnoringSelectedIndicesChanges;

        /// <summary>
        /// Indicates that changes to the SelectedItems collection should
        /// be ignored.
        /// </summary>
        private bool _isIgnoringSelectedItemsChanges;

        /// <summary>
        /// Determines whether we are currently in the SelectedItems Collection
        /// Changed handling.
        /// </summary>
        private bool _isInSelectedItemsCollectionChanged;

        /// <summary>
        /// Determines whether we are currently in the SelectedIndices Collection
        /// Changed handling.
        /// </summary>
        private bool _isInSelectedIndicesCollectionChanged;

        /// <summary>
        /// The item that is currently visually performing an action.
        /// </summary>
        /// <remarks>An action can be expanding, resizing or collapsing.</remarks>
        private AccordionItem _currentActioningItem;

        /// <summary>
        /// Gets the ItemsControlHelper that is associated with this control.
        /// </summary>
        internal ItemsControlHelper ItemsControlHelper { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is currently resizing.
        /// </summary>
        /// <value>True if this instance is resizing; otherwise, false.</value>
        internal bool IsResizing { get; private set; }

        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        private InteractionHelper Interaction { get; set; }

        #region public ExpandDirection ExpandDirection
        /// <summary>
        /// Gets or sets the ExpandDirection property of each 
        /// AccordionItem in the Accordion control and the direction in which
        /// the Accordion does layout.
        /// </summary>
        /// <remarks>Setting the ExpandDirection will set the expand direction 
        /// on the accordionItems.</remarks>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                "ExpandDirection",
                typeof(ExpandDirection),
                typeof(Accordion),
                new PropertyMetadata(ExpandDirection.Down, OnExpandDirectionPropertyChanged));

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its ExpandDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExpandDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;
            ExpandDirection expandDirection = (ExpandDirection)e.NewValue;

            if (expandDirection != ExpandDirection.Down &&
                expandDirection != ExpandDirection.Up &&
                expandDirection != ExpandDirection.Left &&
                expandDirection != ExpandDirection.Right)
            {
                // revert to old value
                source.SetValue(ExpandDirectionProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Accordion_OnExpandDirectionPropertyChanged_InvalidValue,
                    expandDirection);

                throw new ArgumentOutOfRangeException("e", message);
            }

            // force this change to all AccordionItems
            for (int i = 0; i < source.Items.Count; i++)
            {
                AccordionItem accordionItem = source.ItemContainerGenerator.ContainerFromIndex(i) as AccordionItem;

                if (accordionItem != null)
                {
                    accordionItem.ExpandDirection = expandDirection;
                }
            }

            // set panel to align to the change
            source.SetPanelOrientation();

            // schedule a layout pass after this panel has had time to rearrange.
            source.Dispatcher.BeginInvoke(source.LayoutChildren);
        }
        #endregion public ExpandDirection ExpandDirection

        #region public AccordionSelectionMode SelectionMode
        /// <summary>
        /// Gets or sets the AccordionSelectionMode used to determine the minimum 
        /// and maximum selected AccordionItems allowed in the Accordion.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "WPF has a MultiSelector class that will be used in the future.")]
        public AccordionSelectionMode SelectionMode
        {
            get { return (AccordionSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(AccordionSelectionMode),
                typeof(Accordion),
                new PropertyMetadata(AccordionSelectionMode.One, OnSelectionModePropertyChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;

            AccordionSelectionMode newValue = (AccordionSelectionMode)e.NewValue;

            if (newValue != AccordionSelectionMode.One &&
                newValue != AccordionSelectionMode.OneOrMore &&
                newValue != AccordionSelectionMode.ZeroOrMore &&
                newValue != AccordionSelectionMode.ZeroOrOne)
            {
                // revert to old value
                source.SetValue(SelectionModeProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Accordion_OnSelectionModePropertyChanged_InvalidValue,
                    newValue);
                throw new ArgumentOutOfRangeException("e", message);
            }

            // unlock all items
            // a selectionmode change is expected to change the locks.
            for (int i = 0; i < source.Items.Count; i++)
            {
                AccordionItem item = source.ItemContainerGenerator.ContainerFromIndex(i) as AccordionItem;
                if (item != null)
                {
                    item.IsLocked = false;
                }
            }

            // single selection coercion
            if (source.IsMinimumOneSelected)
            {
                // a minimum of one item should be selected
                if (source.GetValue(SelectedItemProperty) == null && source.Items.Count > 0)
                {
                    // select first accordionitem
                    source.SetValue(SelectedItemProperty, source.Items[0]);
                }
            }

            // multi selection coeercion
            if (source.IsMaximumOneSelected)
            {
                // allow at most one item.
                if (source.SelectedIndices.Count > 1)
                {
                    // make copy of collection, since it will be modified
                    List<int> indices = source.SelectedIndices.ToList();
                    foreach (int index in indices)
                    {
                        // unselect all items except the currently selected item.
                        if (index != source.SelectedIndex)
                        {
                            source.UnselectItem(index, null);
                        }
                    }
                }
            }

            // re-evaluate the locking status of the items in this new configuration
            source.SetLockedProperties();
        }

        /// <summary>
        /// Gets a value indicating whether at least one item is selected at 
        /// all times.
        /// </summary>
        private bool IsMinimumOneSelected
        {
            get
            {
                return SelectionMode == AccordionSelectionMode.One || SelectionMode == AccordionSelectionMode.OneOrMore;
            }
        }

        /// <summary>
        /// Gets a value indicating whether at most one item is selected at all times.
        /// </summary>
        private bool IsMaximumOneSelected
        {
            get
            {
                return SelectionMode == AccordionSelectionMode.One || SelectionMode == AccordionSelectionMode.ZeroOrOne;
            }
        }
        #endregion public AccordionSelectionMode SelectionMode

        #region public object SelectedItem
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// When multiple items are allowed (IsMaximumOneSelected false), 
        /// return the first of the selectedItems.
        /// </remarks>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(Accordion),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;

            object oldValue = e.OldValue;
            object newValue = e.NewValue;
            object[] newValues = newValue == null ? new object[0] : new[] { newValue };
            object[] oldValues = oldValue == null ? new object[0] : new[] { oldValue };

            if (oldValue != null && oldValue.Equals(newValue))
            {
                // when value types are used as items, there is a possibility of getting a change notification.
                source.OnSelectedItemChanged(new SelectionChangedEventArgs(oldValues, newValues));
                return;
            }

            if (!source.IsValidItemForSelection(newValue))
            {
                // reset to oldvalue
                source._selectedItemNestedLevel++;
                source.SetValue(SelectedItemProperty, oldValue);
                source._selectedItemNestedLevel--;
            }
            else if (source._selectedItemNestedLevel == 0)
            {
                if (newValue == null)
                {
                    source.SelectedIndex = -1;
                }
                else
                {
                    // be cautious about choosing a new index.
                    int currentIndex = source.SelectedIndex;
                    // use current SelectedIndex if possible
                    if (currentIndex < 0 || currentIndex > source.Items.Count || !newValue.Equals(source.Items[currentIndex]))
                    {
                        // use an index out of SelectedIndices if possible
                        // or fallback to finding the index in the ItemsCollection
                        IEnumerable<int> validIndices = source.SelectedIndices.Where(i => i >= 0 && i < source.Items.Count && newValue.Equals(source.Items[i]));
                        currentIndex = validIndices.Count() > 0 ? validIndices.First() : source.Items.IndexOf(newValue);
                    }
                    source.SelectedIndex = currentIndex;
                }

                source.OnSelectedItemChanged(new SelectionChangedEventArgs(oldValues, newValues));
            }
        }

        /// <summary>
        /// Determines whether the new value can be selected.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// 	<c>True</c> if this item can be selected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidItemForSelection(object newValue)
        {
            // setting to null is supported in some cases
            if (newValue == null)
            {
                // we can always return something since null is not a valid item.
                // if accordion allows no selection, null is accepted
                // if there are currently no items, null is accepted
                return (IsMinimumOneSelected == false || Items.Count == 0);
            }

            // item should be contained inside the items collection.
            return Items.OfType<object>().Contains(newValue);
        }

        /// <summary>
        /// Nested level for SelectedItemCoercion.
        /// </summary>
        private int _selectedItemNestedLevel;

        #endregion public object SelectedItem

        #region public int SelectedIndex
        /// <summary>
        /// Gets or sets the index of the currently selected AccordionItem.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(Accordion),
                new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        /// <summary>
        /// SelectedIndexProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectedIndex.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;
            int oldValue = (int)e.OldValue;
            int newValue = (int)e.NewValue;

            // SelectedIndex will be changed when modifying the SelectionCollections.
            // Those should not trigger changes in the SelectedIndex.
            if (source._isIgnoringSelectedIndicesChanges)
            {
                return;
            }

            if (!source.IsValidIndexForSelection(newValue))
            {
                // oldvalue might not be valid anymore (because of items removed from collection)
                if (source.IsValidIndexForSelection(oldValue))
                {
                    // oldvalue is valid, repress events
                    source._selectedIndexNestedLevel++;
                    source.SetValue(SelectedIndexProperty, oldValue);
                    source._selectedIndexNestedLevel--;
                }
                else
                {
                    // select new
                    source.SetValue(SelectedIndexProperty, source.ProposeSelectedIndexCandidate(newValue));
                }
            }
            else if (source._selectedIndexNestedLevel == 0)
            {
                // synchronize with SelectedItem
                source.SelectedItem = source.Items.ElementAtOrDefault(newValue);

                // SelectedIndex is responsible for kicking off the real work.
                source.ChangeSelectedIndex(oldValue, newValue);
            }
        }

        /// <summary>
        /// Determines whether the new value can be selected.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// 	<c>True</c> if this item can be selected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidIndexForSelection(int newValue)
        {
            // setting to null is supported in some cases
            if (newValue == -1)
            {
                // we can always return something since null is not a valid item.
                // if accordion allows no selection, null is accepted
                // if there are currently no items, null is accepted
                return (IsMinimumOneSelected == false || Items.Count == 0);
            }

            // index should be contained inside the items collection.
            return newValue >= 0 && newValue < Items.Count;
        }

        /// <summary>
        /// Coercion level.
        /// </summary>
        private int _selectedIndexNestedLevel;
        #endregion public int SelectedIndex

        #region public SelectionSequence SelectionSequence
        /// <summary>
        /// Gets or sets the SelectionSequence used to determine 
        /// the order of AccordionItem selection.
        /// </summary>
        public SelectionSequence SelectionSequence
        {
            get { return (SelectionSequence)GetValue(SelectionSequenceProperty); }
            set { SetValue(SelectionSequenceProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionSequence dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionSequenceProperty =
            DependencyProperty.Register(
                "SelectionSequence",
                typeof(SelectionSequence),
                typeof(Accordion),
                new PropertyMetadata(SelectionSequence.Simultaneous, OnSelectionSequencePropertyChanged));

        /// <summary>
        /// Called when SelectionSequenceProperty changed.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectionSequence property.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private static void OnSelectionSequencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectionSequence newValue = (SelectionSequence)e.NewValue;

            if (newValue != SelectionSequence.CollapseBeforeExpand &&
                newValue != SelectionSequence.Simultaneous)
            {
                // revert to old value
                d.SetValue(Accordion.SelectionSequenceProperty, e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Accordion_OnSelectionSequencepropertyChanged_InvalidValue,
                    newValue);
                throw new ArgumentOutOfRangeException("e", message);
            }
        }
        #endregion public SelectionSequence SelectionSequence

        #region public IList SelectedItems
        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <remarks>Does not allow setting.</remarks>
        public IList SelectedItems
        {
            get { return GetValue(SelectedItemsProperty) as IList; }
            private set
            {
                _isAllowedToWriteSelectedItems = true;
                SetValue(SelectedItemsProperty, value);
                _isAllowedToWriteSelectedItems = false;
            }
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(IList),
                typeof(Accordion),
                new PropertyMetadata(OnSelectedItemsChanged));

        /// <summary>
        /// Property changed handler of SelectedItems.
        /// </summary>
        /// <param name="d">Accordion that changed the collection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion accordion = (Accordion)d;

            if (!accordion._isAllowedToWriteSelectedItems)
            {
                // revert to old value
                accordion.SelectedItems = e.OldValue as IList;

                throw new InvalidOperationException(Properties.Resources.Accordion_OnSelectedItemsChanged_InvalidWrite);
            }
        }
        #endregion public IList SelectedItems

        #region public IList<int> SelectedIndices
        /// <summary>
        /// Gets the indices of the currently selected AccordionItems.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Framework uses indices.")]
        public IList<int> SelectedIndices
        {
            get { return GetValue(SelectedIndicesProperty) as IList<int>; }
            private set
            {
                _isAllowedToWriteSelectedIndices = true;
                SetValue(SelectedIndicesProperty, value);
                _isAllowedToWriteSelectedIndices = false;
            }
        }

        /// <summary>
        /// Identifies the SelectedIndices dependency property.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "Framework uses indices.")]
        public static readonly DependencyProperty SelectedIndicesProperty =
            DependencyProperty.Register(
                "SelectedIndices",
                typeof(IList<int>),
                typeof(Accordion),
                new PropertyMetadata(null, OnSelectedIndicesChanged));

        /// <summary>
        /// Property changed handler of SelectedIndices.
        /// </summary>
        /// <param name="d">Accordion that changed the collection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedIndicesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion accordion = (Accordion)d;

            if (!accordion._isAllowedToWriteSelectedIndices)
            {
                // revert to old value
                accordion.SelectedIndices = e.OldValue as IList<int>;

                throw new InvalidOperationException(Properties.Resources.Accordion_OnSelectedIndicesChanged_InvalidWrite);
            }
        }
        #endregion public IList<int> SelectedIndices

        #region public Style ItemContainerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the container element
        /// generated for each item.
        /// </summary>
        public new Style ItemContainerStyle
        {
            get { return GetValue(ItemContainerStyleProperty) as Style; }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public new static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(
                "ItemContainerStyle",
                typeof(Style),
                typeof(Accordion),
                new PropertyMetadata(null, OnItemContainerStylePropertyChanged));

        /// <summary>
        /// ItemContainerStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// TreeView that changed its ItemContainerStyle.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnItemContainerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Accordion source = (Accordion)d;
            Style value = e.NewValue as Style;
            source.ItemsControlHelper.UpdateItemContainerStyle(value);
        }
        #endregion public Style ItemContainerStyle

        #region public Style AccordionButtonStyle
        /// <summary>
        /// Gets or sets the Style that is applied to AccordionButton elements
        /// in the AccordionItems.
        /// </summary>
        public Style AccordionButtonStyle
        {
            get { return GetValue(AccordionButtonStyleProperty) as Style; }
            set { SetValue(AccordionButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the AccordionButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty AccordionButtonStyleProperty =
            DependencyProperty.Register(
                AccordionButtonStyleName,
                typeof(Style),
                typeof(Accordion),
                new PropertyMetadata(null, OnAccordionButtonStylePropertyChanged));

        /// <summary>
        /// AccordionButtonStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its AccordionButtonStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAccordionButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion public Style AccordionButtonStyle

        #region public DataTemplate ContentTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display the content 
        /// of each generated AccordionItem. 
        /// </summary>
        /// <remarks>Either ContentTemplate or ItemTemplate is used. 
        /// Setting both will result in an exception.</remarks>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(Accordion),
                new PropertyMetadata(null));
        #endregion public DataTemplate ContentTemplate

        /// <summary>
        /// Occurs when the SelectedItem or SelectedItems property value changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the SelectedItems collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler SelectedItemsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Accordion"/> class.
        /// </summary>
        public Accordion()
        {
            DefaultStyleKey = typeof(Accordion);
            ItemsControlHelper = new ItemsControlHelper(this);

            ObservableCollection<object> items = new ObservableCollection<object>();
            ObservableCollection<int> indices = new ObservableCollection<int>();

            SelectedItems = items;
            SelectedIndices = indices;

            items.CollectionChanged += OnSelectedItemsCollectionChanged;
            indices.CollectionChanged += OnSelectedIndicesCollectionChanged;

            _scheduledActions = new List<AccordionItem>();
            SizeChanged += OnAccordionSizeChanged;
            Interaction = new InteractionHelper(this);
        }

        /// <summary>
        /// Builds the visual tree for the Accordion control when a 
        /// new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ItemsControlHelper.OnApplyTemplate();
            base.OnApplyTemplate();
            Interaction.OnApplyTemplateBase();
        }

        /// <summary>
        /// Returns a AccordionAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>A AccordionAutomationPeer object for the Accordion.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AccordionAutomationPeer(this);
        }

        #region ItemsControl
        /// <summary>
        /// Creates or identifies the element that is used to display the given 
        /// item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AccordionItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own 
        /// container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// True if the item is (or is eligible to be) its own container; 
        /// otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AccordionItem;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            AccordionItem accordionItem = element as AccordionItem;

            if (accordionItem != null)
            {
                DataTemplate specifiedContentTemplate = accordionItem.ContentTemplate;

                base.PrepareContainerForItemOverride(element, item);
                ItemsControlHelper.PrepareContainerForItemOverride(accordionItem, ItemContainerStyle);
                AccordionItem.PreparePrepareHeaderedContentControlContainerForItemOverride(accordionItem, item, this, ItemContainerStyle);

                // after base.prepare, item template has replaced contenttemplate
                DataTemplate displayMemberTemplate = accordionItem.ContentTemplate;

                // put original contenttemplate back that was overwritten
                // It takes precendence over a generated itemtemplate.
                // this might mean setting back a null, which is correct given the bindings
                // that follow.
                accordionItem.ContentTemplate = specifiedContentTemplate;

                // potentially set contenttemplate if accordionItem did not specify one explicitly
                if (accordionItem.ContentTemplate == null)
                {
                    accordionItem.SetBinding(
                        ContentControl.ContentTemplateProperty,
                        new Binding("ContentTemplate")
                        {
                            Source = this,
                            Mode = BindingMode.OneWay
                        });
                }

                // potentially set headertemplate if accordionItem did not specify one explicitly
                if (accordionItem.HeaderTemplate == null)
                {
                    accordionItem.SetBinding(
                        HeaderedContentControl.HeaderTemplateProperty,
                        new Binding("ItemTemplate")
                        {
                            Source = this,
                            Mode = BindingMode.OneWay
                        });
                }

                // potentially bind AccordionButtonStyle.
                if (accordionItem.AccordionButtonStyle == null)
                {
                    accordionItem.SetBinding(
                        AccordionItem.AccordionButtonStyleProperty,
                        new Binding(AccordionButtonStyleName)
                        {
                            Source = this,
                            Mode = BindingMode.OneWay
                        });
                }

                // possibly set a displaymemberPath on header or content.
                if (displayMemberTemplate != null && !string.IsNullOrEmpty(DisplayMemberPath))
                {
                    if (accordionItem.ContentTemplate == null)
                    {
                        accordionItem.ContentTemplate = displayMemberTemplate;
                    }
                    if (accordionItem.HeaderTemplate == null)
                    {
                        accordionItem.HeaderTemplate = displayMemberTemplate;
                    }
                }

                // give accordionItem a reference back to the parent Accordion.
                accordionItem.ParentAccordion = this;

                // SelectedItem is expected to be set while adding items.
                // Check: does this item belong in the selectedindices
                int index = ItemContainerGenerator.IndexFromContainer(accordionItem);
                if (!accordionItem.IsSelected && SelectedIndices.Contains(index))
                {
                    accordionItem.IsSelected = true;
                }

                // could also be adding an item with the IsSelected set to true.
                if (accordionItem.IsSelected)
                {
                    SelectedItem = item;
                }

                // item might have been preselected when added to the item collection. 
                // at that point the parent had not been registered yet, so no notification was done.
                if (accordionItem.IsSelected)
                {
                    if (!SelectedItems.OfType<object>().Contains(item))
                    {
                        SelectedItems.Add(item);
                    }

                    if (!SelectedIndices.Contains(index))
                    {
                        SelectedIndices.Add(index);
                    }
                }
                accordionItem.ExpandDirection = ExpandDirection;
            }
            else
            {
                base.PrepareContainerForItemOverride(element, item);
                ItemsControlHelper.PrepareContainerForItemOverride(element, ItemContainerStyle);
            }

            // The panel will register itself when it has had a child to add.
            SetPanelOrientation();

            // change has occured, re-evaluate the locked status on items
            SetLockedProperties();

            // At this moment this item has not been added to the panel yet, so we schedule a layoutpass
            Dispatcher.BeginInvoke(LayoutChildren);
        }

        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> 
        /// method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item that should be cleared.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            AccordionItem accordionItem = element as AccordionItem;
            if (accordionItem != null)
            {
                accordionItem.IsLocked = false;
                accordionItem.IsSelected = false;

                // release the parent child relationship.
                accordionItem.ParentAccordion = null;
            }

            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> 
        /// property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    try
                    {
                        _isIgnoringSelectedIndicesChanges = true;
                        for (int i = 0; i < SelectedIndices.Count; i++)
                        {
                            if (SelectedIndices[i] >=
                                e.NewStartingIndex)
                            {
                                // add a value of one
                                SelectedIndices[i] = SelectedIndices[i] + 1;
                            }
                        }
                    }
                    finally
                    {
                        _isIgnoringSelectedIndicesChanges = false;
                    }

                    if (SelectedIndex >= e.NewStartingIndex && SelectedIndex > -1)
                    {
                        SelectedIndex++;
                    }

                    // now add the item, will also add indice at correct position.
                    if (SelectedItem == null && IsMinimumOneSelected)
                    {
                        if (!SelectedItems.OfType<object>().Contains(e.NewItems[0]))
                        {
                            SelectedItems.Add(e.NewItems[0]);
                        }
                        SelectedItem = e.NewItems[0];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        _isIgnoringSelectedIndicesChanges = true;
                        _isIgnoringSelectedItemsChanges = true;
                        try
                        {
                            // Items has been cleared.
                            // so clear selecteditems as well
                            SelectedItems.Clear();
                            SelectedIndices.Clear();
                            SelectedItem = null;
                            SelectedIndex = -1;
                        }
                        finally
                        {
                            _isIgnoringSelectedIndicesChanges = false;
                            _isIgnoringSelectedItemsChanges = false;
                        }

                        // we receive this action when an itemssource is set
                        InitializeNewItemsSource();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        int index = e.OldStartingIndex;
                        object item = e.OldItems[0];

                        try
                        {
                            _isIgnoringSelectedIndicesChanges = true;

                            if (SelectedIndices.Contains(index))
                            {
                                SelectedIndices.Remove(index);
                            }
                            for (int i = 0; i < SelectedIndices.Count; i++)
                            {
                                if (SelectedIndices[i] > index)
                                {
                                    // lower the value by one
                                    SelectedIndices[i] = SelectedIndices[i] - 1;
                                }
                            }
                        }
                        finally
                        {
                            _isIgnoringSelectedIndicesChanges = false;
                        }

                        try
                        {
                            _isIgnoringSelectedItemsChanges = true;

                            if (SelectedItems.Contains(item))
                            {
                                // check that there are no indices pointing to similar
                                // items that are still in the collection
                                if (SelectedIndices.Count(i => i < Items.Count && Items[i].Equals(item)) == 0)
                                {
                                    SelectedItems.Remove(item);
                                }
                            }
                        }
                        finally
                        {
                            _isIgnoringSelectedItemsChanges = false;
                        }

                        if (SelectedIndex == index)
                        {
                            // that item is no longer in the Items collection
                            // so the index is incorrect as well
                            SelectedIndex = -1;
                        }
                        if (SelectedIndex > e.OldStartingIndex && SelectedIndex > -1)
                        {
                            SelectedIndex -= 1;
                        }
                    }
                    break;
            }

            SetPanelOrientation();
        }

        /// <summary>
        /// Initializes the SelectedItem property when a new ItemsSource is set.
        /// </summary>
        private void InitializeNewItemsSource()
        {
            // todo: remove the SelectedItem == null check (should not be necessary)

            // possibly an ItemsSource has been set
            if (IsMinimumOneSelected && SelectedItem == null && Items.Count > 0)
            {
                if (!SelectedItems.OfType<object>().Contains(Items[0]))
                {
                    SelectedItems.Add(Items[0]);
                }
                SelectedItem = Items[0];
            }
        }
        #endregion ItemsControl

        #region Selection handling
        /// <summary>
        /// Called when an AccordionItem is unselected.
        /// </summary>
        /// <param name="accordionItem">The accordion item that was unselected.</param>
        internal void OnAccordionItemUnselected(AccordionItem accordionItem)
        {
            UnselectItem(ItemContainerGenerator.IndexFromContainer(accordionItem), ItemContainerGenerator.ItemFromContainer(accordionItem));
        }

        /// <summary>
        /// Unselects the item.
        /// </summary>
        /// <param name="index">The index of the item that will be unselected.</param>
        /// <param name="item">The item that will be unselected. Can be null.</param>
        private void UnselectItem(int index, object item)
        {
            if (index < 0 || index >= Items.Count)
            {
                // invalid
                return;
            }

            // try through accordionitem
            AccordionItem container = index >= 0 && index < Items.Count ? ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem : null;
            if (container != null && container.IsSelected)
            {
                container.IsLocked = false;
                container.IsSelected = false;
                return;
            }

            item = item ?? Items[index];

            int newSelectedIndex = -1;
            // shortcuts to new item selection.
            if (SelectedIndex > -1 && SelectedIndex == index)
            {
                // this item is no longer the selected item. 
                // in order to keep the amount of raised events down, will select a new selected item here.
                newSelectedIndex = ProposeSelectedIndexCandidate(index);

                // no cancelling possible, undo the action.
                // current template makes sure accordionheader does not allow this unselect
                // that behavior is not enforced which means that it could come
                // from a SelectedItems/Indices manipulation
                SelectedIndex = newSelectedIndex;
            }

            // update selecteditems collection
            if (SelectedItems.OfType<object>().Contains(item) && index != newSelectedIndex && !item.Equals(SelectedItem))
            {
                // if there are indices still pointing to a similar item, do not remove
                if (SelectedIndices.Count(i => i != index && i < Items.Count && Items[i].Equals(item)) == 0)
                {
                    if (_isInSelectedItemsCollectionChanged)
                    {
                        throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                    }
                    SelectedItems.Remove(item);
                }
            }

            // indexes should always be changed, even if we are reselecting a similar item
            if (SelectedIndices.Contains(index))
            {
                if (_isInSelectedIndicesCollectionChanged)
                {
                    throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                }
                SelectedIndices.Remove(index);
            }
        }

        /// <summary>
        /// Called when an AccordionItem selected.
        /// </summary>
        /// <param name="accordionItem">The accordion item that was selected.</param>
        internal void OnAccordionItemSelected(AccordionItem accordionItem)
        {
            SelectItem(ItemContainerGenerator.IndexFromContainer(accordionItem));
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="index">The index of the item to select.</param>
        private void SelectItem(int index)
        {
            // try through accordionitem
            AccordionItem container = index >= 0 && index < Items.Count ? ItemContainerGenerator.ContainerFromIndex(index) as AccordionItem : null;
            if (container != null && !container.IsSelected)
            {
                container.IsSelected = true;
                return;
            }

            SelectedIndex = index;

            object item = Items[index];
            if (item != null)
            {
                // update selecteditems collection
                if (!SelectedItems.OfType<object>().Contains(item))
                {
                    if (_isInSelectedItemsCollectionChanged)
                    {
                        throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                    }
                    SelectedItems.Add(item);
                }

                if (!SelectedIndices.Contains(index))
                {
                    if (_isInSelectedIndicesCollectionChanged)
                    {
                        throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                    }
                    SelectedIndices.Add(index);
                }
            }
        }

        /// <summary>
        /// Changes the selected item, by unselecting and selecting where 
        /// necessary.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        private void ChangeSelectedIndex(int oldIndex, int newIndex)
        {
            AccordionItem oldAccordionItem = oldIndex >= 0 && oldIndex < Items.Count ? ItemContainerGenerator.ContainerFromIndex(oldIndex) as AccordionItem : null;
            AccordionItem newAccordionItem = newIndex >= 0 && newIndex < Items.Count ? ItemContainerGenerator.ContainerFromIndex(newIndex) as AccordionItem : null;

            // unselect the previous item, if we need to
            // we should be able to be called when the oldvalue equals the newvalue
            if (oldIndex != newIndex)
            {
                // we only need to explicitly deselect the oldvalue if there is a maximum
                // of one selected. However, if user explicitly set SelectedItem
                // to null, we should still deselect the old value.
                if (IsMaximumOneSelected || newIndex == -1)
                {
                    if (oldAccordionItem != null)
                    {
                        // unselection can be triggered by selection of another item.
                        oldAccordionItem.IsLocked = false;
                        oldAccordionItem.IsSelected = false;
                    }
                    else if (oldIndex > -1)
                    {
                        // there was no wrapper yet, fallback to regular unselecting
                        UnselectItem(oldIndex, null);
                    }

                    #region raise event for UIAutomation.
                    if (newAccordionItem != null &&
                        AutomationPeer.ListenerExists(
                                AutomationEvents.SelectionItemPatternOnElementSelected))
                    {
                        AutomationPeer peer =
                                FrameworkElementAutomationPeer.CreatePeerForElement(newAccordionItem);
                        if (peer != null)
                        {
                            peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                        }
                    }
                    #endregion
                }
            }

            // make the selection through the container if possible
            if (newAccordionItem != null)
            {
                newAccordionItem.IsSelected = true;
            }
            else if (newIndex != -1)
            {
                // there was no wrapper yet, fallback to regular selecting
                SelectItem(newIndex);
            }

            SelectedIndex = newIndex;
        }

        /// <summary>
        /// Called when selected items collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isIgnoringSelectedItemsChanges)
            {
                return;
            }

            _isInSelectedItemsCollectionChanged = true;

            Action<object> unselectItem = item =>
                                              {
                                                  // since we removed this selecteditem, all selectedindices need to be removed as well
                                                  List<int> valid = SelectedIndices.Where(i => i < Items.Count && item.Equals(Items[i])).ToList();
                                                  if (valid.Count > 0)
                                                  {
                                                      foreach (int index in valid)
                                                      {
                                                          UnselectItem(index, item);
                                                      }
                                                  }
                                                  else
                                                  {
                                                      UnselectItem(Items.IndexOf(item), item);
                                                  }
                                              };

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (IsMaximumOneSelected && (SelectedItem != null && !e.NewItems.Contains(SelectedItem)))
                        {
                            // will always lead to manipulation of the collection
                            throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                        }
                        foreach (object item in e.NewItems)
                        {
                            object tempItem = item;
                            SelectedItem = tempItem;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (IsMinimumOneSelected && e.OldItems.Contains(SelectedItem))
                        {
                            // will always lead to manipulation of the collection
                            throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                        }
                        foreach (object item in e.OldItems)
                        {
                            object tempItem = item;
                            unselectItem(tempItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        // unselect all items.
                        // we use the selectedindices collection to pinpoint the
                        // items we need to unselect
                        if (IsMinimumOneSelected && Items.Count > 0)
                        {
                            // will always lead to manipulation of the collection
                            throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                        }
                        for (int i = SelectedIndices.Count - 1; i >= 0; i--)
                        {
                            int selectedIndex = SelectedIndices[i];

                            if (selectedIndex < Items.Count)
                            {
                                object tempItem = Items[selectedIndex];
                                unselectItem(tempItem);
                            }
                        }
                    }
                    break;
                default:
                    {
                        string message = string.Format(
                                CultureInfo.InvariantCulture,
                                Properties.Resources.Accordion_UnsupportedCollectionAction,
                                e.Action);

                        throw new NotSupportedException(message);
                    }
            }

            // let the outside world know
            RaiseOnSelectedItemsCollectionChanged(e);

            _isInSelectedItemsCollectionChanged = false;
        }

        /// <summary>
        /// Called when selected indices collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Method is best kept coherent.")]
        private void OnSelectedIndicesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isIgnoringSelectedIndicesChanges)
            {
                return;
            }

            _isInSelectedIndicesCollectionChanged = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (IsMaximumOneSelected)
                        {
                            // selectedindex always trails the actual state
                            if (SelectedItem != null && e.NewItems.Count != 1 || ((int)e.NewItems[0] < Items.Count && !Items[(int)e.NewItems[0]].Equals(SelectedItem)))
                            {
                                // will always lead to manipulation of the collection
                                throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                            }
                        }
                        foreach (int index in e.NewItems)
                        {
                            if (index < Items.Count)
                            {
                                SelectedIndex = index;
                                // raise event for UIAutomation, which uses SelectedIndices to query SelectedItems.
                                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
                                {
                                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
                                    if (peer != null)
                                    {
                                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (IsMinimumOneSelected && e.OldItems.Contains(SelectedIndex))
                        {
                            if (SelectedIndex < Items.Count && Items[SelectedIndex].Equals(SelectedItem) && SelectedIndices.Count == 0)
                            {
                                // will always lead to manipulation of the collection
                                throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                            }
                        }
                        foreach (int index in e.OldItems)
                        {
                            if (index < Items.Count)
                            {
                                UnselectItem(index, null);

                                // raise event for UIAutomation, which uses SelectedIndices to query SelectedItems.
                                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                                {
                                    AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
                                    if (peer != null)
                                    {
                                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        // unselect all items.
                        // we use the selectedindices collection to pinpoint the
                        // items we need to unselect
                        if (IsMinimumOneSelected && Items.Count > 0)
                        {
                            // will always lead to manipulation of the collection
                            throw new InvalidOperationException(Properties.Resources.Accordion_InvalidManipulationOfSelectionCollections);
                        }
                        // unselect all items.
                        // we use the selectedItems collection to pinpoint the
                        // items we need to unselect
                        for (int i = SelectedItems.Count - 1; i >= 0; i--)
                        {
                            object item = SelectedItems[i];
                            UnselectItem(i, item);

                            // raise event for UIAutomation, which uses SelectedIndices to query SelectedItems.
                            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                            {
                                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
                                if (peer != null)
                                {
                                    peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                                }
                            }
                        }
                    }
                    break;
                default:
                    {
                        string message = string.Format(
                            CultureInfo.InvariantCulture,
                            Properties.Resources.Accordion_UnsupportedCollectionAction,
                            e.Action);

                        throw new NotSupportedException(message);
                    }
            }

            // change has occured, re-evaluate the locked status on items
            SetLockedProperties();

            // do a layout pass.
            LayoutChildren();

            _isInSelectedIndicesCollectionChanged = false;
        }

        #region Helpers
        /// <summary>
        /// Gets an item that is suitable for selection.
        /// </summary>
        /// <param name="nonCandidateIndex">Index that should not be considered if 
        /// possible.</param>
        /// <returns>An item that should be selected. This could be nonCandidateIndex, 
        /// if no other possibility was found.</returns>
        private int ProposeSelectedIndexCandidate(int nonCandidateIndex)
        {
            // other non candidates are items that are exactly like this item.
            object item = (nonCandidateIndex >= 0 && nonCandidateIndex < Items.Count) ? Items[nonCandidateIndex] : null;

            // see if we can find a suitable item in the selecteditems collection
            IEnumerable<int> validIndices = SelectedIndices.Where(i => i != nonCandidateIndex && (item == null || !item.Equals(Items[i])));

            if (validIndices.Count() > 0)
            {
                return validIndices.First();
            }

            if (IsMinimumOneSelected && Items.Count > 0)
            {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// Selects all the AccordionItems in the Accordion control.
        /// </summary>
        /// <remarks>If the Accordion SelectionMode is OneOrMore or ZeroOrMore all 
        /// AccordionItems would be selected. If the Accordion SelectionMode is 
        /// One or ZeroOrOne all items would be selected and unselected. Only 
        /// the last AccordionItem would remain selected. </remarks>
        public void SelectAll()
        {
            UpdateAccordionItemsSelection(true);
        }

        /// <summary>
        /// Unselects all the AccordionItems in the Accordion control.
        /// </summary>
        /// <remarks>If the Accordion SelectionMode is Zero or ZeroOrMore all 
        /// AccordionItems would be Unselected. If SelectionMode is One or  
        /// OneOrMode  than all items would be Unselected and selected. Only the 
        /// first AccordionItem would still be selected.</remarks>
        public void UnselectAll()
        {
            UpdateAccordionItemsSelection(false);
        }

        /// <summary>
        /// Updates all accordionItems to be selected or unselected.
        /// </summary>
        /// <param name="selectedValue">True to select all items, false to unselect.</param>
        /// <remarks>Will not attempt to change a locked accordionItem.</remarks>
        private void UpdateAccordionItemsSelection(bool selectedValue)
        {
            foreach (object item in Items)
            {
                AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;

                if (accordionItem != null && !accordionItem.IsLocked)
                {
                    accordionItem.IsSelected = selectedValue;
                }
            }
        }

        /// <summary>
        /// Sets the locked properties on all the items.
        /// </summary>
        private void SetLockedProperties()
        {
            // an item that can not be unselected is locked.
            // This happens in 'One' or 'OneOrMore' selection mode, when the first item is selected.
            for (int i = 0; i < Items.Count; i++)
            {
                AccordionItem item = ItemContainerGenerator.ContainerFromIndex(i) as AccordionItem;
                if (item != null)
                {
                    item.IsLocked = (item.IsSelected && IsMinimumOneSelected && SelectedIndices.Count == 1);
                }
            }
        }

        /// <summary>
        /// Raises the SelectedItemChanged event when the SelectedItem 
        /// property value changes.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        protected virtual void OnSelectedItemChanged(SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler handler = SelectionChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raise the SelectedItemsCollectionChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> 
        /// instance containing the event data.</param>
        /// <remarks>This event is raised after the changes to the collection 
        /// have been processed.</remarks>
        private void RaiseOnSelectedItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handler = SelectedItemsChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
        #endregion Selection handling

        #region Layout
        /// <summary>
        /// Called when the size of the Accordion changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private void OnAccordionSizeChanged(object sender, SizeChangedEventArgs e)
        {
            IsResizing = true;
            LayoutChildren();
            IsResizing = false;
        }

        /// <summary>
        /// Called when size of a Header on the item changes.
        /// </summary>
        /// <param name="item">The item whose Header changed.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "item", Justification = "Passing the AccordionItem leads to a better API if we wish to change modifier to protected in the future.")]
        internal void OnHeaderSizeChange(AccordionItem item)
        {
            LayoutChildren();
        }

        /// <summary>
        /// Allows an AccordionItem to signal the need for a visual action 
        /// (resize, collapse, expand).
        /// </summary>
        /// <param name="item">The AccordionItem that signals for a schedule.</param>
        /// <param name="action">The action it is scheduling for.</param>
        /// <returns>True if the item is allowed to proceed without scheduling, 
        /// false if the item needs to wait for a signal to execute the action.</returns>
        internal virtual bool ScheduleAction(AccordionItem item, AccordionAction action)
        {
            if (SelectionSequence == SelectionSequence.CollapseBeforeExpand)
            {
                lock (this)
                {
                    if (!_scheduledActions.Contains(item))
                    {
                        _scheduledActions.Add(item);
                    }
                }
                if (_currentActioningItem == null)
                {
                    Dispatcher.BeginInvoke(StartNextAction);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Signals the finish of an action by an item.
        /// </summary>
        /// <param name="item">The AccordionItem that finishes an action.</param>
        /// <remarks>An AccordionItem should always signal a finish, for this call
        /// will start the next scheduled action.</remarks>
        internal virtual void OnActionFinish(AccordionItem item)
        {
            if (SelectionSequence == SelectionSequence.CollapseBeforeExpand)
            {
                lock (this)
                {
                    if (!_currentActioningItem.Equals(item))
                    {
                        throw new InvalidOperationException(Properties.Resources.Accordion_OnActionFinish_InvalidFinish);
                    }
                    _currentActioningItem = null;

                    StartNextAction();
                }
            }
        }

        /// <summary>
        /// Starts the next action in the list, in a particular order.
        /// </summary>
        /// <remarks>An AccordionItem is should always signal that it is 
        /// finished with an action.</remarks>
        private void StartNextAction()
        {
            if (_currentActioningItem != null)
            {
                return;
            }

            // First do collapses, then resizes and finally expands.
            AccordionItem next = _scheduledActions.FirstOrDefault(item => item.ScheduledAction == AccordionAction.Collapse);
            if (next == null)
            {
                next = _scheduledActions.FirstOrDefault(item => item.ScheduledAction == AccordionAction.Resize);
            }
            if (next == null)
            {
                next = _scheduledActions.FirstOrDefault(item => item.ScheduledAction == AccordionAction.Expand);
            }
            if (next != null)
            {
                _currentActioningItem = next;
                _scheduledActions.Remove(next);
                next.StartAction();
            }
        }

        /// <summary>
        /// Determines and sets the height of the accordion items.
        /// </summary>
        private void LayoutChildren()
        {
            ScrollViewer root = ItemsControlHelper.ScrollHost;
            Size targetSize = new Size(double.NaN, double.NaN);
            if (root != null && ItemsControlHelper.ItemsHost != null)
            {
                if (IsShouldFillWidth)
                {
                    // selected items should fill the remaining width of the container.
                    targetSize.Width = Math.Max(0, root.ViewportWidth - ItemsControlHelper.ItemsHost.ActualWidth);

                    // calculate space currently occupied by items. This space will be redistributed.
                    foreach (object item in Items)
                    {
                        AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                        if (accordionItem != null)
                        {
                            targetSize.Width += accordionItem.RelevantContentSize.Width;
                        }
                    }

                    // offset for the real difference in viewportheight and actualheight. This happens when accordion
                    // was made smaller.
                    double smaller = root.ViewportWidth - ItemsControlHelper.ItemsHost.ActualWidth;

                    if (smaller < 0)
                    {
                        targetSize.Width = Math.Max(0, targetSize.Width + smaller);
                    }

                    // calculated the targetsize for all selected items. Because of rounding issues, the
                    // actual space taken sometimes exceeds the appropriate amount by a fraction. 
                    if (targetSize.Width > 1)
                    {
                        targetSize.Width -= 1;
                    }

                    // possibly we are bigger than we would want, the items
                    // are overflowing. Always try to fit in current viewport.
                    if (root.ExtentWidth > root.ViewportWidth)
                    {
                        targetSize.Width = Math.Max(0, targetSize.Width - (root.ExtentWidth - root.ViewportWidth));
                    }

                    // calculate targetsize per selected item. This is redistribution.
                    targetSize.Width = SelectedItems.Count > 0 ? targetSize.Width / SelectedItems.Count : targetSize.Width;
                }
                else if (IsShouldFillHeight)
                {
                    // selected items should fill the remaining width of the container.
                    targetSize.Height = Math.Max(0, root.ViewportHeight - ItemsControlHelper.ItemsHost.ActualHeight);

                    // calculate space currently occupied by items. This space will be redistributed.
                    foreach (object item in Items)
                    {
                        AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                        if (accordionItem != null)
                        {
                            targetSize.Height += accordionItem.RelevantContentSize.Height;
                        }
                    }

                    // offset for the real difference in viewportheight and actualheight. This happens when accordion
                    // was made smaller.
                    double smaller = root.ViewportHeight - ItemsControlHelper.ItemsHost.ActualHeight;

                    if (smaller < 0)
                    {
                        targetSize.Height = Math.Max(0, targetSize.Height + smaller);
                    }

                    // calculated the targetsize for all selected items. Because of rounding issues, the
                    // actual space taken sometimes exceeds the appropriate amount by a fraction. 
                    if (targetSize.Height > 1)
                    {
                        targetSize.Height -= 1;
                    }

                    // calculate targetsize per selected item. This is redistribution.
                    targetSize.Height = SelectedItems.Count > 0 ? targetSize.Height / SelectedItems.Count : targetSize.Height;
                }

                // set that targetsize
                foreach (object item in Items)
                {
                    AccordionItem accordionItem = ItemContainerGenerator.ContainerFromItem(item) as AccordionItem;
                    if (accordionItem != null)
                    {
                        // the calculated target size is calculated for the selected items.
                        if (accordionItem.IsSelected)
                        {
                            accordionItem.ContentTargetSize = targetSize;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the accordion fills width.
        /// </summary>
        private bool IsShouldFillWidth
        {
            get
            {
                return (ExpandDirection == ExpandDirection.Left || ExpandDirection == ExpandDirection.Right) &&
                       (!Double.IsNaN(Width) || HorizontalAlignment == HorizontalAlignment.Stretch);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the accordion fills height.
        /// </summary>
        private bool IsShouldFillHeight
        {
            get
            {
                return (ExpandDirection == ExpandDirection.Down || ExpandDirection == ExpandDirection.Up) &&
                       (!Double.IsNaN(Height) || VerticalAlignment == VerticalAlignment.Stretch);
            }
        }

        /// <summary>
        /// Sets the orientation of the panel.
        /// </summary>
        private void SetPanelOrientation()
        {
            StackPanel panel = ItemsControlHelper.ItemsHost as StackPanel;
            if (panel != null)
            {
                switch (ExpandDirection)
                {
                    case ExpandDirection.Down:
                    case ExpandDirection.Up:
                        panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                        panel.VerticalAlignment = ExpandDirection == ExpandDirection.Down ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                        panel.Orientation = Orientation.Vertical;
                        break;
                    case ExpandDirection.Left:
                    case ExpandDirection.Right:
                        panel.VerticalAlignment = VerticalAlignment.Stretch;
                        panel.HorizontalAlignment = ExpandDirection == ExpandDirection.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                        panel.Orientation = Orientation.Horizontal;
                        break;
                }
            }
        }
        #endregion Layout

        #region Visual state management
        /// <summary>
        /// Provides handling for the GotFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowGotFocus(e))
            {
                Interaction.OnGotFocusBase();
                base.OnGotFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the LostFocus event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Interaction.AllowLostFocus(e))
            {
                Interaction.OnLostFocusBase();
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseEnter event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (Interaction.AllowMouseEnter(e))
            {
                Interaction.OnMouseEnterBase();
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeave event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Interaction.AllowMouseLeave(e))
            {
                Interaction.OnMouseLeaveBase();
                base.OnMouseLeave(e);
            }
        }

        /// <summary>
        /// Provides handling for the MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Interaction.AllowMouseLeftButtonDown(e))
            {
                Interaction.OnMouseLeftButtonDownBase();
                base.OnMouseLeftButtonDown(e);
            }
        }

        /// <summary>
        /// Called before the MouseLeftButtonUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Interaction.AllowMouseLeftButtonUp(e))
            {
                Interaction.OnMouseLeftButtonUpBase();
                base.OnMouseLeftButtonUp(e);
            }
        }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the current visual state of the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }
        #endregion Visual state management
    }
}
