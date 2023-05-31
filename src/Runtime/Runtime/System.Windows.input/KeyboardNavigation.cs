
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
using Keyboard = System.Windows.Input.Keyboard;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
using Key = Windows.System.VirtualKey;
#endif

#if MIGRATION
namespace System.Windows.Input;
#else
namespace Windows.UI.Xaml.Input;
#endif

///<summary>
/// KeyboardNavigation class provide methods for logical (Tab) navigation between focusable controls
///</summary>
internal sealed class KeyboardNavigation
{
    private KeyboardNavigation() { }

    public static KeyboardNavigation Current { get; } = new KeyboardNavigation();

    internal void ProcessInput(KeyEventArgs keyEventArgs)
    {
        if (keyEventArgs.Handled)
            return;

        DependencyObject sourceElement = keyEventArgs.OriginalSource as DependencyObject;

        // When nothing has focus - we should start from the root of the visual tree
        if (sourceElement == null)
        {
            sourceElement = Application.Current?.RootVisual;
            if (sourceElement == null)
                return;
        }

        keyEventArgs.Handled = Navigate(sourceElement, keyEventArgs.Key, keyEventArgs.KeyModifiers, fromProcessInput: true);
    }

    private static readonly DependencyProperty TabOnceActiveElementProperty =
        DependencyProperty.RegisterAttached(
            "TabOnceActiveElement",
            typeof(WeakReference),
            typeof(KeyboardNavigation),
            null);

    internal static DependencyObject GetTabOnceActiveElement(DependencyObject d)
    {
        WeakReference weakRef = (WeakReference)d.GetValue(TabOnceActiveElementProperty);
        if (weakRef != null && weakRef.IsAlive)
        {
            DependencyObject activeElement = weakRef.Target as DependencyObject;
            // Verify if the element is still in the same visual tree
            if (GetVisualRoot(activeElement) == GetVisualRoot(d))
                return activeElement;
            else
                d.SetValue(TabOnceActiveElementProperty, null);
        }
        return null;
    }

    internal static void SetTabOnceActiveElement(DependencyObject d, DependencyObject value)
    {
        d.SetValue(TabOnceActiveElementProperty, new WeakReference(value));
    }

    private DependencyObject GetActiveElement(DependencyObject d)
    {
        return GetTabOnceActiveElement(d);
    }

    private void SetActiveElement(DependencyObject d, DependencyObject value)
    {
        SetTabOnceActiveElement(d, value);
    }

    internal static DependencyObject GetVisualRoot(DependencyObject d)
    {
        DependencyObject rootVisual = d;
        DependencyObject parentVisual;

        while (rootVisual != null && ((parentVisual = VisualTreeHelper.GetParent(rootVisual)) != null))
        {
            rootVisual = parentVisual;
        }

        return rootVisual;
    }

    internal static void UpdateFocusedElement(UIElement focusTarget)
    {
        DependencyObject focusScope = focusTarget?.INTERNAL_ParentWindow ?? Window.Current;
        if (focusScope != null && focusScope != focusTarget)
        {
            FocusManager.SetFocusedElement(focusScope, focusTarget);
        }
    }

    internal void UpdateActiveElement(DependencyObject activeElement)
    {
        // Update TabNavigation = Once groups
        DependencyObject container = GetGroupParent(activeElement);
        UpdateActiveElement(container, activeElement);
    }

    private void UpdateActiveElement(DependencyObject container, DependencyObject activeElement)
    {
        if (activeElement == container)
            return;

        // Update ActiveElement only if container has TabNavigation = Once
        if (GetKeyNavigationMode(container) == KeyboardNavigationMode.Once)
        {
            SetActiveElement(container, activeElement);
        }
    }

    internal bool Navigate(DependencyObject currentElement, TraversalRequest request)
    {
        return Navigate(currentElement, request, Keyboard.Modifiers);
    }

    // Navigate needs the extra fromProcessInputTabKey parameter to know if this call is coming directly from a keyboard tab key ProcessInput call
    // so it can return false and let the key message go unhandled, we don't want to do this if Navigate was called by MoveFocus, or recursively.
    private bool Navigate(DependencyObject currentElement, TraversalRequest request, ModifierKeys modifierKeys, bool fromProcessInputTabKey = false)
    {
        return Navigate(currentElement, request, modifierKeys, null, fromProcessInputTabKey);
    }

