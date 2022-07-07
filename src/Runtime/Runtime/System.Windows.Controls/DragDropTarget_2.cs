

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


using CSHTML5;
using CSHTML5.Internal;
using System.Windows;
using DotNetForHtml5.Core;
using System.Collections.Generic;
using System.Linq;
#if MIGRATION
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Windows;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using MS = Microsoft.Windows;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Windows.Controls;
using System;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using MS = System.Windows;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    //-----------------
    // Open-source code reference: https://github.com/MicrosoftArchive/SilverlightToolkit/blob/8dd7fb1f77bd61f7ed3eb5f40f69d725c79749bd/Release/Silverlight4/Source/Controls.Toolkit/DragDrop/DragDropTarget.cs
    //-----------------


    /// <summary>
    /// A control that enabled drag and drop operations on an ItemsControl.
    /// </summary>
    /// <typeparam name="TItemsControlType">The type of the items control.</typeparam>
    /// <typeparam name="TItemContainerType">The type of the item container.</typeparam>
    public abstract partial class DragDropTarget<TItemsControlType, TItemContainerType> : ContentControl
        where TItemsControlType : UIElement
        where TItemContainerType : UIElement
    {
        //pointer variable used for mouse operation
        bool _isPointerCaptured;
        Pointer _capturedPointer;
        double _pointerX;
        double _pointerY;

        //variable used when dragging
        TItemsControlType _sourceItemsControl;
        TItemContainerType _sourceItemContainer; // Note: the item container can be for example a ListBoxItem (in case of a ListBox inside a ListBoxDragDropTarget), or the item directly (in case of a StackPanel inside a PanelDragDropTarget).
        FrameworkElement _sourcePlaceholder; // This is a placeholder that we put in place of the source during the drag operation. In fact, the source is moved to a popup, so it leaves an empty space. The placeholder will occupy that empty space. This is useful to: 1) let the user drop over the source itself (cf. "ItemDroppedOnSource" event), and 2) prevent the other elements from being displaced during the drag operation.
        Popup _popup;
        int _indexOfSourceContainerWithinItemsControl;


        //We store this DragDropTarget in the case that we drag from a DragDropTarget to another and we want to compare them
        DragDropTarget<TItemsControlType, TItemContainerType> _previousdragDropTargetUnderPointer = null;

        bool _isDragCancelled;

        UIElement _iconStop;
        UIElement _iconArrow;

        #region public DragDropEffects AllowedSourceEffects
        /// <summary>
        /// Gets or sets the allowed effects when this DragDropTarget is the drag source.
        /// </summary>
        [OpenSilver.NotImplemented]
        public DragDropEffects AllowedSourceEffects
        {
            get { return (DragDropEffects)GetValue(AllowedSourceEffectsProperty); }
            set { SetValue(AllowedSourceEffectsProperty, value); }
        }

        /// <summary>
        /// Identifies the AllowedSourceEffects dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowedSourceEffectsProperty =
            DependencyProperty.Register(
                "AllowedSourceEffects",
                typeof(DragDropEffects),
                typeof(DragDropTarget<TItemsControlType, TItemContainerType>),
                new PropertyMetadata(DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Scroll));
        #endregion public DragDropEffects AllowedSourceEffects


        /// <summary>
        /// Initializes a new instance of the DragDropTarget class.
        /// </summary>
        protected DragDropTarget()
        {
#if MIGRATION
            this.MouseLeftButtonDown += DragDropTarget_MouseLeftButtonDown;
#else
            this.PointerPressed += DragDropTarget_PointerPressed;
#endif
        }


#if MIGRATION
        void DragDropTarget_MouseLeftButtonDown(object sender, Input.MouseButtonEventArgs e)
#else
        private void DragDropTarget_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            //----------------------------------
            // DRAG OPERATION STARTS HERE
            //----------------------------------

#if MIGRATION
            // Add mouse events to the window to enable dropping on any other elements
            this.INTERNAL_ParentWindow.MouseMove -= DragDropTarget_MouseMove;
            this.INTERNAL_ParentWindow.MouseMove += DragDropTarget_MouseMove;
            this.INTERNAL_ParentWindow.MouseLeftButtonUp -= DragDropTarget_MouseLeftButtonUp;
            this.INTERNAL_ParentWindow.MouseLeftButtonUp += DragDropTarget_MouseLeftButtonUp;
#else
            this.INTERNAL_ParentWindow.PointerMoved -= DragDropTarget_PointerMoved;
            this.INTERNAL_ParentWindow.PointerMoved += DragDropTarget_PointerMoved;
            this.INTERNAL_ParentWindow.PointerReleased -= DragDropTarget_PointerReleased;
            this.INTERNAL_ParentWindow.PointerReleased += DragDropTarget_PointerReleased;
#endif

            // Prevent the PointerPressed event from bubbling up so that if there are two nested DragDropTargets, only the inner one will be dragged:
            e.Handled = true;

            // We verify that drag operation is not taking place, which can lead a case if we missed the pointer released event due to a glitch such as moving the mouse outside the browser and releasing
            if (!_isPointerCaptured)
            {
                // Reset some variables:
                _isDragCancelled = false;
                _previousdragDropTargetUnderPointer = null;

                // Remember the current pointer position:
#if MIGRATION
                _pointerX = e.GetPosition(null).X;
                _pointerY = e.GetPosition(null).Y;
#else
                _pointerX = e.GetCurrentPoint(null).Position.X;
                _pointerY = e.GetCurrentPoint(null).Position.Y;
#endif
                // Get the source DragDropTarget element that is under the pointer, if any:
                DragDropTarget<TItemsControlType, TItemContainerType> sourceDragDropTarget = GetDragDropTargetUnderPointer(_pointerX, _pointerY, out _sourceItemContainer, out _);
                if (sourceDragDropTarget is DragDropTarget<TItemsControlType, TItemContainerType> && sourceDragDropTarget != this)
                    throw new Exception("The DragDropTarget is not supposed to support dragging an outer DragDropTarget in case of nested DragDropTargets.");

                // We do something only if the source exists (ie. if an item was found under the pointer):
                if (_sourceItemContainer != null)
                {
                    // Get a reference to the ItemsControl:
                    if (_sourceItemContainer is TreeViewItem treeViewItem)
                    {
                        _sourceItemsControl = (TItemsControlType)(treeViewItem.ParentTreeViewItem as object) ??
                            (TItemsControlType)(treeViewItem.ParentTreeView as object);
                    }
                    else
                    {
                        _sourceItemsControl = (TItemsControlType)this.Content; // Note: there is no risk of InvalidCastException because the type has been tested before, and the derived class (PanelDragDropTarget) also verifies the type in the "OnContentChanged" method.
                    }


#if MIGRATION
                    this.ReleaseMouseCapture();
#else
                    this.ReleasePointerCapture();
#endif

                    // Remember that the pointer is currently captured:
                    _isPointerCaptured = true;
                    _capturedPointer = e.Pointer;
                }
            }
        }


