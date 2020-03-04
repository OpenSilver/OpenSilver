

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using System.Windows.Controls;
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
        protected ItemContainerGenerator _itemContainerGenerator = new ItemContainerGenerator(); // Note: this maps an item (for example a string) to the element that is added to the visual tree (such a datatemplate) or to the native DOM element in case of native combo box for example.
        protected FrameworkElement _placeWhereItemsPanelWillBeRendered = null; // By default, this variable will be equal to "this" (set in the contructor), unless instructed otherwise by a deriving class (such as a templated ListBox or ComboBox):
#if WORKINPROGRESS
        protected Panel _renderedItemsPanel = null;
#else
        protected FrameworkElement _renderedItemsPanel = null;
#endif
        protected IEnumerable _itemsSourceAsSetByUser; // This corresponds to the value that the user has set to ItemsSource. The only case where this value differs from this.ItemsSource is when the validation of the value entered by the user has failed (for example if the user sets to a value while the Items collection is not empty).
        protected IEnumerable _actualItemsSource; // This corresponds to the elements that are actually displayed. It is either equal to "_itemsSourceAsSetByUser" (if the user has specified an ItemsSource), or it is equal to an internally-created ItemsCollection.
        List<object> _snapshotOfTheActualItemsSourceCollectionBeforeCollectionChangedEvent; // This variable stores a "snapshot" of the "Items" collection before the "CollectionChanged" event. The idea here is that when the user adds/removes/clears the collection, we arrive into the "CollectionChanged" event, but we have no mean to know what the collection was like before it was changed. This variables allows us to get the answer to this question.
        bool _disableDefaultRendering; // It disables the default rendering of this control. It can be set to True via the "DisableDefaultRendering()" method. It can be useful for example to replace the rendering with a custom HTML-based one.
        protected bool _workaroundForComboBox; //todo: avoid using this workaround.

        /// <summary>
        /// Derived classes can call this methed in their constructor if they want to disable the default rendering of the ItemsControl. It can be useful for example to replace the rendering with a custom HTML-based one.
        /// </summary>
        protected void DisableDefaultRendering()
        {
            _disableDefaultRendering = true;
        }

#region Properties

        /// <summary>
        /// Gets the collection used to generate the content of the control. WARNING: When ItemsSource is set, it will only return a snapshot. The returned values will not be updated at the same time as the ItemsSource.
        /// </summary>
        public ItemCollection Items
        {
            get
            {
                if (_itemsSourceAsSetByUser == null)
                {
                    //------------------------
                    // The user has not set any ItemsSource. So we return the default collection in order for him to be able to modify it.
                    //------------------------
                    return (ItemCollection)_actualItemsSource; // So we return the default collection.
                }
                else
                {
                    //------------------------
                    // The user has set an ItemsSource for the ItemsControl, so he is not allowed to modify the "Items" collection directly.
                    //------------------------
                    var copy = new ItemCollection(ConvertToListOfObjectsOrNull(_actualItemsSource)); // return a copy o the collection that the user has used for ItemsSource //Note: "_actualItemsSource" can never be null.
                    copy.CollectionChanged += (s, e) =>
                    {
                        throw new InvalidOperationException("Operation is not valid while ItemsSource is in use. Access and modify elements with ItemsControl.ItemsSource instead.");
                    };
                    return copy;
                }
            }
        }


        /// <summary>
        /// Gets or sets the template that defines the panel that controls the layout
        /// of items.
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(ItemsPanelProperty);
            }
            set { SetValue(ItemsPanelProperty, value); }
        }
        /// <summary>
        /// Identifies the ItemsPanel dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(ItemsControl),
                new PropertyMetadata(GetDefaultItemsPanel(), OnItemsPanel_Changed)
                {
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never
                });

        static ItemsPanelTemplate GetDefaultItemsPanel()
        {
            ItemsPanelTemplate template = new ItemsPanelTemplate()
            {
                _methodToInstantiateFrameworkTemplate = (Control templateOwner) => new TemplateInstance() { TemplateContent = new StackPanel() }, // Default items panel. Note: the parameter templateOwner is made necessary for the ControlTemplates but can be kept null for DataTemplates.
            };
            template.Seal();
            //Note: We seal the template in order to avoid letting the user modify the default template itself since it is the same instance that is used as the default value for all ItemsControls.
            //      This would bring issues such as a user modifying the default template for one element then modifying it again for another one and both would have the last one's template.
            return template;
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
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsControl), new PropertyMetadata(null, ItemsSource_Changed));


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
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ItemsControl), new PropertyMetadata(null, OnItemTemplate_Changed));


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

