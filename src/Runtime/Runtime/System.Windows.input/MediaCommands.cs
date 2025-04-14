
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
/// Provides a standard set of media related commands.
/// </summary>
public static class MediaCommands
{
    /// <summary>
    /// Gets the value that represents the Boost Base command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Boost Bass
    /// </returns>
    public static RoutedUICommand BoostBass => EnsureCommand(CommandId.BoostBass);

    /// <summary>
    /// Gets the value that represents the Stop command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Stop
    /// </returns>
    public static RoutedUICommand Stop => EnsureCommand(CommandId.Stop);

    /// <summary>
    /// Gets the value that represents the Select command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Select
    /// </returns>
    public static RoutedUICommand Select => EnsureCommand(CommandId.Select);

    /// <summary>
    /// Gets the value that represents the Rewind command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Rewind
    /// </returns>
    public static RoutedUICommand Rewind => EnsureCommand(CommandId.Rewind);

    /// <summary>
    /// Gets the value that represents the Record command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Record
    /// </returns>
    public static RoutedUICommand Record => EnsureCommand(CommandId.Record);

    /// <summary>
    /// Gets the value that represents the Previous Track command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Previous Track
    /// </returns>
    public static RoutedUICommand PreviousTrack => EnsureCommand(CommandId.PreviousTrack);

    /// <summary>
    /// Gets the value that represents the Play command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Play
    /// </returns>
    public static RoutedUICommand Play => EnsureCommand(CommandId.Play);

    /// <summary>
    /// Gets the value that represents the Pause command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Pause
    /// </returns>
    public static RoutedUICommand Pause => EnsureCommand(CommandId.Pause);

    /// <summary>
    /// Gets the value that represents the Next Track command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Next Track
    /// </returns>
    public static RoutedUICommand NextTrack => EnsureCommand(CommandId.NextTrack);

    /// <summary>
    /// Gets the value that represents the Mute Volume command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Mute Volume
    /// </returns>
    public static RoutedUICommand MuteVolume => EnsureCommand(CommandId.MuteVolume);

    /// <summary>
    /// Gets the value that represents the Mute Microphone Volume command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Mute Microphone Volume
    /// </returns>
    public static RoutedUICommand MuteMicrophoneVolume => EnsureCommand(CommandId.MuteMicrophoneVolume);

    /// <summary>
    /// Gets the value that represents the Increase Volume command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Increase Volume
    /// </returns>
    public static RoutedUICommand IncreaseVolume => EnsureCommand(CommandId.IncreaseVolume);

    /// <summary>
    /// Gets the value that represents the Increase Treble command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Increase Treble
    /// </returns>
    public static RoutedUICommand IncreaseTreble => EnsureCommand(CommandId.IncreaseTreble);

    /// <summary>
    /// Gets the value that represents the Increase Microphone Volume command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Increase Microphone Volume
    /// </returns>
    public static RoutedUICommand IncreaseMicrophoneVolume => EnsureCommand(CommandId.IncreaseMicrophoneVolume);

    /// <summary>
    /// Gets the value that represents the Increase Bass command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Increase Bass
    /// </returns>
    public static RoutedUICommand IncreaseBass => EnsureCommand(CommandId.IncreaseBass);

    /// <summary>
    /// Gets the value that represents the Fast Forward command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Fast Forward
    /// </returns>
    public static RoutedUICommand FastForward => EnsureCommand(CommandId.FastForward);

    /// <summary>
    /// Gets the value that represents the Decrease Volume command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Decrease Volume
    /// </returns>
    public static RoutedUICommand DecreaseVolume => EnsureCommand(CommandId.DecreaseVolume);

    /// <summary>
    /// Gets the value that represents the Decrease Treble command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Decrease Treble
    /// </returns>
    public static RoutedUICommand DecreaseTreble => EnsureCommand(CommandId.ToggleMicrophoneOnOff);

    /// <summary>
    /// Gets the value that represents the Decrease Microphone Volume command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Decrease Microphone Volume
    /// </returns>
    public static RoutedUICommand DecreaseMicrophoneVolume => EnsureCommand(CommandId.DecreaseMicrophoneVolume);

    /// <summary>
    /// Gets the value that represents the Decrease Bass command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Decrease Bass
    /// </returns>
    public static RoutedUICommand DecreaseBass => EnsureCommand(CommandId.DecreaseBass);

    /// <summary>
    /// Gets the value that represents the Channel Up command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Channel Up
    /// </returns>
    public static RoutedUICommand ChannelUp => EnsureCommand(CommandId.ChannelUp);

    /// <summary>
    /// Gets the value that represents the Channel Down command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Channel Down
    /// </returns>
    public static RoutedUICommand ChannelDown => EnsureCommand(CommandId.ChannelDown);

    /// <summary>
    /// Gets the value that represents the Toggle Microphone On Off command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Toggle Microphone OnOff
    /// </returns>
    public static RoutedUICommand ToggleMicrophoneOnOff => EnsureCommand(CommandId.ToggleMicrophoneOnOff);

    /// <summary>
    /// Gets the value that represents the Toggle Play Pause command.
    /// </summary>
    /// <returns>
    /// The command. Default Values Key Gesture No gesture defined. UI Text Toggle Play Pause
    /// </returns>
    public static RoutedUICommand TogglePlayPause => EnsureCommand(CommandId.TogglePlayPause);