#if MIGRATION
        void DragDropTarget_MouseMove(object sender, Input.MouseEventArgs e)
#else
        private void DragDropTarget_PointerMoved(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            //----------------------------------
            // POINTER MOVE
            //----------------------------------
            if (_isPointerCaptured)
            {
                if (_popup?.IsOpen != true)
                {
                    // Prepare the arguments of the "ItemDragStarting" event:
                    Selection selection = new Selection(_sourceItemContainer);
                    SelectionCollection selectionCollection = SelectionCollection.ToSelectionCollection(selection);
                    ItemDragEventArgs itemDragStartingEventArgs = new ItemDragEventArgs(selectionCollection);

                    // Raise the "ItemDragStarting" event:
                    if (ItemDragStarting != null)
                        ItemDragStarting(this, itemDragStartingEventArgs);

                    // Show the popup, unless the user has cancelled the drag operation:
                    if (itemDragStartingEventArgs.Handled && itemDragStartingEventArgs.Cancel)
                    {
                        //----------------------------------
                        // CANCELLED BY USER
                        //----------------------------------

                        if (_isPointerCaptured)
                        {
                            // Stop capturing the pointer:
                            _isPointerCaptured = false;
                        }
                    }
                    else
                    {
                        //----------------------------------
                        // SHOW POPUP
                        //----------------------------------
                        StackPanel stackPanelInPopUp = GeneratePopupContent(out _iconStop, out _iconArrow);
                        this._popup = new Popup()
                        {
                            Child = stackPanelInPopUp,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            VerticalContentAlignment = VerticalAlignment.Top,
                            IsHitTestVisible = false
                        };

                        // Set the popup position:
                        // Offset by 10 pixels to not trigger mouse events on the popup when dropping
                        this._popup.HorizontalOffset = this._pointerX + 10;
                        this._popup.VerticalOffset = this._pointerY + 10;

                        // Show the popup:
                        this._popup.IsOpen = true;
                    }
                }
                else
                {
                    // Calculate the delta of the movement:
#if MIGRATION
                    double horizontalChange = e.GetPosition(null).X - _pointerX;
                    double verticalChange = e.GetPosition(null).Y - _pointerY;
#else
                    double horizontalChange = e.GetCurrentPoint(null).Position.X - _pointerX;
                    double verticalChange = e.GetCurrentPoint(null).Position.Y - _pointerY;
#endif

                    // Remember the new pointer position:  
#if MIGRATION
                    _pointerX = e.GetPosition(null).X;
                    _pointerY = e.GetPosition(null).Y;
#else
                    _pointerX = e.GetCurrentPoint(null).Position.X;
                    _pointerY = e.GetCurrentPoint(null).Position.Y;
#endif

                    // Move the popup and may raise the events DragEnter, DragOver and DragLeave:
                    MovePopupAndRaiseEvents(horizontalChange, verticalChange);
                }
            }
        }

        private void MovePopupAndRaiseEvents(double horizontalChange, double verticalChange)
        {
#if !(BRIDGE && MIGRATION)

            if (_popup != null)
            {
                //popup moving with the pointer
                _popup.HorizontalOffset += horizontalChange;
                _popup.VerticalOffset += verticalChange;
            }

            // Prepare the "DragEventArgs" to pass to the raised events:
            Selection selection = new Selection(_sourceItemContainer);
            SelectionCollection selectionCollection = SelectionCollection.ToSelectionCollection(selection);
            MS.DataObject dataObject = new MS.DataObject();
            dataObject.SetData("ItemDragEventArgs", new ItemDragEventArgs(selectionCollection));
            MS.DragEventArgs dragOverEventArgs = new MS.DragEventArgs(dataObject);

            UIElement allowDropElementUnderPointer;
            // Get the DragDropTarget element that is under the pointer, if any:
            DragDropTarget<TItemsControlType, TItemContainerType> dragDropTargetUnderPointer = GetDragDropTargetUnderPointer(_pointerX, _pointerY, out _, out allowDropElementUnderPointer);

            // Check if the element under the pointer has changed since the last PointerMoved:
            if (_previousdragDropTargetUnderPointer != dragDropTargetUnderPointer)
            {
                //---------------------------------
                // IF WHAT IS UNDER THE POINTER HAS CHANGED
                //---------------------------------

                // Raise the DragLeave event of the element that was under the pointer before:
                if (_previousdragDropTargetUnderPointer != null && _previousdragDropTargetUnderPointer.DragLeave != null)
                {
                    _previousdragDropTargetUnderPointer.DragLeave(_previousdragDropTargetUnderPointer, new MS.DragEventArgs(dataObject));

                    // Reset the value of "_isDragCancelled" when leaving a control. This variable lets the user prevent a Drop on an element when the user sets e.Handled=true in the "DragOver" event of that element.
                    _isDragCancelled = false;
                }
                // Remember the element that is under the pointer:
                _previousdragDropTargetUnderPointer = dragDropTargetUnderPointer;

                // Raise the DragEnter event of the new element that is under the pointer:
                if (dragDropTargetUnderPointer != null && dragDropTargetUnderPointer.DragEnter != null)
                    dragDropTargetUnderPointer.DragEnter(dragDropTargetUnderPointer, new MS.DragEventArgs(dataObject));
            }

            if (dragDropTargetUnderPointer != null)
            {
                //---------------------------------
                // IF UNDER THE POINTER THERE IS A DRAGDROPPANEL
                //---------------------------------

                // Raise the DragOver:
                dragDropTargetUnderPointer.OnDragOver(dragOverEventArgs);

                // Cancel the drag drop operation if the user has marked the DragOver event as "handled":
                if (dragOverEventArgs.Handled && dragOverEventArgs.Effects == DragDropEffects.None)
                    _isDragCancelled = true;
            }

            // Show the appropriate icon depending on whether it is possible to drop or not:
            if ((dragDropTargetUnderPointer != null && dragDropTargetUnderPointer.AllowDrop ||
                allowDropElementUnderPointer != null)
                && !_isDragCancelled)
            {
                //---------------------------------
                // SHOW ICON "DRAG ALLOWED"
                //---------------------------------
                _iconArrow.Visibility = Visibility.Visible;
                _iconStop.Visibility = Visibility.Collapsed;
            }
            else
            {
                //---------------------------------
                // SHOW ICON "DRAG FORBIDDEN"
                //---------------------------------
                _iconArrow.Visibility = Visibility.Collapsed;
                _iconStop.Visibility = Visibility.Visible;
            }
#endif
        }

