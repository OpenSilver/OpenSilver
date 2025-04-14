
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

namespace System.Windows.Input;

/// <summary>
/// Provides a standard set of component-related commands, which have predefined key input gestures and <see cref="RoutedUICommand.Text"/> properties.
/// </summary>
public static class ComponentCommands
{
    /// <summary>
    /// Gets the value that represents the Extend Selection Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+Down UI Text Extend Selection Down
    /// </returns>
    public static RoutedUICommand ExtendSelectionDown => EnsureCommand(CommandId.ExtendSelectionDown);

    /// <summary>
    /// Gets the value that represents the Select To Home command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+Home UI Text Select To Home
    /// </returns>
    public static RoutedUICommand SelectToHome => EnsureCommand(CommandId.SelectToHome);

    /// <summary>
    /// Gets the value that represents the Select To End command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+End UI Text Select To End
    /// </returns>
    public static RoutedUICommand SelectToEnd => EnsureCommand(CommandId.SelectToEnd);

    /// <summary>
    /// Gets the value that represents the Scroll Page Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture PageUp UI Text Scroll Page Up
    /// </returns>
    public static RoutedUICommand ScrollPageUp => EnsureCommand(CommandId.ScrollPageUp);

    /// <summary>
    /// Gets the value that represents the Scroll Page Right command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Scroll Page Right
    /// </returns>
    public static RoutedUICommand ScrollPageRight => EnsureCommand(CommandId.ScrollPageRight);

    /// <summary>
    /// Gets the value that represents the Scroll Page Left command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Scroll Page Left
    /// </returns>
    public static RoutedUICommand ScrollPageLeft => EnsureCommand(CommandId.ScrollPageLeft);

    /// <summary>
    /// Gets the value that represents the Scroll Page Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture PageDown UI Text Scroll Page Down
    /// </returns>
    public static RoutedUICommand ScrollPageDown => EnsureCommand(CommandId.ScrollPageDown);

    /// <summary>
    /// Gets the value that represents the Scroll By Line command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined UI Text Scroll By Line
    /// </returns>
    public static RoutedUICommand ScrollByLine => EnsureCommand(CommandId.ScrollByLine);

    /// <summary>
    /// Gets the value that represents the Move Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Up UI Text Move Up
    /// </returns>
    public static RoutedUICommand MoveUp => EnsureCommand(CommandId.MoveUp);

    /// <summary>
    /// Gets the value that represents the Move To Page Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture PageUp UI Text Move To Page Up
    /// </returns>
    public static RoutedUICommand MoveToPageUp => EnsureCommand(CommandId.MoveToPageUp);

    /// <summary>
    /// Gets the value that represents the Move To Page Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture PageDown UI Text Move To Page Down
    /// </returns>
    public static RoutedUICommand MoveToPageDown => EnsureCommand(CommandId.MoveToPageDown);

    /// <summary>
    /// Gets the value that represents the Move To Home command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Home UI Text Move To Home
    /// </returns>
    public static RoutedUICommand MoveToHome => EnsureCommand(CommandId.MoveToHome);

    /// <summary>
    /// Gets the value that represents the Select To Page Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+PageDown UI Text Select To Page Down
    /// </returns>
    public static RoutedUICommand SelectToPageDown => EnsureCommand(CommandId.SelectToPageDown);

    /// <summary>
    /// Gets the value that represents the Move To End command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture End UI Text Move To End
    /// </returns>
    public static RoutedUICommand MoveToEnd => EnsureCommand(CommandId.MoveToEnd);

    /// <summary>
    /// Gets the value that represents the Move Left command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Left UI Text Move Left
    /// </returns>
    public static RoutedUICommand MoveLeft => EnsureCommand(CommandId.MoveLeft);

    /// <summary>
    /// Gets the value that represents the Move Focus Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+Up UI Text Move Focus Up
    /// </returns>
    public static RoutedUICommand MoveFocusUp => EnsureCommand(CommandId.MoveFocusUp);

    /// <summary>
    /// Gets the value that represents the Move Focus Page Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+PageUp UI Text Move Focus Page Up
    /// </returns>
    public static RoutedUICommand MoveFocusPageUp => EnsureCommand(CommandId.MoveFocusPageUp);

    /// <summary>
    /// Gets the value that represents the Move Focus Page Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+PageDown UI Text Move Focus Page Down
    /// </returns>
    public static RoutedUICommand MoveFocusPageDown => EnsureCommand(CommandId.MoveFocusPageDown);

    /// <summary>
    /// Gets the value that represents the Move Focus Forward command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+Right UI Text Move Focus Forward
    /// </returns>
    public static RoutedUICommand MoveFocusForward => EnsureCommand(CommandId.MoveFocusForward);

    /// <summary>
    /// Gets the value that represents the Move Focus Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+Down UI Text Move Focus Down
    /// </returns>
    public static RoutedUICommand MoveFocusDown => EnsureCommand(CommandId.MoveFocusDown);

