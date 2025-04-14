
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
using System.Globalization;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Markup;

namespace System.Windows.Input;

/// <summary>
/// Converts an <see cref="ICommand"/> object to and from other types.
/// </summary>
public sealed class CommandConverter : TypeConverter
{
    /// <summary>
    /// Determines whether an object of the specified type can be converted to an instance of <see cref="ICommand"/>,
    /// using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="sourceType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if sourceType is of type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        // We can only handle string.
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Determines whether an instance of <see cref="ICommand"/> can be converted to the specified type, using the 
    /// specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="destinationType">
    /// The type being evaluated for conversion.
    /// </param>
    /// <returns>
    /// true if destinationType is of type <see cref="string"/>; otherwise, false.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        // We can only convert a "known" command into a string.  This logic
        // is mirrored in ConvertTo.
        //
        // Example: <Button Command="Copy"/>
        if (destinationType == typeof(string))
        {
            RoutedCommand command = context is not null ? context.Instance as RoutedCommand : null;

            if (command is not null && command.OwnerType is not null && IsKnownType(command.OwnerType))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to convert the specified object to an <see cref="ICommand"/>, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="source">
    /// The object to convert.
    /// </param>
    /// <returns>
    /// The converted object, or null if source is an empty string.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// <paramref name="source"/> cannot be converted.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
    {
        if (source is string stringSource)
        {
            if (stringSource == string.Empty)
            {
                return null; // String.Empty <==> null , (for roundtrip cases where Command property values are null)
            }

            // Parse "ns:Class.Command" into "ns:Class", and "Command".
            ParseUri(stringSource, out string typeName, out string localName);

            // Based on the prefix & type name, figure out the owner type.
            Type ownerType = GetTypeFromContext(context, typeName);

            // Find the command (this is shared with CommandValueSerializer).
            if (ConvertFromHelper(ownerType, localName) is ICommand command)
            {
                return command;
            }
        }

        throw GetConvertFromException(source);
    }

    /// <summary>
    /// Attempts to convert an <see cref="ICommand"/> to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">
    /// A format context that provides information about the environment from which this converter is being invoked.
    /// </param>
    /// <param name="culture">
    /// Culture specific information.
    /// </param>
    /// <param name="value">
    /// The object to convert.
    /// </param>
    /// <param name="destinationType">
    /// The type to convert the object to.
    /// </param>
    /// <returns>
    /// The converted object, or an empty string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="destinationType"/> is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// <paramref name="value"/> cannot be converted.
    /// </exception>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        // We can only convert a "known" command into a string.  This logic
        // is mirrored in CanConvertTo.
        //
        // Example: <Button Command="Copy"/>
        if (destinationType == typeof(string))
        {
            if (value is RoutedCommand command && command.OwnerType is not null && IsKnownType(command.OwnerType))
            {
                return command.Name;
            }
            else
            {
                // Should never happen.  Condition checked in CanConvertTo.
                return string.Empty;
            }
        }

        throw GetConvertToException(value, destinationType);
    }

    internal static ICommand ConvertFromHelper(Type ownerType, string localName)
    {
        ICommand command = null;

        // If no namespaceUri or no prefix or no typename, defaulted to Known Commands.
        // there is no typename too, check for default in Known Commands.

        if (IsKnownType(ownerType) || ownerType is null)
        {
            command = GetKnownCommand(localName, ownerType);
        }

        if (command is null && ownerType is not null) // not a known command
        {
            // Get them from Properties
            if (ownerType.GetProperty(localName, BindingFlags.Public | BindingFlags.Static) is PropertyInfo propertyInfo)
            {
                command = propertyInfo.GetValue(null, null) as ICommand;
            }

            if (command is null)
            {
                // Get them from Fields (ScrollViewer.PageDownCommand is a static readonly field
                if (ownerType.GetField(localName, BindingFlags.Static | BindingFlags.Public) is FieldInfo fieldInfo)
                {
                    command = fieldInfo.GetValue(null) as ICommand;
                }
            }
        }

        return command;
    }

    private static bool IsKnownType(Type commandType)
    {
        return commandType == typeof(ApplicationCommands) ||
               commandType == typeof(EditingCommands) ||
               commandType == typeof(NavigationCommands) ||
               commandType == typeof(ComponentCommands) ||
               commandType == typeof(MediaCommands);
    }

