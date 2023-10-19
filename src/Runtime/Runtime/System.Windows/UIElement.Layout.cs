
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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using CSHTML5.Internal;
using OpenSilver.Internal;

namespace System.Windows
{
    public partial class UIElement
    {
        private Size _desiredSize;

        /// <summary>
        /// Gets the final render size of a <see cref="UIElement"/>.
        /// </summary>
        /// <returns>
        /// The rendered size for this object. There is no default value.
        /// </returns>
        public Size RenderSize { get; internal set; }

        /// <summary>
        /// Gets the size that this <see cref="UIElement"/> computed during the measure
        /// pass of the layout process.
        /// </summary>
        /// <returns>
        /// The size that this <see cref="UIElement"/> computed during the measure pass
        /// of the layout process.
        /// </returns>
        public Size DesiredSize => IsVisible ? _desiredSize : new Size();

        /// <summary>
        /// Invalidates the measurement state (layout) for a <see cref="UIElement"/>.
        /// </summary>
        public void InvalidateMeasure()
        {
            if (!MeasureDirty && !MeasureInProgress)
            {
                if (!NeverMeasured)
                {
                    LayoutManager.Current.MeasureQueue.Add(this);
                }

                MeasureDirty = true;
            }
        }

        internal void InvalidateMeasureInternal()
        {
            MeasureDirty = true;
        }

        /// <summary>
        /// Invalidates the arrange state (layout) for a <see cref="UIElement"/>. After
        /// the invalidation, the <see cref="UIElement"/> will have its layout updated,
        /// which will occur asynchronously.
        /// </summary>
        public void InvalidateArrange()
        {
            if (!ArrangeDirty && !ArrangeInProgress)
            {
                if (!NeverArranged)
                {
                    LayoutManager.Current.ArrangeQueue.Add(this);
                }

                ArrangeDirty = true;
            }
        }

        internal void InvalidateArrangeInternal()
        {
            ArrangeDirty = true;
        }

        /// <summary>
        /// Invalidates the rendering of the element.
        /// </summary>
        internal void InvalidateVisual()
        {
            InvalidateArrange();
            RenderingInvalidated = true;
        }

