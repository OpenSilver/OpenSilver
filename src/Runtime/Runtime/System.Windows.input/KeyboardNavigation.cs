
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
using OpenSilver.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace System.Windows.Input;

/// <summary>
/// Provides logical and directional navigation between focusable objects.
/// </summary>
public sealed class KeyboardNavigation
{
    [ThreadStatic]
    private static KeyboardNavigation _current;
    private static readonly object _fakeNull = new();
    private const double BASELINE_DEFAULT = double.MinValue;

    private readonly WeakReferenceList<KeyboardFocusChangedEventHandler> _weakFocusChangedHandlers = new();
    private readonly Dictionary<DependencyObject, HashSet<object>> _containerHashtable = new(10);
    private double _verticalBaseline = BASELINE_DEFAULT;
    private double _horizontalBaseline = BASELINE_DEFAULT;
    private DependencyProperty _navigationProperty;

    private KeyboardNavigation() { }

    internal static KeyboardNavigation Current => _current ??= new KeyboardNavigation();

    /// <summary>
    /// Identifies the KeyboardNavigation.TabIndex attached property.
    /// </summary>
    public static readonly DependencyProperty TabIndexProperty =
        DependencyProperty.RegisterAttached(
            "TabIndex",
            typeof(int),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(int.MaxValue));