#if MIGRATION
        void DragDropTarget_MouseLeftButtonUp(object sender, Input.MouseButtonEventArgs e)
#else
        void DragDropTarget_PointerReleased(object sender, Input.PointerRoutedEventArgs e)
#endif
        {
            //----------------------------------
            // POINTER RELEASED
            //----------------------------------
#if MIGRATION
            this.INTERNAL_ParentWindow.MouseMove -= DragDropTarget_MouseMove;
            this.INTERNAL_ParentWindow.MouseLeftButtonUp -= DragDropTarget_MouseLeftButtonUp;
#else
            this.INTERNAL_ParentWindow.PointerMoved -= DragDropTarget_PointerMoved;
            this.INTERNAL_ParentWindow.PointerReleased -= DragDropTarget_PointerReleased;
#endif

            // Remember the new pointer position:  
#if MIGRATION
            _pointerX = e.GetPosition(null).X;
            _pointerY = e.GetPosition(null).Y;
#else
            _pointerX = e.GetCurrentPoint(null).Position.X;
            _pointerY = e.GetCurrentPoint(null).Position.Y;
#endif

            if (_isPointerCaptured && e.Pointer == _capturedPointer && _popup?.IsOpen == true)
            {
                // We call MovePopupAndRaiseEvents(0,0) to prevent fast click, move and release of the mouse doing unwanted behavior (bug in case that the buttonup event was triggered before mousemove event)
                MovePopupAndRaiseEvents(0, 0);

                // Stop capturing the pointer:
                _isPointerCaptured = false;

                // Handle the drop:
                OnDropped(e);
            }
        }