        /// <summary>
        /// Updates the <see cref="DesiredSize"/> of a <see cref="UIElement"/>.
        /// Typically, objects that implement custom layout for their layout children call
        /// this method from their own <see cref="FrameworkElement.MeasureOverride(Size)"/>
        /// implementations to form a recursive layout update.
        /// </summary>
        /// <param name="availableSize">
        /// The available space that a parent can allocate a child object. A child object
        /// can request a larger space than what is available; the provided size might be
        /// accommodated if scrolling or other resize behavior is possible in that particular
        /// container.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// availableSize contained a <see cref="double.NaN"/> value. See Remarks.
        /// </exception>
        /// <remarks>
        /// There is no reason to call <see cref="Measure"/> or <see cref="Arrange"/> outside 
        /// of the context of overriding methods that perform custom layout actions. Silverlight 
        /// layout works autonomously, based on detecting changes to the object tree and layout-relevant 
        /// properties at run time.
        /// The availableSize you pass to <see cref="Measure"/> cannot have a <see cref="double.NaN"/> 
        /// value for either the <see cref="Size.Height"/> or <see cref="Size.Width"/> of the <see cref="Size"/>.
        /// availableSize values can be any number from zero to infinite. Elements participating in 
        /// layout should return the minimum <see cref="Size"/> they require for a given availableSize.
        /// </remarks>
        public void Measure(Size availableSize)
        {
            if (INTERNAL_OuterDomElement == null)
            {
                if (MeasureRequest != null)
                    LayoutManager.Current.MeasureQueue.Remove(this);
                MeasureDirty = false;
                return;
            }

            using (Dispatcher.DisableProcessing())
            {
                //enforce that Measure can not receive NaN size .
                if (double.IsNaN(availableSize.Width) || double.IsNaN(availableSize.Height))
                    throw new InvalidOperationException(
                        "UIElement.Measure(availableSize) cannot be called with NaN size.");

                bool neverMeasured = NeverMeasured;
                
                if (neverMeasured)
                {
                    SwitchVisibilityIfNeeded(IsVisible);
                }

                bool isCloseToPreviousMeasure = DoubleUtil.AreClose(availableSize, PreviousAvailableSize);

                if (!IsVisible || ReadVisualFlag(VisualFlags.IsLayoutSuspended))
                {
                    //reset measure request.
                    if (MeasureRequest != null)
                    {
                        LayoutManager.Current.MeasureQueue.Remove(this);
                    }

                    //  remember though that parent tried to measure at this size
                    //  in case when later this element is called to measure incrementally
                    //  it has up-to-date information stored in PreviousAvailableSize
                    if (!isCloseToPreviousMeasure)
                    {
                        //this will ensure that element will be actually re-measured at the new available size
                        //later when it becomes visible.
                        InvalidateMeasureInternal();

                        PreviousAvailableSize = availableSize;
                    }

                    return;
                }
                
                if (IsMeasureValid && !neverMeasured && isCloseToPreviousMeasure)
                {
                    return;
                }

                NeverMeasured = false;
                Size prevSize = _desiredSize;

                //we always want to be arranged, ensure arrange request
                InvalidateArrange();
                
                MeasureInProgress = true;

                Size desiredSize = new Size(0, 0);

                LayoutManager layoutManager = LayoutManager.Current;

                bool gotException = true;

                try
                {
                    layoutManager.EnterMeasure();
                    desiredSize = MeasureCore(availableSize);

                    gotException = false;
                }
                finally
                {
                    MeasureInProgress = false;

                    PreviousAvailableSize = availableSize;

                    layoutManager.ExitMeasure();

                    if (gotException)
                    {
                        // we don't want to reset last exception element on layoutManager if it's been already set.
                        if (layoutManager.GetLastExceptionElement() == null)
                        {
                            layoutManager.SetLastExceptionElement(this);
                        }
                    }
                }

                //enforce that MeasureCore can not return PositiveInfinity size even if given Infinte availabel size.
                //Note: NegativeInfinity can not be returned by definition of Size structure.
                if (double.IsPositiveInfinity(desiredSize.Width) || double.IsPositiveInfinity(desiredSize.Height))
                    throw new InvalidOperationException(string.Format(
                        "Layout measurement override of element '{0}' should not return PositiveInfinity as its DesiredSize, even if Infinity is passed in as available size.",
                        GetType().FullName));

                //enforce that MeasureCore can not return NaN size .
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height))
                    throw new InvalidOperationException(string.Format(
                        "Layout measurement override of element '{0}' should not return NaN values as its DesiredSize.",
                        GetType().FullName));

                //reset measure dirtiness
                
                MeasureDirty = false;

                //reset measure request.
                if (MeasureRequest != null)
                {
                    LayoutManager.Current.MeasureQueue.Remove(this);
                }

                _desiredSize = desiredSize;

