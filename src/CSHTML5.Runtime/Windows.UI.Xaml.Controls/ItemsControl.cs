

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

using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.Diagnostics;
using System.Linq;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control that can be used to present a collection of items.
    /// </summary>
    [ContentProperty("Items")]
    public partial class ItemsControl : Control
    {
        #region Data

        // Note: this maps an item (for example a string) to the element
        // that is added to the visual tree (such a datatemplate) or to 
        // the native DOM element in case of native combo box for example.
        private ItemContainerGenerator _itemContainerGenerator = new ItemContainerGenerator();

        // ItemsPresenter retrieve from this control's Template
        // in which items will be rendered.
        internal ItemsPresenter _itemsPresenter;

        private ItemCollection _items;

        #endregion Data

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the ItemsControl class.
        /// </summary>
        public ItemsControl()
        {
            this.DefaultStyleKey = typeof(ItemsControl);
        }

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
                    this._items = new ItemCollection();
                    this._items.CollectionChanged += this.OnItemCollectionChanged;
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
                return _itemContainerGenerator;
            }
        }

        #endregion Public Properties

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the template that defines the panel that controls the layout
        /// of items.
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsPanel dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel",
                                        typeof(ItemsPanelTemplate),
                                        typeof(ItemsControl),
                                        new PropertyMetadata(GetDefaultItemsPanel(), OnItemsPanelChanged));

        private static ItemsPanelTemplate GetDefaultItemsPanel()
        {
            ItemsPanelTemplate template = new ItemsPanelTemplate()
            {
                _methodToInstantiateFrameworkTemplate = (Control templateOwner) =>
                {
                    return new TemplateInstance()
                    {
                        // Default items panel. 
                        // Note: the parameter templateOwner is made necessary 
                        // for the ControlTemplates but can be kept null for DataTemplates.
                        TemplateContent = new StackPanel()
                    };
                }
            };
            template.Seal();

            // Note: We seal the template in order to avoid letting the user modify the 
            // default template itself since it is the same instance that is used as 
            // the default value for all ItemsControls.
            // This would bring issues such as a user modifying the default template 
            // for one element then modifying it again for another one and both would 
            // have the last one's template.
            return template;
        }

        private static void OnItemsPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).UpdateItemsPanel((ItemsPanelTemplate)e.NewValue);
        }

        /// <summary>
        /// Gets or sets an object source used to generate the content of the ItemsControl.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
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
        /// Gets or sets the DataTemplate used to display each item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate",
                                        typeof(DataTemplate),
                                        typeof(ItemsControl),
                                        new PropertyMetadata(null, OnItemTemplateChanged));

        /// <summary>
        /// Invoked when the value of the ItemTemplate property changes.
        /// </summary>
        private static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)d;
            if (itemsControl.RenderedItemsPanel != null)
            {
                itemsControl.UpdateChildrenInVisualTree(itemsControl.Items, itemsControl.Items, forceUpdateAllChildren: true);
            }
        }

        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the visual
        /// representation of the object.
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayMemberPath dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath",
                                        typeof(string),
                                        typeof(ItemsControl),
                                        new PropertyMetadata(string.Empty, OnDisplayMemberPathChanged));

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)d;
            itemsControl.UpdateChildrenInVisualTree(itemsControl.Items, itemsControl.Items, true);
        }

        /// <summary>
        /// Gets or sets the style that is used when rendering the item containers.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle",
                                        typeof(Style),
                                        typeof(ItemsControl),
                                        new PropertyMetadata(null, OnItemContainerStyleChanged));

        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)d;
            itemsControl.UpdateChildrenInVisualTree(itemsControl.Items, itemsControl.Items, true);
        }

        #endregion Dependency Properties

        #region Public Methods

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Attempt to find the <ItemsPresenter /> in the ControlTemplate and, 
            // if it was found, use it as the place where all the items will be added
            ItemsPresenter itemsPresenter = this.GetTemplateChild("ItemsHost") as ItemsPresenter;
            if (itemsPresenter == null)
            {
                // Note: In Silverlight and WPF, the ItemsPresenter does not need to be
                // named ItemsHost. However, the method used below does not work properly
                // in some cases. For instance, if the ItemsPresenter is the content of a
                // Popup, it will not be found at this point. (ex: ComboBox default style)
                itemsPresenter = INTERNAL_VisualTreeManager.GetChildOfType<ItemsPresenter>(this);
            }

            if (itemsPresenter != null)
            {
                this._itemsPresenter = itemsPresenter;
                itemsPresenter.AttachToOwner(this);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Derived classes can call this methed in their constructor if they 
        /// want to disable the default rendering of the ItemsControl. It can 
        /// be useful for example to replace the rendering with a custom 
        /// HTML-based one.
        /// </summary>
        [Obsolete("Disabling default rendering is not supported anymore.")]
        protected void DisableDefaultRendering()
        {
        }

        /// <summary>
        /// Returns the item itself if the item is already a container of the 
        /// correct type, otherwise it returns null if no container is to be 
        /// created, or it returns the new container otherwise.
        /// </summary>
        /// <param name="item">The item to generate the container with.</param>
        /// <returns>
        /// Returns the item itself if the item is already a container of the 
        /// correct type, otherwise it returns null if no container is to be 
        /// created, or it returns the new container otherwise.
        /// </returns>
        [Obsolete("Use GetContainerFromItem(object item) instead.")]
        protected virtual SelectorItem INTERNAL_GenerateContainer(object item)
        {
            return (SelectorItem)this.GetContainerFromItem(item);
        }

        protected virtual void ManageCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.HandleItemsChanged(e);
        }

        protected virtual void UpdateItemsPanel(ItemsPanelTemplate newTemplate)
        {
            if (this.ItemsPresenter != null)
            {
                this.ItemsPresenter.Template = newTemplate;
            }
        }

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.OnItemsSourceChanged_BeforeVisualUpdate(oldValue, newValue);
        }

        protected virtual void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            //we do nothing here
        }

        protected virtual void OnChildItemRemoved(object item)
        {
            // This is intented to be overridden by the controls that have 
            // a "SelectedItem" to make sure that the item is de-selected 
            // in case that the element is removed.
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

        // "forceUpdateAllChildren" is used to remove all the children
        // and add them back, for example when the ItemsPanel changes.
        protected virtual void UpdateChildrenInVisualTree(
            IEnumerable oldChildrenEnumerable,
            IEnumerable newChildrenEnumerable,
            bool forceUpdateAllChildren = false)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                List<object> oldChildren = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(oldChildrenEnumerable);
                List<object> newChildren = INTERNAL_ListsHelper.ConvertToListOfObjectsOrNull(newChildrenEnumerable);

                // if we want to update all the children (for example because 
                // ItemsPanel or ItemTemplate changed), we need to remove the 
                // old versions of the children
                if (forceUpdateAllChildren)
                {
                    foreach (object oldChild in oldChildren)
                    {
                        TryRemoveChildItemFromVisualTree(oldChild);
                    }
                    foreach (object newChild in newChildren)
                    {
                        AddChildItemToVisualTree(newChild);
                    }
                }
                else
                {
                    //--------------------------------------------------
                    // The way that this algorithm works is the following:
                    // We compare the elements of the new and old collections,
                    // and we add/remove the elements that are missing or added.
                    // To do so, we traverse the old and new collections at the
                    // same time by having a "foreach" on the new collection, and
                    // at the same time keeping a pointer to the current index in
                    // the old collection.
                    //--------------------------------------------------

                    int currentIndexInOldChildren = 0;
                    if (newChildren != null)
                    {
                        foreach (object newChild in newChildren)
                        {
                            if (oldChildren != null)
                            {
                                // we remove the old children that are not the current new
                                // child until we find it
                                while (currentIndexInOldChildren < oldChildren.Count && newChild != oldChildren[currentIndexInOldChildren])
                                {
                                    var oldChild = oldChildren[currentIndexInOldChildren];
                                    TryRemoveChildItemFromVisualTree(oldChild);
                                    ++currentIndexInOldChildren;
                                }
                            }
                            // it means that newChild == oldChildren[currentIndexInOldChildren] 
                            // since we got out of the while
                            if (oldChildren != null && currentIndexInOldChildren < oldChildren.Count)
                            {
                                // we let the current oldChild since it is the new child
                                ++currentIndexInOldChildren;
                            }
                            else
                            {
                                // we need to add the new child (all the old children have 
                                // been removed)
                                AddChildItemToVisualTree(newChild);
                            }

                        }
                    }
                    //we remove the remaining old children
                    if (oldChildren != null)
                    {
                        while (currentIndexInOldChildren < oldChildren.Count)
                        {
                            var oldChild = oldChildren[currentIndexInOldChildren];
                            TryRemoveChildItemFromVisualTree(oldChild);
                            ++currentIndexInOldChildren;
                        }
                    }
                }

                //-------------------------------------------------
                // Call the "OnChildItemRemoved" method for all the 
                // children that were in the old collection and are 
                // no longer in the new one:
                //-------------------------------------------------

                // For performance reasons, start by building a dictionary 
                // that contains all the new children as well as a count 
                // that says how may times each child is present (> 1 in 
                // case of multiple identical children)

                // Note: the "int" is the number of occurences in case the 
                // same child is present twice.
                Dictionary<object, int> newChildrenFastAccess = new Dictionary<object, int>();
                if (newChildren != null)
                {
                    foreach (var newChild in newChildren)
                    {
                        if (newChildrenFastAccess.ContainsKey(newChild))
                            newChildrenFastAccess[newChild] = newChildrenFastAccess[newChild] + 1;
                        else
                            newChildrenFastAccess.Add(newChild, 1);
                    }
                }
                // Then, traverse the collection of old children to call
                // "OnChildItemRemoved" for each of those that are no longer 
                // in the "newChildren" collection:
                if (oldChildren != null)
                {
                    foreach (var oldChild in oldChildren)
                    {
                        if (!newChildrenFastAccess.ContainsKey(oldChild))
                        {
                            OnChildItemRemoved(oldChild);
                        }
                        else
                        {
                            int newCount = newChildrenFastAccess[oldChild] - 1;
                            newChildrenFastAccess[oldChild] = newCount;
                            if (newCount == 0)
                                newChildrenFastAccess.Remove(oldChild);
                        }
                    }
                }
            }
        }

        protected virtual bool TryRemoveChildItemFromVisualTree(object item)
        {
            //-----------------------------
            // DETACH ITEM
            //-----------------------------
            var containerIfAny = ItemContainerGenerator.ContainerFromItem(item);
            if (containerIfAny == null)
            {
                if (item is UIElement itemAsUIElement)
                {
                    // It means that no DataTemplate was applied,
                    // so we just remove the element.
                    if (this.RenderedItemsPanel != null)
                    {
                        return this.RenderedItemsPanel.Children.Remove(itemAsUIElement);
                    }
                }
            }
            else if (containerIfAny is FrameworkElement containerAsFE)
            {
                if (this.RenderedItemsPanel != null)
                {
                    return this.RenderedItemsPanel.Children.Remove(containerAsFE);
                }
                return ItemContainerGenerator.INTERNAL_TryUnregisterContainer(containerIfAny, item);
            }
            return false;
        }

        private FrameworkElement CreateContainerFromItem(object item)
        {
            FrameworkElement newContent = GenerateFrameworkElementToRenderTheItem(item);

            ContentControl containerIfAny = GetContainerFromItem(item) as ContentControl;

            if (containerIfAny == null)
            {
                //-------------------------------------------------------
                // If we arrive here, it means that there is no container
                // to be generated, such as in a standard ItemsControl.
                //-------------------------------------------------------

                // We remember the new content (it may be a DataTemplate 
                // for example), so that we can later remove it by finding 
                // it in the "_itemContainerGenerator" collection based on 
                // the business object "item"
                this.ItemContainerGenerator.INTERNAL_RegisterContainer(newContent, item);

                return newContent;
            }
            else
            {
                //-------------------------------------------------------
                // If we arrive here, it means that either the newContent 
                // is already a container (of the correct type), or a new 
                // container was generated.
                //-------------------------------------------------------

                containerIfAny.DataContext = item;

                //if the user defined a style for the container, 
                // we apply it
                containerIfAny.Style = this.ItemContainerStyle;

                if (containerIfAny == newContent)
                {
                    //---------------------------------------------------
                    // If we arrive here, it means that the newContent is
                    // already a container (of the correct type). For 
                    // example, this happens if the user adds a 
                    // ListBoxItem to a ListBox.
                    //---------------------------------------------------

                    // (Nothing to do)
                }
                else
                {
                    //---------------------------------------------------
                    // If we arrive here, it means a container was 
                    // generated. For example, when the user adds an 
                    // object to a ListBox, a ListBoxItem container is 
                    // generated.
                    //---------------------------------------------------

                    // We put the content into the container
                    containerIfAny.Content = newContent;
                }

                // We register the container so that later we can
                // find it back, given the "item"
                this.ItemContainerGenerator.INTERNAL_RegisterContainer(containerIfAny, item);

                return containerIfAny;
            }
        }

        protected virtual void AddChildItemToVisualTree(object item)
        {
            //-------------------------------------------------------
            // ATTACH NEW CONTENT
            //-------------------------------------------------------

            // Generate a FrameworkElement from the "item"
            FrameworkElement newContent = GenerateFrameworkElementToRenderTheItem(item);

            // If necessary, generate a container (such as "ListBoxItem",
            // "ComboBoxItem", etc.), and attach the item to the visual tree
            if (RenderedItemsPanel != null)
            {
                if (newContent != null) //otherwise, we are not in the Visual tree
                {
                    var containerIfAny = GetContainerFromItem(item) as ContentControl;

                    if (containerIfAny == null)
                    {
                        //-------------------------------------------------------
                        // If we arrive here, it means that there is no container
                        // to be generated, such as in a standard ItemsControl.
                        //-------------------------------------------------------

                        // We remember the new content (it may be a DataTemplate 
                        // for example), so that we can later remove it by finding 
                        // it in the "_itemContainerGenerator" collection based on 
                        // the business object "item"
                        ItemContainerGenerator.INTERNAL_RegisterContainer(newContent, item);

                        // We directly attach the content to the visual tree
                        this.RenderedItemsPanel.Children.Add(newContent);
                    }
                    else
                    {
                        //-------------------------------------------------------
                        // If we arrive here, it means that either the newContent 
                        // is already a container (of the correct type), or a new 
                        // container was generated.
                        //-------------------------------------------------------

                        containerIfAny.DataContext = item;

                        //if the user defined a style for the container, 
                        // we apply it
                        if (ItemContainerStyle != null)
                        {
                            containerIfAny.Style = ItemContainerStyle;
                        }

                        // We register the container so that later we can
                        // find it back, given the "item"
                        ItemContainerGenerator.INTERNAL_RegisterContainer(containerIfAny, item);

                        // We attach the container to the visual tree
                        this.RenderedItemsPanel.Children.Add(containerIfAny);

                        if (containerIfAny == newContent)
                        {
                            //---------------------------------------------------
                            // If we arrive here, it means that the newContent is
                            // already a container (of the correct type). For 
                            // example, this happens if the user adds a 
                            // ListBoxItem to a ListBox.
                            //---------------------------------------------------

                            // (Nothing to do)
                        }
                        else
                        {
                            //---------------------------------------------------
                            // If we arrive here, it means a container was 
                            // generated. For example, when the user adds an 
                            // object to a ListBox, a ListBoxItem container is 
                            // generated.
                            //---------------------------------------------------

                            // We put the content into the container
                            containerIfAny.Content = newContent;
                        }
                    }
                }
            }
        }

        protected FrameworkElement GenerateFrameworkElementToRenderTheItem(object item)
        {
            //---------------------------------------------------
            // if the item is a FrameworkElement, return itself
            //---------------------------------------------------
            FrameworkElement result = item as FrameworkElement;
            if (result == null)
            {
                object displayElement = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, this.DisplayMemberPath);
                if (this.ItemTemplate != null)
                {
                    //---------------------------------------------------
                    // An ItemTemplate was specified, so we instantiate 
                    // it and return it
                    //---------------------------------------------------

                    // Apply the data template
                    result = ItemTemplate.INTERNAL_InstantiateFrameworkTemplate();
                    result.DataContext = displayElement;
                }
                else
                {
                    //---------------------------------------------------
                    // Otherwise we simply call "ToString()" to display 
                    // the item as a string inside a TextBlock
                    //---------------------------------------------------

                    ContentPresenter container = new ContentPresenter();
                    Binding b = new Binding(this.DisplayMemberPath);
                    container.SetBinding(ContentControl.ContentProperty, b);
                    container.DataContext = item;
                    result = container;
                }
            }
