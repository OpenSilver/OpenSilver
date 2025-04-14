
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
/// Provides a standard set of navigation-related commands.
/// </summary>
public static class NavigationCommands
{
    /// <summary>
    /// Gets the value that represents the Browse Back command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture ALT+LEFT UI Text Back
    /// </returns>
    public static RoutedUICommand BrowseBack => EnsureCommand(CommandId.BrowseBack);

    /// <summary>
    /// Gets the value that represents the Browse Forward command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture ALT+RIGHT UI Text Forward
    /// </returns>
    public static RoutedUICommand BrowseForward => EnsureCommand(CommandId.BrowseForward);

    /// <summary>
    /// Gets the value that represents the Browse Home command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture ALT+HOME UI Text Home
    /// </returns>
    public static RoutedUICommand BrowseHome => EnsureCommand(CommandId.BrowseHome);

    /// <summary>
    /// Gets the value that represents the Browse Stop command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture ALT+ESC UI Text Stop
    /// </returns>
    public static RoutedUICommand BrowseStop => EnsureCommand(CommandId.BrowseStop);

    /// <summary>
    /// Gets the value that represents the Decrease Zoom command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Decrease Zoom
    /// </returns>
    public static RoutedUICommand DecreaseZoom => EnsureCommand(CommandId.DecreaseZoom);

    /// <summary>
    /// Gets the value that represents the Favorites command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture CTRL+I UI Text Favorites
    /// </returns>
    public static RoutedUICommand Favorites => EnsureCommand(CommandId.Favorites);

    /// <summary>
    /// Gets the value that represents the First Page command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text First Page
    /// </returns>
    public static RoutedUICommand FirstPage => EnsureCommand(CommandId.FirstPage);

    /// <summary>
    /// Gets the value that represents the Go To Page command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Go To Page
    /// </returns>
    public static RoutedUICommand GoToPage => EnsureCommand(CommandId.GoToPage);

    /// <summary>
    /// Gets the value that represents the Increase Zoom command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Increase Zoom
    /// </returns>
    public static RoutedUICommand IncreaseZoom => EnsureCommand(CommandId.IncreaseZoom);

    /// <summary>
    /// Gets the value that represents the Last Page command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Last Page
    /// </returns>
    public static RoutedUICommand LastPage => EnsureCommand(CommandId.LastPage);

    /// <summary>
    /// Gets the value that represents the Navigate Journal command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Navigation Journal
    /// </returns>
    public static RoutedUICommand NavigateJournal => EnsureCommand(CommandId.NavigateJournal);

    /// <summary>
    /// Gets the value that represents the Next Page command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Next Page
    /// </returns>
    public static RoutedUICommand NextPage => EnsureCommand(CommandId.NextPage);

    /// <summary>
    /// Gets the value that represents the Previous Page command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Previous Page
    /// </returns>
    public static RoutedUICommand PreviousPage => EnsureCommand(CommandId.PreviousPage);

    /// <summary>
    /// Gets the value that represents the Refresh command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture F5 UI Text Refresh
    /// </returns>
    public static RoutedUICommand Refresh => EnsureCommand(CommandId.Refresh);

    /// <summary>
    /// Gets the value that represents the Search command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture F3 UI Text Search
    /// </returns>
    public static RoutedUICommand Search => EnsureCommand(CommandId.Search);

    /// <summary>
    /// Gets the value that represents the Zoom command.
    /// </summary>
    /// <returns>
    /// The routed UI command. Default Values Key Gesture N/A UI Text Zoom
    /// </returns>
    public static RoutedUICommand Zoom => EnsureCommand(CommandId.Zoom);