#endregion


        /// <summary>
        /// Initializes a new instance of the ItemsControl class.
        /// </summary>
        public ItemsControl()
        {
            var defaultItemsCollection = new ItemCollection(); // The type is "object" because the user can enter any type here.
            defaultItemsCollection.CollectionChanged += ItemCollection_CollectionChanged;
            _actualItemsSource = defaultItemsCollection;

            // By default, the ItemsPanel will be attached to this very control, unless instructed otherwise by a deriving class (such as a templated ListBox or ComboBox):
            _placeWhereItemsPanelWillBeRendered = this;

            // We remember the content of the "Items" collection, for future use. We do this every time that the _actualItemsSource changes. This is useful for example in the CollectionChanged event, to compare the new collection content with the old collection content.
            RememberTheContentOfTheActualItemsSourceForFutureUse();
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            if (!_disableDefaultRendering)
            {
                // We update the ItemsPanel only if there is no ControlTemplate. Otherwise, it will be done in the "OnApplyTemplate" method.
                if (!this.HasTemplate)
                {
                    _placeWhereItemsPanelWillBeRendered = this;
                    UpdateItemsPanel(ItemsPanel);
                }
            }
        }

        void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ManageCollectionChanged(e);

            // We remember the content of the new "Items" collection, for future use. We do this every time that the _actualItemsSource changes. This is useful for example in the CollectionChanged event, to compare the new collection content with the old collection content.
            RememberTheContentOfTheActualItemsSourceForFutureUse();
        }

        protected virtual void ManageCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_disableDefaultRendering)
            {
                // We get the collection in the state that it was BEFORE the CollectionChanged event. In fact, e.OldItems contains only the "delta" of the change (for example, if 2 items are deleted, it contains only the 2 deleted items), and we have no other way to knwo what items the collection contained before it was changed.
                List<object> previousCollection = _snapshotOfTheActualItemsSourceCollectionBeforeCollectionChangedEvent;

                //if (_renderedItemsPanel != null) //todo: see if we need a test of whether the _renderedItemsPanelTemplate is in the visualTree or not.
                //{
                OnItemsChanged(e);
                UpdateChildrenInVisualTree(previousCollection, Items); //todo-perfs: in case that "e.Action" is "Replace" or "Move", we should do something more optimized than just re-drawing the entire list.
                //}
            }
        }

        static void OnItemsPanel_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ItemsControl)d).UpdateItemsPanel((ItemsPanelTemplate)e.NewValue);
        }

        protected virtual void UpdateItemsPanel(ItemsPanelTemplate newTemplate)
        {
            if (!_disableDefaultRendering)
            {
                //if (_placeWhereItemsPanelWillBeRendered != null
                //    //&& INTERNAL_VisualTreeManager.IsElementInVisualTree(_placeWhereItemsPanelWillBeRendered))
                //    && _placeWhereItemsPanelWillBeRendered._isLoaded) //Note: we replaced "IsElementInVisualTree" with _isLoaded on on March 22, 2017 to fix an issue where a "Binding" on ListBox.ItemsSource caused the selection to not work properly. This change can be reverted the day that the implementation of the "IsElementInVisualTree" method becomes based on the "_isLoaded" property (at the time of writing, it was implemented by checking if the visual parent is null).
                if (this._placeWhereItemsPanelWillBeRendered != null)
                {
                    if (this._placeWhereItemsPanelWillBeRendered.IsLoaded)
                    {
                        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(this._renderedItemsPanel, _placeWhereItemsPanelWillBeRendered);

                        if (newTemplate != null)
                        {
                            // Create an instance of the Panel:
#if WORKINPROGRESS
                            FrameworkElement template = newTemplate.INTERNAL_InstantiateFrameworkTemplate();
                            if (template is Panel panel)
                            {
                                this._renderedItemsPanel = panel;
                            }
                            else
                            {
                                throw new InvalidOperationException("ItemsControl.ItemsPanelTemplate must derive from Panel.");
                            }
                            // Make sure that the panel contains no children:
                            if (this._renderedItemsPanel.Children != null && this._renderedItemsPanel.Children.Count > 0)
                            {
                                throw new InvalidOperationException("Cannot explicitly modify Children collection of Panel used as ItemsPanel for ItemsControl. ItemsControl generates child elements for Panel.");
                            }
#else
                            _renderedItemsPanel = newTemplate.INTERNAL_InstantiateFrameworkTemplate();

                            // Make sure the panel derives from the type "Panel":
                            if (!(_renderedItemsPanel is Panel))
                            {
                                throw new InvalidOperationException("ItemsControl.ItemsPanelTemplate must derive from Panel.");
                            }

                            // Make sure that the panel contains no children:
                            if (((Panel)_renderedItemsPanel).Children != null && ((Panel)_renderedItemsPanel).Children.Count > 0)
                            {
                                throw new InvalidOperationException("Cannot explicitly modify Children collection of Panel used as ItemsPanel for ItemsControl. ItemsControl generates child elements for Panel.");
                            }
#endif

                            // Attach the panel:
#if REWORKLOADED
                            this._placeWhereItemsPanelWillBeRendered.AddVisualChild(this._renderedItemsPanel);
#else
                            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_renderedItemsPanel, _placeWhereItemsPanelWillBeRendered);
#endif

                            // Update the children:
                            if (_actualItemsSource != null)
                            {
                                OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                UpdateChildrenInVisualTree(_actualItemsSource, _actualItemsSource, forceUpdateAllChildren: true);
                            }
                        }
                        else
                        {
                            // The ItemsPanel is null, so we display nothing (like in WPF):
                            _renderedItemsPanel = null;
                            _itemContainerGenerator.INTERNAL_Clear();
                        }
                    }
                    else
                    {
                        this._placeWhereItemsPanelWillBeRendered.Loaded -= new RoutedEventHandler(this.UpdateItemsPanelOnContainerLoaded);
                        this._placeWhereItemsPanelWillBeRendered.Loaded += new RoutedEventHandler(this.UpdateItemsPanelOnContainerLoaded);
                    }
                }
            }
        }

        private void UpdateItemsPanelOnContainerLoaded(object sender, RoutedEventArgs e)
        {
            this._placeWhereItemsPanelWillBeRendered.Loaded -= new RoutedEventHandler(this.UpdateItemsPanelOnContainerLoaded);
            this.UpdateItemsPanel(this.ItemsPanel);
        }

        protected virtual void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Do something only if the ItemsSource has changed:
            if (newValue != this._actualItemsSource)
            {
                //throw an exception if Items is not empty and the old ItemsSource is null (which means the items in Items have been put there directly by the user without passing through ItemsSource):
                bool areWeMovingFromDefaultCollectionToCustomOne = (oldValue == null);
                bool previousCollectionWasNotEmpty = (this._actualItemsSource is ItemCollection itemCollection) && itemCollection.Count > 0;
                if (areWeMovingFromDefaultCollectionToCustomOne && previousCollectionWasNotEmpty)
                {
                    throw new InvalidOperationException("Items collection must be empty before using ItemsSource.");
                }

                // Remember the value set by the user (see note next to the declaration of this variable):
                _itemsSourceAsSetByUser = newValue;

                // Unregister the "CollectionChanged" of the previous collection:
                if (this._actualItemsSource is INotifyCollectionChanged oldItemsSourceAsCollectionChanged)
                {
                    oldItemsSourceAsCollectionChanged.CollectionChanged -= this.ItemCollection_CollectionChanged;
                }

                // Distinguish between the case where the user is setting its own ItemsSource and the case where he is setting null to use the default collection provided by the control:
                if (newValue != null)
                {
                    // Remember the new ItemsSource:
                    this._actualItemsSource = newValue;
                }
                else
                {
                    // Remember the new ItemsSource:
                    this._actualItemsSource = new ItemCollection();
                }

                // We remember the content of the new "Items" collection, for future use. We do this every time that the _actualItemsSource changes. This is useful for example in the CollectionChanged event, to compare the new collection content with the old collection content.
                RememberTheContentOfTheActualItemsSourceForFutureUse();

                // Register the "CollectionChanged" event:
                if (this._actualItemsSource is INotifyCollectionChanged itemsSourceAsCollectionChanged)
                {
                    itemsSourceAsCollectionChanged.CollectionChanged += this.ItemCollection_CollectionChanged;
                }

                OnItemsSourceChanged_BeforeVisualUpdate(oldValue, newValue);

                if (_renderedItemsPanel == null && ItemsPanel != null && !_workaroundForComboBox)
                {
                    UpdateItemsPanel(ItemsPanel);
                }
                else //if (_renderedItemsPanel != null)
                {
                    //we set the new Items (which will refresh the display)
                    OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    UpdateChildrenInVisualTree(oldValue, newValue);
                }
            }
        }

        protected virtual void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            //we do nothing here
        }

        protected virtual void OnChildItemRemoved(object item)
        {
            //This is intented to be overridden by the controls that have a "SelectedItem" to make sure that the item is de-selected in case that the element is removed.
        }

        /// <summary>
        ///  Called when the value of the System.Windows.Controls.ItemsControl.Items property changes.
        /// </summary>
        /// <param name="e">A System.Collections.Specialized.NotifyCollectionChangedEventArgs that contains the event data</param>
        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Attempt to find the <ItemsPresenter /> in the ControlTemplate and, if it was found, use it as the place where all the items should be added:
            ItemsPresenter itemsPresenter = INTERNAL_VisualTreeManager.GetChildOfType<ItemsPresenter>(this);
            if (itemsPresenter != null)
                _placeWhereItemsPanelWillBeRendered = itemsPresenter;

            // Update the ItemsPanel:
            UpdateItemsPanel(ItemsPanel);
        }

        static void ItemsSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: test scenario: Add ItemsSource, change it for something that is not IEnumerable, change it for something that is IEnumerable
            //todo: also see DataGrid.OnCollectionChanged (it covers one more case)
            ((ItemsControl)d).OnItemsSourceChanged((IEnumerable)e.OldValue, (IEnumerable)e.NewValue);
        }


        protected virtual void UpdateChildrenInVisualTree(IEnumerable oldChildrenEnumerable, IEnumerable newChildrenEnumerable, bool forceUpdateAllChildren = false) // "forceUpdateAllChildren" is used to remove all the children and add them back, for example when the ItemsPanel changes.
        {
            if (!_disableDefaultRendering)
            {
                if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this)) // && _renderedItemsPanel != null)
                {
                    ////// In case that a Binding causes the call of this UpdateChildrenInVisualTree" method before we were able to call "UpdateItemsPanel":
                    ////if (_actualItemsPanel is NotInitializedItemsPanelTemplate)
                    ////    UpdateItemsPanel(null, this);

#if PERFSTAT
                    var t0 = Performance.now();
#endif
                    List<object> oldChildren = ConvertToListOfObjectsOrNull(oldChildrenEnumerable);
                    List<object> newChildren = ConvertToListOfObjectsOrNull(newChildrenEnumerable);
#if PERFSTAT
                    Performance.Counter("ItemsControl.UpdateChildrenInVisualTree => ConvertToListOfObjectsOrNull", t0);
#endif

                    if (forceUpdateAllChildren) //if we want to update all the children (for example because ItemsPanel or ItemTemplate changed), we need to remove the old versions of the children
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
                                    //we remove the old children that are not the current new child until we find it:
                                    while (currentIndexInOldChildren < oldChildren.Count && newChild != oldChildren[currentIndexInOldChildren])
                                    {
                                        var oldChild = oldChildren[currentIndexInOldChildren];
                                        TryRemoveChildItemFromVisualTree(oldChild);
                                        ++currentIndexInOldChildren;
                                    }
                                }
                                if (oldChildren != null && currentIndexInOldChildren < oldChildren.Count) //it means that newChild == oldChildren[currentIndexInOldChildren] since we got out of the while
                                {
                                    ++currentIndexInOldChildren; //we let the current oldChild since it is the new child
                                }
                                else //we need to add the new child (all the old children have been removed)
                                {
                                    AddChildItemToVisualTree(newChild);
                                }

                            }
                        }
                        //we remove the remaining old children:
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

                    //----------------------------------------
                    // Call the "OnChildItemRemoved" method for all the children that were in the old collection and are no longer in the new one:
                    //----------------------------------------

                    // For performance reasons, start by building a dictionary that contains all the new children as well as a count that says how may times each child is present (> 1 in case of multiple identical children)
                    Dictionary<object, int> newChildrenFastAccess = new Dictionary<object, int>(); //the "int" is the number of occurences in case the same child is present twice.
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
                    // Then, traverse the collection of old children to call "OnChildItemRemoved" for each of those that are no longer in the "newChildren" collection:
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
        }

        /// <summary>
        /// Invoked when the value of the ItemTemplate property changes.
        /// </summary>
        static void OnItemTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl parent = (ItemsControl)d;
            if (!parent._disableDefaultRendering)
            {
                if (e.NewValue != e.OldValue)
                {
                    if (parent._renderedItemsPanel != null)
                    {
                        var itemCollection = parent.Items;
                        parent.OnItemsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, itemCollection));
                        parent.UpdateChildrenInVisualTree(itemCollection, itemCollection, forceUpdateAllChildren: true);
                    }
                }
            }
        }

        protected virtual bool TryRemoveChildItemFromVisualTree(object item)
        {
            if (!_disableDefaultRendering)
            {
                ////-----------------------------
                //// DETACH ITEM
                ////-----------------------------
                var containerIfAny = _itemContainerGenerator.ContainerFromItem(item);
                if (containerIfAny == null)
                {
                    if (item is UIElement itemAsUIElement)
                    {
                        // It means that no DataTemplate was applied, so we just remove the element.

                        if (itemAsUIElement.INTERNAL_VisualParent != null)
                        {
                            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(itemAsUIElement, (UIElement)itemAsUIElement.INTERNAL_VisualParent);
                            return true;
                        }
                    }
                }
                else if (containerIfAny is FrameworkElement containerAsFE)
                {
                    if ((UIElement)containerAsFE.INTERNAL_VisualParent != null)
                    {
                        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(containerAsFE, (UIElement)containerAsFE.INTERNAL_VisualParent);
                    }
                    if (INTERNAL_VisualTreeManager.IsElementInVisualTree(_renderedItemsPanel))
                    {
                        return _itemContainerGenerator.INTERNAL_TryUnregisterContainer(containerIfAny, item);
                    }
                }
            }
            return false;
        }

        protected virtual void AddChildItemToVisualTree(object item)
        {
            if (!_disableDefaultRendering)
            {
                //-----------------------------
                // ATTACH NEW CONTENT
                //-----------------------------

                // Generate a FrameworkElement from the "item":
                FrameworkElement newContent = GenerateFrameworkElementToRenderTheItem(item);

                // If necessary, generate a container (such as "ListBoxItem", "ComboBoxItem", etc.), and attach the item to the visual tree:
                if (_renderedItemsPanel != null && INTERNAL_VisualTreeManager.IsElementInVisualTree(_renderedItemsPanel))
                {
                    if (newContent != null) //otherwise, we are not in the Visual tree
                    {
                        var containerIfAny = (GetContainerFromItem(item) ?? INTERNAL_GenerateContainer(newContent)) as ContentControl; //todo: Remove INTERNAL_GenerateContainer()

                        if (containerIfAny == null)
                        {
                            //-----------------------------
                            // If we arrive here, it means that there is no container to be generated, such as in a standard ItemsControl.
                            //-----------------------------

                            // We remember the new content (it may be a DataTemplate for example), so that we can later remove it by finding it in the "_itemContainerGenerator" collection based on the business object "item":
                            _itemContainerGenerator.INTERNAL_RegisterContainer(newContent, item);

                            // We directly attach the content to the visual tree:
#if WORKINPROGRESS
                            this._renderedItemsPanel.Children.Add(newContent);
#else
                            ((Panel)this._renderedItemsPanel).Children.Add(newContent);
#endif
                        }
                        else
                        {
                            //-----------------------------
                            // If we arrive here, it means that either the newContent is already a container (of the correct type), or a new container was generated.
                            //-----------------------------

                            //if the user defined a style for the container, we apply it:
                            if (ItemContainerStyle != null)
                            {
                                containerIfAny.Style = ItemContainerStyle;
                            }

                            // We register the container so that later we can find it back, given the "item":
                            _itemContainerGenerator.INTERNAL_RegisterContainer(containerIfAny, item);

                            //TODO: remove this and do it directly in selector controls.
                            // We keep a reference from the SelectorItem to the Selector:
                            if (this is Selector)
                            {
                                // We remember the item associated to the container:
                                ((SelectorItem)containerIfAny).INTERNAL_CorrespondingItem = item;
                                ((SelectorItem)containerIfAny).INTERNAL_ParentSelectorControl = (Selector)this;
                            }

                            // We attach the container to the visual tree:
#if WORKINPROGRESS
                            this._renderedItemsPanel.Children.Add(containerIfAny);
#else
                            ((Panel)this._renderedItemsPanel).Children.Add(containerIfAny);
#endif

                            if (containerIfAny == newContent)
                            {
                                //-----------------------------
                                // If we arrive here, it means that the newContent is already a container (of the correct type). For example, this happens if the user adds a ListBoxItem to a ListBox.
                                //-----------------------------

                                // (Nothing to do)
                            }
                            else
                            {
                                //-----------------------------
                                // If we arrive here, it means a container was generated. For example, when the user adds an object to a ListBox, a ListBoxItem container is generated.
                                //-----------------------------

                                // We put the content into the container:
                                containerIfAny.Content = newContent;
                            }
                        }
                    }
                }
            }
        }

        protected FrameworkElement GenerateFrameworkElementToRenderTheItem(object item)
        {
            //---------------
            // if the item is a FrameworkElement, return itself:
            //---------------
            FrameworkElement result = item as FrameworkElement;
            if (result == null)
            {
                object displayElement = PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, this.DisplayMemberPath);
                if (this.ItemTemplate != null)
                {
                    //---------------
                    // An ItemTemplate was specified, so we instantiate it and return it:
                    //---------------

                    // Apply the data template:
                    result = ItemTemplate.INTERNAL_InstantiateFrameworkTemplate();
                    result.DataContext = displayElement;
                }
                else
                {
                    //---------------
                    // Otherwise we simply call "ToString()" to display the item as a string inside a TextBlock:
                    //---------------

                    // Show as string:
                    //result = new TextBlock() { Text = displayElement.ToString() };

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
        /// Returns the item itself if the item is already a container of the correct type, otherwise it returns null if no container is to be created, or it returns the new container otherwise.
        /// </summary>
        /// <param name="item">The item to generate the container with.</param>
        /// <returns>Returns the item itself if the item is already a container of the correct type, otherwise it returns null if no container is to be created, or it returns the new container otherwise.</returns>
        [Obsolete("Use GetContainerFromItem(object item) instead.")]
        protected virtual SelectorItem INTERNAL_GenerateContainer(object item)
        {
            return null; // In a simple ItemsControl, not container is to be generated. Derived classes may want to generate a ListBoxItem, a ComboBoxItem, etc.
        }

        /// <summary>
        /// Create or identify the element used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected virtual DependencyObject GetContainerFromItem(object item)
        {
            return null;
        }


        protected static List<object> ConvertToListOfObjectsOrNull(IEnumerable enumerable)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif
            if (enumerable is List<object>)
            {
#if PERFSTAT
                Performance.Counter("ItemsControl.ConvertToListOfObjectsOrNull", t0);
#endif
                return (List<object>)enumerable;
            }
            else
            {
                List<object> result = null;
                if (enumerable != null)
                {
                    result = new List<object>();
                    foreach (var obj in enumerable)
                    {
                        result.Add(obj);
                    }
                }
#if PERFSTAT
                Performance.Counter("ItemsControl.ConvertToListOfObjectsOrNull", t0);
#endif
                return result;
            }
        }

        void RememberTheContentOfTheActualItemsSourceForFutureUse()
        {
            // We remember the content of the "Items" collection, for future use. We do this every time that the _actualItemsSource changes. This is useful for example in the CollectionChanged event, to compare the new collection content with the old collection content. (Read the note next to the definition of that variable for more details)
            _snapshotOfTheActualItemsSourceCollectionBeforeCollectionChangedEvent = ConvertToListOfObjectsOrNull(_actualItemsSource);
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
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(ItemsControl), new PropertyMetadata(string.Empty, DisplayMemberPath_Changed));
        private static void DisplayMemberPath_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ItemsControl), new PropertyMetadata(null, ItemContainerStyle_Changed));

        private static void ItemContainerStyle_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)d;
            itemsControl.UpdateChildrenInVisualTree(itemsControl.Items, itemsControl.Items, true);
        }


