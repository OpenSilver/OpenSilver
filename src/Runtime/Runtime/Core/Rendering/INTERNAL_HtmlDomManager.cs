
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
using System.Text;
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

        [Obsolete(Helper.ObsoleteMemberMessage)]
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

            RemoveFromGlobalStore(htmlDomElRef);
        }

        internal static void RemoveNodeNative(INTERNAL_HtmlDomElementReference element) =>
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"document.detachView('{element.UniqueIdentifier}')");

        private static object _window;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetHtmlWindow()
        {
            return _window ??= OpenSilver.Interop.ExecuteJavaScript("window");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomStyleReference GetFrameworkElementOuterStyleForModification(UIElement element)
            => element.OuterDiv.Style;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INTERNAL_HtmlDomStyleReference GetDomElementStyleForModification(object domElementRef)
            => ((INTERNAL_HtmlDomElementReference)domElementRef).Style;

        [Obsolete(Helper.ObsoleteMemberMessage)]
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

        internal static void SetDomElementProperty(INTERNAL_HtmlDomElementReference element, string propertyName, double value)
            => SetDomElementPropertyImpl(element, propertyName, value.ToInvariantString());

        internal static void SetDomElementProperty(INTERNAL_HtmlDomElementReference element, string propertyName, string value, bool escape = false)
            => SetDomElementPropertyImpl(element, propertyName, $"\"{(escape ? EscapeStringForUseInJavaScript(value) : value)}\"");

        private static void SetDomElementPropertyImpl(INTERNAL_HtmlDomElementReference element, string propertyName, string value)
            => OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.setProp('{element.UniqueIdentifier}','{propertyName}',{value})");

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, double value)
            => SetDomElementAttributeImpl(domElementRef, attributeName, value.ToInvariantString());

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, int value)
            => SetDomElementAttributeImpl(domElementRef, attributeName, value.ToInvariantString());

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, bool value)
            => SetDomElementAttributeImpl(domElementRef, attributeName, value ? "true" : "false");

        internal static void SetDomElementAttribute(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, string value, bool escape = false)
            => SetDomElementAttributeImpl(domElementRef, attributeName, $"\"{(escape ? EscapeStringForUseInJavaScript(value) : value)}\"");

        private static void SetDomElementAttributeImpl(INTERNAL_HtmlDomElementReference domElementRef, string attributeName, string value)
            => OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.setAttr('{domElementRef.UniqueIdentifier}','{attributeName}',{value})");

        internal static void AddCSSClass(INTERNAL_HtmlDomElementReference domElementRef, string className)
        {
            Debug.Assert(domElementRef is not null);

            string sDiv = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.classList.add('{className}')");
        }

        internal static void RemoveCSSClass(INTERNAL_HtmlDomElementReference domElementRef, string className)
        {
            Debug.Assert(domElementRef is not null);

            string sDiv = OpenSilver.Interop.GetVariableStringForJS(domElementRef);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"{sDiv}.classList.remove('{className}')");
        }

        internal static void SetVisible(INTERNAL_HtmlDomElementReference element, bool visible)
        {
            Debug.Assert(element is not null);

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.setVisible('{element.UniqueIdentifier}',{(visible ? "true" : "false")})");
        }

        internal static void RemoveAttribute(INTERNAL_HtmlDomElementReference element, string attributeName) =>
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.unsetAttr('{element.UniqueIdentifier}','{attributeName}')");

        [Obsolete(Helper.ObsoleteMemberMessage)]
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

            if (parentRef is INTERNAL_HtmlDomElementReference parent)
            {
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

            return new(uid);
        }

        internal static INTERNAL_HtmlDomElementReference CreateDomLayoutElementAndAppendIt(
            string tagName, object parentRef, UIElement uie, bool isKeyboardFocusable)
        {
            string uid = NewId();

            if (parentRef is INTERNAL_HtmlDomElementReference parent)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createLayout('{tagName}','{uid}','{parent.UniqueIdentifier}'{(isKeyboardFocusable ? ",true" : string.Empty)})");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createLayout('{tagName}','{uid}',{sParentRef}{(isKeyboardFocusable ? ",true" : string.Empty)})");
            }

            AddToGlobalStore(uid, uie);

            return new(uid);
        }

        internal static INTERNAL_HtmlDomElementReference CreatePopupRootDomElementAndAppendIt(PopupRoot popupRoot)
        {
            Debug.Assert(popupRoot != null);

            string uid = NewId();

            string sPointerEvents = popupRoot.Popup.StayOpen ? "none" : "auto";
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.createPopupRoot('{uid}','{popupRoot.ParentWindow.RootDomElement.UniqueIdentifier}','{sPointerEvents}')");

            AddToGlobalStore(uid, popupRoot);

            return new(uid);
        }

        internal static INTERNAL_HtmlDomElementReference CreateTextBlockDomElementAndAppendIt(object parentRef, UIElement associatedUIElement)
        {
#if PERFSTAT
            Performance.Counter("CreateTextBlockDomElementAndAppendIt", t0);
#endif
            string uniqueIdentifier = NewId();

            if (parentRef is INTERNAL_HtmlDomElementReference parent)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createTextBlock('{uniqueIdentifier}','{parent.UniqueIdentifier}')");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $@"document.createTextBlock('{uniqueIdentifier}',{sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, associatedUIElement);

            return new(uniqueIdentifier);
        }

        internal static (INTERNAL_HtmlDomElementReference OuterDiv, INTERNAL_HtmlDomElementReference Image) CreateImageDomElementAndAppendIt(
            INTERNAL_HtmlDomElementReference parent, Image image)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(image is not null);

            string uid = NewId();
            string imgUid = NewId();

            ImageManager.Instance.CreateImage(uid, imgUid, parent.UniqueIdentifier);

            AddToGlobalStore(uid, image);
            AddToGlobalStore(imgUid, image);

            return (new(uid), new(imgUid));
        }

        internal static (INTERNAL_HtmlDomElementReference OuterDiv, INTERNAL_HtmlDomElementReference Canvas) CreateInkPresenterDomElementAndAppendIt(
            object parentRef, InkPresenter inkPresenter)
        {
            Debug.Assert(parentRef is not null);
            Debug.Assert(inkPresenter is not null);

            string uid = NewId();
            string canvasUid = NewId();

            if (parentRef is INTERNAL_HtmlDomElementReference parent)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createInkPresenter('{uid}','{canvasUid}','{parent.UniqueIdentifier}')");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $@"document.createInkPresenter('{uid}','{canvasUid}',{sParentRef})");
            }

            AddToGlobalStore(uid, inkPresenter);
            AddToGlobalStore(canvasUid, inkPresenter);

            return (new(uid), new(canvasUid));
        }

        internal static INTERNAL_HtmlDomElementReference CreateTextElementDomElementAndAppendIt(object parentRef, TextElement textElement)
        {
            string uniqueIdentifier = NewId();

            if (parentRef is INTERNAL_HtmlDomElementReference parent)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createText('{textElement.TagName}','{uniqueIdentifier}','{parent.UniqueIdentifier}')");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createText('{textElement.TagName}','{uniqueIdentifier}',{sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, textElement);

            return new(uniqueIdentifier);
        }

        internal static INTERNAL_HtmlDomElementReference CreateBorderDomElementAndAppendIt(object parentRef, Border border)
        {
            Debug.Assert(border is not null);

            string uniqueIdentifier = NewId();

            if (parentRef is INTERNAL_HtmlDomElementReference parent)
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createBorder('{uniqueIdentifier}','{parent.UniqueIdentifier}')");
            }
            else
            {
                string sParentRef = OpenSilver.Interop.GetVariableStringForJS(parentRef);
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.createBorder('{uniqueIdentifier}',{sParentRef})");
            }

            AddToGlobalStore(uniqueIdentifier, border);

            return new(uniqueIdentifier);
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
                $"document.createShape('{shape.SvgTagName}','{svgUid}','{shapeUid}','{defsUid}','{parent.UniqueIdentifier}')");

            AddToGlobalStore(svgUid, shape);
            AddToGlobalStore(shapeUid, shape);

            return (new(svgUid), new(shapeUid), new(defsUid));
        }

        internal static INTERNAL_HtmlDomElementReference CreateSvgElementAndAppendIt(INTERNAL_HtmlDomElementReference parent, string tagName)
        {
            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.createSvg('{uid}','{parent.UniqueIdentifier}','{tagName}')");

            return new(uid);
        }

        internal static (INTERNAL_HtmlDomElementReference PresenterElement, INTERNAL_HtmlDomElementReference ContentElement)
            CreateHtmlPresenterElementAndAppendIt(INTERNAL_HtmlDomElementReference parent, HtmlPresenter htmlPresenter)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(htmlPresenter is not null);

            string id = NewId();
            string contentId = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"document.htmlPresenterHelpers.createView('{id}','{contentId}','{parent.UniqueIdentifier}',{(htmlPresenter.IsUsingShadowDOM ? "true" : "false")})");

            AddToGlobalStore(id, htmlPresenter);

            return (new(id), new(contentId));
        }

        internal static INTERNAL_HtmlDomElementReference CreateTextBoxViewDomElementAndAppendIt(
            INTERNAL_HtmlDomElementReference parent,
            TextBoxView textBoxView)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(textBoxView is not null);

            string uid = NewId();

            TextViewManager.Instance.CreateTextView(uid, parent.UniqueIdentifier);
            
            AddToGlobalStore(uid, textBoxView);

            return new(uid);
        }

        internal static INTERNAL_HtmlDomElementReference CreatePasswordBoxViewDomElementAndAppendIt(
            INTERNAL_HtmlDomElementReference parent,
            PasswordBoxView passwordBoxView)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(passwordBoxView is not null);

            string uid = NewId();

            TextViewManager.Instance.CreatePasswordView(uid, parent.UniqueIdentifier);
            
            AddToGlobalStore(uid, passwordBoxView);

            return new(uid);
        }

        internal static INTERNAL_HtmlDomElementReference CreateRichTextBoxViewDomElementAndAppendIt(
            INTERNAL_HtmlDomElementReference parent,
            RichTextBoxView richTextBoxView)
        {
            Debug.Assert(parent is not null);
            Debug.Assert(richTextBoxView is not null);

            string uid = NewId();

            RichTextViewManager.Instance.CreateView(uid, parent.UniqueIdentifier);

            AddToGlobalStore(uid, richTextBoxView);

            return new INTERNAL_HtmlDomElementReference(uid);
        }

        private static string NewId() => $"id{_idGenerator.NewId()}";

        internal static string EscapeStringForUseInJavaScript(string s)
        {
            // credits: http://stackoverflow.com/questions/1242118/how-to-escape-json-string

            if (s == null || s.Length == 0)
            {
                return string.Empty;
            }

            int i;
            int len = s.Length;
            StringBuilder sb = StringBuilderCache.Acquire();

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

            return StringBuilderCache.GetStringAndRelease(sb);
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

        internal static void ArrangeNative(INTERNAL_HtmlDomStyleReference style, Point offset, Size size, Rect? clip)
        {
            string left = Math.Round(offset.X, 2).ToInvariantString();
            string top = Math.Round(offset.Y, 2).ToInvariantString();
            string width = Math.Round(size.Width, 2).ToInvariantString();
            string height = Math.Round(size.Height, 2).ToInvariantString();
            
            if (clip.HasValue)
            {
                Rect clipRect = clip.Value;
                string clipLeft = Math.Round(clipRect.Left, 2).ToInvariantString();
                string clipTop = Math.Round(clipRect.Top, 2).ToInvariantString();
                string clipRight = Math.Round(clipRect.Right, 2).ToInvariantString();
                string clipBottom = Math.Round(clipRect.Bottom, 2).ToInvariantString();

                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.arrange('{style.Uid}',{left},{top},{width},{height},true,{clipLeft},{clipTop},{clipRight},{clipBottom})");
            }
            else
            {
                OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                    $"document.arrange('{style.Uid}',{left},{top},{width},{height})");
            }
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
