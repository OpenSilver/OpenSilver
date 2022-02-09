

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
#if unsupported
                new PropertyMetadata(DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Scroll));
#else
                new PropertyMetadata(DragDropEffects.None));
#endif
        #endregion public DragDropEffects AllowedSourceEffects


        /// <summary>
        /// Initializes a new instance of the DragDropTarget class.
        /// </summary>
        protected DragDropTarget()
        {
#if MIGRATION
            this.MouseLeftButtonDown += DragDropTarget_MouseLeftButtonDown;
            this.MouseMove += DragDropTarget_MouseMove;
            this.MouseLeftButtonUp += DragDropTarget_MouseLeftButtonUp;
#else
            this.PointerPressed += DragDropTarget_PointerPressed;
            this.PointerMoved += DragDropTarget_PointerMoved;
            this.PointerReleased += DragDropTarget_PointerReleased;
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
                DragDropTarget<TItemsControlType, TItemContainerType> sourceDragDropTarget = GetDragDropTargetUnderPointer(_pointerX, _pointerY, out _sourceItemContainer);
                if (sourceDragDropTarget != this)
                    throw new Exception("The DragDropTarget is not supposed to support dragging an outer DragDropTarget in case of nested DragDropTargets.");

                // We do something only if the source exists (ie. if an item was found under the pointer):
                if (_sourceItemContainer != null)
                {
                    // Get a reference to the ItemsControl:
                    _sourceItemsControl = (TItemsControlType)this.Content; // Note: there is no risk of InvalidCastException because the type has been tested before, and the derived class (PanelDragDropTarget) also verifies the type in the "OnContentChanged" method.

                    // Capture the pointer so that when dragged outside the DragDropPanel, we can still get its position:
    #if MIGRATION
                    this.CaptureMouse();
    #else
                    this.CapturePointer(e.Pointer);
    #endif
                    // Remember that the pointer is currently captured:
                    _isPointerCaptured = true;
                    _capturedPointer = e.Pointer;

                    //Size of the content
                    double height;
                    double width;
                    if (_sourceItemContainer is FrameworkElement)
                    {
                        Size actualSize = (_sourceItemContainer as FrameworkElement).INTERNAL_GetActualWidthAndHeight();
                        height = actualSize.Height;
                        width = actualSize.Width;
                    }
                    else
                    {
                        height = double.NaN;
                        width = double.NaN;
                    }

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

#if MIGRATION
                            this.ReleaseMouseCapture();
#else
                            this.ReleasePointerCapture(_capturedPointer);
#endif
                        }
                    }
                    else
                    {
                        //----------------------------------
                        // SHOW POPUP
                        //----------------------------------

                        // Put a placeholder in place of the source that will occupy the same space. This is useful to: 1) let the user drop over the source itself (cf. "ItemDroppedOnSource" event), and 2) prevent the other elements from being displaced during the drag operation.
                        RemoveSourceAndPutTransparentPlaceholderInPlace(height, width);

                        // Put the source into a popup:
                        StackPanel stackPanelInPopUp = GeneratePopupContent(_sourceItemContainer, out _iconStop, out _iconArrow);
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
                        this._popup.HorizontalOffset = this._pointerX;
                        this._popup.VerticalOffset = this._pointerY;

                        // Show the popup:
                        this._popup.IsOpen = true;
                    }
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

            // Get the DragDropTarget element that is under the pointer, if any:
            TItemContainerType targetItemContainer;
            DragDropTarget<TItemsControlType, TItemContainerType> dragDropTargetUnderPointer = GetDragDropTargetUnderPointer(_pointerX, _pointerY, out targetItemContainer);

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
            if (dragDropTargetUnderPointer != null && dragDropTargetUnderPointer.AllowDrop && !_isDragCancelled)
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

            // Remember the new pointer position:  
#if MIGRATION
            _pointerX = e.GetPosition(null).X;
            _pointerY = e.GetPosition(null).Y;
#else
            _pointerX = e.GetCurrentPoint(null).Position.X;
            _pointerY = e.GetCurrentPoint(null).Position.Y;
