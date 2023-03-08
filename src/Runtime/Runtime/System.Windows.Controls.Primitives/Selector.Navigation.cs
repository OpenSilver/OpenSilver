
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
using System.Diagnostics;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Key = Windows.System.VirtualKey;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives;
#else
namespace Windows.UI.Xaml.Controls.Primitives;
#endif

public partial class Selector
{
    /// <summary> 
    /// Tracks the index of the focused element.
    /// </summary>
    private int _focusedIndex = -1;

    /// <summary>
    /// The item corresponding to the UI container which has focus.
    /// Virtualizing panels remove visual children you can't see.
    /// When you scroll the focused element out of view we throw
    /// focus back on to the items control and remember the item which
    /// was focused.  When it scrolls back into view (and focus is
    /// still on the ItemsControl) we'll focus it.
    /// </summary>
    internal int FocusedIndex => _focusedIndex;

    internal override bool FocusItem(ItemInfo info)
    {
        if (info.Index == -1 || info.Index >= Items.Count)
        {
            return false;
        }

        ScrollIntoViewImpl(info.Index);

        return FocusItemInternal(info.Index);
    }

    internal bool FocusItemInternal(int index)
    {
        bool focused = false;
        if (index >= 0 && index < Items.Count)
        {
            _focusedIndex = index;
            if (ItemContainerGenerator.ContainerFromIndex(index) is ListBoxItem listBoxItem)
            {
                focused = listBoxItem.Focus();
            }
        }
        return focused;
    }

    /// <summary> 
    /// Called by ListBoxItem instances when they get focus
    /// </summary> 
    /// <param name="listBoxItemNewFocus">ListBoxItem that got focus</param>
    internal void NotifyListItemGotFocus(ListBoxItem listBoxItemNewFocus)
    {
        // Track the focused index 
        _focusedIndex = ItemContainerGenerator.IndexFromContainer(listBoxItemNewFocus);
    }

    /// <summary>
    /// Called by ListBoxItem instances when they lose focus 
    /// </summary>
    /// <param name="listBoxItemOldFocus">ListBoxItem that lost focus</param>
    internal void NotifyListItemLostFocus(ListBoxItem listBoxItemOldFocus)
    {
        // Stop tracking state
        _focusedIndex = -1;
    }

    /// <summary>
    /// Call ElementScrollViewer.ScrollInDirection if possible. 
    /// </summary>
    /// <param name="key">Key corresponding to the direction.</param>
    internal void ElementScrollViewerScrollInDirection(Key key)
    {
        ScrollHost?.ScrollInDirection(key);
    }

    /// <summary>
    /// Indicate whether the orientation of the Selector's items is vertical.
    /// </summary> 
    /// <returns>True if the orientation is vertical; false otherwise.</returns>
    internal bool IsVerticalOrientation()
    {
        return ItemsHost is null || ItemsHost.LogicalOrientation == Orientation.Vertical;
    }
    
    internal int NavigateToStart()
    {
        int newFocusedIndex = -1;
        if (Items.Count > 0)
        {
            newFocusedIndex = 0;
            if (newFocusedIndex != _focusedIndex)
            {
                ScrollIntoViewImpl(newFocusedIndex);
                ScrollHost?.UpdateLayout();
            }
        }

        return newFocusedIndex;
    }

    internal int NavigateToEnd()
    {
        int newFocusedIndex = -1;
        if (Items.Count > 0)
        {
            newFocusedIndex = Items.Count - 1;
            if (newFocusedIndex != _focusedIndex)
            {
                ScrollIntoViewImpl(newFocusedIndex);
                ScrollHost?.UpdateLayout();
            }
        }

        return newFocusedIndex;
    }

