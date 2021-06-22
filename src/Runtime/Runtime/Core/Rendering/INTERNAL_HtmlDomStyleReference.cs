

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


//#define CHECK_THAT_ID_EXISTS 
//#define PERFORMANCE_ANALYSIS

#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
#if !BRIDGE
    [JSIgnore]
#else
    [External]
#endif

#if CSHTML5NETSTANDARD
    public class INTERNAL_HtmlDomStyleReference : DynamicObject
#else
    internal class INTERNAL_HtmlDomStyleReference : DynamicObject
#endif
    {
#if PERFORMANCE_ANALYSIS
        static Dictionary<string, int> NumberOfTimesEachDynamicMemberIsCalled = new Dictionary<string, int>();
#endif

        static Dictionary<string, INTERNAL_HtmlDomStyleReference> IdToInstance = new Dictionary<string, INTERNAL_HtmlDomStyleReference>();

        public static INTERNAL_HtmlDomStyleReference GetInstance(string elementId)
        {
            if (IdToInstance.ContainsKey(elementId))
                return IdToInstance[elementId];
            else
            {
                var newInstance = new INTERNAL_HtmlDomStyleReference(elementId);
                IdToInstance[elementId] = newInstance;
                return newInstance;
            }
        }

        string _domElementUniqueIdentifier;

        // Note: It's important that the constructor stays Private because we need to recycle the instances that correspond to the same ID using the "GetInstance" public static method, so thateach ID always corresponds to the same instance. This is useful to ensure that private fields such as "_display" work propertly.
        private INTERNAL_HtmlDomStyleReference(string elementId)
        {
            _domElementUniqueIdentifier = elementId;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            //            string javaScriptCodeToExecute = string.Format(@"
            //                    var element = document.getElementByIdSafe(""{0}"");
            //                    element.style.{1} = ""{2}"";
            //                ", _domElementUniqueIdentifier, binder.Name, value);

            SetStylePropertyValue(binder.Name, (value ?? "").ToString());

            //System.Diagnostics.Debug.WriteLine("Style property: " + binder.Name);

#if PERFORMANCE_ANALYSIS
            if (NumberOfTimesEachDynamicMemberIsCalled.ContainsKey(binder.Name))
                NumberOfTimesEachDynamicMemberIsCalled[binder.Name] = NumberOfTimesEachDynamicMemberIsCalled[binder.Name] + 1;
            else
                NumberOfTimesEachDynamicMemberIsCalled.Add(binder.Name, 1);
#endif

#if CHECK_THAT_ID_EXISTS
            var domElement = INTERNAL_HtmlDomManager.ExecuteJavaScriptWithResult("document.getElementByIdSafe(\"" + _domElementUniqueIdentifier + "\")");
            if (domElement == null)
                throw new Exception("DOM element ID not found: " + _domElementUniqueIdentifier);
#endif

            return true;
        }

        void SetStylePropertyValue(string propertyName, string propertyValue)
        {
            string javaScriptCodeToExecute = "var element = document.getElementByIdSafe(\"" + _domElementUniqueIdentifier + "\");if (element) { element.style." + propertyName + " = \"" + propertyValue + "\"; } ";
            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
        }

        //        string GetStylePropertyValue(string propertyName)
        //        {
        //            return ExecuteJavaScriptWithResult(
        //                    string.Format(@"
        //var element = document.getElementByIdSafe(""{0}"");
        //if (element)
        //    return element.style.{1};
        //else
        //    return """";", _domElementUniqueIdentifier, propertyName));
        //        }

        string _display = "block";
        string _width = "";
        string _height = "";
        string _maxWidth = "";
        string _maxHeight = "";

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
        public string boxSizing { set { SetStylePropertyValue("boxSizing", value); } }
        public string boxShadow { set { SetStylePropertyValue("boxShadow", value); } }
        public string color { set { SetStylePropertyValue("color", value); } }
        public string cursor { set { SetStylePropertyValue("cursor", value); } }
        public string display { set { SetStylePropertyValue("display", value); _display = value; } get { return _display; } }
        public string fontSize { set { SetStylePropertyValue("fontSize", value); } }
        public string gridColumnEnd { set { SetStylePropertyValue("gridColumnEnd", value); } }
        public string gridColumnStart { set { SetStylePropertyValue("gridColumnStart", value); } }
        public string gridRowEnd { set { SetStylePropertyValue("gridRowEnd", value); } }
        public string gridRowStart { set { SetStylePropertyValue("gridRowStart", value); } }
        public string gridTemplateColumns { set { SetStylePropertyValue("gridTemplateColumns", value); } }
        public string gridTemplateRows { set { SetStylePropertyValue("gridTemplateRows", value); } }
        public string height { set { SetStylePropertyValue("height", value); _height = value; } get { return _height; } }
        public string left { set { SetStylePropertyValue("left", value); } }
        public string lineHeight { set { SetStylePropertyValue("lineHeight", value); } }
        public string marginBottom { set { SetStylePropertyValue("marginBottom", value); } }
        public string marginLeft { set { SetStylePropertyValue("marginLeft", value); } }
        public string marginRight { set { SetStylePropertyValue("marginRight", value); } }
        public string marginTop { set { SetStylePropertyValue("marginTop", value); } }
        public string minHeight { set { SetStylePropertyValue("minHeight", value); } }
        public string minWidth { set { SetStylePropertyValue("minWidth", value); } }
        public string maxHeight { set { SetStylePropertyValue("maxHeight", value); _maxHeight = value; } get { return _maxHeight; } }
        public string maxWidth { set { SetStylePropertyValue("maxWidth", value); _maxWidth = value; } get { return _maxWidth; } }
        public string msGridColumn { set { SetStylePropertyValue("msGridColumn", value); } }
        public string msGridColumns { set { SetStylePropertyValue("msGridColumns", value); } }
        public string msGridColumnSpan { set { SetStylePropertyValue("msGridColumnSpan", value); } }
        public string msGridRow { set { SetStylePropertyValue("msGridRow", value); } }
        public string msGridRows { set { SetStylePropertyValue("msGridRows", value); } }
        public string msGridRowSpan { set { SetStylePropertyValue("msGridRowSpan", value); } }
        public string msGridRowAlign { set { SetStylePropertyValue("msGridRowAlign", value); } }
        public string msTransform { set { SetStylePropertyValue("msTransform", value); } }
        public string msTransformOrigin { set { SetStylePropertyValue("msTransformOrigin", value); } }
        public string objectPosition { set { SetStylePropertyValue("objectPosition", value); } }
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
        public string position { set { SetStylePropertyValue("position", value); } }
        public string pointerEvents { set { SetStylePropertyValue("pointerEvents", value); } }
        public string tableLayout { set { SetStylePropertyValue("tableLayout", value); } }
        public string textAlign { set { SetStylePropertyValue("textAlign", value); } }
        public string textDecoration { set { SetStylePropertyValue("textDecoration", value); } }
        public string textOverflow { set { SetStylePropertyValue("textOverflow", value); } }
        public string textShadow { set { SetStylePropertyValue("textShadow", value); } }
        public string transform { set { SetStylePropertyValue("transform", value); } }
        public string transformOrigin { set { SetStylePropertyValue("transformOrigin", value); } }
        public string top { set { SetStylePropertyValue("top", value); } }
        public string verticalAlign { set { SetStylePropertyValue("verticalAlign", value); } }
        public string WebkitOverflowScrolling { set { SetStylePropertyValue("WebkitOverflowScrolling", value); } }
        public string WebkitTransform { set { SetStylePropertyValue("WebkitTransform", value); } }
        public string webkitTransformOrigin { set { SetStylePropertyValue("webkitTransformOrigin", value); } }
        public string whiteSpace { set { SetStylePropertyValue("whiteSpace", value); } }
        public string width { set { SetStylePropertyValue("width", value); _width = value; } get { return _width; } }
        public string zIndex { set { SetStylePropertyValue("zIndex", value); } }
        
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
    }
}