#endif

            if (_isPointerCaptured && e.Pointer == _capturedPointer)
            {
                // We call MovePopupAndRaiseEvents(0,0) to prevent fast click, move and release of the mouse doing unwanted behavior (bug in case that the buttonup event was triggered before mousemove event)
                MovePopupAndRaiseEvents(0, 0);

                // Stop capturing the pointer:
                _isPointerCaptured = false;

#if MIGRATION
                this.ReleaseMouseCapture();
#else
                this.ReleasePointerCapture(_capturedPointer);
#endif
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
            Selection selection = new Selection(_sourceItemContainer);
            SelectionCollection selectionCollection = SelectionCollection.ToSelectionCollection(selection);

            // Get the DragDropTarget element that is under the pointer, if any:
            TItemContainerType targetItemContainer;
            DragDropTarget<TItemsControlType, TItemContainerType> dragDropTargetUnderPointer = GetDragDropTargetUnderPointer(_pointerX, _pointerY, out targetItemContainer);

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
                    if (ContainerFromIndex(targetContainer, _indexOfSourceContainerWithinItemsControl) == _sourcePlaceholder)
                    {
                        //---------------------------------
                        // IF WE ARE DROPPING THE SOURCE ON ITSELF
                        //---------------------------------

                        // Put the dragged element back to where it was:
                        PutSourceBackToOriginalPlace();

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

                        // Remove the temporary placeholder (see the comment where the placeholder is defined):
                        this.RemoveItemAtIndex(_sourceItemsControl, _indexOfSourceContainerWithinItemsControl);

                        // Raise the Drop event:
                        if (dragDropTargetUnderPointer.Drop != null)
                        {
                            // Prepare the event args:
                            MS.DataObject dataObject = new MS.DataObject();
                            dataObject.SetData("ItemDragEventArgs", new ItemDragEventArgs(selectionCollection));

                            // Raise the Drop event:
#if !(BRIDGE && MIGRATION)
                            dragDropTargetUnderPointer.Drop(dragDropTargetUnderPointer, new MS.DragEventArgs(dataObject, e));
#endif
                        }

                        // Put the source into the target:
                        dragDropTargetUnderPointer.AddItem((TItemsControlType)dragDropTargetUnderPointer.Content, _sourceItemContainer);
                    }
                }
                //Can't drop so we put what was in the popup back in the content
                else
                {
                    // Put the dragged element back to where it was:
                    PutSourceBackToOriginalPlace();
                }
            }
            //not a DragDropTarget under the pointer so we put what was in the popup back in the content
            else
            {
                // Put the dragged element back to where it was:
                PutSourceBackToOriginalPlace();
            }

            // Raise the "ItemDragCompleted" event:
            if (ItemDragCompleted != null)
                ItemDragCompleted(this, new ItemDragEventArgs(selectionCollection));
        }

        void PutSourceBackToOriginalPlace()
        {
            // Remove the temporary placeholder (see the comment where the placeholder is defined):
            this.RemoveItemAtIndex(_sourceItemsControl, _indexOfSourceContainerWithinItemsControl);

            // Put the dragged element back to where it was:
            this.InsertItem(_sourceItemsControl, _indexOfSourceContainerWithinItemsControl, _sourceItemContainer);
        }

        void RemoveSourceAndPutTransparentPlaceholderInPlace(double sourceHeight, double sourceWidth)
        {
            // Find at which index the source is located in its ItemsControl:
            _indexOfSourceContainerWithinItemsControl = (int)this.IndexFromContainer(_sourceItemsControl, _sourceItemContainer);

            // Remove the source from its original location (we will put it back at the end of the drag operation):
            this.RemoveItem(_sourceItemsControl, _sourceItemContainer);

            // We use a border as a placeholder for the source while the source is being dragged in the popup:
            _sourcePlaceholder = new Border()
            {
                Background = new SolidColorBrush(Colors.Transparent) // Note: we set the background to "Transparent" in order to catch the pointer events (this is useful for example when dragging the source on itself, cf. "ItemDroppedOnSource" event
            };

            // Set the palceholder size to be the same as the space previously occupied by the source:
            if (!double.IsNaN(sourceHeight) && !double.IsNaN(sourceWidth))
            {
                _sourcePlaceholder.Height = sourceHeight;
                _sourcePlaceholder.Width = sourceWidth;
            }

            // Put the placeholder where the source was originally located:
            this.InsertItem(_sourceItemsControl, _indexOfSourceContainerWithinItemsControl, _sourcePlaceholder);
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
            if (false)
#elif BRIDGE
            if (!CSHTML5.Interop.IsRunningInTheSimulator)
#endif
            {
                // Prevent the selection of text while dragging from the DragDropTarget
                CSHTML5.Interop.ExecuteJavaScriptAsync("$0.onselectstart = function() { return false; }", this.INTERNAL_OuterDomElement);
            }
        }


