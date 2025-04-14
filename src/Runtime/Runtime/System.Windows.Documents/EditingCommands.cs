
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

using System.Windows.Input;

namespace System.Windows.Documents;

/// <summary>
/// Provides a standard set of editing related commands.
/// </summary>
public static class EditingCommands
{
    // Typing Commands
    // ---------------
    /// <summary>
    /// Represents the <see cref="ToggleInsert"/> command, which toggles the typing mode between Insert and Overtype.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Insert.
    /// </returns>
    public static RoutedUICommand ToggleInsert => EnsureCommand(ref _ToggleInsert, nameof(ToggleInsert));

    /// <summary>
    /// Represents the <see cref="Delete"/> command, which requests that the current selection be deleted.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Delete.
    /// </returns>
    public static RoutedUICommand Delete => EnsureCommand(ref _Delete, nameof(Delete));

    internal static RoutedUICommand Clear => EnsureCommand(ref _Clear, nameof(Clear));

    /// <summary>
    /// Represents the <see cref="Backspace"/> command, which requests that a backspace be entered at the current 
    /// position or over the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Backspace.
    /// </returns>
    public static RoutedUICommand Backspace => EnsureCommand(ref _Backspace, nameof(Backspace));

    /// <summary>
    /// Represents the <see cref="DeleteNextWord"/> command, which requests that the next word (relative to a current 
    /// position) be deleted.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Delete.
    /// </returns>
    public static RoutedUICommand DeleteNextWord => EnsureCommand(ref _DeleteNextWord, nameof(DeleteNextWord));

    /// <summary>
    /// Represents the <see cref="DeletePreviousWord"/> command, which requests that the previous word (relative to a 
    /// current position) be deleted.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Backspace.
    /// </returns>
    public static RoutedUICommand DeletePreviousWord => EnsureCommand(ref _DeletePreviousWord, nameof(DeletePreviousWord));

    /// <summary>
    /// Represents the <see cref="EnterParagraphBreak"/> command, which requests that a paragraph break be inserted at 
    /// the current position or over the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Enter.
    /// </returns>
    public static RoutedUICommand EnterParagraphBreak => EnsureCommand(ref _EnterParagraphBreak, nameof(EnterParagraphBreak));

    /// <summary>
    /// Represents the <see cref="EnterLineBreak"/> command, which requests that a line break be inserted at the current 
    /// position or over the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Enter.
    /// </returns>
    public static RoutedUICommand EnterLineBreak => EnsureCommand(ref _EnterLineBreak, nameof(EnterLineBreak));

    /// <summary>
    /// Represents the <see cref="TabForward"/> command.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Tab.
    /// </returns>
    public static RoutedUICommand TabForward => EnsureCommand(ref _TabForward, nameof(TabForward));

    /// <summary>
    /// Represents the <see cref="TabBackward"/> command.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Tab.
    /// </returns>
    public static RoutedUICommand TabBackward => EnsureCommand(ref _TabBackward, nameof(TabBackward));

    // Caret navigation commands
    // -------------------------
    /// <summary>
    /// Represents the <see cref="MoveRightByCharacter"/> command, which requests that the caret move one character right.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Right.
    /// </returns>
    public static RoutedUICommand MoveRightByCharacter => EnsureCommand(ref _MoveRightByCharacter, nameof(MoveRightByCharacter));

    /// <summary>
    /// Represents the <see cref="MoveLeftByCharacter"/> command, which requests that the caret move one character left.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Left.
    /// </returns>
    public static RoutedUICommand MoveLeftByCharacter => EnsureCommand(ref _MoveLeftByCharacter, nameof(MoveLeftByCharacter));

    /// <summary>
    /// Represents the <see cref="MoveRightByWord"/> command, which requests that the caret move right by one word.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Right.
    /// </returns>
    public static RoutedUICommand MoveRightByWord => EnsureCommand(ref _MoveRightByWord, nameof(MoveRightByWord));

    /// <summary>
    /// Represents the <see cref="MoveLeftByWord"/> command, which requests that the caret move one word left.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Left.
    /// </returns>
    public static RoutedUICommand MoveLeftByWord => EnsureCommand(ref _MoveLeftByWord, nameof(MoveLeftByWord));

