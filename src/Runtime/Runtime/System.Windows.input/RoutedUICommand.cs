
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

using System.ComponentModel;

namespace System.Windows.Input;

/// <summary>
/// Defines an <see cref="ICommand"/> that is routed through the element tree and contains a text property.
/// </summary>
[TypeConverter(typeof(CommandConverter))]
public class RoutedUICommand : RoutedCommand
{
    private string _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedUICommand"/> class.
    /// </summary>
    public RoutedUICommand()
    {
        _text = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedUICommand"/> class, using the specified descriptive text,
    /// declared name, and owner type.
    /// </summary>
    /// <param name="text">
    /// Descriptive text for the command.
    /// </param>
    /// <param name="name">
    /// The declared name of the command for serialization.
    /// </param>
    /// <param name="ownerType">
    /// The type that is registering the command.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> or <paramref name="ownerType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="name"/> is zero.
    /// </exception>
    public RoutedUICommand(string text, string name, Type ownerType)
        : this(text, name, ownerType, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedUICommand"/> class, using the specified descriptive text,
    /// declared name, owner type, and input gestures.
    /// </summary>
    /// <param name="text">
    /// Descriptive text for the command.
    /// </param>
    /// <param name="name">
    /// The declared name of the command for serialization.
    /// </param>
    /// <param name="ownerType">
    /// The type that is registering the command.
    /// </param>
    /// <param name="inputGestures">
    /// A collection of gestures to associate with the command.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="name"/> or <paramref name="ownerType"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="name"/> is zero.
    /// </exception>
    public RoutedUICommand(string text, string name, Type ownerType, InputGestureCollection inputGestures)
        : base(name, ownerType, inputGestures)
    {
        ArgumentNullException.ThrowIfNull(text);

        _text = text;
    }

    /// <summary>
    /// Creates an instance of this class. Allows lazy initialization of InputGestureCollection and Text properties.
    /// </summary>
    /// <param name="name">Declared Name of the RoutedCommand for Serialization</param>
    /// <param name="ownerType">Type that is registering the property</param>
    /// <param name="commandId">An identifier assigned by the owning type to the command</param>
    internal RoutedUICommand(string name, Type ownerType, byte commandId)
        : base(name, ownerType, commandId)
    {
    }

    /// <summary>
    /// Gets or sets the text that describes this command.
    /// </summary>
    /// <returns>
    /// The text that describes the command. The default is an empty string.
    /// </returns>
    public string Text
    {
        get => _text ??= GetText();
        set => _text = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Fetches the text by invoking the GetUIText function on the owning type.
    /// </summary>
    /// <returns>The text for the command</returns>
    private string GetText()
    {
        if (OwnerType == typeof(ApplicationCommands))
        {
            return ApplicationCommands.GetUIText(CommandId);
        }
        else if (OwnerType == typeof(NavigationCommands))
        {
            return NavigationCommands.GetUIText(CommandId);
        }
        else if (OwnerType == typeof(MediaCommands))
        {
            return MediaCommands.GetUIText(CommandId);
        }
        else if (OwnerType == typeof(ComponentCommands))
        {
            return ComponentCommands.GetUIText(CommandId);
        }
        return null;
    }
}
