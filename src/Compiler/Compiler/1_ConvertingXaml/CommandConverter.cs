
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
using System;
using System.Xml.Linq;

namespace OpenSilver.Compiler;

internal sealed class CommandConverter
{
    private readonly AssembliesInspector _inspector;
    private readonly TypeReferenceHelper _helper;

    public CommandConverter(AssembliesInspector inspector, TypeReferenceHelper helper)
    {
        _inspector = inspector;
        _helper = helper;
    }

    public string Convert(XObject context, string source)
    {
        if (source == string.Empty)
        {
            return _helper.Null; // String.Empty <==> null , (for roundtrip cases where Command property values are null)
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

        throw CoreTypesConverter.GetConvertException(source, "System.Windows.Input.ICommand", context);
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
            (PropertyDefinition property, TypeReference declaringType) = _inspector.GetProperty(
                ownerType,
                localName,
                MemberFlags.Public | MemberFlags.Static);

            if (property is not null)
            {
                return $"{_helper.Global}{_helper.ConvertToString(declaringType)}.{property.Name}";
            }

            if (command is null)
            {
                // Get them from Fields (ScrollViewer.PageDownCommand is a static readonly field
                (FieldDefinition field, declaringType) = _inspector.GetField(ownerType, localName, MemberFlags.Public | MemberFlags.Static);
                if (field is not null)
                {
                    return $"{_helper.Global}{_helper.ConvertToString(declaringType)}.{field.Name}";
                }
            }
        }

        return command;
    }

    private static bool IsKnownType(TypeDefinition commandType)
    {
        if (commandType is not null && commandType.GetAssemblyName() == "OpenSilver")
        {
            return commandType.FullName == "System.Windows.Input.ApplicationCommands" ||
                   commandType.FullName == "System.Windows.Input.EditingCommands" ||
                   commandType.FullName == "System.Windows.Input.NavigationCommands" ||
                   commandType.FullName == "System.Windows.Input.ComponentCommands" ||
                   commandType.FullName == "System.Windows.Input.MediaCommands";
        }
        return false;
    }