    /// <summary>
    /// Represents the <see cref="MoveDownByLine"/> command, which requests that the caret move down by one line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Down.
    /// </returns>
    public static RoutedUICommand MoveDownByLine => EnsureCommand(ref _MoveDownByLine, nameof(MoveDownByLine));

    /// <summary>
    /// Represents the <see cref="MoveUpByLine"/> command, which requests that the caret move up by one line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Up.
    /// </returns>
    public static RoutedUICommand MoveUpByLine => EnsureCommand(ref _MoveUpByLine, nameof(MoveUpByLine));

    /// <summary>
    /// Represents the <see cref="MoveDownByParagraph"/> command, which requests that the caret move down by one paragraph.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Down.
    /// </returns>
    public static RoutedUICommand MoveDownByParagraph => EnsureCommand(ref _MoveDownByParagraph, nameof(MoveDownByParagraph));

    /// <summary>
    /// Represents the <see cref="MoveUpByParagraph"/> command, which requests that the caret move up by one paragraph.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Up.
    /// </returns>
    public static RoutedUICommand MoveUpByParagraph => EnsureCommand(ref _MoveUpByParagraph, nameof(MoveUpByParagraph));

    /// <summary>
    /// Represents the <see cref="MoveDownByPage"/> command, which requests that the caret move down by one page.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is PageDown.
    /// </returns>
    public static RoutedUICommand MoveDownByPage => EnsureCommand(ref _MoveDownByPage, nameof(MoveDownByPage));

    /// <summary>
    /// Represents the <see cref="MoveUpByPage"/> command, which requests that the caret move up by one page.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is PageUp.
    /// </returns>
    public static RoutedUICommand MoveUpByPage => EnsureCommand(ref _MoveUpByPage, nameof(MoveUpByPage));

    /// <summary>
    /// Represents the <see cref="MoveToLineStart"/> command, which requests that the caret move to the beginning of the 
    /// current line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Home.
    /// </returns>
    public static RoutedUICommand MoveToLineStart => EnsureCommand(ref _MoveToLineStart, nameof(MoveToLineStart));

    /// <summary>
    /// Represents the <see cref="MoveToLineEnd"/> command, which requests that the caret move to the end of the current line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is End.
    /// </returns>
    public static RoutedUICommand MoveToLineEnd => EnsureCommand(ref _MoveToLineEnd, nameof(MoveToLineEnd));

    /// <summary>
    /// Represents the <see cref="MoveToDocumentStart"/> command, which requests that the caret move to the very beginning 
    /// of content.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Home.
    /// </returns>
    public static RoutedUICommand MoveToDocumentStart => EnsureCommand(ref _MoveToDocumentStart, nameof(MoveToDocumentStart));

    /// <summary>
    /// Represents the <see cref="MoveToDocumentEnd"/> command, which requests that the caret move to the very end of content.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+End.
    /// </returns>
    public static RoutedUICommand MoveToDocumentEnd => EnsureCommand(ref _MoveToDocumentEnd, nameof(MoveToDocumentEnd));

    // Selection extension commands
    // ----------------------------

    /// <summary>
    /// Represents the <see cref="SelectRightByCharacter"/> command, which requests that the current selection be expanded 
    /// right by one character.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Right.
    /// </returns>
    public static RoutedUICommand SelectRightByCharacter => EnsureCommand(ref _SelectRightByCharacter, nameof(SelectRightByCharacter));

    /// <summary>
    /// Represents the <see cref="SelectLeftByCharacter"/> command, which requests that the current selection be expanded left 
    /// by one character.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Left.
    /// </returns>
    public static RoutedUICommand SelectLeftByCharacter => EnsureCommand(ref _SelectLeftByCharacter, nameof(SelectLeftByCharacter));

    /// <summary>
    /// Represents the <see cref="SelectRightByWord"/> command, which requests that the current selection be expanded right by one word.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+Right.
    /// </returns>
    public static RoutedUICommand SelectRightByWord => EnsureCommand(ref _SelectRightByWord, nameof(SelectRightByWord));

