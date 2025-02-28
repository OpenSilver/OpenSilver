
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
using System.Windows.Documents;
using System.Windows.Media;
using CSHTML5.Internal;

namespace OpenSilver.Internal.Controls;

internal abstract partial class TextViewBase : FrameworkElement
{
    static TextViewBase()
    {
        TextElement.CharacterSpacingProperty.AddOwner(
            typeof(TextViewBase),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetCharacterSpacing((int)newValue),
            });

        TextElement.FontFamilyProperty.AddOwner(
            typeof(TextViewBase),
            new FrameworkPropertyMetadata(FontFamily.Default, FrameworkPropertyMetadataOptions.Inherits, OnFontFamilyChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetFontFamily((FontFamily)newValue),
            });

        TextElement.FontSizeProperty.AddOwner(
            typeof(TextViewBase),
            new FrameworkPropertyMetadata(11d, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetFontSize((double)newValue),
            });

        TextElement.FontStyleProperty.AddOwner(
            typeof(TextViewBase),
            new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetFontStyle((FontStyle)newValue),
            });

        TextElement.FontWeightProperty.AddOwner(
           typeof(TextViewBase),
           new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure)
           {
               MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetFontWeight((FontWeight)newValue),
           });

        TextElement.ForegroundProperty.AddOwner(
            typeof(TextViewBase),
            new FrameworkPropertyMetadata(
                TextElement.ForegroundProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.Inherits,
                OnForegroundChanged)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetForeground(oldValue as Brush, (Brush)newValue),
            });

        IsHitTestableProperty.OverrideMetadata(typeof(TextViewBase), new PropertyMetadata(BooleanBoxes.TrueBox));

        FlowDirectionProperty.OverrideMetadata(
            typeof(TextViewBase),
            new FrameworkPropertyMetadata(
                FlowDirection.LeftToRight,
                FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsParentArrange)
            {
                MethodToUpdateDom2 = static (d, oldValue, newValue) => ((TextViewBase)d).SetDirection((FlowDirection)newValue),
            });
    }

    private Size _contentSize;
    private WeakEventListener<TextViewBase, Brush, EventArgs> _foregroundChangedListener;

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

    internal sealed override bool ShouldApplyMirrorTransform()
    {
        return GetFlowDirectionFromVisual(VisualTreeHelper.GetParent(this)) == FlowDirection.RightToLeft;
    }

    private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UIElementHelpers.InvalidateMeasureOnFontFamilyChanged((TextViewBase)d, (FontFamily)e.NewValue);
    }

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (TextViewBase)d;

        if (view._foregroundChangedListener != null)
        {
            view._foregroundChangedListener.Detach();
            view._foregroundChangedListener = null;
        }

        if (e.NewValue is Brush newBrush)
        {
            view._foregroundChangedListener = new(view, newBrush)
            {
                OnEventAction = static (instance, sender, args) => instance.OnForegroundChanged(sender, args),
                OnDetachAction = static (listener, source) => source.Changed -= listener.OnEvent,
            };
            newBrush.Changed += view._foregroundChangedListener.OnEvent;
        }
    }

    private void OnForegroundChanged(object sender, EventArgs e)
    {
        if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
        {
            var foreground = (Brush)sender;
            this.SetForeground(foreground, foreground);
        }
    }
}