    private static string GetPropertyName(CommandId commandId)
    {
        return commandId switch
        {
            CommandId.BrowseBack => "BrowseBack",
            CommandId.BrowseForward => "BrowseForward",
            CommandId.BrowseHome => "BrowseHome",
            CommandId.BrowseStop => "BrowseStop",
            CommandId.Refresh => "Refresh",
            CommandId.Favorites => "Favorites",
            CommandId.Search => "Search",
            CommandId.IncreaseZoom => "IncreaseZoom",
            CommandId.DecreaseZoom => "DecreaseZoom",
            CommandId.Zoom => "Zoom",
            CommandId.NextPage => "NextPage",
            CommandId.PreviousPage => "PreviousPage",
            CommandId.FirstPage => "FirstPage",
            CommandId.LastPage => "LastPage",
            CommandId.GoToPage => "GoToPage",
            CommandId.NavigateJournal => "NavigateJournal",
            _ => string.Empty,
        };
    }

    internal static string GetUIText(byte commandId)
    {
        return (CommandId)commandId switch
        {
            CommandId.BrowseBack => Strings.BrowseBackText,
            CommandId.BrowseForward => Strings.BrowseForwardText,
            CommandId.BrowseHome => Strings.BrowseHomeText,
            CommandId.BrowseStop => Strings.BrowseStopText,
            CommandId.Refresh => Strings.RefreshText,
            CommandId.Favorites => Strings.FavoritesText,
            CommandId.Search => Strings.SearchText,
            CommandId.IncreaseZoom => Strings.IncreaseZoomText,
            CommandId.DecreaseZoom => Strings.DecreaseZoomText,
            CommandId.Zoom => Strings.ZoomText,
            CommandId.NextPage => Strings.NextPageText,
            CommandId.PreviousPage => Strings.PreviousPageText,
            CommandId.FirstPage => Strings.FirstPageText,
            CommandId.LastPage => Strings.LastPageText,
            CommandId.GoToPage => Strings.GoToPageText,
            CommandId.NavigateJournal => Strings.NavigateJournalText,
            _ => string.Empty,
        };
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        InputGestureCollection gestures = [];

        // Standard Commands
        switch ((CommandId)commandId)
        {
            case CommandId.BrowseBack:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Alt+Left;Backspace",
                    Strings.BrowseBackKeyDisplayString,
                    gestures);
                break;
            case CommandId.BrowseForward:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Alt+Right;Shift+Backspace",
                    Strings.BrowseForwardKeyDisplayString,
                    gestures);
                break;
            case CommandId.BrowseHome:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Alt+Home", // "Alt+Home;BrowserHome" BrowserHome is not supported 
                    Strings.BrowseHomeKeyDisplayString,
                    gestures);
                break;
            case CommandId.BrowseStop:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Alt+Esc", // "Alt+Esc;BrowserStop" BrowserStop is not supported
                    Strings.BrowseStopKeyDisplayString,
                    gestures);
                break;
            case CommandId.Refresh:
                KeyGesture.AddGesturesFromResourceStrings(
                    "F5",
                    Strings.RefreshKeyDisplayString,
                    gestures);
                break;
            case CommandId.Favorites:
                KeyGesture.AddGesturesFromResourceStrings(
                    "Ctrl+I",
                    Strings.FavoritesKeyDisplayString,
                    gestures);
                break;
            case CommandId.Search:
                KeyGesture.AddGesturesFromResourceStrings(
                    "F3",
                    Strings.SearchKeyDisplayString,
                    gestures);
                break;
            case CommandId.IncreaseZoom:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.DecreaseZoom:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Zoom:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.NextPage:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.PreviousPage:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.FirstPage:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.LastPage:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.GoToPage:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.NavigateJournal:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
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
                        typeof(NavigationCommands),
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
        BrowseBack = 1,
        BrowseForward = 2,
        BrowseHome = 3,
        BrowseStop = 4,
        Refresh = 5,
        Favorites = 6,
        Search = 7,
        IncreaseZoom = 8,
        DecreaseZoom = 9,
        Zoom = 10,
        NextPage = 11,
        PreviousPage = 12,
        FirstPage = 13,
        LastPage = 14,
        GoToPage = 15,
        NavigateJournal = 16,
        // Last
        Last = 17
    }

    private static readonly RoutedUICommand[] _internalCommands = new RoutedUICommand[(int)CommandId.Last];
}
