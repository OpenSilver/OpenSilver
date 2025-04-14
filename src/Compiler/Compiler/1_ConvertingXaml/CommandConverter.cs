
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using Mono.Cecil;
using OpenSilver.Compiler.OtherHelpersAndHandlers.MonoCecilAssembliesInspector;
using System;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal sealed class CommandConverter
{
    private readonly AssembliesInspector _inspector;
    private readonly SupportedLanguage _language;
    private readonly string _globalKeyword;
    private readonly string _nullKeyword;

    public CommandConverter(AssembliesInspector inspector, SupportedLanguage language, string globalKeyword, string nullKeyword)
    {
        _inspector = inspector;
        _language = language;
        _globalKeyword = globalKeyword;
        _nullKeyword = nullKeyword;
    }

    public string Convert(XElement context, string source)
    {
        if (source == string.Empty)
        {
            return _nullKeyword; // String.Empty <==> null , (for roundtrip cases where Command property values are null)
        }

        // Parse "ns:Class.Command" into "ns:Class", and "Command".
        ParseUri(source, out string typeName, out string localName);

        // Based on the prefix & type name, figure out the owner type.
        TypeDefinition ownerType = GetTypeFromContext(context, typeName);

        // Find the command (this is shared with CommandValueSerializer).
        if (ConvertFromHelper(ownerType, localName) is string command)
        {
            return command;
        }

        throw CoreTypesConverter.GetConvertException(source, "System.Windows.Input.ICommand");
    }

    private string ConvertFromHelper(TypeDefinition ownerType, string localName)
    {
        string command = null;

        // If no namespaceUri or no prefix or no typename, defaulted to Known Commands.
        // there is no typename too, check for default in Known Commands.

        if (IsKnownType(ownerType) || ownerType is null)
        {
            command = GetKnownCommand(localName, ownerType);
        }

        if (command is null && ownerType is not null) // not a known command
        {
            // Get them from Properties
            (PropertyDefinition property, TypeReference declaringType) = _inspector.GetProperty(ownerType, localName, true, true);
            if (property is not null)
            {
                return $"{_globalKeyword}{declaringType.ConvertToString(_language)}.{property.Name}";
            }

            if (command is null)
            {
                // Get them from Fields (ScrollViewer.PageDownCommand is a static readonly field
                (FieldDefinition field, declaringType) = _inspector.GetField(ownerType, localName, true, true);
                if (field is not null)
                {
                    return $"{_globalKeyword}{declaringType.ConvertToString(_language)}.{field.Name}";
                }
            }
        }

        return command;
    }

    private static bool IsKnownType(TypeDefinition commandType)
    {
        if (commandType.Scope.Name == "OpenSilver")
        {
            return commandType.FullName == "System.Windows.Input.ApplicationCommands" ||
                   commandType.FullName == "System.Windows.Input.EditingCommands" ||
                   commandType.FullName == "System.Windows.Input.NavigationCommands" ||
                   commandType.FullName == "System.Windows.Input.ComponentCommands" ||
                   commandType.FullName == "System.Windows.Input.MediaCommands";
        }
        return false;
    }

    private TypeDefinition GetTypeFromContext(XElement context, string typeName)
    {
        // Parser Context must exist to get the namespace info from prefix, if not, we assume it is known command.
        if (context is not null && typeName is not null)
        {
            string xmlns;

            int offset = typeName.IndexOf(':');
            if (offset >= 0)
            {
                string prefix = typeName.Substring(0, offset);
                xmlns = context.GetNamespaceOfPrefix(prefix).NamespaceName;
                typeName = typeName.Substring(offset + 1);
            }
            else
            {
                xmlns = context.GetDefaultNamespace().NamespaceName;
            }

            return _inspector.GetTypeDefinition(xmlns, typeName, null, throwIfNull: false);
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

    private string GetKnownCommand(string localName, TypeDefinition ownerType)
    {
        bool searchAll = ownerType is null;

        if (searchAll || (ownerType.Scope.Name == "OpenSilver" && ownerType.FullName == "System.Windows.Input.NavigationCommands"))
        {
            string knownCommand = localName switch
            {
                "BrowseBack" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.BrowseBack",
                "BrowseForward" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.BrowseForward",
                "BrowseHome" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.BrowseHome",
                "BrowseStop" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.BrowseStop",
                "Refresh" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.Refresh",
                "Favorites" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.Favorites",
                "Search" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.Search",
                "IncreaseZoom" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.IncreaseZoom",
                "DecreaseZoom" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.DecreaseZoom",
                "Zoom" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.Zoom",
                "NextPage" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.NextPage",
                "PreviousPage" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.PreviousPage",
                "FirstPage" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.FirstPage",
                "LastPage" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.LastPage",
                "GoToPage" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.GoToPage",
                "NavigateJournal" => $"{_globalKeyword}System.Windows.Input.NavigationCommands.NavigateJournal",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.Scope.Name == "OpenSilver" && ownerType.FullName == "System.Windows.Input.ApplicationCommands"))
        {
            string knownCommand = localName switch
            {
                "Cut" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Cut",
                "Copy" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Copy",
                "Paste" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Paste",
                "Undo" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Undo",
                "Redo" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Redo",
                "Delete" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Delete",
                "Find" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Find",
                "Replace" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Replace",
                "Help" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Help",
                "New" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.New",
                "Open" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Open",
                "Save" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Save",
                "SaveAs" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.SaveAs",
                "Close" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Close",
                "Print" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Print",
                "CancelPrint" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.CancelPrint",
                "PrintPreview" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.PrintPreview",
                "Properties" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Properties",
                "ContextMenu" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.ContextMenu",
                "CorrectionList" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.CorrectionList",
                "SelectAll" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.SelectAll",
                "Stop" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.Stop",
                "NotACommand" => $"{_globalKeyword}System.Windows.Input.ApplicationCommands.NotACommand",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.Scope.Name == "OpenSilver" && ownerType.FullName == "System.Windows.Input.ComponentCommands"))
        {
            string knownCommand = localName switch
            {
                "ScrollPageLeft" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ScrollPageLeft",
                "ScrollPageRight" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ScrollPageRight",
                "ScrollPageUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ScrollPageUp",
                "ScrollPageDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ScrollPageDown",
                "ScrollByLine" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ScrollByLine",
                "MoveLeft" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveLeft",
                "MoveRight" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveRight",
                "MoveUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveUp",
                "MoveDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveDown",
                "ExtendSelectionUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ExtendSelectionUp",
                "ExtendSelectionDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ExtendSelectionDown",
                "ExtendSelectionLeft" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ExtendSelectionLeft",
                "ExtendSelectionRight" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.ExtendSelectionRight",
                "MoveToHome" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveToHome",
                "MoveToEnd" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveToEnd",
                "MoveToPageUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveToPageUp",
                "MoveToPageDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveToPageDown",
                "SelectToHome" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.SelectToHome",
                "SelectToEnd" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.SelectToEnd",
                "SelectToPageDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.SelectToPageDown",
                "SelectToPageUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.SelectToPageUp",
                "MoveFocusUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveFocusUp",
                "MoveFocusDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveFocusDown",
                "MoveFocusBack" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveFocusBack",
                "MoveFocusForward" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveFocusForward",
                "MoveFocusPageUp" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveFocusPageUp",
                "MoveFocusPageDown" => $"{_globalKeyword}System.Windows.Input.ComponentCommands.MoveFocusPageDown",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.Scope.Name == "OpenSilver" && ownerType.FullName == "System.Windows.Input.EditingCommands"))
        {
            string knownCommand = localName switch
            {
                "ToggleInsert" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleInsert",
                "Delete" => $"{_globalKeyword}System.Windows.Input.EditingCommands.Delete",
                "Backspace" => $"{_globalKeyword}System.Windows.Input.EditingCommands.Backspace",
                "DeleteNextWord" => $"{_globalKeyword}System.Windows.Input.EditingCommands.DeleteNextWord",
                "DeletePreviousWord" => $"{_globalKeyword}System.Windows.Input.EditingCommands.DeletePreviousWord",
                "EnterParagraphBreak" => $"{_globalKeyword}System.Windows.Input.EditingCommands.EnterParagraphBreak",
                "EnterLineBreak" => $"{_globalKeyword}System.Windows.Input.EditingCommands.EnterLineBreak",
                "TabForward" => $"{_globalKeyword}System.Windows.Input.EditingCommands.TabForward",
                "TabBackward" => $"{_globalKeyword}System.Windows.Input.EditingCommands.TabBackward",
                "MoveRightByCharacter" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveRightByCharacter",
                "MoveLeftByCharacter" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveLeftByCharacter",
                "MoveRightByWord" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveRightByWord",
                "MoveLeftByWord" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveLeftByWord",
                "MoveDownByLine" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveDownByLine",
                "MoveUpByLine" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveUpByLine",
                "MoveDownByParagraph" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveDownByParagraph",
                "MoveUpByParagraph" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveUpByParagraph",
                "MoveDownByPage" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveDownByPage",
                "MoveUpByPage" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveUpByPage",
                "MoveToLineStart" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveToLineStart",
                "MoveToLineEnd" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveToLineEnd",
                "MoveToDocumentStart" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveToDocumentStart",
                "MoveToDocumentEnd" => $"{_globalKeyword}System.Windows.Input.EditingCommands.MoveToDocumentEnd",
                "SelectRightByCharacter" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectRightByCharacter",
                "SelectLeftByCharacter" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectLeftByCharacter",
                "SelectRightByWord" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectRightByWord",
                "SelectLeftByWord" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectLeftByWord",
                "SelectDownByLine" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectDownByLine",
                "SelectUpByLine" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectUpByLine",
                "SelectDownByParagraph" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectDownByParagraph",
                "SelectUpByParagraph" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectUpByParagraph",
                "SelectDownByPage" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectDownByPage",
                "SelectUpByPage" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectUpByPage",
                "SelectToLineStart" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectToLineStart",
                "SelectToLineEnd" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectToLineEnd",
                "SelectToDocumentStart" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectToDocumentStart",
                "SelectToDocumentEnd" => $"{_globalKeyword}System.Windows.Input.EditingCommands.SelectToDocumentEnd",
                "ToggleBold" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleBold",
                "ToggleItalic" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleItalic",
                "ToggleUnderline" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleUnderline",
                "ToggleSubscript" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleSubscript",
                "ToggleSuperscript" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleSuperscript",
                "IncreaseFontSize" => $"{_globalKeyword}System.Windows.Input.EditingCommands.IncreaseFontSize",
                "DecreaseFontSize" => $"{_globalKeyword}System.Windows.Input.EditingCommands.DecreaseFontSize",
                // BEGIN Application Compatibility Note
                // The following commands are internal, but they are exposed publicly
                // from our command converter.  We cannot change this behavior
                // because it is well documented.  For example, in the
                // "WPF XAML Vocabulary Specification 2006" found here:
                // http://msdn.microsoft.com/en-us/library/dd361848(PROT.10).aspx
                "ApplyFontSize" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ApplyFontSize",
                "ApplyFontFamily" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ApplyFontFamily",
                "ApplyForeground" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ApplyForeground",
                "ApplyBackground" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ApplyBackground",
                // END Application Compatibility Note
                "AlignLeft" => $"{_globalKeyword}System.Windows.Input.EditingCommands.AlignLeft",
                "AlignCenter" => $"{_globalKeyword}System.Windows.Input.EditingCommands.AlignCenter",
                "AlignRight" => $"{_globalKeyword}System.Windows.Input.EditingCommands.AlignRight",
                "AlignJustify" => $"{_globalKeyword}System.Windows.Input.EditingCommands.AlignJustify",
                "ToggleBullets" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleBullets",
                "ToggleNumbering" => $"{_globalKeyword}System.Windows.Input.EditingCommands.ToggleNumbering",
                "IncreaseIndentation" => $"{_globalKeyword}System.Windows.Input.EditingCommands.IncreaseIndentation",
                "DecreaseIndentation" => $"{_globalKeyword}System.Windows.Input.EditingCommands.DecreaseIndentation",
                "CorrectSpellingError" => $"{_globalKeyword}System.Windows.Input.EditingCommands.CorrectSpellingError",
                "IgnoreSpellingError" => $"{_globalKeyword}System.Windows.Input.EditingCommands.IgnoreSpellingError",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.Scope.Name == "OpenSilver" && ownerType.FullName == "System.Windows.Input.MediaCommands"))
        {
            string knownCommand = localName switch
            {
                "Play" => $"{_globalKeyword}System.Windows.Input.MediaCommands.Play",
                "Pause" => $"{_globalKeyword}System.Windows.Input.MediaCommands.Pause",
                "Stop" => $"{_globalKeyword}System.Windows.Input.MediaCommands.Stop",
                "Record" => $"{_globalKeyword}System.Windows.Input.MediaCommands.Record",
                "NextTrack" => $"{_globalKeyword}System.Windows.Input.MediaCommands.NextTrack",
                "PreviousTrack" => $"{_globalKeyword}System.Windows.Input.MediaCommands.PreviousTrack",
                "FastForward" => $"{_globalKeyword}System.Windows.Input.MediaCommands.FastForward",
                "Rewind" => $"{_globalKeyword}System.Windows.Input.MediaCommands.Rewind",
                "ChannelUp" => $"{_globalKeyword}System.Windows.Input.MediaCommands.ChannelUp",
                "ChannelDown" => $"{_globalKeyword}System.Windows.Input.MediaCommands.ChannelDown",
                "TogglePlayPause" => $"{_globalKeyword}System.Windows.Input.MediaCommands.TogglePlayPause",
                "IncreaseVolume" => $"{_globalKeyword}System.Windows.Input.MediaCommands.IncreaseVolume",
                "DecreaseVolume" => $"{_globalKeyword}System.Windows.Input.MediaCommands.DecreaseVolume",
                "MuteVolume" => $"{_globalKeyword}System.Windows.Input.MediaCommands.MuteVolume",
                "IncreaseTreble" => $"{_globalKeyword}System.Windows.Input.MediaCommands.IncreaseTreble",
                "DecreaseTreble" => $"{_globalKeyword}System.Windows.Input.MediaCommands.DecreaseTreble",
                "IncreaseBass" => $"{_globalKeyword}System.Windows.Input.MediaCommands.IncreaseBass",
                "DecreaseBass" => $"{_globalKeyword}System.Windows.Input.MediaCommands.DecreaseBass",
                "BoostBass" => $"{_globalKeyword}System.Windows.Input.MediaCommands.BoostBass",
                "IncreaseMicrophoneVolume" => $"{_globalKeyword}System.Windows.Input.MediaCommands.IncreaseMicrophoneVolume",
                "DecreaseMicrophoneVolume" => $"{_globalKeyword}System.Windows.Input.MediaCommands.DecreaseMicrophoneVolume",
                "MuteMicrophoneVolume" => $"{_globalKeyword}System.Windows.Input.MediaCommands.MuteMicrophoneVolume",
                "ToggleMicrophoneOnOff" => $"{_globalKeyword}System.Windows.Input.MediaCommands.ToggleMicrophoneOnOff",
                "Select" => $"{_globalKeyword}System.Windows.Input.MediaCommands.Select",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        return null;
    }
}