#if WORKINPROGRESS
        //// Returns:
        ////     The name or path of the property that is displayed for each the data item
        ////     in the control. The default is an empty string ("").
        ///// <summary>
        ///// Gets or sets the name or path of the property that is displayed for each
        ///// data item.
        ///// </summary>
        //public string DisplayMemberPath { get; set; }

        //// Returns:
        ////     The identifier for the DisplayMemberPath dependency property.
        ///// <summary>
        ///// Identifies the DisplayMemberPath dependency property.
        ///// </summary>
        //public static DependencyProperty DisplayMemberPathProperty { get; }

        //// Returns:
        ////     A collection of GroupStyle objects that define the appearance of each level
        ////     of groups.
        ///// <summary>
        ///// Gets a collection of GroupStyle objects that define the appearance of each
        ///// level of groups.
        ///// </summary>
        //public IObservableVector<GroupStyle> GroupStyle { get; }

        //// Returns:
        ////     A reference to a custom GroupStyleSelector logic class.
        ///// <summary>
        ///// Gets or sets a reference to a custom GroupStyleSelector logic class. The
        ///// GroupStyleSelector returns different GroupStyle values to use for content
        ///// based on the characteristics of that content.
        ///// </summary>
        //public GroupStyleSelector GroupStyleSelector { get; set; }

        //// Returns:
        ////     The identifier for the GroupStyleSelector dependency property.
        ///// <summary>
        ///// Identifies the GroupStyleSelector dependency property.
        ///// </summary>
        //public static DependencyProperty GroupStyleSelectorProperty { get; }

        //// Returns:
        ////     True if a control is using grouping; otherwise, false.
        ///// <summary>
        ///// Gets a value that indicates whether the control is using grouping.
        ///// </summary>
        //public bool IsGrouping { get; }

        //// Returns:
        ////     The identifier for the IsGrouping dependency property.
        ///// <summary>
        ///// Identifies the IsGrouping dependency property.
        ///// </summary>
        //public static DependencyProperty IsGroupingProperty { get; }

        //// Returns:
        ////     The ItemContainerGenerator associated with this ItemsControl.
        ///// <summary>
        ///// Gets the ItemContainerGenerator associated with this ItemsControl.
        ///// </summary>
        //public ItemContainerGenerator ItemContainerGenerator { get; }

        //// Returns:
        ////     A custom StyleSelector logic class.
        ///// <summary>
        ///// Gets or sets a reference to a custom StyleSelector logic class. The StyleSelector
        ///// returns different Style values to use for the item container based on characteristics
        ///// of the object being displayed.
        ///// </summary>
        //public StyleSelector ItemContainerStyleSelector { get; set; }

        //public static DependencyProperty ItemContainerStyleSelectorProperty { get; }

        //// Returns:
        ////     The collection of Transition style elements that apply to the item containers
        ////     of an ItemsControl.
        ///// <summary>
        ///// Gets or sets the collection of Transition style elements that apply to the
        ///// item containers of an ItemsControl.
        ///// </summary>
        //public TransitionCollection ItemContainerTransitions { get; set; }

        //public static DependencyProperty ItemContainerTransitionsProperty { get; }

        ///// <summary>
        ///// Gets or sets a reference to a custom DataTemplateSelector logic class. The
        ///// DataTemplateSelector referenced by this property returns a template to apply
        ///// to items.
        ///// </summary>
        //public DataTemplateSelector ItemTemplateSelector { get; set; }

        //public static DependencyProperty ItemTemplateSelectorProperty { get; }


        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected virtual void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Creates or identifies the element that is used to display the given item.
        //
        // Returns:
        //     The element that is used to display the given item.
        protected virtual DependencyObject GetContainerForItemOverride()
        {
            throw new NotImplementedException();
        }
        ////
        //// Summary:
        ////     Returns the ItemsControl that the specified element hosts items for.
        ////
        //// Parameters:
        ////   element:
        ////     The host element.
        ////
        //// Returns:
        ////     The ItemsControl that the specified element hosts items for, or null.
        //public static ItemsControl GetItemsOwner(DependencyObject element);
        //
        // Summary:
        //     Determines whether the specified item is (or is eligible to be) its own container.
        //
        // Parameters:
        //   item:
        //     The item to check.
        //
        // Returns:
        //     True if the item is (or is eligible to be) its own container; otherwise,
        //     false.
        protected virtual bool IsItemItsOwnContainerOverride(object item)
        {
            throw new NotImplementedException();
        }

        //// Summary:
        ////     Returns the ItemsControl that owns the specified container element.
        ////
        //// Parameters:
        ////   container:
        ////     The container element to return the ItemsControl for.
        ////
        //// Returns:
        ////     The ItemsControl that owns the specified container element; otherwise, null.
        //public static ItemsControl ItemsControlFromItemContainer(DependencyObject container);
        ////
        //// Summary:
        ////     Invoked when the value of the GroupStyleSelector property changes.
        ////
        //// Parameters:
        ////   oldGroupStyleSelector:
        ////     The previous value of the GroupStyleSelector property.
        ////
        ////   newGroupStyleSelector:
        ////     The current value of the GroupStyleSelector property.
        //protected virtual void OnGroupStyleSelectorChanged(GroupStyleSelector oldGroupStyleSelector, GroupStyleSelector newGroupStyleSelector);
        ////
        //// Summary:
        ////     Invoked when the value of the ItemContainerStyle property changes.
        ////
        //// Parameters:
        ////   oldItemContainerStyle:
        ////     The previous value of the ItemContainerStyle property.
        ////
        ////   newItemContainerStyle:
        ////     The current value of the ItemContainerStyle property.
        //protected virtual void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle);
        ////
        //// Summary:
        ////     Invoked when the value of the ItemContainerStyleSelector property changes.
        ////
        //// Parameters:
        ////   oldItemContainerStyleSelector:
        ////     The previous value of the ItemContainerStyleSelector property.
        ////
        ////   newItemContainerStyleSelector:
        ////     The current value of the ItemContainerStyleSelector property.
        //protected virtual void OnItemContainerStyleSelectorChanged(StyleSelector oldItemContainerStyleSelector, StyleSelector newItemContainerStyleSelector);


        ////
        //// Summary:
        ////     Invoked when the value of the ItemTemplateSelector property changes.
        ////
        //// Parameters:
        ////   oldItemTemplateSelector:
        ////     The previous value of the ItemTemplateSelector property.
        ////
        ////   newItemTemplateSelector:
        ////     The current value of the ItemTemplateSelector property.
        //protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldItemTemplateSelector, DataTemplateSelector newItemTemplateSelector);

        // Summary:
        //     Prepares the specified element to display the specified item.
        //
        // Parameters:
        //   element:
        //     The element that's used to display the specified item.
        //
        //   item:
        //     The item to display.
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

    }
}