    private bool Navigate(DependencyObject currentElement, TraversalRequest request, ModifierKeys modifierKeys, DependencyObject firstElement, bool fromProcessInputTabKey = false)
    {
        Debug.Assert(currentElement != null, "currentElement should not be null");
        DependencyObject nextTab = null;

        switch (request.FocusNavigationDirection)
        {
            case FocusNavigationDirection.Next:
                nextTab = GetNextTab(currentElement, GetGroupParent(currentElement, true /*includeCurrent*/), false);
                break;

            case FocusNavigationDirection.Previous:
                nextTab = GetPrevTab(currentElement, null, false);
                break;

            case FocusNavigationDirection.First:
                nextTab = GetNextTab(null, currentElement, true);
                break;

            case FocusNavigationDirection.Last:
                nextTab = GetPrevTab(null, currentElement, true);
                break;
        }

        // If there are no other tabstops, try to pass focus outside PresentationSource
        if (nextTab == null)
        {
            return false;
        }

        return nextTab is Control iie && iie.Focus();
    }

    internal bool Navigate(DependencyObject sourceElement, Key key, ModifierKeys modifiers, bool fromProcessInput = false)
    {
        bool success = false;

        if (key == Key.Tab)
        {
            success = Navigate(sourceElement,
                new TraversalRequest(((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) ?
                FocusNavigationDirection.Previous : FocusNavigationDirection.Next), modifiers, fromProcessInput);
        }

        return success;
    }

    // Filter the visual tree and return true if visual is a visible UIElement
    private static bool IsInNavigationTree(DependencyObject visual)
    {
        return visual is UIElement uiElement && uiElement.IsVisible;
    }

    private DependencyObject GetPreviousSibling(DependencyObject e)
    {
        DependencyObject parent = GetParent(e);

        // If parent is UIElement - return visual sibling
        DependencyObject parentAsUIElement = parent as UIElement;
        DependencyObject elementAsVisual = e as UIElement;
        
        if (parentAsUIElement != null && elementAsVisual != null)
        {
            int count = VisualTreeHelper.GetChildrenCount(parentAsUIElement);
            DependencyObject prev = null;
            for (int i = 0; i < count; i++)
            {
                DependencyObject vchild = VisualTreeHelper.GetChild(parentAsUIElement, i);
                if (vchild == elementAsVisual) break;
                if (IsInNavigationTree(vchild))
                    prev = vchild;
            }
            return prev;
        }
        return null;
    }

    private DependencyObject GetNextSibling(DependencyObject e)
    {
        DependencyObject parent = GetParent(e);

        // If parent is UIElement(3D) - return visual sibling
        DependencyObject parentAsUIElement = parent as UIElement;
        DependencyObject elementAsVisual = e as UIElement;

        if (parentAsUIElement != null && elementAsVisual != null)
        {
            int count = VisualTreeHelper.GetChildrenCount(parentAsUIElement);
            int i = 0;
            //go till itself
            for (; i < count; i++)
            {
                DependencyObject vchild = VisualTreeHelper.GetChild(parentAsUIElement, i);
                if (vchild == elementAsVisual) break;
            }
            i++;
            //search ahead
            for (; i < count; i++)
            {
                DependencyObject visual = VisualTreeHelper.GetChild(parentAsUIElement, i);
                if (IsInNavigationTree(visual))
                    return visual;
            }
        }

        return null;
    }

    // For Control+Tab navigation or TabNavigation when fe is not a FocusScope:
    // Scenarios:
    // 1. UserControl can set its FocusedElement to delegate focus when Tab navigation happens
    // 2. ToolBar or Menu (which have IsFocusScope=true) both have FocusedElement but included only in Control+Tab navigation
    private DependencyObject FocusedElement(DependencyObject e)
    {
        UIElement iie = e as UIElement;
        // Focus delegation is enabled only if keyboard focus is outside the container
        if (iie != null)
        {
            DependencyObject focusedElement = FocusManager.GetFocusedElement(e) as DependencyObject;
            if (focusedElement != null)
            {
                if (!IsFocusScope(e))
                {
                    // Verify if focusedElement is a visual descendant of e
                    UIElement visualFocusedElement = focusedElement as UIElement;
                    
                    if (visualFocusedElement != null && visualFocusedElement != e && visualFocusedElement.IsDescendantOf(e))
                    {
                        return focusedElement;
                    }
                }
            }
        }

        return null;
    }

    // We traverse only UIElement(3D) or ContentElement
    private DependencyObject GetFirstChild(DependencyObject e)
    {
        // If the element has a FocusedElement it should be its first child
        DependencyObject focusedElement = FocusedElement(e);
        if (focusedElement != null)
            return focusedElement;

        // Return the first visible UIElement(3D) or IContentHost
        UIElement uiElement = e as UIElement;

        if (uiElement == null || uiElement.IsVisible)
        {
            DependencyObject elementAsVisual = uiElement;

            if (elementAsVisual != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(elementAsVisual);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject visual = VisualTreeHelper.GetChild(elementAsVisual, i);
                    if (IsInNavigationTree(visual))
                        return visual;
                    else
                    {
                        DependencyObject firstChild = GetFirstChild(visual);
                        if (firstChild != null)
                            return firstChild;
                    }
                }
            }
        }

        // If element is ContentElement for example
        return null;
    }