#if WORKINPROGRESS
            this.PrepareContainerForItemOverride(result, item);
#endif
            return result;
        }

        /// <summary>
        /// Create or identify the element used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected virtual DependencyObject GetContainerFromItem(object item)
        {
            return null;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            // We update the ItemsPanel only if there is no ControlTemplate.
            // Otherwise, it will be done in the "OnApplyTemplate" method.
            base.INTERNAL_OnAttachedToVisualTree();

            if (!this.HasTemplate)
            {
                // If we have no template we have to create an ItemPresenter
                // manually and attach it to this control.
                // This can happen for instance if a class derive from
                // ItemsControl and specify a DefaultStyleKey and the associated
                // default style does not contain a Setter for the Template
                // property.
                // Note: this is a Silverlight specific behavior.
                // In WPF the content of the ItemsControl would simply not be
                // displayed in this scenario.
                this._itemsPresenter = new ItemsPresenter();
                this._itemsPresenter.AttachToOwner(this);

#if REWORKLOADED
                this.AddVisualChild(this._itemsPresenter);
#else
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this._itemsPresenter, this);
#endif

                // we need to set this variable so that if we move from no
                // template to a template, the "manually generated" template
                // will be detached as expected.
                this._renderedControlTemplate = this._itemsPresenter;
            }
        }

        #endregion Protected Methods

        #region Internal Properties

        internal ItemsPresenter ItemsPresenter
        {
            get { return this._itemsPresenter; }
        }

        internal Panel RenderedItemsPanel
        {
            get
            {
                if (this.ItemsPresenter != null)
                {
                    return this.ItemsPresenter.ItemsHost;
                }
                return null;
            }
        }

        #endregion Internal Properties

        #region Internal Methods

        internal void HandleItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.RenderedItemsPanel == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.RenderedItemsPanel.Children.Clear();
                    this.ItemContainerGenerator.INTERNAL_Clear();
                    foreach (var item in this.Items)
                    {
                        this.RenderedItemsPanel.Children.Add(this.CreateContainerFromItem(item));
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    FrameworkElement newChild = this.CreateContainerFromItem(e.NewItems[0]);
                    this.RenderedItemsPanel.Children.Insert(e.NewStartingIndex, newChild);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.ItemContainerGenerator.INTERNAL_TryUnregisterContainer(
                        this.RenderedItemsPanel.Children[e.OldStartingIndex],
                        e.OldItems[0]);
                    this.RenderedItemsPanel.Children.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    FrameworkElement newItem = this.CreateContainerFromItem(e.NewItems[0]);
                    this.ItemContainerGenerator.INTERNAL_TryUnregisterContainer(
                        this.RenderedItemsPanel.Children[e.OldStartingIndex],
                        e.OldItems[0]);
                    this.RenderedItemsPanel.Children[e.OldStartingIndex] = newItem;
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Unexpected collection change action '{0}'.", e.Action));
            }
        }

        internal void UpdateChildrenInVisualTree(NotifyCollectionChangedEventArgs e)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (this.RenderedItemsPanel != null)
                    {
                        this.RenderedItemsPanel.Children.Clear();
                    }
                    this.ItemContainerGenerator.INTERNAL_Clear();
                    if (this.Items.Count > 0)
                    {
                        this.UpdateChildrenInVisualTree(Enumerable.Empty<object>(), this.Items, true);
                    }
                    return;
                }

                if (e.Action == NotifyCollectionChangedAction.Remove ||
                    e.Action == NotifyCollectionChangedAction.Replace)
                {
                    Debug.Assert(e.OldItems.Count == 1);
                    List<object> removedChildren = new List<object>(e.OldItems.Count);
                    foreach (object item in e.OldItems)
                    {
                        bool removed = this.TryRemoveChildItemFromVisualTree(item);
                        if (removed)
                        {
                            removedChildren.Add(item);
                        }
                    }

                    //-------------------------------------------------
                    // Call the "OnChildItemRemoved" method for all the 
                    // children that were in the old collection
                    //-------------------------------------------------
                    foreach (object removedItem in removedChildren)
                    {
                        this.OnChildItemRemoved(removedItem);
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Add ||
                    e.Action == NotifyCollectionChangedAction.Replace)
                {
                    Debug.Assert(e.NewItems.Count == 1);
                    foreach (object item in e.NewItems)
                    {
                        this.AddChildItemToVisualTree(item);
                    }
                }
            }
        }

        private void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ManageCollectionChanged(e);

            this.OnItemsChanged(e);
        }

        #endregion Internal Methods

        #region Work in progress
#if WORKINPROGRESS

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected virtual void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected virtual DependencyObject GetContainerForItemOverride()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            //throw new NotImplementedException();
            //todo: implement this
            //for now the implementation of this method is fully handled by derived classes.
        }

        public static ItemsControl GetItemsOwner(DependencyObject element)
        {
            return null;
        }

        public static ItemsControl ItemsControlFromItemContainer(DependencyObject container)
        {
            return null;
        }
#endif
        #endregion

    }
}