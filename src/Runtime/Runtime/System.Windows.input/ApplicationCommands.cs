
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
/// Provides a standard set of application related commands.
/// </summary>
public static class ApplicationCommands
{
    /// <summary>
    /// Gets the value that represents the Cancel Print command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Cancel Print
    /// </returns>
    public static RoutedUICommand CancelPrint => EnsureCommand(CommandId.CancelPrint);

    /// <summary>
    /// Gets the value that represents the Select All command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+A UI Text Select All
    /// </returns>
    public static RoutedUICommand SelectAll => EnsureCommand(CommandId.SelectAll);

    /// <summary>
    /// Gets the value that represents the Save As command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Save As
    /// </returns>
    public static RoutedUICommand SaveAs => EnsureCommand(CommandId.SaveAs);

    /// <summary>
    /// Gets the value that represents the Save command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+S UI Text Save
    /// </returns>
    public static RoutedUICommand Save => EnsureCommand(CommandId.Save);

    /// <summary>
    /// Gets the value that represents the Replace command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+H UI Text Replace
    /// </returns>
    public static RoutedUICommand Replace => EnsureCommand(CommandId.Replace);

    /// <summary>
    /// Gets the value that represents the Redo command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+Y UI Text Redo
    /// </returns>
    public static RoutedUICommand Redo => EnsureCommand(CommandId.Redo);

    /// <summary>
    /// Gets the value that represents the Properties command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture F4 UI Text Properties
    /// </returns>
    public static RoutedUICommand Properties => EnsureCommand(CommandId.Properties);

    /// <summary>
    /// Gets the value that represents the Print Preview command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+F2 UI Text Print Preview
    /// </returns>
    public static RoutedUICommand PrintPreview => EnsureCommand(CommandId.PrintPreview);

    /// <summary>
    /// Gets the value that represents the Print command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+P UI Text Print
    /// </returns>
    public static RoutedUICommand Print => EnsureCommand(CommandId.Print);

    /// <summary>
    /// Gets the value that represents the Paste command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+V Shift+Insert UI Text Paste
    /// </returns>
    public static RoutedUICommand Paste => EnsureCommand(CommandId.Paste);

    /// <summary>
    /// Gets the value that represents the Stop command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Esc UI Text Stop
    /// </returns>
    public static RoutedUICommand Stop => EnsureCommand(CommandId.Stop);

    /// <summary>
    /// Gets the value that represents the Open command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+O UI Text Open
    /// </returns>
    public static RoutedUICommand Open => EnsureCommand(CommandId.Open);

    /// <summary>
    /// Gets the value that represents the New command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+N UI Text New
    /// </returns>
    public static RoutedUICommand New => EnsureCommand(CommandId.New);

    /// <summary>
    /// Gets the value that represents the Help command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture F1 UI Text Help
    /// </returns>
    public static RoutedUICommand Help => EnsureCommand(CommandId.Help);

    /// <summary>
    /// Gets the value that represents the Find command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+F UI Text Find
    /// </returns>
    public static RoutedUICommand Find => EnsureCommand(CommandId.Find);

    /// <summary>
    /// Gets the value that represents the Delete command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Del UI Text Delete
    /// </returns>
    public static RoutedUICommand Delete => EnsureCommand(CommandId.Delete);

    /// <summary>
    /// Gets the value that represents the Cut command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+X Shift+Delete UI Text Cut
    /// </returns>
    public static RoutedUICommand Cut => EnsureCommand(CommandId.Cut);

    /// <summary>
    /// Gets the value that represents the Correction List command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Correction List
    /// </returns>
    public static RoutedUICommand CorrectionList => EnsureCommand(CommandId.CorrectionList);

    /// <summary>
    /// Gets the value that represents the Copy command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl+C Ctrl+Insert UI Text Copy
    /// </returns>
    public static RoutedUICommand Copy => EnsureCommand(CommandId.Copy);

    /// <summary>
    /// Gets the value that represents the Context Menu command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Shift+F10 Apps Mouse Gesture A Mouse Gesture is not attached to 
    /// this command, but most applications follow the convention of using the Right Click gesture to invoke the 
    /// context menu. UI Text Context Menu
    /// </returns>
    public static RoutedUICommand ContextMenu => EnsureCommand(CommandId.ContextMenu);

    /// <summary>
    /// Gets the value that represents the Close command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Close
    /// </returns>
    public static RoutedUICommand Close => EnsureCommand(CommandId.Close);

    /// <summary>
    /// Represents a command which is always ignored.
    /// </summary>
    /// <returns>
    /// The command.
    /// </returns>
    public static RoutedUICommand NotACommand => EnsureCommand(CommandId.NotACommand);

    /// <summary>
    /// Gets the value that represents the Undo command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture Ctrl-Z UI Text Undo
    /// </returns>
    public static RoutedUICommand Undo => EnsureCommand(CommandId.Undo);

    private static string GetPropertyName(CommandId commandId)
    {
        return commandId switch
        {
            CommandId.Cut => "Cut",
            CommandId.Copy => "Copy",
            CommandId.Paste => "Paste",
            CommandId.Undo => "Undo",
            CommandId.Redo => "Redo",
            CommandId.Delete => "Delete",
            CommandId.Find => "Find",
            CommandId.Replace => "Replace",
            CommandId.Help => "Help",
            CommandId.New => "New",
            CommandId.Open => "Open",
            CommandId.Save => "Save",
            CommandId.SaveAs => "SaveAs",
            CommandId.Close => "Close",
            CommandId.Print => "Print",
            CommandId.CancelPrint => "CancelPrint",
            CommandId.PrintPreview => "PrintPreview",
            CommandId.Properties => "Properties",
            CommandId.ContextMenu => "ContextMenu",
            CommandId.CorrectionList => "CorrectionList",
            CommandId.SelectAll => "SelectAll",
            CommandId.Stop => "Stop",
            CommandId.NotACommand => "NotACommand",
            _ => string.Empty,
        };
    }

