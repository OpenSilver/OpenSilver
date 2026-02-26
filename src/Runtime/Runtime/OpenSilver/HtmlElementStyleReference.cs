
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
using System.ComponentModel;

namespace OpenSilver;

[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct HtmlElementStyleReference
{
#pragma warning disable IDE1006 // Naming Styles
    internal string Uid { get; }

    internal HtmlElementStyleReference(string elementId)
    {
        Uid = elementId;
    }

    public string background { set { SetStylePropertyValue(CssPropertyNames.Background, value); } }
    public string backgroundClip { set { SetStylePropertyValue(CssPropertyNames.BackgroundClip, value); } }
    public string backgroundImage { set { SetStylePropertyValue(CssPropertyNames.BackgroundImage, value); } }
    public string border { set { SetStylePropertyValue(CssPropertyNames.Border, value); } }
    public string borderColor { set { SetStylePropertyValue(CssPropertyNames.BorderColor, value); } }
    public string borderRadius { set { SetStylePropertyValue(CssPropertyNames.BorderRadius, value); } }
    public string borderWidth { set { SetStylePropertyValue(CssPropertyNames.BorderWidth, value); } }
    public string borderImageSource { set { SetStylePropertyValue(CssPropertyNames.BorderImageSource, value); } }
    public string borderImageSlice { set { SetStylePropertyValue(CssPropertyNames.BorderImageSlice, value); } }
    public string boxShadow { set { SetStylePropertyValue(CssPropertyNames.BoxShadow, value); } }
    public string caretColor { set { SetStylePropertyValue(CssPropertyNames.CaretColor, value); } }
    public string color { set { SetStylePropertyValue(CssPropertyNames.Color, value); } }
    public string cursor { set { SetStylePropertyValue(CssPropertyNames.Cursor, value); } }
    public string display { get { return GetCSSProperty(CssPropertyNames.Display); } set { SetStylePropertyValue(CssPropertyNames.Display, value); } }
    public string filter { set { SetStylePropertyValue(CssPropertyNames.Filter, value); } }
    public string fontFamily { set { SetStylePropertyValue(CssPropertyNames.FontFamily, value); } }
    public string fontSize { set { SetStylePropertyValue(CssPropertyNames.FontSize, value); } }
    public string fontStyle { set { SetStylePropertyValue(CssPropertyNames.FontStyle, value); } }
    public string fontWeight { set { SetStylePropertyValue(CssPropertyNames.FontWeight, value); } }
    public string height { get { return GetCSSProperty(CssPropertyNames.Height); } set { SetStylePropertyValue(CssPropertyNames.Height, value); } }
    public string lineHeight { set { SetStylePropertyValue(CssPropertyNames.LineHeight, value); } }
    public string letterSpacing { set { SetStylePropertyValue(CssPropertyNames.LetterSpacing, value); } }
    public string margin { set { SetStylePropertyValue(CssPropertyNames.Margin, value); } }
    public string maskImage { set { SetStylePropertyValue(CssPropertyNames.MaskImage, value); } }
    public string objectPosition { set { SetStylePropertyValue(CssPropertyNames.ObjectPosition, value); } }
    public string objectFit { set { SetStylePropertyValue(CssPropertyNames.ObjectFit, value); } }
    public string opacity { set { SetStylePropertyValue(CssPropertyNames.Opacity, value); } }
    public string outline { set { SetStylePropertyValue(CssPropertyNames.Outline, value); } }
    public string overflow { set { SetStylePropertyValue(CssPropertyNames.Overflow, value); } }
    public string padding { set { SetStylePropertyValue(CssPropertyNames.Padding, value); } }
    public string position { set { SetStylePropertyValue(CssPropertyNames.Position, value); } }
    public string pointerEvents { set { SetStylePropertyValue(CssPropertyNames.PointerEvents, value); } }
    public string textAlign { set { SetStylePropertyValue(CssPropertyNames.TextAlign, value); } }
    public string textDecoration { set { SetStylePropertyValue(CssPropertyNames.TextDecoration, value); } }
    public string textShadow { set { SetStylePropertyValue(CssPropertyNames.TextShadow, value); } }
    public string transform { set { SetStylePropertyValue(CssPropertyNames.Transform, value); } }
    public string transformOrigin { set { SetStylePropertyValue(CssPropertyNames.TransformOrigin, value); } }
    public string touchAction { set { SetStylePropertyValue(CssPropertyNames.TouchAction, value); } }
    public string whiteSpace { set { SetStylePropertyValue(CssPropertyNames.WhiteSpace, value); } }
    public string width { get { return GetCSSProperty(CssPropertyNames.Width); } set { SetStylePropertyValue(CssPropertyNames.Width, value); } }
    public string zIndex { set { SetStylePropertyValue(CssPropertyNames.ZIndex, value); } }
    public string clipPath { set { SetStylePropertyValue(CssPropertyNames.ClipPath, value); } }
    public string overflowWrap { set { SetStylePropertyValue(CssPropertyNames.OverflowWrap, value); } }
    public string userSelect { set { SetStylePropertyValue(CssPropertyNames.UserSelect, value); } }

    private void SetStylePropertyValue(string propertyName, string value) =>
        Interop.ExecuteJavaScriptVoidAsync(
            $"osjs.setCSS('{Uid}','{propertyName}','{value}')");

    private string GetCSSProperty(string propertyName) =>
        Interop.ExecuteJavaScriptString(
            $"document.getElementById('{Uid}').style.{propertyName}");

    //-----------------------------------------------------------------------
    // Usage stats for To-Do Calendar (number of types each property is set):
    //-----------------------------------------------------------------------
    //backgroundColor, 373
    //borderBottomLeftRadius, 2
    //borderBottomRightRadius, 2
    //borderCollapse, 1
    //borderTopLeftRadius, 2
    //borderTopRightRadius, 2
    //boxSizing, 2206
    //color, 370
    //display, 3837
    //fontSize, 3
    //height, 5330
    //marginLeft, 2182
    //marginRight, 2182
    //overflow, 1
    //overflowX, 2000
    //overflowY, 552
    //padding, 2
    //paddingBottom, 187
    //paddingLeft, 187
    //paddingRight, 187
    //paddingTop, 187
    //position, 4585
    //textAlign, 373
    //verticalAlign, 552
    //whiteSpace, 378
    //width, 6973
#pragma warning restore IDE1006 // Naming Styles
}
