

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


#if !BRIDGE
using JSIL;
using JSIL.Meta;
#if !BUILDINGDOCUMENTATION && !CSHTML5NETSTANDARD
using DotNetBrowser;
#endif
#else
using Bridge;
#endif
using OpenSilver.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSHTML5;
using DotNetForHtml5.Core;

#if !CSHTML5NETSTANDARD
using DotNetBrowser;
#endif
#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace CSHTML5.Internal // IMPORTANT: if you change this namespace, make sure to change the dynamic call from the Simulator as well.
{
    public static class INTERNAL_HtmlDomManager // Note: this class is "internal" but still visible to the Simulator because of the "InternalsVisibleTo" flag in "Assembly.cs".
    {
        //------
        // All JavaScript functions (called through dynamic objects) for manipulating the DOM should go here.
        //------
        private static readonly Dictionary<string, UIElement> _store;
        private static readonly ReferenceIDGenerator _idGenerator = new ReferenceIDGenerator();

        static INTERNAL_HtmlDomManager()
        {
            if (!IsRunningInJavaScript())
            {
                _store = new Dictionary<string, UIElement>(2048);
            }
        }

#if !CSHTML5NETSTANDARD
#if !BRIDGE
        [JSReplacement("document")]
#else
        [Template("document")]
#endif
        static dynamic GetHtmlDocument()
        {
            // Simulator only:
            return INTERNAL_Simulator.HtmlDocument;
        }
#endif

        public static object GetApplicationRootDomElement()
        {
            var rootDomElement = Interop.ExecuteJavaScriptAsync("document.getXamlRoot()");

            if (rootDomElement == null)
            {
#if OPENSILVER
                const string ROOT_NAME = "opensilver-root";
#elif BRIDGE
                const string ROOT_NAME = "cshtml5-root";
#endif
                throw new Exception("The application root DOM element was not found. To fix this issue, please add a DIV with the ID '" + ROOT_NAME + "' to the HTML page.");
            }

            return rootDomElement;
        }

        public static void RemoveFromDom(object domNode, string commentForDebugging = null)
        {
            if (IsRunningInJavaScript())
            {
                ((dynamic)domNode).parentNode.removeChild(domNode);
            }

            else
            {
                string javaScriptCodeToExecute = $@"
                    var element = document.getElementById(""{((INTERNAL_HtmlDomElementReference)domNode).UniqueIdentifier}"");
                    if (element) element.parentNode.removeChild(element);";
                ExecuteJavaScript(javaScriptCodeToExecute, commentForDebugging); // IMPORTANT: This cannot be replaced by "INTERNAL_SimulatorPerformanceOptimizer.QueueJavaScriptCode" because the element may no longer be in the tree when we try to remove it (cf. issues we had with the Grid on 2015.08.26)
                if (((INTERNAL_HtmlDomElementReference)domNode).Parent != null)
                {
                    ((INTERNAL_HtmlDomElementReference)domNode).Parent.FirstChild = null;
                }
            }
        }

#if CSHTML5NETSTANDARD
        public static INTERNAL_HtmlDomElementReference GetParentDomElement(object domElementRef)
        {
            var parent = ((INTERNAL_HtmlDomElementReference)domElementRef).Parent;
            return parent;
        }
#else
        //[JSReplacement("$domElementRef.parentNode")] // Commented because of a JSIL bug: the attribute is not taken into account: the method is ignored.
#if BRIDGE
        [Template("{domElementRef}.parentNode")]
#endif  
        public static dynamic GetParentDomElement(dynamic domElementRef)
        {
            if (IsRunningInJavaScript())
            {
                var parent = domElementRef.parentNode;
                //if (parent.id != "INTERNAL_RootElement")
                return parent;
                //else
                //return null;
            }
            else
            {
                var parent = ((INTERNAL_HtmlDomElementReference)domElementRef).Parent;
                //if (parent.UniqueIdentifier != "INTERNAL_RootElement")
                return parent;
                //else
                //return null;
            }
        }
#endif

#if CSHTML5NETSTANDARD
        public static INTERNAL_HtmlDomElementReference GetFirstChildDomElement(object domElementRef)
        {
            return ((INTERNAL_HtmlDomElementReference)domElementRef).FirstChild;
        }
#else
        //[JSReplacement("$domElementRef.parentNode")] // Commented because of a JSIL bug: the attribute is not taken into account: the method is ignored.
#if BRIDGE
        [Template("{domElementRef}.firstChild")]
#endif  
        public static dynamic GetFirstChildDomElement(dynamic domElementRef)
        {
            if (IsRunningInJavaScript())
            {
                if (domElementRef.childNodes.length > 0)
                    return domElementRef.childNodes[0];
                else
                    return null;
            }
            else
            {
                return ((INTERNAL_HtmlDomElementReference)domElementRef).FirstChild;
            }
        }
#endif

#if !BRIDGE
        [JSIgnore]
#else
        [External]
#endif
        public static INTERNAL_HtmlDomElementReference GetChildDomElementAt(INTERNAL_HtmlDomElementReference domElementRef, int index)
        {
            int length = Convert.ToInt32(Interop.ExecuteJavaScript("$0.childNodes.length", domElementRef));
            if (index < 0 || length <= index)
            {
                throw new IndexOutOfRangeException();
            }

            string childNodeId = Interop.ExecuteJavaScript(@"var child = $0.childNodes[$1]; child.id;", domElementRef, index).ToString();
            return new INTERNAL_HtmlDomElementReference(childNodeId, domElementRef);
        }


#if CSHTML5NETSTANDARD
        public static object GetHtmlWindow()
#else

#if !BRIDGE
        [JSReplacement("window")]
#else
        [Template("window")]
#endif
        public static dynamic GetHtmlWindow()
#endif
        {
            return Interop.ExecuteJavaScript("window");
        }

        // Note: JSReplacement does not work here (for some unknown reason)
        public static bool IsNotUndefinedOrNull(object obj)
        {
            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                return JSIL.Verbatim.Expression("(typeof $0 !== 'undefined' && $0 !== null)", obj);
#else
                return Script.Write<bool>("(typeof $0 !== 'undefined' && $0 !== null)", obj);
#endif
            }
            else
                return obj != null; //Note: This line does not translate well to JS, so that is why we call the "JSIL.Verbatim" method above. Note that for some unknown reason, "JSReplacement" did not work here.
        }

        public static void SetFocus(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                var domElementRefConcernedByFocus = element.GetFocusTarget();
                if (domElementRefConcernedByFocus == null)
                {
                    return;
                }

#if CSHTML5BLAZOR
                // In the OpenSilver we can never be running in javascript but we may not be in the simulator
                // todo: find a way to use a more generic method (see: IsRunningInTheSimulator)
                if (!Interop.IsRunningInTheSimulator_WorkAround)
#else
                if (IsRunningInJavaScript())
#endif
                {
                    CSHTML5.Interop.ExecuteJavaScriptAsync("setTimeout(function() { $0.focus(); }, 1)", domElementRefConcernedByFocus);
                }
                else
                {
                    SetFocus_SimulatorOnly(domElementRefConcernedByFocus);
                }
            }
        }