    /// <summary>
    /// Represents the <see cref="SelectLeftByWord"/> command, which requests that the current selection be expanded left by one word.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+Left.
    /// </returns>
    public static RoutedUICommand SelectLeftByWord => EnsureCommand(ref _SelectLeftByWord, nameof(SelectLeftByWord));

    /// <summary>
    /// Represents the <see cref="SelectDownByLine"/> command, which requests that the current selection be expanded down by one line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Down.
    /// </returns>
    public static RoutedUICommand SelectDownByLine => EnsureCommand(ref _SelectDownByLine, nameof(SelectDownByLine));

    /// <summary>
    /// Represents the <see cref="SelectUpByLine"/> command, which requests that the current selection be expanded up by one line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Up.
    /// </returns>
    public static RoutedUICommand SelectUpByLine => EnsureCommand(ref _SelectUpByLine, nameof(SelectUpByLine));

    /// <summary>
    /// Represents the <see cref="SelectDownByParagraph"/> command, which requests that the current selection be expanded down by one
    /// paragraph.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+Down.
    /// </returns>
    public static RoutedUICommand SelectDownByParagraph => EnsureCommand(ref _SelectDownByParagraph, nameof(SelectDownByParagraph));

    /// <summary>
    /// Represents the <see cref="SelectUpByParagraph"/> command, which requests that the current selection be expanded up by one paragraph.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+Up.
    /// </returns>
    public static RoutedUICommand SelectUpByParagraph => EnsureCommand(ref _SelectUpByParagraph, nameof(SelectUpByParagraph));

    /// <summary>
    /// Represents the <see cref="SelectDownByPage"/> command, which requests that the current selection be expanded down by one page.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+PageDown.
    /// </returns>
    public static RoutedUICommand SelectDownByPage => EnsureCommand(ref _SelectDownByPage, nameof(SelectDownByPage));

    /// <summary>
    /// Represents the <see cref="SelectUpByPage"/> command, which requests that the current selection be expanded up by one page.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+PageUp.
    /// </returns>
    public static RoutedUICommand SelectUpByPage => EnsureCommand(ref _SelectUpByPage, nameof(SelectUpByPage));

    /// <summary>
    /// Represents the <see cref="SelectToLineStart"/> command, which requests that the current selection be expanded to the beginning 
    /// of the current line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+Home.
    /// </returns>
    public static RoutedUICommand SelectToLineStart => EnsureCommand(ref _SelectToLineStart, nameof(SelectToLineStart));

    /// <summary>
    /// Represents the <see cref="SelectToLineEnd"/> command, which requests that the current selection be expanded to the end of the 
    /// current line.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Shift+End.
    /// </returns>
    public static RoutedUICommand SelectToLineEnd => EnsureCommand(ref _SelectToLineEnd, nameof(SelectToLineEnd));

    /// <summary>
    /// Represents the <see cref="SelectToDocumentStart"/> command, which requests that the current selection be expanded to the very 
    /// beginning of content.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+Home.
    /// </returns>
    public static RoutedUICommand SelectToDocumentStart => EnsureCommand(ref _SelectToDocumentStart, nameof(SelectToDocumentStart));

    /// <summary>
    /// Represents the <see cref="SelectToDocumentEnd"/> command, which requests that the current selection be expanded to the very end 
    /// of content.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+End.
    /// </returns>
    public static RoutedUICommand SelectToDocumentEnd => EnsureCommand(ref _SelectToDocumentEnd, nameof(SelectToDocumentEnd));

    // Character editing commands
    // --------------------------

    /// <summary>
    /// Represents the <see cref="ToggleBold"/> command, which requests that <see cref="Bold"/> formatting be toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+B.
    /// </returns>
    public static RoutedUICommand ToggleBold => EnsureCommand(ref _ToggleBold, nameof(ToggleBold));

    /// <summary>
    /// Represents the <see cref="ToggleItalic"/> command, which requests that <see cref="Italic"/> formatting be toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+I.
    /// </returns>
    public static RoutedUICommand ToggleItalic => EnsureCommand(ref _ToggleItalic, nameof(ToggleItalic));