#if MIGRATION
        void OnDropped(MouseButtonEventArgs e)
#else
        void OnDropped(PointerRoutedEventArgs e)
#endif
        {

            _popup.IsOpen = false;

            //We no longer have use for the popup
            _popup.Child = null;
            _popup = null;

            // Prepare the event arguments:
            Selection selection = new Selection(ItemFromContainer(_sourceItemsControl, _sourceItemContainer));
            SelectionCollection selectionCollection = SelectionCollection.ToSelectionCollection(selection);

            UIElement allowDropElementUnderPointer;
            // Get the DragDropTarget element that is under the pointer, if any:
            DragDropTarget<TItemsControlType, TItemContainerType> dragDropTargetUnderPointer = GetDragDropTargetUnderPointer(_pointerX, _pointerY, out _, out allowDropElementUnderPointer);

            bool moveItem = false;
            MS.DragEventArgs dragArgs = null;
            if (dragDropTargetUnderPointer != null)
            {
                //---------------------------------
                // IF A DROP TARGET IS FOUND
                //---------------------------------

                // Check if drop is allowed:
                if (dragDropTargetUnderPointer.AllowDrop && !_isDragCancelled)
                {
                    TItemsControlType targetContainer = (TItemsControlType)dragDropTargetUnderPointer.Content; // Note: a "container" is for example a Panel in case of PanekDragDropTarget.

                    // Check if we are dropping on the source element itself (ie. from the source to the placeholder that we put in place of the source) (cf. "ItemDropOnSource" event):
                    if (dragDropTargetUnderPointer.GetItemCount(targetContainer) > 0
                        && ContainerFromIndex(targetContainer, _indexOfSourceContainerWithinItemsControl) == _sourcePlaceholder)
                    {
                        //---------------------------------
                        // IF WE ARE DROPPING THE SOURCE ON ITSELF
                        //---------------------------------

                        //Raise the "ItemDroppedOnSource" Event
                        if (dragDropTargetUnderPointer.ItemDroppedOnSource != null)
                        {
                            // Prepare the event args:
                            MS.DataObject dataObject = new MS.DataObject();
                            dataObject.SetData("ItemDragEventArgs", new ItemDragEventArgs(selectionCollection));

#if !(BRIDGE && MIGRATION)
                            dragDropTargetUnderPointer.ItemDroppedOnSource(dragDropTargetUnderPointer, new MS.DragEventArgs(dataObject, e));
#endif
                        }
                    }
                    //else we drop on another DragDropTarget
                    else
                    {
                        //---------------------------------
                        // IF WE ARE DROPPING ON ANOTHER DRAGDROPTARGET
                        //---------------------------------

                        // Raise the Drop event:
                        if (dragDropTargetUnderPointer.Drop != null)
                        {
                            // Prepare the event args:
                            MS.DataObject dataObject = new MS.DataObject();
                            dataObject.SetData("ItemDragEventArgs", new ItemDragEventArgs(selectionCollection));

                            // Raise the Drop event:
#if !(BRIDGE && MIGRATION)
                            dragArgs = new MS.DragEventArgs(dataObject, e);
                            dragDropTargetUnderPointer.Drop(this, dragArgs);
#endif
                        }

                        moveItem = true;
                    }
                }
            }

            // The event is triggered in Silverlight regardless of whether the target has AllowDrop=true
            ItemDroppedOnTarget?.Invoke(this, new ItemDragEventArgs(selectionCollection));

            if (dragArgs is null || !dragArgs.Handled)
            {
                if (moveItem)
                {
                    object item = _sourceItemContainer.GetValue(ItemContainerGenerator.ItemForItemContainerProperty);
                    RemoveItem(_sourceItemsControl, item);
                    // Put the source into the target:
                    dragDropTargetUnderPointer.AddItem((TItemsControlType)dragDropTargetUnderPointer.Content, item);
                }
                else if (allowDropElementUnderPointer != null)
                {
                    RemoveItem(_sourceItemsControl,
                        _sourceItemContainer.GetValue(ItemContainerGenerator.ItemForItemContainerProperty));
                }
                if (dragArgs != null)
                {
                    dragArgs.Handled = true;
                }
            }

            // Raise the "ItemDragCompleted" event:
            if (ItemDragCompleted != null)
                ItemDragCompleted(this, new ItemDragEventArgs(selectionCollection));
        }

        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        /// <param name="dragOverEventArgs">Information about the event.</param>
        protected virtual void OnDragOver(MS.DragEventArgs dragOverEventArgs)
        {
            if (this.DragOver != null)
            {
                this.DragOver(this, dragOverEventArgs);
            }

        }


        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

