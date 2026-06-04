
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.Windows.Input;

/// <summary>
/// Maintains the registration of all access keys and the handling of interop keyboard commands between 
/// Windows Forms, Win32, and Windows Presentation Foundation (WPF).
/// </summary>
public sealed class AccessKeyManager
{
    [ThreadStatic]
    private static AccessKeyManager _accessKeyManager;

    private readonly Dictionary<string, List<WeakReference<IInputElement>>> _keyToElements = new(10);

    private AccessKeyManager() { }

    internal static AccessKeyManager Current => _accessKeyManager ??= new();

    /// <summary>
    /// Identifies the <b>AccessKeyManager.AccessKeyPressed</b> routed event.
    /// </summary>
    public static readonly RoutedEvent AccessKeyPressedEvent =
        EventManager.RegisterRoutedEvent(
            "AccessKeyPressed",
            RoutingStrategy.Bubble,
            typeof(AccessKeyPressedEventHandler),
            typeof(AccessKeyManager));

    /// <summary>
    /// Adds a handler for the <b>AccessKeyManager.AccessKeyPressed</b> attached event.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be added.
    /// </param>
    public static void AddAccessKeyPressedHandler(DependencyObject element, AccessKeyPressedEventHandler handler)
        => UIElement.AddHandler(element, AccessKeyPressedEvent, handler);

    /// <summary>
    /// Removes the specified <b>AccessKeyManager.AccessKeyPressed</b> event handler from the specified object.
    /// </summary>
    /// <param name="element">
    /// The <see cref="UIElement"/> that listens to this event.
    /// </param>
    /// <param name="handler">
    /// The event handler to be removed.
    /// </param>
    public static void RemoveAccessKeyPressedHandler(DependencyObject element, AccessKeyPressedEventHandler handler)
        => UIElement.RemoveHandler(element, AccessKeyPressedEvent, handler);

    /// <summary>
    /// Associates the specified access keys with the specified element.
    /// </summary>
    /// <param name="key">
    /// The access key.
    /// </param>
    /// <param name="element">
    /// The element to associate key with.
    /// </param>
    public static void Register(string key, IInputElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        key = NormalizeKey(key);

        AccessKeyManager akm = Current;

        lock (akm._keyToElements)
        {
            if (!akm._keyToElements.TryGetValue(key, out List<WeakReference<IInputElement>> elements))
            {
                elements = new List<WeakReference<IInputElement>>(1);
                akm._keyToElements[key] = elements;
            }
            else
            {
                // There were some elements there, remove dead ones
                PurgeDead(elements, null);
            }

            elements.Add(new WeakReference<IInputElement>(element));
        }
    }

    /// <summary>
    /// Disassociates the specified access keys from the specified element.
    /// </summary>
    /// <param name="key">
    /// The access key.
    /// </param>
    /// <param name="element">
    /// The element from which to disassociate key.
    /// </param>
    public static void Unregister(string key, IInputElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        key = NormalizeKey(key);

        AccessKeyManager akm = Current;

        lock (akm._keyToElements)
        {
            // Get all elements bound to this key and remove this element
            if (akm._keyToElements.TryGetValue(key, out List<WeakReference<IInputElement>> elements))
            {
                PurgeDead(elements, element);
                if (elements.Count == 0)
                {
                    akm._keyToElements.Remove(key);
                }
            }
        }
    }

    /// <summary>
    /// Indicates whether the specified key is registered as an access keys in the specified scope.
    /// </summary>
    /// <param name="scope">
    /// The presentation source to query for key.
    /// </param>
    /// <param name="key">
    /// The key to query.
    /// </param>
    /// <returns>
    /// true if the key is registered; otherwise, false.
    /// </returns>
    public static bool IsKeyRegistered(object scope, string key)
    {
        key = NormalizeKey(key);

        AccessKeyManager akm = Current;
        List<IInputElement> targets = akm.GetTargetsForScope(scope, key, null, AccessKeyInformation.Empty);
        return targets is not null && targets.Count > 0;
    }

    /// <summary>
    /// Processes the specified access keys as if a <see cref="UIElement.KeyDown"/> event for the key 
    /// was passed to the <see cref="AccessKeyManager"/>.
    /// </summary>
    /// <param name="scope">
    /// The scope for the access key.
    /// </param>
    /// <param name="key">
    /// The access key.
    /// </param>
    /// <param name="isMultiple">
    /// Indicates if key has multiple matches.
    /// </param>
    /// <returns>
    /// true if there are more keys that match; otherwise, false.
    /// </returns>
    public static bool ProcessKey(object scope, string key, bool isMultiple)
    {
        key = NormalizeKey(key);

        AccessKeyManager akm = Current;
        return akm.ProcessKeyForScope(scope, key, isMultiple, false) == ProcessKeyResult.MoreMatches;
    }

