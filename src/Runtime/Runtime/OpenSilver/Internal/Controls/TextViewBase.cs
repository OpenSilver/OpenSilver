
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
using CSHTML5.Internal;

#if MIGRATION
using System.Windows;
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace OpenSilver.Internal.Controls;

internal abstract partial class TextViewBase<T> : FrameworkElement
    where T : UIElement
{
    private Size _contentSize;
    private JavaScriptCallback _inputCallback;
    private JavaScriptCallback _scrollCallback;

    internal TextViewBase(T host)
    {
        Host = host ?? throw new ArgumentNullException(nameof(host));
    }

    internal T Host { get; }

    internal object InputDiv => INTERNAL_OuterDomElement;

    internal sealed override UIElement KeyboardTarget => Host;

    internal sealed override bool EnablePointerEventsCore => true;

    public sealed override void INTERNAL_AttachToDomEvents()
    {
        base.INTERNAL_AttachToDomEvents();

        _inputCallback = JavaScriptCallback.Create(OnInputNative, true);
        _scrollCallback = JavaScriptCallback.Create(OnScrollNative, true);

        string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
        string sInputCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_inputCallback);
        string sScrollCallback = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(_scrollCallback);
        Interop.ExecuteJavaScriptFastAsync($@"{sDiv}.addEventListener('input', {sInputCallback});
{sDiv}.addEventListener('scroll', {sScrollCallback});");
    }

    public sealed override void INTERNAL_DetachFromDomEvents()
    {
        base.INTERNAL_DetachFromDomEvents();

        _inputCallback?.Dispose();
        _inputCallback = null;

        _scrollCallback?.Dispose();
        _scrollCallback = null;
    }

    internal sealed override void AddEventListeners() => InputManager.Current.AddEventListeners(this, true);

    protected abstract Size MeasureContent(Size constraint);

    protected abstract void OnInput();

    private void OnInputNative() => OnInput();

    private void OnScrollNative()
    {
        if (!IsScrollClient) return;

        string sDiv = CSHTML5.INTERNAL_InteropImplementation.GetVariableStringForJS(INTERNAL_OuterDomElement);
        double scrollLeft = Interop.ExecuteJavaScriptDouble($"{sDiv}.scrollLeft;");
        double scrollTop = Interop.ExecuteJavaScriptDouble($"{sDiv}.scrollTop;");

        UpdateOffsets(new Point(scrollLeft, scrollTop));
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