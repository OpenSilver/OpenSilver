
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text;
using OpenSilver.Internal;
using System.Linq;
using System.Diagnostics;
using System.Globalization;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
#endif

namespace CSHTML5.Internal // IMPORTANT: if you change this namespace, make sure to change the dynamic call from the Simulator as well.
{
    public static class INTERNAL_HtmlDomManager // Note: this class is "internal" but still visible to the Simulator because of the "InternalsVisibleTo" flag in "Assembly.cs".
    {
        //------
        // All JavaScript functions (called through dynamic objects) for manipulating the DOM should go here.
        //------
        private static readonly Dictionary<string, WeakReference<UIElement>> _store;
        private static readonly ReferenceIDGenerator _idGenerator = new ReferenceIDGenerator();

        static INTERNAL_HtmlDomManager()
        {
            _store = new Dictionary<string, WeakReference<UIElement>>(2048);
        }

        public static object GetApplicationRootDomElement() => Application.Current?.GetRootDiv();

        internal static UIElement GetElementById(string id)
        {
            if (_store.TryGetValue(id ?? string.Empty, out WeakReference<UIElement> weakRef)
                && weakRef.TryGetTarget(out UIElement uie))
            {
                return uie;
            }

            return null;
        }

        private static void AddToGlobalStore(string uniqueIdentifier, UIElement el)
        {
            _store.Add(uniqueIdentifier, new WeakReference<UIElement>(el));
        }

        internal static void RemoveFromGlobalStore(INTERNAL_HtmlDomElementReference htmlDomElRef)
        {
            if (htmlDomElRef == null)
            {
                return;
            }

            _store.Remove(htmlDomElRef.UniqueIdentifier);
        }

        public static void RemoveFromDom(object domNode, string commentForDebugging = null)
        {
            var htmlDomElRef = (INTERNAL_HtmlDomElementReference)domNode;
            var javaScriptCodeToExecute = $@"
                var element = document.getElementById(""{htmlDomElRef.UniqueIdentifier}"");
                if (element) element.parentNode.removeChild(element);";
            ExecuteJavaScript(javaScriptCodeToExecute, commentForDebugging); // IMPORTANT: This cannot be replaced by "INTERNAL_SimulatorPerformanceOptimizer.QueueJavaScriptCode" because the element may no longer be in the tree when we try to remove it (cf. issues we had with the Grid on 2015.08.26)
            if (htmlDomElRef.Parent != null)
            {
                htmlDomElRef.Parent.FirstChild = null;
            }

            RemoveFromGlobalStore(htmlDomElRef);
        }

        public static INTERNAL_HtmlDomElementReference GetParentDomElement(object domElementRef)
        {
            var parent = ((INTERNAL_HtmlDomElementReference)domElementRef).Parent;
            return parent;
        }

        public static INTERNAL_HtmlDomElementReference GetFirstChildDomElement(object domElementRef)
        {
            return ((INTERNAL_HtmlDomElementReference)domElementRef).FirstChild;
        }

        public static INTERNAL_HtmlDomElementReference GetChildDomElementAt(INTERNAL_HtmlDomElementReference domElementRef, int index)
        {
            string sDomElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
            int length = OpenSilver.Interop.ExecuteJavaScriptInt32($"{sDomElement}.childNodes.length");
            if (index < 0 || length <= index)
            {
                throw new IndexOutOfRangeException();
            }

            string childNodeId = OpenSilver.Interop.ExecuteJavaScriptString($@"var child = {sDomElement}.childNodes[{index.ToInvariantString()}]; child.id;");
            return new INTERNAL_HtmlDomElementReference(childNodeId, domElementRef);
        }

        private static object _window;

        public static object GetHtmlWindow()
        {
            return _window ??= OpenSilver.Interop.ExecuteJavaScript("window");
        }

        public static bool IsNotUndefinedOrNull(object obj)
        {
            return obj != null;
        }