    /// <summary> 
    /// Move the focus forward/backward one page.
    /// </summary>
    /// <param name="forward">Forward if true; backward otherwise</param> 
    /// <returns>New focused index.</returns>
    /// <remarks>Similar to WPF's corresponding ItemsControl method.</remarks>
    internal int NavigateByPage(bool forward)
    {
        int newFocusedIndex = -1;
        // Get it visible to start with
        if (_focusedIndex != -1 && !IsOnCurrentPage(_focusedIndex))
        {
            ScrollIntoViewImpl(_focusedIndex);
            ScrollHost?.UpdateLayout();
        }
        // Inlined implementation of NavigateByPageInternal
        if (_focusedIndex == -1)
        {
            // Select something
            newFocusedIndex = GetFirstItemOnCurrentPage(_focusedIndex, forward);
        }
        else
        {
            int firstItemOnCurrentPage = GetFirstItemOnCurrentPage(_focusedIndex, forward);
            if (firstItemOnCurrentPage != _focusedIndex)
            {
                // Select the "edge" element 
                newFocusedIndex = firstItemOnCurrentPage;
            }
            else
            {
                ScrollViewer scrollHost = ScrollHost;
                if (scrollHost != null)
                {
                    // Scroll a page in the relevant direction
                    if (IsVerticalOrientation())
                    {
                        scrollHost.ScrollToVerticalOffset(
                            Math.Max(0, Math.Min(scrollHost.ScrollableHeight,
                            scrollHost.VerticalOffset + (scrollHost.ViewportHeight * (forward ? 1 : -1)))));
                    }
                    else
                    {
                        scrollHost.ScrollToHorizontalOffset(
                            Math.Max(0, Math.Min(scrollHost.ScrollableWidth,
                            scrollHost.HorizontalOffset + (scrollHost.ViewportWidth * (forward ? 1 : -1)))));
                    }
                    scrollHost.UpdateLayout();
                }
                // Select the "edge" element
                newFocusedIndex = GetFirstItemOnCurrentPage(_focusedIndex, forward);
            }
        }
        return newFocusedIndex;
    }

    internal int NavigateByLine(bool forward)
    {
        int newFocusedIndex = -1;
        // Get it visible to start with
        if (_focusedIndex != -1 && !IsOnCurrentPage(_focusedIndex))
        {
            ScrollIntoViewImpl(_focusedIndex);
            ScrollHost?.UpdateLayout();
        }

        if (forward)
        {
            int count = Items.Count;
            if (_focusedIndex < count)
                newFocusedIndex = GetNextSelectableIndex(_focusedIndex + 1, 1, count);
        }
        else
        {
            if (_focusedIndex >= 0)
                newFocusedIndex = GetNextSelectableIndex(_focusedIndex - 1, -1, -1);
        }

        if (newFocusedIndex != -1 && ScrollHost != null)
        {
            ScrollIntoViewImpl(newFocusedIndex);
            ScrollHost.UpdateLayout();
        }
        return newFocusedIndex;
    }

    /// <summary>
    /// Indicate whether the specified item is currently visible. 
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>True if the item is visible; false otherwise.</returns> 
    /// <remarks>Similar to WPF's corresponding ItemsControl method.</remarks> 
    private bool IsOnCurrentPage(int index)
    {
        return IsOnCurrentPage(index, out _, out _);
    }