    /// <summary>
    /// Represents the <see cref="ToggleUnderline"/> command, which requests that <see cref="Underline"/> formatting be toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+U.
    /// </returns>
    public static RoutedUICommand ToggleUnderline => EnsureCommand(ref _ToggleUnderline, nameof(ToggleUnderline));

    /// <summary>
    /// Represents the <see cref="ToggleSubscript"/> command, which requests that subscript formatting be toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+OemPlus.
    /// </returns>
    public static RoutedUICommand ToggleSubscript => EnsureCommand(ref _ToggleSubscript, nameof(ToggleSubscript));

    /// <summary>
    /// Represents the <see cref="ToggleSuperscript"/> command, which requests that superscript formatting be toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+OemPlus.
    /// </returns>
    public static RoutedUICommand ToggleSuperscript => EnsureCommand(ref _ToggleSuperscript, nameof(ToggleSuperscript));

    /// <summary>
    /// Represents the <see cref="IncreaseFontSize"/> command, which requests that the font size for the current selection be increased by 1 point.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+OemCloseBrackets.
    /// </returns>
    public static RoutedUICommand IncreaseFontSize => EnsureCommand(ref _IncreaseFontSize, nameof(IncreaseFontSize));

    /// <summary>
    /// Represents the <see cref="DecreaseFontSize"/> command, which requests that the font size for the current selection be decreased by 1 point.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+OemOpenBrackets.
    /// </returns>
    public static RoutedUICommand DecreaseFontSize => EnsureCommand(ref _DecreaseFontSize, nameof(DecreaseFontSize));

    // Paragraph editing commands
    // --------------------------

    /// <summary>
    /// Represents the <see cref="AlignLeft"/> command, which requests that a selection of content be aligned left.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+L.
    /// </returns>
    public static RoutedUICommand AlignLeft => EnsureCommand(ref _AlignLeft, nameof(AlignLeft));

    /// <summary>
    /// Represents the <see cref="AlignCenter"/> command, which requests that the current paragraph or a selection of paragraphs be centered.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+E.
    /// </returns>
    public static RoutedUICommand AlignCenter => EnsureCommand(ref _AlignCenter, nameof(AlignCenter));

    /// <summary>
    /// Represents the <see cref="AlignRight"/> command, which requests that a selection of content be aligned right.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+R.
    /// </returns>
    public static RoutedUICommand AlignRight => EnsureCommand(ref _AlignRight, nameof(AlignRight));

    /// <summary>
    /// Represents the <see cref="AlignJustify"/> command, which requests that the current paragraph or a selection of paragraphs be justified.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+J.
    /// </returns>
    public static RoutedUICommand AlignJustify => EnsureCommand(ref _AlignJustify, nameof(AlignJustify));

    // List editing commands
    // ---------------------

    /// <summary>
    /// Represents the <see cref="ToggleBullets"/> command, which requests that unordered list (also referred to as bulleted list) formatting be 
    /// toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+L.
    /// </returns>
    public static RoutedUICommand ToggleBullets => EnsureCommand(ref _ToggleBullets, nameof(ToggleBullets));

    /// <summary>
    /// Represents the <see cref="ToggleNumbering"/> command, which requests that ordered list (also referred to as numbered list) formatting be 
    /// toggled on the current selection.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+N.
    /// </returns>
    public static RoutedUICommand ToggleNumbering => EnsureCommand(ref _ToggleNumbering, nameof(ToggleNumbering));

    /// <summary>
    /// Represents the <see cref="IncreaseIndentation"/> command, which requests that indentation for the current paragraph be increased by one tab stop.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+T.
    /// </returns>
    public static RoutedUICommand IncreaseIndentation => EnsureCommand(ref _IncreaseIndentation, nameof(IncreaseIndentation));

    /// <summary>
    /// Represents the <see cref="DecreaseIndentation"/> command, which requests that indentation for the current paragraph be decreased by one tab stop.
    /// </summary>
    /// <returns>
    /// The requested command. The default key gesture for this command is Ctrl+Shift+T.
    /// </returns>
    public static RoutedUICommand DecreaseIndentation => EnsureCommand(ref _DecreaseIndentation, nameof(DecreaseIndentation));