#region Abstract methods

        /// <summary>
        /// Retrieves the item container at a given index.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to retrieve the container.
        /// </param>
        /// <returns>The item container at a given index.</returns>
        protected abstract UIElement ContainerFromIndex(TItemsControlType itemsControl, int index);

        /// <summary>
        /// Adds an item to an items control.
        /// </summary>
        /// <param name="control">The items control.</param>
        /// <param name="data">The data to be inserted.</param>
        protected abstract void AddItem(TItemsControlType control, object data);

        /// <summary>
        /// Create a new TItemsControlType
        /// </summary>
        /// <returns>A new TItemsControlType</returns>
        protected abstract TItemsControlType INTERNAL_ReturnNewTItemsControl();

        /// <summary>
        /// Inserts an item into an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to insert the item.</param>
        /// <param name="data">The data to be inserted.</param>
        protected abstract void InsertItem(TItemsControlType itemsControl, int index, object data);

        /// <summary>
        /// Retrieves the index of an item container.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="itemContainer">The item container.</param>
        /// <returns>The index of an item container.</returns>
        protected abstract int? IndexFromContainer(TItemsControlType itemsControl, UIElement itemContainer);

        /// <summary>
        /// Removes an item from an items control.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="data">The data to be removed.</param>
        protected abstract void RemoveItem(TItemsControlType itemsControl, object data);

        /// <summary>
        /// Removes data from an ItemsControl.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <param name="index">The index at which to remove an item.</param>
        protected abstract void RemoveItemAtIndex(TItemsControlType itemsControl, int index);

#endregion


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
        [OpenSilver.NotImplemented]
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
        static DragDropTarget<TItemsControlType, TItemContainerType> GetDragDropTargetUnderPointer(double x, double y, out TItemContainerType itemContainerUnderPointer)
        {
            UIElement element = VisualTreeHelper.FindElementInHostCoordinates(new Point(x, y));
            List<object> ElementsBetweenClickedElementAndDragDropTarget = new List<object>(); //This list will contain all the elements we go through when going from the clicked element to the DragDropTarget (both included)
            DragDropTarget<TItemsControlType, TItemContainerType> dragDropTargetUnder = null;
            // 1) Walk up the visual tree from the clicked element until we find the DragDropTarget:
            while (element != null)
            {
                ElementsBetweenClickedElementAndDragDropTarget.Add(element);
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

            if (dragDropTargetUnder != null)
            {
                //We found the DragDropTarget:
                // 2) Find the item to move from the list of the children of the DragDropTarget
                int indexOfLastElementInList = ElementsBetweenClickedElementAndDragDropTarget.Count - 1;
                int amoutOfElementsBetweenItemsRootAndDragDropTarget = dragDropTargetUnder.INTERNAL_GetNumberOfElementsBetweenItemsRootAndDragDropTarget();
                if(indexOfLastElementInList < amoutOfElementsBetweenItemsRootAndDragDropTarget) //Note: this can happen while dragging: the element under the pointer can be closer to the DragDropTarget than the root of the item we are dragging.
                {
                    itemContainerUnderPointer = null;
                    return dragDropTargetUnder;
                }
                object elementToMove = ElementsBetweenClickedElementAndDragDropTarget.ElementAt(indexOfLastElementInList - amoutOfElementsBetweenItemsRootAndDragDropTarget);
                itemContainerUnderPointer = (TItemContainerType)elementToMove;
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
        /// Returns the amount of times we have to get the parent of the Root of an Item to reach the DragDropTarget.
        /// </summary>
        /// <returns>The amount of times we have to get the parent of the Root of an Item to have the DragDropTarget.</returns>
        internal abstract int INTERNAL_GetNumberOfElementsBetweenItemsRootAndDragDropTarget();

        static StackPanel GeneratePopupContent(UIElement sourceItemContainer, out UIElement iconStop, out UIElement iconArrow)
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
            stackPanelInPopUp.Children.Add(sourceItemContainer);
            stackPanelInPopUp.Children.Add(iconStop);
            stackPanelInPopUp.Children.Add(iconArrow);
            return stackPanelInPopUp;
        }

#endregion
    }
}