    /// <summary>
    /// Gets the value of the KeyboardNavigation.TabIndex attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the attached property.
    /// </param>
    /// <returns>
    /// The value of the KeyboardNavigation.TabIndex property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static int GetTabIndex(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (int)element.GetValue(TabIndexProperty);
    }

    /// <summary>
    /// Set the value of the KeyboardNavigation.TabIndex attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element on which to set the attached property to.
    /// </param>
    /// <param name="index">
    /// The property value to set.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static void SetTabIndex(DependencyObject element, int index)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(TabIndexProperty, index);
    }

    /// <summary>
    /// Identifies the KeyboardNavigation.IsTabStop attached property.
    /// </summary>
    public static readonly DependencyProperty IsTabStopProperty =
        DependencyProperty.RegisterAttached(
            "IsTabStop",
            typeof(bool),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets the value of the KeyboardNavigation.IsTabStop attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the attached property.
    /// </param>
    /// <returns>
    /// The value of the KeyboardNavigation.IsTabStop property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetIsTabStop(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(IsTabStopProperty);
    }

    /// <summary>
    /// Sets the value of the KeyboardNavigation.IsTabStop attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to which to write the attached property.
    /// </param>
    /// <param name="isTabStop">
    /// The property value to set.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static void SetIsTabStop(DependencyObject element, bool isTabStop)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(IsTabStopProperty, isTabStop);
    }

    /// <summary>
    /// Identifies the KeyboardNavigation.TabNavigation attached property.
    /// </summary>
    public static readonly DependencyProperty TabNavigationProperty =
        DependencyProperty.RegisterAttached(
            "TabNavigation",
            typeof(KeyboardNavigationMode),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue),
            IsValidKeyNavigationMode);

    /// <summary>
    /// Gets the value of the KeyboardNavigation.TabNavigation attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// Element from which to get the attached property.
    /// </param>
    /// <returns>
    /// The value of the KeyboardNavigation.TabNavigation property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static KeyboardNavigationMode GetTabNavigation(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (KeyboardNavigationMode)element.GetValue(TabNavigationProperty);
    }

    /// <summary>
    /// Sets the value of the KeyboardNavigation.TabNavigation attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// Element on which to set the attached property.
    /// </param>
    /// <param name="mode">
    /// Property value to set.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static void SetTabNavigation(DependencyObject element, KeyboardNavigationMode mode)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(TabNavigationProperty, mode);
    }

    /// <summary>
    /// Identifies the KeyboardNavigation.ControlTabNavigation attached property
    /// </summary>
    public static readonly DependencyProperty ControlTabNavigationProperty =
        DependencyProperty.RegisterAttached(
            "ControlTabNavigation",
            typeof(KeyboardNavigationMode),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue),
            IsValidKeyNavigationMode);

    /// <summary>
    /// Gets the value of the KeyboardNavigation.ControlTabNavigation attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// Element from which to get the attached property.
    /// </param>
    /// <returns>
    /// The value of the KeyboardNavigation.ControlTabNavigation property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static KeyboardNavigationMode GetControlTabNavigation(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (KeyboardNavigationMode)element.GetValue(ControlTabNavigationProperty);
    }

    /// <summary>
    /// Sets the value of the KeyboardNavigation.ControlTabNavigation attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// Element on which to set the attached property.
    /// </param>
    /// <param name="mode">
    /// The property value to set.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetControlTabNavigation(DependencyObject element, KeyboardNavigationMode mode)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(ControlTabNavigationProperty, mode);
    }

    /// <summary>
    /// Identifies the KeyboardNavigation.DirectionalNavigation attached property.
    /// </summary>
    public static readonly DependencyProperty DirectionalNavigationProperty =
        DependencyProperty.RegisterAttached(
            "DirectionalNavigation",
            typeof(KeyboardNavigationMode),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(KeyboardNavigationMode.Continue),
            IsValidKeyNavigationMode);

    /// <summary>
    /// Gets the value of the KeyboardNavigation.DirectionalNavigation attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// Element from which to get the attached property.
    /// </param>
    /// <returns>
    /// The value of the KeyboardNavigation.DirectionalNavigation property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static KeyboardNavigationMode GetDirectionalNavigation(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (KeyboardNavigationMode)element.GetValue(DirectionalNavigationProperty);
    }

    /// <summary>
    /// Sets the value of the KeyboardNavigation.DirectionalNavigation attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// Element on which to set the attached property.
    /// </param>
    /// <param name="mode">
    /// Property value to set.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="element"/> is null.
    /// </exception>
    public static void SetDirectionalNavigation(DependencyObject element, KeyboardNavigationMode mode)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(DirectionalNavigationProperty, mode);
    }

    private static bool IsValidKeyNavigationMode(object o)
    {
        var value = (KeyboardNavigationMode)o;
        return value == KeyboardNavigationMode.Continue ||
               value == KeyboardNavigationMode.Once ||
               value == KeyboardNavigationMode.Cycle ||
               value == KeyboardNavigationMode.None ||
               value == KeyboardNavigationMode.Contained ||
               value == KeyboardNavigationMode.Local;
    }

    /// <summary>
    /// Identifies the KeyboardNavigation.AcceptsReturn attached property.
    /// </summary>
    public static readonly DependencyProperty AcceptsReturnProperty =
        DependencyProperty.RegisterAttached(
            "AcceptsReturn",
            typeof(bool),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets the value of the KeyboardNavigation.AcceptsReturn attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the attached property.
    /// </param>
    /// <returns>
    /// The value of the KeyboardNavigation.AcceptsReturn property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
    public static bool GetAcceptsReturn(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);

        return (bool)element.GetValue(AcceptsReturnProperty);
    }

    /// <summary>
    /// Sets the value of the KeyboardNavigation.AcceptsReturn attached property for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element to write the attached property to.
    /// </param>
    /// <param name="enabled">
    /// The property value to set.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// element is null.
    /// </exception>
    public static void SetAcceptsReturn(DependencyObject element, bool enabled)
    {
        ArgumentNullException.ThrowIfNull(element);

        element.SetValueInternal(AcceptsReturnProperty, enabled);
    }

    /// <summary>
    /// Attached property set on elements registered with AccessKeyManager when AccessKeyCues should be shown.
    /// </summary>
    internal static readonly DependencyProperty IsAccessKeyModeProperty =
        DependencyProperty.RegisterAttached(
            "IsAccessKeyMode",
            typeof(bool),
            typeof(KeyboardNavigation),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

    internal static bool GetIsAccessKeyMode(DependencyObject d)
    {
        Debug.Assert(d is not null);
        return (bool)d.GetValue(IsAccessKeyModeProperty);
    }

    private static void SetIsAccessKeyMode(DependencyObject d, bool value)
    {
        Debug.Assert(d is not null);
        d.SetValueInternal(IsAccessKeyModeProperty, value);
    }

    internal event KeyboardFocusChangedEventHandler FocusChanged
    {
        add
        {
            lock (_weakFocusChangedHandlers)
            {
                _weakFocusChangedHandlers.Add(value);
            }
        }
        remove
        {
            lock (_weakFocusChangedHandlers)
            {
                _weakFocusChangedHandlers.Remove(value);
            }
        }
    }

    internal void NotifyFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
    {
        _weakFocusChangedHandlers.Process(
            static (handler, sender, e) =>
            {
                handler?.Invoke(sender, e);
                return false;
            },
            sender,
            e);
    }

    internal void ProcessInput(KeyEventArgs keyEventArgs)
    {
        if (keyEventArgs.Handled)
        {
            return;
        }

        // When nothing has focus - we should start from the root of the visual tree
        if (keyEventArgs.OriginalSource is not DependencyObject sourceElement)
        {
            sourceElement = Window.Current?.Content;
            if (sourceElement is null)
            {
                return;
            }
        }

        if (keyEventArgs.Key == Key.Alt)
        {
            ToggleKeyboardCues(sourceElement);
        }

        keyEventArgs.Handled = Navigate(sourceElement, keyEventArgs.Key, keyEventArgs.KeyModifiers, fromProcessInput: true);
    }

    internal static void ToggleKeyboardCues(DependencyObject element)
    {
        if (element is not UIElement visual)
        {
            return;
        }

        if (VisualTreeHelper.GetVisualRoot(visual) is DependencyObject rootVisual)
        {
            SetIsAccessKeyMode(rootVisual, !GetIsAccessKeyMode(rootVisual));
        }
    }

    internal static FocusNavigationDirection KeyToTraversalDirection(Key key)
    {
        return key switch
        {
            Key.Left => FocusNavigationDirection.Left,
            Key.Right => FocusNavigationDirection.Right,
            Key.Up => FocusNavigationDirection.Up,
            Key.Down => FocusNavigationDirection.Down,
            _ => throw new NotSupportedException(),
        };
    }

    internal DependencyObject PredictFocusedElement(DependencyObject sourceElement, FocusNavigationDirection direction)
    {
        return PredictFocusedElement(sourceElement, direction, /*treeViewNavigation*/ false);
    }

    internal DependencyObject PredictFocusedElement(DependencyObject sourceElement, FocusNavigationDirection direction, bool treeViewNavigation)
    {
        return PredictFocusedElement(sourceElement, direction, treeViewNavigation, considerDescendants: true);
    }

    internal DependencyObject PredictFocusedElement(DependencyObject sourceElement,
        FocusNavigationDirection direction,
        bool treeViewNavigation,
        bool considerDescendants)
    {
        if (sourceElement == null)
        {
            return null;
        }

        _navigationProperty = DirectionalNavigationProperty;
        _verticalBaseline = BASELINE_DEFAULT;
        _horizontalBaseline = BASELINE_DEFAULT;
        return GetNextInDirection(sourceElement, direction, treeViewNavigation, considerDescendants);
    }

    private static readonly DependencyProperty TabOnceActiveElementProperty =
        DependencyProperty.RegisterAttached(
            "TabOnceActiveElement",
            typeof(WeakReference<DependencyObject>),
            typeof(KeyboardNavigation),
            null);

    internal static DependencyObject GetTabOnceActiveElement(DependencyObject d)
    {
        var weakRef = (WeakReference<DependencyObject>)d.GetValue(TabOnceActiveElementProperty);
        if (weakRef != null && weakRef.TryGetTarget(out DependencyObject activeElement))
        {
            // Verify if the element is still in the same visual tree
            if (VisualTreeHelper.GetVisualRoot(activeElement) == VisualTreeHelper.GetVisualRoot(d))
                return activeElement;
            else
                d.SetValueInternal(TabOnceActiveElementProperty, null);
        }
        return null;
    }

    internal static void SetTabOnceActiveElement(DependencyObject d, DependencyObject value)
    {
        d.SetValueInternal(TabOnceActiveElementProperty, new WeakReference<DependencyObject>(value));
    }

    private static readonly DependencyProperty ControlTabOnceActiveElementProperty =
        DependencyProperty.RegisterAttached(
            "ControlTabOnceActiveElement",
            typeof(WeakReference<DependencyObject>),
            typeof(KeyboardNavigation),
            null);

    private static DependencyObject GetControlTabOnceActiveElement(DependencyObject d)
    {
        var weakRef = (WeakReference<DependencyObject>)d.GetValue(ControlTabOnceActiveElementProperty);
        if (weakRef != null && weakRef.TryGetTarget(out DependencyObject activeElement))
        {
            // Verify if the element is still in the same visual tree
            if (VisualTreeHelper.GetVisualRoot(activeElement) == VisualTreeHelper.GetVisualRoot(d))
                return activeElement;
            else
                d.SetValueInternal(ControlTabOnceActiveElementProperty, null);
        }
        return null;
    }

    private static void SetControlTabOnceActiveElement(DependencyObject d, DependencyObject value)
    {
        d.SetValueInternal(ControlTabOnceActiveElementProperty, new WeakReference<DependencyObject>(value));
    }

    private DependencyObject GetActiveElement(DependencyObject d)
    {
        return _navigationProperty == ControlTabNavigationProperty ?
            GetControlTabOnceActiveElement(d) :
            GetTabOnceActiveElement(d);
    }

    private void SetActiveElement(DependencyObject d, DependencyObject value)
    {
        if (_navigationProperty == TabNavigationProperty)
            SetTabOnceActiveElement(d, value);
        else
            SetControlTabOnceActiveElement(d, value);
    }

    internal static void UpdateFocusedElement(UIElement focusTarget)
    {
        DependencyObject focusScope = FocusManager.GetFocusScope(focusTarget);
        if (focusScope is not null && focusScope != focusTarget)
        {
            FocusManager.SetFocusedElement(focusScope, focusTarget);
        }
    }

    internal void UpdateActiveElement(DependencyObject activeElement)
    {
        // Update TabNavigation = Once groups
        UpdateActiveElement(activeElement, TabNavigationProperty);

        // Update ControlTabNavigation = Once groups
        UpdateActiveElement(activeElement, ControlTabNavigationProperty);
    }

    private void UpdateActiveElement(DependencyObject activeElement, DependencyProperty dp)
    {
        _navigationProperty = dp;
        DependencyObject container = GetGroupParent(activeElement);
        UpdateActiveElement(container, activeElement, dp);
    }

    internal void UpdateActiveElement(DependencyObject container, DependencyObject activeElement)
    {
        // Update TabNavigation = Once groups
        UpdateActiveElement(container, activeElement, TabNavigationProperty);

        // Update ControlTabNavigation = Once groups
        UpdateActiveElement(container, activeElement, ControlTabNavigationProperty);
    }

    private void UpdateActiveElement(DependencyObject container, DependencyObject activeElement, DependencyProperty dp)
    {
        _navigationProperty = dp;

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
                _navigationProperty = (modifierKeys & ModifierKeys.Control) == ModifierKeys.Control ? ControlTabNavigationProperty : TabNavigationProperty;
                nextTab = GetNextTab(currentElement, GetGroupParent(currentElement, true /*includeCurrent*/), false);
                break;

            case FocusNavigationDirection.Previous:
                _navigationProperty = (modifierKeys & ModifierKeys.Control) == ModifierKeys.Control ? ControlTabNavigationProperty : TabNavigationProperty;
                nextTab = GetPrevTab(currentElement, null, false);
                break;

            case FocusNavigationDirection.First:
                _navigationProperty = (modifierKeys & ModifierKeys.Control) == ModifierKeys.Control ? ControlTabNavigationProperty : TabNavigationProperty;
                nextTab = GetNextTab(null, currentElement, true);
                break;

            case FocusNavigationDirection.Last:
                _navigationProperty = (modifierKeys & ModifierKeys.Control) == ModifierKeys.Control ? ControlTabNavigationProperty : TabNavigationProperty;
                nextTab = GetPrevTab(null, currentElement, true);
                break;

            case FocusNavigationDirection.Left:
            case FocusNavigationDirection.Right:
            case FocusNavigationDirection.Up:
            case FocusNavigationDirection.Down:
                _navigationProperty = DirectionalNavigationProperty;
                nextTab = GetNextInDirection(currentElement, request.FocusNavigationDirection);
                break;
        }

        // If there are no other tabstops, try to pass focus outside PresentationSource
        if (nextTab == null)
        {
            return false;
        }

        return nextTab is UIElement iie && iie.Focus();
    }

    private DependencyObject GetNextInDirection(DependencyObject sourceElement, FocusNavigationDirection direction)
    {
        return GetNextInDirection(sourceElement, direction, /*treeViewNavigation*/ false);
    }

    private DependencyObject GetNextInDirection(DependencyObject sourceElement, FocusNavigationDirection direction, bool treeViewNavigation)
    {
        return GetNextInDirection(sourceElement, direction, treeViewNavigation, considerDescendants: true);
    }

    private DependencyObject GetNextInDirection(DependencyObject sourceElement,
        FocusNavigationDirection direction,
        bool treeViewNavigation,
        bool considerDescendants)
    {
        _containerHashtable.Clear();
        DependencyObject targetElement = MoveNext(sourceElement, null, direction, BASELINE_DEFAULT, BASELINE_DEFAULT, treeViewNavigation, considerDescendants);

        if (targetElement != null)
        {
            UIElement sourceUIElement = sourceElement as UIElement;
            if (sourceUIElement != null)
                sourceUIElement.RemoveHandler(Keyboard.PreviewLostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(LostFocus));

            UIElement targetUIElement = targetElement as UIElement;

            if (targetUIElement != null)
            {
                // When layout is changed we need to reset the base line
                // Set up a layout invalidation listener.
                targetUIElement.LayoutUpdated += new EventHandler(OnLayoutUpdated);

                // When Focus is changed we need to reset the base line
                if (targetElement == targetUIElement)
                    targetUIElement.AddHandler(Keyboard.PreviewLostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(LostFocus), true);
            }
        }

        _containerHashtable.Clear();
        return targetElement;
    }

    // LayoutUpdated handler.
    private void OnLayoutUpdated(object sender, EventArgs e)
    {
        UIElement uiElement = sender as UIElement;
        // Disconnect the layout listener.
        uiElement?.LayoutUpdated -= new EventHandler(OnLayoutUpdated);

        _verticalBaseline = BASELINE_DEFAULT;
        _horizontalBaseline = BASELINE_DEFAULT;
    }

    private void LostFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        _verticalBaseline = BASELINE_DEFAULT;
        _horizontalBaseline = BASELINE_DEFAULT;

        if (sender is UIElement uie)
            uie.RemoveHandler(Keyboard.PreviewLostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(LostFocus));
    }

    private bool IsEndlessLoop(DependencyObject element, DependencyObject container)
    {
        object elementObject = element ?? _fakeNull;

        // If entry exists then we have endless loop
        if (_containerHashtable.TryGetValue(container, out HashSet<object> elementTable))
        {
            if (elementTable.Contains(elementObject))
                return true;
        }
        else
        {
            // Adding the entry to the collection
            elementTable = [];
            _containerHashtable[container] = elementTable;
        }

        elementTable.Add(elementObject);
        return false;
    }

    private void ResetBaseLines(double value, bool horizontalDirection)
    {
        if (horizontalDirection)
        {
            _verticalBaseline = BASELINE_DEFAULT;
            if (_horizontalBaseline == BASELINE_DEFAULT)
                _horizontalBaseline = value;
        }
        else // vertical direction
        {
            _horizontalBaseline = BASELINE_DEFAULT;
            if (_verticalBaseline == BASELINE_DEFAULT)
                _verticalBaseline = value;
        }
    }

    private DependencyObject FindNextInDirection(DependencyObject sourceElement,
        Rect sourceRect,
        DependencyObject container,
        FocusNavigationDirection direction,
        double startRange,
        double endRange,
        bool treeViewNavigation,
        bool considerDescendants)   // when false, descendants of sourceElement are not candidates
    {
        DependencyObject result = null;
        Rect resultRect = Rect.Empty;
        double resultScore = 0d;
        bool searchInsideContainer = sourceElement == null;
        DependencyObject currElement = container;
        while ((currElement = GetNextInTree(currElement, container)) != null)
        {
            if (currElement != sourceElement &&
                IsGroupElementEligible(currElement, treeViewNavigation))
            {
                Rect currentRect = GetRepresentativeRectangle(currElement);

                // Consider the current element as a result candidate only if its layout is valid.
                if (currentRect != Rect.Empty)
                {
                    bool isInDirection = IsInDirection(sourceRect, currentRect, direction);
                    bool isInRange = IsInRange(sourceElement, currElement, sourceRect, currentRect, direction, startRange, endRange);
                    if (searchInsideContainer || isInDirection || isInRange)
                    {
                        double score = isInRange ? GetPerpDistance(sourceRect, currentRect, direction) : GetDistance(sourceRect, currentRect, direction);

                        if (double.IsNaN(score))
                        {
                            continue;
                        }

                        // Keep the first element in the result
                        if (result == null && (considerDescendants || !IsAncestorOfEx(sourceElement, currElement)))
                        {
                            result = currElement;
                            resultRect = currentRect;
                            resultScore = score;
                        }
                        else if ((DoubleUtil.LessThan(score, resultScore) ||
                            (DoubleUtil.AreClose(score, resultScore) && GetDistance(sourceRect, resultRect, direction) > GetDistance(sourceRect, currentRect, direction))) &&
                            (considerDescendants || !IsAncestorOfEx(sourceElement, currElement)))
                        {
                            result = currElement;
                            resultRect = currentRect;
                            resultScore = score;
                        }
                    }
                }
            }
        }

        return result;
    }

    private DependencyObject MoveNext(DependencyObject sourceElement,
        DependencyObject container,
        FocusNavigationDirection direction,
        double startRange,
        double endRange,
        bool treeViewNavigation,
        bool considerDescendants)
    {
        Debug.Assert(!(sourceElement == null && container == null), "Both sourceElement and container cannot be null");

        if (container == null)
        {
            container = GetGroupParent(sourceElement);
            Debug.Assert(container != null, "container cannot be null");
        }

        // If we get to the tree root, return null
        if (container == sourceElement)
            return null;

        if (IsEndlessLoop(sourceElement, container))
            return null;

        KeyboardNavigationMode mode = GetKeyNavigationMode(container);
        bool searchInsideContainer = (sourceElement == null);

        // Don't navigate inside None containers
        if (mode == KeyboardNavigationMode.None && searchInsideContainer)
            return null;

        Rect sourceRect = searchInsideContainer ? GetRectangle(container) : GetRepresentativeRectangle(sourceElement);
        bool horizontalDirection = direction == FocusNavigationDirection.Right || direction == FocusNavigationDirection.Left;

        // Reset the baseline when we change the direction
        ResetBaseLines(horizontalDirection ? sourceRect.Top : sourceRect.Left, horizontalDirection);

        // If range is not set - use source rect
        if (startRange == BASELINE_DEFAULT || endRange == BASELINE_DEFAULT)
        {
            startRange = horizontalDirection ? sourceRect.Top : sourceRect.Left;
            endRange = horizontalDirection ? sourceRect.Bottom : sourceRect.Right;
        }

        // Navigate outside the container
        if (mode == KeyboardNavigationMode.Once && !searchInsideContainer)
            return MoveNext(container, null, direction, startRange, endRange, treeViewNavigation, considerDescendants: true);

        DependencyObject result = FindNextInDirection(sourceElement, sourceRect, container, direction, startRange, endRange, treeViewNavigation, considerDescendants);

        // If there is no next element in current container
        if (result == null)
        {
            switch (mode)
            {
                case KeyboardNavigationMode.Cycle:
                    return MoveNext(null, container, direction, startRange, endRange, treeViewNavigation, considerDescendants: true);

                case KeyboardNavigationMode.Contained:
                    return null;

                default: // Continue, Once, None, Local - search outside the container
                    return MoveNext(container, null, direction, startRange, endRange, treeViewNavigation, considerDescendants: true);
            }
        }

        if (IsElementEligible(result, treeViewNavigation))
            return result;

        // Using ActiveElement if set
        DependencyObject activeElement = GetActiveElementChain(result, treeViewNavigation);
        if (activeElement != null)
            return activeElement;

        // Try to find focus inside the element
        // result is not TabStop, which means it is a group
        DependencyObject insideElement = MoveNext(null, result, direction, startRange, endRange, treeViewNavigation, considerDescendants: true);
        if (insideElement != null)
            return insideElement;

        return MoveNext(result, null, direction, startRange, endRange, treeViewNavigation, considerDescendants: true);
    }

    /// <summary>
    ///     Returns the element rectange relative to the root.
    ///     Also calls UpdateLayout if the layout of element is
    ///     not valid.
    /// </summary>
    internal static Rect GetRectangle(DependencyObject element)
    {
        if (element is UIElement uiElement)
        {
            if (!uiElement.IsArrangeValid)
            {
                // Call UpdateLayout if qualifies.
                uiElement.UpdateLayout();
            }

            if (VisualTreeHelper.GetVisualRoot(uiElement) is UIElement rootVisual)
            {
                Matrix transform = uiElement.InternalTransformToAncestor(rootVisual);
                return Rect.Transform(new Rect(uiElement.RenderSize), transform);
            }
        }

        return Rect.Empty;
    }

    // return the rectangle representing the given element.  Usually this is
    // just GetRectangle(element).  But a TreeViewItem surrounds its children,
    // which produces wrong results.  Instead use a rectangle that excludes
    // the children in the vertical direction, and extends as much as the
    // TreeViewItem in the horizontal direction.
    private Rect GetRepresentativeRectangle(DependencyObject element)
    {
        Rect rect = GetRectangle(element);
        TreeViewItem tvi = element as TreeViewItem;
        if (tvi != null)
        {
            Panel itemsHost = tvi.ItemsHost;
            if (itemsHost != null && itemsHost.IsVisible)
            {
                Rect itemsHostRect = GetRectangle(itemsHost);
                if (itemsHostRect != Rect.Empty)
                {
                    bool? placeBeforeChildren = null;

                    // if there's a header, put the representative Rect on
                    // the same side of the children as the header.
                    FrameworkElement header = tvi.TryGetHeaderElement();
                    if (header != null && header != tvi && header.IsVisible)
                    {
                        Rect headerRect = GetRectangle(header);
                        if (!headerRect.IsEmpty)
                        {
                            if (DoubleUtil.LessThan(headerRect.Top, itemsHostRect.Top))
                            {
                                // header starts before children - put rect before
                                // (this is the most common case, used by the default layout)
                                placeBeforeChildren = true;
                            }
                            else if (DoubleUtil.GreaterThan(headerRect.Bottom, itemsHostRect.Bottom))
                            {
                                // header ends after children - put rect after
                                placeBeforeChildren = false;
                            }
                        }
                    }

                    double before = itemsHostRect.Top - rect.Top;
                    double after = rect.Bottom - itemsHostRect.Bottom;

                    // If there is no header, or if the header doesn't extend
                    // past the children, put the representative Rect on
                    // whichever side of the children has more room.
                    // This handles the case where the TVI's template has
                    // content that's not explicitly marked as "header".
                    if (placeBeforeChildren == null)
                    {
                        placeBeforeChildren = DoubleUtil.GreaterThanOrClose(before, after);
                    }

                    // adjust the rect according to the placement computed above.
                    // Ensure that its height is non-negative, but no taller than the TVI.
                    if (placeBeforeChildren == true)
                    {
                        rect.Height = Math.Min(Math.Max(before, 0.0d), rect.Height);
                    }
                    else
                    {
                        double height = Math.Min(Math.Max(after, 0.0d), rect.Height);
                        rect.Y = rect.Bottom - height;
                        rect.Height = height;
                    }
                }
            }
        }

        return rect;
    }

    // distance between two points
    private double GetDistance(Point p1, Point p2)
    {
        double deltaX = p1.X - p2.X;
        double deltaY = p1.Y - p2.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    private double GetPerpDistance(Rect sourceRect, Rect targetRect, FocusNavigationDirection direction)
    {
        return direction switch
        {
            FocusNavigationDirection.Right => targetRect.Left - sourceRect.Left,
            FocusNavigationDirection.Left => sourceRect.Right - targetRect.Right,
            FocusNavigationDirection.Up => sourceRect.Bottom - targetRect.Bottom,
            FocusNavigationDirection.Down => targetRect.Top - sourceRect.Top,
            _ => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(FocusNavigationDirection)),
        };
    }

    // Example when moving down:
    // distance between sourceRect.TopLeft (or Y=vertical baseline)
    // and targetRect.TopLeft
    private double GetDistance(Rect sourceRect, Rect targetRect, FocusNavigationDirection direction)
    {
        Point startPoint;
        Point endPoint;
        switch (direction)
        {
            case FocusNavigationDirection.Right:
                startPoint = sourceRect.TopLeft;
                if (_horizontalBaseline != BASELINE_DEFAULT)
                    startPoint.Y = _horizontalBaseline;
                endPoint = targetRect.TopLeft;
                break;

            case FocusNavigationDirection.Left:
                startPoint = sourceRect.TopRight;
                if (_horizontalBaseline != BASELINE_DEFAULT)
                    startPoint.Y = _horizontalBaseline;
                endPoint = targetRect.TopRight;
                break;

            case FocusNavigationDirection.Up:
                startPoint = sourceRect.BottomLeft;
                if (_verticalBaseline != BASELINE_DEFAULT)
                    startPoint.X = _verticalBaseline;
                endPoint = targetRect.BottomLeft;
                break;

            case FocusNavigationDirection.Down:
                startPoint = sourceRect.TopLeft;
                if (_verticalBaseline != BASELINE_DEFAULT)
                    startPoint.X = _verticalBaseline;
                endPoint = targetRect.TopLeft;
                break;

            default:
                throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(FocusNavigationDirection));
        }
        return GetDistance(startPoint, endPoint);
    }

    // Example when moving down:
    // true if the top of the toRect is below the bottom of fromRect
    private bool IsInDirection(Rect fromRect, Rect toRect, FocusNavigationDirection direction)
    {
        return direction switch
        {
            FocusNavigationDirection.Right => DoubleUtil.LessThanOrClose(fromRect.Right, toRect.Left),
            FocusNavigationDirection.Left => DoubleUtil.GreaterThanOrClose(fromRect.Left, toRect.Right),
            FocusNavigationDirection.Up => DoubleUtil.GreaterThanOrClose(fromRect.Top, toRect.Bottom),
            FocusNavigationDirection.Down => DoubleUtil.LessThanOrClose(fromRect.Bottom, toRect.Top),
            _ => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(FocusNavigationDirection)),
        };
    }

    // this is like the previous method, except it works when targetElement is
    // a ContentElement (e.g. Hyperlink).  "Works" means it gives results consistent
    // with the tree navigation methods GetParent, Get*Child, Get*Sibling.
    // [It might be correct to have only one method - this one - but we need
    // to keep the existing calls to the previous method around for compat.]
    internal bool IsAncestorOfEx(DependencyObject sourceElement, DependencyObject targetElement)
    {
        Debug.Assert(sourceElement != null, "sourceElement must not be null");

        while (targetElement != null && targetElement != sourceElement)
        {
            targetElement = GetParent(targetElement);
        }

        return targetElement == sourceElement;
    }

    // Example: When moving down:
    // Range is the sourceRect width extended to the vertical baseline
    // targetRect.Top > sourceRect.Top (target is below the source)
    // targetRect.Right > sourceRect.Left || targetRect.Left < sourceRect.Right
    private bool IsInRange(DependencyObject sourceElement,
        DependencyObject targetElement,
        Rect sourceRect,
        Rect targetRect,
        FocusNavigationDirection direction,
        double startRange,
        double endRange)
    {
        switch (direction)
        {
            case FocusNavigationDirection.Right:
            case FocusNavigationDirection.Left:
                if (_horizontalBaseline != BASELINE_DEFAULT)
                {
                    startRange = Math.Min(startRange, _horizontalBaseline);
                    endRange = Math.Max(endRange, _horizontalBaseline);
                }

                if (DoubleUtil.GreaterThan(targetRect.Bottom, startRange) && DoubleUtil.LessThan(targetRect.Top, endRange))
                {
                    // If there is no sourceElement - checking the range is enough
                    if (sourceElement == null)
                        return true;

                    if (direction == FocusNavigationDirection.Right)
                        return DoubleUtil.GreaterThan(targetRect.Left, sourceRect.Left) || (DoubleUtil.AreClose(targetRect.Left, sourceRect.Left) && IsAncestorOfEx(sourceElement, targetElement));
                    else
                        return DoubleUtil.LessThan(targetRect.Right, sourceRect.Right) || (DoubleUtil.AreClose(targetRect.Right, sourceRect.Right) && IsAncestorOfEx(sourceElement, targetElement));
                }
                break;

            case FocusNavigationDirection.Up:
            case FocusNavigationDirection.Down:
                if (_verticalBaseline != BASELINE_DEFAULT)
                {
                    startRange = Math.Min(startRange, _verticalBaseline);
                    endRange = Math.Max(endRange, _verticalBaseline);
                }

                if (DoubleUtil.GreaterThan(targetRect.Right, startRange) && DoubleUtil.LessThan(targetRect.Left, endRange))
                {
                    // If there is no sourceElement - checking the range is enough
                    if (sourceElement == null)
                        return true;

                    if (direction == FocusNavigationDirection.Down)
                        return DoubleUtil.GreaterThan(targetRect.Top, sourceRect.Top) || (DoubleUtil.AreClose(targetRect.Top, sourceRect.Top) && IsAncestorOfEx(sourceElement, targetElement));
                    else
                        return DoubleUtil.LessThan(targetRect.Bottom, sourceRect.Bottom) || (DoubleUtil.AreClose(targetRect.Bottom, sourceRect.Bottom) && IsAncestorOfEx(sourceElement, targetElement));
                }
                break;

            default:
                throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(FocusNavigationDirection));
        }

        return false;
    }

    private DependencyObject GetActiveElementChain(DependencyObject element, bool treeViewNavigation)
    {
        DependencyObject validActiveElement = null;
        DependencyObject activeElement = element;
        while ((activeElement = GetActiveElement(activeElement)) != null)
        {
            if (IsElementEligible(activeElement, treeViewNavigation))
                validActiveElement = activeElement;
        }

        return validActiveElement;
    }

    internal bool Navigate(DependencyObject sourceElement, Key key, ModifierKeys modifiers, bool fromProcessInput = false)
    {
        bool success = false;

        switch (key)
        {
            case Key.Tab:
                success = Navigate(sourceElement,
                    new TraversalRequest(((modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) ?
                    FocusNavigationDirection.Previous : FocusNavigationDirection.Next), modifiers, fromProcessInput);
                break;

            case Key.Right:
                success = Navigate(sourceElement, new TraversalRequest(FocusNavigationDirection.Right), modifiers);
                break;

            case Key.Left:
                success = Navigate(sourceElement, new TraversalRequest(FocusNavigationDirection.Left), modifiers);
                break;

            case Key.Up:
                success = Navigate(sourceElement, new TraversalRequest(FocusNavigationDirection.Up), modifiers);
                break;

            case Key.Down:
                success = Navigate(sourceElement, new TraversalRequest(FocusNavigationDirection.Down), modifiers);
                break;
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
        IInputElement iie = e as IInputElement;
        // Focus delegation is enabled only if keyboard focus is outside the container
        if (iie != null && !iie.IsKeyboardFocusWithin)
        {
            DependencyObject focusedElement = FocusManager.GetFocusedElement(e) as DependencyObject;
            if (focusedElement != null)
            {
                if (_navigationProperty == ControlTabNavigationProperty || !IsFocusScope(e))
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

    internal bool IsTabStop(DependencyObject e)
    {
        if (e is FrameworkElement fe)
        {
            return fe.Focusable
                && (bool)fe.GetValue(IsTabStopProperty)
                && fe.IsEnabled
                && fe.IsVisible
                && INTERNAL_VisualTreeManager.IsElementInVisualTree(fe);
        }
        return false;
    }

    private bool IsGroup(DependencyObject e) => GetKeyNavigationMode(e) != KeyboardNavigationMode.Continue;

    private bool IsFocusableInternal(DependencyObject element)
    {
        if (element is UIElement uie)
        {
            return uie.Focusable && uie.IsEnabled && uie.IsVisible;
        }

        return false;
    }

    private bool IsElementEligible(DependencyObject element, bool treeViewNavigation)
    {
        if (treeViewNavigation)
        {
            return (element is TreeViewItem) && IsFocusableInternal(element);
        }
        else
        {
            return IsTabStop(element);
        }
    }

    private bool IsGroupElementEligible(DependencyObject element, bool treeViewNavigation)
    {
        if (treeViewNavigation)
        {
            return (element is TreeViewItem) && IsFocusableInternal(element);
        }
        else
        {
            return IsTabStopOrGroup(element);
        }
    }

    private KeyboardNavigationMode GetKeyNavigationMode(DependencyObject e)
    {
        return (KeyboardNavigationMode)e.GetValue(_navigationProperty);
    }

    private bool IsTabStopOrGroup(DependencyObject e)
    {
        return IsTabStop(e) || IsGroup(e);
    }

    private static int GetTabIndexHelper(DependencyObject d)
    {
        return (int)d.GetValue(TabIndexProperty);
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
        int minIndexFirstTab = int.MinValue;
        int minIndex = int.MinValue;
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
        // None groups: Tab navigation is not supported
        if (tabbingType == KeyboardNavigationMode.None)
            return null;

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

        // Focus can be called during directional navigation, so we need to restore _navigationProperty
        // when we are done.

        DependencyProperty navigationProperty = _navigationProperty;
        _navigationProperty = TabNavigationProperty;

        DependencyObject nextTab = GetNextTab(container, container, true);

        _navigationProperty = navigationProperty;

        return nextTab;
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
            if (tabbingType == KeyboardNavigationMode.Once || tabbingType == KeyboardNavigationMode.None)
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

            // If we want to continue searching inside the Once groups, we should change the navigation mode
            if (currentTabbingType == KeyboardNavigationMode.Once)
                currentTabbingType = KeyboardNavigationMode.Contained;
        }

        // If there is no next element in the group (nextTabElement == null)

        // Search up in the tree if allowed
        // consider: Use original tabbingType instead of currentTabbingType
        if (!goDownOnly && currentTabbingType != KeyboardNavigationMode.Contained && GetParent(container) != null)
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
        // None groups: Tab navigation is not supported
        if (tabbingType == KeyboardNavigationMode.None)
            return null;

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
            if (tabbingType == KeyboardNavigationMode.Once || tabbingType == KeyboardNavigationMode.None)
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

        if (tabbingType == KeyboardNavigationMode.Contained)
            return null;

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
        return FocusManager.GetIsFocusScope(e) || GetParent(e) == null;
    }

    private sealed class WeakReferenceList<T> : DispatcherObject
        where T : class
    {
        public int Count => _list.Count;

        // add a weak reference to the item
        public void Add(T item)
        {
            // before growing the list, purge it of dead entries.
            // The expense of purging amortizes to O(1) per entry, because
            // the the list doubles its capacity when it grows.
            if (_list.Count == _list.Capacity)
            {
                Purge();
            }

            _list.Add(new WeakReference<T>(item));
        }

        // remove all references to the target item
        public void Remove(object target)
        {
            bool hasDeadEntries = false;
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].TryGetTarget(out T item))
                {
                    if (item == target)
                    {
                        _list.RemoveAt(i);
                        --i;
                    }
                }
                else
                {
                    hasDeadEntries = true;
                }
            }

            if (hasDeadEntries)
            {
                Purge();
            }
        }

        // invoke the given action on each item
        public void Process<TArg0, TArg1>(Func<T, TArg0, TArg1, bool> action, TArg0 arg0, TArg1 arg1)
        {
            bool hasDeadEntries = false;
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].TryGetTarget(out T item))
                {
                    if (action(item, arg0, arg1))
                    {
                        break;
                    }
                }
                else
                {
                    hasDeadEntries = true;
                }
            }

            if (hasDeadEntries)
            {
                // some actions cause the loop to exit early (often after
                // the first call.  Don't penalize them with a synchronous
                // purge;  instead purge later when there's nothing more
                // important to do.
                ScheduleCleanup();
            }
        }

        // purge the list of dead references
        private void Purge()
        {
            int destIndex = 0;
            int n = _list.Count;

            // move valid entries toward the beginning, into one
            // contiguous block
            for (int i = 0; i < n; ++i)
            {
                if (_list[i].TryGetTarget(out _))
                {
                    _list[destIndex++] = _list[i];
                }
            }

            // remove the remaining entries and shrink the list
            if (destIndex < n)
            {
                _list.RemoveRange(destIndex, n - destIndex);

                // shrink the list if it would be less than half full otherwise.
                // This is more liberal than List<T>.TrimExcess(), because we're
                // probably in the situation where additions to the list are common.
                int newCapacity = destIndex << 1;
                if (newCapacity < _list.Capacity)
                {
                    _list.Capacity = newCapacity;
                }
            }
        }

        // schedule a cleanup pass
        private void ScheduleCleanup()
        {
            if (!_isCleanupRequested)
            {
                _isCleanupRequested = true;
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new DispatcherOperationCallback(CleanupOperation), this);
            }
        }

        private static object CleanupOperation(object arg)
        {
            var list = (WeakReferenceList<T>)arg;

            lock (list)
            {
                list.Purge();

                // cleanup is done
                list._isCleanupRequested = false;
            }

            return null;
        }

        private readonly List<WeakReference<T>> _list = new(1);
        private bool _isCleanupRequested;
    }
}
