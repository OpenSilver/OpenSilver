
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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal abstract partial class TextViewBase : FrameworkElement
{
    private static readonly JavaScriptCallback _inputHandler;
    private static readonly JavaScriptCallback _scrollHandler;

    private Size _contentSize;

    static TextViewBase()
    {
        _inputHandler = JavaScriptCallback.Create(OnInputNative, true);
        _scrollHandler = JavaScriptCallback.Create(OnScrollNative, true);
        string sInputHandler = Interop.GetVariableStringForJS(_inputHandler);
        string sScrollHandler = Interop.GetVariableStringForJS(_scrollHandler);
        Interop.ExecuteJavaScriptVoidAsync($"document.createTextviewManager({sInputHandler},{sScrollHandler});");
    }

    internal TextViewBase(UIElement host)
    {
        Debug.Assert(host is TextBox || host is PasswordBox);

        Host = host ?? throw new ArgumentNullException(nameof(host));
    }

    internal UIElement Host { get; }

    internal sealed override UIElement KeyboardTarget => Host;

    internal sealed override bool EnablePointerEventsCore => true;

    protected abstract Size MeasureContent(Size constraint);

    protected abstract void OnInput();

    private static void OnInputNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextViewBase textview)
        {
            textview.OnInput();
        }
    }

    private static void OnScrollNative(string id)
    {
        if (INTERNAL_HtmlDomManager.GetElementById(id) is TextViewBase textview)
        {
            if (!textview.IsScrollClient) return;

            string sDiv = Interop.GetVariableStringForJS(textview.OuterDiv);
            double scrollLeft = Interop.ExecuteJavaScriptDouble($"{sDiv}.scrollLeft;");
            double scrollTop = Interop.ExecuteJavaScriptDouble($"{sDiv}.scrollTop;");

            textview.UpdateOffsets(new Point(scrollLeft, scrollTop));
        }
    }

    protected sealed override Size MeasureOverride(Size availableSize)
    {
        Size constraint = availableSize;

        if (IsScrollClient)
        {
            if (_scrollData.CanHorizontallyScroll) constraint.Width = double.PositiveInfinity;
            if (_scrollData.CanVerticallyScroll) constraint.Height = double.PositiveInfinity;
        }

        Size textSize = MeasureContent(constraint);

        _contentSize = textSize;

        // DesiredSize is set to the calculated size of the content.
        // If hosted by ScrollViewer, desired size is limited to constraint.
        if (IsScrollClient)
        {
            textSize.Width = Math.Min(textSize.Width, availableSize.Width);
            textSize.Height = Math.Min(textSize.Height, availableSize.Height);
        }

        return textSize;
    }

    protected sealed override Size ArrangeOverride(Size finalSize)
    {
        ArrangeScrollData(finalSize);

        return finalSize;
    }
}