    /// <summary> 
    /// Indicate whether the specified item is currently visible. 
    /// </summary>
    /// <param name="index">The index.</param> 
    /// <param name="itemsHostRect">Rect for the item host element.</param>
    /// <param name="listBoxItemRect">Rect for the ListBoxItem element.</param>
    /// <returns>True if the item is visible; false otherwise.</returns> 
    /// <remarks>Similar to WPF's corresponding ItemsControl method.</remarks>
    private bool IsOnCurrentPage(int index, out Rect itemsHostRect, out Rect listBoxItemRect)
    {
        // Get Rect for item host element 
        FrameworkElement viewport = GetViewportElement();
        if (viewport == null)
        {
            itemsHostRect = Rect.Empty;
            listBoxItemRect = Rect.Empty;
            return false;
        }
        itemsHostRect = new Rect(new Point(), new Point(viewport.ActualWidth, viewport.ActualHeight));

        if (ItemContainerGenerator.ContainerFromIndex(index) is not ListBoxItem listBoxItem || !listBoxItem.IsConnectedToLiveTree)
        {
            listBoxItemRect = Rect.Empty;
            return false;
        }

        Size listBoxItemSize = new Size(listBoxItem.ActualWidth, listBoxItem.ActualHeight);
        listBoxItemRect = new Rect(new Point(), listBoxItemSize);

        // Adjust Rect to account for padding 
        Control itemsHostControl = viewport as Control;
        if (null != itemsHostControl)
        {
            Thickness padding = itemsHostControl.Padding;
            itemsHostRect = new Rect(
                itemsHostRect.Left + padding.Left,
                itemsHostRect.Top + padding.Top,
                itemsHostRect.Width - padding.Left - padding.Right,
                itemsHostRect.Height - padding.Top - padding.Bottom);
        }
        // Get relative Rect for ListBoxItem 
        GeneralTransform generalTransform = listBoxItem.TransformToVisual(viewport);
        if (generalTransform != null)
        {
            listBoxItemRect = new Rect(
                generalTransform.Transform(new Point()),
                generalTransform.Transform(new Point(listBoxItemSize.Width, listBoxItemSize.Height)));
        }

        // Return result
        return (IsVerticalOrientation() ?
            (itemsHostRect.Top <= listBoxItemRect.Top) && (listBoxItemRect.Bottom <= itemsHostRect.Bottom) :
            (itemsHostRect.Left <= listBoxItemRect.Left) && (listBoxItemRect.Right <= itemsHostRect.Right));
    }

    /// <summary> 
    /// Get the first visible item.
    /// </summary>
    /// <param name="startingIndex">Starting index to search from.</param> 
    /// <param name="forward">Search forward if true; backward otherwise.</param>
    /// <returns>Index of first visible item.</returns>
    /// <remarks>Similar to WPF's corresponding ItemsControl method.</remarks> 
    private int GetFirstItemOnCurrentPage(int startingIndex, bool forward)
    {
        int delta = (forward ? 1 : -1);
        int firstItemOnCurrentPage = -1;
        int probeIndex = startingIndex;
        // Scan looking for the first visible element 
        while ((0 <= probeIndex) && (probeIndex < Items.Count) && !IsOnCurrentPage(probeIndex))
        {
            firstItemOnCurrentPage = probeIndex;
            probeIndex += delta;
        }
        // Then scan looking for the last visible element 
        while ((0 <= probeIndex) && (probeIndex < Items.Count) && IsOnCurrentPage(probeIndex))
        {
            firstItemOnCurrentPage = probeIndex;
            probeIndex += delta;
        }
        return firstItemOnCurrentPage;
    }

    private FrameworkElement GetViewportElement()
    {
        // NOTE: When ScrollHost is non-null, we use ScrollHost instead of
        //       ItemsHost because ItemsHost in the physically scrolling
        //       case will just have its layout offset shifted, and all
        //       items will always be within the bounding box of the ItemsHost,
        //       and we want to know if you can actually see the element.
        ScrollViewer viewPort = ScrollHost;
        if (viewPort == null)
        {
            return ItemsHost;
        }
        else
        {
            // Try use the ScrollContentPresenter as the viewport it is it available
            // because that is more representative of the viewport in case of
            // DataGrid when the ColumnHeaders need to be excluded from the
            // dimensions of the viewport.
            ScrollContentPresenter scp = viewPort.ElementScrollContentPresenter;
            if (scp != null)
            {
                return scp;
            }
        }

        return viewPort;
    }