        public static void SetFocus(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                object domElement = element.GetFocusTarget();
                if (domElement != null)
                {
                    string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElement);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"document.setFocus({sElement});");
                }
            }
        }

        public static void SetContentString(UIElement element, string content, bool removeTextWrapping = false)
        {
            object domElement = element.INTERNAL_InnerDomElement;

            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElement).UniqueIdentifier;
            string javaScriptCodeToExecute = $@"document.setContentString(""{ uniqueIdentifier}"",""{EscapeStringForUseInJavaScript(content)}"",{removeTextWrapping.ToString().ToLower()})";
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javaScriptCodeToExecute);

            INTERNAL_WorkaroundIE11IssuesWithScrollViewerInsideGrid.RefreshLayoutIfIE();
        }

        internal static void SetUIElementContentString(UIElement uiElement, string newText)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)uiElement.INTERNAL_OuterDomElement).UniqueIdentifier;
            string javaScriptCodeToExecute = $@"
var element = document.getElementById(""{uniqueIdentifier}"");
if (element)
{{
element.value = ""{EscapeStringForUseInJavaScript(newText)}"";
element.style.visibility=""collapse"";
setTimeout(function(){{ var element2 = document.getElementById(""{uniqueIdentifier}""); if (element2) {{ element2.style.visibility=""visible""; }} }}, 0);
}}";

            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javaScriptCodeToExecute);
        }

        public static string GetTextBoxText(object domElementRef)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
            return OpenSilver.Interop.ExecuteJavaScriptString(
                $"if ({sElement} instanceof HTMLTextAreaElement) {sElement}.value; else {sElement}.innerText;");
        }

        public static object AddOptionToNativeComboBox(
            object nativeComboBoxDomElement,
            string elementToAdd,
            int index)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(nativeComboBoxDomElement);
            var optionDomElement = OpenSilver.Interop.ExecuteJavaScriptAsync($@"
(function(){{
    var option = document.createElement(""option"");
    option.text = ""{EscapeStringForUseInJavaScript(elementToAdd)}"";
    {sElement}.add(option, {index.ToInvariantString()});
    return option;
}}())");

            return optionDomElement;
        }

        public static object AddOptionToNativeComboBox(object nativeComboBoxDomElement, string elementToAdd)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(nativeComboBoxDomElement);
            var optionDomElement = OpenSilver.Interop.ExecuteJavaScriptAsync($@"
(function(){{
    var option = document.createElement(""option"");
    option.text = ""{EscapeStringForUseInJavaScript(elementToAdd)}"";
    {sElement}.add(option);
    return option;
}}())");
            return optionDomElement;
        }

        public static void RemoveOptionFromNativeComboBox(object optionToRemove, object nativeComboBoxDomElement)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(nativeComboBoxDomElement);
            string sToRemove = INTERNAL_InteropImplementation.GetVariableStringForJS(optionToRemove);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sElement}.removeChild({sToRemove})");
        }

        public static void RemoveOptionFromNativeComboBox(object nativeComboBoxDomElement, int index)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(nativeComboBoxDomElement);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sElement}.remove({index.ToInvariantString()})");
        }

        public static INTERNAL_HtmlDomStyleReference GetFrameworkElementOuterStyleForModification(UIElement element)
        {
            return GetDomElementStyleForModification(element.INTERNAL_OuterDomElement);
        }

        public static INTERNAL_HtmlDomStyleReference GetDomElementStyleForModification(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Style;

        public static INTERNAL_Html2dContextReference Get2dCanvasContext(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Context2d;

        public static INTERNAL_HtmlDomStyleReference CreateDomElementAppendItAndGetStyle(
            string domElementTag,
            object parentRef,
            UIElement associatedUIElement,
            out object newElementRef)
        {
            newElementRef = CreateDomElementAndAppendIt(domElementTag, parentRef, associatedUIElement);
            return GetDomElementStyleForModification(newElementRef);
        }

        internal static INTERNAL_HtmlDomStyleReference CreateDomLayoutElementAppendItAndGetStyle(
            string tagName,
            object parentRef,
            UIElement uie,
            out object newElementRef)
        {
            newElementRef = CreateDomLayoutElementAndAppendIt(tagName, parentRef, uie);
            return GetDomElementStyleForModification(newElementRef);
        }

        // Note: "forceSimulatorExecuteImmediately" will disable the simulator optimization that consists in deferring the execution of the JavaScript code to a later time so as to group the JavaScript calls into a single call. Disabling deferral can be useful for example in the cases where we may read the value back immediately after setting it.
        public static void SetDomElementProperty(object domElementRef, string attributeName, object attributeValue, bool forceSimulatorExecuteImmediately = false)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            string javaScriptCodeToExecute =
                $@"var element = document.getElementById(""{uniqueIdentifier}"");if (element) {{ element[""{attributeName}""] = {ConvertToStringToUseInJavaScriptCode(attributeValue)} }};";

            if (forceSimulatorExecuteImmediately)
                ExecuteJavaScript(javaScriptCodeToExecute);
            else
                INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javaScriptCodeToExecute);
        }

        public static void SetDomElementAttribute(object domElementRef, string attributeName, object attributeValue)
            => SetDomElementAttributeImpl(domElementRef, attributeName, ConvertToStringToUseInJavaScriptCode(attributeValue));

        internal static void SetDomElementAttribute(object domElementRef, string attributeName, double value)
            => SetDomElementAttributeImpl(domElementRef, attributeName, value.ToInvariantString());

        internal static void SetDomElementAttribute(object domElementRef, string attributeName, string value, bool escape = false)
            => SetDomElementAttributeImpl(domElementRef, attributeName, $"\"{(escape ? EscapeStringForUseInJavaScript(value) : value)}\"");

        private static void SetDomElementAttributeImpl(object domElementRef, string attributeName, string value)
        {
            string uid = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"document.setDomAttribute(\"{uid}\",\"{attributeName}\",{value})");
        }

        internal static void AddCSSClass(object domElementRef, string className)
        {
            Debug.Assert(domElementRef is not null);

            string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sDiv}.classList.add('{className}');");
        }

        internal static void RemoveCSSClass(object domElementRef, string className)
        {
            Debug.Assert(domElementRef is not null);

            string sDiv = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"{sDiv}.classList.remove('{className}');");
        }

        internal static void SetCSSStyleProperty(object domElementRef, string propertyName, string value)
        {
            Debug.Assert(domElementRef != null);
            string uid = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            OpenSilver.Interop.ExecuteJavaScriptFastAsync($"document.setDomStyle('{uid}', '{propertyName}', '{value}');");
        }

        // Note: "forceSimulatorExecuteImmediately" will disable the simulator optimization that consists in deferring the execution of the JavaScript code to a later time so as to group the JavaScript calls into a single call. Disabling deferral can be useful for example in the cases where we may read the value back immediately after setting it.
        public static void SetDomElementStyleProperty(object domElementRef, List<string> propertyCSSNames, object attributeValue, bool forceSimulatorExecuteImmediately = false)
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
                INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javaScriptCodeToExecute);
        }

        internal static void SetDomElementStylePropertyUsingVelocity(object domElement, List<string> cssPropertyNames, object cssValue)
        {
            if (cssPropertyNames != null && cssPropertyNames.Count != 0)
            {
                if (domElement != null)
                {
                    string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElement);
                    string sCssValue = INTERNAL_InteropImplementation.GetVariableStringForJS(cssValue);
                    OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                        $"document.velocityHelpers.setDomStyle({sElement}, '{string.Join(",", cssPropertyNames)}', {sCssValue});");
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
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            string javaScriptCodeToExecute =
                $@"var element = document.getElementById(""{uniqueIdentifier}"");if (element) {{ element.removeAttribute(""{attributeName}"") }};";

            if (forceSimulatorExecuteImmediately)
                ExecuteJavaScript(javaScriptCodeToExecute);
            else
                INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javaScriptCodeToExecute);
        }

        internal static void RemoveAttribute(object domElement, string attributeName)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElement);
            OpenSilver.Interop.ExecuteJavaScriptVoid($"{sElement}.removeAttribute('{attributeName}');");
        }

        public static object GetDomElementAttribute(object domElementRef, string attributeName)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
            return OpenSilver.Interop.ExecuteJavaScript($@"{sElement}[""{attributeName}""]");
        }

        internal static int GetDomElementAttributeInt32(object domElementRef, string attributeName)
        {
            string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
            return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sElement}['{attributeName}']");
        }

        public static object CallDomMethod(object domElementRef, string methodName, params object[] args)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            string parameters = "";
            foreach (object obj in args)
            {
                parameters += ConvertToStringToUseInJavaScriptCode(obj);
            }

            string javaScriptCodeToExecute =
                $@"var element = document.getElementById(""{uniqueIdentifier}"");if (element) {{ element[""{methodName}""]({parameters}) }};";

            return ExecuteJavaScriptWithResult(javaScriptCodeToExecute);
        }

        public static object CreateDomElementAndAppendIt(
            string domElementTag,
            object parentRef,
            UIElement associatedUIElement,
            int index = -1)
        {
            return CreateDomElementAndAppendIt_ForUseByTheSimulator(domElementTag, parentRef, associatedUIElement, index);
        }

        internal static object CreateDomLayoutElementAndAppendIt(
            string tagName,
            object parentRef,
            UIElement uie,
            int index = -1)
        {
            string uid = NewId();

            INTERNAL_HtmlDomElementReference parent = null;
            if (parentRef is INTERNAL_HtmlDomElementReference domRef)
            {
                parent = domRef;
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.createLayoutElement('{tagName}', '{uid}', '{parent.UniqueIdentifier}', {index.ToInvariantString()})");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.createLayoutElement('{tagName}', '{uid}', {sParentRef}, {index.ToInvariantString()})");
            }

            AddToGlobalStore(uid, uie);

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

        internal static object CreatePopupRootDomElementAndAppendIt(PopupRoot popupRoot)
        {
            Debug.Assert(popupRoot != null);

            string uniqueIdentifier = popupRoot.INTERNAL_UniqueIndentifier;
            string sRootElement = INTERNAL_InteropImplementation.GetVariableStringForJS(
                popupRoot.INTERNAL_ParentWindow.INTERNAL_RootDomElement);
            string sPointerEvents = popupRoot.INTERNAL_LinkedPopup.StayOpen ? "none" : "auto";
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                $"document.createPopupRootElement('{uniqueIdentifier}', {sRootElement}, '{sPointerEvents}');");

            AddToGlobalStore(uniqueIdentifier, popupRoot);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, null);
        }

        internal static object CreateTextBlockDomElementAndAppendIt(
            object parentRef,
            UIElement associatedUIElement,
            bool wrap)
        {
#if PERFSTAT
            Performance.Counter("CreateTextBlockDomElementAndAppendIt", t0);
#endif
            string uniqueIdentifier = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createTextBlockElement(""{uniqueIdentifier}"", ""{parent.UniqueIdentifier}"", {(wrap ? "true" : "false")})");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createTextBlockElement(""{uniqueIdentifier}"", {sParentRef}, {(wrap ? "true" : "false")})");
            }

            AddToGlobalStore(uniqueIdentifier, associatedUIElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static object CreateImageDomElementAndAppendIt(
            object parentRef,
            UIElement associatedUIElement)
        {
#if PERFSTAT
            Performance.Counter("CreateImageDomElementAndAppendIt", t0);
#endif
            string uniqueIdentifier = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createImageElement(""{uniqueIdentifier}"", ""{parent.UniqueIdentifier}"")");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createImageElement(""{uniqueIdentifier}"", {sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, associatedUIElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static object CreateTextElementDomElementAndAppendIt(object parentRef, TextElement textElement)
        {
            string uniqueIdentifier = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.createTextElement('{uniqueIdentifier}', '{textElement.TagName}', '{parent.UniqueIdentifier}');");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.createTextElement('{uniqueIdentifier}', '{textElement.TagName}', {sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, textElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static object CreateShapeOuterDomElementAndAppendIt(
            object parentRef,
            UIElement associatedUIElement)
        {
#if PERFSTAT
            Performance.Counter("CreateShapeOuterDomElementAndAppendIt", t0);
#endif
            string uniqueIdentifier = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createShapeOuterElement(""{uniqueIdentifier}"", ""{parent.UniqueIdentifier}"")");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createShapeOuterElement(""{uniqueIdentifier}"", {sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, associatedUIElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static object CreateShapeInnerDomElementAndAppendIt(
            object parentRef,
            UIElement associatedUIElement)
        {
#if PERFSTAT
            Performance.Counter("CreateShapeInnerDomElementAndAppendIt", t0);
#endif
            string uniqueIdentifier = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createShapeInnerElement(""{uniqueIdentifier}"", ""{parent.UniqueIdentifier}"")");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.createShapeInnerElement(""{uniqueIdentifier}"", {sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, associatedUIElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static INTERNAL_HtmlDomElementReference CreateTextBoxViewDomElementAndAppendIt(
            object parentRef,
            TextBoxView textBoxView)
        {
            Debug.Assert(parentRef is not null);
            Debug.Assert(textBoxView is not null);

            string uid = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent is not null)
            {
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.textboxHelpers.createView('{uid}', '{parent.UniqueIdentifier}');");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $@"document.textboxHelpers.createView('{uid}', {sParentRef})");
            }

            AddToGlobalStore(uid, textBoxView);

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

        public static object CreateDomElementAndInsertIt(string domElementTag, object parentRef, UIElement associatedUIElement, int insertionIndex, string relativePosition) //associatedUIElement is the UIElement of which the current dom element is a part.
        {
#if PERFSTAT
            var t0 = Performance.now();
            Performance.Counter("CreateDomElementAndInsertIt", t0);
#endif
            return CreateDomElementAndInsertIt_ForUseByTheSimulator(domElementTag, parentRef, associatedUIElement, insertionIndex, relativePosition);
        }

        private static string NewId() => "id" + _idGenerator.NewId().ToString();

        private static object CreateDomElementAndAppendIt_ForUseByTheSimulator(
            string tagName,
            object parentRef,
            UIElement uie,
            int index)
        {
            //------------------
            // This is the WPF version of the DOM element creation, intented to be used only by the Simulator (not by
            // the compiled JavaScript). For performance reasons, in the Simulator we never store a direct reference
            // to DOM elements, instead we only store their Id and we retrieve them in every JS call by using
            // document.getElementByIdSafe().
            //------------------

            string uid = NewId();

            INTERNAL_HtmlDomElementReference parent = null;
            if (parentRef is INTERNAL_HtmlDomElementReference domRef)
            {
                parent = domRef;
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.createElementSafe('{tagName}', '{uid}', '{parent.UniqueIdentifier}', {index.ToInvariantString()})");
            }
            else
            {
                string sParentRef = INTERNAL_InteropImplementation.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptFastAsync(
                    $"document.createElementSafe('{tagName}', '{uid}', {sParentRef}, {index.ToInvariantString()})");
            }

            AddToGlobalStore(uid, uie);

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

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
            AddToGlobalStore(uniqueIdentifier, associatedUIElement);
            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, (INTERNAL_HtmlDomElementReference)parentRef);
        }

        public static object CreateDomFromStringAndAppendIt(string domAsString, object parentRef, UIElement associatedUIElement)
        {
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
            AddToGlobalStore(uniqueIdentifier, associatedUIElement);
            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, ((INTERNAL_HtmlDomElementReference)parentRef).Parent);
            //todo-perfs: check if there is a better solution in terms of performance (while still remaining compatible with all browsers).
        }

        internal static void AppendChild_ForUseByPublicAPIOnly_SimulatorOnly(
            INTERNAL_HtmlDomElementReference domElementRef,
            INTERNAL_HtmlDomElementReference parentDomElementRef)
        {
            string childUniqueIdentifier = domElementRef.UniqueIdentifier;
            string parentUniqueIdentifier = parentDomElementRef.UniqueIdentifier;
            string javaScriptToExecute = $@"
var child = document.getElementByIdSafe(""{childUniqueIdentifier}"");
var parentElement = document.getElementByIdSafe(""{parentUniqueIdentifier}"");
parentElement.appendChild(child);";

            ExecuteJavaScript(javaScriptToExecute);
            if (_store.TryGetValue(parentUniqueIdentifier, out var parentWeakRef))
            {
                _store[childUniqueIdentifier] = parentWeakRef;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage)]
        public static bool IsInternetExplorer()
        {
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage)]
        public static bool IsEdge()
        {
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage)]
        public static bool IsFirefox()
        {
            return false;
        }

        internal static string EscapeStringForUseInJavaScript(string s)
        {
            // credits: http://stackoverflow.com/questions/1242118/how-to-escape-json-string

            if (s == null || s.Length == 0)
            {
                return string.Empty;
            }

            int i;
            int len = s.Length;
            StringBuilder sb = StringBuilderFactory.Get();
            string t;

            for (i = 0; i < len; i += 1)
            {
                char c = s[i];
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
                            sb.Append($"\\u{(int)c:x4}");
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            string ret = sb.ToString();
            StringBuilderFactory.Return(sb);

            return ret;
        }

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

        private static void ExecuteJavaScript(string javaScriptToExecute, string commentForDebugging = null)
        {
            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(
                javaScriptToExecute,
                INTERNAL_ExecuteJavaScript.EnableInteropLogging ? "(Called from HtmlDomManager.ExecuteJavaScript)" + (commentForDebugging != null ? commentForDebugging : "") : "" );
        }

        private static object ExecuteJavaScriptWithResult(string javaScriptToExecute, string commentForDebugging = null, bool hasImpactOnPendingJSCode = true)
        {
            var referenceId = 0;
            var wantsResult = true;
            return INTERNAL_ExecuteJavaScript.ExecuteJavaScriptSync(
                javaScriptToExecute, referenceId, wantsResult,
                INTERNAL_ExecuteJavaScript.EnableInteropLogging ? "(Called from HtmlDomManager.ExecuteJavaScriptWithResult)" + (commentForDebugging != null ? commentForDebugging : "") : "", 
                hasImpactOnPendingJSCode);
        }

        /// <summary>
        /// Retrieves an object that is located within a specified point of an object's coordinate space.
        /// </summary>
        /// <returns>The UIElement object that is determined to be located
        /// in the visual tree composition at the specified point.</returns>
        public static UIElement FindElementInHostCoordinates_UsedBySimulatorToo(double x, double y) // IMPORTANT: If you rename this method or change its signature, make sure to rename its dynamic call in the Simulator.
        {
            using (var domElementAtCoordinates = OpenSilver.Interop.ExecuteJavaScript($@"
(function(){{
    var domElementAtCoordinates = document.elementFromPoint({x.ToInvariantString()}, {y.ToInvariantString()});
    if (!domElementAtCoordinates || domElementAtCoordinates === document.documentElement)
    {{
        return null;
    }}
    else
    {{
        return domElementAtCoordinates;
    }}
}}())"))
                return GetUIElementFromDomElement(domElementAtCoordinates);

        }

        public static UIElement GetUIElementFromDomElement(object domElementRef)
        {
            return GetUIElementFromDomElement_UsedBySimulatorToo(domElementRef);
        }

        internal static UIElement GetUIElementFromDomElement_UsedBySimulatorToo(object domElementRef)
        {
            UIElement result = null;

            while (!IsNullOrUndefined(domElementRef))
            {
                string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domElementRef);
                // Walk up the DOM tree until we find a DOM element that has a corresponding CSharp object:

                // Check if element exists in the DOM Tree; strangely, sometimes it doesn't
                if (bool.Parse(OpenSilver.Interop.ExecuteJavaScript("$0 == null", domElementRef).ToString()))
                    break;

                // In the Simulator, we get the CSharp object associated to a DOM element by searching for the DOM element ID in the "INTERNAL_idsToUIElements" dictionary.

                using (var jsId = OpenSilver.Interop.ExecuteJavaScript($"{sElement}.id"))
                {
                    if (!IsNullOrUndefined(jsId))
                    {
                        string id = Convert.ToString(jsId);
                        if (_store.TryGetValue(id, out var elemWeakRef))
                        {
                            if (elemWeakRef.TryGetTarget(out var uie))
                            {
                                result = uie;
                            }
                            else
                            {
                                _store.Remove(id);
                            }
                            break;
                        }
                    }
                }

                // Move to the parent:
                domElementRef = OpenSilver.Interop.ExecuteJavaScript($"{sElement}.parentNode");
            }

            return result;
        }

        public static bool IsNullOrUndefined(object jsObject)
        {
            return OpenSilver.Interop.IsNull(jsObject) || OpenSilver.Interop.IsUndefined(jsObject);
        }

        internal static void SetVisualBounds(INTERNAL_HtmlDomStyleReference style, Point offset, Size size, Rect? clip)
        {
            string left = Math.Round(offset.X, 2).ToInvariantString();
            string top = Math.Round(offset.Y, 2).ToInvariantString();
            string width = Math.Round(size.Width, 2).ToInvariantString();
            string height = Math.Round(size.Height, 2).ToInvariantString();
            
            string javaScriptCodeToExecute;
            
            if (clip.HasValue)
            {
                Rect clipRect = clip.Value;
                string clipLeft = Math.Round(clipRect.Left, 2).ToInvariantString();
                string clipTop = Math.Round(clipRect.Top, 2).ToInvariantString();
                string clipWidth = Math.Round(clipRect.Width, 2).ToInvariantString();
                string clipHeight = Math.Round(clipRect.Height, 2).ToInvariantString();

                javaScriptCodeToExecute =
                    $"document.setVisualBounds('{style.Uid}',{left},{top},{width},{height},true,{clipLeft},{clipTop},{clipWidth},{clipHeight});";
            }
            else
            {
                javaScriptCodeToExecute =
                    $"document.setVisualBounds('{style.Uid}',{left},{top},{width},{height},false);";
            }

            INTERNAL_ExecuteJavaScript.QueueExecuteJavaScript(javaScriptCodeToExecute);
        }

        internal static Size GetBoundingClientSize(object domRef)
        {
            if (domRef is not null)
            {
                string sElement = INTERNAL_InteropImplementation.GetVariableStringForJS(domRef);
                string concatenated = OpenSilver.Interop.ExecuteJavaScriptString(
                    $"(function() {{ var v = {sElement}.getBoundingClientRect(); return v.width.toFixed(3) + '|' + v.height.toFixed(3) }})()");
                int sepIndex = concatenated != null ? concatenated.IndexOf('|') : -1;
                if (sepIndex > -1)
                {
                    string widthStr = concatenated.Substring(0, sepIndex);
                    string heightStr = concatenated.Substring(sepIndex + 1);
                    if (double.TryParse(widthStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double width)
                        && double.TryParse(heightStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double height))
                    {
                        return new Size(width, height);
                    }
                }
            }

            return new Size();
        }
    }
}