    internal static string GetUIText(byte commandId)
    {
        return (CommandId)commandId switch
        {
            CommandId.Cut => Strings.CutText,
            CommandId.Copy => Strings.CopyText,
            CommandId.Paste => Strings.PasteText,
            CommandId.Undo => Strings.UndoText,
            CommandId.Redo => Strings.RedoText,
            CommandId.Delete => Strings.DeleteText,
            CommandId.Find => Strings.FindText,
            CommandId.Replace => Strings.ReplaceText,
            CommandId.SelectAll => Strings.ReplaceText,
            CommandId.Help => Strings.HelpText,
            CommandId.New => Strings.NewText,
            CommandId.Open => Strings.OpenText,
            CommandId.Save => Strings.SaveText,
            CommandId.SaveAs => Strings.SaveAsText,
            CommandId.Print => Strings.PrintText,
            CommandId.CancelPrint => Strings.CancelPrintText,
            CommandId.PrintPreview => Strings.PrintPreviewText,
            CommandId.Close => Strings.CloseText,
            CommandId.ContextMenu => Strings.ContextMenuText,
            CommandId.CorrectionList => Strings.CorrectionListText,
            CommandId.Properties => Strings.PropertiesText,
            CommandId.Stop => Strings.StopText,
            CommandId.NotACommand => Strings.NotACommandText,
            _ => string.Empty,
        };
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        InputGestureCollection gestures = [];

        //Standard Commands
        switch ((CommandId)commandId)
        {
            case CommandId.Cut:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+X;Shift+Delete",
                    Strings.CutKeyDisplayString,
                    gestures);
                break;
            case CommandId.Copy:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+C;Ctrl+Insert",
                    Strings.CopyKeyDisplayString,
                    gestures);
                break;
            case CommandId.Paste:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+V;Shift+Insert",
                    Strings.PasteKeyDisplayString,
                    gestures);
                break;
            case CommandId.Undo:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+Z",
                    Strings.UndoKeyDisplayString,
                    gestures);
                break;
            case CommandId.Redo:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+Y",
                    Strings.RedoKeyDisplayString,
                    gestures);
                break;
            case CommandId.Delete:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Del",
                    Strings.DeleteKeyDisplayString,
                    gestures);
                break;
            case CommandId.Find:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+F",
                    Strings.FindKeyDisplayString,
                    gestures);
                break;
            case CommandId.Replace:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+H",
                    Strings.ReplaceKeyDisplayString,
                    gestures);
                break;
            case CommandId.SelectAll:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+A",
                    Strings.SelectAllKeyDisplayString,
                    gestures);
                break;
            case CommandId.Help:
                KeyGesture.AddGesturesFromResourceStrings(
                    "F1",
                    Strings.HelpKeyDisplayString,
                    gestures);
                break;
            case CommandId.New:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+N",
                    Strings.NewKeyDisplayString,
                    gestures);
                break;
            case CommandId.Open:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+O",
                    Strings.OpenKeyDisplayString,
                    gestures);
                break;
            case CommandId.Save:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+S",
                    Strings.SaveKeyDisplayString,
                    gestures);
                break;
            case CommandId.SaveAs:
                break; // there are no default bindings for  CommandId.SaveAs
            case CommandId.Print:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+P",
                    Strings.PrintKeyDisplayString,
                    gestures);
                break;
            case CommandId.CancelPrint:
                break; // there are no default bindings for  CommandId.CancelPrint
            case CommandId.PrintPreview:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+F2",
                    Strings.PrintPreviewKeyDisplayString,
                    gestures);
                break;
            case CommandId.Close:
                break; // there are no default bindings for  CommandId.Close
            case CommandId.ContextMenu:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Shift+F10;Apps",
                    Strings.ContextMenuKeyDisplayString,
                    gestures);
                break;
            case CommandId.CorrectionList:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Properties:
                KeyGesture.AddGesturesFromResourceStrings(
                    "F4",
                    Strings.PropertiesKeyDisplayString,
                    gestures);
                break;
            case CommandId.Stop:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Esc",
                    Strings.StopKeyDisplayString,
                    gestures);
                break;
            case CommandId.NotACommand:
                break; // there are no default bindings for  CommandId.NotACommand
        }

        return gestures;
    }

    private static RoutedUICommand EnsureCommand(CommandId idCommand)
    {
        if (idCommand >= 0 && idCommand < CommandId.Last)
        {
            lock (_internalCommands)
            {
                if (_internalCommands[(int)idCommand] is null)
                {
                    RoutedUICommand newCommand = CommandLibraryHelper.CreateUICommand(
                        GetPropertyName(idCommand),
                        typeof(ApplicationCommands),
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
        Cut = 0,
        Copy = 1,
        Paste = 2,
        Undo = 3,
        Redo = 4,
        Delete = 5,
        Find = 6,
        Replace = 7,
        Help = 8,
        SelectAll = 9,
        New = 10,
        Open = 11,
        Save = 12,
        SaveAs = 13,
        Print = 14,
        CancelPrint = 15,
        PrintPreview = 16,
        Close = 17,
        Properties = 18,
        ContextMenu = 19,
        CorrectionList = 20,
        Stop = 21,
        NotACommand = 22,

        // Last
        Last = 23
    }

    private static readonly RoutedUICommand[] _internalCommands = new RoutedUICommand[(int)CommandId.Last];
}