    // Walk in the specified direction until we get to a selectable
    // item or to the stopIndex.
    // NOTE: stopIndex is not inclusive (it should be one past the end of the range)
    internal int GetNextSelectableIndex(int startIndex, int increment, int stopIndex)
    {
        Debug.Assert((increment > 0 && startIndex <= stopIndex) || (increment < 0 && startIndex >= stopIndex), "Infinite loop detected");

        for (int i = startIndex; i != stopIndex; i += increment)
        {
            // If the item is selectable and the wrapper is selectable, select it.
            // Need to check both because the user could set any combination of
            // IsSelectable and IsEnabled on the item and wrapper.
            object item = Items[i];
            DependencyObject container = ItemContainerGenerator.ContainerFromIndex(i);
            if (IsSelectableHelper(item) && IsSelectableHelper(container))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsSelectableHelper(object o)
    {
        // If o is not a DependencyObject, it is just a plain
        // object and must be selectable and enabled.
        if (o is not FrameworkElement fe)
        {
            return true;
        }
        // It's selectable if IsSelectable is true and IsEnabled is true.
        return (bool)fe.GetValue(IsEnabledProperty);
    }

    internal void ScrollIntoViewImpl(int index)
    {
        ScrollViewer scrollHost = ScrollHost;
        if (scrollHost != null && index >= 0 && index < Items.Count)
        {
            if (scrollHost.ScrollInfo == null)
            {
                ScrollIntoViewNative(index);
                return;
            }

            // If the element is virtualizing we have to scroll to the index of the element
            // we've selected. This will force the virtualizing panel to rerender the required
            // elements.
            bool virtualizing = VirtualizingStackPanel.GetIsVirtualizing(this);
            if (!IsOnCurrentPage(index, out Rect itemsHostRect, out Rect listBoxItemRect))
            {
                if (IsVerticalOrientation())
                {
                    double verticalOffset = scrollHost.VerticalOffset;
                    if (virtualizing)
                    {
                        if (verticalOffset - index > 0)
                        {
                            scrollHost.ScrollToVerticalOffset(index);
                        }
                        else
                        {
                            scrollHost.ScrollToVerticalOffset(index - scrollHost.ViewportHeight + 1);
                        }
                        return;
                    }
                    // Scroll into view vertically (first make the right bound visible, then the left)                    
                    double verticalDelta = 0;
                    if (itemsHostRect.Bottom < listBoxItemRect.Bottom)
                    {
                        verticalDelta = listBoxItemRect.Bottom - itemsHostRect.Bottom;
                        verticalOffset += verticalDelta;
                    }
                    if (listBoxItemRect.Top - verticalDelta < itemsHostRect.Top)
                    {
                        verticalOffset -= itemsHostRect.Top - (listBoxItemRect.Top - verticalDelta);
                    }
                    scrollHost.ScrollToVerticalOffset(verticalOffset);
                }
                else
                {
                    double horizontalOffset = scrollHost.HorizontalOffset;
                    if (virtualizing)
                    {
                        if (horizontalOffset - index > 0)
                        {
                            scrollHost.ScrollToHorizontalOffset(index);
                        }
                        else
                        {
                            scrollHost.ScrollToHorizontalOffset(index - scrollHost.ViewportWidth + 1);
                        }
                        return;
                    }
                    // Scroll into view horizontally (first make the bottom bound visible, then the top) 
                    double horizontalDelta = 0;
                    if (itemsHostRect.Right < listBoxItemRect.Right)
                    {
                        horizontalDelta = listBoxItemRect.Right - itemsHostRect.Right;
                        horizontalOffset += horizontalDelta;
                    }
                    if (listBoxItemRect.Left - horizontalDelta < itemsHostRect.Left)
                    {
                        horizontalOffset -= itemsHostRect.Left - (listBoxItemRect.Left - horizontalDelta);
                    }
                    scrollHost.ScrollToHorizontalOffset(horizontalOffset);
                }
            }
        }
    }

    private void ScrollIntoViewNative(int index)
    {
        if (ItemContainerGenerator.ContainerFromIndex(index) is ListBoxItem container
            && container.INTERNAL_OuterDomElement != null)
        {
            string sDomElement = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(container.INTERNAL_OuterDomElement);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sDomElement}.scrollIntoView({{ block: 'nearest' }})");
        }
    }
}
