

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

using System;
using System.Dynamic;

namespace CSHTML5.Internal
{
    public class INTERNAL_HtmlDomStyleReference : DynamicObject
    {
#pragma warning disable IDE1006 // Naming Styles
        internal string Uid { get; }

        internal INTERNAL_HtmlDomStyleReference(string elementId)
        {
            Uid = elementId;
        }

        [Obsolete("This method is not meant to be used outside of OpenSilver.")]
        public static INTERNAL_HtmlDomStyleReference GetInstance(string elementId)
            => new INTERNAL_HtmlDomStyleReference(elementId);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetStylePropertyValue(binder.Name, (value ?? string.Empty).ToString());
            return true;
        }

        public string background { set { SetStylePropertyValue("background", value); } }
        public string backgroundColor { set { SetStylePropertyValue("backgroundColor", value); } }
        public string border { set { SetStylePropertyValue("border", value); } }
        public string borderRight { set { SetStylePropertyValue("borderRight", value); } }
        public string borderLeft { set { SetStylePropertyValue("borderLeft", value); } }
        public string borderBottom { set { SetStylePropertyValue("borderBottom", value); } }
        public string borderTop { set { SetStylePropertyValue("borderTop", value); } }
        public string borderBottomLeftRadius { set { SetStylePropertyValue("borderBottomLeftRadius", value); } }
        public string borderBottomRightRadius { set { SetStylePropertyValue("borderBottomRightRadius", value); } }
        public string borderCollapse { set { SetStylePropertyValue("borderCollapse", value); } }
        public string borderColor { set { SetStylePropertyValue("borderColor", value); } }
        public string borderRadius { set { SetStylePropertyValue("borderRadius", value); } }
        public string borderSpacing { set { SetStylePropertyValue("borderSpacing", value); } }
        public string borderStyle { set { SetStylePropertyValue("borderStyle", value); } }
        public string borderTopLeftRadius { set { SetStylePropertyValue("borderTopLeftRadius", value); } }
        public string borderTopRightRadius { set { SetStylePropertyValue("borderTopRightRadius", value); } }
        public string borderWidth { set { SetStylePropertyValue("borderWidth", value); } }
        public string borderImageSource { set { SetStylePropertyValue("border-image-source", value); } }
        public string borderImageSlice { set { SetStylePropertyValue("border-image-slice", value); } }
        public string boxSizing { set { SetStylePropertyValue("boxSizing", value); } }
        public string boxShadow { set { SetStylePropertyValue("boxShadow", value); } }
        public string color { set { SetStylePropertyValue("color", value); } }
        public string cursor { set { SetStylePropertyValue("cursor", value); } }
        public string display { get { return GetCSSProperty("display"); } set { SetStylePropertyValue("display", value); } }
        public string filter { set { SetStylePropertyValue("filter", value); } }
        public string fontFamily { set { SetStylePropertyValue("fontFamily", value); } }
        public string fontSize { set { SetStylePropertyValue("fontSize", value); } }
        public string fontStyle { set { SetStylePropertyValue("fontStyle", value); } }
        public string fontWeight { set { SetStylePropertyValue("fontWeight", value); } }
        public string gridColumnEnd { set { SetStylePropertyValue("gridColumnEnd", value); } }
        public string gridColumnStart { set { SetStylePropertyValue("gridColumnStart", value); } }
        public string gridRowEnd { set { SetStylePropertyValue("gridRowEnd", value); } }
        public string gridRowStart { set { SetStylePropertyValue("gridRowStart", value); } }
        public string gridTemplateColumns { set { SetStylePropertyValue("gridTemplateColumns", value); } }
        public string gridTemplateRows { set { SetStylePropertyValue("gridTemplateRows", value); } }
        public string height { get { return GetCSSProperty("height"); } set { SetStylePropertyValue("height", value); } }
        public string left { set { SetStylePropertyValue("left", value); } }
        public string lineHeight { set { SetStylePropertyValue("lineHeight", value); } }
        public string margin { set { SetStylePropertyValue("margin", value); } }
        public string marginBottom { set { SetStylePropertyValue("marginBottom", value); } }
        public string marginLeft { set { SetStylePropertyValue("marginLeft", value); } }
        public string marginRight { set { SetStylePropertyValue("marginRight", value); } }
        public string marginTop { set { SetStylePropertyValue("marginTop", value); } }
        public string marginInlineStart { set { SetStylePropertyValue("marginInlineStart", value); } }
        public string marginInlineEnd { set { SetStylePropertyValue("marginInlineEnd", value); } }
        public string minHeight { set { SetStylePropertyValue("minHeight", value); } }
        public string minWidth { set { SetStylePropertyValue("minWidth", value); } }
        public string maxHeight { get { return GetCSSProperty("maxHeight"); } set { SetStylePropertyValue("maxHeight", value); } }
        public string maxWidth { get { return GetCSSProperty("maxWidth"); } set { SetStylePropertyValue("maxWidth", value); } }
        public string msGridColumn { set { SetStylePropertyValue("msGridColumn", value); } }
        public string msGridColumns { set { SetStylePropertyValue("msGridColumns", value); } }
        public string msGridColumnSpan { set { SetStylePropertyValue("msGridColumnSpan", value); } }
        public string msGridRow { set { SetStylePropertyValue("msGridRow", value); } }
        public string msGridRows { set { SetStylePropertyValue("msGridRows", value); } }
        public string msGridRowSpan { set { SetStylePropertyValue("msGridRowSpan", value); } }
        public string msGridRowAlign { set { SetStylePropertyValue("msGridRowAlign", value); } }
        public string msTransform { set { transform = value; } }
        public string msTransformOrigin { set { transformOrigin = value; } }
        public string objectPosition { set { SetStylePropertyValue("objectPosition", value); } }
        public string objectFit { set { SetStylePropertyValue("objectFit", value); } }
        public string opacity { set { SetStylePropertyValue("opacity", value); } }
        public string outline { set { SetStylePropertyValue("outline", value); } }
        public string overflow { set { SetStylePropertyValue("overflow", value); } }
        public string overflowX { set { SetStylePropertyValue("overflowX", value); } }
        public string overflowY { set { SetStylePropertyValue("overflowY", value); } }
        public string padding { set { SetStylePropertyValue("padding", value); } }
        public string paddingBottom { set { SetStylePropertyValue("paddingBottom", value); } }
        public string paddingLeft { set { SetStylePropertyValue("paddingLeft", value); } }
        public string paddingRight { set { SetStylePropertyValue("paddingRight", value); } }
        public string paddingTop { set { SetStylePropertyValue("paddingTop", value); } }
        public string paddingInlineStart { set { SetStylePropertyValue("paddingInlineStart", value); } }
        public string paddingInlineEnd { set { SetStylePropertyValue("paddingInlineEnd", value); } }
        public string position { set { SetStylePropertyValue("position", value); } }
        public string pointerEvents { set { SetStylePropertyValue("pointerEvents", value); } }
        public string resize { set { SetStylePropertyValue("resize", value); } }
        public string tableLayout { set { SetStylePropertyValue("tableLayout", value); } }
        public string textAlign { set { SetStylePropertyValue("textAlign", value); } }
        public string textDecoration { set { SetStylePropertyValue("textDecoration", value); } }
        public string textOverflow { set { SetStylePropertyValue("textOverflow", value); } }
        public string textShadow { set { SetStylePropertyValue("textShadow", value); } }
        public string transform { set { SetTransformPropertyValue(value); } }
        public string transformOrigin { set { SetTransformOriginPropertyValue(value); } }
        public string top { set { SetStylePropertyValue("top", value); } }
        public string verticalAlign { set { SetStylePropertyValue("verticalAlign", value); } }
        public string WebkitOverflowScrolling { set { SetStylePropertyValue("WebkitOverflowScrolling", value); } }
        public string WebkitTransform { set { transform = value; } }
        public string webkitTransformOrigin { set { transformOrigin = value; } }
        public string whiteSpace { set { SetStylePropertyValue("whiteSpace", value); } }
        public string width { get { return GetCSSProperty("width"); } set { SetStylePropertyValue("width", value); } }
        public string zIndex { set { SetStylePropertyValue("zIndex", value); } }
        public string gridArea { set { SetStylePropertyValue("gridArea", value); } }
        public string visibility { set { SetStylePropertyValue("visibility", value); } }
        public string clip { set { SetStylePropertyValue("clip", value); } }
        public string overflowWrap { set { SetStylePropertyValue("overflowWrap", value); } }
        public string alignItems { set { SetStylePropertyValue("alignItems", value); } }
        public string justifyContent { set { SetStylePropertyValue("justifyContent", value); } }
        public string flexWrap { set { SetStylePropertyValue("flexWrap", value); } }
        public string flexDirection { set { SetStylePropertyValue("flexDirection", value); } }
        public string flex { set { SetStylePropertyValue("flex", value); } }
        public string flexGrow { set { SetStylePropertyValue("flexGrow", value); } }
        public string flexShrink { set { SetStylePropertyValue("flexShrink", value); } }
        public string flexBasis { set { SetStylePropertyValue("flexBasis", value); } }
        public string userSelect { set { SetStylePropertyValue("userSelect", value); } }

        internal void setProperty(string propertyName, string value) =>
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(
                $"document.setStyleProperty('{Uid}', '{propertyName}', '{value}');");

        internal void setProperty(string propertyName, string value, string priority) =>
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(
                $"document.setStyleProperty('{Uid}', '{propertyName}', '{value}', '{priority}');");

        private void SetStylePropertyValue(string propertyName, string value) =>
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(
                $"document.setDomStyle('{Uid}', '{propertyName}', '{value}');");

        private void SetTransformPropertyValue(string propertyValue) =>
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(
                $"document.setDomTransform('{Uid}', '{propertyValue}');");

        private void SetTransformOriginPropertyValue(string propertyValue) =>
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(
                $"document.setDomTransformOrigin('{Uid}', '{propertyValue}');");

        private string GetCSSProperty(string propertyName) =>
            OpenSilver.Interop.ExecuteJavaScriptString(
                $"document.getElementById('{Uid}').style.{propertyName};");

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
}