    /// <summary>
    /// Gets the value that represents the Move Focus Back command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+Left UI Text Move Focus Back
    /// </returns>
    public static RoutedUICommand MoveFocusBack => EnsureCommand(CommandId.MoveFocusBack);

    /// <summary>
    /// Gets the value that represents the Move Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Down UI Text Move Down
    /// </returns>
    public static RoutedUICommand MoveDown => EnsureCommand(CommandId.MoveDown);

    /// <summary>
    /// Gets the value that represents the Extend Selection Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+Up UI Text Extend Selection Up
    /// </returns>
    public static RoutedUICommand ExtendSelectionUp => EnsureCommand(CommandId.ExtendSelectionUp);

    /// <summary>
    /// Gets the value that represents the Extend Selection Right command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+Right UI Text Extend Selection Right
    /// </returns>
    public static RoutedUICommand ExtendSelectionRight => EnsureCommand(CommandId.ExtendSelectionRight);

    /// <summary>
    /// Gets the value that represents the Extend Selection Left command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+Left UI Text Extend Selection Left
    /// </returns>
    public static RoutedUICommand ExtendSelectionLeft => EnsureCommand(CommandId.ExtendSelectionLeft);

    /// <summary>
    /// Gets the value that represents the Move Right command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Right UI Text Move Right
    /// </returns>
    public static RoutedUICommand MoveRight => EnsureCommand(CommandId.MoveRight);

    /// <summary>
    /// Gets the value that represents the Select To Page Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+PageUp UI Text Select To Page Up
    /// </returns>
    public static RoutedUICommand SelectToPageUp => EnsureCommand(CommandId.SelectToPageUp);

    private static string GetPropertyName(CommandId commandId)
    {
        return commandId switch
        {
            CommandId.ScrollPageUp => "ScrollPageUp",
            CommandId.ScrollPageDown => "ScrollPageDown",
            CommandId.ScrollPageLeft => "ScrollPageLeft",
            CommandId.ScrollPageRight => "ScrollPageRight",
            CommandId.ScrollByLine => "ScrollByLine",
            CommandId.MoveLeft => "MoveLeft",
            CommandId.MoveRight => "MoveRight",
            CommandId.MoveUp => "MoveUp",
            CommandId.MoveDown => "MoveDown",
            CommandId.ExtendSelectionUp => "ExtendSelectionUp",
            CommandId.ExtendSelectionDown => "ExtendSelectionDown",
            CommandId.ExtendSelectionLeft => "ExtendSelectionLeft",
            CommandId.ExtendSelectionRight => "ExtendSelectionRight",
            CommandId.MoveToHome => "MoveToHome",
            CommandId.MoveToEnd => "MoveToEnd",
            CommandId.MoveToPageUp => "MoveToPageUp",
            CommandId.MoveToPageDown => "MoveToPageDown",
            CommandId.SelectToHome => "SelectToHome",
            CommandId.SelectToEnd => "SelectToEnd",
            CommandId.SelectToPageDown => "SelectToPageDown",
            CommandId.SelectToPageUp => "SelectToPageUp",
            CommandId.MoveFocusUp => "MoveFocusUp",
            CommandId.MoveFocusDown => "MoveFocusDown",
            CommandId.MoveFocusBack => "MoveFocusBack",
            CommandId.MoveFocusForward => "MoveFocusForward",
            CommandId.MoveFocusPageUp => "MoveFocusPageUp",
            CommandId.MoveFocusPageDown => "MoveFocusPageDown",
            _ => string.Empty,
        };
    }

