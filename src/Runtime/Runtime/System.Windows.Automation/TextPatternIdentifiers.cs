
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

using System.Globalization;

#if MIGRATION
using System.Windows.Automation.Provider;
#else
using Windows.UI.Xaml.Automation.Provider;
#endif

#if MIGRATION
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Contains values that are used as identifiers for the <see cref="ITextProvider"/> class.
    /// </summary>
    public static class TextPatternIdentifiers
    {
        /// <summary>
        /// Identifies the AnimationStyle attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute AnimationStyleAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.AnimationStyle,
                "TextPatternIdentifiers.AnimationStyleAttribute");

        /// <summary>
        /// Identifies the BackgroundColor attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute BackgroundColorAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.BackgroundColor,
                "TextPatternIdentifiers.BackgroundColorAttribute");

        /// <summary>
        /// Identifies the System.Windows.Automation.Text.BulletStyle attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute BulletStyleAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.BulletStyle,
                "TextPatternIdentifiers.BulletStyleAttribute");

        /// <summary>
        /// Identifies the CapStyle attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute CapStyleAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.CapStyle,
                "TextPatternIdentifiers.CapStyleAttribute");

        /// <summary>
        /// Identifies the Culture (<see cref="CultureInfo"/>) attribute of a text
        /// range. Includes the sub-language level; for example, specifies French–Switzerland
        /// (fr-CH) instead of merely French (fr).
        /// </summary>
        public static readonly AutomationTextAttribute CultureAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.Culture,
                "TextPatternIdentifiers.CultureAttribute");

        /// <summary>
        /// Identifies the FontName attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute FontNameAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.FontName,
                "TextPatternIdentifiers.FontNameAttribute");

        /// <summary>
        /// Identifies the FontSize attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute FontSizeAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.FontSize,
                "TextPatternIdentifiers.FontSizeAttribute");

        /// <summary>
        /// Identifies the FontWeight attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute FontWeightAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.FontWeight,
                "TextPatternIdentifiers.FontWeightAttribute");

        /// <summary>
        /// Identifies the ForegroundColor attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute ForegroundColorAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.ForegroundColor,
                "TextPatternIdentifiers.ForegroundColorAttribute");

        /// <summary>
        /// Identifies the HorizontalTextAlignment attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute HorizontalTextAlignmentAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.HorizontalTextAlignment,
                "TextPatternIdentifiers.HorizontalTextAlignmentAttribute");

        /// <summary>
        /// Identifies the IndentationFirstLine attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IndentationFirstLineAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IndentationFirstLine,
                "TextPatternIdentifiers.IndentationFirstLineAttribute");

        /// <summary>
        /// Identifies the IndentationLeadingattribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IndentationLeadingAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IndentationLeading,
                "TextPatternIdentifiers.IndentationLeadingAttribute");

        /// <summary>
        /// Identifies the IndentationTrailingattribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IndentationTrailingAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IndentationTrailing,
                "TextPatternIdentifiers.IndentationTrailingAttribute");

        /// <summary>
        /// Identifies the IsHidden attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IsHiddenAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IsHidden,
                "TextPatternIdentifiers.IsHiddenAttribute");

        /// <summary>
        /// Identifies the IsItalic attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IsItalicAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IsItalic,
                "TextPatternIdentifiers.IsItalicAttribute");

        /// <summary>
        /// Identifies the IsReadOnly attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IsReadOnlyAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IsReadOnly,
                "TextPatternIdentifiers.IsReadOnlyAttribute");

        /// <summary>
        /// Identifies the IsSubscript attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IsSubscriptAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IsSubscript,
                "TextPatternIdentifiers.IsSubscriptAttribute");

        /// <summary>
        /// Identifies the IsSuperscript attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute IsSuperscriptAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.IsSuperscript,
                "TextPatternIdentifiers.IsSuperscriptAttribute");

        /// <summary>
        /// Identifies the MarginBottom attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute MarginBottomAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.MarginBottom,
                "TextPatternIdentifiers.MarginBottomAttribute");

        /// <summary>
        /// Identifies the MarginLeading attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute MarginLeadingAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.MarginLeading,
                "TextPatternIdentifiers.MarginLeadingAttribute");
        
        /// <summary>
        /// Identifies the MarginTop attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute MarginTopAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.MarginTop,
                "TextPatternIdentifiers.MarginTopAttribute");

        /// <summary>
        /// Identifies the MarginTrailing attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute MarginTrailingAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.MarginTrailing,
                "TextPatternIdentifiers.MarginTrailingAttribute");

        /// <summary>
        /// Identifies the OutlineStyles attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute OutlineStylesAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.OutlineStyles,
                "TextPatternIdentifiers.OutlineStylesAttribute");

        /// <summary>
        /// Identifies the OverlineColor attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute OverlineColorAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.OverlineColor,
                "TextPatternIdentifiers.OverlineColorAttribute");

        /// <summary>
        /// Identifies the OverlineStyle attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute OverlineStyleAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.OverlineStyle,
                "TextPatternIdentifiers.OverlineStyleAttribute");

        /// <summary>
        /// Identifies the StrikethroughColor attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute StrikethroughColorAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.StrikethroughColor,
                "TextPatternIdentifiers.StrikethroughColorAttribute");

        /// <summary>
        /// Identifies the StrikethroughStyle attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute StrikethroughStyleAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.StrikethroughStyle,
                "TextPatternIdentifiers.StrikethroughStyleAttribute");

        /// <summary>
        /// Identifies the Tabs attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute TabsAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.Tabs,
                "TextPatternIdentifiers.TabsAttribute");

        /// <summary>
        /// Identifies the TextFlowDirections attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute TextFlowDirectionsAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.TextFlowDirections,
                "TextPatternIdentifiers.TextFlowDirectionsAttribute");

        /// <summary>
        /// Identifies the UnderlineColor attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute UnderlineColorAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.UnderlineColor,
                "TextPatternIdentifiers.UnderlineColorAttribute");

        /// <summary>
        /// Identifies the UnderlineStyle attribute of a text range.
        /// </summary>
        public static readonly AutomationTextAttribute UnderlineStyleAttribute =
            AutomationTextAttribute.Register(
                AutomationIdentifierConstants.TextAttributes.UnderlineStyle,
                "TextPatternIdentifiers.UnderlineStyleAttribute");
    }
}