    // Spelling commands
    // ---------------------

    /// <summary>
    /// Represents the <see cref="CorrectSpellingError"/> command, which requests that any misspelled word at the current position be corrected.
    /// </summary>
    /// <returns>
    /// The requested command. This command has no default key gesture.
    /// </returns>
    public static RoutedUICommand CorrectSpellingError => EnsureCommand(ref _CorrectSpellingError, nameof(CorrectSpellingError));

    /// <summary>
    /// Represents the <see cref="IgnoreSpellingError"/> command, which requests that any instances of misspelled words at the current position 
    /// or in the current selection be ignored.
    /// </summary>
    /// <returns>
    /// The requested command. This command has no default key gesture.
    /// </returns>
    public static RoutedUICommand IgnoreSpellingError => EnsureCommand(ref _IgnoreSpellingError, nameof(IgnoreSpellingError));

    // Typing commands
    internal static RoutedUICommand Space => EnsureCommand(ref _Space, nameof(Space));
    internal static RoutedUICommand ShiftSpace => EnsureCommand(ref _ShiftSpace, nameof(ShiftSpace));

    // Caret navigation commands
    // -------------------------
    internal static RoutedUICommand MoveToColumnStart => EnsureCommand(ref _MoveToColumnStart, nameof(MoveToColumnStart));
    internal static RoutedUICommand MoveToColumnEnd => EnsureCommand(ref _MoveToColumnEnd, nameof(MoveToColumnEnd));
    internal static RoutedUICommand MoveToWindowTop => EnsureCommand(ref _MoveToWindowTop, nameof(MoveToWindowTop));
    internal static RoutedUICommand MoveToWindowBottom => EnsureCommand(ref _MoveToWindowBottom, nameof(MoveToWindowBottom));

    // Selection extension commands
    // ----------------------------
    internal static RoutedUICommand SelectToColumnStart => EnsureCommand(ref _SelectToColumnStart, nameof(SelectToColumnStart));
    internal static RoutedUICommand SelectToColumnEnd => EnsureCommand(ref _SelectToColumnEnd, nameof(SelectToColumnEnd));
    internal static RoutedUICommand SelectToWindowTop => EnsureCommand(ref _SelectToWindowTop, nameof(SelectToWindowTop));
    internal static RoutedUICommand SelectToWindowBottom => EnsureCommand(ref _SelectToWindowBottom, nameof(SelectToWindowBottom));

    // Character editing commands
    // --------------------------
    internal static RoutedUICommand ResetFormat => EnsureCommand(ref _ResetFormat, nameof(ResetFormat));
    internal static RoutedUICommand ToggleSpellCheck => EnsureCommand(ref _ToggleSpellCheck, nameof(ToggleSpellCheck));

    // BEGIN Application Compatibility Note
    // The following commands are internal, but they are exposed publicly
    // from our command converter.  We cannot change this behavior
    // because it is well documented.  For example, in the
    // "WPF XAML Vocabulary Specification 2006" found here:
    // http://msdn.microsoft.com/en-us/library/dd361848(PROT.10).aspx
    internal static RoutedUICommand ApplyFontSize => EnsureCommand(ref _ApplyFontSize, nameof(ApplyFontSize));
    internal static RoutedUICommand ApplyFontFamily => EnsureCommand(ref _ApplyFontFamily, nameof(ApplyFontFamily));
    internal static RoutedUICommand ApplyForeground => EnsureCommand(ref _ApplyForeground, nameof(ApplyForeground));
    internal static RoutedUICommand ApplyBackground => EnsureCommand(ref _ApplyBackground, nameof(ApplyBackground));
    // END Application Compatibility Note

    internal static RoutedUICommand ApplyInlineFlowDirectionRTL => EnsureCommand(ref _ApplyInlineFlowDirectionRTL, nameof(ApplyInlineFlowDirectionRTL));
    internal static RoutedUICommand ApplyInlineFlowDirectionLTR => EnsureCommand(ref _ApplyInlineFlowDirectionLTR, nameof(ApplyInlineFlowDirectionLTR));