    private DependencyObject GetLastChild(DependencyObject e)
    {
        // If the element has a FocusedElement it should be its last child
        DependencyObject focusedElement = FocusedElement(e);
        if (focusedElement != null)
            return focusedElement;

        // Return the last visible UIElement
        UIElement uiElement = e as UIElement;

        if (uiElement == null || uiElement.IsVisible)
        {
            DependencyObject elementAsVisual = uiElement;

            if (elementAsVisual != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(elementAsVisual);
                for (int i = count - 1; i >= 0; i--)
                {
                    DependencyObject visual = VisualTreeHelper.GetChild(elementAsVisual, i);
                    if (IsInNavigationTree(visual))
                        return visual;
                    else
                    {
                        DependencyObject lastChild = GetLastChild(visual);
                        if (lastChild != null)
                            return lastChild;
                    }
                }
            }
        }

        return null;
    }

    internal static DependencyObject GetParent(DependencyObject e)
    {
        // For Visual - go up the visual parent chain until we find Visual
        if (e is UIElement)
        {
            DependencyObject visual = e;

            while ((visual = GetVisualParentHelper(visual)) != null)
            {
                // bug here related to movemement when you need to go from a child UI3D
                // and you get a parent Viewport3D

                if (IsInNavigationTree(visual))
                    return visual;
            }
        }

        return null;
    }