#if OPENSILVER
            if (true)
#elif BRIDGE
            if (!CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                // Prevent the selection of text while dragging from the DragDropTarget
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.onselectstart = function() { return false; }", this.INTERNAL_OuterDomElement);
            }
        }


#region Methods for sub-class to implement

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected abstract void InsertItem(TItemsControlType itemsControl, int index, object data);

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected abstract void AddItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected abstract void RemoveItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Removes an item from an items control by index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index of the item to be removed.</param>
        protected virtual void RemoveItemAtIndex(TItemsControlType itemsControl, int index)
        {
            throw new InvalidOperationException("Removal by index is not supported");
        }

        // TODO: Make int? to accomodate possibility that there is no concept of items?

        /// <summary>
        /// Gets the number of items in an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The number of items in the items control.</returns>
        protected abstract int GetItemCount(TItemsControlType itemsControl);

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected abstract TItemContainerType ContainerFromIndex(TItemsControlType itemsControl, int index);

        /// <summary>
        /// Retrieves the items host for a given items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The items host for a given items control.</returns>
        protected abstract Panel GetItemsHost(TItemsControlType itemsControl);

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected abstract int? IndexFromContainer(TItemsControlType itemsControl, TItemContainerType itemContainer);

        /// <summary>
        /// Gets the item from an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The data contained by the item container.</returns>
        protected abstract object ItemFromContainer(TItemsControlType itemsControl, TItemContainerType itemContainer);

        /// <summary>
        /// Returns a value indicating whether an item can be removed from the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>A value indicating whether an item can be removed from the
        /// items control.</returns>
        protected abstract bool CanRemove(TItemsControlType itemsControl);

        /// <summary>
        /// Returns a value indicating whether an item can be added to the
        /// items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be added.</param>
        /// <returns>A value indicating whether an item can be added to the
        /// items control.</returns>
        protected abstract bool CanAddItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Returns a value indicating whether a container belongs to an items 
        /// control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>A value indicating whether a container belongs to an items 
        /// control.</returns>
        protected abstract bool IsItemContainerOfItemsControl(TItemsControlType itemsControl, DependencyObject itemContainer);

        /// <summary>
        /// Returns the items control ancestor of a dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object to retrieve the
        /// element for.</param>
        /// <returns>The items control ancestor of the dependency object.
        /// </returns>
        protected virtual TItemsControlType GetItemsControlAncestor(DependencyObject dependencyObject)
        {
            return (TItemsControlType)this.Content;
        }

        /// <summary>
        /// Returns the item container ancestor of a dependency object.
        /// </summary>
        /// <param name="itemsControl">The items control that contains the
        /// item container.</param>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>The item container ancestor of the dependency object.
        /// </returns>
        public virtual TItemContainerType GetItemContainerAncestor(TItemsControlType itemsControl, DependencyObject dependencyObject)
        {
            return
                dependencyObject
                    .GetVisualAncestors()
                    .Prepend(dependencyObject)
                    .OfType<TItemContainerType>()
                    .Where(ancestor => IsItemContainerOfItemsControl(itemsControl, ancestor))
                    .FirstOrDefault();
        }

        /// <summary>
        /// Returns a value indicating whether a given items control
        /// can scroll.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>The value indicating whether the given items control
        /// can scroll.</returns>
        protected virtual bool CanScroll(TItemsControlType itemsControl)
        {
            return false;
        }

        /// <summary>
        /// Scrolls a given item container into the view.
        /// </summary>
        /// <param name="itemsControl">The items control that contains
        /// the item container.</param>
        /// <param name="itemContainer">The item container to scroll into
        /// view.</param>
        protected virtual void ScrollIntoView(TItemsControlType itemsControl, TItemContainerType itemContainer)
        {
        }

        /// <summary>
        /// Retrieves the drop target of a drag event.
        /// </summary>
        /// <param name="args">Information about the drag event.</param>
        /// <returns>The drop target of a drag event.</returns>
        protected virtual TItemsControlType GetDropTarget(DragEventArgs args)
        {
            return GetItemsControlAncestor((DependencyObject)args.OriginalSource);
        }

        /// <summary>
        /// Create a new TItemsControlType
        /// </summary>
        /// <returns>A new TItemsControlType</returns>
        protected abstract TItemsControlType INTERNAL_ReturnNewTItemsControl();

        #endregion Methods for sub-class to implement


        #region Events

        /// <summary>
        /// An event raised when a drag operation is starting on an item.
        /// </summary>
        public event EventHandler<ItemDragEventArgs> ItemDragStarting;

        /// <summary>
        /// Fires when the DragDropTarget control loses mouse capture.
        /// </summary>
        public event EventHandler<ItemDragEventArgs> ItemDragCompleted;
        /// <summary>
        /// Raises the DragEnter event.
        /// </summary>
        public event MS.DragEventHandler DragEnter;
        /// <summary>
        /// Raises the DragLeave event.
        /// </summary>
        public event MS.DragEventHandler DragLeave;
        /// <summary>
        /// Raises the DragOver event.
        /// </summary>
        public event MS.DragEventHandler DragOver;
        /// <summary>
        /// Raises the Drop event
        /// </summary>
        public event MS.DragEventHandler Drop;
        /// <summary>
        /// Raises the ItemDroppedOnSource event
        /// </summary>
        public event MS.DragEventHandler ItemDroppedOnSource;
        /// <summary>
        /// Raises the ItemDroppedOnTarget event
        /// </summary>
        public event EventHandler<ItemDragEventArgs> ItemDroppedOnTarget;