                if (!MeasureDuringArrange && !DoubleUtil.AreClose(prevSize, desiredSize))
                {
                    UIElement parent = GetLayoutParent(this);
                    if (parent != null && !parent.MeasureInProgress)
                    {
                        parent.InvalidateMeasure();
                    }
                }
            }
        }

        internal virtual Size MeasureCore(Size availableSize)
        {
            return new Size(0, 0);
        }

        /// <summary>
        /// Positions child objects and determines a size for a <see cref="UIElement"/>.
        /// Parent objects that implement custom layout for their child elements should call
        /// this method from their layout override implementations to form a recursive layout
        /// update.
        /// </summary>
        /// <param name="finalRect">
        /// The final size that the parent computes for the child in layout, provided as
        /// a <see cref="Rect"/> value.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// finalRect contained a <see cref="double.NaN"/> or infinite value. See Remarks.
        /// </exception>
        /// <remarks>
        /// There is no reason to call <see cref="Measure"/> or <see cref="Arrange"/> outside of 
        /// the context of overriding methods that perform custom layout actions. Silverlight 
        /// layout works autonomously, based on detecting changes to the object tree and 
        /// layout-relevant properties at run time. The finalRect you pass to <see cref="Measure"/> 
        /// cannot have a <see cref="double.NaN"/> value for any <see cref="Rect"/> value. Also, 
        /// finalRect cannot have any infinite values for any <see cref="Rect"/> value.
        /// Typically, calls to <see cref="Arrange"/> will incorporate a finalRect that uses the 
        /// height and width values from <see cref="DesiredSize"/> for each element . Exceptions 
        /// to this typical behavior might be necessary if an element holds a <see cref="DesiredSize"/> 
        /// that the layout parent cannot accommodate, or if the sum total of all child element 
        /// <see cref="DesiredSize"/> values cannot be accommodated or arranged. In such cases the 
        /// child element content might be clipped, resized, or placed in a scroll region, which all 
        /// depends on the specific functionality that is enabled in the layout parent container.
        /// </remarks>
        public void Arrange(Rect finalRect)
        {
            if (INTERNAL_OuterDomElement == null)
            {
                if (ArrangeRequest != null)
                    LayoutManager.Current.ArrangeQueue.Remove(this);
                ArrangeDirty = false;
                return;
            }

            using (Dispatcher.DisableProcessing())
            {
                //enforce that Arrange can not come with Infinity size or NaN
                if (double.IsPositiveInfinity(finalRect.Width)
                    || double.IsPositiveInfinity(finalRect.Height)
                    || double.IsNaN(finalRect.Width)
                    || double.IsNaN(finalRect.Height))
                {
                    DependencyObject parent = GetLayoutParent(this);
                    throw new InvalidOperationException(string.Format(
                        "Cannot call Arrange on a UIElement with infinite size or NaN. Parent of type '{0}' invokes the UIElement. Arrange called on element of type '{1}'.",
                        parent == null ? string.Empty : parent.GetType().FullName,
                        GetType().FullName));
                }

                if (!IsVisible || ReadVisualFlag(VisualFlags.IsLayoutSuspended))
                {
                    //reset arrange request.
                    if (ArrangeRequest != null)
                    {
                        LayoutManager.Current.ArrangeQueue.Remove(this);
                    }

                    //  remember though that parent tried to arrange at this rect
                    //  in case when later this element is called to arrange incrementally
                    //  it has up-to-date information stored in _finalRect
                    PreviousArrangeRect = finalRect;

                    return;
                }

                if (MeasureDirty || NeverMeasured)
                {
                    try
                    {
                        MeasureDuringArrange = true;
                        //If never measured - that means "set size", arrange-only scenario
                        //Otherwise - the parent previously measured the element at constriant
                        //and the fact that we are arranging the measure-dirty element now means
                        //we are not in the UpdateLayout loop but rather in manual sequence of Measure/Arrange
                        //(like in HwndSource when new RootVisual is attached) so there are no loops and there could be
                        //measure-dirty elements left after previous single Measure pass) - so need to use cached constraint
                        Size previousDesiredSizeInArrange = DesiredSize;
                        
                        if (NeverMeasured)
                        {
                            Measure(finalRect.Size);
                        }
                        else
                        {
                            Measure(PreviousAvailableSize);
                        }

                        if (!DoubleUtil.AreClose(previousDesiredSizeInArrange, DesiredSize))
                        {
                            InvalidateParentMeasure();
                            InvalidateParentArrange();
                        }
                    }
                    finally
                    {
                        MeasureDuringArrange = false;
                    }
                }

                //bypass - if clean and rect is the same, no need to re-arrange
                if (!IsArrangeValid || NeverArranged || !DoubleUtil.AreClose(finalRect, PreviousArrangeRect))
                {
                    bool firstArrange = NeverArranged;
                    NeverArranged = false;
                    ArrangeInProgress = true;

                    LayoutManager layoutManager = LayoutManager.Current;

                    Size oldSize = RenderSize;
                    Point oldOffset = VisualOffset;
                    Rect? oldLayoutClip = LayoutClip;
                    bool sizeChanged = false;
                    bool gotException = true;

                    try
                    {
                        layoutManager.EnterArrange();

                        //This has to update RenderSize
                        ArrangeCore(finalRect);

                        //to make sure Clip is tranferred to Visual
                        LayoutClip = GetLayoutClip(finalRect.Size);
                        
                        // see if we need to call OnRenderSizeChanged on this element
                        sizeChanged = MarkForSizeChangedIfNeeded(oldSize, RenderSize);

                        sizeChanged |= !DoubleUtil.AreClose(oldOffset, VisualOffset) 
                                    || !DoubleUtil.AreClose(oldLayoutClip, LayoutClip);

                        gotException = false;
                    }
                    finally
                    {
                        ArrangeInProgress = false;
                        layoutManager.ExitArrange();

                        if (gotException)
                        {
                            // we don't want to reset last exception element on layoutManager if it's been already set.
                            if (layoutManager.GetLastExceptionElement() == null)
                            {
                                layoutManager.SetLastExceptionElement(this);
                            }
                        }
                    }

                    PreviousArrangeRect = finalRect;

                    ArrangeDirty = false;

                    //reset request.
                    if (ArrangeRequest != null)
                    {
                        LayoutManager.Current.ArrangeQueue.Remove(this);
                    }

                    if (sizeChanged || RenderingInvalidated || firstArrange)
                    {
                        // Render with new size & location
                        Render();
                        RenderingInvalidated = false;
                    }
                }
            }
        }

        internal virtual void ArrangeCore(Rect finalRect) { }

        internal virtual Rect? GetLayoutClip(Size layoutSlotSize)
        {
            return null;
        }

        /// <summary>
        /// This is invoked after layout update before rendering if the element's RenderSize
        /// has changed as a result of layout update.
        /// </summary>
        /// <param name="info">
        /// Packaged parameters (<seealso cref="SizeChangedInfo"/>, includes old and new sizes 
        /// and which dimension actually changes.
        /// </param>
        internal virtual void OnRenderSizeChanged(SizeChangedInfo info) { }

        private bool MarkForSizeChangedIfNeeded(Size oldSize, Size newSize)
        {
            //already marked for SizeChanged, simply update the newSize
            bool widthChanged = !DoubleUtil.AreClose(oldSize.Width, newSize.Width);
            bool heightChanged = !DoubleUtil.AreClose(oldSize.Height, newSize.Height);

            SizeChangedInfo info = SizeChangedInfo;

            if (info != null)
            {
                info.Update(widthChanged, heightChanged);
                return true;
            }
            else if (widthChanged || heightChanged)
            {
                info = new SizeChangedInfo(this, oldSize, widthChanged, heightChanged);
                SizeChangedInfo = info;
                LayoutManager.Current.AddToSizeChangedChain(info);

                return true;
            }

            //this result is used to determine if we need to call OnRender after Arrange
            //OnRender is called for 2 reasons - someone called InvalidateVisual - then OnRender is called
            //on next Arrange, or the size changed.
            return false;
        }

        /// <summary>
        /// Ensures that all positions of child objects of a <see cref="UIElement"/> are
        /// properly updated for layout.
        /// </summary>
        public void UpdateLayout()
        {
            LayoutManager.Current.UpdateLayout();
        }

        internal void InvalidateParentMeasure() => GetLayoutParent(this)?.InvalidateMeasure();

        internal void InvalidateParentArrange() => GetLayoutParent(this)?.InvalidateArrange();

        internal static UIElement GetLayoutParent(UIElement element)
            => VisualTreeHelper.GetParent(element) switch
            {
                PopupRoot => null,
                UIElement uie => uie,
                null when element is FrameworkElement fe && fe.Parent is Popup popup => popup.PopupRoot?.Content,
                _ => null,
            };

        private void Render()
        {
            if (!BypassLayoutPolicies)
            {
                INTERNAL_HtmlDomManager.SetVisualBounds(
                    INTERNAL_HtmlDomManager.GetDomElementStyleForModification(INTERNAL_OuterDomElement),
                    VisualOffset, RenderSize, LayoutClip);
            }
        }

        private static void ResetLayoutProperties(UIElement e)
        {
            e.TreeLevel = 0;
            e.NeverMeasured = true;
            e.NeverArranged = true;
            e.PreviousArrangeRect = new Rect();
            e.PreviousAvailableSize = new Size();
            e.VisualOffset = new Point();
            e.LayoutClip = null;
            e._desiredSize = new Size();
            e.RenderSize = new Size();
        }

        /// <summary>
        /// Recursively propagates IsLayoutSuspended flag down to the whole v's sub tree.
        /// </summary>
        internal static void PropagateSuspendLayout(UIElement v)
        {
            //the subtree is already suspended - happens when already suspended tree is further disassembled
            //no need to walk down in this case
            if (v.ReadVisualFlag(VisualFlags.IsLayoutSuspended)) return;

            //  Assert that a UIElement has not being
            //  removed from the visual tree while updating layout.
            Debug.Assert(!v.MeasureInProgress && !v.ArrangeInProgress);

            v.WriteVisualFlag(VisualFlags.IsLayoutSuspended, true);

            ResetLayoutProperties(v);
            
            int count = v.VisualChildrenCount;

            for (int i = 0; i < count; i++)
            {
                UIElement cv = v.GetVisualChild(i);
                if (cv != null)
                {
                    PropagateSuspendLayout(cv);
                }
            }
        }

        /// <summary>
        /// Recursively resets IsLayoutSuspended flag on all visuals of the whole v's sub tree.
        /// For UIElements also re-inserts the UIElement into Measure and / or Arrange update queues
        /// if necessary.
        /// </summary>
        internal static void PropagateResumeLayout(UIElement parent, UIElement v)
        {
            //the subtree is already active - happens when new elements are added to the active tree
            //elements are created layout-active so they don't need to be specifically unsuspended
            //no need to walk down in this case
            //if(!v.CheckFlagsAnd(VisualFlags.IsLayoutSuspended)) return;

            //that can be true only on top of recursion, if suspended v is being connected to suspended parent.
            bool parentIsSuspended = parent == null ? false : parent.ReadVisualFlag(VisualFlags.IsLayoutSuspended);
            uint parentTreeLevel = parent == null ? 0 : parent.TreeLevel;

            if (parentIsSuspended) return;

            v.WriteVisualFlag(VisualFlags.IsLayoutSuspended, false);
            v.TreeLevel = parentTreeLevel + 1;

            //  re-insert UIElement into the update queues

            Debug.Assert(!v.MeasureInProgress && !v.ArrangeInProgress);

            bool requireMeasureUpdate = v.MeasureDirty && !v.NeverMeasured && (v.MeasureRequest == null);
            bool requireArrangeUpdate = v.ArrangeDirty && !v.NeverArranged && (v.ArrangeRequest == null);

            LayoutManager contextLayoutManager = LayoutManager.Current;

            if (requireMeasureUpdate)
            {
                contextLayoutManager.MeasureQueue.Add(v);
            }

            if (requireArrangeUpdate)
            {
                contextLayoutManager.ArrangeQueue.Add(v);
            }

            int count = v.VisualChildrenCount;

            for (int i = 0; i < count; i++)
            {
                UIElement cv = v.GetVisualChild(i);
                if (cv != null)
                {
                    PropagateResumeLayout(v, cv);
                }
            }
        }

        internal Point VisualOffset { get; set; }

        internal Rect? LayoutClip { get; private set; }

        internal Rect PreviousArrangeRect { get; private set; }

        internal Size PreviousAvailableSize { get; private set; }

        internal LayoutManager.LayoutQueue.Request MeasureRequest;
        internal LayoutManager.LayoutQueue.Request ArrangeRequest;

        internal SizeChangedInfo SizeChangedInfo;

        internal bool IsMeasureValid => !MeasureDirty;

        internal bool IsArrangeValid => !ArrangeDirty;

        private bool RenderingInvalidated
        {
            get { return ReadFlag(CoreFlags.RenderingInvalidated); }
            set { WriteFlag(CoreFlags.RenderingInvalidated, value); }
        }

        internal bool ClipToBoundsCache
        {
            get { return ReadFlag(CoreFlags.ClipToBoundsCache); }
            set { WriteFlag(CoreFlags.ClipToBoundsCache, value); }
        }

        internal bool MeasureDirty
        {
            get { return ReadFlag(CoreFlags.MeasureDirty); }
            set { WriteFlag(CoreFlags.MeasureDirty, value); }
        }

        internal bool ArrangeDirty
        {
            get { return ReadFlag(CoreFlags.ArrangeDirty); }
            set { WriteFlag(CoreFlags.ArrangeDirty, value); }
        }

        internal bool MeasureInProgress
        {
            get { return ReadFlag(CoreFlags.MeasureInProgress); }
            set { WriteFlag(CoreFlags.MeasureInProgress, value); }
        }

        internal bool ArrangeInProgress
        {
            get { return ReadFlag(CoreFlags.ArrangeInProgress); }
            set { WriteFlag(CoreFlags.ArrangeInProgress, value); }
        }

        internal bool NeverMeasured
        {
            get { return ReadFlag(CoreFlags.NeverMeasured); }
            set { WriteFlag(CoreFlags.NeverMeasured, value); }
        }

        internal bool NeverArranged
        {
            get { return ReadFlag(CoreFlags.NeverArranged); }
            set { WriteFlag(CoreFlags.NeverArranged, value); }
        }

        internal bool MeasureDuringArrange
        {
            get { return ReadFlag(CoreFlags.MeasureDuringArrange); }
            set { WriteFlag(CoreFlags.MeasureDuringArrange, value); }
        }

        internal bool BypassLayoutPolicies
        {
            get { return ReadFlag(CoreFlags.BypassLayoutPolicies); }
            set { WriteFlag(CoreFlags.BypassLayoutPolicies, value); }
        }

        internal uint TreeLevel { get; set; }
    }
}
