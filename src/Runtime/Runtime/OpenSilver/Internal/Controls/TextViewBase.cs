
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

namespace OpenSilver.Internal.Controls;

internal abstract partial class TextViewBase : FrameworkElement
{
    private Size _contentSize;

    internal TextViewBase(UIElement host)
    {
        Debug.Assert(host is TextBox || host is PasswordBox || host is RichTextBox);

        Host = host ?? throw new ArgumentNullException(nameof(host));
    }

    internal UIElement Host { get; }

    internal sealed override UIElement KeyboardTarget => Host;

    internal sealed override bool EnablePointerEventsCore => true;

    protected abstract Size MeasureContent(Size constraint);

    internal protected abstract void OnInput();

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