    private TypeDefinition GetTypeFromContext(XObject context, string typeName)
    {
        XElement nsProvider = CoreTypesConverter.GetClosestXElement(context);

        // Parser Context must exist to get the namespace info from prefix, if not, we assume it is known command.
        if (context is not null && typeName is not null)
        {
            string xmlns;

            int offset = typeName.IndexOf(':');
            if (offset >= 0)
            {
                string prefix = typeName.Substring(0, offset);
                xmlns = nsProvider.GetNamespaceOfPrefix(prefix).NamespaceName;
                typeName = typeName.Substring(offset + 1);
            }
            else
            {
                xmlns = nsProvider.GetDefaultNamespace().NamespaceName;
            }

            return _inspector.GetTypeDefinition(xmlns, typeName, null, context, throwIfNull: false);
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

        if (searchAll || (ownerType.GetAssemblyName() == "OpenSilver" && ownerType.FullName == "System.Windows.Input.NavigationCommands"))
        {
            string knownCommand = localName switch
            {
                "BrowseBack" => $"{_helper.Global}System.Windows.Input.NavigationCommands.BrowseBack",
                "BrowseForward" => $"{_helper.Global}System.Windows.Input.NavigationCommands.BrowseForward",
                "BrowseHome" => $"{_helper.Global}System.Windows.Input.NavigationCommands.BrowseHome",
                "BrowseStop" => $"{_helper.Global}System.Windows.Input.NavigationCommands.BrowseStop",
                "Refresh" => $"{_helper.Global}System.Windows.Input.NavigationCommands.Refresh",
                "Favorites" => $"{_helper.Global}System.Windows.Input.NavigationCommands.Favorites",
                "Search" => $"{_helper.Global}System.Windows.Input.NavigationCommands.Search",
                "IncreaseZoom" => $"{_helper.Global}System.Windows.Input.NavigationCommands.IncreaseZoom",
                "DecreaseZoom" => $"{_helper.Global}System.Windows.Input.NavigationCommands.DecreaseZoom",
                "Zoom" => $"{_helper.Global}System.Windows.Input.NavigationCommands.Zoom",
                "NextPage" => $"{_helper.Global}System.Windows.Input.NavigationCommands.NextPage",
                "PreviousPage" => $"{_helper.Global}System.Windows.Input.NavigationCommands.PreviousPage",
                "FirstPage" => $"{_helper.Global}System.Windows.Input.NavigationCommands.FirstPage",
                "LastPage" => $"{_helper.Global}System.Windows.Input.NavigationCommands.LastPage",
                "GoToPage" => $"{_helper.Global}System.Windows.Input.NavigationCommands.GoToPage",
                "NavigateJournal" => $"{_helper.Global}System.Windows.Input.NavigationCommands.NavigateJournal",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.GetAssemblyName() == "OpenSilver" && ownerType.FullName == "System.Windows.Input.ApplicationCommands"))
        {
            string knownCommand = localName switch
            {
                "Cut" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Cut",
                "Copy" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Copy",
                "Paste" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Paste",
                "Undo" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Undo",
                "Redo" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Redo",
                "Delete" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Delete",
                "Find" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Find",
                "Replace" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Replace",
                "Help" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Help",
                "New" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.New",
                "Open" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Open",
                "Save" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Save",
                "SaveAs" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.SaveAs",
                "Close" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Close",
                "Print" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Print",
                "CancelPrint" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.CancelPrint",
                "PrintPreview" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.PrintPreview",
                "Properties" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Properties",
                "ContextMenu" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.ContextMenu",
                "CorrectionList" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.CorrectionList",
                "SelectAll" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.SelectAll",
                "Stop" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.Stop",
                "NotACommand" => $"{_helper.Global}System.Windows.Input.ApplicationCommands.NotACommand",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.GetAssemblyName() == "OpenSilver" && ownerType.FullName == "System.Windows.Input.ComponentCommands"))
        {
            string knownCommand = localName switch
            {
                "ScrollPageLeft" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ScrollPageLeft",
                "ScrollPageRight" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ScrollPageRight",
                "ScrollPageUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ScrollPageUp",
                "ScrollPageDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ScrollPageDown",
                "ScrollByLine" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ScrollByLine",
                "MoveLeft" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveLeft",
                "MoveRight" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveRight",
                "MoveUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveUp",
                "MoveDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveDown",
                "ExtendSelectionUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ExtendSelectionUp",
                "ExtendSelectionDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ExtendSelectionDown",
                "ExtendSelectionLeft" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ExtendSelectionLeft",
                "ExtendSelectionRight" => $"{_helper.Global}System.Windows.Input.ComponentCommands.ExtendSelectionRight",
                "MoveToHome" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveToHome",
                "MoveToEnd" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveToEnd",
                "MoveToPageUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveToPageUp",
                "MoveToPageDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveToPageDown",
                "SelectToHome" => $"{_helper.Global}System.Windows.Input.ComponentCommands.SelectToHome",
                "SelectToEnd" => $"{_helper.Global}System.Windows.Input.ComponentCommands.SelectToEnd",
                "SelectToPageDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.SelectToPageDown",
                "SelectToPageUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.SelectToPageUp",
                "MoveFocusUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveFocusUp",
                "MoveFocusDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveFocusDown",
                "MoveFocusBack" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveFocusBack",
                "MoveFocusForward" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveFocusForward",
                "MoveFocusPageUp" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveFocusPageUp",
                "MoveFocusPageDown" => $"{_helper.Global}System.Windows.Input.ComponentCommands.MoveFocusPageDown",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.GetAssemblyName() == "OpenSilver" && ownerType.FullName == "System.Windows.Input.EditingCommands"))
        {
            string knownCommand = localName switch
            {
                "ToggleInsert" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleInsert",
                "Delete" => $"{_helper.Global}System.Windows.Input.EditingCommands.Delete",
                "Backspace" => $"{_helper.Global}System.Windows.Input.EditingCommands.Backspace",
                "DeleteNextWord" => $"{_helper.Global}System.Windows.Input.EditingCommands.DeleteNextWord",
                "DeletePreviousWord" => $"{_helper.Global}System.Windows.Input.EditingCommands.DeletePreviousWord",
                "EnterParagraphBreak" => $"{_helper.Global}System.Windows.Input.EditingCommands.EnterParagraphBreak",
                "EnterLineBreak" => $"{_helper.Global}System.Windows.Input.EditingCommands.EnterLineBreak",
                "TabForward" => $"{_helper.Global}System.Windows.Input.EditingCommands.TabForward",
                "TabBackward" => $"{_helper.Global}System.Windows.Input.EditingCommands.TabBackward",
                "MoveRightByCharacter" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveRightByCharacter",
                "MoveLeftByCharacter" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveLeftByCharacter",
                "MoveRightByWord" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveRightByWord",
                "MoveLeftByWord" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveLeftByWord",
                "MoveDownByLine" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveDownByLine",
                "MoveUpByLine" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveUpByLine",
                "MoveDownByParagraph" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveDownByParagraph",
                "MoveUpByParagraph" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveUpByParagraph",
                "MoveDownByPage" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveDownByPage",
                "MoveUpByPage" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveUpByPage",
                "MoveToLineStart" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveToLineStart",
                "MoveToLineEnd" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveToLineEnd",
                "MoveToDocumentStart" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveToDocumentStart",
                "MoveToDocumentEnd" => $"{_helper.Global}System.Windows.Input.EditingCommands.MoveToDocumentEnd",
                "SelectRightByCharacter" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectRightByCharacter",
                "SelectLeftByCharacter" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectLeftByCharacter",
                "SelectRightByWord" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectRightByWord",
                "SelectLeftByWord" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectLeftByWord",
                "SelectDownByLine" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectDownByLine",
                "SelectUpByLine" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectUpByLine",
                "SelectDownByParagraph" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectDownByParagraph",
                "SelectUpByParagraph" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectUpByParagraph",
                "SelectDownByPage" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectDownByPage",
                "SelectUpByPage" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectUpByPage",
                "SelectToLineStart" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectToLineStart",
                "SelectToLineEnd" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectToLineEnd",
                "SelectToDocumentStart" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectToDocumentStart",
                "SelectToDocumentEnd" => $"{_helper.Global}System.Windows.Input.EditingCommands.SelectToDocumentEnd",
                "ToggleBold" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleBold",
                "ToggleItalic" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleItalic",
                "ToggleUnderline" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleUnderline",
                "ToggleSubscript" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleSubscript",
                "ToggleSuperscript" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleSuperscript",
                "IncreaseFontSize" => $"{_helper.Global}System.Windows.Input.EditingCommands.IncreaseFontSize",
                "DecreaseFontSize" => $"{_helper.Global}System.Windows.Input.EditingCommands.DecreaseFontSize",
                // BEGIN Application Compatibility Note
                // The following commands are internal, but they are exposed publicly
                // from our command converter.  We cannot change this behavior
                // because it is well documented.  For example, in the
                // "WPF XAML Vocabulary Specification 2006" found here:
                // http://msdn.microsoft.com/en-us/library/dd361848(PROT.10).aspx
                "ApplyFontSize" => $"{_helper.Global}System.Windows.Input.EditingCommands.ApplyFontSize",
                "ApplyFontFamily" => $"{_helper.Global}System.Windows.Input.EditingCommands.ApplyFontFamily",
                "ApplyForeground" => $"{_helper.Global}System.Windows.Input.EditingCommands.ApplyForeground",
                "ApplyBackground" => $"{_helper.Global}System.Windows.Input.EditingCommands.ApplyBackground",
                // END Application Compatibility Note
                "AlignLeft" => $"{_helper.Global}System.Windows.Input.EditingCommands.AlignLeft",
                "AlignCenter" => $"{_helper.Global}System.Windows.Input.EditingCommands.AlignCenter",
                "AlignRight" => $"{_helper.Global}System.Windows.Input.EditingCommands.AlignRight",
                "AlignJustify" => $"{_helper.Global}System.Windows.Input.EditingCommands.AlignJustify",
                "ToggleBullets" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleBullets",
                "ToggleNumbering" => $"{_helper.Global}System.Windows.Input.EditingCommands.ToggleNumbering",
                "IncreaseIndentation" => $"{_helper.Global}System.Windows.Input.EditingCommands.IncreaseIndentation",
                "DecreaseIndentation" => $"{_helper.Global}System.Windows.Input.EditingCommands.DecreaseIndentation",
                "CorrectSpellingError" => $"{_helper.Global}System.Windows.Input.EditingCommands.CorrectSpellingError",
                "IgnoreSpellingError" => $"{_helper.Global}System.Windows.Input.EditingCommands.IgnoreSpellingError",
                _ => null,
            };

            if (knownCommand is not null)
            {
                return knownCommand;
            }
        }

        if (searchAll || (ownerType.GetAssemblyName() == "OpenSilver" && ownerType.FullName == "System.Windows.Input.MediaCommands"))
        {
            string knownCommand = localName switch
            {
                "Play" => $"{_helper.Global}System.Windows.Input.MediaCommands.Play",
                "Pause" => $"{_helper.Global}System.Windows.Input.MediaCommands.Pause",
                "Stop" => $"{_helper.Global}System.Windows.Input.MediaCommands.Stop",
                "Record" => $"{_helper.Global}System.Windows.Input.MediaCommands.Record",
                "NextTrack" => $"{_helper.Global}System.Windows.Input.MediaCommands.NextTrack",
                "PreviousTrack" => $"{_helper.Global}System.Windows.Input.MediaCommands.PreviousTrack",
                "FastForward" => $"{_helper.Global}System.Windows.Input.MediaCommands.FastForward",
                "Rewind" => $"{_helper.Global}System.Windows.Input.MediaCommands.Rewind",
                "ChannelUp" => $"{_helper.Global}System.Windows.Input.MediaCommands.ChannelUp",
                "ChannelDown" => $"{_helper.Global}System.Windows.Input.MediaCommands.ChannelDown",
                "TogglePlayPause" => $"{_helper.Global}System.Windows.Input.MediaCommands.TogglePlayPause",
                "IncreaseVolume" => $"{_helper.Global}System.Windows.Input.MediaCommands.IncreaseVolume",
                "DecreaseVolume" => $"{_helper.Global}System.Windows.Input.MediaCommands.DecreaseVolume",
                "MuteVolume" => $"{_helper.Global}System.Windows.Input.MediaCommands.MuteVolume",
                "IncreaseTreble" => $"{_helper.Global}System.Windows.Input.MediaCommands.IncreaseTreble",
                "DecreaseTreble" => $"{_helper.Global}System.Windows.Input.MediaCommands.DecreaseTreble",
                "IncreaseBass" => $"{_helper.Global}System.Windows.Input.MediaCommands.IncreaseBass",
                "DecreaseBass" => $"{_helper.Global}System.Windows.Input.MediaCommands.DecreaseBass",
                "BoostBass" => $"{_helper.Global}System.Windows.Input.MediaCommands.BoostBass",
                "IncreaseMicrophoneVolume" => $"{_helper.Global}System.Windows.Input.MediaCommands.IncreaseMicrophoneVolume",
                "DecreaseMicrophoneVolume" => $"{_helper.Global}System.Windows.Input.MediaCommands.DecreaseMicrophoneVolume",
                "MuteMicrophoneVolume" => $"{_helper.Global}System.Windows.Input.MediaCommands.MuteMicrophoneVolume",
                "ToggleMicrophoneOnOff" => $"{_helper.Global}System.Windows.Input.MediaCommands.ToggleMicrophoneOnOff",
                "Select" => $"{_helper.Global}System.Windows.Input.MediaCommands.Select",
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