    private static string NormalizeKey(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        string firstCharacter = StringInfo.GetNextTextElement(key);

        if (key != firstCharacter)
        {
            throw new ArgumentException(string.Format(Strings.AccessKeyManager_NotAUnicodeCharacter, nameof(key)));
        }

        return firstCharacter.ToUpperInvariant();
    }

    internal void ProcessInput(InputEventArgs input)
    {
        if (input.Handled)
        {
            return;
        }

        if (input.RoutedEvent == Keyboard.KeyDownEvent)
        {
            OnKeyDown((KeyEventArgs)input);
        }
        else if (input.RoutedEvent == UIElement.TextInputEvent)
        {
            OnText((TextCompositionEventArgs)input);
        }
    }

    // Assumes key is already a single unicode character
    private ProcessKeyResult ProcessKeyForSender(object sender, string key, bool existsElsewhere, bool userInitiated)
    {
        // This comes from OnKeyDown or OnText and though it is a single character it might not be uppercased.
        key = key.ToUpperInvariant();

        IInputElement inputElementSender = sender as IInputElement;
        List<IInputElement> targets = GetTargetsForSender(inputElementSender, key);

        return ProcessKey(targets, key, existsElsewhere, userInitiated);
    }

    // Assumes key is already a single unicode character AND is uppercased
    private ProcessKeyResult ProcessKeyForScope(object scope, string key, bool existsElsewhere, bool userInitiated)
    {
        List<IInputElement> targets = GetTargetsForScope(scope, key, null, AccessKeyInformation.Empty);

        return ProcessKey(targets, key, existsElsewhere, userInitiated);
    }

    private ProcessKeyResult ProcessKey(List<IInputElement> targets, string key, bool existsElsewhere, bool userInitiated)
    {
        if (targets is not null)
        {
            bool oneUIElement = true;
            UIElement invokeUIElement = null;
            bool lastWasAccessed = false;

            int chosenIndex = 0;
            for (int i = 0; i < targets.Count; i++)
            {
                UIElement target = targets[i] as UIElement;
                Debug.Assert(target is not null, "Targets should only be UIElements");

                if (!target.IsEnabled)
                {
                    continue;
                }

                if (invokeUIElement is null)
                {
                    invokeUIElement = target;
                    chosenIndex = i;
                }
                else
                {
                    if (lastWasAccessed)
                    {
                        invokeUIElement = target;
                        chosenIndex = i;
                    }

                    oneUIElement = false;
                }

                lastWasAccessed = target.HasEffectiveKeyboardFocus;
            }

            if (invokeUIElement is not null)
            {
                var args = new AccessKeyEventArgs(key, isMultiple: !oneUIElement || existsElsewhere, userInitiated);
                try
                {
                    invokeUIElement.InvokeAccessKey(args);
                }
                finally
                {
                    args.ClearUserInitiated();
                }

                return chosenIndex == targets.Count - 1 ? ProcessKeyResult.LastMatch : ProcessKeyResult.MoreMatches;
            }
        }

        return ProcessKeyResult.NoMatch;
    }

    private void OnKeyDown(KeyEventArgs e)
    {
        string text = e.Key switch
        {
            Key.Enter => "\x000D",
            Key.Escape => "\x001B",
            _ => null,
        };

        if (text is not null)
        {
            if (ProcessKeyForSender(e.OriginalSource, text, existsElsewhere: false, e.UserInitiated) != ProcessKeyResult.NoMatch)
            {
                e.Handled = true;
            }
        }
    }