    internal static string GetUIText(byte commandId)
    {
        return (CommandId)commandId switch
        {
            CommandId.ScrollPageUp => Strings.ScrollPageUpText,
            CommandId.ScrollPageDown => Strings.ScrollPageDownText,
            CommandId.ScrollPageLeft => Strings.ScrollPageLeftText,
            CommandId.ScrollPageRight => Strings.ScrollPageRightText,
            CommandId.ScrollByLine => Strings.ScrollByLineText,
            CommandId.MoveLeft => Strings.MoveLeftText,
            CommandId.MoveRight => Strings.MoveRightText,
            CommandId.MoveUp => Strings.MoveUpText,
            CommandId.MoveDown => Strings.MoveDownText,
            CommandId.ExtendSelectionUp => Strings.ExtendSelectionUpText,
            CommandId.ExtendSelectionDown => Strings.ExtendSelectionDownText,
            CommandId.ExtendSelectionLeft => Strings.ExtendSelectionLeftText,
            CommandId.ExtendSelectionRight => Strings.ExtendSelectionRightText,
            CommandId.MoveToHome => Strings.MoveToHomeText,
            CommandId.MoveToEnd => Strings.MoveToEndText,
            CommandId.MoveToPageUp => Strings.MoveToPageUpText,
            CommandId.MoveToPageDown => Strings.MoveToPageDownText,
            CommandId.SelectToHome => Strings.SelectToHomeText,
            CommandId.SelectToEnd => Strings.SelectToEndText,
            CommandId.SelectToPageDown => Strings.SelectToPageDownText,
            CommandId.SelectToPageUp => Strings.SelectToPageUpText,
            CommandId.MoveFocusUp => Strings.MoveFocusUpText,
            CommandId.MoveFocusDown => Strings.MoveFocusDownText,
            CommandId.MoveFocusBack => Strings.MoveFocusBackText,
            CommandId.MoveFocusForward => Strings.MoveFocusForwardText,
            CommandId.MoveFocusPageUp => Strings.MoveFocusPageUpText,
            CommandId.MoveFocusPageDown => Strings.MoveFocusPageDownText,
            _ => string.Empty,
        };
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        InputGestureCollection gestures = [];

        // Standard Commands
        switch ((CommandId)commandId)
        {
            case CommandId.ScrollPageUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "PageUp",
                    Strings.ScrollPageUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.ScrollPageDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "PageDown",
                    Strings.ScrollPageDownKeyDisplayString,
                    gestures);
                break;
            case CommandId.ScrollPageLeft:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.ScrollPageRight:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.ScrollByLine:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.MoveLeft:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Left",
                    Strings.MoveLeftKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveRight:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Right",
                    Strings.MoveRightKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Up",
                    Strings.MoveUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Down",
                    Strings.MoveDownKeyDisplayString,
                    gestures);
                break;
            case CommandId.ExtendSelectionUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+Up",
                    Strings.ExtendSelectionUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.ExtendSelectionDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+Down",
                    Strings.ExtendSelectionDownKeyDisplayString,
                    gestures);
                break;
            case CommandId.ExtendSelectionLeft:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+Left",
                    Strings.ExtendSelectionLeftKeyDisplayString,
                    gestures);
                break;
            case CommandId.ExtendSelectionRight:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+Right",
                    Strings.ExtendSelectionRightKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveToHome:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Home",
                    Strings.MoveToHomeKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveToEnd:
                KeyGesture.AddGesturesFromResourceStrings(
                    "End",
                    Strings.MoveToEndKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveToPageUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "PageUp",
                    Strings.MoveToPageUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveToPageDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "PageDown",
                    Strings.MoveToPageDownKeyDisplayString,
                    gestures);
                break;
            case CommandId.SelectToHome:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+Home",
                    Strings.SelectToHomeKeyDisplayString,
                    gestures);
                break;
            case CommandId.SelectToEnd:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+End",
                    Strings.SelectToEndKeyDisplayString,
                    gestures);
                break;
            case CommandId.SelectToPageDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+PageDown",
                    Strings.SelectToPageDownKeyDisplayString,
                    gestures);
                break;
            case CommandId.SelectToPageUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+PageUp",
                    Strings.SelectToPageUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveFocusUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+Up",
                    Strings.MoveFocusUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveFocusDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+Down",
                    Strings.MoveFocusDownKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveFocusBack:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+Left",
                    Strings.MoveFocusBackKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveFocusForward:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+Right",
                    Strings.MoveFocusForwardKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveFocusPageUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+PageUp",
                    Strings.MoveFocusPageUpKeyDisplayString,
                    gestures);
                break;
            case CommandId.MoveFocusPageDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+PageDown",
                    Strings.MoveFocusPageDownKeyDisplayString,
                    gestures);
                break;
        }

        return gestures;
    }

    private static RoutedUICommand EnsureCommand(CommandId idCommand)
    {
        if (idCommand >= 0 && idCommand < CommandId.Last)
        {
            lock (_internalCommands.SyncRoot)
            {
                if (_internalCommands[(int)idCommand] is null)
                {
                    RoutedUICommand newCommand = CommandLibraryHelper.CreateUICommand(
                        GetPropertyName(idCommand),
                        typeof(ComponentCommands),
                        (byte)idCommand);

                    _internalCommands[(int)idCommand] = newCommand;
                }
            }
            return _internalCommands[(int)idCommand];
        }
        return null;
    }

    private enum CommandId : byte
    {
        // Formatting
        ScrollPageUp = 1,
        ScrollPageDown = 2,
        ScrollPageLeft = 3,
        ScrollPageRight = 4,
        ScrollByLine = 5,
        MoveLeft = 6,
        MoveRight = 7,
        MoveUp = 8,
        MoveDown = 9,
        MoveToHome = 10,
        MoveToEnd = 11,
        MoveToPageUp = 12,
        MoveToPageDown = 13,
        SelectToHome = 14,
        SelectToEnd = 15,
        SelectToPageUp = 16,
        SelectToPageDown = 17,
        MoveFocusUp = 18,
        MoveFocusDown = 19,
        MoveFocusForward = 20,
        MoveFocusBack = 21,
        MoveFocusPageUp = 22,
        MoveFocusPageDown = 23,
        ExtendSelectionLeft = 24,
        ExtendSelectionRight = 25,
        ExtendSelectionUp = 26,
        ExtendSelectionDown = 27,

        // Last
        Last = 28
    }

    private static readonly RoutedUICommand[] _internalCommands = new RoutedUICommand[(int)CommandId.Last];
}