#if !BRIDGE
        [JSIgnore]
#else
        [External]
#endif
        static void SetFocus_SimulatorOnly(object domElementRef)
        {
            ExecuteJavaScript($@"var domElement = document.getElementByIdSafe(""{((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier}"");
                                        setTimeout(function() {{ 
                                            domElement.focus();
                                        }}, 1);");
        }

        public static void SetContentString(UIElement element, string content, bool removeTextWrapping = false)
        {
            object domElement = element.GetDomElementToSetContentString();

            //Commented because it does not support Line Breaks:
            //domElement.textContent = content;

            //content = content.Replace("\n", "\r\n");
            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                JSIL.Verbatim.Expression(@"
if ('innerText' in $0 && !window.IE_VERSION)
{
    // --- CHROME & IE: ---
    $0.innerText = $1;
    if ($2)
    {
        $0.style.whiteSpace = 'nowrap';
    }
}
else
{
    // --- FIREFOX: ---
    // First, escape the string so that if it contains some HTML, it is made harmless:
    var tempDiv = document.createElement('div');
    tempDiv.appendChild(document.createTextNode($1));
    var escapedText = tempDiv.innerHTML;
    // Then, replace all line breaks with '<br>' so that FireFox can render them properly:
    var finalHtml = escapedText.replace(/\r\n/g,'<br>');
    finalHtml = finalHtml.replace(/\n/g,'<br>');
    // Finally, display the HTML:
    $0.innerHTML = finalHtml;
    if ($2)
    {
        $0.style.whiteSpace = 'nowrap';
    }
}", domElement, content, removeTextWrapping);
#else
                Script.Write(@"
if ('innerText' in {0} && !window.IE_VERSION)
{
    // --- CHROME & IE: ---
    {0}.innerText = {1};
    if ({2})
    {
        {0}.style.whiteSpace = 'nowrap';
    }
}
else
{
    // --- FIREFOX: ---
    // First, escape the string so that if it contains some HTML, it is made harmless:
    var tempDiv = document.createElement('div');
    tempDiv.appendChild(document.createTextNode({1}));
    var escapedText = tempDiv.innerHTML;
    // Then, replace all line breaks with '<br>' so that FireFox can render them properly:
    var finalHtml = escapedText.replace(/\r\n/g,'<br>');
    finalHtml = finalHtml.replace(/\n/g,'<br>');
    // Finally, display the HTML:
    {0}.innerHTML = finalHtml;
    if ({2})
    {
        {0}.style.whiteSpace = 'nowrap';
    }
}", domElement, content, removeTextWrapping);
#endif
            }
            else
            {
                // Note: this is intended to be called by the simulator only:
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElement).UniqueIdentifier;
                string javaScriptCodeToExecute = $@"document.setContentString(""{ uniqueIdentifier}"",""{EscapeStringForUseInJavaScript(content)}"",{removeTextWrapping.ToString().ToLower()})";
                INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            }

            INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();

            //todo-perfs: in the Simulator, use the WebControl instead, like in the other methods of this class.
        }

        internal static void SetUIElementContentString(UIElement uiElement, string newText)
        {
#if !CSHTML5NETSTANDARD
            if (IsRunningInJavaScript())
            {
                CSHTML5.Interop.ExecuteJavaScript("$0['value'] = newText", uiElement.INTERNAL_InnerDomElement);
            }
            else
            {
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)uiElement.INTERNAL_InnerDomElement).UniqueIdentifier;

                //below: workaround for a bug in Awesomium in which the text was not refreshed inside the <input type="text"/> elements when set programatically. 
                //element.style.visibility=""collapse"";
                //setTimeout(function(){ element.style.visibility=""visible""; }, 10);

                // Note: in the "setTimeout", we must call "document.getElementByIdSafe" otherwise it does not work.
                string javaScriptCodeToExecute = string.Format(@"
var element = document.getElementByIdSafe(""{0}"");
if (element) 
{{
    element.value = ""{1}"";
    element.style.visibility=""collapse"";
    setTimeout(function(){{ var element2 = document.getElementByIdSafe(""{0}""); if (element2) {{ element2.style.visibility=""visible""; }} }}, 0);
}}
                ", uniqueIdentifier, EscapeStringForUseInJavaScript(newText));

                INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            }
#else
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)uiElement.INTERNAL_OuterDomElement).UniqueIdentifier;
            string javaScriptCodeToExecute = $@"