    // Paragraph editing commands
    // --------------------------
    internal static RoutedUICommand ApplySingleSpace => EnsureCommand(ref _ApplySingleSpace, nameof(ApplySingleSpace));
    internal static RoutedUICommand ApplyOneAndAHalfSpace => EnsureCommand(ref _ApplyOneAndAHalfSpace, nameof(ApplyOneAndAHalfSpace));
    internal static RoutedUICommand ApplyDoubleSpace => EnsureCommand(ref _ApplyDoubleSpace, nameof(ApplyDoubleSpace));
    internal static RoutedUICommand ApplyParagraphFlowDirectionRTL => EnsureCommand(ref _ApplyParagraphFlowDirectionRTL, nameof(ApplyParagraphFlowDirectionRTL));
    internal static RoutedUICommand ApplyParagraphFlowDirectionLTR => EnsureCommand(ref _ApplyParagraphFlowDirectionLTR, nameof(ApplyParagraphFlowDirectionLTR));

    // CopyPaste Commands
    // ------------------
    internal static RoutedUICommand CopyFormat => EnsureCommand(ref _CopyFormat, nameof(CopyFormat));
    internal static RoutedUICommand PasteFormat => EnsureCommand(ref _PasteFormat, nameof(PasteFormat));

    // List editing commands
    // ---------------------
    internal static RoutedUICommand RemoveListMarkers => EnsureCommand(ref _RemoveListMarkers, nameof(RemoveListMarkers));

    // Table editing commands
    // ----------------------
    internal static RoutedUICommand InsertTable => EnsureCommand(ref _InsertTable, nameof(InsertTable));
    internal static RoutedUICommand InsertRows => EnsureCommand(ref _InsertRows, nameof(InsertRows));
    internal static RoutedUICommand InsertColumns => EnsureCommand(ref _InsertColumns, nameof(InsertColumns));
    internal static RoutedUICommand DeleteRows => EnsureCommand(ref _DeleteRows, nameof(DeleteRows));
    internal static RoutedUICommand DeleteColumns => EnsureCommand(ref _DeleteColumns, nameof(DeleteColumns));
    internal static RoutedUICommand MergeCells => EnsureCommand(ref _MergeCells, nameof(MergeCells));
    internal static RoutedUICommand SplitCell => EnsureCommand(ref _SplitCell, nameof(SplitCell));

    // Initializes a static command definition - by demand
    private static RoutedUICommand EnsureCommand(ref RoutedUICommand command, string commandPropertyName)
    {
        lock (_synchronize)
        {
            // The first parameter should be localized
            command ??= new RoutedUICommand(commandPropertyName, commandPropertyName, typeof(EditingCommands));
        }
        return command;
    }

    private static readonly object _synchronize = new();

    // Input commands
    // --------------
    private static RoutedUICommand _ToggleInsert;
    private static RoutedUICommand _Delete;
    private static RoutedUICommand _Clear;
    private static RoutedUICommand _Backspace;
    private static RoutedUICommand _DeleteNextWord;
    private static RoutedUICommand _DeletePreviousWord;
    private static RoutedUICommand _EnterParagraphBreak;
    private static RoutedUICommand _EnterLineBreak;
    private static RoutedUICommand _TabForward;
    private static RoutedUICommand _TabBackward;
    private static RoutedUICommand _Space;
    private static RoutedUICommand _ShiftSpace;

    // Caret navigation commands
    // -------------------------
    private static RoutedUICommand _MoveRightByCharacter;
    private static RoutedUICommand _MoveLeftByCharacter;
    private static RoutedUICommand _MoveRightByWord;
    private static RoutedUICommand _MoveLeftByWord;
    private static RoutedUICommand _MoveDownByLine;
    private static RoutedUICommand _MoveUpByLine;
    private static RoutedUICommand _MoveDownByParagraph;
    private static RoutedUICommand _MoveUpByParagraph;
    private static RoutedUICommand _MoveDownByPage;
    private static RoutedUICommand _MoveUpByPage;
    private static RoutedUICommand _MoveToLineStart;
    private static RoutedUICommand _MoveToLineEnd;
    private static RoutedUICommand _MoveToColumnStart;
    private static RoutedUICommand _MoveToColumnEnd;
    private static RoutedUICommand _MoveToWindowTop;
    private static RoutedUICommand _MoveToWindowBottom;
    private static RoutedUICommand _MoveToDocumentStart;
    private static RoutedUICommand _MoveToDocumentEnd;