    private void OnText(TextCompositionEventArgs e)
    {
        string text = e.Text;
        if (!string.IsNullOrEmpty(text))
        {
            if (ProcessKeyForSender(e.OriginalSource, text, existsElsewhere: false, e.UserInitiated) != ProcessKeyResult.NoMatch)
            {
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Get the list of access key targets for the sender of the keyboard event.  If sender is null, 
    /// pretend key was pressed in the active window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private List<IInputElement> GetTargetsForSender(IInputElement sender, string key)
    {
        // Find the scope for the sender -- will be matched against the possible targets' scopes
        AccessKeyInformation senderInfo = GetInfoForElement(sender, key);

        return GetTargetsForScope(senderInfo.Scope, key, sender, senderInfo);
    }

    private List<IInputElement> GetTargetsForScope(object scope, string key, IInputElement sender, AccessKeyInformation senderInfo)
    {
        // null scope defaults to the active window
        if (scope is null)
        {
            scope = CriticalGetActiveSource();

            // if there is no active scope then give up
            if (scope is null)
            {
                return null;
            }
        }

        if (CoreCompatibilityPreferences.GetIsAltKeyRequiredInAccessKeyDefaultScope() &&
            scope is Window or PopupRoot &&
            !KeyboardNavigation.GetIsAccessKeyMode((DependencyObject)scope))
        {
            // In WPF, access keys are activated by keeping ALT pressed.
            // We cannot reliably reproduce that behavior in browsers because ALT is
            // intercepted by the browser/OS (menus, shortcuts, accessibility features)
            // and may move focus outside of the page. Instead, we use a sticky
            // access-key mode that is toggled on/off and remains active until the
            // next key press.

            // If AltKey is required and it isnt pressed then dont match against any targets
            return null;
        }

        //Scoping:
        //    1) When key is pressed, find matching AKs -> S
        //    3) find scope for keyevent.Source
        //    4) find scope for everything in S. throw away those that don't match.
        //    5) Final selection uses S.  yay!
        // 
        // 
        List<IInputElement> possibleElements = null;
        lock (_keyToElements)
        {
            if (_keyToElements.TryGetValue(key, out List<WeakReference<IInputElement>> elements))
            {
                possibleElements = CopyAndPurgeDead(elements);
            }
        }

        if (possibleElements is null)
        {
            return null;
        }

        var finalTargets = new List<IInputElement>(1);

        // Go through all the possible elements, find the interesting candidates
        for (int i = 0; i < possibleElements.Count; i++)
        {
            IInputElement element = possibleElements[i];
            if (element != sender)
            {
                if (IsTargetable(element))
                {
                    AccessKeyInformation elementInfo = GetInfoForElement(element, key);

                    if (elementInfo.Target is null)
                    {
                        continue;
                    }

                    if (scope == elementInfo.Scope)
                    {
                        finalTargets.Add(elementInfo.Target);
                    }
                }
            }
            else
            {
                // This is the same element that sent the event so it must be in the same scope.  
                // Just add it to the final targets
                if (senderInfo.Target is not null)
                {
                    finalTargets.Add(senderInfo.Target);
                }
            }
        }

        return finalTargets;
    }

    /// <summary>
    /// Returns scope for the given element.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="key"></param>
    /// <returns>Scope for the given element, null means the context global scope</returns>
    private AccessKeyInformation GetInfoForElement(IInputElement element, string key)
    {
        if (element is not null)
        {
            var args = new AccessKeyPressedEventArgs(key);
            element.RaiseEvent(args);

            return new AccessKeyInformation(args.Scope ?? GetSourceForElement(element), args.Target);
        }
        else
        {
            return new AccessKeyInformation(CriticalGetActiveSource(), null);
        }
    }

    private DependencyObject GetSourceForElement(IInputElement element)
    {
        DependencyObject source = null;

        if (element is UIElement containingVisual)
        {
            source = VisualTreeHelper.GetVisualRoot(containingVisual);
        }

        // NOTE: source can be null but IsTargetable(element) == true if the
        // element is in an orphaned tree but the tree has not yet been garbage collected.  
        return source;
    }

    private static Window CriticalGetActiveSource() => Window.Current;

    private static bool IsTargetable(IInputElement element)
    {
        // For an element to be a valid target it must be visible and enabled
        if (element is UIElement uielement && IsVisible(uielement) && IsEnabled(uielement))
        {
            return true;
        }

        return false;
    }

    private static bool IsVisible(DependencyObject element)
    {
        while (element is not null)
        {
            if (element is not UIElement uie)
            {
                break;
            }

            if (uie.Visibility != Visibility.Visible)
            {
                return false;
            }

            element = VisualTreeHelper.GetParent(uie);
        }

        return true;
    }

    // returns whether the given DO is enabled or not
    private static bool IsEnabled(DependencyObject element) => (bool)element.GetValue(UIElement.IsEnabledProperty);

    private static void PurgeDead(List<WeakReference<IInputElement>> elements, IInputElement elementToRemove)
    {
        for (int i = 0; i < elements.Count;)
        {
            WeakReference<IInputElement> weakReference = elements[i];

            if (!weakReference.TryGetTarget(out IInputElement element) || element == elementToRemove)
            {
                elements.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    ///     Takes an ArrayList of WeakReferences, removes the dead references and returns
    ///     a generic List of IInputElements (strong references)
    /// </summary>
    private static List<IInputElement> CopyAndPurgeDead(List<WeakReference<IInputElement>> elements)
    {
        if (elements is null)
        {
            return null;
        }

        var copy = new List<IInputElement>(elements.Count);

        for (int i = 0; i < elements.Count;)
        {
            WeakReference<IInputElement> weakReference = elements[i];

            if (!weakReference.TryGetTarget(out IInputElement element))
            {
                elements.RemoveAt(i);
            }
            else
            {
                copy.Add(element);
                i++;
            }
        }

        return copy;
    }

    /////////////////////////////////////////////////////////////////////////////////
    // Overview: Algorithm to look up access key from the element for which it is a target.
    //           
    //     When the AccessKeyCharacter for an element is requested we see if there
    //     is a corresponding AccessKeyElement stashed on the element.  If there is,
    //     raise the AccessKeyPressed event on it to see if that element is  still the 
    //     target for it.  If not, go through all registered accesskeys and get their 
    //     targets until we find the desired element.  The "primary" access key character
    //     is the first one we find.
    //     
    //     Note: The algorithm ends up being O(n) for each request for AccessKeyCharacter 
    //     because there is no mapping from AccessKeyElement to its "primary" character.
    //     Maintaining this would require storing a hash from AccessKeyElement to character
    //     or requiring that each element registered implement an interface or some other
    //     kind of contract.  Because we don't keep track of this or enforce this, to find 
    //     the "primary" character we must go through all registered pairs of 
    //     (character, element) to find the character -- O(n).
    //
    //     This ends up being just fine, because in any given context there shouldn't be
    //     so many elements registered that this cost is at all noticable.
    //
    /////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     The primary access key element for an element.  This is stored as a WeakReference.
    /// </summary>
    private static readonly DependencyProperty AccessKeyElementProperty =
        DependencyProperty.RegisterAttached(
            "AccessKeyElement",
            typeof(WeakReference<IInputElement>),
            typeof(AccessKeyManager));

    internal static string InternalGetAccessKeyCharacter(DependencyObject d) => Current.GetAccessKeyCharacter(d);

    private string GetAccessKeyCharacter(DependencyObject d)
    {
        // See what the local value for AccessKeyElement is first and start with that.
        var cachedElementWeakRef = (WeakReference<IInputElement>)d.GetValue(AccessKeyElementProperty);

        if (cachedElementWeakRef is not null && cachedElementWeakRef.TryGetTarget(out IInputElement accessKeyElement))
        {
            // First figure out if the target of accessKeyElement is still "d", then go find
            // the "primary" character for the accessKeyElement.  

            var accessKeyPressedEventArgs = new AccessKeyPressedEventArgs();
            accessKeyElement.RaiseEvent(accessKeyPressedEventArgs);
            if (accessKeyPressedEventArgs.Target == d)
            {
                // Because there is no way to get at the access key element's character from the
                // element (there is no interface or anything) we have to go through all registered 
                // access keys and see if this access key element is still registered and what its
                // "primary" character is.

                foreach (var entry in Current._keyToElements)
                {
                    List<WeakReference<IInputElement>> elements = entry.Value;
                    for (int i = 0; i < elements.Count; i++)
                    {
                        // If this element matches accessKeyElement, then return the current character
                        WeakReference<IInputElement> currentElementWeakRef = elements[i];

                        if (currentElementWeakRef.TryGetTarget(out IInputElement target) && target == accessKeyElement)
                        {
                            return entry.Key;
                        }
                    }
                }
            }
        }


        // There was no access key stored or it no longer matched.  Clear out the cache and figure it out again.
        d.ClearValue(AccessKeyElementProperty);

        foreach (var entry in Current._keyToElements)
        {
            List<WeakReference<IInputElement>> elements = entry.Value;
            for (int i = 0; i < elements.Count; i++)
            {
                // Determine the target for this element.  Cache the weak reference for the element on the target.
                WeakReference<IInputElement> currentElementWeakRef = elements[i];

                if (currentElementWeakRef.TryGetTarget(out IInputElement currentElement))
                {
                    var accessKeyPressedEventArgs = new AccessKeyPressedEventArgs();
                    currentElement.RaiseEvent(accessKeyPressedEventArgs);

                    // If the target was non-null, cache the access key element on the target.
                    // if the target matches "d", return the current character.
                    if (accessKeyPressedEventArgs.Target is not null)
                    {
                        accessKeyPressedEventArgs.Target.SetValue(AccessKeyElementProperty, currentElementWeakRef);

                        if (accessKeyPressedEventArgs.Target == d)
                        {
                            return entry.Key;
                        }
                    }
                }
            }
        }


        return string.Empty;
    }

    private enum ProcessKeyResult
    {
        NoMatch,
        MoreMatches,
        LastMatch
    }

    private readonly struct AccessKeyInformation
    {
        public AccessKeyInformation(object scope, UIElement target)
        {
            Scope = scope;
            Target = target;
        }

        public readonly object Scope;
        public readonly UIElement Target;

        public static readonly AccessKeyInformation Empty = new(null, null);
    }
}