    // Utility helper to get the required information from the parserContext
    private Type GetTypeFromContext(ITypeDescriptorContext context, string typeName)
    {
        // We do not pass the context in the compiler, and don't support type resolution for xaml file that have
        // been compiled. As a workaround, we ignore the namespace and look for knowns types (see IsKnownType).
        if (context is null)
        {
            return typeName switch
            {
                nameof(ApplicationCommands) => typeof(ApplicationCommands),
                nameof(EditingCommands) => typeof(EditingCommands),
                nameof(NavigationCommands) => typeof(NavigationCommands),
                nameof(ComponentCommands) => typeof(ComponentCommands),
                nameof(MediaCommands) => typeof(MediaCommands),
                _ => null,
            };
        }

        // Parser Context must exist to get the namespace info from prefix, if not, we assume it is known command.
        if (context is not null && typeName is not null)
        {
            if (context.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver xamlTypeResolver)
            {
                return xamlTypeResolver.Resolve(typeName);
            }
        }

        return null;
    }

    private static void ParseUri(string source, out string typeName, out string localName)
    {
        typeName = null;
        localName = source.Trim();

        // split CommandName from its TypeName (e.g. ScrollViewer.PageDownCommand to Scrollviewerand PageDownCommand)
        int offset = localName.LastIndexOf(".", StringComparison.Ordinal);
        if (offset >= 0)
        {
            typeName = localName.Substring(0, offset);
            localName = localName.Substring(offset + 1);
        }
    }