var element = document.getElementByIdSafe(""{uniqueIdentifier}"");
if (element)
{{
element.value = ""{EscapeStringForUseInJavaScript(newText)}"";
element.style.visibility=""collapse"";
setTimeout(function(){{ var element2 = document.getElementByIdSafe(""{uniqueIdentifier}""); if (element2) {{ element2.style.visibility=""visible""; }} }}, 0);
}}";

            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
#endif
        }

        public static string GetTextBoxText(object domElementRef)
        {
#if !CSHTML5NETSTANDARD
            if (IsRunningInJavaScript())
            {
                return GetTextBoxText_JavaScript(domElementRef);
            }
            else
            {
#endif
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
                object domElement = Interop.ExecuteJavaScriptAsync(@"document.getElementByIdSafe($0)", uniqueIdentifier);
                //todo-perfs: replace the code above with a call to the faster "ExecuteJavaScript" method instead of "ExecuteJavaScriptWithResult". To do so, see other methods in this class, or see the class "INTERNAL_HtmlDomStyleReference.cs".

                return Interop.ExecuteJavaScript("getTextAreaInnerText($0)", domElement).ToString();
#if !CSHTML5NETSTANDARD
            }
#endif
        }

        static string GetTextBoxText_JavaScript(object domElementRef)
        {

#if !BRIDGE
            JSIL.Verbatim.Expression(@"
var innerText = getTextAreaInnerText($0);

return innerText;
", domElementRef);
#else
            Script.Write(@"
                var innerText = getTextAreaInnerText({0});

                return innerText;
                ", domElementRef);

#endif
            return "";
        }

        public static object AddOptionToNativeComboBox(
            object nativeComboBoxDomElement,
            string elementToAdd,
            int index)
        {
            var optionDomElement = CSHTML5.Interop.ExecuteJavaScriptAsync(@"
(function(){
    var option = document.createElement(""option"");
    option.text = $1;
    $0.add(option, $2);
    return option;
}())
", nativeComboBoxDomElement, elementToAdd, index);

            return optionDomElement;
        }

        public static object AddOptionToNativeComboBox(object nativeComboBoxDomElement, string elementToAdd)
        {
            // Note: we add an "option" element to the DOM, instead of the string directly, because it works better.
            /*
            var optionDomElement = CSHTML5.Interop.ExecuteJavaScript(@"
function(){
    var option = htmlDocument.createElement(""option"");
    option.text = $1;
    $0.add(option);
    return option;
}()
", domNode, elementToAdd);
*/
            var optionDomElement = CSHTML5.Interop.ExecuteJavaScriptAsync(@"
(function(){
    var option = document.createElement(""option"");
    option.text = $1;
    $0.add(option);
    return option;
}())
", nativeComboBoxDomElement, elementToAdd);
            return optionDomElement;
            /*
            if (IsRunningInJavaScript())
            {
                var option = htmlDocument.createElement("option");
                option.text = elementToAdd;
                domNode.add(option); //add option instead of string, it'll work better
            }
            else
            {
                string javaScriptCodeToExecute = string.Format(@"
var element = document.getElementByIdSafe(""{0}"");
var option = document.createElement(""option"");
    option.text = ""{1}"";
element.add(option);
", ((INTERNAL_HtmlDomElementReference)domNode).UniqueIdentifier, elementToAdd);
                ExecuteJavaScript(javaScriptCodeToExecute);
            }
             */
        }

        /*
        public static void RemoveFromSelectDomElement(dynamic domNode, int indexToRemove) //todo: find a better name for this method (it removes an entry from the native ComboBox (tag "select"))
        {
            if (IsRunningInJavaScript())
            {
                domNode.remove(indexToRemove);
            }
            else
            {
                string javaScriptCodeToExecute = string.Format(@"
var element = document.getElementByIdSafe(""{0}"");
element.remove({1});

", ((INTERNAL_HtmlDomElementReference)domNode).UniqueIdentifier, indexToRemove);
                ExecuteJavaScript(javaScriptCodeToExecute);
            }
        }
         */

        public static void RemoveOptionFromNativeComboBox(object optionToRemove, object nativeComboBoxDomElement)
        {
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.removeChild($1)", nativeComboBoxDomElement, optionToRemove);
        }

        public static void RemoveOptionFromNativeComboBox(object nativeComboBoxDomElement, int index)
        {
            CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0.remove($1)", nativeComboBoxDomElement, index);
        }

        //public static void AppendChild(dynamic parentDomElement, dynamic childDomElement)
        //{
        //    parentDomElement.appendChild(childDomElement);
        //}

#if CSHTML5NETSTANDARD
        public static INTERNAL_HtmlDomStyleReference GetFrameworkElementOuterStyleForModification(UIElement element)
#else
        public static dynamic GetFrameworkElementOuterStyleForModification(UIElement element)
#endif
        {
            return GetDomElementStyleForModification(element.INTERNAL_OuterDomElement);
        }

#if CSHTML5NETSTANDARD
        internal static INTERNAL_HtmlDomStyleReference GetFrameworkElementBoxSizingStyleForModification(UIElement element)
#else
        internal static dynamic GetFrameworkElementBoxSizingStyleForModification(UIElement element)
#endif
        {
            return GetDomElementStyleForModification(element.INTERNAL_AdditionalOutsideDivForMargins);
        }

#if CSHTML5NETSTANDARD
        public static INTERNAL_HtmlDomStyleReference GetDomElementStyleForModification(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Style;
#else
        //[JSReplacement("$domElementRef.style")] // Commented because of a JSIL bug: the attribute is not taken into account: the method is ignored.
        public static dynamic GetDomElementStyleForModification(dynamic domElementRef)
        {
            if (IsRunningInJavaScript())
            {
                return domElementRef.style;
            }
            else
            {
                return ((INTERNAL_HtmlDomElementReference)domElementRef).Style;
            }
        }
#endif

#if CSHTML5NETSTANDARD
        public static INTERNAL_Html2dContextReference Get2dCanvasContext(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Context2d;
#else
        //[JSReplacement("$domElementRef.getContext('2d')")] // Commented because of a JSIL bug: the attribute is not taken into account: the method is ignored.
        public static dynamic Get2dCanvasContext(dynamic domElementRef)
        {
            if (IsRunningInJavaScript())
            {
                return domElementRef.getContext("2d");
            }
            else
            {
                return ((INTERNAL_HtmlDomElementReference)domElementRef).Context2d;
            }
        }
#endif

#if CSHTML5NETSTANDARD
        public static INTERNAL_HtmlDomStyleReference CreateDomElementAppendItAndGetStyle(string domElementTag, object parentRef, UIElement associatedUIElement, out object newElementRef)
#else
        public static dynamic CreateDomElementAppendItAndGetStyle(string domElementTag, object parentRef, UIElement associatedUIElement, out object newElementRef)
#endif
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif
            newElementRef = CreateDomElementAndAppendIt(domElementTag, parentRef, associatedUIElement);

            return GetDomElementStyleForModification(newElementRef);

#if PERFSTAT
            Performance.Counter("CreateDomElementAppendItAndGetStyle", t0);
#endif
        }

        // Note: "forceSimulatorExecuteImmediately" will disable the simulator optimization that consists in deferring the execution of the JavaScript code to a later time so as to group the JavaScript calls into a single call. Disabling deferral can be useful for example in the cases where we may read the value back immediately after setting it.
        public static void SetDomElementProperty(object domElementRef, string attributeName, object attributeValue, bool forceSimulatorExecuteImmediately = false)
        {
            if (IsRunningInJavaScript())
            {
                //domElementRef.selectedIndex = attributeValue;
                ((dynamic)domElementRef)[attributeName] = attributeValue; //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
                //domElementRef.setAttribute(attributeName, attributeValue); //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
            }
            else
            {
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
                string javaScriptCodeToExecute =
                    $@"var element = document.getElementByIdSafe(""{uniqueIdentifier}"");if (element) {{ element[""{attributeName}""] = {ConvertToStringToUseInJavaScriptCode(attributeValue)} }};";

                if (forceSimulatorExecuteImmediately)
                    ExecuteJavaScript(javaScriptCodeToExecute);
                else
                    INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            }
        }

        // Note: "forceSimulatorExecuteImmediately" will disable the simulator optimization that consists in deferring the execution of the JavaScript code to a later time so as to group the JavaScript calls into a single call. Disabling deferral can be useful for example in the cases where we may read the value back immediately after setting it.
        public static void SetDomElementAttribute(object domElementRef, string attributeName, object attributeValue, bool forceSimulatorExecuteImmediately = false)
        {
            if (IsRunningInJavaScript())
            {
                //domElementRef[attributeName] = attributeValue; //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
                ((dynamic)domElementRef).setAttribute(attributeName, attributeValue); //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
            }
            else
            {
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
                string javaScriptCodeToExecute = $@"document.setDomAttribute(""{uniqueIdentifier}"",""{attributeName}"",{ConvertToStringToUseInJavaScriptCode(attributeValue)})";

                if (forceSimulatorExecuteImmediately)
                    ExecuteJavaScript(javaScriptCodeToExecute);
                else
                    INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            }
        }

        // Note: "forceSimulatorExecuteImmediately" will disable the simulator optimization that consists in deferring the execution of the JavaScript code to a later time so as to group the JavaScript calls into a single call. Disabling deferral can be useful for example in the cases where we may read the value back immediately after setting it.
        public static void SetDomElementStyleProperty(object domElementRef, List<string> propertyCSSNames, object attributeValue, bool forceSimulatorExecuteImmediately = false)
        {
            //Note: the following implementation gives the same result as the following commented line but is more efficient. Since the style is often changed, we chose performance over simplicity on the implementation.
            //CSHTML5.Interop.ExecuteJavaScript("$0.style[$1] = $2;", domElementRef, propertyName, attributeValue);


            if (IsRunningInJavaScript())
            {
                foreach (string propertyName in propertyCSSNames)
                {
                    ((dynamic)domElementRef).style[propertyName] = attributeValue;
                }
            }
            else
            {
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
                string javaScriptCodeToExecute;
                var value = ConvertToStringToUseInJavaScriptCode(attributeValue);
                if (propertyCSSNames.Count == 1)
                {
                    javaScriptCodeToExecute = $@"document.setDomStyle(""{uniqueIdentifier}"", ""{propertyCSSNames[0]}"", {value})";
                }
                else
                {
                    string settingProperties = string.Empty;
                    foreach (string propertyName in propertyCSSNames)
                    {
                        settingProperties += $"element.style.{propertyName} = {value};";
                    }
                    javaScriptCodeToExecute =
                        $@"var element = document.getElementById(""{uniqueIdentifier}"");if (element) {{ {settingProperties} }};";
                }

                if (forceSimulatorExecuteImmediately)
                    ExecuteJavaScript(javaScriptCodeToExecute);
                else
                    INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            }
        }

        internal static void SetDomElementStylePropertyUsingVelocity(object domElement, List<string> cssPropertyNames, object cssValue)
        {
            if (cssPropertyNames != null && cssPropertyNames.Count != 0)
            {
                if (domElement != null)
                {
                    //INTERNAL_HtmlDomManager.SetDomElementStyleProperty(cssEquivalent.DomElement, cssEquivalent.Name, cssValue);
                    object newObj = CSHTML5.Interop.ExecuteJavaScriptAsync(@"new Object()");

                    foreach (string csspropertyName in cssPropertyNames)
                    {
                        CSHTML5.Interop.ExecuteJavaScriptAsync(@"$0[$1] = $2;", newObj, csspropertyName, cssValue);
                    }
                    CSHTML5.Interop.ExecuteJavaScriptAsync(@"Velocity($0, $1, {duration:1, queue:false});", domElement, newObj);
                }

            }
            else
            {
                throw new InvalidOperationException("Please set the Name property of the CSSEquivalent class.");
            }
        }


        // Note: "forceSimulatorExecuteImmediately" will disable the simulator optimization that consists in deferring the execution of the JavaScript code to a later time so as to group the JavaScript calls into a single call. Disabling deferral can be useful for example in the cases where we may read the value back immediately after setting it.
        public static void RemoveDomElementAttribute(object domElementRef, string attributeName, bool forceSimulatorExecuteImmediately = false)
        {
            if (IsRunningInJavaScript())
            {
                ((dynamic)domElementRef).removeAttribute(attributeName); //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
            }
            else
            {
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
                string javaScriptCodeToExecute =
                    $@"var element = document.getElementByIdSafe(""{uniqueIdentifier}"");if (element) {{ element.removeAttribute(""{attributeName}"") }};";

                if (forceSimulatorExecuteImmediately)
                    ExecuteJavaScript(javaScriptCodeToExecute);
                else
                    INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            }
        }

        public static object GetDomElementAttribute(object domElementRef, string attributeName)
        {
            return Interop.ExecuteJavaScript("$0[$1]", domElementRef, attributeName);

            //            if (IsRunningInJavaScript())
            //            {
            //                return domElementRef[attributeName]; //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
            //                //return domElementRef.getAttribute(attributeName); //todo-perfs: to improve performance on some browsers, we may want to declare the "INTERNAL_HtmlDomElementReference" as a "DynamicObject" and use the property assignment syntax everywhere in the solution instead of the "setAttribute" syntax. However in this case it may be more difficult to refact the code with the "Find All References" command.
            //            }
            //            else
            //            {
            //                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            //                return ExecuteJavaScriptWithResult(
            //                        string.Format(@"
            //var element = document.getElementByIdSafe(""{0}"");
            //element[""{1}""]", uniqueIdentifier, attributeName));
            //            }
        }

        public static object CallDomMethod(object domElementRef, string methodName, params object[] args)
        {
            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                return Verbatim.Expression("domElementRef[methodName].apply(domElementRef, Array.prototype.slice.call(arguments, 2))");
#else
                return Script.Write<object>("domElementRef[methodName].apply(domElementRef, Array.prototype.slice.call(arguments, 2))");
#endif
            }
            else
            {
                string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
                string parameters = "";
                foreach (object obj in args)
                {
                    parameters += ConvertToStringToUseInJavaScriptCode(obj);
                }

                string javaScriptCodeToExecute =
                    $@"var element = document.getElementByIdSafe(""{uniqueIdentifier}"");if (element) {{ element[""{methodName}""]({parameters}) }};";

                return ExecuteJavaScriptWithResult(javaScriptCodeToExecute);
            }
        }



        /*
        [Obsolete]
        public static dynamic CreateDomElement(string domElementTag)
        {
            if (IsRunningInJavaScript())
            {
                return htmlDocument.createElement(domElementTag);
            }
            else
            {
                string uniqueIdentifier = INTERNAL_HtmlDomUniqueIdentifiers.CreateNew();
                return CastToJsObject_SimulatorOnly(ExecuteJavaScriptWithResult(string.Format(@"
var newElement = document.createElement(""{0}"");
newElement.setAttribute(""id"", ""{1}"");
newElement
", domElementTag, uniqueIdentifier)));
            }
        }
        */

#if PUBLIC_API_THAT_REQUIRES_SUPPORT_OF_DYNAMIC
        internal static INTERNAL_HtmlDomElementReference CreateDomElementAndAppendItToTempLocation_ForUseByPublicAPIOnly_SimulatorOnly(string domElementTag)
        {
            string uniqueIdentifier = INTERNAL_HtmlDomUniqueIdentifiers.CreateNew();
            string javaScriptToExecute = string.Format(@"
var newElement = document.createElement(""{0}"");
newElement.setAttribute(""id"", ""{1}"");
var temporaryParentElement = document.getElementByIdSafe(""INTERNAL_TemporaryStorageForNewlyCreatedElements"");
temporaryParentElement.appendChild(newElement);
", domElementTag, uniqueIdentifier); // Note: we need to append the element to a temporary location, so that we can later find it with the "document.getElementByIdSafe" method.
            INTERNAL_HtmlDomManager.ExecuteJavaScript(javaScriptToExecute);
            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, null);
        }
#endif

        public static object CreateDomElementAndAppendIt(string domElementTag, object parentRef, UIElement associatedUIElement, int index = -1) //associatedUIElement is the UIElement of which the current dom element is a part.
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif

            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                var result = JSIL.Verbatim.Expression(@"
function(){
    var newElement = document.createElement($0);
    newElement.associatedUIElement = $2;
    if({3} < 0 || {3} >= {1}.children.length)
    {
        $1.appendChild(newElement);
    }
    else
    {
        var nextSibling = $1.children[$3];
        $1.insertBefore(newElement,nextSibling);
    }
    return newElement;
}()
", domElementTag, parentRef, associatedUIElement, index);
#else
                var result = Script.Write<object>(@"
(function(){
    var newElement = document.createElement({0});
    newElement.associatedUIElement = {2};
    if({3} < 0 || {3} >= {1}.children.length)
    {
        {1}.appendChild(newElement);
    }
    else
    {
        var nextSibling = {1}.children[{3}];
        {1}.insertBefore(newElement,nextSibling);
    }
    return newElement;
}());
", domElementTag, parentRef, associatedUIElement, index);
#endif

#if PERFSTAT
                Performance.Counter("CreateDomElementAndAppendIt", t0);
#endif
                return result;
            }
            else
            {
#if PERFSTAT
                Performance.Counter("CreateDomElementAndAppendIt", t0);
#endif
                return CreateDomElementAndAppendIt_ForUseByTheSimulator(domElementTag, parentRef, associatedUIElement, index);
            }
        }

        public static object CreateDomElementAndInsertIt(string domElementTag, object parentRef, UIElement associatedUIElement, int insertionIndex, string relativePosition) //associatedUIElement is the UIElement of which the current dom element is a part.
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif

            if (IsRunningInJavaScript())
            {
#if !BRIDGE
                var result = JSIL.Verbatim.Expression(@"
function(){
    var newElement = document.createElement($0);
    newElement.associatedUIElement = $2;
    $1.children[$3].insertAdjacentElement($4, newElement);
    return newElement;
}()
", domElementTag, parentRef, associatedUIElement, insertionIndex, relativePosition);

#else
                var result = Script.Write<object>(@"
(function(){
    var newElement = document.createElement({0});
    newElement.associatedUIElement = {2};
    {1}.children[{3}].insertAdjacentElement({4}, newElement);
    return newElement;
}());
", domElementTag, parentRef, associatedUIElement, insertionIndex, relativePosition);
#endif
#if PERFSTAT
                Performance.Counter("CreateDomElementAndInsertIt", t0);
#endif
                return result;
            }
            else
            {
#if PERFSTAT
                Performance.Counter("CreateDomElementAndInsertIt", t0);
#endif
                return CreateDomElementAndInsertIt_ForUseByTheSimulator(domElementTag, parentRef, associatedUIElement, insertionIndex, relativePosition);
            }
        }

        private static string NewId() => "id" + _idGenerator.NewId().ToString();

#if !BRIDGE
        [JSReplacement("null")]
#else
        [External]
#endif
        static object CreateDomElementAndAppendIt_ForUseByTheSimulator(string domElementTag, object parentRef, UIElement associatedUIElement, int index)
        {
            //------------------
            // This is the WPF version of the DOM element creation, intented to be used only by the Simulator (not by
            // the compiled JavaScript). For performance reasons, in the Simulator we never store a direct reference
            // to DOM elements, instead we only store their Id and we retrieve them in every JS call by using
            // document.getElementByIdSafe().
            //------------------

            string uniqueIdentifier = NewId();

            INTERNAL_HtmlDomElementReference parent = null;
            if (parentRef is INTERNAL_HtmlDomElementReference)
            {
                parent = (INTERNAL_HtmlDomElementReference)parentRef;
                Interop.ExecuteJavaScriptAsync(@"document.createElementSafe($0, $1, $2, $3)", domElementTag, uniqueIdentifier, parent.UniqueIdentifier, index);
            }
            else
            {
                Interop.ExecuteJavaScriptAsync(@"document.createElementSafe($0, $1, $2, $3)", domElementTag, uniqueIdentifier, parentRef, index);
            }
            
            _store.Add(uniqueIdentifier, associatedUIElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent); //todo: when parent is null this breaks for the root control, but the whole logic will be replaced with simple "ExecuteJavaScript" calls in the future, so it will not be a problem.
        }

#if !BRIDGE
        [JSReplacement("null")]
#else
        [External]
#endif
        static object CreateDomElementAndInsertIt_ForUseByTheSimulator(string domElementTag, object parentRef, UIElement associatedUIElement, int insertionIndex, string relativePosition)
        {
            //------------------
            // This is the WPF version of the DOM element creation, intented to be used only by the Simulator (not by
            // the compiled JavaScript). For performance reasons, in the Simulator we never store a direct reference
            // to DOM elements, instead we only store their Id and we retrieve them in every JS call by using
            // document.getElementByIdSafe().
            //------------------

            string uniqueIdentifier = NewId();
            string parentUniqueIdentifier = ((INTERNAL_HtmlDomElementReference)parentRef).UniqueIdentifier;
            string javaScriptToExecute = $@"
var newElement = document.createElement(""{domElementTag}"");
newElement.setAttribute(""id"", ""{uniqueIdentifier}"");
var parentElement = document.getElementByIdSafe(""{parentUniqueIdentifier}"");
    parentElement.children[{insertionIndex}].insertAdjacentElement(""{relativePosition}"", newElement);";

            ExecuteJavaScript(javaScriptToExecute);
            _store.Add(uniqueIdentifier, associatedUIElement);
            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, (INTERNAL_HtmlDomElementReference)parentRef);
        }


        public static object CreateDomFromStringAndAppendIt(string domAsString, object parentRef, UIElement associatedUIElement)
        {
#if !CSHTML5NETSTANDARD
            // Create a temporary parent div to which we can write the innerHTML, then extract the contents: // c.f http://stackoverflow.com/questions/3103962/converting-html-string-into-dom-elements
            if (IsRunningInJavaScript())
            {
                var tempDiv = GetHtmlDocument().createElement("div");
                tempDiv.innerHTML = domAsString;
                var newElement = tempDiv.firstChild;
                newElement.associatedUIElement = associatedUIElement;
                ((dynamic)parentRef).appendChild(newElement);
                return newElement;
                //todo-perfs: check if there is a better solution in terms of performance (while still remaining compatible with all browsers).
            }
            else
            {
#endif
                string uniqueIdentifier = NewId();
                string parentUniqueIdentifier = ((INTERNAL_HtmlDomElementReference)parentRef).UniqueIdentifier;
                // Create a temporary parent div to which we can write the innerHTML, then extract the contents:
                string javaScriptToExecute = $@"
var tempDiv = document.createElement(""div"");
tempDiv.innerHTML = ""{domAsString.Replace('\"', '\'').Replace("\r", "").Replace("\n", "")}"";
var newElement = tempDiv.firstChild;
newElement.setAttribute(""id"", ""{uniqueIdentifier}"");
var parentElement = document.getElementByIdSafe(""{parentUniqueIdentifier}"");
parentElement.appendChild(newElement);";

                ExecuteJavaScript(javaScriptToExecute);
                _store.Add(uniqueIdentifier, associatedUIElement);
                return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, ((INTERNAL_HtmlDomElementReference)parentRef).Parent);
                //todo-perfs: check if there is a better solution in terms of performance (while still remaining compatible with all browsers).
#if !CSHTML5NETSTANDARD
            }
#endif
        }

        internal static void AppendChild_ForUseByPublicAPIOnly_SimulatorOnly(
            INTERNAL_HtmlDomElementReference domElementRef,
            INTERNAL_HtmlDomElementReference parentDomElementRef)
        {
            string childUniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            string parentUniqueIdentifier = ((INTERNAL_HtmlDomElementReference)parentDomElementRef).UniqueIdentifier;
            string javaScriptToExecute = $@"
var child = document.getElementByIdSafe(""{childUniqueIdentifier}"");
var parentElement = document.getElementByIdSafe(""{parentUniqueIdentifier}"");
parentElement.appendChild(child);";

            ExecuteJavaScript(javaScriptToExecute);
            if (_store.TryGetValue(parentUniqueIdentifier, out UIElement parent))
            {
                _store[childUniqueIdentifier] = parent;
            }
        }

#if !BRIDGE
        [JSReplacement("true")]
#else
        [Template("true")]
#endif
        static bool IsRunningInJavaScript()
        {
            return false;
        }

#if !BRIDGE
        [JSReplacement("(window.IE_VERSION)")] // Note: This will return "true" if the variable "IE_VERSION" is defined.
#else
        [Template("(window.IE_VERSION)")] // Note: This will return "true" if the variable "IE_VERSION" is defined.
#endif
        public static bool IsInternetExplorer()
        {
            return false;
        }

#if !BRIDGE
        [JSReplacement("(window.IS_EDGE)")] // Note: This will return "true" if the variable "IE_VERSION" is defined.
#else
        [Template("(window.IS_EDGE)")] // Note: This will return "true" if the variable "IE_VERSION" is defined.
#endif
        public static bool IsEdge()
        {
            return false;
        }


#if !BRIDGE
        [JSReplacement("(window.FIREFOX_VERSION)")] // Note: This will return "true" if the variable "FIREFOX_VERSION" is defined.
#else
        [Template("(window.FIREFOX_VERSION)")] // Note: This will return "true" if the variable "FIREFOX_VERSION" is defined.
#endif
        public static bool IsFirefox()
        {
            return false;
        }

#if !BRIDGE
        [JSReplacement("null")]
#else
        [Template("null")]
#endif
        internal static string EscapeStringForUseInJavaScript(string s)
        {
            // credits: http://stackoverflow.com/questions/1242118/how-to-escape-json-string

            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '`':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

#if !BRIDGE
        [JSReplacement("null")]
#else
        [Template("null")]
#endif
        public static string ConvertToStringToUseInJavaScriptCode(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            else if (obj is string str)
            {
                return @"""" + EscapeStringForUseInJavaScript(str) + @"""";
            }
            else if (obj is bool b)
            {
                return (b ? "true" : "false");
            }
            else if (obj is char c)
            {
                return @"""" + EscapeStringForUseInJavaScript(c.ToString()) + @"""";
            }
            else if (obj is IFormattable formattable)
            {
                return formattable.ToInvariantString();
            }
            else
            {
                return @"""" + EscapeStringForUseInJavaScript(obj.ToString()) + @"""";
            }
        }

#if !BRIDGE
        [JSIgnore]

#else
        [External]
#endif
        public static void ExecuteJavaScript(string javaScriptToExecute, string commentForDebugging = null)
        {
            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(
                javaScriptToExecute,
                INTERNAL_SimulatorExecuteJavaScript.EnableInteropLogging ? "(Called from HtmlDomManager.ExecuteJavaScript)" + (commentForDebugging != null ? commentForDebugging : "") : "" );
        }

#if !BRIDGE
        [JSIgnore]
#else
        [External]
#endif
        public static object ExecuteJavaScriptWithResult(string javaScriptToExecute, string commentForDebugging = null, bool noImpactOnPendingJSCode = false)
        {
            return INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptSync(
                javaScriptToExecute,
                INTERNAL_SimulatorExecuteJavaScript.EnableInteropLogging ? "(Called from HtmlDomManager.ExecuteJavaScriptWithResult)" + (commentForDebugging != null ? commentForDebugging : "") : "", 
                noImpactOnPendingJSCode);
        }

        /// <summary>
        /// Retrieves an object that is located within a specified point of an object's coordinate space.
        /// </summary>
        /// <returns>The UIElement object that is determined to be located
        /// in the visual tree composition at the specified point.</returns>
        public static UIElement FindElementInHostCoordinates_UsedBySimulatorToo(double x, double y) // IMPORTANT: If you rename this method or change its signature, make sure to rename its dynamic call in the Simulator.
        {
            object domElementAtCoordinates = Interop.ExecuteJavaScript(@"
(function(){
    var domElementAtCoordinates = document.elementFromPoint($0, $1);
    if (!domElementAtCoordinates || domElementAtCoordinates === document.documentElement)
    {
        return null;
    }
    else
    {
        return domElementAtCoordinates;
    }
}())
", x, y);

            UIElement result = GetUIElementFromDomElement(domElementAtCoordinates);

            return result;
        }

        internal static IEnumerable<UIElement> FindElementsInHostCoordinates(Point intersectingPoint, UIElement subtree)
        {
            string[] elements;
            if (subtree != null)
            {
                elements = JsonSerializer.Deserialize<string[]>(
                    Convert.ToString(OpenSilver.Interop.ExecuteJavaScript(
                        @"window.elementsFromPointOpensilver($0,$1,$2)",
                        intersectingPoint.X,
                        intersectingPoint.Y,
                        OpenSilver.Interop.GetDiv(subtree))));
            }
            else
            {
                elements = JsonSerializer.Deserialize<string[]>(
                    Convert.ToString(OpenSilver.Interop.ExecuteJavaScript(
                        @"window.elementsFromPointOpensilver($0,$1,null)",
                        intersectingPoint.X,
                        intersectingPoint.Y)));
            }

            for (int i = elements.Length - 1; i >= 0; i--)
            {
                if (_store.TryGetValue(elements[i], out UIElement uie))
                {
                    yield return uie;
                }
            }
        }

        public static UIElement GetUIElementFromDomElement(object domElementRef)
        {
            UIElement result = null;

            while (!IsNullOrUndefined(domElementRef))
            {
                // Walk up the DOM tree until we find a DOM element that has a corresponding CSharp object:

                // Check if element exists in the DOM Tree; strangely, sometimes it doesn't
                if (bool.Parse(Interop.ExecuteJavaScript("$0 == null", domElementRef).ToString()))
                    break;

#if OPENSILVER
                if (true)
#elif BRIDGE
                if (Interop.IsRunningInTheSimulator)
#endif
                {
                    // In the Simulator, we get the CSharp object associated to a DOM element by searching for the DOM element ID in the "INTERNAL_idsToUIElements" dictionary.

                    object jsId = Interop.ExecuteJavaScript("$0.id", domElementRef);
                    if (!IsNullOrUndefined(jsId))
                    {
                        string id = Convert.ToString(jsId);
                        if (_store.TryGetValue(id, out UIElement uie))
                        {
                            result = uie;
                            break;
                        }
                    }
                }
                else
                {
                    // In JavaScript, we get the CSharp object associated to a DOM element by reading the "associatedUIElement" property:

                    object associatedUIElement = Interop.ExecuteJavaScript("$0.associatedUIElement", domElementRef);
                    if (!IsNullOrUndefined(associatedUIElement))
                    {
                        result = (UIElement)associatedUIElement;
                        break;
                    }
                }

                // Move to the parent:
                domElementRef = Interop.ExecuteJavaScript("$0.parentNode", domElementRef);
            }

            return result;
        }

        public static bool IsNullOrUndefined(object jsObject)
        {
            return Interop.IsNull(jsObject) || Interop.IsUndefined(jsObject);
        }

        static HashSet<Type> NumericTypes;

#if !BRIDGE
        [JSIgnore]
#else
        [External]
#endif
        static bool IsNumericType(object obj)
        {
            if (NumericTypes == null)
            {
                NumericTypes = new HashSet<Type>
                    {
                        typeof(Byte),
                        typeof(SByte),
                        typeof(UInt16),
                        typeof(UInt32),
                        typeof(UInt64),
                        typeof(Int16),
                        typeof(Int32),
                        typeof(Int64),
                        typeof(Decimal),
                        typeof(Double),
                        typeof(Single),
                        typeof(Byte?),
                        typeof(SByte?),
                        typeof(UInt16?),
                        typeof(UInt32?),
                        typeof(UInt64?),
                        typeof(Int16?),
                        typeof(Int32?),
                        typeof(Int64?),
                        typeof(Decimal?),
                        typeof(Double?),
                        typeof(Single?),
                    };
            }
            if (obj != null && NumericTypes.Contains(obj.GetType()))
                return true;
            else
                return false;
        }

#if OPENSILVER
        internal static void SetVisualBounds(INTERNAL_HtmlDomStyleReference style, Rect visualBounds, bool bSetPositionAbsolute, bool bSetZeroMargin, bool bSetZeroPadding)
        {
            string left = $"{visualBounds.Left.ToInvariantString("0.##")}";
            string top = $"{visualBounds.Top.ToInvariantString("0.##")}";
            string width = $"{visualBounds.Width.ToInvariantString("0.##")}";
            string height = $"{visualBounds.Height.ToInvariantString("0.##")}";

            string javaScriptCodeToExecute = $@"document.setVisualBounds(""{style.Uid}"",{left},{top},{width},{height},{(bSetPositionAbsolute ? "1": "0")},{(bSetZeroMargin ? "1" : "0")},{(bSetZeroPadding ? "1" : "0")})";

            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
        }
#elif BRIDGE
        internal static void SetVisualBounds(dynamic style, Rect visualBounds, bool bSetPositionAbsolute, bool bSetZeroMargin, bool bSetZeroPadding)
        {
            style.left = visualBounds.Left.ToInvariantString() + "px";
            style.top = visualBounds.Top.ToInvariantString() + "px";
            style.width = visualBounds.Width.ToInvariantString() + "px";
            style.height = visualBounds.Height.ToInvariantString() + "px";
            if (bSetPositionAbsolute)
            {
                style.position = "absolute";
            }
            if (bSetZeroMargin)
            {
                style.margin = "0";
            }
            if (bSetZeroPadding)
            {
                style.padding = "0";
            }
        }
#endif

#if OPENSILVER
        internal static void SetPosition(INTERNAL_HtmlDomStyleReference style, Rect visualBounds, bool bSetPositionAbsolute, bool bSetZeroMargin, bool bSetZeroPadding)
        {
            string left = $"{visualBounds.Left.ToInvariantString("0.##")}";
            string top = $"{visualBounds.Top.ToInvariantString("0.##")}";

            string javaScriptCodeToExecute = $@"document.setPosition(""{style.Uid}"",{left},{top},{(bSetPositionAbsolute ? "1" : "0")},{(bSetZeroMargin ? "1" : "0")},{(bSetZeroPadding ? "1" : "0")})";

            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
        }
#elif BRIDGE
        internal static void SetPosition(dynamic style, Rect visualBounds, bool bSetPositionAbsolute, bool bSetZeroMargin, bool bSetZeroPadding)
        {
            style.left = visualBounds.Left.ToInvariantString() + "px";
            style.top = visualBounds.Top.ToInvariantString() + "px";
            if (bSetPositionAbsolute)
            {
                style.position = "absolute";
            }
            if (bSetZeroMargin)
            {
                style.margin = "0";
            }
            if (bSetZeroPadding)
            {
                style.padding = "0";
            }
        }
#endif
    }
}