    // Selection extension commands
    // ----------------------------
    private static RoutedUICommand _SelectRightByCharacter;
    private static RoutedUICommand _SelectLeftByCharacter;
    private static RoutedUICommand _SelectRightByWord;
    private static RoutedUICommand _SelectLeftByWord;
    private static RoutedUICommand _SelectDownByLine;
    private static RoutedUICommand _SelectUpByLine;
    private static RoutedUICommand _SelectDownByParagraph;
    private static RoutedUICommand _SelectUpByParagraph;
    private static RoutedUICommand _SelectDownByPage;
    private static RoutedUICommand _SelectUpByPage;
    private static RoutedUICommand _SelectToLineStart;
    private static RoutedUICommand _SelectToLineEnd;
    private static RoutedUICommand _SelectToColumnStart;
    private static RoutedUICommand _SelectToColumnEnd;
    private static RoutedUICommand _SelectToWindowTop;
    private static RoutedUICommand _SelectToWindowBottom;
    private static RoutedUICommand _SelectToDocumentStart;
    private static RoutedUICommand _SelectToDocumentEnd;

    // Character editing commands
    // --------------------------
    private static RoutedUICommand _CopyFormat;
    private static RoutedUICommand _PasteFormat;
    private static RoutedUICommand _ResetFormat;
    private static RoutedUICommand _ToggleBold;
    private static RoutedUICommand _ToggleItalic;
    private static RoutedUICommand _ToggleUnderline;
    private static RoutedUICommand _ToggleSubscript;
    private static RoutedUICommand _ToggleSuperscript;
    private static RoutedUICommand _IncreaseFontSize;
    private static RoutedUICommand _DecreaseFontSize;
    private static RoutedUICommand _ApplyFontSize;
    private static RoutedUICommand _ApplyFontFamily;
    private static RoutedUICommand _ApplyForeground;
    private static RoutedUICommand _ApplyBackground;
    private static RoutedUICommand _ToggleSpellCheck;
    private static RoutedUICommand _ApplyInlineFlowDirectionRTL;
    private static RoutedUICommand _ApplyInlineFlowDirectionLTR;

    // Paragraph editing commands
    // --------------------------
    private static RoutedUICommand _AlignLeft;
    private static RoutedUICommand _AlignCenter;
    private static RoutedUICommand _AlignRight;
    private static RoutedUICommand _AlignJustify;
    private static RoutedUICommand _ApplySingleSpace;
    private static RoutedUICommand _ApplyOneAndAHalfSpace;
    private static RoutedUICommand _ApplyDoubleSpace;
    private static RoutedUICommand _IncreaseIndentation;
    private static RoutedUICommand _DecreaseIndentation;
    private static RoutedUICommand _ApplyParagraphFlowDirectionRTL;
    private static RoutedUICommand _ApplyParagraphFlowDirectionLTR;

    // List editing commands
    // ---------------------
    private static RoutedUICommand _RemoveListMarkers;
    private static RoutedUICommand _ToggleBullets;
    private static RoutedUICommand _ToggleNumbering;

    // Table editing commands
    // ----------------------
    private static RoutedUICommand _InsertTable;
    private static RoutedUICommand _InsertRows;
    private static RoutedUICommand _InsertColumns;
    private static RoutedUICommand _DeleteRows;
    private static RoutedUICommand _DeleteColumns;
    private static RoutedUICommand _MergeCells;
    private static RoutedUICommand _SplitCell;

    // Spelling Commands
    // -----------------
    private static RoutedUICommand _CorrectSpellingError;
    private static RoutedUICommand _IgnoreSpellingError;
}
