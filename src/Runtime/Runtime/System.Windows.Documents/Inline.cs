
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

namespace System.Windows.Documents;

/// <summary>
/// Provides a base for inline flow content element behavior.
/// </summary>
public abstract class Inline : TextElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Inline"/> class.
    /// </summary>
    protected Inline() { }

    /// <summary>
    /// Identifies the <see cref="TextElement.FontSize" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty;

    /// <summary>
    /// Identifies the <see cref="TextElement.FontFamily" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty;

    /// <summary>
    /// Identifies the <see cref="TextElement.FontWeight" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty;

    /// <summary>
    /// Identifies the <see cref="TextElement.FontStyle" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty;

    /// <summary>
    /// Identifies the <see cref="TextElement.FontStretch" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty;

    /// <summary>
    /// Identifies the <see cref="TextElement.Foreground" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty;

    /// <summary>
    /// Identifies the <see cref="TextElement.Language" /> dependency property.
    /// </summary>
    public new static readonly DependencyProperty LanguageProperty = TextElement.LanguageProperty;

    /// <summary>
    /// Identifies the <see cref="TextDecorations"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TextDecorationsProperty =
        DependencyProperty.RegisterAttached(
            nameof(TextDecorations),
            typeof(TextDecorationCollection),
            typeof(Inline),
            new PropertyMetadata((object)null) { Inherits = true, });

    /// <summary>
    /// Gets or sets a value that specifies the text decorations that are applied to
    /// the content in an <see cref="Inline"/> element.
    /// </summary>
    /// <returns>
    /// A <see cref="TextDecorationCollection"/>, or null if no text decorations are
    /// applied.
    /// </returns>
    public TextDecorationCollection TextDecorations
    {
        get => (TextDecorationCollection)GetValue(TextDecorationsProperty);
        set => SetValueInternal(TextDecorationsProperty, value);
    }

    // Defining an implicit conversion from string to Inline allows to
    // support the following usage: TextBlock1.Inlines.Add("test");
    public static implicit operator Inline(string s) => new Run() { Text = s };
}