#endregion


#region Private helper methods
        /// <summary>
        /// This method returns null if no DragDropTarget is under the pointer, else it returns the DragDropTarget under it (the first Parent found)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="itemContainerUnderPointer">The ListBoxItem or other container that is being moved. If there is no container, it is the item directly.</param>
        /// <returns>Null if no DragDropTarget is under the pointer, else it returns the DragDropTarget under it (the first Parent found)</returns>
        static DragDropTarget<TItemsControlType, TItemContainerType> GetDragDropTargetUnderPointer(double x, double y, out TItemContainerType itemContainerUnderPointer, out UIElement allowDropElementUnderPointer)
        {
            UIElement element = VisualTreeHelper.FindElementInHostCoordinates(new Point(x, y));
            List<object> ElementsBetweenClickedElementAndDragDropTarget = new List<object>(); //This list will contain all the elements we go through when going from the clicked element to the DragDropTarget (both included)
            DragDropTarget<TItemsControlType, TItemContainerType> dragDropTargetUnder = null;
            TItemsControlType itemsControl = null;
            allowDropElementUnderPointer = null;
            // 1) Walk up the visual tree from the clicked element until we find the DragDropTarget:
            while (element != null)
            {
                ElementsBetweenClickedElementAndDragDropTarget.Add(element);
                if (element is TItemsControlType ic)
                {
                    itemsControl = ic;
                }
                if (allowDropElementUnderPointer == null && element.AllowDrop)
                {
                    allowDropElementUnderPointer = element;
                }
                if (element is DragDropTarget<TItemsControlType, TItemContainerType>)
                {
                    //------------------
                    // FOUND
                    //------------------
                    dragDropTargetUnder = (DragDropTarget<TItemsControlType, TItemContainerType>)element;
                    break;
                }

                //Move up to the parent
                element = (UIElement)element.INTERNAL_VisualParent;
            }

            if (dragDropTargetUnder != null && itemsControl != null)
            {
                itemContainerUnderPointer = dragDropTargetUnder.INTERNAL_GetDeepestItemContainer(
                    itemsControl, ElementsBetweenClickedElementAndDragDropTarget);
                return dragDropTargetUnder;
            }
            else
            {
                // Not found:
                itemContainerUnderPointer = null;
                return null;
            }
        }

        /// <summary>
        /// Gets the item container of the expected type by this DragDropTarget, from a list of elements
        /// </summary>
        /// <param name="itemsControl">The ItemsControl instance hosted by this DragDropTarget.</param>
        /// <param name="elementsFromDeepestToRoot">List of elements to search in order from deepest to the root.</param>
        /// <returns>The item container of type TItemContainerType if found, null otherwise.</returns>
        internal virtual TItemContainerType INTERNAL_GetDeepestItemContainer(TItemsControlType itemsControl,
            List<object> elementsFromDeepestToRoot)
        {
            // Traverse in inverse order to get deepest item container
            foreach (object element in elementsFromDeepestToRoot)
            {
                if (element is TItemContainerType containerElement &&
                    (IndexFromContainer(itemsControl, containerElement) ?? -1) > -1)
                {
                    return containerElement;
                }
            }
            return null;
        }

        static StackPanel GeneratePopupContent(out UIElement iconStop, out UIElement iconArrow)
        {
            StackPanel stackPanelInPopUp = new StackPanel();
            iconStop = new Path()
            {
                Data = (Geometry)TypeFromStringConverters.ConvertFromInvariantString(typeof(Geometry), @"M13.4005,19.7645 L13.3231,19.8886 C11.5316,22.8369 10.5,26.298 10.5,30 C10.5,40.7696 19.2305,49.5 30,49.5 C33.702,49.5 37.1631,48.4684 40.1115,46.6769 L40.2355,46.5995 z M30,10.5 C26.298,10.5 22.8369,11.5316 19.8886,13.3231 L19.7645,13.4005 L46.5995,40.2355 L46.6769,40.1115 C48.4684,37.1631 49.5,33.702 49.5,30 C49.5,19.2305 40.7696,10.5 30,10.5 z M30,0.5 C46.2924,0.5 59.5,13.7076 59.5,30 C59.5,46.2924 46.2924,59.5 30,59.5 C13.7076,59.5 0.5,46.2924 0.5,30 C0.5,13.7076 13.7076,0.5 30,0.5 z"),
                Fill = new SolidColorBrush(Colors.Red),
                Height = 10,
                Width = 10,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            iconArrow = new Path()
            {
                Data = (Geometry)TypeFromStringConverters.ConvertFromInvariantString(typeof(Geometry), @"M120,60 L120,80 L160,80 L160,90 L180,70 L160,50 L160,60 z"),
                Fill = new SolidColorBrush(Colors.Blue),
                Height = 7.057,
                Width = 10.5,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            iconArrow.Visibility = Visibility.Collapsed;
            stackPanelInPopUp.Orientation = Orientation.Horizontal;

            stackPanelInPopUp.Children.Add(iconStop);
            stackPanelInPopUp.Children.Add(iconArrow);
            return stackPanelInPopUp;
        }

        private void InsertContainer(TItemsControlType itemsControl, int index, UIElement container)
        {
            (itemsControl as ItemsControl).GetItemsHost().Children.Insert(index, container);
        }

        private void ReplaceContainer(TItemsControlType itemsControl, int index, UIElement container)
        {
            (itemsControl as ItemsControl).GetItemsHost().Children.RemoveAt(index);
            (itemsControl as ItemsControl).GetItemsHost().Children.Insert(index, container);
        }

        private void RemoveContainer(TItemsControlType itemsControl, int index)
        {
            (itemsControl as ItemsControl).GetItemsHost().Children.RemoveAt(index);
        }
#endregion
    }
}
