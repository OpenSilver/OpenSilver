
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Shapes;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using HtmlPresenter = CSHTML5.Native.Html.Controls.HtmlPresenter;
using OpenSilver;

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

        private static void AddToGlobalStore(string uid, UIElement element)
        {
            _store.Add(uid, new WeakReference<UIElement>(element));
        }

        private static void AddToGlobalStore(string uid1, string uid2, UIElement element)
        {
            var wr = new WeakReference<UIElement>(element);
            _store.Add(uid1, wr);
            _store.Add(uid2, wr);
        }

        internal static void RemoveFromGlobalStore(HtmlElementReference element)
        {
            if (element.IsConnected)
            {
                _store.Remove(element.Uid);
            }
        }

        internal static void RemoveFromDom(HtmlElementReference element)
        {
            if (SyncRenderingWithLayout)
            {
                LayoutManager.Current.UIRenderer.RemoveRootComponent(element);
            }
            else
            {
                RemoveNodeNative(element);
            }

            RemoveFromGlobalStore(element);
        }

        internal static void RemoveNodeNative(HtmlElementReference element) =>
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.detachView('{element.Uid}')");

        private static object _window;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static object GetHtmlWindow()
        {
            return _window ??= OpenSilver.Interop.ExecuteJavaScript("window");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static HtmlElementStyleReference GetFrameworkElementOuterStyleForModification(UIElement element)
            => new(element.OuterDiv.Uid);

        internal static void AddCSSClass(HtmlElementReference element, string className)
        {
            Debug.Assert(element.IsConnected);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.addClass('{element.Uid}','{className}')");
        }

        internal static void RemoveCSSClass(HtmlElementReference element, string className)
        {
            Debug.Assert(element.IsConnected);
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.removeClass('{element.Uid}','{className}')");
        }

        internal static void SetVisibility(HtmlElementReference element, Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.Visible:
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.setVisible('{element.Uid}')");
                    break;

                case Visibility.Hidden:
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.setHidden('{element.Uid}')");
                    break;

                case Visibility.Collapsed:
                    OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.setCollapsed('{element.Uid}')");
                    break;
            }
        }

        internal static HtmlElementReference AppendDomElement(
            string tagName,
            HtmlElementReference parent,
            UIElement uie,
            int index = -1)
        {
            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createVisual('{tagName}', '{uid}', '{parent.Uid}', {index.ToInvariantString()})");

            AddToGlobalStore(uid, uie);

            return new(uid);
        }

        internal static HtmlElementReference CreateDomLayoutElementAndAppendIt(
            string tagName, HtmlElementReference parent, UIElement uie)
        {
            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.createLayout('{tagName}','{uid}','{parent.Uid}')");

            AddToGlobalStore(uid, uie);

            return new(uid);
        }

        internal static HtmlElementReference CreateWindowDomElementAndAppendIt(Window window)
        {
            Debug.Assert(window is not null);

            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createWindow('{uid}', '{window.RootDomElement.Uid}')");

            AddToGlobalStore(uid, window);

            return new(uid);
        }

        internal static HtmlElementReference CreatePopupRootDomElementAndAppendIt(PopupRoot popupRoot)
        {
            Debug.Assert(popupRoot != null);

            string uid = NewId();

            string sPointerEvents = popupRoot.Popup.StayOpen ? "none" : "auto";
            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createPopupRoot('{uid}','{popupRoot.ParentWindow.RootDomElement.Uid}','{sPointerEvents}')");

            AddToGlobalStore(uid, popupRoot);

            return new(uid);
        }

        internal static HtmlElementReference CreatePopupRootDomElementAndAppendIt(HtmlElementReference parentDiv, UIElement element)
        {
            Debug.Assert(element is not null);

            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createPopupRoot('{uid}','{parentDiv.Uid}','auto')");

            AddToGlobalStore(uid, element);

            return new(uid);
        }

        internal static HtmlElementReference CreateWindowHostRootDomElementAndAppendIt(HtmlElementReference parentDiv, UIElement element)
        {
            Debug.Assert(element is not null);

            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createWindowHostRoot('{uid}','{parentDiv.Uid}')");

            AddToGlobalStore(uid, element);

            return new(uid);
        }

        internal static HtmlElementReference CreateTaskbarItemRootDomElementAndAppendIt(HtmlElementReference parentDiv, UIElement element)
        {
            Debug.Assert(element is not null);

            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createTaskbarItemRoot('{uid}','{parentDiv.Uid}')");

            AddToGlobalStore(uid, element);

            return new(uid);
        }

        internal static HtmlElementReference CreateWindowOverlayDomElementAndAppendIt(
            Window window, HtmlElementReference rootElement, bool isModal)
        {
            Debug.Assert(window is not null);

            string uid = NewId();
            string pointerEvents = isModal ? "auto" : "none";

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createWindowOverlay('{uid}', '{rootElement.Uid}', '{pointerEvents}')");

            return new(uid);
        }


        internal static HtmlElementReference CreateWindowContentDomElementAndAppendIt(Window window, HtmlElementReference chromeDomDiv)
        {
            Debug.Assert(window is not null);

            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createWindowContent('{uid}', '{chromeDomDiv.Uid}')");

            AddToGlobalStore(uid, window);

            return new(uid);
        }

        internal static HtmlElementReference CreateTextBlockDomElementAndAppendIt(HtmlElementReference parent, UIElement textBlock)
        {
#if PERFSTAT
            Performance.Counter("CreateTextBlockDomElementAndAppendIt", t0);
#endif
            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createTextBlock('{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, textBlock);

            return new(uniqueIdentifier);
        }

        internal static (HtmlElementReference OuterDiv, HtmlElementReference Image) CreateImageDomElementAndAppendIt(
            HtmlElementReference parent, Image image)
        {
            Debug.Assert(image is not null);

            string uid = NewId();
            string imgUid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.image.create('{uid}','{imgUid}','{parent.Uid}')");

            AddToGlobalStore(uid, imgUid, image);

            return (new(uid), new(imgUid));
        }

        internal static (HtmlElementReference OuterDiv, HtmlElementReference Canvas) CreateInkPresenterDomElementAndAppendIt(
            HtmlElementReference parent, InkPresenter inkPresenter)
        {
            Debug.Assert(parent.IsConnected);
            Debug.Assert(inkPresenter is not null);

            string uid = NewId();
            string canvasUid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createInkPresenter('{uid}','{canvasUid}','{parent.Uid}')");

            AddToGlobalStore(uid, canvasUid, inkPresenter);

            return (new(uid), new(canvasUid));
        }

        internal static HtmlElementReference CreateInlineDomElementAndAppendIt(HtmlElementReference parent, Inline inline)
        {
            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createInline('{inline.TagName}','{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, inline);

            return new(uniqueIdentifier);
        }

        internal static HtmlElementReference CreateBlockDomElementAndAppendIt(HtmlElementReference parent, Block block)
        {
            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createBlock('{block.TagName}','{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, block);

            return new(uniqueIdentifier);
        }

        internal static HtmlElementReference CreateListDomElementAndAppendIt(HtmlElementReference parent, List list)
        {
            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createList('{list.TagName}','{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, list);

            return new(uniqueIdentifier);
        }

        internal static HtmlElementReference CreateListItemDomElementAndAppendIt(HtmlElementReference parent, ListItem listItem)
        {
            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createListItem('{listItem.TagName}','{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, listItem);

            return new(uniqueIdentifier);
        }

        internal static HtmlElementReference CreateHyperlinkDomElementAndAppendIt(HtmlElementReference parent, Hyperlink hyperlink)
        {
            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createHyperlink('{hyperlink.TagName}','{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, hyperlink);

            return new(uniqueIdentifier);
        }

        internal static HtmlElementReference CreateBorderDomElementAndAppendIt(HtmlElementReference parent, UIElement border)
        {
            Debug.Assert(border is IBorderElement);

            string uniqueIdentifier = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createBorder('{uniqueIdentifier}','{parent.Uid}')");

            AddToGlobalStore(uniqueIdentifier, border);

            return new(uniqueIdentifier);
        }

        internal static (HtmlElementReference SvgElement, HtmlElementReference SvgShape, HtmlElementReference SvgDefs)
            CreateShapeElementAndAppendIt(HtmlElementReference parent, Shape shape)
        {
            Debug.Assert(parent.IsConnected);
            Debug.Assert(shape is not null);

            string svgUid = NewId();
            string shapeUid = NewId();
            string defsUid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createShape('{shape.SvgTagName}','{svgUid}','{shapeUid}','{defsUid}','{parent.Uid}')");

            AddToGlobalStore(svgUid, shapeUid, shape);

            return (new(svgUid), new(shapeUid), new(defsUid));
        }

        internal static HtmlElementReference CreateSvgElementAndAppendIt(HtmlElementReference parent, string tagName)
        {
            string uid = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.createSvg('{uid}','{parent.Uid}','{tagName}')");

            return new(uid);
        }

        internal static (HtmlElementReference PresenterElement, HtmlElementReference ContentElement)
            CreateHtmlPresenterElementAndAppendIt(HtmlElementReference parent, HtmlPresenter htmlPresenter)
        {
            Debug.Assert(parent.IsConnected);
            Debug.Assert(htmlPresenter is not null);

            string id = NewId();
            string contentId = NewId();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync(
                $"osjs.htmlPresenter.createView('{id}','{contentId}','{parent.Uid}',{(htmlPresenter.UseShadowDom ? "true" : "false")})");

            AddToGlobalStore(id, htmlPresenter);

            return (new(id), new(contentId));
        }

        internal static HtmlElementReference CreateTextBoxViewDomElementAndAppendIt(
            HtmlElementReference parent,
            TextBoxView textBoxView)
        {
            Debug.Assert(parent.IsConnected);
            Debug.Assert(textBoxView is not null);

            string uid = NewId();

            TextViewManager.CreateTextView(uid, parent.Uid);

            AddToGlobalStore(uid, textBoxView);

            return new(uid);
        }

        internal static HtmlElementReference CreatePasswordBoxViewDomElementAndAppendIt(
            HtmlElementReference parent,
            PasswordBoxView passwordBoxView)
        {
            Debug.Assert(passwordBoxView is not null);

            string uid = NewId();

            TextViewManager.CreatePasswordView(uid, parent.Uid);

            AddToGlobalStore(uid, passwordBoxView);

            return new(uid);
        }

        internal static HtmlElementReference CreateRichTextBoxViewDomElementAndAppendIt(
            HtmlElementReference parent,
            RichTextBoxView richTextBoxView)
        {
            Debug.Assert(parent.IsConnected);
            Debug.Assert(richTextBoxView is not null);

            string uid = NewId();

            RichTextViewManager.CreateView(uid, parent.Uid);

            AddToGlobalStore(uid, richTextBoxView);

            return new(uid);
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
            ArgumentNullException.ThrowIfNull(obj);

            if (obj is string str)
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

        internal static void ArrangeNative(string htmlId, Vector offset, Size size)
        {
            string left = Math.Round(offset.X, 2).ToInvariantString();
            string top = Math.Round(offset.Y, 2).ToInvariantString();
            string width = Math.Round(size.Width, 2).ToInvariantString();
            string height = Math.Round(size.Height, 2).ToInvariantString();

            OpenSilver.Interop.ExecuteJavaScriptVoidAsync($"osjs.arrange('{htmlId}',{left},{top},{width},{height})");
        }
    }
}
