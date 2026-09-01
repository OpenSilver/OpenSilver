
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

using CSHTML5.Internal;
using OpenSilver.Internal;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace System.Windows;

public partial class FrameworkElement
{
    /// <summary>
    /// Identifies the <see cref="HorizontalAlignment"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.Register(
            nameof(HorizontalAlignment),
            typeof(HorizontalAlignment),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Gets or sets the horizontal alignment characteristics that are applied to a <see cref="FrameworkElement"/>
    /// when it is composed in a layout parent, such as a panel or items control.
    /// </summary>
    /// <returns>
    /// A horizontal alignment setting, as a value of the enumeration. The default is <see cref="HorizontalAlignment.Stretch"/>.
    /// </returns>
    public HorizontalAlignment HorizontalAlignment
    {
        get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
        set => SetValueInternal(HorizontalAlignmentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="VerticalAlignment"/> dependency  property.
    /// </summary>
    public static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.Register(
            nameof(VerticalAlignment),
            typeof(VerticalAlignment),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Gets or sets the vertical alignment characteristics that are applied to a <see cref="FrameworkElement"/>
    /// when it is composed in a parent object such as a panel or items control.
    /// </summary>
    /// <returns>
    /// A vertical alignment setting. The default is <see cref="VerticalAlignment.Stretch"/>.
    /// </returns>
    public VerticalAlignment VerticalAlignment
    {
        get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
        set => SetValueInternal(VerticalAlignmentProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register(
            nameof(Margin),
            typeof(Thickness),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure),
            IsMarginValid);

    /// <summary>
    /// Gets or sets the outer margin of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// Provides margin values for the object. The default value is a default <see cref="Thickness"/> 
    /// with all properties (dimensions) equal to 0.
    /// </returns>
    public Thickness Margin
    {
        get => (Thickness)GetValue(MarginProperty);
        set => SetValueInternal(MarginProperty, value);
    }

    private static bool IsMarginValid(object value)
    {
        Thickness m = (Thickness)value;
        return Thickness.IsValid(m, true, false, false, false);
    }

    /// <summary>
    /// Identifies the <see cref="LayoutTransform"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LayoutTransformProperty =
        DependencyProperty.Register(
            nameof(LayoutTransform),
            typeof(Transform),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(Transform.Identity, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty));

    /// <summary>
    /// Gets or sets a graphics transformation that should apply to this element when layout is performed.
    /// </summary>
    /// <returns>
    /// The transform this element should use. The default is <see cref="Transform.Identity"/>.
    /// </returns>
    public Transform LayoutTransform
    {
        get => (Transform)GetValue(LayoutTransformProperty);
        set => SetValueInternal(LayoutTransformProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Width"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(
            nameof(Width),
            typeof(double),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty),
            IsWidthHeightValid);

    /// <summary>
    /// Gets or sets the width of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The width of the object, in pixels. The default is <see cref="double.NaN"/>. Except
    /// for the special <see cref="double.NaN"/> value, this value must be equal to or greater
    /// than 0.
    /// </returns>
    [TypeConverter(typeof(LengthConverter))]
    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValueInternal(WidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinWidthProperty =
        DependencyProperty.Register(
            nameof(MinWidth),
            typeof(double),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty),
            IsMinWidthHeightValid);

    /// <summary>
    /// Gets or sets the minimum width constraint of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The minimum width of the object, in pixels. The default is 0. This value can be any value 
    /// equal to or greater than 0. However, <see cref="double.PositiveInfinity"/> is not valid.
    /// </returns>
    public double MinWidth
    {
        get => (double)GetValue(MinWidthProperty);
        set => SetValueInternal(MinWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MaxWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxWidthProperty =
        DependencyProperty.Register(
            nameof(MaxWidth),
            typeof(double),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty),
            IsMaxWidthHeightValid);

    /// <summary>
    /// Gets or sets the maximum width constraint of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The maximum width of the object, in pixels. The default is <see cref="double.PositiveInfinity"/>.
    /// This value can be any value equal to or greater than 0. <see cref="double.PositiveInfinity"/> is also valid.
    /// </returns>
    public double MaxWidth
    {
        get => (double)GetValue(MaxWidthProperty);
        set => SetValueInternal(MaxWidthProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Height"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register(
            nameof(Height),
            typeof(double),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty),
            IsWidthHeightValid);

    /// <summary>
    /// Gets or sets the suggested height of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The height, in pixels, of the object. The default is <see cref="double.NaN"/>. Except
    /// for the special <see cref="double.NaN"/> value, this value must be equal to or greater
    /// than 0.
    /// </returns>
    [TypeConverter(typeof(LengthConverter))]
    public double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValueInternal(HeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MinHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MinHeightProperty =
        DependencyProperty.Register(
            nameof(MinHeight),
            typeof(double),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty),
            IsMinWidthHeightValid);

    /// <summary>
    /// Gets or sets the minimum height constraint of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The minimum height of the object, in pixels. The default is 0. This value can be any value 
    /// equal to or greater than 0. However, <see cref="double.PositiveInfinity"/> is not valid.
    /// </returns>
    public double MinHeight
    {
        get => (double)GetValue(MinHeightProperty);
        set => SetValueInternal(MinHeightProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="MaxHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxHeightProperty =
        DependencyProperty.Register(
            nameof(MaxHeight),
            typeof(double),
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsMeasure, OnTransformDirty),
            IsMaxWidthHeightValid);

    /// <summary>
    /// Gets or sets the maximum height constraint of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The maximum height of the object, in pixels. The default value is <see cref="double.PositiveInfinity"/>.
    /// This value can be any value equal to or greater than 0. <see cref="double.PositiveInfinity"/> is also valid.
    /// </returns>
    public double MaxHeight
    {
        get => (double)GetValue(MaxHeightProperty);
        set => SetValueInternal(MaxHeightProperty, value);
    }

    private static bool IsWidthHeightValid(object value)
    {
        double v = (double)value;
        return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
    }

    internal static bool IsMinWidthHeightValid(object value)
    {
        double v = (double)value;
        return !double.IsNaN(v) && v >= 0.0d && !double.IsPositiveInfinity(v);
    }

    internal static bool IsMaxWidthHeightValid(object value)
    {
        double v = (double)value;
        return !double.IsNaN(v) && v >= 0.0;
    }

    private static void OnTransformDirty(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Callback for MinWidth, MaxWidth, Width, MinHeight, MaxHeight, Height, and LayoutTransform
        ((FrameworkElement)d).AreTransformsClean = false;
    }

    private static readonly PropertyMetadata _actualWidthMetadata = new ReadOnlyPropertyMetadata(0d, GetActualWidth);

    private static readonly DependencyPropertyKey ActualWidthPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ActualWidth),
            typeof(double),
            typeof(FrameworkElement),
            _actualWidthMetadata);

    private static object GetActualWidth(DependencyObject d) => ((FrameworkElement)d).ActualWidth;

    /// <summary>
    /// Identifies the <see cref="ActualWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the rendered width of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The width, in pixels, of the object. The default is 0. The default might be 
    /// encountered if the object has not been loaded and undergone a layout pass.
    /// </returns>
    public double ActualWidth => ActualWidthInternal;

    internal virtual double ActualWidthInternal => RenderSize.Width;

    private static readonly PropertyMetadata _actualHeightMetadata = new ReadOnlyPropertyMetadata(0d, GetActualHeight);

    private static readonly DependencyPropertyKey ActualHeightPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(ActualHeight),
            typeof(double),
            typeof(FrameworkElement),
            _actualHeightMetadata);

    private static object GetActualHeight(DependencyObject d) => ((FrameworkElement)d).ActualHeight;

    /// <summary>
    /// Identifies the <see cref="ActualHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the rendered height of a <see cref="FrameworkElement"/>.
    /// </summary>
    /// <returns>
    /// The height, in pixels, of the object. The default is 0. The default might be
    /// encountered if the object has not been loaded and undergone a layout pass.
    /// </returns>
    public double ActualHeight => ActualHeightInternal;

    internal virtual double ActualHeightInternal => RenderSize.Height;

    /// <summary>
    /// Implements basic measure-pass layout system behavior for <see cref="FrameworkElement"/>.
    /// </summary>
    /// <param name="availableSize">
    /// The available size that the parent element can give to the child elements.
    /// </param>
    /// <returns>
    /// The desired size of this element in layout.
    /// </returns>
    protected sealed override Size MeasureCore(Size availableSize)
    {
        // build the visual tree from styles first
        ApplyTemplate();

        if (BypassLayoutPolicies)
        {
            return MeasureOverride(availableSize);
        }
        else
        {
            Thickness margin = Margin;
            double marginWidth = margin.Left + margin.Right;
            double marginHeight = margin.Top + margin.Bottom;

            // parent size is what parent want us to be
            Size frameworkAvailableSize = new Size(
                Math.Max(availableSize.Width - marginWidth, 0),
                Math.Max(availableSize.Height - marginHeight, 0));

            MinMax mm = new MinMax(this);

            LayoutTransformData ltd = LayoutTransformData.GetData(this);
            Transform layoutTransform = LayoutTransform;

            // check that LayoutTransform is non-trivial
            if (layoutTransform is not null && !Transform.IsIdentityTransform(layoutTransform))
            {
                // allocate and store ltd if needed
                ltd ??= LayoutTransformData.CreateData(this);

                ltd.CreateTransformSnapshot(layoutTransform);
                ltd.UntransformedDS = new Size();
            }
            else if (ltd is not null)
            {
                // clear ltd storage
                ltd = null;
                LayoutTransformData.ClearData(this);
            }

            if (ltd is not null)
            {
                // Find the maximal area rectangle in local (child) space that we can fit, post-transform
                // in the decorator's measure constraint.
                frameworkAvailableSize = FindMaximalAreaLocalSpaceRect(ltd.Transform, frameworkAvailableSize);
            }

            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            // call to specific layout to measure
            Size desiredSize = MeasureOverride(frameworkAvailableSize);

            // maximize desiredSize with user provided min size
            desiredSize = new Size(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            // here is the "true minimum" desired size - the one that is
            // for sure enough for the control to render its content.
            Size unclippedDesiredSize = desiredSize;

            if (ltd is not null)
            {
                // need to store unclipped, untransformed desired size to be able to arrange later
                ltd.UntransformedDS = unclippedDesiredSize;

                // transform unclipped desired size
                Rect unclippedBoundsTransformed = Rect.Transform(
                    new Rect(0, 0, unclippedDesiredSize.Width, unclippedDesiredSize.Height), ltd.Transform);

                unclippedDesiredSize.Width = unclippedBoundsTransformed.Width;
                unclippedDesiredSize.Height = unclippedBoundsTransformed.Height;
            }

            bool clipped = false;

            // User-specified max size starts to "clip" the control here.
            // Starting from this point desiredSize could be smaller then actually
            // needed to render the whole control
            if (desiredSize.Width > mm.maxWidth)
            {
                desiredSize.Width = mm.maxWidth;
                clipped = true;
            }

            if (desiredSize.Height > mm.maxHeight)
            {
                desiredSize.Height = mm.maxHeight;
                clipped = true;
            }

            // transform desired size to layout slot space
            if (ltd is not null)
            {
                Rect childBoundsTransformed = Rect.Transform(
                    new Rect(0, 0, desiredSize.Width, desiredSize.Height), ltd.Transform);

                desiredSize.Width = childBoundsTransformed.Width;
                desiredSize.Height = childBoundsTransformed.Height;
            }

            // because of negative margins, clipped desired size may be negative.
            // need to keep it as doubles for that reason and maximize with 0 at the
            // very last point - before returning desired size to the parent.
            double clippedDesiredWidth = desiredSize.Width + marginWidth;
            double clippedDesiredHeight = desiredSize.Height + marginHeight;

            // In overconstrained scenario, parent wins and measured size of the child,
            // including any sizes set or computed, can not be larger then
            // available size. We will clip the guy later.
            if (clippedDesiredWidth > availableSize.Width)
            {
                clippedDesiredWidth = availableSize.Width;
                clipped = true;
            }

            if (clippedDesiredHeight > availableSize.Height)
            {
                clippedDesiredHeight = availableSize.Height;
                clipped = true;
            }

            // Note: unclippedDesiredSize is needed in ArrangeCore,
            // because due to the layout protocol, arrange should be called
            // with constraints greater or equal to child's desired size
            // returned from MeasureOverride. But in most circumstances
            // it is possible to reconstruct original unclipped desired size.
            // In such cases we want to optimize space and save 16 bytes by
            // not storing it on each FrameworkElement.
            //
            // The if statement conditions below lists the cases when
            // it is NOT possible to recalculate unclipped desired size later
            // in ArrangeCore, thus we save it...
            if (clipped
                || clippedDesiredWidth < 0
                || clippedDesiredHeight < 0)
            {
                _unclippedDesiredSize = unclippedDesiredSize;
            }
            else
            {
                _unclippedDesiredSize = Size.Empty;
            }

            return new Size(Math.Max(0, clippedDesiredWidth), Math.Max(0, clippedDesiredHeight));
        }
    }

    /// <summary>
    /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override 
    /// this method to define their own Measure pass behavior.
    /// </summary>
    /// <param name="availableSize">
    /// The available size that this object can give to child objects. Infinity (<see cref="double.PositiveInfinity"/>)
    /// can be specified as a value to indicate that the object will size to whatever content is 
    /// available.
    /// </param>
    /// <returns>
    /// The size that this object determines it needs during layout, based on its calculations
    /// of the allocated sizes for child objects; or based on other considerations, such as a 
    /// fixed container size.
    /// </returns>
    protected virtual Size MeasureOverride(Size availableSize) => new Size(0, 0);

    /// <summary>
    /// Implements <see cref="UIElement.ArrangeCore(Rect)"/> (defined as virtual in <see cref="UIElement"/>) 
    /// and seals the implementation.
    /// </summary>
    /// <param name="finalRect">
    /// The final area within the parent that this element should use to arrange itself and its children.
    /// </param>
    protected sealed override void ArrangeCore(Rect finalRect)
    {
        LayoutTransformData ltd = LayoutTransformData.GetData(this);

        if (BypassLayoutPolicies)
        {
            Size oldRenderSize = RenderSize;
            Size inkSize = ArrangeOverride(finalRect.Size);
            RenderSize = inkSize;
            SetLayoutOffset(new Vector(finalRect.X, finalRect.Y), oldRenderSize, ltd);
        }
        else
        {
            // If LayoutConstrained==true (parent wins in layout),
            // we might get finalRect.Size smaller then UnclippedDesiredSize.
            // Stricltly speaking, this may be the case even if LayoutConstrained==false (child wins),
            // since who knows what a particualr parent panel will try to do in error.
            // In this case we will not actually arrange a child at a smaller size,
            // since the logic of the child does not expect to receive smaller size
            // (if it coudl deal with smaller size, it probably would accept it in MeasureOverride)
            // so lets replace the smaller arreange size with UnclippedDesiredSize
            // and then clip the guy later.
            // We will use at least UnclippedDesiredSize to compute arrangeSize of the child, and
            // we will use layoutSlotSize to compute alignments - so the bigger child can be aligned within
            // smaller slot.

            // This is computed on every ArrangeCore. Depending on LayoutConstrained, actual clip may apply or not
            NeedsClipBounds = false;

            // Start to compute arrange size for the child.
            // It starts from layout slot or deisred size if layout slot is smaller then desired,
            // and then we reduce it by margins, apply Width/Height etc, to arrive at the size
            // that child will get in its ArrangeOverride.
            Size arrangeSize = finalRect.Size;

            Thickness margin = Margin;
            double marginWidth = margin.Left + margin.Right;
            double marginHeight = margin.Top + margin.Bottom;
            arrangeSize.Width = Math.Max(0, arrangeSize.Width - marginWidth);
            arrangeSize.Height = Math.Max(0, arrangeSize.Height - marginHeight);

            // Next, compare against unclipped, transformed size.
            Size sb = _unclippedDesiredSize;
            Size unclippedDesiredSize;
            if (sb.IsEmpty)
            {
                unclippedDesiredSize = new Size(Math.Max(0, DesiredSize.Width - marginWidth),
                                                Math.Max(0, DesiredSize.Height - marginHeight));
            }
            else
            {
                unclippedDesiredSize = new Size(sb.Width, sb.Height);
            }

            if (DoubleUtil.LessThan(arrangeSize.Width, unclippedDesiredSize.Width))
            {
                NeedsClipBounds = true;
                arrangeSize.Width = unclippedDesiredSize.Width;
            }

            if (DoubleUtil.LessThan(arrangeSize.Height, unclippedDesiredSize.Height))
            {
                NeedsClipBounds = true;
                arrangeSize.Height = unclippedDesiredSize.Height;
            }

            // Alignment==Stretch --> arrange at the slot size minus margins
            // Alignment!=Stretch --> arrange at the unclippedDesiredSize
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                arrangeSize.Width = unclippedDesiredSize.Width;
            }

            if (VerticalAlignment != VerticalAlignment.Stretch)
            {
                arrangeSize.Height = unclippedDesiredSize.Height;
            }

            // if LayoutTransform is set, arrange at untransformed DS always
            // alignments apply to the BoundingBox after transform
            if (ltd is not null)
            {
                // Repeat the measure-time algorithm for finding a best fit local rect.
                // This essentially implements Stretch in case of LayoutTransform
                Size potentialArrangeSize = FindMaximalAreaLocalSpaceRect(ltd.Transform, arrangeSize);
                arrangeSize = potentialArrangeSize;

                // If using layout rounding, round untransformed desired size - in MeasureCore, this value is first transformed and clipped
                // before rounding, and hence saved unrounded.
                unclippedDesiredSize = ltd.UntransformedDS;

                // only use max area rect if both dimensions of it are larger then
                // desired size - replace with desired size otherwise
                if (!DoubleUtil.IsZero(potentialArrangeSize.Width) && !DoubleUtil.IsZero(potentialArrangeSize.Height))
                {
                    // Use less precise comparision - otherwise FP jitter may cause drastic jumps here
                    if (LayoutDoubleUtil.LessThan(potentialArrangeSize.Width, unclippedDesiredSize.Width) ||
                        LayoutDoubleUtil.LessThan(potentialArrangeSize.Height, unclippedDesiredSize.Height))
                    {
                        arrangeSize = unclippedDesiredSize;
                    }
                }

                // if pre-transformed into local space arrangeSize is smaller in any dimension then
                // unclipped local DesiredSize of the element, extend the arrangeSize but
                // remember that we potentially need to clip the result of such arrange.
                if (DoubleUtil.LessThan(arrangeSize.Width, unclippedDesiredSize.Width))
                {
                    NeedsClipBounds = true;
                    arrangeSize.Width = unclippedDesiredSize.Width;
                }

                if (DoubleUtil.LessThan(arrangeSize.Height, unclippedDesiredSize.Height))
                {
                    NeedsClipBounds = true;
                    arrangeSize.Height = unclippedDesiredSize.Height;
                }

            }

            MinMax mm = new MinMax(this);

            // we have to choose max between UnclippedDesiredSize and Max here, because
            // otherwise setting of max property could cause arrange at less then unclippedDS.
            // Clipping by Max is needed to limit stretch here
            double effectiveMaxWidth = Math.Max(unclippedDesiredSize.Width, mm.maxWidth);
            if (DoubleUtil.LessThan(effectiveMaxWidth, arrangeSize.Width))
            {
                NeedsClipBounds = true;
                arrangeSize.Width = effectiveMaxWidth;
            }

            double effectiveMaxHeight = Math.Max(unclippedDesiredSize.Height, mm.maxHeight);
            if (DoubleUtil.LessThan(effectiveMaxHeight, arrangeSize.Height))
            {
                NeedsClipBounds = true;
                arrangeSize.Height = effectiveMaxHeight;
            }

            Size oldRenderSize = RenderSize;
            Size innerInkSize = ArrangeOverride(arrangeSize);

            // Here we use un-clipped InkSize because element does not know that it is
            // clipped by layout system and it shoudl have as much space to render as
            // it returned from its own ArrangeOverride
            RenderSize = innerInkSize;

            // clippedInkSize differs from InkSize only what MaxWidth/Height explicitly clip the
            // otherwise good arrangement. For ex, DS<clientSize but DS>MaxWidth - in this
            // case we should initiate clip at MaxWidth and only show Top-Left portion
            // of the element limited by Max properties. It is Top-left because in case when we
            // are clipped by container we also degrade to Top-Left, so we are consistent.
            Size clippedInkSize = new Size(Math.Min(innerInkSize.Width, mm.maxWidth),
                                           Math.Min(innerInkSize.Height, mm.maxHeight));

            // remember we have to clip if Max properties limit the inkSize
            NeedsClipBounds |=
                    DoubleUtil.LessThan(clippedInkSize.Width, innerInkSize.Width)
                || DoubleUtil.LessThan(clippedInkSize.Height, innerInkSize.Height);

            // if LayoutTransform is set, get the "outer bounds" - the alignments etc work on them
            if (ltd is not null)
            {
                Rect inkRectTransformed = Rect.Transform(
                    new Rect(0, 0, clippedInkSize.Width, clippedInkSize.Height),
                    ltd.Transform);

                clippedInkSize.Width = inkRectTransformed.Width;
                clippedInkSize.Height = inkRectTransformed.Height;
            }

            // Note that inkSize now can be bigger then layoutSlotSize-margin (because of layout
            // squeeze by the parent or LayoutConstrained=true, which clips desired size in Measure).

            // The client size is the size of layout slot decreased by margins.
            // This is the "window" through which we see the content of the child.
            // Alignments position ink of the child in this "window".
            // Max with 0 is neccessary because layout slot may be smaller then unclipped desired size.
            Size clientSize = new Size(Math.Max(0, finalRect.Width - marginWidth),
                                       Math.Max(0, finalRect.Height - marginHeight));

            //remember we have to clip if clientSize limits the inkSize
            NeedsClipBounds |=
                    DoubleUtil.LessThan(clientSize.Width, clippedInkSize.Width)
                || DoubleUtil.LessThan(clientSize.Height, clippedInkSize.Height);

            Vector offset = ComputeAlignmentOffset(clientSize, clippedInkSize);

            offset.X += finalRect.X + margin.Left;
            offset.Y += finalRect.Y + margin.Top;

            SetLayoutOffset(offset, oldRenderSize, ltd);
        }
    }

    /// <summary>
    /// Returns a geometry for a clipping mask. The mask applies if the layout system attempts 
    /// to arrange an element that is larger than the available display space.
    /// </summary>
    /// <param name="layoutSlotSize">
    /// The size of the part of the element that does visual presentation.
    /// </param>
    /// <returns>
    /// The clipping geometry.
    /// </returns>
    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
        if (NeedsClipBounds || ClipToBounds)
        {
            // see if  MaxWidth/MaxHeight limit the element
            var mm = new MinMax(this);

            // this is in element's local rendering coord system
            Size inkSize = RenderSize;

            double maxWidthClip = double.IsPositiveInfinity(mm.maxWidth) ? inkSize.Width : mm.maxWidth;
            double maxHeightClip = double.IsPositiveInfinity(mm.maxHeight) ? inkSize.Height : mm.maxHeight;

            // need to clip because the computed sizes exceed MaxWidth/MaxHeight/Width/Height
            bool needToClipLocally =
                 ClipToBounds //need to clip at bounds even if inkSize is less then maxSize
              || DoubleUtil.LessThan(maxWidthClip, inkSize.Width)
              || DoubleUtil.LessThan(maxHeightClip, inkSize.Height);

            // now lets say we already clipped by MaxWidth/MaxHeight, lets see if further clipping is needed
            inkSize.Width = Math.Min(inkSize.Width, mm.maxWidth);
            inkSize.Height = Math.Min(inkSize.Height, mm.maxHeight);

            // if LayoutTransform is set, convert RenderSize to "outer bounds"
            LayoutTransformData ltd = LayoutTransformData.GetData(this);
            var inkRectTransformed = new Rect();
            if (ltd is not null)
            {
                inkRectTransformed = Rect.Transform(new Rect(0, 0, inkSize.Width, inkSize.Height), ltd.Transform);
                inkSize.Width = inkRectTransformed.Width;
                inkSize.Height = inkRectTransformed.Height;
            }

            // now see if layout slot should clip the element
            Thickness margin = Margin;
            double marginWidth = margin.Left + margin.Right;
            double marginHeight = margin.Top + margin.Bottom;

            var clippingSize = new Size(Math.Max(0, layoutSlotSize.Width - marginWidth),
                                        Math.Max(0, layoutSlotSize.Height - marginHeight));

            bool needToClipSlot =
                ClipToBounds // forces clip at layout slot bounds even if reported sizes are ok
             || DoubleUtil.LessThan(clippingSize.Width, inkSize.Width)
             || DoubleUtil.LessThan(clippingSize.Height, inkSize.Height);

            Matrix? rtlMirror = GetFlowDirectionMatrix();

            if (needToClipSlot)
            {
                Vector offset = ComputeAlignmentOffset(clippingSize, inkSize);

                if (ltd is not null)
                {
                    var slotClipRect = new Rect(-offset.X + inkRectTransformed.X,
                                                -offset.Y + inkRectTransformed.Y,
                                                clippingSize.Width,
                                                clippingSize.Height);

                    var slotClip = new RectangleGeometry(slotClipRect);

                    Matrix layoutTransform = Matrix.Identity;
                    if (ltd.Transform.HasInverse)
                    {
                        layoutTransform = ltd.Transform;
                        layoutTransform.Invert();
                    }

                    if (needToClipLocally)
                    {
                        if (!layoutTransform.IsIdentity)
                        {
                            slotClip.Transform = new MatrixTransform(layoutTransform);
                        }

                        var localClip = new RectangleGeometry(new Rect(0, 0, maxWidthClip, maxHeightClip));
                        var combinedClip = Geometry.Combine(localClip, slotClip, GeometryCombineMode.Intersect, null);
                        if (rtlMirror.HasValue)
                        {
                            combinedClip.Transform = new MatrixTransform(rtlMirror.Value);
                        }

                        return combinedClip;
                    }
                    else
                    {
                        Matrix m = rtlMirror.HasValue ? layoutTransform * rtlMirror.Value : layoutTransform;
                        if (!m.IsIdentity)
                        {
                            slotClip.Transform = new MatrixTransform(m);
                        }
                        
                        return slotClip;
                    }
                }
                else
                {
                    var slotRect = new Rect(-offset.X + inkRectTransformed.X,
                                            -offset.Y + inkRectTransformed.Y,
                                            clippingSize.Width,
                                            clippingSize.Height);

                    if (needToClipLocally) // intersect 2 rects
                    {
                        slotRect.Intersect(new Rect(0, 0, maxWidthClip, maxHeightClip));
                    }

                    var combinedClip = new RectangleGeometry(slotRect);
                    if (rtlMirror.HasValue)
                    {
                        combinedClip.Transform = new MatrixTransform(rtlMirror.Value);
                    }
                    return combinedClip;
                }
            }

            if (needToClipLocally)
            {
                var clipRect = new Rect(0, 0, maxWidthClip, maxHeightClip);
                
                var localClip = new RectangleGeometry(clipRect);
                if (rtlMirror.HasValue)
                {
                    localClip.Transform = new MatrixTransform(rtlMirror.Value);
                }
                return localClip;
            }

            return null;
        }

        return base.GetLayoutClip(layoutSlotSize);
    }

    // see LayoutInformation
    internal Geometry GetLayoutClipInternal()
    {
        if (IsMeasureValid && IsArrangeValid)
        {
            return GetLayoutClip(PreviousArrangeRect.Size);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can
    /// override this method to define their own Arrange pass behavior.
    /// </summary>
    /// <param name="finalSize">
    /// The final area within the parent that this object should use to arrange itself
    /// and its children.
    /// </param>
    /// <returns>
    /// The actual size that is used after the element is arranged in layout.
    /// </returns>
    protected virtual Size ArrangeOverride(Size finalSize) => finalSize;

    /// <summary>
    /// Occurs when the layout of the Silverlight visual tree changes.
    /// </summary>
    public new event EventHandler LayoutUpdated
    {
        add => base.LayoutUpdated += value;
        remove => base.LayoutUpdated -= value;
    }

    /// <summary>
    /// Identifies the <see cref="SizeChanged"/> routed event.
    /// </summary>
    public static readonly RoutedEvent SizeChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(SizeChanged),
            RoutingStrategy.Direct,
            typeof(SizeChangedEventHandler),
            typeof(FrameworkElement));

    /// <summary>
    /// Occurs when either the <see cref="ActualHeight"/> or the <see cref="ActualWidth"/>
    /// properties change value on a <see cref="FrameworkElement"/>.
    /// </summary>
    public event SizeChangedEventHandler SizeChanged
    {
        add => AddHandler(SizeChangedEvent, value, false);
        remove => RemoveHandler(SizeChangedEvent, value);
    }

    /// <summary>
    /// Raises the <see cref="SizeChanged"/> event, using the specified information as part of the eventual event data.
    /// </summary>
    /// <param name="info">
    /// Details of the old and new size involved in the change.
    /// </param>
    protected internal override void OnRenderSizeChanged(SizeChangedInfo info)
    {
        base.OnRenderSizeChanged(info);

        // first, invalidate ActualWidth and/or ActualHeight
        // Note: if any handler of invalidation will dirtyfy layout,
        // subsequent handlers will run on effectively dirty layouts
        // we only guarantee cleaning between elements, not between handlers here
        if (info.WidthChanged)
        {
            NotifyPropertyChange(
                new DependencyPropertyChangedEventArgs(
                    info.PreviousSize.Width,
                    info.NewSize.Width,
                    ActualWidthProperty,
                    _actualWidthMetadata));
        }

        if (info.HeightChanged)
        {
            NotifyPropertyChange(
                new DependencyPropertyChangedEventArgs(
                    info.PreviousSize.Height,
                    info.NewSize.Height,
                    ActualHeightProperty,
                    _actualHeightMetadata));
        }

        RaiseEvent(new SizeChangedEventArgs(info)
        {
            RoutedEvent = SizeChangedEvent
        });
    }

    // Method FindMaximalAreaLocalSpaceRect - used only if LayoutTransform is specified
    // Summary:
    //   Given the transform currently applied to child, this method finds (in
    //     axis-aligned local space) the largest rectangle that, after transform,
    //   fits within transformSpaceBounds.  Largest rectangle means rectangle
    //   of greatest area in local space (although maximal area in local space
    //   implies maximal area in transform space).
    // Parameters:
    //   transformSpaceBounds: the bounds (in destination/transform space) that
    //   the
    // Returns:
    //   The dimensions, in local space, of the maximal area rectangle found.
    private Size FindMaximalAreaLocalSpaceRect(Matrix trMatrix, Size transformSpaceBounds)
    {
        // X (width) and Y (height) constraints for axis-aligned bounding box in dest. space
        double xConstr = transformSpaceBounds.Width;
        double yConstr = transformSpaceBounds.Height;

        //if either of the sizes is 0, return 0,0 to avoid doing math on an empty rect (bug 963569)
        if (DoubleUtil.IsZero(xConstr) || DoubleUtil.IsZero(yConstr))
        {
            return new Size(0, 0);
        }

        bool xConstrInfinite = double.IsInfinity(xConstr);
        bool yConstrInfinite = double.IsInfinity(yConstr);

        if (xConstrInfinite && yConstrInfinite)
        {
            return new Size(double.PositiveInfinity, double.PositiveInfinity);
        }
        else if (xConstrInfinite) //assume square for one-dimensional constraint
        {
            xConstr = yConstr;
        }
        else if (yConstrInfinite)
        {
            yConstr = xConstr;
        }

        // We only deal with nonsingular matrices here. The nonsingular matrix is the one
        // that has inverse (determinant != 0).
        if (!trMatrix.HasInverse)
        {
            return new Size(0, 0);
        }

        double a = trMatrix.M11;
        double b = trMatrix.M12;
        double c = trMatrix.M21;
        double d = trMatrix.M22;

        // Result width and height (in child/local space)
        double w, h;

        // because we are dealing with nonsingular transform matrices,
        // we have (b==0 || c==0) XOR (a==0 || d==0)

        if (DoubleUtil.IsZero(b) || DoubleUtil.IsZero(c))
        {
            // (b==0 || c==0) ==> a!=0 && d!=0

            double yCoverD = yConstrInfinite ? double.PositiveInfinity : Math.Abs(yConstr / d);
            double xCoverA = xConstrInfinite ? double.PositiveInfinity : Math.Abs(xConstr / a);

            if (DoubleUtil.IsZero(b))
            {
                if (DoubleUtil.IsZero(c))
                {
                    // Case: b=0, c=0, a!=0, d!=0

                    // No constraint relation; use maximal width and height

                    h = yCoverD;
                    w = xCoverA;
                }
                else
                {
                    // Case: b==0, a!=0, c!=0, d!=0

                    // Maximizing under line (hIntercept=xConstr/c, wIntercept=xConstr/a)
                    // BUT we still have constraint: h <= yConstr/d

                    h = Math.Min(0.5 * Math.Abs(xConstr / c), yCoverD);
                    w = xCoverA - (c * h / a);
                }
            }
            else
            {
                // Case: c==0, a!=0, b!=0, d!=0

                // Maximizing under line (hIntercept=yConstr/d, wIntercept=yConstr/b)
                // BUT we still have constraint: w <= xConstr/a

                w = Math.Min(0.5 * Math.Abs(yConstr / b), xCoverA);
                h = yCoverD - (b * w / d);
            }
        }
        else if (DoubleUtil.IsZero(a) || DoubleUtil.IsZero(d))
        {
            // (a==0 || d==0) ==> b!=0 && c!=0

            double yCoverB = Math.Abs(yConstr / b);
            double xCoverC = Math.Abs(xConstr / c);

            if (DoubleUtil.IsZero(a))
            {
                if (DoubleUtil.IsZero(d))
                {
                    // Case: a=0, d=0, b!=0, c!=0

                    // No constraint relation; use maximal width and height

                    h = xCoverC;
                    w = yCoverB;
                }
                else
                {
                    // Case: a==0, b!=0, c!=0, d!=0

                    // Maximizing under line (hIntercept=yConstr/d, wIntercept=yConstr/b)
                    // BUT we still have constraint: h <= xConstr/c

                    h = Math.Min(0.5 * Math.Abs(yConstr / d), xCoverC);
                    w = yCoverB - (d * h / b);
                }
            }
            else
            {
                // Case: d==0, a!=0, b!=0, c!=0

                // Maximizing under line (hIntercept=xConstr/c, wIntercept=xConstr/a)
                // BUT we still have constraint: w <= yConstr/b

                w = Math.Min(0.5 * Math.Abs(xConstr / a), yCoverB);
                h = xCoverC - (a * w / c);
            }
        }
        else
        {
            double xCoverA = Math.Abs(xConstr / a);        // w-intercept of x-constraint line.
            double xCoverC = Math.Abs(xConstr / c);        // h-intercept of x-constraint line.

            double yCoverB = Math.Abs(yConstr / b);        // w-intercept of y-constraint line.
            double yCoverD = Math.Abs(yConstr / d);        // h-intercept of y-constraint line.

            // The tighest constraint governs, so we pick the lowest constraint line.
            //
            //   The optimal point (w,h) for which Area = w*h is maximized occurs halfway
            //   to each intercept.

            w = Math.Min(yCoverB, xCoverA) * 0.5;
            h = Math.Min(xCoverC, yCoverD) * 0.5;

            if ((DoubleUtil.GreaterThanOrClose(xCoverA, yCoverB) && DoubleUtil.LessThanOrClose(xCoverC, yCoverD)) ||
                (DoubleUtil.LessThanOrClose(xCoverA, yCoverB) && DoubleUtil.GreaterThanOrClose(xCoverC, yCoverD)))
            {
                // Constraint lines cross; since the most restrictive constraint wins,
                // we have to maximize under two line segments, which together are discontinuous.
                // Instead, we maximize w*h under the line segment from the two smallest endpoints.

                // Since we are not (except for in corner cases) on the original constraint lines,
                // we are not using up all the available area in transform space.  So scale our shape up
                // until it does in at least one dimension.

                Rect childBoundsTr = Rect.Transform(new Rect(0, 0, w, h), trMatrix);
                double expandFactor = Math.Min(xConstr / childBoundsTr.Width, yConstr / childBoundsTr.Height);

                if (!double.IsNaN(expandFactor) && !double.IsInfinity(expandFactor))
                {
                    w *= expandFactor;
                    h *= expandFactor;
                }
            }
        }

        return new Size(w, h);
    }

    private Vector ComputeAlignmentOffset(Size clientSize, Size inkSize)
    {
        var offset = new Vector();

        HorizontalAlignment ha = HorizontalAlignment;
        VerticalAlignment va = VerticalAlignment;

        //this is to degenerate Stretch to Top-Left in case when clipping is about to occur
        //if we need it to be Center instead, simply remove these 2 ifs
        if (ha == HorizontalAlignment.Stretch
            && inkSize.Width > clientSize.Width)
        {
            ha = HorizontalAlignment.Left;
        }

        if (va == VerticalAlignment.Stretch
            && inkSize.Height > clientSize.Height)
        {
            va = VerticalAlignment.Top;
        }
        //end of degeneration of Stretch to Top-Left

        if (ha == HorizontalAlignment.Center
            || ha == HorizontalAlignment.Stretch)
        {
            offset.X = (clientSize.Width - inkSize.Width) * 0.5;
        }
        else if (ha == HorizontalAlignment.Right)
        {
            offset.X = clientSize.Width - inkSize.Width;
        }
        else
        {
            offset.X = 0;
        }

        if (va == VerticalAlignment.Center
            || va == VerticalAlignment.Stretch)
        {
            offset.Y = (clientSize.Height - inkSize.Height) * 0.5;
        }
        else if (va == VerticalAlignment.Bottom)
        {
            offset.Y = clientSize.Height - inkSize.Height;
        }
        else
        {
            offset.Y = 0;
        }

        return offset;
    }

    /// <summary>
    /// This is the method layout parent uses to set a location of the child
    /// relative to parent's visual as a result of layout. Typically, this is called
    /// by the parent inside of its ArrangeOverride implementation after calling Arrange on a child.
    /// </summary>
    private void SetLayoutOffset(Vector offset, Size oldRenderSize, LayoutTransformData ltd)
    {
        if (!AreTransformsClean || !DoubleUtil.AreClose(RenderSize, oldRenderSize))
        {
            Transform additionalTransform = GetFlowDirectionTransform(); // rtl
            Transform renderTransform = (Transform)GetValue(RenderTransformProperty);
            if (renderTransform == Transform.Identity)
            {
                renderTransform = null;
            }

            TransformGroup t = null;

            // arbitrary transform, create a collection
            if (additionalTransform is not null || renderTransform is not null || ltd is not null)
            {
                // Create a TransformGroup and make sure it does not participate
                // in the InheritanceContext treeness because it is internal operation only.
                t = new TransformGroup();
                t.CanBeInheritanceContext = false;
                t.Children.CanBeInheritanceContext = false;

                if (additionalTransform is not null)
                {
                    t.Children.Add(additionalTransform);
                }

                if (ltd is not null)
                {
                    t.Children.Add(new MatrixTransform(ltd.Transform));

                    // see if MaxWidth/MaxHeight limit the element
                    var mm = new MinMax(this);

                    // this is in element's local rendering coord system
                    Size inkSize = RenderSize;

                    // get the size clipped by the MaxWidth/MaxHeight/Width/Height
                    inkSize.Width = Math.Min(inkSize.Width, mm.maxWidth);
                    inkSize.Height = Math.Min(inkSize.Height, mm.maxHeight);

                    Rect inkRectTransformed = Rect.Transform(new Rect(inkSize), ltd.Transform);

                    t.Children.Add(new TranslateTransform(-inkRectTransformed.X, -inkRectTransformed.Y));
                }

                if (renderTransform is not null)
                {
                    Point origin = GetRenderTransformOrigin();
                    bool hasOrigin = origin.X != 0d || origin.Y != 0d;
                    if (hasOrigin)
                    {
                        var backOrigin = new TranslateTransform(-origin.X, -origin.Y);
                        backOrigin.Seal();
                        t.Children.Add(backOrigin);
                    }

                    //can not freeze render transform - it can be animated
                    t.Children.Add(renderTransform);

                    if (hasOrigin)
                    {
                        var forwardOrigin = new TranslateTransform(origin.X, origin.Y);
                        forwardOrigin.Seal();
                        t.Children.Add(forwardOrigin);
                    }
                }
            }

            VisualTransform = t;
            AreTransformsClean = true;
        }

        VisualOffset = offset;
    }

    private Point GetRenderTransformOrigin()
    {
        Point relativeOrigin = RenderTransformOrigin;
        Size renderSize = RenderSize;
        return new Point(renderSize.Width * relativeOrigin.X, renderSize.Height * relativeOrigin.Y);
    }

    private Matrix? GetFlowDirectionMatrix()
    {
        if (ShouldApplyMirrorTransform())
        {
            return new Matrix(-1.0, 0.0, 0.0, 1.0, RenderSize.Width, 0.0);
        }

        return null;
    }

    private Transform GetFlowDirectionTransform()
    {
        if (GetFlowDirectionMatrix() is Matrix matrix)
        {
            return new MatrixTransform(matrix);
        }
        return null;
    }

    internal virtual bool ShouldApplyMirrorTransform()
    {
        FlowDirection thisFlowDirection = FlowDirection;

        // If the element is connected to visual tree, get FlowDirection
        // from its visual parent.
        FlowDirection parentFlowDirection = GetFlowDirectionFromVisual(VisualTreeHelper.GetParent(this));

        //  if direction changes, instantiate a mirroring transform
        return ApplyMirrorTransform(parentFlowDirection, thisFlowDirection);
    }

    internal static FlowDirection GetFlowDirectionFromVisual(DependencyObject visual)
    {
        return visual is FrameworkElement fe ? fe.FlowDirection : FlowDirection.LeftToRight;
    }

    private static bool ApplyMirrorTransform(FlowDirection parentFD, FlowDirection thisFD)
    {
        return (parentFD == FlowDirection.LeftToRight && thisFD == FlowDirection.RightToLeft) ||
               (parentFD == FlowDirection.RightToLeft && thisFD == FlowDirection.LeftToRight);
    }

    private bool NeedsClipBounds
    {
        get => ReadInternalFlag(InternalFlags.NeedsClipBounds);
        set => WriteInternalFlag(InternalFlags.NeedsClipBounds, value);
    }

    private bool HasLayoutTransformData
    {
        get => ReadInternalFlag(InternalFlags.HasLayoutTransformData);
        set => WriteInternalFlag(InternalFlags.HasLayoutTransformData, value);
    }

    private Size _unclippedDesiredSize;

    private struct MinMax
    {
        internal MinMax(FrameworkElement e)
        {
            maxHeight = e.MaxHeight;
            minHeight = e.MinHeight;
            double l = e.Height;

            double height = (double.IsNaN(l) ? double.PositiveInfinity : l);
            maxHeight = Math.Max(Math.Min(height, maxHeight), minHeight);

            height = (double.IsNaN(l) ? 0 : l);
            minHeight = Math.Max(Math.Min(maxHeight, height), minHeight);

            maxWidth = e.MaxWidth;
            minWidth = e.MinWidth;
            l = e.Width;

            double width = (double.IsNaN(l) ? double.PositiveInfinity : l);
            maxWidth = Math.Max(Math.Min(width, maxWidth), minWidth);

            width = (double.IsNaN(l) ? 0 : l);
            minWidth = Math.Max(Math.Min(maxWidth, width), minWidth);
        }

        internal double minWidth;
        internal double maxWidth;
        internal double minHeight;
        internal double maxHeight;
    }

    // LayoutTransform property may be animated and its value change in time,
    // LayoutTransformData is used to store a snapshot of LayoutTransform
    // property value to avoid layout / render inconsistencies caused by
    // animated LayoutTransforms...
    private sealed class LayoutTransformData
    {
        private static readonly UncommonField<LayoutTransformData> LayoutTransformDataField = new();

        internal static LayoutTransformData GetData(FrameworkElement fe)
        {
            if (fe.HasLayoutTransformData)
            {
                return LayoutTransformDataField.GetValue(fe);
            }

            return null;
        }

        internal static LayoutTransformData CreateData(FrameworkElement fe)
        {
            Debug.Assert(!fe.HasLayoutTransformData);

            fe.HasLayoutTransformData = true;
            var data = new LayoutTransformData();
            LayoutTransformDataField.SetValue(fe, data);
            return data;
        }

        internal static void ClearData(FrameworkElement fe)
        {
            Debug.Assert(fe.HasLayoutTransformData);

            fe.HasLayoutTransformData = false;
            LayoutTransformDataField.ClearValue(fe);
        }

        private LayoutTransformData() { }

        internal Size UntransformedDS;
        internal Matrix Transform;

        internal void CreateTransformSnapshot(Transform sourceTransform)
        {
            Debug.Assert(sourceTransform is not null);
            Transform = sourceTransform.Matrix;
        }
    }

    /// <summary>
    /// Identifies the <see cref="CustomLayout"/> dependency property.
    /// </summary>
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DependencyProperty CustomLayoutProperty =
        DependencyProperty.Register(
            nameof(CustomLayout),
            typeof(bool),
            typeof(FrameworkElement),
            new PropertyMetadata(true));

    /// <summary>
    /// Enable or disable measure/arrange layout system in a sub part
    /// </summary>
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool CustomLayout
    {
        get => (bool)GetValue(CustomLayoutProperty);
        set => SetValueInternal(CustomLayoutProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsAutoWidthOnCustomLayout"/> dependency property.
    /// </summary>
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DependencyProperty IsAutoWidthOnCustomLayoutProperty =
        DependencyProperty.Register(
            nameof(IsAutoWidthOnCustomLayout),
            typeof(bool?),
            typeof(FrameworkElement),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the Auto Width to the root of CustomLayout
    /// </summary>
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool? IsAutoWidthOnCustomLayout
    {
        get => (bool?)GetValue(IsAutoWidthOnCustomLayoutProperty);
        set => SetValueInternal(IsAutoWidthOnCustomLayoutProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="IsAutoHeightOnCustomLayout"/> dependency property.
    /// </summary>
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly DependencyProperty IsAutoHeightOnCustomLayoutProperty =
        DependencyProperty.Register(
            nameof(IsAutoHeightOnCustomLayout),
            typeof(bool?),
            typeof(FrameworkElement),
            new PropertyMetadata((object)null));

    /// <summary>
    /// Gets or sets the Auto Height to the root of CustomLayout
    /// </summary>
    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool? IsAutoHeightOnCustomLayout
    {
        get => (bool?)GetValue(IsAutoHeightOnCustomLayoutProperty);
        set => SetValueInternal(IsAutoHeightOnCustomLayoutProperty, value);
    }

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnAfterApplyHorizontalAlignmentAndWidth() { }

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnAfterApplyVerticalAlignmentAndWidth() { }
}

// LayoutDoubleUtil, uses fixed eps unlike DoubleUtil which uses relative one.
// This is more suitable for some layout comparisons because the computation
// paths in layout may easily be quite long so DoubleUtil method gives a lot of false
// results, while bigger absolute deviation is normally harmless in layout.
// Note that FP noise is a big problem and using any of these compare methods is
// not a complete solution, but rather the way to reduce the probability
// of the dramatically bad-looking results.
internal static class LayoutDoubleUtil
{
    private const double eps = 0.00000153; // more or less random more or less small number

    internal static bool AreClose(double value1, double value2)
    {
        if (value1 == value2) return true;

        double diff = value1 - value2;
        return (diff < eps) && (diff > -eps);
    }

    internal static bool LessThan(double value1, double value2)
    {
        return (value1 < value2) && !AreClose(value1, value2);
    }
}