    private static string GetPropertyName(CommandId commandId)
    {
        return commandId switch
        {
            CommandId.Play => "Play",
            CommandId.Pause => "Pause",
            CommandId.Stop => "Stop",
            CommandId.Record => "Record",
            CommandId.NextTrack => "NextTrack",
            CommandId.PreviousTrack => "PreviousTrack",
            CommandId.FastForward => "FastForward",
            CommandId.Rewind => "Rewind",
            CommandId.ChannelUp => "ChannelUp",
            CommandId.ChannelDown => "ChannelDown",
            CommandId.TogglePlayPause => "TogglePlayPause",
            CommandId.IncreaseVolume => "IncreaseVolume",
            CommandId.DecreaseVolume => "DecreaseVolume",
            CommandId.MuteVolume => "MuteVolume",
            CommandId.IncreaseTreble => "IncreaseTreble",
            CommandId.DecreaseTreble => "DecreaseTreble",
            CommandId.IncreaseBass => "IncreaseBass",
            CommandId.DecreaseBass => "DecreaseBass",
            CommandId.BoostBass => "BoostBass",
            CommandId.IncreaseMicrophoneVolume => "IncreaseMicrophoneVolume",
            CommandId.DecreaseMicrophoneVolume => "DecreaseMicrophoneVolume",
            CommandId.MuteMicrophoneVolume => "MuteMicrophoneVolume",
            CommandId.ToggleMicrophoneOnOff => "ToggleMicrophoneOnOff",
            CommandId.Select => "Select",
            _ => string.Empty,
        };
    }

    internal static string GetUIText(byte commandId)
    {
        return (CommandId)commandId switch
        {
            CommandId.Play => Strings.MediaPlayText,
            CommandId.Pause => Strings.MediaPauseText,
            CommandId.Stop => Strings.MediaStopText,
            CommandId.Record => Strings.MediaRecordText,
            CommandId.NextTrack => Strings.MediaNextTrackText,
            CommandId.PreviousTrack => Strings.MediaPreviousTrackText,
            CommandId.FastForward => Strings.MediaFastForwardText,
            CommandId.Rewind => Strings.MediaRewindText,
            CommandId.ChannelUp => Strings.MediaChannelUpText,
            CommandId.ChannelDown => Strings.MediaChannelDownText,
            CommandId.TogglePlayPause => Strings.MediaTogglePlayPauseText,
            CommandId.IncreaseVolume => Strings.MediaIncreaseVolumeText,
            CommandId.DecreaseVolume => Strings.MediaDecreaseVolumeText,
            CommandId.MuteVolume => Strings.MediaMuteVolumeText,
            CommandId.IncreaseTreble => Strings.MediaIncreaseTrebleText,
            CommandId.DecreaseTreble => Strings.MediaDecreaseTrebleText,
            CommandId.IncreaseBass => Strings.MediaIncreaseBassText,
            CommandId.DecreaseBass => Strings.MediaDecreaseBassText,
            CommandId.BoostBass => Strings.MediaBoostBassText,
            CommandId.IncreaseMicrophoneVolume => Strings.MediaIncreaseMicrophoneVolumeText,
            CommandId.DecreaseMicrophoneVolume => Strings.MediaDecreaseMicrophoneVolumeText,
            CommandId.MuteMicrophoneVolume => Strings.MediaMuteMicrophoneVolumeText,
            CommandId.ToggleMicrophoneOnOff => Strings.MediaToggleMicrophoneOnOffText,
            CommandId.Select => Strings.MediaSelectText,
            _ => string.Empty,
        };
    }

    internal static InputGestureCollection LoadDefaultGestureFromResource(byte commandId)
    {
        InputGestureCollection gestures = [];

        // Standard Commands
        switch ((CommandId)commandId)
        {
            case CommandId.Play:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Pause:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Stop:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Record:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.NextTrack:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.PreviousTrack:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.FastForward:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Rewind:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.ChannelUp:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.ChannelDown:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.TogglePlayPause:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.IncreaseVolume:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.DecreaseVolume:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.MuteVolume:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.IncreaseTreble:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.DecreaseTreble:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.IncreaseBass:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.DecreaseBass:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.BoostBass:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.IncreaseMicrophoneVolume:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.DecreaseMicrophoneVolume:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.MuteMicrophoneVolume:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.ToggleMicrophoneOnOff:
                KeyGesture.AddGesturesFromResourceStrings(
                    string.Empty,
                    string.Empty,
                    gestures);
                break;
            case CommandId.Select:
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
                        typeof(MediaCommands),
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
        Play = 1,
        Pause = 2,
        Stop = 3,
        Record = 4,
        NextTrack = 5,
        PreviousTrack = 6,
        FastForward = 7,
        Rewind = 8,
        ChannelUp = 9,
        ChannelDown = 10,
        TogglePlayPause = 11,
        IncreaseVolume = 12,
        DecreaseVolume = 13,
        MuteVolume = 14,
        IncreaseTreble = 15,
        DecreaseTreble = 16,
        IncreaseBass = 17,
        DecreaseBass = 18,
        BoostBass = 19,
        IncreaseMicrophoneVolume = 20,
        DecreaseMicrophoneVolume = 21,
        MuteMicrophoneVolume = 22,
        ToggleMicrophoneOnOff = 23,
        Select = 24,

        // Last
        Last = 25
    }

    private static readonly RoutedUICommand[] _internalCommands = new RoutedUICommand[(int)CommandId.Last];
}
