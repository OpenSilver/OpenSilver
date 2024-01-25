
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
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Shapes;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using HtmlPresenter = CSHTML5.Native.Html.Controls.HtmlPresenter;

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

        internal static bool SyncRenderingWithLayout { get; set; } = false;

        [EditorBrowsable(EditorBrowsableState.Never)]
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RemoveFromDom(object domNode) => RemoveFromDom((INTERNAL_HtmlDomElementReference)domNode);

        internal static void RemoveFromDom(INTERNAL_HtmlDomElementReference htmlDomElRef)
        {
            if (SyncRenderingWithLayout)
            {
                LayoutManager.Current.UIRenderer.RemoveRootComponent(htmlDomElRef);
            }
            else
            {
                RemoveNodeNative(htmlDomElRef);
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (htmlDomElRef.Parent != null)
            {
                htmlDomElRef.Parent.FirstChild = null;
            }
#pragma warning restore CS0618 // Type or member is obsolete

            RemoveFromGlobalStore(htmlDomElRef);
        }

        internal static void RemoveNodeNative(INTERNAL_HtmlDomElementReference domNode)
        {
            string sDiv = OpenSilver.Interop.GetVariableStringForJS(domNode);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.remove();");
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomElementReference GetParentDomElement(object domElementRef)
        {
            var parent = ((INTERNAL_HtmlDomElementReference)domElementRef).Parent;
            return parent;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomElementReference GetFirstChildDomElement(object domElementRef)
        {
            return ((INTERNAL_HtmlDomElementReference)domElementRef).FirstChild;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomElementReference GetChildDomElementAt(INTERNAL_HtmlDomElementReference domElementRef, int index)
        {
            string sDomElement = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            int length = OpenSilver.Interop.ExecuteJavaScriptInt32($"{sDomElement}.childNodes.length");
            if (index < 0 || length <= index)
            {
                throw new IndexOutOfRangeException();
            }

            string childNodeId = OpenSilver.Interop.ExecuteJavaScriptString($@"var child = {sDomElement}.childNodes[{index.ToInvariantString()}]; child.id;");
            return new INTERNAL_HtmlDomElementReference(childNodeId, domElementRef);
        }

        private static object _window;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetHtmlWindow()
        {
            return _window ??= OpenSilver.Interop.ExecuteJavaScript("window");
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsNotUndefinedOrNull(object obj)
        {
            return obj != null;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetFocus(UIElement element)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(element))
            {
                if (element.GetFocusTarget() is INTERNAL_HtmlDomElementReference domElement)
                {
                    string sElement = OpenSilver.Interop.GetVariableStringForJS(domElement);
                    OpenSilver.Interop.ExecuteJavaScriptVoid($"document.setFocus({sElement});");
                }
            }
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetContentString(UIElement element, string content, bool removeTextWrapping = false)
        {
            string javaScriptCodeToExecute = $@"document.setContentString(""{element.InnerDiv.UniqueIdentifier}"",""{EscapeStringForUseInJavaScript(content)}"",{removeTextWrapping.ToString().ToLower()})";
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptCodeToExecute);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetTextBoxText(object domElementRef)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            return OpenSilver.Interop.ExecuteJavaScriptString(
                $"if ({sElement} instanceof HTMLTextAreaElement) {sElement}.value; else {sElement}.innerText;");
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object AddOptionToNativeComboBox(
            object nativeComboBoxDomElement,
            string elementToAdd,
            int index)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(nativeComboBoxDomElement);
            var optionDomElement = OpenSilver.Interop.ExecuteJavaScriptAsync($@"
(function(){{
    var option = document.createElement(""option"");
    option.text = ""{EscapeStringForUseInJavaScript(elementToAdd)}"";
    {sElement}.add(option, {index.ToInvariantString()});
    return option;
}}())");

            return optionDomElement;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object AddOptionToNativeComboBox(object nativeComboBoxDomElement, string elementToAdd)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(nativeComboBoxDomElement);
            var optionDomElement = OpenSilver.Interop.ExecuteJavaScriptAsync($@"
(function(){{
    var option = document.createElement(""option"");
    option.text = ""{EscapeStringForUseInJavaScript(elementToAdd)}"";
    {sElement}.add(option);
    return option;
}}())");
            return optionDomElement;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RemoveOptionFromNativeComboBox(object optionToRemove, object nativeComboBoxDomElement)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(nativeComboBoxDomElement);
            string sToRemove = OpenSilver.Interop.GetVariableStringForJS(optionToRemove);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.removeChild({sToRemove})");
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RemoveOptionFromNativeComboBox(object nativeComboBoxDomElement, int index)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(nativeComboBoxDomElement);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.remove({index.ToInvariantString()})");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomStyleReference GetFrameworkElementOuterStyleForModification(UIElement element) =>
            element.OuterDiv.Style;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomStyleReference GetDomElementStyleForModification(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Style;

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_Html2dContextReference Get2dCanvasContext(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Context2d;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomStyleReference CreateDomElementAppendItAndGetStyle(
            string domElementTag,
            object parentRef,
            UIElement associatedUIElement,
            out object newElementRef)
        {
            var element = AppendDomElement(domElementTag, parentRef, associatedUIElement);
            newElementRef = element;
            return element.Style;
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetDomElementProperty(object domElementRef,
            string attributeName,
            object attributeValue,
            bool forceSimulatorExecuteImmediately = false) 
            => SetDomElementProperty((INTERNAL_HtmlDomElementReference)domElementRef,
                attributeName,
                attributeValue);

        internal static void SetDomElementProperty(INTERNAL_HtmlDomElementReference domElementRef,
            string attributeName,
            object attributeValue)
        {
            string uniqueIdentifier = domElementRef.UniqueIdentifier;
            string javaScriptCodeToExecute =
                $@"var element = document.getElementById(""{uniqueIdentifier}"");if (element) {{ element[""{attributeName}""] = {ConvertToStringToUseInJavaScriptCode(attributeValue)} }};";

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptCodeToExecute);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetDomElementAttribute(object domElementRef, string attributeName, object attributeValue)
            => SetDomElementAttribute(
                (INTERNAL_HtmlDomElementReference)domElementRef,
                attributeName,
                ConvertToStringToUseInJavaScriptCode(attributeValue));

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, object attributeValue)
            => SetDomElementAttributeImpl(domElementRef, attributeName, ConvertToStringToUseInJavaScriptCode(attributeValue));

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, double value)
            => SetDomElementAttributeImpl(domElementRef, attributeName, value.ToInvariantString());

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, bool value)
            => SetDomElementAttributeImpl(domElementRef, attributeName, value ? "true" : "false");

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, string value, bool escape = false)
            => SetDomElementAttributeImpl(domElementRef, attributeName, $"\"{(escape ? EscapeStringForUseInJavaScript(value) : value)}\"");

        private static void SetDomElementAttributeImpl(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, string value)
        {
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.setDomAttribute(\"{domElementRef.UniqueIdentifier}\",\"{attributeName}\",{value})");
        }

        internal static void AddCSSClass(INTERNAL_HtmlDomElementReference domElementRef, string className)
        {
            Debug.Assert(domElementRef is not null);

            string sDiv = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.classList.add('{className}');");
        }

        internal static void RemoveCSSClass(INTERNAL_HtmlDomElementReference domElementRef, string className)
        {
            Debug.Assert(domElementRef is not null);

            string sDiv = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.classList.remove('{className}');");
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptCodeToExecute);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void RemoveDomElementAttribute(object domElementRef, string attributeName, bool forceSimulatorExecuteImmediately = false)
        {
            string uniqueIdentifier = ((INTERNAL_HtmlDomElementReference)domElementRef).UniqueIdentifier;
            string javaScriptCodeToExecute =
                $@"var element = document.getElementById(""{uniqueIdentifier}"");if (element) {{ element.removeAttribute(""{attributeName}"") }};";

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptCodeToExecute);
        }

        internal static void RemoveAttribute(INTERNAL_HtmlDomElementReference domElement, string attributeName)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(domElement);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sElement}.removeAttribute('{attributeName}');");
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetDomElementAttribute(object domElementRef, string attributeName)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            return OpenSilver.Interop.ExecuteJavaScript($@"{sElement}[""{attributeName}""]");
        }

        internal static int GetDomElementAttributeInt32(INTERNAL_HtmlDomElementReference domElementRef, string attributeName)
        {
            string sElement = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            return OpenSilver.Interop.ExecuteJavaScriptInt32($"{sElement}['{attributeName}']");
        }

        [Obsolete(Helper.ObsoleteMemberMessage, true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object CallDomMethod(object domElementRef, string methodName, params object[] args)
        {
            throw new NotSupportedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object CreateDomElementAndAppendIt(
            string domElementTag,
            object parentRef,
            UIElement associatedUIElement,
            int index = -1)
        {
            return AppendDomElement(domElementTag, parentRef, associatedUIElement, index);
        }

        internal static INTERNAL_HtmlDomElementReference AppendDomElement(
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
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createElementSafe('{tagName}', '{uid}', '{parent.UniqueIdentifier}', {index.ToInvariantString()})");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createElementSafe('{tagName}', '{uid}', {sParentRef}, {index.ToInvariantString()})");
            }

            AddToGlobalStore(uid, uie);

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

        internal static INTERNAL_HtmlDomElementReference CreateDomLayoutElementAndAppendIt(
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
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createLayoutElement('{tagName}', '{uid}', '{parent.UniqueIdentifier}', {index.ToInvariantString()})");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createLayoutElement('{tagName}', '{uid}', {sParentRef}, {index.ToInvariantString()})");
            }

            AddToGlobalStore(uid, uie);

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

        internal static INTERNAL_HtmlDomElementReference CreatePopupRootDomElementAndAppendIt(PopupRoot popupRoot)
        {
            Debug.Assert(popupRoot != null);

            string uniqueIdentifier = popupRoot.UniqueIndentifier;
            string sRootElement = OpenSilver.Interop.GetVariableStringForJS(
                popupRoot.ParentWindow.RootDomElement);
            string sPointerEvents = popupRoot.ParentPopup.StayOpen ? "none" : "auto";
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.createPopupRootElement('{uniqueIdentifier}', {sRootElement}, '{sPointerEvents}');");

            AddToGlobalStore(uniqueIdentifier, popupRoot);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, null);
        }

        internal static INTERNAL_HtmlDomElementReference CreateTextBlockDomElementAndAppendIt(
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
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $@"document.createTextBlockElement(""{uniqueIdentifier}"", ""{parent.UniqueIdentifier}"", {(wrap ? "true" : "false")})");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $@"document.createTextBlockElement(""{uniqueIdentifier}"", {sParentRef}, {(wrap ? "true" : "false")})");
            }

            AddToGlobalStore(uniqueIdentifier, associatedUIElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static INTERNAL_HtmlDomElementReference CreateImageDomElementAndAppendIt(
            INTERNAL_HtmlDomElementReference parent,
            Image image)
        {
#if PERFSTAT
            Performance.Counter("CreateImageDomElementAndAppendIt", t0);
#endif
            Debug.Assert(parent is not null);
            Debug.Assert(image is not null);

            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $@"document.createImageElement(""{uniqueIdentifier}"", ""{parent.UniqueIdentifier}"")");

            AddToGlobalStore(uniqueIdentifier, image);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static INTERNAL_HtmlDomElementReference CreateTextElementDomElementAndAppendIt(object parentRef, TextElement textElement)
        {
            string uniqueIdentifier = NewId();

            var parent = parentRef as INTERNAL_HtmlDomElementReference;
            if (parent != null)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createTextElement('{uniqueIdentifier}', '{textElement.TagName}', '{parent.UniqueIdentifier}');");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createTextElement('{uniqueIdentifier}', '{textElement.TagName}', {sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, textElement);

            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, parent);
        }

        internal static (INTERNAL_HtmlDomElementReference SvgElement, INTERNAL_HtmlDomElementReference SvgShape, INTERNAL_HtmlDomElementReference SvgDefs)
            CreateShapeElementAndAppendIt(INTERNAL_HtmlDomElementReference parent, Shape shape)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(shape is not null);

            string svgUid = NewId();
            string shapeUid = NewId();
            string defsUid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.createShapeElement('{svgUid}', '{shapeUid}', '{defsUid}', '{shape.SvgTagName}', '{parent.UniqueIdentifier}');");

            AddToGlobalStore(svgUid, shape);
            AddToGlobalStore(shapeUid, shape);

            var svg = new INTERNAL_HtmlDomElementReference(svgUid, parent);
            var svgShape = new INTERNAL_HtmlDomElementReference(shapeUid, svg);
            var svgDefs = new INTERNAL_HtmlDomElementReference(defsUid, svg);

            return (svg, svgShape, svgDefs);
        }

        internal static INTERNAL_HtmlDomElementReference CreateSvgElementAndAppendIt(
            INTERNAL_HtmlDomElementReference parent,
            string tagName)
        {
            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.createSvgElement('{uid}', '{tagName}', '{parent.UniqueIdentifier}');");

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

        internal static (INTERNAL_HtmlDomElementReference PresenterElement, INTERNAL_HtmlDomElementReference ContentElement)
            CreateHtmlPresenterElementAndAppendIt(INTERNAL_HtmlDomElementReference parent, HtmlPresenter htmlPresenter)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(htmlPresenter is not null);

            string id = NewId();
            string contentId = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.htmlPresenterHelpers.createView('{id}', '{contentId}', '{parent.UniqueIdentifier}');");

            AddToGlobalStore(id, htmlPresenter);

            var presenter = new INTERNAL_HtmlDomElementReference(id, parent);
            var content = new INTERNAL_HtmlDomElementReference(contentId, presenter);

            return (presenter, content);
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
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.textboxHelpers.createView('{uid}', '{parent.UniqueIdentifier}');");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $@"document.textboxHelpers.createView('{uid}', {sParentRef})");
            }

            AddToGlobalStore(uid, textBoxView);

            return new INTERNAL_HtmlDomElementReference(uid, parent);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object CreateDomElementAndInsertIt(string domElementTag, object parentRef, UIElement associatedUIElement, int insertionIndex, string relativePosition) //associatedUIElement is the UIElement of which the current dom element is a part.
        {
#if PERFSTAT
            var t0 = Performance.now();
            Performance.Counter("CreateDomElementAndInsertIt", t0);
#endif
            return CreateDomElementAndInsertIt_ForUseByTheSimulator(domElementTag, parentRef, associatedUIElement, insertionIndex, relativePosition);
        }

        private static string NewId() => "id" + _idGenerator.NewId().ToString();

        private static INTERNAL_HtmlDomElementReference CreateDomElementAndInsertIt_ForUseByTheSimulator(string domElementTag,
            object parentRef,
            UIElement associatedUIElement,
            int insertionIndex,
            string relativePosition)
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

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptToExecute);
            AddToGlobalStore(uniqueIdentifier, associatedUIElement);
            return new INTERNAL_HtmlDomElementReference(uniqueIdentifier, (INTERNAL_HtmlDomElementReference)parentRef);
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
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

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptToExecute);
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

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptToExecute);
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
                throw new ArgumentNullException(nameof(obj));
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

        /// <summary>
        /// Retrieves an object that is located within a specified point of an object's coordinate space.
        /// </summary>
        /// <returns>The UIElement object that is determined to be located
        /// in the visual tree composition at the specified point.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
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
            {
                return GetUIElementFromDomElement(domElementAtCoordinates);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static UIElement GetUIElementFromDomElement(object domElementRef)
        {
            return GetUIElementFromDomElement_UsedBySimulatorToo(domElementRef);
        }

        internal static UIElement GetUIElementFromDomElement_UsedBySimulatorToo(object domElementRef)
        {
            UIElement result = null;

            while (!IsNullOrUndefined(domElementRef))
            {
                string sElement = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
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

        [EditorBrowsable(EditorBrowsableState.Never)]
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
                string clipRight = Math.Round(clipRect.Right, 2).ToInvariantString();
                string clipBottom = Math.Round(clipRect.Bottom, 2).ToInvariantString();

                javaScriptCodeToExecute =
                    $"document.setVisualBounds('{style.Uid}',{left},{top},{width},{height},true,{clipLeft},{clipTop},{clipRight},{clipBottom});";
            }
            else
            {
                javaScriptCodeToExecute =
                    $"document.setVisualBounds('{style.Uid}',{left},{top},{width},{height},false);";
            }

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(javaScriptCodeToExecute);
        }

        internal static Size GetBoundingClientSize(INTERNAL_HtmlDomElementReference domRef)
        {
            if (domRef is not null)
            {
                string sElement = OpenSilver.Interop.GetVariableStringForJS(domRef);
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
