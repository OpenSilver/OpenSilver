
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

using System.Windows.Threading;

namespace System.Windows.Input;

/// <summary>
/// Provides facilities for managing events related to input and text compositions.
/// </summary>
public sealed class TextCompositionManager : DispatcherObject
{
    internal TextCompositionManager() { }

    /// <summary>
    /// Identifies the <b>TextCompositionManager.PreviewTextInputStart</b> attached event.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly RoutedEvent PreviewTextInputStartEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewTextInputStart",
            RoutingStrategy.Tunnel,
            typeof(TextCompositionEventHandler),
            typeof(TextCompositionManager));

    /// <summary>
    /// Adds a handler for the <b>TextCompositionManager.PreviewTextInputStart</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to add the event handler to. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to add.
    /// </param>
    [OpenSilver.NotImplemented]
    public static void AddPreviewTextInputStartHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.AddHandler(element, PreviewTextInputStartEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the <b>TextCompositionManager.TextInputStart</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to remove the event handler from. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to remove.
    /// </param>
    [OpenSilver.NotImplemented]
    public static void RemovePreviewTextInputStartHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.RemoveHandler(element, PreviewTextInputStartEvent, handler);
    }

    /// <summary>
    /// Identifies the <b>TextCompositionManager.TextInputStart</b> attached event.
    /// </summary>
    public static readonly RoutedEvent TextInputStartEvent =
        EventManager.RegisterCoreEvent(
            "TextInputStart",
            RoutingStrategy.Bubble,
            typeof(TextCompositionEventHandler),
            typeof(TextCompositionManager));

    /// <summary>
    /// Adds a handler for the <b>TextCompositionManager.TextInputStart</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to add the event handler to. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to add.
    /// </param>
    public static void AddTextInputStartHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.AddHandler(element, TextInputStartEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the <b>TextCompositionManager.TextInputStart</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to remove the event handler from. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to remove.
    /// </param>
    public static void RemoveTextInputStartHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.RemoveHandler(element, TextInputStartEvent, handler);
    }

    /// <summary>
    /// Identifies the <b>TextCompositionManager.PreviewTextInput</b> attached event.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly RoutedEvent PreviewTextInputEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewTextInput",
            RoutingStrategy.Tunnel,
            typeof(TextCompositionEventHandler),
            typeof(TextCompositionManager));

    /// <summary>
    /// Adds a handler for the <b>TextCompositionManager.PreviewTextInput</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to add the event handler to. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to add.
    /// </param>
    [OpenSilver.NotImplemented]
    public static void AddPreviewTextInputHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.AddHandler(element, PreviewTextInputEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the <b>TextCompositionManager.PreviewTextInput</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to remove the event handler from. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to remove.
    /// </param>
    [OpenSilver.NotImplemented]
    public static void RemovePreviewTextInputHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.RemoveHandler(element, PreviewTextInputEvent, handler);
    }

    /// <summary>
    /// Identifies the <b>TextCompositionManager.TextInput</b> attached event.
    /// </summary>
    public static readonly RoutedEvent TextInputEvent =
        EventManager.RegisterCoreEvent(
            "TextInput",
            RoutingStrategy.Bubble,
            typeof(TextCompositionEventHandler),
            typeof(TextCompositionManager));

    /// <summary>
    /// Adds a handler for the <b>TextCompositionManager.TextInput</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to add the event handler to. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to add.
    /// </param>
    public static void AddTextInputHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.AddHandler(element, TextInputEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the <b>TextCompositionManager.TextInput</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to remove the event handler from. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to remove.
    /// </param>
    public static void RemoveTextInputHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.RemoveHandler(element, TextInputEvent, handler);
    }

    /// <summary>
    /// Identifies the <b>TextCompositionManager.PreviewTextInputUpdate</b> attached event.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static readonly RoutedEvent PreviewTextInputUpdateEvent =
        EventManager.RegisterRoutedEvent(
            "PreviewTextInputUpdate",
            RoutingStrategy.Tunnel,
            typeof(TextCompositionEventHandler),
            typeof(TextCompositionManager));

    /// <summary>
    /// Adds a handler for the <b>TextCompositionManager.PreviewTextInputUpdate</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to add the event handler to. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to add.
    /// </param>
    [OpenSilver.NotImplemented]
    public static void AddPreviewTextInputUpdateHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.AddHandler(element, PreviewTextInputUpdateEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the <b>TextCompositionManager.PreviewTextInputUpdate</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to remove the event handler from. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to remove.
    /// </param>
    [OpenSilver.NotImplemented]
    public static void RemovePreviewTextInputUpdateHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.RemoveHandler(element, PreviewTextInputUpdateEvent, handler);
    }

    /// <summary>
    /// Identifies the <b>TextCompositionManager.TextInputUpdate</b> attached event.
    /// </summary>
    public static readonly RoutedEvent TextInputUpdateEvent =
        EventManager.RegisterCoreEvent(
            "TextInputUpdate",
            RoutingStrategy.Bubble,
            typeof(TextCompositionEventHandler),
            typeof(TextCompositionManager));

    /// <summary>
    /// Adds a handler for the <b>TextCompositionManager.TextInputUpdate</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to add the event handler to. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to add.
    /// </param>
    public static void AddTextInputUpdateHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.AddHandler(element, TextInputUpdateEvent, handler);
    }

    /// <summary>
    /// Removes a handler for the <b>TextCompositionManager.TextInputUpdate</b> attached event.
    /// </summary>
    /// <param name="element">
    /// A dependency object to remove the event handler from. The dependency object must be a <see cref="UIElement"/>.
    /// </param>
    /// <param name="handler">
    /// A delegate that designates the handler to remove.
    /// </param>
    public static void RemoveTextInputUpdateHandler(DependencyObject element, TextCompositionEventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(element);

        UIElement.RemoveHandler(element, TextInputUpdateEvent, handler);
    }
}
