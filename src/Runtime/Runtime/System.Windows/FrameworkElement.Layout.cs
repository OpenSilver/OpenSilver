
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
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class FrameworkElement
    {
        /// <summary>
        /// Identifies the <see cref="CustomLayout"/> dependency property.
        /// </summary>
        [Obsolete]
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
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CustomLayout
        {
            get => (bool)GetValue(CustomLayoutProperty);
            set => SetValue(CustomLayoutProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsAutoWidthOnCustomLayout"/> dependency property.
        /// </summary>
        [Obsolete]
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
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool? IsAutoWidthOnCustomLayout
        {
            get => (bool?)GetValue(IsAutoWidthOnCustomLayoutProperty);
            set => SetValue(IsAutoWidthOnCustomLayoutProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsAutoHeightOnCustomLayout"/> dependency property.
        /// </summary>
        [Obsolete]
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
        [Obsolete]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool? IsAutoHeightOnCustomLayout
        {
            get => (bool?)GetValue(IsAutoHeightOnCustomLayoutProperty);
            set => SetValue(IsAutoHeightOnCustomLayoutProperty, value);
        }

        internal sealed override Size MeasureCore(Size availableSize)
        {
            //build the visual tree from styles first
            if (!ApplyTemplate() && TemplateChild is not null)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(TemplateChild, this, 0);
            }

            bool isLayoutRoot = IsLayoutRoot;

            Thickness margin = Margin;
            double marginWidth = isLayoutRoot ? 0 : margin.Left + margin.Right;
            double marginHeight = isLayoutRoot ? 0 : margin.Top + margin.Bottom;

            //  parent size is what parent want us to be
            Size frameworkAvailableSize = new Size(
                Math.Max(availableSize.Width - marginWidth, 0),
                Math.Max(availableSize.Height - marginHeight, 0));

            MinMax mm = new MinMax(this);

            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            //  call to specific layout to measure
            Size desiredSize = MeasureOverride(frameworkAvailableSize);

            //  maximize desiredSize with user provided min size
            desiredSize = new Size(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            //here is the "true minimum" desired size - the one that is
            //for sure enough for the control to render its content.
            Size unclippedDesiredSize = desiredSize;

            bool clipped = false;

            // User-specified max size starts to "clip" the control here.
            //Starting from this point desiredSize could be smaller then actually
            //needed to render the whole control
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

            //  because of negative margins, clipped desired size may be negative.
            //  need to keep it as doubles for that reason and maximize with 0 at the
            //  very last point - before returning desired size to the parent.
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

            //  Note: unclippedDesiredSize is needed in ArrangeCore,
            //  because due to the layout protocol, arrange should be called
            //  with constraints greater or equal to child's desired size
            //  returned from MeasureOverride. But in most circumstances
            //  it is possible to reconstruct original unclipped desired size.
            //  In such cases we want to optimize space and save 16 bytes by
            //  not storing it on each FrameworkElement.
            //
            //  The if statement conditions below lists the cases when
            //  it is NOT possible to recalculate unclipped desired size later
            //  in ArrangeCore, thus we save it...
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

        internal sealed override void ArrangeCore(Rect finalRect)
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

            bool isLayoutRoot = IsLayoutRoot;

            Thickness margin = Margin;
            double marginWidth = isLayoutRoot ? 0 : margin.Left + margin.Right;
            double marginHeight = isLayoutRoot ? 0 : margin.Top + margin.Bottom;
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

            MinMax mm = new MinMax(this);

            //we have to choose max between UnclippedDesiredSize and Max here, because
            //otherwise setting of max property could cause arrange at less then unclippedDS.
            //Clipping by Max is needed to limit stretch here
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

            //Here we use un-clipped InkSize because element does not know that it is
            //clipped by layout system and it shoudl have as much space to render as
            //it returned from its own ArrangeOverride
            RenderSize = innerInkSize;

            //clippedInkSize differs from InkSize only what MaxWidth/Height explicitly clip the
            //otherwise good arrangement. For ex, DS<clientSize but DS>MaxWidth - in this
            //case we should initiate clip at MaxWidth and only show Top-Left portion
            //of the element limited by Max properties. It is Top-left because in case when we
            //are clipped by container we also degrade to Top-Left, so we are consistent.
            Size clippedInkSize = new Size(Math.Min(innerInkSize.Width, mm.maxWidth),
                                           Math.Min(innerInkSize.Height, mm.maxHeight));

            //remember we have to clip if Max properties limit the inkSize
            NeedsClipBounds |=
                    DoubleUtil.LessThan(clippedInkSize.Width, innerInkSize.Width)
                || DoubleUtil.LessThan(clippedInkSize.Height, innerInkSize.Height);

            //Note that inkSize now can be bigger then layoutSlotSize-margin (because of layout
            //squeeze by the parent or LayoutConstrained=true, which clips desired size in Measure).

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

            Point offset = isLayoutRoot ? new Point() : ComputeAlignmentOffset(clientSize, clippedInkSize);

            offset.X += finalRect.X + margin.Left;
            offset.Y += finalRect.Y + margin.Top;

            SetLayoutOffset(offset, oldRenderSize);

            if (IsFirstRendering)
            {
                IsFirstRendering = false;
                INTERNAL_HtmlDomManager.GetDomElementStyleForModification(INTERNAL_OuterDomElement).visibility = "visible";
            }
        }

        internal override Rect? GetLayoutClip(Size layoutSlotSize)
        {
            if (NeedsClipBounds || ClipToBounds)
            {
                // see if  MaxWidth/MaxHeight limit the element
                MinMax mm = new MinMax(this);

                //this is in element's local rendering coord system
                Size inkSize = RenderSize;

                double maxWidthClip = double.IsPositiveInfinity(mm.maxWidth) ? inkSize.Width : mm.maxWidth;
                double maxHeightClip = double.IsPositiveInfinity(mm.maxHeight) ? inkSize.Height : mm.maxHeight;

                //need to clip because the computed sizes exceed MaxWidth/MaxHeight/Width/Height
                bool needToClipLocally =
                     ClipToBounds //need to clip at bounds even if inkSize is less then maxSize
                  || DoubleUtil.LessThan(maxWidthClip, inkSize.Width)
                  || DoubleUtil.LessThan(maxHeightClip, inkSize.Height);

                //now lets say we already clipped by MaxWidth/MaxHeight, lets see if further clipping is needed
                inkSize.Width = Math.Min(inkSize.Width, mm.maxWidth);
                inkSize.Height = Math.Min(inkSize.Height, mm.maxHeight);

                //now see if layout slot should clip the element
                Thickness margin = Margin;
                double marginWidth = margin.Left + margin.Right;
                double marginHeight = margin.Top + margin.Bottom;

                Size clippingSize = new Size(Math.Max(0, layoutSlotSize.Width - marginWidth),
                                             Math.Max(0, layoutSlotSize.Height - marginHeight));

                bool needToClipSlot =
                    ClipToBounds //forces clip at layout slot bounds even if reported sizes are ok
                 || DoubleUtil.LessThan(clippingSize.Width, inkSize.Width)
                 || DoubleUtil.LessThan(clippingSize.Height, inkSize.Height);

                if (needToClipSlot)
                {
                    Point offset = VisualOffset;

                    double left, top, width, height;
                    if (offset.X < 0)
                    {
                        left = -offset.X;
                        width = clippingSize.Width - offset.X;
                    }
                    else
                    {
                        left = 0;
                        width = clippingSize.Width;
                    }
                    if (offset.Y < 0)
                    {
                        top = -offset.Y;
                        height = clippingSize.Height - offset.Y;
                    }
                    else
                    {
                        top = 0;
                        height = clippingSize.Height;
                    }

                    Rect slotRect = new Rect(left, top, width, height);

                    if (needToClipLocally) //intersect 2 rects
                    {
                        slotRect.Intersect(new Rect(0, 0, maxWidthClip, maxHeightClip));
                    }

                    return slotRect;
                }

                if (needToClipLocally)
                {
                    return new Rect(0, 0, maxWidthClip, maxHeightClip);
                }

                return null;
            }

            return base.GetLayoutClip(layoutSlotSize);
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
        public event EventHandler LayoutUpdated
        {
            add
            {
                LayoutEventList.ListItem item = GetLayoutUpdatedHandler(value);

                if (item == null)
                {
                    //set a weak ref in LM
                    item = LayoutManager.Current.LayoutEvents.Add(value);
                    AddLayoutUpdatedHandler(value, item);
                }
            }
            remove
            {
                LayoutEventList.ListItem item = GetLayoutUpdatedHandler(value);

                if (item != null)
                {
                    RemoveLayoutUpdatedHandler(value);
                    //remove a weak ref from LM
                    LayoutManager.Current.LayoutEvents.Remove(item);
                }
            }
        }

        private static readonly DependencyProperty LayoutUpdatedListItemsField =
            DependencyProperty.Register(
                "_LayoutUpdatedListItems",
                typeof(object),
                typeof(FrameworkElement),
                null);

        private static readonly DependencyProperty LayoutUpdatedHandlersField =
            DependencyProperty.Register(
                "_LayoutUpdatedHandlers",
                typeof(EventHandler),
                typeof(FrameworkElement),
                null);

        private void AddLayoutUpdatedHandler(EventHandler handler, LayoutEventList.ListItem item)
        {
            object cachedLayoutUpdatedItems = GetValue(LayoutUpdatedListItemsField);

            if (cachedLayoutUpdatedItems == null)
            {
                SetValue(LayoutUpdatedListItemsField, item);
                SetValue(LayoutUpdatedHandlersField, handler);
            }
            else
            {
                EventHandler cachedLayoutUpdatedHandler = (EventHandler)GetValue(LayoutUpdatedHandlersField);
                if (cachedLayoutUpdatedHandler != null)
                {
                    //second unique handler is coming in.
                    //allocate a datastructure
                    var list = new Dictionary<EventHandler, object>(2)
                    {
                        //add previously cached handler
                        { cachedLayoutUpdatedHandler, cachedLayoutUpdatedItems },

                        //add new handler
                        { handler, item }
                    };

                    ClearValue(LayoutUpdatedHandlersField);
                    SetValue(LayoutUpdatedListItemsField, list);
                }
                else //already have a list
                {
                    var list = (Dictionary<EventHandler, object>)cachedLayoutUpdatedItems;
                    list.Add(handler, item);
                }
            }
        }

        private LayoutEventList.ListItem GetLayoutUpdatedHandler(EventHandler d)
        {
            object cachedLayoutUpdatedItems = GetValue(LayoutUpdatedListItemsField);

            if (cachedLayoutUpdatedItems == null)
            {
                return null;
            }
            else
            {
                EventHandler cachedLayoutUpdatedHandler = (EventHandler)GetValue(LayoutUpdatedHandlersField);
                if (cachedLayoutUpdatedHandler != null)
                {
                    if (cachedLayoutUpdatedHandler == d) return (LayoutEventList.ListItem)cachedLayoutUpdatedItems;
                }
                else //already have a list
                {
                    var list = (Dictionary<EventHandler, object>)cachedLayoutUpdatedItems;
                    LayoutEventList.ListItem item = (LayoutEventList.ListItem)list[d];
                    return item;
                }
                return null;
            }
        }

        private void RemoveLayoutUpdatedHandler(EventHandler d)
        {
            object cachedLayoutUpdatedItems = GetValue(LayoutUpdatedListItemsField);
            EventHandler cachedLayoutUpdatedHandler = (EventHandler)GetValue(LayoutUpdatedHandlersField);

            if (cachedLayoutUpdatedHandler != null) //single handler
            {
                if (cachedLayoutUpdatedHandler == d)
                {
                    ClearValue(LayoutUpdatedListItemsField);
                    ClearValue(LayoutUpdatedHandlersField);
                }
            }
            else //there is an ArrayList allocated
            {
                var list = (Dictionary<EventHandler, object>)cachedLayoutUpdatedItems;
                list.Remove(d);
            }
        }

        /// <summary>
        /// Occurs when either the <see cref="ActualHeight"/> or the <see cref="ActualWidth"/>
        /// properties change value on a <see cref="FrameworkElement"/>.
        /// </summary>
        public event SizeChangedEventHandler SizeChanged;

        internal sealed override void OnRenderSizeChanged(SizeChangedInfo info)
        {
            SizeChanged?.Invoke(this, new SizeChangedEventArgs(info.PreviousSize, info.NewSize));
        }

        private Point ComputeAlignmentOffset(Size clientSize, Size inkSize)
        {
            Point offset = new Point();

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
        private void SetLayoutOffset(Point offset, Size oldRenderSize)
        {
            VisualOffset = offset;
        }

        private bool NeedsClipBounds
        {
            get { return ReadInternalFlag(InternalFlags.NeedsClipBounds); }
            set { WriteInternalFlag(InternalFlags.NeedsClipBounds, value); }
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
    }
}