    private static RoutedUICommand GetKnownCommand(string localName, Type ownerType)
    {
        bool searchAll = ownerType is null;

        if (searchAll || ownerType == typeof(NavigationCommands))
        {
            RoutedUICommand knownCommand = localName switch
            {
                "BrowseBack" => NavigationCommands.BrowseBack,
                "BrowseForward" => NavigationCommands.BrowseForward,
                "BrowseHome" => NavigationCommands.BrowseHome,
                "BrowseStop" => NavigationCommands.BrowseStop,
                "Refresh" => NavigationCommands.Refresh,
                "Favorites" => NavigationCommands.Favorites,
                "Search" => NavigationCommands.Search,
                "IncreaseZoom" => NavigationCommands.IncreaseZoom,
                "DecreaseZoom" => NavigationCommands.DecreaseZoom,
                "Zoom" => NavigationCommands.Zoom,
                "NextPage" => NavigationCommands.NextPage,
                "PreviousPage" => NavigationCommands.PreviousPage,
                "FirstPage" => NavigationCommands.FirstPage,
                "LastPage" => NavigationCommands.LastPage,
                "GoToPage" => NavigationCommands.GoToPage,
                "NavigateJournal" => NavigationCommands.NavigateJournal,
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || ownerType == typeof(ApplicationCommands))
        {
            RoutedUICommand knownCommand = localName switch
            {
                "Cut" => ApplicationCommands.Cut,
                "Copy" => ApplicationCommands.Copy,
                "Paste" => ApplicationCommands.Paste,
                "Undo" => ApplicationCommands.Undo,
                "Redo" => ApplicationCommands.Redo,
                "Delete" => ApplicationCommands.Delete,
                "Find" => ApplicationCommands.Find,
                "Replace" => ApplicationCommands.Replace,
                "Help" => ApplicationCommands.Help,
                "New" => ApplicationCommands.New,
                "Open" => ApplicationCommands.Open,
                "Save" => ApplicationCommands.Save,
                "SaveAs" => ApplicationCommands.SaveAs,
                "Close" => ApplicationCommands.Close,
                "Print" => ApplicationCommands.Print,
                "CancelPrint" => ApplicationCommands.CancelPrint,
                "PrintPreview" => ApplicationCommands.PrintPreview,
                "Properties" => ApplicationCommands.Properties,
                "ContextMenu" => ApplicationCommands.ContextMenu,
                "CorrectionList" => ApplicationCommands.CorrectionList,
                "SelectAll" => ApplicationCommands.SelectAll,
                "Stop" => ApplicationCommands.Stop,
                "NotACommand" => ApplicationCommands.NotACommand,
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || ownerType == typeof(ComponentCommands))
        {
            RoutedUICommand knownCommand = localName switch
            {
                "ScrollPageLeft" => ComponentCommands.ScrollPageLeft,
                "ScrollPageRight" => ComponentCommands.ScrollPageRight,
                "ScrollPageUp" => ComponentCommands.ScrollPageUp,
                "ScrollPageDown" => ComponentCommands.ScrollPageDown,
                "ScrollByLine" => ComponentCommands.ScrollByLine,
                "MoveLeft" => ComponentCommands.MoveLeft,
                "MoveRight" => ComponentCommands.MoveRight,
                "MoveUp" => ComponentCommands.MoveUp,
                "MoveDown" => ComponentCommands.MoveDown,
                "ExtendSelectionUp" => ComponentCommands.ExtendSelectionUp,
                "ExtendSelectionDown" => ComponentCommands.ExtendSelectionDown,
                "ExtendSelectionLeft" => ComponentCommands.ExtendSelectionLeft,
                "ExtendSelectionRight" => ComponentCommands.ExtendSelectionRight,
                "MoveToHome" => ComponentCommands.MoveToHome,
                "MoveToEnd" => ComponentCommands.MoveToEnd,
                "MoveToPageUp" => ComponentCommands.MoveToPageUp,
                "MoveToPageDown" => ComponentCommands.MoveToPageDown,
                "SelectToHome" => ComponentCommands.SelectToHome,
                "SelectToEnd" => ComponentCommands.SelectToEnd,
                "SelectToPageDown" => ComponentCommands.SelectToPageDown,
                "SelectToPageUp" => ComponentCommands.SelectToPageUp,
                "MoveFocusUp" => ComponentCommands.MoveFocusUp,
                "MoveFocusDown" => ComponentCommands.MoveFocusDown,
                "MoveFocusBack" => ComponentCommands.MoveFocusBack,
                "MoveFocusForward" => ComponentCommands.MoveFocusForward,
                "MoveFocusPageUp" => ComponentCommands.MoveFocusPageUp,
                "MoveFocusPageDown" => ComponentCommands.MoveFocusPageDown,
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || ownerType == typeof(EditingCommands))
        {
            RoutedUICommand knownCommand = localName switch
            {
                "ToggleInsert" => EditingCommands.ToggleInsert,
                "Delete" => EditingCommands.Delete,
                "Backspace" => EditingCommands.Backspace,
                "DeleteNextWord" => EditingCommands.DeleteNextWord,
                "DeletePreviousWord" => EditingCommands.DeletePreviousWord,
                "EnterParagraphBreak" => EditingCommands.EnterParagraphBreak,
                "EnterLineBreak" => EditingCommands.EnterLineBreak,
                "TabForward" => EditingCommands.TabForward,
                "TabBackward" => EditingCommands.TabBackward,
                "MoveRightByCharacter" => EditingCommands.MoveRightByCharacter,
                "MoveLeftByCharacter" => EditingCommands.MoveLeftByCharacter,
                "MoveRightByWord" => EditingCommands.MoveRightByWord,
                "MoveLeftByWord" => EditingCommands.MoveLeftByWord,
                "MoveDownByLine" => EditingCommands.MoveDownByLine,
                "MoveUpByLine" => EditingCommands.MoveUpByLine,
                "MoveDownByParagraph" => EditingCommands.MoveDownByParagraph,
                "MoveUpByParagraph" => EditingCommands.MoveUpByParagraph,
                "MoveDownByPage" => EditingCommands.MoveDownByPage,
                "MoveUpByPage" => EditingCommands.MoveUpByPage,
                "MoveToLineStart" => EditingCommands.MoveToLineStart,
                "MoveToLineEnd" => EditingCommands.MoveToLineEnd,
                "MoveToDocumentStart" => EditingCommands.MoveToDocumentStart,
                "MoveToDocumentEnd" => EditingCommands.MoveToDocumentEnd,
                "SelectRightByCharacter" => EditingCommands.SelectRightByCharacter,
                "SelectLeftByCharacter" => EditingCommands.SelectLeftByCharacter,
                "SelectRightByWord" => EditingCommands.SelectRightByWord,
                "SelectLeftByWord" => EditingCommands.SelectLeftByWord,
                "SelectDownByLine" => EditingCommands.SelectDownByLine,
                "SelectUpByLine" => EditingCommands.SelectUpByLine,
                "SelectDownByParagraph" => EditingCommands.SelectDownByParagraph,
                "SelectUpByParagraph" => EditingCommands.SelectUpByParagraph,
                "SelectDownByPage" => EditingCommands.SelectDownByPage,
                "SelectUpByPage" => EditingCommands.SelectUpByPage,
                "SelectToLineStart" => EditingCommands.SelectToLineStart,
                "SelectToLineEnd" => EditingCommands.SelectToLineEnd,
                "SelectToDocumentStart" => EditingCommands.SelectToDocumentStart,
                "SelectToDocumentEnd" => EditingCommands.SelectToDocumentEnd,
                "ToggleBold" => EditingCommands.ToggleBold,
                "ToggleItalic" => EditingCommands.ToggleItalic,
                "ToggleUnderline" => EditingCommands.ToggleUnderline,
                "ToggleSubscript" => EditingCommands.ToggleSubscript,
                "ToggleSuperscript" => EditingCommands.ToggleSuperscript,
                "IncreaseFontSize" => EditingCommands.IncreaseFontSize,
                "DecreaseFontSize" => EditingCommands.DecreaseFontSize,
                // BEGIN Application Compatibility Note
                // The following commands are internal, but they are exposed publicly
                // from our command converter.  We cannot change this behavior
                // because it is well documented.  For example, in the
                // "WPF XAML Vocabulary Specification 2006" found here:
                // http://msdn.microsoft.com/en-us/library/dd361848(PROT.10).aspx
                "ApplyFontSize" => EditingCommands.ApplyFontSize,
                "ApplyFontFamily" => EditingCommands.ApplyFontFamily,
                "ApplyForeground" => EditingCommands.ApplyForeground,
                "ApplyBackground" => EditingCommands.ApplyBackground,
                // END Application Compatibility Note
                "AlignLeft" => EditingCommands.AlignLeft,
                "AlignCenter" => EditingCommands.AlignCenter,
                "AlignRight" => EditingCommands.AlignRight,
                "AlignJustify" => EditingCommands.AlignJustify,
                "ToggleBullets" => EditingCommands.ToggleBullets,
                "ToggleNumbering" => EditingCommands.ToggleNumbering,
                "IncreaseIndentation" => EditingCommands.IncreaseIndentation,
                "DecreaseIndentation" => EditingCommands.DecreaseIndentation,
                "CorrectSpellingError" => EditingCommands.CorrectSpellingError,
                "IgnoreSpellingError" => EditingCommands.IgnoreSpellingError,
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || ownerType == typeof(MediaCommands))
        {
            RoutedUICommand knownCommand = localName switch
            {
                "Play" => MediaCommands.Play,
                "Pause" => MediaCommands.Pause,
                "Stop" => MediaCommands.Stop,
                "Record" => MediaCommands.Record,
                "NextTrack" => MediaCommands.NextTrack,
                "PreviousTrack" => MediaCommands.PreviousTrack,
                "FastForward" => MediaCommands.FastForward,
                "Rewind" => MediaCommands.Rewind,
                "ChannelUp" => MediaCommands.ChannelUp,
                "ChannelDown" => MediaCommands.ChannelDown,
                "TogglePlayPause" => MediaCommands.TogglePlayPause,
                "IncreaseVolume" => MediaCommands.IncreaseVolume,
                "DecreaseVolume" => MediaCommands.DecreaseVolume,
                "MuteVolume" => MediaCommands.MuteVolume,
                "IncreaseTreble" => MediaCommands.IncreaseTreble,
                "DecreaseTreble" => MediaCommands.DecreaseTreble,
                "IncreaseBass" => MediaCommands.IncreaseBass,
                "DecreaseBass" => MediaCommands.DecreaseBass,
                "BoostBass" => MediaCommands.BoostBass,
                "IncreaseMicrophoneVolume" => MediaCommands.IncreaseMicrophoneVolume,
                "DecreaseMicrophoneVolume" => MediaCommands.DecreaseMicrophoneVolume,
                "MuteMicrophoneVolume" => MediaCommands.MuteMicrophoneVolume,
                "ToggleMicrophoneOnOff" => MediaCommands.ToggleMicrophoneOnOff,
                "Select" => MediaCommands.Select,
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

#if DEBUG
        if (ownerType is not null)
        {
            VerifyCommandDoesntExist(ownerType, localName);
        }
        else
        {
            VerifyCommandDoesntExist(typeof(NavigationCommands), localName);
            VerifyCommandDoesntExist(typeof(ApplicationCommands), localName);
            VerifyCommandDoesntExist(typeof(MediaCommands), localName);
            VerifyCommandDoesntExist(typeof(EditingCommands), localName);
            VerifyCommandDoesntExist(typeof(ComponentCommands), localName);
        }
#endif

        return null;
    }

#if DEBUG
    private static void VerifyCommandDoesntExist(Type type, string name)
    {
        PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
        System.Diagnostics.Debug.Assert(propertyInfo is null, "KnownCommand isn't known to CommandConverter.GetKnownCommand");

        FieldInfo fieldInfo = type.GetField(name, BindingFlags.Static | BindingFlags.Public);
        System.Diagnostics.Debug.Assert(fieldInfo is null, "KnownCommand isn't known to CommandConverter.GetKnownCommand");
    }
#endif
}
