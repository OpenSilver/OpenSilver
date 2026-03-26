
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

using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal.Xaml.Context;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that can be used to present a collection of items.
    /// </summary>
    [ContentProperty(nameof(Items))]
    public partial class ItemsControl : Control, IGeneratorHost
    {
        #region Data

        private static readonly UncommonField<DisplayMemberPathTemplate> DisplayMemberPathTemplateField = new();

        // Note: this maps an item (for example a string) to the element
        // that is added to the visual tree (such a datatemplate) or to 
        // the native DOM element in case of native combo box for example.
        private ItemContainerGenerator _itemContainerGenerator;

        private ItemCollection _items;

        #endregion Data

        #region Contructor

        static ItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemsControl), new PropertyMetadata(typeof(ItemsControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsControl"/> class.
        /// </summary>
        public ItemsControl() { }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Items is the collection of data that is used to generate the content
        /// of this control.
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                if (this._items == null)
                {
                    this.CreateItemCollectionAndGenerator();
                }
                return this._items;
            }
        }

        /// <summary>
        /// Gets the ItemContainerGenerator associated with this ItemsControl.
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get
            {
                if (this._itemContainerGenerator == null)
                {
                    this.CreateItemCollectionAndGenerator();
                }
                return _itemContainerGenerator;
            }
        }

        private void CreateItemCollectionAndGenerator()
        {
            _items = new ItemCollection(this);

            // ItemInfos must get adjusted before the generator's change handler is called,
            // so that any new ItemInfos arising from the generator don't get adjusted by mistake
            _items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemCollectionChanged1);

            // the generator must attach its collection change handler before
            // the control itself, so that the generator is up-to-date by the
            // time the control tries to use it
            _itemContainerGenerator = new ItemContainerGenerator(this);

            _itemContainerGenerator.ChangeAlternationCount();

            _items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemCollectionChanged2);
        }

        #endregion Public Properties

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the template that defines the panel that controls the layout of items.
        /// </summary>
        /// <returns>
        /// An <see cref="ItemsPanelTemplate"/> that defines the panel to use for the layout of the items. The default value 
        /// for the <see cref="ItemsControl"/> is an <see cref="ItemsPanelTemplate"/> that specifies a <see cref="StackPanel"/>.
        /// </returns>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValueInternal(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemsPanel"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register(
                nameof(ItemsPanel),
                typeof(ItemsPanelTemplate),
                typeof(ItemsControl),
                new PropertyMetadata(GetDefaultItemsPanel(), OnItemsPanelChanged));

        private static ItemsPanelTemplate GetDefaultItemsPanel()
        {
            var template = new ItemsPanelTemplate
            {
                Template = new CompiledTemplateContent(
                    new XamlContext(),
                    static (owner, context) =>
                    {
                        var panel = new StackPanel();
                        panel.SetTemplatedParent(context.TemplateOwnerReference);
                        return panel;
                    }),
            };

            template.Seal();

            return template;
        }

        private static void OnItemsPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).OnItemsPanelChanged((ItemsPanelTemplate)e.OldValue, (ItemsPanelTemplate)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="ItemsPanel"/> property changes.
        /// </summary>
        /// <param name="oldItemsPanel">
        /// Old value of the <see cref="ItemsPanel"/> property.
        /// </param>
        /// <param name="newItemsPanel">
        /// New value of the <see cref="ItemsPanel"/> property.
        /// </param>
        protected virtual void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel)
        {
            ItemContainerGenerator.OnPanelChanged();
        }

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see cref="ItemsControl"/>.
        /// </summary>
        /// <returns>
        /// A collection that is used to generate the content of the <see cref="ItemsControl"/>. The default is null.
        /// </returns>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValueInternal(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(ItemsControl),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl ic = (ItemsControl)d;
            IEnumerable oldValue = (IEnumerable)e.OldValue;
            IEnumerable newValue = (IEnumerable)e.NewValue;

            if (e.NewValue != null)
            {
                // ItemsSource is non-null.  Go to ItemsSource mode
                ic.Items.SetItemsSource(newValue);
            }
            else
            {
                // ItemsSource is explicitly null.  Return to normal mode.
                ic.Items.ClearItemsSource();
            }

            ic.OnItemsSourceChanged(oldValue, newValue);
        }

        /// <summary>
        /// Called when the <see cref="ItemsSource"/> property changes.
        /// </summary>
        /// <param name="oldValue">
        /// Old value of the <see cref="ItemsSource"/> property.
        /// </param>
        /// <param name="newValue">
        /// New value of the <see cref="ItemsSource"/> property.
        /// </param>
        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display each item.
        /// </summary>
        /// <returns>
        /// A <see cref="DataTemplate"/> that specifies the visualization of the data objects. The default is null.
        /// </returns>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValueInternal(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(ItemsControl),
                new PropertyMetadata(null, OnItemTemplateChanged));

        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).OnItemTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="ItemTemplate"/> property changes.
        /// </summary>
        /// <param name="oldItemTemplate">
        /// The old <see cref="ItemTemplate"/> property value.
        /// </param>
        /// <param name="newItemTemplate">
        /// The new <see cref="ItemTemplate"/> property value.
        /// </param>
        protected virtual void OnItemTemplateChanged(DataTemplate oldItemTemplate, DataTemplate newItemTemplate)
        {
            CheckTemplateSource();

            _itemContainerGenerator?.Refresh();
        }

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(
                nameof(ItemTemplateSelector),
                typeof(DataTemplateSelector),
                typeof(ItemsControl),
                new PropertyMetadata(null, OnItemTemplateSelectorChanged));

        /// <summary>
        /// Gets or sets the custom logic for choosing a template used to display each item.
        /// </summary>
        /// <returns>
        /// A custom <see cref="DataTemplateSelector"/> object that provides logic and returns a <see cref="DataTemplate"/>.
        /// The default is null.
        /// </returns>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValueInternal(ItemTemplateSelectorProperty, value); }
        }

        private static void OnItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).OnItemTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="ItemTemplateSelector"/> property changes.
        /// </summary>
        /// <param name="oldItemTemplateSelector">
        /// Old value of the <see cref="ItemTemplateSelector"/> property.
        /// </param>
        /// <param name="newItemTemplateSelector">
        /// New value of the <see cref="ItemTemplateSelector"/> property.
        /// </param>
        protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldItemTemplateSelector, DataTemplateSelector newItemTemplateSelector)
        {
            CheckTemplateSource();

            if (_itemContainerGenerator is not null && ItemTemplate is null)
            {
                _itemContainerGenerator.Refresh();
            }
        }

        private void CheckTemplateSource()
        {
            if (!string.IsNullOrEmpty(DisplayMemberPath))
            {
                if (ItemTemplateSelector is not null)
                {
                    throw new InvalidOperationException(Strings.ItemTemplateSelectorBreaksDisplayMemberPath);
                }

                if (ItemTemplate is not null)
                {
                    throw new InvalidOperationException(Strings.DisplayMemberPathAndItemTemplateDefined);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name or path of the property that is displayed for each data item.
        /// </summary>
        /// <returns>
        /// The name or path of the property that is displayed for each the data item in the control. The default is an empty string ("").
        /// </returns>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValueInternal(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DisplayMemberPath"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                nameof(DisplayMemberPath),
                typeof(string),
                typeof(ItemsControl),
                new PropertyMetadata(string.Empty, OnDisplayMemberPathChanged));

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)d;
            itemsControl.OnDisplayMemberPathChanged((string)e.OldValue, (string)e.NewValue);
            itemsControl.UpdateDisplayMemberTemplate();
        }

        /// <summary>
        /// Invoked when the <see cref="DisplayMemberPath"/> property changes.
        /// </summary>
        /// <param name="oldDisplayMemberPath">
        /// The old value of the <see cref="DisplayMemberPath"/> property.
        /// </param>
        /// <param name="newDisplayMemberPath">
        /// New value of the <see cref="DisplayMemberPath"/> property.
        /// </param>
        protected virtual void OnDisplayMemberPathChanged(string oldDisplayMemberPath, string newDisplayMemberPath)
        {
        }

        private void UpdateDisplayMemberTemplate()
        {
            string displayMemberPath = DisplayMemberPath;

            if (!string.IsNullOrEmpty(displayMemberPath))
            {
                CheckTemplateSource();

                DisplayMemberPathTemplateField.SetValue(this, new DisplayMemberPathTemplate(displayMemberPath));
            }
            else
            {
                DisplayMemberPathTemplateField.ClearValue(this);
            }

            _itemContainerGenerator?.Refresh();
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> that is applied to the container element generated for each item.
        /// </summary>
        /// <returns>
        /// The <see cref="Style"/> that is applied to the container element generated for each item. The default is null.
        /// </returns>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValueInternal(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(
                nameof(ItemContainerStyle),
                typeof(Style),
                typeof(ItemsControl),
                new PropertyMetadata(null, OnItemContainerStyleChanged));

        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).OnItemContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="ItemContainerStyle"/> property changes.
        /// </summary>
        /// <param name="oldItemContainerStyle">
        /// Old value of the <see cref="ItemContainerStyle"/> property.
        /// </param>
        /// <param name="newItemContainerStyle">
        /// New value of the <see cref="ItemContainerStyle"/> property.
        /// </param>
        protected virtual void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            _itemContainerGenerator?.Refresh();
        }

        /// <summary>
        /// Identifies the <see cref="ItemContainerStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register(
                nameof(ItemContainerStyleSelector),
                typeof(StyleSelector),
                typeof(ItemsControl),
                new PropertyMetadata(null, OnItemContainerStyleSelectorChanged));

        /// <summary>
        /// Gets or sets custom style-selection logic for a style that can be applied to each generated container element.
        /// </summary>
        /// <returns>
        /// A <see cref="StyleSelector"/> object that contains logic that chooses the style to use as the <see cref="ItemContainerStyle"/>.
        /// The default is null.
        /// </returns>
        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValueInternal(ItemContainerStyleSelectorProperty, value); }
        }

        private static void OnItemContainerStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).OnItemContainerStyleSelectorChanged((StyleSelector)e.OldValue, (StyleSelector)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="ItemContainerStyleSelector"/> property changes.
        /// </summary>
        /// <param name="oldItemContainerStyleSelector">
        /// Old value of the <see cref="ItemContainerStyleSelector"/> property.
        /// </param>
        /// <param name="newItemContainerStyleSelector">
        /// New value of the <see cref="ItemContainerStyleSelector"/> property.
        /// </param>
        protected virtual void OnItemContainerStyleSelectorChanged(StyleSelector oldItemContainerStyleSelector, StyleSelector newItemContainerStyleSelector)
        {
            if (_itemContainerGenerator is not null && ItemContainerStyle is null)
            {
                _itemContainerGenerator.Refresh();
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsTextSearchEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTextSearchEnabledProperty =
            DependencyProperty.Register(
                nameof(IsTextSearchEnabled),
                typeof(bool),
                typeof(ItemsControl),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether <see cref="TextSearch"/> is enabled on the <see cref="ItemsControl"/> instance.
        /// </summary>
        /// <returns>
        /// true if <see cref="TextSearch"/> is enabled; otherwise, true. The default is false.
        /// </returns>
        public bool IsTextSearchEnabled
        {
            get { return (bool)GetValue(IsTextSearchEnabledProperty); }
            set { SetValueInternal(IsTextSearchEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsTextSearchCaseSensitive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTextSearchCaseSensitiveProperty =
            DependencyProperty.Register(
                nameof(IsTextSearchCaseSensitive),
                typeof(bool),
                typeof(ItemsControl),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether case is a condition when searching for items.
        /// </summary>
        /// <returns>
        /// true if text searches are case-sensitive; otherwise, false.
        /// </returns>
        public bool IsTextSearchCaseSensitive
        {
            get { return (bool)GetValue(IsTextSearchCaseSensitiveProperty); }
            set { SetValueInternal(IsTextSearchCaseSensitiveProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AlternationCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternationCountProperty =
            DependencyProperty.Register(
                nameof(AlternationCount),
                typeof(int),
                typeof(ItemsControl),
                new PropertyMetadata(0, OnAlternationCountChanged));

        /// <summary>
        /// Gets or sets the number of alternating item containers in the <see cref="ItemsControl"/>, which enables 
        /// alternating containers to have a unique appearance.
        /// </summary>
        /// <returns>
        /// The number of alternating item containers in the <see cref="ItemsControl"/>.
        /// </returns>
        public int AlternationCount
        {
            get { return (int)GetValue(AlternationCountProperty); }
            set { SetValueInternal(AlternationCountProperty, value); }
        }

        private static void OnAlternationCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).OnAlternationCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="AlternationCount"/> property changes.
        /// </summary>
        /// <param name="oldAlternationCount">
        /// The old value of <see cref="AlternationCount"/>.
        /// </param>
        /// <param name="newAlternationCount">
        /// The new value of <see cref="AlternationCount"/>.
        /// </param>
        protected virtual void OnAlternationCountChanged(int oldAlternationCount, int newAlternationCount)
        {
            ItemContainerGenerator.ChangeAlternationCount();
        }

        private static readonly DependencyPropertyKey AlternationIndexPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "AlternationIndex",
                typeof(int),
                typeof(ItemsControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the AlternationIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternationIndexProperty = AlternationIndexPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the alternation index for the specified object.
        /// </summary>
        /// <param name="element">
        /// The object from which to get the alternation index.
        /// </param>
        /// <returns>
        /// The value of the alternation index.
        /// </returns>
        public static int GetAlternationIndex(DependencyObject element)
        {
            ArgumentNullException.ThrowIfNull(element);

            return (int)element.GetValue(AlternationIndexProperty);
        }

        // internal setter for AlternationIndex.  This property is not settable by
        // an app, only by internal code
        internal static void SetAlternationIndex(DependencyObject d, int value)
        {
            d.SetValue(AlternationIndexPropertyKey, value);
        }

        // internal clearer for AlternationIndex.  This property is not settable by
        // an app, only by internal code
        internal static void ClearAlternationIndex(DependencyObject d)
        {
            d.ClearValue(AlternationIndexPropertyKey);
        }

        #endregion Dependency Properties

        #region IGeneratorHost

        /// <summary>
        /// The view of the data
        /// </summary>
        IList IGeneratorHost.View
        {
            get { return Items; }
        }

        /// <summary>
        /// The AlternationCount
        /// </summary>
        int IGeneratorHost.AlternationCount
        {
            get { return AlternationCount; }
        }

        void IGeneratorHost.ClearContainerForItem(DependencyObject container, object item)
        {
            ClearContainerForItemOverride(container, item);
        }

        DependencyObject IGeneratorHost.GetContainerForItem(object item, DependencyObject recycledContainer)
        {
            DependencyObject container;

            // use the item directly, if possible
            if (IsItemItsOwnContainerOverride(item))
            {
                // Note: There was once an exception thrown here if 
                // (this.Items.IsUsingItemsSource && this.ItemTemplate != null) 
                // was true, stating that "ItemsControl.Items must not be a 
                // UIElement type when an ItemTemplate is set."
                // I checked in a WPF Project and it seems the ItemTemplate is 
                // simply ignored for items that are UIElements so I removed that 
                // exception.
                // todo: see if the exception of the note above should be there in 
                // certain cases (it was added in Commit 8eff80c0).
                container = item as DependencyObject;
            }
            else
            {
                container = recycledContainer ?? GetContainerForItemOverride();
            }

            return container;
        }

        bool IGeneratorHost.IsHostForItemContainer(DependencyObject container)
        {
            // If ItemsControlFromItemContainer can determine who owns the element,
            // use its decision.
            ItemsControl ic = ItemsControlFromItemContainer(container);
            if (ic != null)
                return (ic == this);

            // If the element is in my items view, and if it can be its own ItemContainer,
            // it's mine.  Contains may be expensive, so we avoid calling it in cases
            // where we already know the answer - namely when the element has a
            // logical parent (ItemsControlFromItemContainer handles this case).  This
            // leaves only those cases where the element belongs to my items
            // without having a logical parent (e.g. via ItemsSource) and without
            // having been generated yet. HasItem indicates if anything has been generated.

            DependencyObject parent = (container as FrameworkElement)?.Parent;
            if (parent == null)
            {
                return IsItemItsOwnContainerOverride(container) &&
                    HasItems && Items.Contains(container);
            }

            // Otherwise it's not mine
            return false;
        }

        /// <summary>
        /// Return true if the item is (or is eligible to be) its own ItemContainer
        /// </summary>
        bool IGeneratorHost.IsItemItsOwnContainer(object item)
        {
            return IsItemItsOwnContainer(item);
        }

        void IGeneratorHost.PrepareItemContainer(DependencyObject container, object item)
        {
            if (ShouldApplyItemContainerStyle(container, item))
            {
                // apply the ItemContainer style (if any)
                ApplyItemContainerStyle(container, item);
            }

            // forward ItemTemplate, et al.
            PrepareContainerForItemOverride(container, item);
        }

        #endregion IGeneratorHost

        #region Protected Methods

        [Obsolete(Helper.ObsoleteMemberMessage + "Use ItemsControl.OnItemsPanelChanged instead.", true)]
        protected virtual void UpdateItemsPanel(ItemsPanelTemplate newTemplate)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///  Called when the value of the <see cref="ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">
        /// A <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data
        /// </param>
        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Adjust ItemInfos when the Items property changes.
        /// </summary>
        internal virtual void AdjustItemInfoOverride(NotifyCollectionChangedEventArgs e)
        {
            AdjustItemInfo(e, _focusedInfo);
        }

        #endregion Protected Methods

        #region Internal Properties

        /// <summary>
        /// Gets an enumerator for the logical child objects of the <see cref="ItemsControl"/> object.
        /// </summary>
        /// <returns>
        /// An enumerator for the logical child objects of the <see cref="ItemsControl"/> object.
        /// The default is null.
        /// </returns>
        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                if (!HasItems)
                {
                    return EmptyEnumerator.Instance;
                }

                // Items in direct-mode of ItemCollection are the only model children.
                // note: the enumerator walks the ItemCollection.InnerList as-is,
                // no flattening of any content on model children level!
                return Items.LogicalChildren;
            }
        }

        internal override FrameworkTemplate TemplateInternal => base.TemplateInternal ?? DefaultTemplate;

        internal sealed override FrameworkTemplate TemplateCache
        {
            get { return base.TemplateCache; }
            set
            {
                base.TemplateCache = value;

                // This is a workaround to ensure that resources held by the current ItemsPresenter and
                // ItemsHost are are released. We put this code here because this cleanup needs to happen
                // before the previous template is cleared, and we do not have any other method or event
                // to do this.
                if (ItemsPresenter.FromPanel(ItemsHost) is ItemsPresenter ip)
                {
                    ip.DetachFromOwner();
                }
            }
        }

        internal Panel ItemsHost { get; set; }

        internal static readonly DependencyPropertyKey HasItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasItems),
                typeof(bool),
                typeof(ItemsControl),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="HasItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="ItemsControl"/> contains items.
        /// </summary>
        /// <returns>
        /// true if the items count is greater than 0; otherwise, false. The default is false.
        /// </returns>
        public bool HasItems => (bool)GetValue(HasItemsProperty);

        private static ControlTemplate DefaultTemplate { get; } =
            new ControlTemplate
            {
                TargetType = typeof(ItemsControl),
                Template = new CompiledTemplateContent(
                    new XamlContext(),
                    static (owner, context) =>
                    {
                        var presenter = new ItemsPresenter();
                        presenter.SetTemplatedParent(context.TemplateOwnerReference);
                        return presenter;
                    }
                ),
            };

        #endregion Internal Properties

        #region Internal Methods

        // adjust ItemInfos after a generator status change
        internal void AdjustItemInfoAfterGeneratorChange(ItemInfo info)
        {
            if (info != null)
            {
                ItemInfo[] a = [info];
                AdjustItemInfosAfterGeneratorChange(a, claimUniqueContainer: false);
            }
        }

        // adjust ItemInfos after a generator status change
        internal void AdjustItemInfosAfterGeneratorChange(IEnumerable<ItemInfo> list, bool claimUniqueContainer)
        {
            // detect discarded containers and mark the ItemInfo accordingly
            // (also see if there are infos awaiting containers)
            bool resolvePendingContainers = false;
            foreach (ItemInfo info in list)
            {
                DependencyObject container = info.Container;
                if (container == null)
                {
                    resolvePendingContainers = true;
                }
                else if (info.IsRemoved || !ItemsControl.EqualsEx(info.Item,
                            container.ReadLocalValue(ItemContainerGenerator.ItemForItemContainerProperty)))
                {
                    info.Container = null;
                    resolvePendingContainers = true;
                }
            }

            // if any of the ItemInfos correspond to containers
            // that are now realized, record the container in the ItemInfo
            if (resolvePendingContainers)
            {
                // first find containers that are already claimed by the list
                List<DependencyObject> claimedContainers = new List<DependencyObject>();
                if (claimUniqueContainer)
                {
                    foreach (ItemInfo info in list)
                    {
                        DependencyObject container = info.Container;
                        if (container != null)
                        {
                            claimedContainers.Add(container);
                        }
                    }
                }

                // now try to match the pending items with an unclaimed container
                foreach (ItemInfo info in list)
                {
                    DependencyObject container = info.Container;
                    if (container == null)
                    {
                        int index = info.Index;
                        if (index >= 0)
                        {
                            // if we know the index, see if the container exists
                            container = ItemContainerGenerator.ContainerFromIndex(index);
                        }
                        else
                        {
                            // otherwise see if an unclaimed container matches the item
                            object item = info.Item;
                            ItemContainerGenerator.FindItem(
                                static (state, o, d) => EqualsEx(o, state.item) && !state.claimedContainers.Contains(d),
                                (item, claimedContainers),
                                out container, out index);
                        }

                        if (container != null)
                        {
                            // update ItemInfo and claim the container
                            info.Container = container;
                            info.Index = index;
                            if (claimUniqueContainer)
                            {
                                claimedContainers.Add(container);
                            }
                        }
                    }
                }
            }
        }

        // correct the indices in the given ItemInfo, in response to a collection change event
        internal void AdjustItemInfo(NotifyCollectionChangedEventArgs e, ItemInfo info)
        {
            if (info != null)
            {
                ItemInfo[] a = [info];
                AdjustItemInfos(e, a);
            }
        }

        // correct the indices in the given ItemInfos, in response to a collection change event
        internal void AdjustItemInfos(NotifyCollectionChangedEventArgs e, IEnumerable<ItemInfo> list)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // items at NewStartingIndex and above have moved up 1
                    foreach (ItemInfo info in list)
                    {
                        int index = info.Index;
                        if (index >= e.NewStartingIndex)
                        {
                            info.Index = index + 1;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // items at OldStartingIndex and above have moved down 1
                    foreach (ItemInfo info in list)
                    {
                        int index = info.Index;
                        if (index > e.OldStartingIndex)
                        {
                            info.Index = index - 1;
                        }
                        else if (index == e.OldStartingIndex)
                        {
                            info.Index = -1;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    // items between New and Old have moved.  The direction and
                    // exact endpoints depends on whether New comes before Old.
                    int left, right, delta;
                    if (e.OldStartingIndex < e.NewStartingIndex)
                    {
                        left = e.OldStartingIndex + 1;
                        right = e.NewStartingIndex;
                        delta = -1;
                    }
                    else
                    {
                        left = e.NewStartingIndex;
                        right = e.OldStartingIndex - 1;
                        delta = 1;
                    }

                    foreach (ItemInfo info in list)
                    {
                        int index = info.Index;
                        if (index == e.OldStartingIndex)
                        {
                            info.Index = e.NewStartingIndex;
                        }
                        else if (left <= index && index <= right)
                        {
                            info.Index = index + delta;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // nothing to do
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // the indices and containers are no longer valid
                    foreach (ItemInfo info in list)
                    {
                        info.Index = -1;
                        info.Container = null;
                    }
                    break;
            }
        }

        internal object GetItemOrContainerFromContainer(DependencyObject container)
        {
            object item = ItemContainerGenerator.ItemFromContainer(container);

            if (item == DependencyProperty.UnsetValue
                && ItemsControlFromItemContainer(container) == this
                && ((IGeneratorHost)this).IsItemItsOwnContainer(container))
            {
                item = container;
            }

            return item;
        }

        // A version of Object.Equals with paranoia for mismatched types, to avoid problems
        // with classes that implement Object.Equals poorly, as in Dev11 439664, 746174, DDVSO 602650
        internal static bool EqualsEx(object o1, object o2)
        {
            try
            {
                return Equals(o1, o2);
            }
            catch (InvalidCastException)
            {
                // A common programming error: the type of o1 overrides Equals(object o2)
                // but mistakenly assumes that o2 has the same type as o1:
                //     MyType x = (MyType)o2;
                // This throws InvalidCastException when o2 is a sentinel object,
                // e.g. UnsetValue, DisconnectedItem, NewItemPlaceholder, etc.
                // Rather than crash, just return false - the objects are clearly unequal.
                return false;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether to apply the style from the <see cref="ItemContainerStyle"/> or
        /// <see cref="ItemContainerStyleSelector"/> property to the container element of the specified item.
        /// </summary>
        /// <param name="container">
        /// The container element.
        /// </param>
        /// <param name="item">
        /// The item of interest.
        /// </param>
        /// <returns>
        /// Always true for the base implementation.
        /// </returns>
        protected virtual bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
        {
            return true;
        }

        private void ApplyItemContainerStyle(DependencyObject container, object item)
        {
            if (container is not FrameworkElement feContainer)
            {
                return;
            }

            // don't overwrite a locally-defined style
            if (!feContainer.IsStyleSetFromGenerator &&
                feContainer.ReadLocalValue(StyleProperty) != DependencyProperty.UnsetValue)
            {
                return;
            }

            // Control's ItemContainerStyle has first stab
            Style style = ItemContainerStyle;

            // no ItemContainerStyle set, try ItemContainerStyleSelector
            if (style is null)
            {
                if (ItemContainerStyleSelector is StyleSelector itemContainerStyleSelector)
                {
                    style = itemContainerStyleSelector.SelectStyle(item, container);
                }
            }

            // apply the style, if found
            if (style is not null)
            {
                // verify style is appropriate before applying it
                if (!style.TargetType.IsInstanceOfType(container))
                {
                    throw new InvalidOperationException(string.Format(Strings.StyleForWrongType, style.TargetType.Name, container.GetType().Name));
                }

                feContainer.Style = style;
                feContainer.IsStyleSetFromGenerator = true;
            }
            else
            {
                // if Style was formerly set from ItemContainerStyle, clear it
                feContainer.IsStyleSetFromGenerator = false;
                feContainer.ClearValue(StyleProperty);
            }
        }

        /// <summary>
        /// Return true if the item is (or should be) its own item container
        /// </summary>
        internal bool IsItemItsOwnContainer(object item)
        {
            return IsItemItsOwnContainerOverride(item);
        }

        private void OnItemCollectionChanged1(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.AdjustItemInfoOverride(e);
        }

        private void OnItemCollectionChanged2(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetValue(HasItemsPropertyKey, _items.Count > 0);

            // If the focused item is removed, drop our reference to it.
            if (_focusedInfo is not null && _focusedInfo.Index < 0)
            {
                _focusedInfo = null;
            }

            OnItemsChanged(e);
        }

        #endregion Internal Methods

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected virtual void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // Note: This is the WPF implementation.
            // Silverlight does not clear containers on the ItemsControl level.
            //switch (element)
            //{
            //    case ContentControl cc:
            //        cc.ClearContentControl(item);
            //        break;

            //    case ContentPresenter cp:
            //        cp.ClearContentPresenter(item);
            //        break;
            //}
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected virtual DependencyObject GetContainerForItemOverride()
        {
            return new ContentPresenter();
        }

        /// <summary>
        /// Determines whether the specified item is (or is eligible to be) its 
        /// own container.
        /// </summary>
        /// <param name="item">
        /// The item to check.
        /// </param>
        /// <returns>
        /// True if the item is (or is eligible to be) its own container; otherwise,
        /// false.
        /// </returns>
        protected virtual bool IsItemItsOwnContainerOverride(object item)
        {
            return item is UIElement;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">
        /// The element that's used to display the specified item.
        /// </param>
        /// <param name="item">
        /// The item to display.
        /// </param>
        protected virtual void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            DataTemplate itemTemplate = ItemTemplate;
            DataTemplateSelector itemTemplateSelector = ItemTemplateSelector;

            if (itemTemplate is null && itemTemplateSelector is null)
            {
                itemTemplate = DisplayMemberPathTemplateField.GetValue(this);
            }

            switch (element)
            {
                case ContentControl cc:
                    cc.PrepareContentControl(item, itemTemplate, itemTemplateSelector);
                    break;

                case ContentPresenter cp:
                    cp.PrepareContentPresenter(item, itemTemplate, itemTemplateSelector);
                    break;
            }
        }

        /// <summary>
        /// Returns the <see cref="ItemsControl"/> that the specified element hosts items for.
        /// </summary>
        /// <param name="element">
        /// The host element.
        /// </param>
        /// <returns>
        /// The <see cref="ItemsControl"/> that the specified element hosts items for, or null.
        /// </returns>
        public static ItemsControl GetItemsOwner(DependencyObject element)
        {
            ItemsControl container = null;

            if (element is Panel panel && panel.IsItemsHost)
            {
                // see if element was generated for an ItemsPresenter
                ItemsPresenter ip = ItemsPresenter.FromPanel(panel);

                if (ip != null)
                {
                    // if so use the element whose style begat the ItemsPresenter
                    container = ip.Owner;
                }
            }

            return container;
        }

        /// <summary>
        /// Returns the <see cref="ItemsControl"/> that owns the specified container element.
        /// </summary>
        /// <param name="container">
        /// The container element to return the <see cref="ItemsControl"/> for.
        /// </param>
        /// <returns>
        /// The <see cref="ItemsControl"/> that owns the specified container element.
        /// </returns>
        public static ItemsControl ItemsControlFromItemContainer(DependencyObject container)
        {
            if (container is not UIElement ui)
            {
                return null;
            }

            // ui appeared in items collection
            if (LogicalTreeHelper.GetParent(ui) is ItemsControl ic)
            {
                // this is the right ItemsControl as long as the item
                // is (or is eligible to be) its own container
                IGeneratorHost host = ic;
                if (host.IsItemItsOwnContainer(ui))
                    return ic;
                else
                    return null;
            }

            ui = VisualTreeHelper.GetParent(ui) as UIElement;

            return GetItemsOwner(ui);
        }

        /// <inheritdoc />
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);

            if (!string.IsNullOrEmpty(e.Text) && IsTextSearchEnabled)
            {
                if (TextSearch.EnsureInstance(this) is TextSearch instance)
                {
                    instance.DoSearch(e.Text);                    
                }
            }

            e.Handled = true;
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (IsTextSearchEnabled)
            {
                // If the pressed the backspace key, delete the last character
                // in the TextSearch current prefix.
                if (e.Key == Key.Back)
                {
                    if (TextSearch.EnsureInstance(this) is TextSearch instance)
                    {
                        instance.DeleteLastCharacter();
                    }
                }
            }
        }

        internal static DataTemplate GetDisplayMemberPathTemplate(ItemsControl itemsControl) => DisplayMemberPathTemplateField.GetValue(itemsControl);

        private sealed class DisplayMemberPathTemplate : DataTemplate
        {
            private readonly Binding _binding;

            public DisplayMemberPathTemplate(string displayMemberPath)
            {
                _binding = new Binding(displayMemberPath);
            }

            internal override bool BuildVisualTree(IFrameworkElement container)
            {
                Debug.Assert(container is ContentControl || container is ContentPresenter);

                var feContainer = (FrameworkElement)container;

                var textBlock = new TextBlock();
                textBlock.SetTemplatedParent(new(feContainer));
                textBlock.SetBinding(TextBlock.TextProperty, _binding);

                feContainer.TemplateChild = textBlock;

                return true;
            }
        }
    }
}