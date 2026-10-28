
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
using System.Windows;

namespace OpenSilver;

/// <summary>
/// Receives notifications from the runtime: layout passes, visibility changes, elements leaving
/// the visual tree, and <see cref="ResponsiveExtension"/> values that switch.
/// </summary>
/// <remarks>
/// Used by OpenSilver.Animations through InternalsVisibleTo (see AssemblyInfo.cs): when you change
/// this class, update that library too.
/// The measure and arrange notifications are raised while the dispatcher processing is
/// disabled. Observers should only record data there, and post any work that needs the
/// dispatcher. All notifications are raised on the UI thread.
/// </remarks>
internal abstract class InternalHooks
{
    private static InternalHooks[] _observers;

    /// <summary>
    /// Registers an observer for the lifetime of the application.
    /// </summary>
    /// <param name="observer">
    /// The observer to register.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// observer is null.
    /// </exception>
    internal static void Register(InternalHooks observer)
    {
        if (observer is null)
        {
            throw new ArgumentNullException(nameof(observer));
        }

        InternalHooks[] observers = _observers;
        if (observers is null)
        {
            _observers = new[] { observer };
        }
        else
        {
            var copy = new InternalHooks[observers.Length + 1];
            Array.Copy(observers, copy, observers.Length);
            copy[observers.Length] = observer;
            _observers = copy;
        }
    }

    /// <summary>
    /// Called at the end of <see cref="UIElement.Measure(Size)"/>, when the element was actually measured.
    /// </summary>
    /// <param name="element">
    /// The measured element.
    /// </param>
    /// <param name="sameAvailableSize">
    /// true if the available size is the same as the previous one.
    /// </param>
    /// <param name="previousDesiredSize">
    /// The desired size before this measure. The new one is <see cref="UIElement.DesiredSize"/>.
    /// </param>
    /// <param name="isFirstMeasure">
    /// true if the element was never measured since it was attached to the visual tree.
    /// </param>
    protected internal virtual void OnMeasured(UIElement element, bool sameAvailableSize, Size previousDesiredSize, bool isFirstMeasure) { }

    /// <summary>
    /// Called by <see cref="UIElement.Arrange(Rect)"/>, before the element arranges itself and its children.
    /// </summary>
    /// <param name="element">
    /// The element being arranged.
    /// </param>
    /// <param name="finalRect">
    /// The rectangle the element is arranged in.
    /// </param>
    /// <param name="previousFinalRect">
    /// The rectangle the element was previously arranged in.
    /// </param>
    /// <param name="isFirstArrange">
    /// true if the element was never arranged since it was attached to the visual tree.
    /// </param>
    protected internal virtual void OnArranging(UIElement element, Rect finalRect, Rect previousFinalRect, bool isFirstArrange) { }

    /// <summary>
    /// Called by <see cref="UIElement.Arrange(Rect)"/>, after the element and its children have been arranged.
    /// Always paired with a previous call to <see cref="OnArranging(UIElement, Rect, Rect, bool)"/>, unless
    /// an exception was thrown during the arrange.
    /// </summary>
    /// <param name="element">
    /// The arranged element.
    /// </param>
    /// <param name="previousVisualRect">
    /// The position (relative to the visual parent) and size of the element before this arrange.
    /// </param>
    /// <param name="visualRect">
    /// The position (relative to the visual parent) and size of the element after this arrange.
    /// </param>
    /// <param name="rendered">
    /// true if the new position and size were written to the DOM.
    /// </param>
    protected internal virtual void OnArranged(UIElement element, Rect previousVisualRect, Rect visualRect, bool rendered) { }

    /// <summary>
    /// Called after the <see cref="UIElement.Visibility"/> of an element has changed and the layout
    /// system has been notified.
    /// </summary>
    /// <param name="element">
    /// The element whose visibility changed.
    /// </param>
    /// <param name="oldValue">
    /// The old visibility.
    /// </param>
    /// <param name="newValue">
    /// The new visibility.
    /// </param>
    protected internal virtual void OnVisibilityChanged(UIElement element, Visibility oldValue, Visibility newValue) { }

    /// <summary>
    /// Called when an element that is in the visual tree is about to be detached from its visual parent.
    /// The DOM of the element still exists at this point, and it is removed after any JavaScript
    /// queued by this method.
    /// </summary>
    /// <param name="child">
    /// The element being detached.
    /// </param>
    /// <param name="parent">
    /// The visual parent of the element.
    /// </param>
    protected internal virtual void OnDetaching(UIElement child, UIElement parent) { }

    /// <summary>
    /// Called when the value of a <see cref="ResponsiveExtension"/> switches because the width it watches
    /// crossed a breakpoint, after the new value has been applied. This happens in the <see cref="FrameworkElement.SizeChanged"/>
    /// event of the watched element, so during a layout pass, between its measure and arrange rounds.
    /// </summary>
    /// <param name="element">
    /// The element the value applies to, or the element that owns the object it applies to, for example
    /// the <see cref="System.Windows.Controls.Grid"/> of a <see cref="System.Windows.Controls.ColumnDefinition"/>.
    /// </param>
    /// <param name="target">
    /// The object whose property changed.
    /// </param>
    /// <param name="property">
    /// The property that changed.
    /// </param>
    protected internal virtual void OnResponsiveValueChanged(FrameworkElement element, DependencyObject target, DependencyProperty property) { }

    /// <summary>
    /// Called at the end of each layout pass, before the pending JavaScript is sent to the browser.
    /// </summary>
    protected internal virtual void OnLayoutPassCompleted() { }

    internal static void NotifyMeasured(UIElement element, bool sameAvailableSize, Size previousDesiredSize, bool isFirstMeasure)
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnMeasured(element, sameAvailableSize, previousDesiredSize, isFirstMeasure);
            }
        }
    }

    internal static void NotifyArranging(UIElement element, Rect finalRect, Rect previousFinalRect, bool isFirstArrange)
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnArranging(element, finalRect, previousFinalRect, isFirstArrange);
            }
        }
    }

    internal static void NotifyArranged(UIElement element, Rect previousVisualRect, Rect visualRect, bool rendered)
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnArranged(element, previousVisualRect, visualRect, rendered);
            }
        }
    }

    internal static void NotifyVisibilityChanged(UIElement element, Visibility oldValue, Visibility newValue)
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnVisibilityChanged(element, oldValue, newValue);
            }
        }
    }

    internal static void NotifyDetaching(UIElement child, UIElement parent)
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnDetaching(child, parent);
            }
        }
    }

    internal static void NotifyResponsiveValueChanged(FrameworkElement element, DependencyObject target, DependencyProperty property)
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnResponsiveValueChanged(element, target, property);
            }
        }
    }

    internal static void NotifyLayoutPassCompleted()
    {
        if (_observers is InternalHooks[] observers)
        {
            foreach (InternalHooks observer in observers)
            {
                observer.OnLayoutPassCompleted();
            }
        }
    }
}