    private static DependencyObject GetVisualParentHelper(DependencyObject e)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(e);
        if (parent == null && e is FrameworkElement fe && fe.Parent is Popup popup)
        {
            parent = popup.PopupRoot;
        }
        return parent;
    }

    /***************************************************************************\
    *
    * GetNextInTree(DependencyObject e, DependencyObject container)
    * Search the subtree with container root; Don't go inside TabGroups
    *
    * Return the next Element in tree in depth order (self-child-sibling).
    *            1
    *           / \
    *          2   5
    *         / \
    *        3   4
    *
    \***************************************************************************/
    private DependencyObject GetNextInTree(DependencyObject e, DependencyObject container)
    {
        Debug.Assert(e != null, "e should not be null");
        Debug.Assert(container != null, "container should not be null");

        DependencyObject result = null;

        if (e == container || !IsGroup(e))
            result = GetFirstChild(e);

        if (result != null || e == container)
            return result;

        DependencyObject parent = e;
        do
        {
            DependencyObject sibling = GetNextSibling(parent);
            if (sibling != null)
                return sibling;

            parent = GetParent(parent);
        } while (parent != null && parent != container);

        return null;
    }

    /***************************************************************************\
    *
    * GetPreviousInTree(DependencyObject e, DependencyObject container)
    * Don't go inside TabGroups
    * Return the previous Element in tree in depth order (self-child-sibling).
    *            5
    *           / \
    *          4   1
    *         / \
    *        3   2
    \***************************************************************************/
    private DependencyObject GetPreviousInTree(DependencyObject e, DependencyObject container)
    {
        if (e == container)
            return null;

        DependencyObject result = GetPreviousSibling(e);

        if (result != null)
        {
            if (IsGroup(result))
                return result;
            else
                return GetLastInTree(result);
        }
        else
            return GetParent(e);
    }

    // Find the last element in the subtree
    private DependencyObject GetLastInTree(DependencyObject container)
    {
        DependencyObject result;
        do
        {
            result = container;
            container = GetLastChild(container);
        } while (container != null && !IsGroup(container));

        if (container != null)
            return container;

        return result;
    }

    private DependencyObject GetGroupParent(DependencyObject e)
    {
        return GetGroupParent(e, false /*includeCurrent*/);
    }

    // Go up thru the parent chain until we find TabNavigation != Continue
    // In case all parents are Continue then return the root
    private DependencyObject GetGroupParent(DependencyObject e, bool includeCurrent)
    {
        Debug.Assert(e != null, "e cannot be null");

        DependencyObject result = e; // Keep the last non null element

        // If we don't want to include the current element,
        // start at the parent of the element.  If the element
        // is the root, then just return it as the group parent.
        if (!includeCurrent)
        {
            result = e;
            e = GetParent(e);
            if (e == null)
            {
                return result;
            }
        }

        while (e != null)
        {
            if (IsGroup(e))
                return e;

            result = e;
            e = GetParent(e);
        }

        return result;
    }

    private bool IsTabStop(DependencyObject e)
    {
        return e is Control control && IsTabStop(control);
    }

    internal bool IsTabStop(Control control)
    {
        return control.IsTabStop
            && control.IsEnabled
            && control.IsVisible
            && INTERNAL_VisualTreeManager.IsElementInVisualTree(control);
    }

    private bool IsGroup(DependencyObject e)
    {
        return e is Control && e is not TextBlock && e is not TextElement;
    }

    private KeyboardNavigationMode GetKeyNavigationMode(DependencyObject e)
    {
        return e switch
        {
            Control => (KeyboardNavigationMode)e.GetValue(Control.TabNavigationProperty),
            PopupRoot or Window => KeyboardNavigationMode.Cycle,
            _ => KeyboardNavigationMode.Local,
        };
    }

    private bool IsTabStopOrGroup(DependencyObject e)
    {
        return IsTabStop(e) || IsGroup(e);
    }

    private static int GetTabIndexHelper(DependencyObject d)
    {
        return (int)d.GetValue(Control.TabIndexProperty);
    }

    // Find the element with highest priority (lowest index) inside the group
    internal DependencyObject GetFirstTabInGroup(DependencyObject container)
    {
        DependencyObject firstTabElement = null;
        int minIndexFirstTab = int.MinValue;

        DependencyObject currElement = container;
        while ((currElement = GetNextInTree(currElement, container)) != null)
        {
            if (IsTabStopOrGroup(currElement))
            {
                int currPriority = GetTabIndexHelper(currElement);

                if (currPriority < minIndexFirstTab || firstTabElement == null)
                {
                    minIndexFirstTab = currPriority;
                    firstTabElement = currElement;
                }
            }
        }
        return firstTabElement;
    }

    // Find the element with the same TabIndex after the current element
    private DependencyObject GetNextTabWithSameIndex(DependencyObject e, DependencyObject container)
    {
        int elementTabPriority = GetTabIndexHelper(e);
        DependencyObject currElement = e;
        while ((currElement = GetNextInTree(currElement, container)) != null)
        {
            if (IsTabStopOrGroup(currElement) && GetTabIndexHelper(currElement) == elementTabPriority)
            {
                return currElement;
            }
        }

        return null;
    }

    // Find the element with the next TabIndex after the current element
    private DependencyObject GetNextTabWithNextIndex(DependencyObject e, DependencyObject container, KeyboardNavigationMode tabbingType)
    {
        // Find the next min index in the tree
        // min (index>currentTabIndex)
        DependencyObject nextTabElement = null;
        DependencyObject firstTabElement = null;
        int minIndexFirstTab = Int32.MinValue;
        int minIndex = Int32.MinValue;
        int elementTabPriority = GetTabIndexHelper(e);

        DependencyObject currElement = container;
        while ((currElement = GetNextInTree(currElement, container)) != null)
        {
            if (IsTabStopOrGroup(currElement))
            {
                int currPriority = GetTabIndexHelper(currElement);
                if (currPriority > elementTabPriority)
                {
                    if (currPriority < minIndex || nextTabElement == null)
                    {
                        minIndex = currPriority;
                        nextTabElement = currElement;
                    }
                }

                if (currPriority < minIndexFirstTab || firstTabElement == null)
                {
                    minIndexFirstTab = currPriority;
                    firstTabElement = currElement;
                }
            }
        }

        // Cycle groups: if not found - return first element
        if (tabbingType == KeyboardNavigationMode.Cycle && nextTabElement == null)
            nextTabElement = firstTabElement;

        return nextTabElement;
    }

    private DependencyObject GetNextTabInGroup(DependencyObject e, DependencyObject container, KeyboardNavigationMode tabbingType)
    {
        // e == null or e == container -> return the first TabStopOrGroup
        if (e == null || e == container)
        {
            return GetFirstTabInGroup(container);
        }

        if (tabbingType == KeyboardNavigationMode.Once)
            return null;

        DependencyObject nextTabElement = GetNextTabWithSameIndex(e, container);
        if (nextTabElement != null)
            return nextTabElement;

        return GetNextTabWithNextIndex(e, container, tabbingType);
    }

    internal DependencyObject Focus(DependencyObject container)
    {
        Debug.Assert(container != null);

        if (IsTabStop(container))
        {
            return container;
        }

        return GetNextTab(container, container, true);
    }

    private DependencyObject GetNextTab(DependencyObject e, DependencyObject container, bool goDownOnly)
    {
        Debug.Assert(container != null, "container should not be null");

        KeyboardNavigationMode tabbingType = GetKeyNavigationMode(container);

        if (e == null)
        {
            if (IsTabStop(container))
                return container;

            // Using ActiveElement if set
            DependencyObject activeElement = GetActiveElement(container);
            if (activeElement != null)
                return GetNextTab(null, activeElement, true);
        }
        else
        {
            if (tabbingType == KeyboardNavigationMode.Once)
            {
                if (container != e)
                {
                    if (goDownOnly)
                        return null;
                    DependencyObject parentContainer = GetGroupParent(container);
                    return GetNextTab(container, parentContainer, goDownOnly);
                }
            }
        }

        // All groups
        DependencyObject loopStartElement = null;
        DependencyObject nextTabElement = e;
        KeyboardNavigationMode currentTabbingType = tabbingType;

        // Search down inside the container
        while ((nextTabElement = GetNextTabInGroup(nextTabElement, container, currentTabbingType)) != null)
        {
            Debug.Assert(IsTabStopOrGroup(nextTabElement), "nextTabElement should be IsTabStop or group");

            // Avoid the endless loop here for Cycle groups
            if (loopStartElement == nextTabElement)
                break;
            if (loopStartElement == null)
                loopStartElement = nextTabElement;

            DependencyObject firstTabElementInside = GetNextTab(null, nextTabElement, true);
            if (firstTabElementInside != null)
                return firstTabElementInside;
        }

        // If there is no next element in the group (nextTabElement == null)

        // Search up in the tree if allowed
        // consider: Use original tabbingType instead of currentTabbingType
        if (!goDownOnly && GetParent(container) != null)
        {
            return GetNextTab(container, GetGroupParent(container), false);
        }

        return null;
    }

    internal DependencyObject GetLastTabInGroup(DependencyObject container)
    {
        DependencyObject lastTabElement = null;
        int maxIndexFirstTab = int.MaxValue;
        DependencyObject currElement = GetLastInTree(container);
        while (currElement != null && currElement != container)
        {
            if (IsTabStopOrGroup(currElement))
            {
                int currPriority = GetTabIndexHelper(currElement);

                if (currPriority > maxIndexFirstTab || lastTabElement == null)
                {
                    maxIndexFirstTab = currPriority;
                    lastTabElement = currElement;
                }
            }
            currElement = GetPreviousInTree(currElement, container);
        }
        return lastTabElement;
    }

    // Look for element with the same TabIndex before the current element
    private DependencyObject GetPrevTabWithSameIndex(DependencyObject e, DependencyObject container)
    {
        int elementTabPriority = GetTabIndexHelper(e);
        DependencyObject currElement = GetPreviousInTree(e, container);
        while (currElement != null)
        {
            if (IsTabStopOrGroup(currElement) && GetTabIndexHelper(currElement) == elementTabPriority && currElement != container)
            {
                return currElement;
            }
            currElement = GetPreviousInTree(currElement, container);
        }
        return null;
    }

    private DependencyObject GetPrevTabWithPrevIndex(DependencyObject e, DependencyObject container, KeyboardNavigationMode tabbingType)
    {
        // Find the next max index in the tree
        // max (index<currentTabIndex)
        DependencyObject lastTabElement = null;
        DependencyObject nextTabElement = null;
        int elementTabPriority = GetTabIndexHelper(e);
        int maxIndexFirstTab = int.MaxValue;
        int maxIndex = int.MaxValue;
        DependencyObject currElement = GetLastInTree(container);
        while (currElement != null)
        {
            if (IsTabStopOrGroup(currElement) && currElement != container)
            {
                int currPriority = GetTabIndexHelper(currElement);
                if (currPriority < elementTabPriority)
                {
                    if (currPriority > maxIndex || nextTabElement == null)
                    {
                        maxIndex = currPriority;
                        nextTabElement = currElement;
                    }
                }

                if (currPriority > maxIndexFirstTab || lastTabElement == null)
                {
                    maxIndexFirstTab = currPriority;
                    lastTabElement = currElement;
                }
            }

            currElement = GetPreviousInTree(currElement, container);
        }

        // Cycle groups: if not found - return first element
        if (tabbingType == KeyboardNavigationMode.Cycle && nextTabElement == null)
            nextTabElement = lastTabElement;

        return nextTabElement;
    }

    private DependencyObject GetPrevTabInGroup(DependencyObject e, DependencyObject container, KeyboardNavigationMode tabbingType)
    {
        // Search the last index inside the group
        if (e == null)
        {
            return GetLastTabInGroup(container);
        }

        if (tabbingType == KeyboardNavigationMode.Once)
            return null;

        if (e == container)
            return null;

        DependencyObject nextTabElement = GetPrevTabWithSameIndex(e, container);
        if (nextTabElement != null)
            return nextTabElement;

        return GetPrevTabWithPrevIndex(e, container, tabbingType);
    }

    private DependencyObject GetPrevTab(DependencyObject e, DependencyObject container, bool goDownOnly)
    {
        Debug.Assert(e != null || container != null, "e or container should not be null");

        if (container == null)
            container = GetGroupParent(e);

        KeyboardNavigationMode tabbingType = GetKeyNavigationMode(container);

        if (e == null)
        {
            // Using ActiveElement if set
            DependencyObject activeElement = GetActiveElement(container);
            if (activeElement != null)
                return GetPrevTab(null, activeElement, true);
            else
            {
                // If we Shift+Tab on a container with KeyboardNavigationMode=Once, and ActiveElement is null
                // then we want to go to the fist item (not last) within the container
                if (tabbingType == KeyboardNavigationMode.Once)
                {
                    DependencyObject firstTabElement = GetNextTabInGroup(null, container, tabbingType);
                    if (firstTabElement == null)
                    {
                        if (IsTabStop(container))
                            return container;
                        if (goDownOnly)
                            return null;

                        return GetPrevTab(container, null, false);
                    }
                    else
                    {
                        return GetPrevTab(null, firstTabElement, true);
                    }
                }
            }
        }
        else
        {
            if (tabbingType == KeyboardNavigationMode.Once)
            {
                if (goDownOnly || container == e)
                    return null;

                // FocusedElement should not be e otherwise we will delegate focus to the same element
                if (IsTabStop(container))
                    return container;

                return GetPrevTab(container, null, false);
            }
        }

        // All groups (except Once) - continue
        DependencyObject loopStartElement = null;
        DependencyObject nextTabElement = e;

        // Look for element with the same TabIndex before the current element
        while ((nextTabElement = GetPrevTabInGroup(nextTabElement, container, tabbingType)) != null)
        {
            if (nextTabElement == container && tabbingType == KeyboardNavigationMode.Local)
                break;

            // At this point nextTabElement is TabStop or TabGroup
            // In case it is a TabStop only return the element
            if (IsTabStop(nextTabElement) && !IsGroup(nextTabElement))
                return nextTabElement;

            // Avoid the endless loop here
            if (loopStartElement == nextTabElement)
                break;
            if (loopStartElement == null)
                loopStartElement = nextTabElement;

            // At this point nextTabElement is TabGroup
            DependencyObject lastTabElementInside = GetPrevTab(null, nextTabElement, true);
            if (lastTabElementInside != null)
                return lastTabElementInside;
        }

        if (e != container && IsTabStop(container))
            return container;

        // If end of the subtree is reached or there no other elements above
        if (!goDownOnly && GetParent(container) != null)
        {
            return GetPrevTab(container, null, false);
        }

        return null;
    }

    // The element is focus scope if it is the visual tree root
    private bool IsFocusScope(DependencyObject e)
    {
        return GetParent(e) == null;
    }
}
